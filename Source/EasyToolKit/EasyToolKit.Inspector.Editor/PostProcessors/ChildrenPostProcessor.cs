namespace EasyToolKit.Inspector.Editor
{
    [PostProcessorPriority(PostProcessorPriorityLevel.Lowest)]
    public class ChildrenPostProcessor : PostProcessor
    {
        protected override void Process()
        {
            if (Element.Children != null)
            {
                foreach (var child in Element.Children)
                {
                    child.PostProcess();
                }
            }
        }
    }
}
