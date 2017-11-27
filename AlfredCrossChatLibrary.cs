using Mixer.Base;
using NLog;
using System;
using TwitchLib;
using TwitchLib.Models.Client;

namespace AlfredCrossChatLibrary
{
    public class AlfredCrossChatLibrary
    {
        private MixerConnection mixer;
        private TwitchClient twitchClient;
        private static Logger logger;

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
    }
}
