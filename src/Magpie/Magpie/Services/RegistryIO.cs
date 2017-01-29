using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using Microsoft.Win32;

namespace Magpie.Services
{
    internal class RegistryIO
    {
        private readonly string _magpieRegistryPath;

        internal RegistryIO()
        {
            var accessor = new AssemblyAccessor();
            if (string.IsNullOrWhiteSpace(accessor.Company) || string.IsNullOrWhiteSpace(accessor.Product))
            {
                throw new FormatException(
                    "Your main assembly is missing company and/or product name. Both fields are required.");
            }
            _magpieRegistryPath = string.Format(MagicStrings.REG_ROOT_PATH, accessor.Company, accessor.Product);
        }

        internal virtual string ReadFromRegistry(string key, string defaultValue = "")
        {
            try
            {
                var regEntry = Registry.CurrentUser.OpenSubKey(_magpieRegistryPath);
                return regEntry == null ? defaultValue : regEntry.GetValue(key, defaultValue).ToString();
            }
            catch (SecurityException ex)
            {
                Trace.TraceError(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                Trace.TraceError(ex.Message);
            }
            catch (IOException ex)
            {
                Trace.TraceError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Trace.TraceError(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.TraceError(ex.Message);
            }
            return null;
        }

        internal virtual void WriteToRegistry(string key, string value)
        {
            try
            {
                var regEntry = Registry.CurrentUser.CreateSubKey(_magpieRegistryPath);
                if (regEntry == null) return;
                regEntry.SetValue(key, value);
            }
            catch (SecurityException ex)
            {
                Trace.TraceError(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                Trace.TraceError(ex.Message);
            }
            catch (IOException ex)
            {
                Trace.TraceError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Trace.TraceError(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.TraceError(ex.Message);
            }
        }
    }
}