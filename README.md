# FtpLibDotNet #
FTP client library

## Basic usage ##
```csharp
FtpClient client = new FtpClient(HOST, USER, PASSWORD);
client.Open();
client.DownloadFile("readme.txt",@"c:\temp\readme.txt");
client.Close();
```
