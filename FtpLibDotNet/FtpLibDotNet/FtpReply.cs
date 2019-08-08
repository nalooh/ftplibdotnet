namespace FtpLibDotNet
{
    internal class FtpReply
    {
        public FtpReply(string replyText)
        {
            Reply = replyText;
        }

        public string Reply { get; }

        public string Message { get { return Reply.Substring(4); } }

        public int ReplyCodeNumber { get { return System.Int32.Parse(Reply.Substring(0, 3)); } }

        public FtpReplyCode ReplyCode { get { return (FtpReplyCode)ReplyCodeNumber; } }
    }
}
