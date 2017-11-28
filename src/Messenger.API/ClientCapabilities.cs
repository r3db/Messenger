using System;

namespace Messenger.API
{
    [Flags]
    public enum ClientCapabilities : ulong
    {
        None = 0x0,
        MobileDevice = 0x00000001,
        MSNExplorer8 = 0x00000002,
        InkAsGif = 0x00000004,
        InkAsIsf = 0x00000008,
        WebCam = 0x00000010,
        MultiPacketMessaging = 0x00000020,
        MsnMobileDevice = 0x00000040,
        MsnDirectDevice = 0x00000080,
        WebMessenger = 0x00000200,
        InternalOrOfficeLive = 0x00000800,
        MsnSpace = 0x00001000,
        XPMediaCenter = 0x00002000,
        DirectIM = 0x00004000,
        Winks = 0x00008000,
        Search = 0x00010000,
        VoiceClips = 0x00040000,
        SecureChannel = 0x00080000,
        SIPInvitations = 0x00100000,
        SharingFolders = 0x00400000,
        MSNC1 = 0x10000000,
        MSNC2 = 0x20000000,
        MSNC3 = 0x30000000,
        MSNC4 = 0x40000000,
        MSNC5 = 0x50000000,
        MSNC6 = 0x60000000,
        MSNC7 = 0x70000000,
        MSNC8 = 0x80000000
    }
}
