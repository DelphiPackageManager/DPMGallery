using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace DPMGallery.Types
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CompilerVersion
    {
        UnknownVersion = 0,
        [Description("XE2")]
        RSXE2,
        [Description("XE3")]
        RSXE3,
        [Description("XE4")]
        RSXE4,
        [Description("XE5")]
        RSXE5,
        [Description("XE6")]
        RSXE6,
        [Description("XE7")]
        RSXE7,
        [Description("XE8")]
        RSXE8,
        [Description("10.0")]
        RS10_0,
        [Description("10.1")]
        RS10_1,
        [Description("10.2")]
        RS10_2,
        [Description("10.3")]
        RS10_3,
        [Description("10.4")]
        RS10_4,
        [Description("11.0")]
        RS11_0
    }

    public static class CompilerVersionExtensions
    {
        public static CompilerVersion ToCompilerVersion(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return CompilerVersion.UnknownVersion;
            string theValue = "";
            if (!value.StartsWith("RS", StringComparison.InvariantCultureIgnoreCase))
                theValue = "RS";
            theValue = theValue + value.Replace('.', '_');
            if (Enum.TryParse(theValue, true, out CompilerVersion result))
            {
                return result;
            }
            else
            {
                return CompilerVersion.UnknownVersion;
            }
        }

        public static string Sanitise(this CompilerVersion version)
        {
            return $"{version}".Replace("RS", "").Replace('_', '.');
        }
    }

}
