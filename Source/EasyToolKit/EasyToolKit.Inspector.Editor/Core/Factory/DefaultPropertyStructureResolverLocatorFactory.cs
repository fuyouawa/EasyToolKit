using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// 默认属性结构解析器定位器工厂
    /// 基于优先级的解析器选择系统
    /// </summary>
    public class DefaultPropertyStructureResolverLocatorFactory : IPropertyStructureResolverLocatorFactory
    {
        private static readonly Lazy<List<Type>> _locatorTypes = new Lazy<List<Type>>(LoadLocatorTypes);
        private static readonly Dictionary<Type, double> _locatorPriorities = new Dictionary<Type, double>();

        /// <summary>
        /// 创建解析器定位器
        /// </summary>
        /// <param name="property">检查器属性</param>
        /// <returns>匹配的解析器定位器</returns>
        public IPropertyStructureResolverLocator CreateLocator(InspectorProperty property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            // 按优先级排序的所有定位器类型
            var sortedLocatorTypes = _locatorTypes.Value
                .OrderByDescending(t => GetLocatorPriority(t))
                .ToList();

            // 依次尝试每个定位器
            foreach (var locatorType in sortedLocatorTypes)
            {
                try
                {
                    var locator = (IPropertyStructureResolverLocator)Activator.CreateInstance(locatorType);
                    if (locator.CanResolver(property))
                    {
                        return locator;
                    }
                }
                catch (Exception ex)
                {
                    // 记录错误但继续尝试其他定位器
                    UnityEngine.Debug.LogWarning($"Failed to create locator of type {locatorType.Name}: {ex.Message}");
                }
            }

            // 如果没有找到匹配的定位器，返回默认的通用定位器
            return new GenericPropertyStructureResolverLocator();
        }

        /// <summary>
        /// 加载所有实现 IPropertyStructureResolverLocator 的类型
        /// </summary>
        private static List<Type> LoadLocatorTypes()
        {
            var locatorTypes = new List<Type>();

            // 获取当前程序集
            var currentAssembly = Assembly.GetExecutingAssembly();

            // 查找所有实现 IPropertyStructureResolverLocator 的具体类
            var types = currentAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IPropertyStructureResolverLocator).IsAssignableFrom(t))
                .ToList();

            locatorTypes.AddRange(types);

            // 缓存优先级信息
            foreach (var type in types)
            {
                _locatorPriorities[type] = GetLocatorPriority(type);
            }

            return locatorTypes;
        }

        /// <summary>
        /// 获取定位器的优先级
        /// </summary>
        private static double GetLocatorPriority(Type locatorType)
        {
            if (_locatorPriorities.TryGetValue(locatorType, out var cachedPriority))
                return cachedPriority;

            var priorityAttribute = locatorType.GetCustomAttribute<ResolverLocatorPriorityAttribute>();
            var priority = priorityAttribute?.Priority ?? 0;

            _locatorPriorities[locatorType] = priority;
            return priority;
        }
    }
}
