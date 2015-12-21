using Magpie.Interfaces;

namespace Magpie.Services
{
    public class AnalyticsLogger : IAnalyticsLogger
    {
        public virtual void LogDownloadNow()
        {
        }

        public virtual void LogSkipThisVersion()
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
    }
}