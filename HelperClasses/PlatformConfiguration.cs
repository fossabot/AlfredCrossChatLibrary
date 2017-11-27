namespace AlfredCrossChatLibrary
{
    public class PlatformConfiguration
    {
        public Platform platform;
        public string UserID;
        public string UserSecret;
    }

    public enum Platform
    {
        TWITCH = 0,
        MIXER = 1
    }
}