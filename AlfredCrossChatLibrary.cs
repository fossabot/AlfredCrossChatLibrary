using Mixer.Base;
using NLog;
using System;
using TwitchLib;
using TwitchLib.Exceptions.API.UploadVideo.CreateVideo;
using TwitchLib.Models.Client;

namespace AlfredCrossChatLibrary
{
    public class AlfredCrossChatLibrary
    {
        #region Connection Objects
        private MixerConnection mixer;
        private TwitchClient twitchClient;
        private static Logger logger;
        #endregion

        #region Events
        public EventHandler<OnChatReceivedArgs> OnChatReceived;
        public EventHandler<OnBanReceivedArgs> OnBanReceived;
        public EventHandler<OnStreamStartedArgs> OnStreamStarted;
        public EventHandler<OnStreamEndedArgs> OnStreamEnded;
        public EventHandler<OnCOmmandReceivedArgs> OnCommandReceived;
        #endregion

        /// <summary>
        /// Configure or reconfigure multiple platforms. This will disconnect all reconfigured platforms.
        /// Ignores all exceptions.
        /// </summary>
        /// <param name="configurations">Data for reconfiguring.</param>
        public void Configure(PlatformConfiguration[] configurations, bool ignoreExceptions = false)
        {

            logger.Info(string.Format("Configuring for {0} platforms.", configurations.Length));
            foreach (var config in configurations)
            {
                try
                {
                    this.Configure(config);
                }
                catch (Exception ex)
                {
                    logger.Error("Exception reached. " + ex.Message);
                    logger.Debug(ex.StackTrace);
                    if (ignoreExceptions)
                        continue;
                    else
                        throw ex;
                }
            }
        }
        /// <summary>
        /// Forcefully disconnects the client being configured and (re)configures it
        /// </summary>
        /// <param name="config"></param>
        private void Configure(PlatformConfiguration config)
        {
            switch (config.platform)
            {
                case Platform.TWITCH:
                    logger.Info(string.Format("Configuring Twitch with user {0}", config.UserID));
                    if (twitchClient != null && twitchClient.IsConnected)
                    {
                        twitchClient.Disconnect();
                    }
                    if (twitchClient != null)
                    {
                        twitchClient = null;
                    }
                    ConnectionCredentials twitchCreds = new ConnectionCredentials(config.UserID, config.UserSecret);
                    twitchClient = new TwitchClient(twitchCreds);
                    twitchClient.Connect();
                    logger.Info("Connected to twitch with user " + config.UserID);
                    break;
                case Platform.MIXER:
                    logger.Info("Configuring Mixer with user " + config.UserID);
                    throw new NotImplementedException();
                    break;
                default:
                    logger.Error("Invalid platform found. Throwing exception.");
                    throw new ChatConfigurationException("Invalid platform provided. Can not configure.");
            }
        }

        public void SendMessage(Platform platform, string channelName, string Message)
        {

            if (!IsConfigured(platform))
                throw new ChatConfigurationException("Platform is not configured.");

            if (!IsConnected(platform))
                throw new NotConnectedException("Platform is not connected.");

            if (!IsPresentInChannel(platform, channelName))
                throw new NotConnectedException("Not present in channel");

            switch (platform)
            {
                case Platform.TWITCH:
                    JoinedChannel joinedChannel = twitchClient.GetJoinedChannel(channelName);
                    twitchClient.SendMessage(joinedChannel, Message);
                    break;
                case Platform.MIXER:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new InvalidPlatformException("Can not send a message to " + platform);
            }
        }

        private bool IsPresentInChannel(Platform platform, string channelName)
        {
            if (!IsConnected(platform))
                return false;

            switch (platform)
            {
                case Platform.TWITCH:
                    JoinedChannel channel = twitchClient.GetJoinedChannel(channelName);
                    if (channel != null)
                        return true;
                    return false;
                case Platform.MIXER:
                    throw new NotImplementedException();
                default:
                    throw new InvalidPlatformException();
            }
        }

        public bool IsConnected(Platform platform)
        {
            if (!IsConfigured(platform))
                return false;
            switch (platform)
            {
                case Platform.TWITCH:
                    return twitchClient.IsConnected;
                case Platform.MIXER:
                    throw new NotImplementedException();
                default:
                    throw new InvalidPlatformException();
            }
        }
        public bool IsConfigured(Platform platform)
        {
            switch (platform)
            {
                case Platform.TWITCH:
                    if (twitchClient == null)
                        return false;
                    return true;
                case Platform.MIXER:
                    if (mixer == null)
                        return false;
                    return true;
                default:
                    throw new InvalidPlatformException();
            }
        }

        public bool JoinChat(Platform platform, string channelName)
        {
            if (!IsConnected(platform))
                throw new NotConnectedException(platform + " is not connected.");
            switch (platform)
            {
                case Platform.TWITCH:
                    try
                    {
                        twitchClient.JoinChannel(channelName);
                        return true; //Todo: add joining worked?
                    }
                    catch
                    {
                        return false;
                    }
                case Platform.MIXER:
                    throw new NotImplementedException();
                default:
                    throw new InvalidPlatformException("Can not join a chat on " + platform);
            }
        }

        public bool LeaveChat(Platform platform, string channelName)
        {
            if (!IsPresentInChannel(platform, channelName))
                return true;
            switch (platform)
            {
                case Platform.TWITCH:
                    try
                    {
                        twitchClient.LeaveChannel(channelName);
                        return true;
                    } catch
                    {
                        return false;
                    }
                case Platform.MIXER:
                    throw new NotImplementedException();
                default:
                    throw new InvalidPlatformException("Can not leave a chat on " + platform);
            }
        }
    }
}
