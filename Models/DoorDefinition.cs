using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Globalization;

namespace DoorTool.Models
{
    public enum DoorType { Single, Double, Sliding, Unknown }

    public class DoorDefinition
    {
        public string   BlockName   { get; }
        public string   DisplayName { get; }
        public ObjectId ObjectId    { get; }
        public DoorType DoorType    { get; }

        public DoorDefinition(string blockName, ObjectId objectId, string prefix)
        {
            BlockName   = blockName;
            ObjectId    = objectId;
            DoorType    = DetectType(blockName);
            DisplayName = BuildDisplayName(blockName, prefix);
        }


        private static DoorType DetectType(string name)
        {
            var u = name.ToUpperInvariant();
            if (u.Contains("SINGLE")) return DoorType.Single;
            if (u.Contains("DOUBLE")) return DoorType.Double;
            if (u.Contains("SLID"))   return DoorType.Sliding;
            return DoorType.Unknown;
        }

        private static string BuildDisplayName(string blockName, string prefix)
        {
            var raw = blockName;
            if (!string.IsNullOrEmpty(prefix) &&
                raw.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                raw = raw.Substring(prefix.Length).TrimStart('_');

            return CultureInfo.CurrentCulture.TextInfo
                              .ToTitleCase(raw.Replace('_', ' ').ToLower());
        }
    }
}