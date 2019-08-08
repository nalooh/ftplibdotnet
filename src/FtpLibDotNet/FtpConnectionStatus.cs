using System;

namespace FtpLibDotNet
{
    public enum FtpConnectionStatus
    {
        NotConnected,
        Connecting,
        LogingIn,
        Ready,
        Transfering,
        Busy
    }
}
