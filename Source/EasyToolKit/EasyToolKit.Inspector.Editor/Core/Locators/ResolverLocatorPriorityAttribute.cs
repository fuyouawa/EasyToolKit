using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// 解析器定位器优先级属性
    /// 参考 DrawerPriorityAttribute 设计
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ResolverLocatorPriorityAttribute : Attribute
    {
        /// <summary>
        /// 优先级值，数值越大优先级越高
        /// </summary>
        public double Priority { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="priority">优先级值，默认为0</param>
        public ResolverLocatorPriorityAttribute(double priority = 0)
        {
            Priority = priority;
        }
    }
}