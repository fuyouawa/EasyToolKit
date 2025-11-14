
using System.IO;

namespace EasyToolKit.Core.Internal
{
    public static class AssetPaths
    {
        public static readonly string PluginsDirectory;

        public static string GetModuleAssetDirectory(string moduleName)
        {
            return $"{PluginsDirectory}/{moduleName}/Assets";
        }

        public static string GetModuleResourcesDirectory(string moduleName)
        {
            return $"{GetModuleAssetDirectory(moduleName)}/Resources";
        }

        public static string GetModuleConfigsDirectory(string moduleName)
        {
            return $"{GetModuleResourcesDirectory(moduleName)}/Configs";
        }

        static AssetPaths()
        {
            //TODO support auto detect
            PluginsDirectory = "Plugins/EasyToolKit";
        }
    }

    public class ModuleConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ModuleConfigsPathAttribute(string moduleName) : base(AssetPaths.GetModuleConfigsDirectory(moduleName))
        {
        }
    }
}
