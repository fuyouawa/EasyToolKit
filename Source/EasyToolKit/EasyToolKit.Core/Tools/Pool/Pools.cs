namespace EasyToolKit.Core
{
    public static class Pools
    {
        public static IPoolManagerFactory ManagerFactory { get; }
        public static IPoolConfigurator Configurator { get; }

        static Pools()
        {
            ManagerFactory = new Implementations.PoolManagerFactory();
            Configurator = new Implementations.PoolConfigurator();
        }
    }
}
