namespace EasyToolKit.Inspector.Editor
{
    public interface IPostProcessor : IHandler
    {
        PostProcessorChain Chain { get; set; }
        void Process();
    }
}
