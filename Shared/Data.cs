#define ALT // alternatywne dane

using System;

#if (SRV)
namespace Serwer
#elif (WINDOWS_UWP)
namespace Dom_Background
#elif(ANDROID)
namespace Dom_android
#endif
{
    public static class Data
    {
#if (ALT)
        private static string _serwerIp = "192.168.43.163";
        private static int _serwerPort = 80;
#else
        public static string _serwerIp = "192.168.1.4";
        public static int _serwerPort = 80;
#endif
        public static string SerwerIp { get => _serwerIp; }
        public static int SerwerPort { get => _serwerPort;}
    }
}