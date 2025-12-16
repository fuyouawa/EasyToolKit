using System.Linq;
using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides extension methods for Unity's Animator component
    /// to simplify parameter validation and management.
    /// </summary>
    public static class AnimatorExtensions
    {
        /// <summary>
        /// Checks if the Animator has a parameter with the specified name.
        /// This is useful for safely setting parameters without causing runtime errors.
        /// </summary>
        /// <param name="animator">The Animator component to check</param>
        /// <param name="name">The name of the parameter to check for</param>
        /// <returns>True if the Animator has a parameter with the specified name, false otherwise</returns>
        public static bool HasParam(this Animator animator, string name)
        {
            return animator.parameters.FirstOrDefault(x => x.name == name) != null;
        }

        /// <summary>
        /// Checks if the Animator has a parameter with the specified name and type.
        /// This provides type-safe parameter validation for Animator operations.
        /// </summary>
        /// <param name="animator">The Animator component to check</param>
        /// <param name="name">The name of the parameter to check for</param>
        /// <param name="typeCheck">The expected parameter type (Float, Int, Bool, or Trigger)</param>
        /// <returns>True if the Animator has a parameter with the specified name and type, false otherwise</returns>
        public static bool HasParam(this Animator animator, string name, AnimatorControllerParameterType typeCheck)
        {
            return animator.parameters.FirstOrDefault(x => x.name == name && x.type == typeCheck) != null;
        }
    }
}
