﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace MagpieUpdater.Models
{
    [DataContract]
    public class Channel
    {
        [DataMember(Name = "id", IsRequired = false)] private int? _id;
        public int Id { get; protected set; }

        [DataMember(Name = "build", IsRequired = false)]
        public string Build { get; private set; }

        [DataMember(Name = "version", IsRequired = true)] private string _version;
        public Version Version { get; protected set; }

        [DataMember(Name = "release_notes_url", IsRequired = true)]
        public string ReleaseNotesUrl { get; private set; }

        [DataMember(Name = "artifact_url", IsRequired = true)]
        public string ArtifactUrl { get; protected set; }

        [DataMember(Name = "dsa_signature", IsRequired = false)]
        public string DSASignature { get; private set; }

        [DataMember(Name = "requires_enrollment", IsRequired = false)]
        public bool RequiresEnrollment { get; private set; }

        [DataMember(Name = "enrollment_eula_url", IsRequired = false)]
        public string EnrollmentEulaUrl { get; private set; }

        // Dates example:
        // e.g. January 30, 2015 18:15:00 +0200
        // 10/03/2015
        // 2015-10-03
        [DataMember(Name = "build_date", IsRequired = false)] private string _buildDate;
        public DateTime BuildDate { get; private set; }

        public Dictionary<string, object> RawDictionary { get; internal set; }

        [OnDeserialized]
        private void SetValuesOnDeserialized(StreamingContext context)
        {
            ParseId();
            ParseVersion();
            ParseBuildDate();
        }

        private void ParseId()
        {
            Id = _id ?? int.MaxValue;
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
            if (DateTime.TryParse(_buildDate, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces,
                out buildDate))
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
            return string.Format("{0} Build: {1} Build Date: {2}", Version, Build, BuildDate);
        }
    }
}