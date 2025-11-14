using System.IO;
using EasyToolKit.Core.Internal;
using UnityEditor;

namespace EasyToolKit.Core.Editor.Internal
{
    public static class EditorAssetPaths
    {
        public static readonly string TemporaryDirectory;

        public static string GetModuleEditorDirectory(string moduleName)
        {
            return $"Assets/{AssetPaths.GetModuleAssetDirectory(moduleName)}/Editor";
        }

        public static string GetModuleTemporaryDirectory(string moduleName)
        {
            return $"{TemporaryDirectory}/{moduleName}";
        }

        public static string GetModuleConfigsDirectory(string moduleName)
        {
            return $"{GetModuleEditorDirectory(moduleName)}/Configs";
        }

        static EditorAssetPaths()
        {
            TemporaryDirectory = EasyPath.FromPath(Path.GetTempPath())
                .AddName(PlayerSettings.companyName)
                .AddName(PlayerSettings.productName)
                .AddName("EasyToolKit")
                .ToString();
        }
    }

    public class ModuleEditorConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ModuleEditorConfigsPathAttribute(string moduleName) : base(
            EditorAssetPaths.GetModuleConfigsDirectory(moduleName))
        {
        }
    }
}
