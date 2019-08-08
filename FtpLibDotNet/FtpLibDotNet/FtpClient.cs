using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace FtpLibDotNet
{
    public class FtpClient : IDisposable
    {
        #region Properties

        /// <summary>
        /// Buffer block size (512B)
        /// </summary>
        private const int BLOCK_SIZE = 512;

        /// <summary>
        /// Data buffer
        /// </summary>
        private byte[] buffer = new byte[BLOCK_SIZE];

        /// <summary>
        /// Timer that keeps connection alive
        /// </summary>
        private readonly Timer keepAliveTimer;

        public string RemoteHost { get; }

        private readonly string remoteUserName;

        private readonly string remotePassword;

        private readonly string remotePath;

        private readonly int remotePort;

        private Socket client_socket;

        public bool Connected { get; private set; } = false;

        public FtpConnectionStatus ConnectionStatus { get; private set; } = FtpConnectionStatus.NotConnected;

        #endregion

        #region Constructors

        public FtpClient(string remoteHost, string remoteUser, string remotePassword)
            : this(remoteHost, remoteUser, remotePassword, 21)
        {
        }

        public FtpClient(string remoteHost, string remoteUser, string remotePassword, int remotePort)
        {
            this.RemoteHost = remoteHost;
            this.remoteUserName = remoteUser;
            this.remotePassword = remotePassword;
            this.remotePath = ".";
            this.remotePort = remotePort;
            this.ConnectionClosed = new EventHandler(this.FtpClient_ConnectionClosed);
            this.ConnectionOpened = new EventHandler(this.FtpClient_ConnectionOpened);

            // Construct keep-alive timer
            this.keepAliveTimer = new Timer
            {
                Interval = 120000,
                AutoReset = true,
            };
            this.keepAliveTimer.Elapsed += new ElapsedEventHandler(this.keepAliveTimer_Elapsed);
        }

        #endregion

        #region Events

        public event EventHandler ConnectionClosed;

        public event EventHandler ConnectionOpened;

        private void FtpClient_ConnectionOpened(object sender, EventArgs e)
        {
            this.keepAliveTimer.Start();
        }

        private void FtpClient_ConnectionClosed(object sender, EventArgs e)
        {
            this.keepAliveTimer.Stop();
        }

        private void keepAliveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendCommand(FtpCommands.NOOP());
        }

        #endregion

        #region General Methods

        public void Open()
        {
            ConnectionStatus = FtpConnectionStatus.Connecting;
            client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEP;
            try
            {
                remoteEP = new IPEndPoint(Dns.GetHostEntry(RemoteHost).AddressList[0], remotePort);
            }
            catch (SocketException ex)
            {
                ConnectionStatus = FtpConnectionStatus.NotConnected;
                Connected = false;
                throw new Exception("Can't translate domain name to IP adress", ex);
            }
            try
            {
                this.client_socket.Connect(remoteEP);
            }
            catch (Exception ex)
            {
                ConnectionStatus = FtpConnectionStatus.NotConnected;
                Connected = false;
                throw new Exception("Can't connect to remote server", ex);
            }
            FtpReply reply = null;
            try
            {
                reply = ReadReply();
            }
            catch (Exception ex)
            {
                Close();
                throw new FtpException(FtpReplyCode.OK, reply.Reply, ex);
            }
            if (reply.ReplyCode != FtpReplyCode.ServiceReadyForNewUser)
            {
                Close();
                throw new FtpException(reply);
            }
            ConnectionStatus = FtpConnectionStatus.LogingIn;
            reply = SendCommand(FtpCommands.USER(this.remoteUserName));
            if (reply.ReplyCode != FtpReplyCode.NeedPassword && reply.ReplyCode != FtpReplyCode.UserLoggedIn)
            {
                Close();
                throw new FtpException(reply);
            }
            if (reply.ReplyCode != FtpReplyCode.UserLoggedIn)
            {
                reply = SendCommand(FtpCommands.PASS(this.remotePassword));
                if (reply.ReplyCode != FtpReplyCode.UserLoggedIn && reply.ReplyCode != FtpReplyCode.CommandNotImplementedSuperfluousAtThisSite)
                {
                    Close();
                    throw new FtpException(reply);
                }
            }
            Connected = true;
            ConnectionStatus = FtpConnectionStatus.Busy;
            SendCommand(FtpCommands.CWD(this.remotePath));
            ConnectionStatus = FtpConnectionStatus.Ready;
            ConnectionOpened(this, new EventArgs());
        }

        public void Close()
        {
            // Close socket ff exists
            if (client_socket != null)
            {
                SendCommand(FtpCommands.QUIT());
                if (client_socket.Connected)
                    client_socket.Close();
            }
            client_socket.Dispose();
            client_socket = null;

            // Raise ConnectionClosed event if connected
            if (ConnectionStatus != FtpConnectionStatus.NotConnected)
                ConnectionClosed(this, new EventArgs());

            // Set connection status
            ConnectionStatus = FtpConnectionStatus.NotConnected;
            Connected = false;
        }

        private Socket CreateDataSocket()
        {
            var reply = SendCommand(FtpCommands.PASV(), false);
            if (reply.ReplyCode != FtpReplyCode.EnteringPassiveMode)
                return null;

            int num = reply.Reply.IndexOf('(');
            int num2 = reply.Reply.IndexOf(')');
            string text = reply.Reply.Substring(num + 1, num2 - num - 1);

            string[] addressParts = text.Split(',');

            string hostName = String.Format("{0}.{1}.{2}.{3}", Int32.Parse(addressParts[0]), Int32.Parse(addressParts[1]), Int32.Parse(addressParts[2]), Int32.Parse(addressParts[3]));

            int port = (Int32.Parse(addressParts[4]) << 8) + Int32.Parse(addressParts[5]);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(hostName), port);
            try
            {
                socket.Connect(remoteEP);
            }
            catch (Exception)
            {
                throw new Exception("Can't connect to remote server");
            }
            return socket;
        }

        private FtpReply SendCommand(string command, bool setStatus = true)
        {
            try
            {
                if (setStatus) ConnectionStatus = FtpConnectionStatus.Busy;
                SendData(command);
                FtpReply reply = ReadReply();
                if (setStatus) ConnectionStatus = FtpConnectionStatus.Ready;
                return reply;
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10035 || ex.ErrorCode == 10050 || ex.ErrorCode == 10051 || ex.ErrorCode == 10052 || ex.ErrorCode == 10053 || ex.ErrorCode == 10060 || ex.ErrorCode == 10064)
                {
                    ConnectionStatus = FtpConnectionStatus.NotConnected;
                    client_socket = null;
                    Close();
                }
                else
                    ConnectionStatus = FtpConnectionStatus.Ready;

                throw;
            }
        }

        private void SendData(string Data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes((Data + "\r\n").ToCharArray());
            this.client_socket.Send(bytes, bytes.Length, SocketFlags.None);
        }

        private FtpReply ReadReply()
        {
            return new FtpReply(ReceiveData());
        }

        private string ReceiveData(int level = 0)
        {
            string text = "";
            int num;
            do
            {
                num = this.client_socket.Receive(this.buffer, this.buffer.Length, SocketFlags.None);
                text += Encoding.ASCII.GetString(this.buffer, 0, num);
            }
            while (num > this.buffer.Length);
            string[] array = text.Split('\n');
            if (array.Length > 2)
            {
                text = array[array.Length - 2];
            }
            else
            {
                text = array[0];
            }
            if (text.Length >= 3 && text.Substring(3, 1).Equals(" "))
            {
                return text;
            }
            if (level > 25)
            {
                throw new Exception();
            }
            return ReceiveData(level + 1);
        }

        private void SetBinaryMode(bool isBinary)
        {
            FtpReply reply = null;

            if (isBinary)
                reply = SendCommand(FtpCommands.TYPE("I"));
            else
                reply = SendCommand(FtpCommands.TYPE("A"));

            if (reply.ReplyCode != FtpReplyCode.OK)
                throw new FtpException(reply);
        }

        void IDisposable.Dispose()
        {
            keepAliveTimer.Stop();
            Close();
        }

        public override string ToString()
        {
            return "FTP client .NET v1.1" + (this.Connected ? (" connected to " + this.RemoteHost) : "");
        }

        #endregion

        #region FTP Commands

        public void CreateDirectory(string pathname)
        {
            // Check opened connection
            if (!Connected) throw new FtpNotConnectedException();

            // Send command
            FtpReply reply = SendCommand(FtpCommands.MKD(pathname));

            // Raise exception if wrong response
            if (reply.ReplyCode != FtpReplyCode.PathCreated && reply.ReplyCode != FtpReplyCode.RequestedFileActionCompleted)
                throw new FtpException(reply);
        }

        public void DeleteDirectory(string pathname)
        {
            // Check opened connection
            if (!Connected) throw new FtpNotConnectedException();

            // Send command
            FtpReply reply = SendCommand(FtpCommands.RMD(pathname));

            // Raise exception if wrong response
            if (reply.ReplyCode != FtpReplyCode.PathCreated && reply.ReplyCode != FtpReplyCode.RequestedFileActionCompleted)
                throw new FtpException(reply);
        }

        public void RenameDirectory(string oldPathname, string newPathname)
        {
            // Check opened connection
            if (!Connected) throw new FtpNotConnectedException();

            // Send command rename from
            FtpReply reply = SendCommand(FtpCommands.RNFR(oldPathname));
            if (reply.ReplyCode != FtpReplyCode.RequestedFileActionPendingFurtherInformation)
                throw new FtpException(reply);

            // Send command rename to
            reply = SendCommand(FtpCommands.RNTO(newPathname));
            if (reply.ReplyCode != FtpReplyCode.RequestedFileActionCompleted)
                throw new FtpException(reply);
        }

        public bool DirectoryExists(string pathname)
        {
            return Array.IndexOf(GetFileList(), pathname) > -1;
        }

        public void ChangeWorkingDirectory(string pathname)
        {
            // Check opened connection
            if (!Connected) throw new FtpNotConnectedException();

            // Send command
            var reply = SendCommand(FtpCommands.CWD(pathname));

            // Raise exception when wrong reply
            if (reply.ReplyCode != FtpReplyCode.RequestedFileActionCompleted)
                throw new FtpException(reply);
        }

        public void ChangeWorkingDirectoryUp()
        {
            // Check opened connection
            if (!Connected) throw new FtpNotConnectedException();

            // Send command
            var reply = SendCommand(FtpCommands.CDUP());

            // Raise exception when wrong reply
            if (reply.ReplyCode != FtpReplyCode.RequestedFileActionCompleted)
                throw new FtpException(reply);
        }

        public string GetWorkingDirectory()
        {
            // Check opened connection
            if (!Connected) throw new FtpNotConnectedException();

            // Send command
            FtpReply reply = SendCommand(FtpCommands.PWD());

            // Raise exception when wrong reply
            if (reply.ReplyCode != FtpReplyCode.PathCreated)
                throw new FtpException(reply);

            // Return directory name
            int q1 = reply.Message.IndexOf('"');
            int q2 = reply.Message.Substring(q1+1).IndexOf('"');
            return reply.Message.Substring(q1 + 1, q2);
        }

        public string[] GetFileList()
        {
            return GetFileList(".");
        }

        public string[] GetFileList(string pathname)
        {
            // Check opened connection
            if (!Connected) throw new FtpNotConnectedException();

            // Set connection status to Busy
            this.ConnectionStatus = FtpConnectionStatus.Busy;

            // Create data socket
            Socket socket = CreateDataSocket();

            // Send command
            FtpReply reply = SendCommand(FtpCommands.NLST(pathname), false);

            // Raise exception if wrong reply
            if (reply.ReplyCode != FtpReplyCode.DataConnectionAlreadyOpenTransferStarting && reply.ReplyCode != FtpReplyCode.FileStatusOk)
            {
                this.ConnectionStatus = FtpConnectionStatus.Ready;
                throw new FtpException(reply);
            }

            // Set connection status to Transfering
            this.ConnectionStatus = FtpConnectionStatus.Transfering;

            // Read data
            string text = String.Empty;
            int num;
            do
            {
                num = socket.Receive(this.buffer, this.buffer.Length, SocketFlags.None);
                text += Encoding.ASCII.GetString(this.buffer, 0, num);
            }
            while (num > 0);

            // Close data socket
            socket.Close();

            // Set connection status back to Busy
            this.ConnectionStatus = FtpConnectionStatus.Busy;

            // Read end data transfer reply
            reply = ReadReply();

            // Raise exception when wrong reply
            if (reply.ReplyCode != FtpReplyCode.ClosingDataConnection && reply.ReplyCode != FtpReplyCode.RequestedFileActionCompleted)
            {
                this.ConnectionStatus = FtpConnectionStatus.Ready;
                throw new FtpException(reply);
            }

            // Set connection status to Ready
            this.ConnectionStatus = FtpConnectionStatus.Ready;

            // Return list of files
            return text.Split('\n');
        }

        public void UploadFile(string localFilename, string remoteFilename)
        {
            UploadFile(remoteFilename, localFilename, true);
        }

        public void UploadFile(string localFilename, string remoteFilename, bool createDirectoryIfNotExists)
        {
            // Check opened connection
            if (!Connected) throw new FtpNotConnectedException();

            // Set connection status
            this.ConnectionStatus = FtpConnectionStatus.Busy;

            // Create directory if not exists
            if (createDirectoryIfNotExists)
            {
                if (remoteFilename.LastIndexOf('/') != -1)
                {
                    int length = remoteFilename.LastIndexOf('/');
                    string pathname = remoteFilename.Substring(0, length);
                    if (!DirectoryExists(pathname)) CreateDirectory(pathname);
                }
                if (remoteFilename.LastIndexOf('\\') != -1)
                {
                    int length = remoteFilename.LastIndexOf('\\');
                    string pathname2 = remoteFilename.Substring(0, length);
                    if (!DirectoryExists(pathname2)) CreateDirectory(pathname2);
                }
            }

            // Set binary mode
            SetBinaryMode(true);

            // Open file
            FileStream fileStream = new FileStream(localFilename, FileMode.Open);
            // Open data socket
            Socket socket = this.CreateDataSocket();

            // Send command store
            FtpReply reply = SendCommand(FtpCommands.STOR(remoteFilename));
            // Raise exception when wrong reply
            if (reply.ReplyCode != FtpReplyCode.FileStatusOk && reply.ReplyCode != FtpReplyCode.DataConnectionAlreadyOpenTransferStarting)
                throw new FtpException(reply);

            // Send data
            int size;
            while ((size = fileStream.Read(this.buffer, 0, this.buffer.Length)) > 0)
                socket.Send(this.buffer, size, SocketFlags.None);

            // Close file
            fileStream.Close();
            // Close data socket
            if (socket.Connected) socket.Close();

            // Read end of transfer message
            reply = ReadReply();
            // Raise exception when wrong reply
            if (reply.ReplyCode != FtpReplyCode.ClosingDataConnection && reply.ReplyCode != FtpReplyCode.RequestedFileActionCompleted)
                throw new FtpException(reply);
        }

        public void DownloadFile(string remoteFilename, string localFilename)
        {
            DownloadFile(remoteFilename, localFilename, true);
        }

        public void DownloadFile(string remoteFilename, string localFilename, bool createDirectoryIfNotExists)
        {
            // Check opened connection
            if (!Connected) throw new FtpNotConnectedException();

            // Set connection status
            ConnectionStatus = FtpConnectionStatus.Busy;

            // Create local directory if not exists
            FileInfo fileInfo = new FileInfo(localFilename);
            if (createDirectoryIfNotExists)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(fileInfo.DirectoryName);
                if (!directoryInfo.Exists) directoryInfo.Create();
            }

            // Set binary mode
            SetBinaryMode(true);

            // Open data socket
            Socket socket = this.CreateDataSocket();
            // Open file
            FileStream fileStream = new FileStream(localFilename, FileMode.OpenOrCreate);

            // Send command retrieve
            FtpReply reply = SendCommand(FtpCommands.RETR(remoteFilename));
            // Raise exception when wrong reply
            if (reply.ReplyCode != FtpReplyCode.FileStatusOk && reply.ReplyCode != FtpReplyCode.DataConnectionAlreadyOpenTransferStarting && reply.ReplyCode != FtpReplyCode.RestartMarkerReply)
                throw new FtpException(reply);

            // Read data from data socket
            int num;
            do
            {
                num = socket.Receive(this.buffer, this.buffer.Length, SocketFlags.None);
                fileStream.Write(this.buffer, 0, num);
            }
            while (num > 0);

            // Close file
            fileStream.Close();
            // Close data socket
            if (socket.Connected) socket.Close();

            // Read end of transfer message
            reply = ReadReply();
            // Raise exception when wrong reply
            if (reply.ReplyCode != FtpReplyCode.ClosingDataConnection && reply.ReplyCode != FtpReplyCode.RequestedFileActionCompleted)
                throw new FtpException(reply);
        }

        public void DeleteFile(string filename)
        {
            // Check opened connection
            if (!Connected) throw new FtpNotConnectedException();

            // Send command
            FtpReply reply = SendCommand(FtpCommands.DELE(filename));
            //Raise exception when wrong reply
            if (reply.ReplyCode != FtpReplyCode.RequestedFileActionCompleted)
                throw new FtpException(reply);
        }

        public void RenameFile(string oldFilename, string newFilename)
        {
            // Check opened connection
            if (!Connected) throw new FtpNotConnectedException();

            // Send command rename from
            FtpReply reply = SendCommand(FtpCommands.RNFR(oldFilename));
            // Raise exception when wrong reply
            if (reply.ReplyCode != FtpReplyCode.RequestedFileActionPendingFurtherInformation)
                throw new FtpException(reply);

            // Send command rename to
            reply = SendCommand(FtpCommands.RNTO(newFilename));
            //Raise exception when wrong reply
            if (reply.ReplyCode != FtpReplyCode.RequestedFileActionCompleted)
                throw new FtpException(reply);
        }

        public bool FileExists(string filename)
        {
            return Array.IndexOf(GetFileList(), filename) > -1;
        }

        #endregion
    }
}
