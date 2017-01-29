using System.Diagnostics;
using MagpieUpdater.Interfaces;

namespace MagpieUpdater.ViewModels
{
    internal class DebuggingWindowViewModel : IDebuggingInfoLogger
    {
        public void Log(string message)
        {
            Trace.TraceInformation("Magpie: {0}", message);
        }
    }
}