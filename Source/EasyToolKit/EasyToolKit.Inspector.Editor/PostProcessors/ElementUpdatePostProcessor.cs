namespace EasyToolKit.Inspector.Editor
{
    [ElementPostProcessorPriority(ElementPostProcessorPriorityLevel.Lowest)]
    public class ElementUpdatePostProcessor : ElementPostProcessor
    {
        protected override void Process(IElement element)
        {
            element.Update(forceUpdate: true);
        }
    }
}
