namespace FtpLibDotNet
{
    public class FtpException : System.Exception
    {
        public FtpReplyCode ReplyCode { get; set; }

        internal FtpException(FtpReplyCode code, string message)
            : base(message)
        {
            ReplyCode = code;
        }

        internal FtpException(FtpReply reply)
            : this(reply.ReplyCode, reply.Message)
        {
        }

        internal FtpException(FtpReplyCode code, string message, System.Exception innerException)
            : base(message, innerException)
        {
            ReplyCode = code;
        }
    }
}
