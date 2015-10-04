﻿using System.Diagnostics;
using System.Security.Policy;
using Magpie.Interfaces;

namespace Magpie.ViewModels
{
    internal class DebuggingWindowViewModel : IDebuggingInfoLogger
    {
        public void Log(string message)
        {
            Trace.TraceInformation("Magpie: {0}", message);
        }
    }
}