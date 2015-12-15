namespace Magpie.Interfaces
{
    public interface IAnalyticsLogger
    {
        void LogContinueUpdate();
        void LogSkipThisVersion();
        void LogRemindMeLater();
    }
}