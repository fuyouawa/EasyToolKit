namespace EasyToolKit.Inspector.Editor
{
    public static class PersistentContext
    {
        public static GlobalPersistentContext<TValue> Get<TValue>(string key, TValue defaultValue)
        {
            var context = PersistentContextCache.Instance.GetContext<TValue>(key, out var isNew);
            if (isNew)
            {
                context.Value = defaultValue;
            }

            return context;
        }

        public static LocalPersistentContext<TValue> GetLocal<TValue>(string key, TValue defaultValue)
        {
            return LocalPersistentContext<TValue>.Create(Get(key, defaultValue));
        }
    }
}
