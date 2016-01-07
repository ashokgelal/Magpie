using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace Magpie.Models
{
    [DataContract]
    public class RemoteAppcast
    {
        [DataMember(Name = "version", IsRequired = true)]
        private string _version;
        public Version Version { get; protected set; }
        public Dictionary<string, object> RawDictionary { get; internal set; }

        // Dates example:
        // e.g. January 30, 2015 18:15:00 +0200
        // 10/03/2015
        // 2015-10-03
        [DataMember(Name = "build_date", IsRequired = false)]
        private string _buildDate;
        public DateTime BuildDate { get; private set; }

        [DataMember(Name = "artifact_url", IsRequired = true)]
        public string ArtifactUrl { get; private set; }

        [DataMember(Name = "release_notes_url", IsRequired = true)]
        public string ReleaseNotesUrl { get; private set; }

        [DataMember(Name = "title", IsRequired = true)]
        public string Title { get; private set; }

        [OnDeserialized]
        private void SetValuesOnDeserialized(StreamingContext context)
        {
            ParseVersion();
            ParseBuildDate();
        }

        private void ParseVersion()
        {
            Version version;
            if (Version.TryParse(_version, out version))
            {
                Version = version;
            }
            else
            {
                Trace.TraceError("Magpie: Error parsing version");
            }
        }

        private void ParseBuildDate()
        {
            if (string.IsNullOrWhiteSpace(_buildDate)) return;
            DateTime buildDate;
            if (DateTime.TryParse(_buildDate, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out buildDate))
            {
                BuildDate = buildDate;
            }
            else
            {
                Trace.TraceError("Magpie: Error parsing build date");
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}) Build Date: {2}", Title, Version, BuildDate);
        }
    }
}