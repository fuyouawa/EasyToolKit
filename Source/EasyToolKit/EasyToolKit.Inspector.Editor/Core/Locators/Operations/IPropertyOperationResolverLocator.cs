namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// 属性操作解析器定位器接口
    /// 负责找到合适的属性操作解析器
    /// </summary>
    public interface IPropertyOperationResolverLocator
    {
        /// <summary>
        /// 检查是否可以定位解析器
        /// </summary>
        /// <param name="property">检查器属性</param>
        /// <returns>如果可以定位返回 true</returns>
        bool CanResolve(InspectorProperty property);

        /// <summary>
        /// 获取属性操作解析器
        /// </summary>
        /// <param name="property">检查器属性</param>
        /// <returns>属性操作解析器</returns>
        IPropertyOperationResolver GetResolver(InspectorProperty property);
    }
}
