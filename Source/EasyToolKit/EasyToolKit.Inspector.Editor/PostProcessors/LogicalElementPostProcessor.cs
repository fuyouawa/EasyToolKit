namespace EasyToolKit.Inspector.Editor
{
    [PostProcessorPriority(PostProcessorPriorityLevel.Super)]
    public class LogicalElementPostProcessor : PostProcessor
    {
        protected override bool CanProcess(IElement element)
        {
            return element is ILogicalElement;
        }

        protected override void Process()
        {
            var logicalElement = (ILogicalElement)Element;

            if (logicalElement.LogicalChildren != null)
            {
                foreach (var child in logicalElement.LogicalChildren)
                {
                    logicalElement.Children.Add(child);
                }
            }
            CallNextProcessor();
        }
    }
}
