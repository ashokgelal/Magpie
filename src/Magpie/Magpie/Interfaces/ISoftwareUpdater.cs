namespace MagpieUpdater.Interfaces
{
    public interface ISoftwareUpdater
    {
        /// <summary>
        /// Retreives the AppCast data and either shows the UpdateWindow or NoUpdateWindow based on comparison with version and BuildDate of host application
        /// </summary>
        /// <param name="appcastUrl">URL for the AppCast file</param>
        /// <param name="showDebuggingWindow">Does nothing unless you provide an overriden implementation</param>
        void CheckInBackground(string appcastUrl, bool showDebuggingWindow);

        /// <summary>
        /// Does exactly the same thing as <see cref="CheckInBackground"/> unless you provide and overriden implementation
        /// </summary>
        /// <param name="appcastUrl">URL for the AppCast file</param>
        /// <param name="showDebuggingWindow">Does nothing unless you provide an overriden implementation</param>
        void ForceCheckInBackground(string appcastUrl, bool showDebuggingWindow);
    }
}