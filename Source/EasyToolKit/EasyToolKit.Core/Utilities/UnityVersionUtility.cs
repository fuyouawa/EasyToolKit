namespace EasyToolKit.Core
{
    public static class UnityVersionUtility
    {
        public static bool IsVersionOrGreater(int major, int minor)
        {
            return ThirdParty.OdinSerializer.Utilities.UnityVersion.IsVersionOrGreater(major, minor);
        }
    }
}
