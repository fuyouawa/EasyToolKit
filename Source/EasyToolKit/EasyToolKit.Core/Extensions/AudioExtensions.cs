using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides extension methods for AudioSource to enhance audio playback functionality
    /// in Unity applications.
    /// </summary>
    public static class AudioExtensions
    {
        /// <summary>
        /// Plays the audio source and all AudioSource components found in its children.
        /// This is useful for playing hierarchical audio setups where multiple audio sources
        /// are organized under a parent object.
        /// </summary>
        /// <param name="audioSource">The parent AudioSource component</param>
        /// <example>
        /// <code>
        /// // Plays the parent audio source and all child audio sources
        /// audioSource.PlayWithChildren();
        /// </code>
        /// </example>
        public static void PlayWithChildren(this AudioSource audioSource)
        {
            foreach (var child in audioSource.GetComponentsInChildren<AudioSource>())
            {
                child.Play();
            }
        }
    }
}
