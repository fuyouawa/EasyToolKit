using UnityEngine;

namespace EasyToolKit.Core
{
    public static class AudioExtensions
    {
        public static void PlayWithChildren(this AudioSource audioSource)
        {
            foreach (var child in audioSource.GetComponentsInChildren<AudioSource>())
            {
                child.Play();
            }
        }
    }
}
