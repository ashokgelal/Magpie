namespace Magpie.Interfaces
{
    public interface IDebuggingInfoLogger
    {
        /// <summary>
        /// Logs a string to active logger
        /// </summary>
        /// <param name="message">The string to be logged</param>
        void Log(string message);
    }
}