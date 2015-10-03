using System.Diagnostics;
using Magpie.Interfaces;

namespace Magpie
{
    internal class DebuggingWindowViewModel : IDebuggingInfoLogger
    {
        public void Log(string message)
        {
            Trace.TraceInformation("Magpie: {0}", message);
        }
    }
}