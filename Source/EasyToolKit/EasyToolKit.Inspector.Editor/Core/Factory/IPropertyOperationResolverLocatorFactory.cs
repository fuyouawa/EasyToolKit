namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// 属性操作解析器定位器工厂接口
    /// 负责创建属性操作解析器定位器
    /// </summary>
    public interface IPropertyOperationResolverLocatorFactory
    {
        /// <summary>
        /// 创建解析器定位器
        /// </summary>
        /// <param name="property">检查器属性</param>
        /// <returns>解析器定位器</returns>
        IPropertyOperationResolverLocator CreateLocator(InspectorProperty property);
    }
}
