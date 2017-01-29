using System.IO;
using System.Reflection;

namespace Magpie.Services
{
    internal class AssemblyAccessor
    {
        private readonly Assembly _assembly;

        internal AssemblyAccessor()
        {
            _assembly = Assembly.GetEntryAssembly();
        }

        internal string Title
        {
            get
            {
                var attributes = _assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute) attributes[0];
                    if (!string.IsNullOrEmpty(titleAttribute.Title))
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        internal string Version
        {
            get { return _assembly.GetName().Version.ToString(); }
        }

        internal string Product
        {
            get
            {
                var attributes = _assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyProductAttribute) attributes[0]).Product;
            }
        }

        internal string Company
        {
            get
            {
                var attributes = _assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyCompanyAttribute) attributes[0]).Company;
            }
        }
    }
}