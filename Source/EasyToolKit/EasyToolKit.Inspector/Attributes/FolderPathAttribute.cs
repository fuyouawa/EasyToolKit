using System;

namespace EasyToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class FolderPathAttribute : InspectorAttribute
    {
        public string ParentFolder { get; set; }
        public bool RequireExistingPath { get; set; }

        public FolderPathAttribute(string parentFolder = "")
        {
            ParentFolder = parentFolder;
            RequireExistingPath = false;
        }
    }
}
