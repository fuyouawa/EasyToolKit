using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides extension methods for AppDomain to filter and retrieve specific assembly types
    /// commonly used in Unity development environments.
    /// </summary>
    public static class AppDomainExtensions
    {
        /// <summary>
        /// Retrieves all assemblies that belong to the current Unity project.
        /// These assemblies typically start with "Assembly." prefix.
        /// </summary>
        /// <param name="domain">The AppDomain instance</param>
        /// <returns>An enumerable collection of project assemblies</returns>
        public static IEnumerable<Assembly> GetProjectAssemblies(this AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("Assembly."));
        }

        /// <summary>
        /// Retrieves all assemblies that belong to the Unity Engine runtime.
        /// These assemblies typically start with "UnityEngine." prefix.
        /// </summary>
        /// <param name="domain">The AppDomain instance</param>
        /// <returns>An enumerable collection of Unity Engine assemblies</returns>
        public static IEnumerable<Assembly> GetUnityEngineAssemblies(this AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("UnityEngine."));
        }

        /// <summary>
        /// Retrieves all assemblies that belong to the Unity Editor.
        /// These assemblies typically start with "UnityEditor." prefix.
        /// Note: These assemblies are only available in the Unity Editor environment.
        /// </summary>
        /// <param name="domain">The AppDomain instance</param>
        /// <returns>An enumerable collection of Unity Editor assemblies</returns>
        public static IEnumerable<Assembly> GetUnityEditorAssemblies(this AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("UnityEditor."));
        }

        /// <summary>
        /// Retrieves all assemblies that belong to the Unity framework.
        /// These assemblies typically start with "Unity." prefix.
        /// </summary>
        /// <param name="domain">The AppDomain instance</param>
        /// <returns>An enumerable collection of Unity framework assemblies</returns>
        public static IEnumerable<Assembly> GetUnityAssemblies(this AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("Unity."));
        }

        /// <summary>
        /// Retrieves all assemblies that belong to the .NET System framework.
        /// These assemblies typically start with "System." prefix.
        /// </summary>
        /// <param name="domain">The AppDomain instance</param>
        /// <returns>An enumerable collection of System framework assemblies</returns>
        public static IEnumerable<Assembly> GetSystemAssemblies(this AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("System."));
        }
    }
}
