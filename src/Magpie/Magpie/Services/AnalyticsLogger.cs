using MagpieUpdater.Interfaces;
using MagpieUpdater.Models;

namespace MagpieUpdater.Services
{
    public class AnalyticsLogger : IAnalyticsLogger
    {
        public virtual void LogDownloadNow()
        {
        }

        public virtual void LogUserSkipsUpdate(Channel channel)
        {
        }

        public virtual void LogRemindMeLater()
        {
        }

        public virtual void LogContinueWithInstallation()
        {
        }

        public virtual void LogOldVersion(string oldVersion)
        {
        }

        public virtual void LogNewVersion(string s)
        {
        }

        public virtual void LogAppTitle(string mySuperAwesomeApp)
        {
        }

        public virtual void LogUpdateCancelled()
        {
        }

        public virtual void LogUpdateAvailable(Channel channel)
        {
        }
    }
}