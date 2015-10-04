using System;
using System.Globalization;
using System.Reflection;
using Magpie.Interfaces;
using Magpie.Models;

namespace Magpie.Services
{
    internal class UpdateDecider
    {
        private readonly IDebuggingInfoLogger _logger;
        private readonly RegistryIO _registryIO;

        protected virtual string SkipVersion
        {
            get
            {
                return _registryIO.ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, string.Empty);
            }
        }

        protected virtual bool IsThisFirstRun
        {
            get
            {
                var firstRun =
                    _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, "true").ToLowerInvariant();
                return firstRun.Equals("1") || firstRun.Equals("true");
            }
        }

        protected virtual DateTime LastCheckDate
        {
            get
            {
                var date = _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, DateTime.MinValue.ToString(CultureInfo.InvariantCulture));
                return Convert.ToDateTime(date);
            }
        }

        internal UpdateDecider(IDebuggingInfoLogger debuggingInfoLogger)
        {
            _logger = debuggingInfoLogger;
            _registryIO = new RegistryIO();
        }

        internal bool ShouldUpdate(RemoteAppcast appcast, bool shouldIgnoreRegistryChecks = false)
        {
            if (!shouldIgnoreRegistryChecks)
            {
                _logger.Log("Checking registry entries for updates.");
                if (!CheckRegistry(appcast))
                {
                    _logger.Log("Registry entries says not to tell if there is an update available.");
                    return false;
                }
                _logger.Log("Registry entries says go and tell if there is an update available.");
            }
            else
            {
                _logger.Log("Skipping registry checks. This probably means we are doing forced update checks.");
            }
            UpdateCheckDate();
            var higherVersion = CheckVersion(appcast);
            return higherVersion;
        }

        protected virtual bool CheckRegistry(RemoteAppcast appcast)
        {
            if (IsThisFirstRun)
            {
                EnsureNotFirstRun();
                _logger.Log("App is running for the first time. Skipping further checks.");
                return false;
            }
            _logger.Log("This is not app's first run. Continuing with other registry checks.");

            var skipVersion = SkipVersion;
            if (!string.IsNullOrWhiteSpace(skipVersion))
            {
                Version ver;
                if (Version.TryParse(skipVersion, out ver) && appcast.Version == ver)
                {
                    _logger.Log(string.Format("User has asked to skip version: {0}. Skipping further checks.", ver));
                    return false;
                }
            }
            _logger.Log("Seems like user doesn\'t want to skip this version. Continuing with other registry checks.");

            var lastCheckDate = LastCheckDate;
            if (DateTime.Now.Subtract(lastCheckDate).TotalDays < 1)
            {
                _logger.Log("Last check was done less than a day ago. Skipping further checks");
                return false;
            }
            _logger.Log(string.Format("Last check was done on: {0}", lastCheckDate.ToShortDateString()));
            return true;
        }

        protected virtual bool CheckVersion(RemoteAppcast appcast)
        {
            var curVer = Assembly.GetExecutingAssembly().GetName().Version;
            var isHigherVersionAvailable = appcast.Version.IsHigherThan(curVer);
            _logger.Log(string.Format("Higher version of app is {0}available", isHigherVersionAvailable ? "" : "not "));
            return isHigherVersionAvailable;
        }

        protected virtual void EnsureNotFirstRun()
        {
            _registryIO.WriteToRegistry(MagicStrings.IS_FIRST_RUN, "false");
        }

        protected virtual void UpdateCheckDate()
        {
            _registryIO.WriteToRegistry(MagicStrings.LAST_CHECK_DATE, DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }
    }
}