namespace Magpie.Interfaces
{
    public interface IAnalyticsLogger
    {
        /// <summary>
        /// Log analytics when user chooses to download the new version of the software.
        /// </summary>
        void LogDownloadNow();

        /// <summary>
        /// Log analytics when user chooses to skip this version update.
        /// </summary>
        void LogSkipThisVersion();

        /// <summary>
        /// Log analytics when user asks to remind them about the update later.
        /// </summary>
        void LogRemindMeLater();

        /// <summary>
        /// Log analytics when user has downloaded the new version and chooses to install it.
        /// </summary>
        void LogContinueWithInstallation();

        /// <summary>
        /// Log the old version number of the software.
        /// </summary>
        /// <param name="oldVersion">Old version of the software</param>
        void LogOldVersion(string oldVersion);

        /// <summary>
        /// Log the new version number of the software.
        /// </summary>
        /// <param name="s">New version of the software</param>
        void LogNewVersion(string s);

        /// <summary>
        /// Log the name of the application to be updated.
        /// </summary>
        /// <param name="mySuperAwesomeApp">Application name</param>
        void LogAppTitle(string mySuperAwesomeApp);

        /// <summary>
        /// Log analytics when the user cancels the update.
        /// </summary>
        void LogUpdateCancelled();
    }
}