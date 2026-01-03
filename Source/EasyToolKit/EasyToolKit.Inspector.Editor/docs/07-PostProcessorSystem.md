# 后处理器系统 (PostProcessor System)

## 概述

后处理器系统是 EasyToolKit.Inspector.Editor 的树结构修改系统，负责在元素创建后执行后处理逻辑。通过后处理器，可以动态地修改元素树结构，如创建分组元素、调整元素顺序等。

## 系统架构

```
IPostProcessorChainResolver
    ↓
PostProcessorChain (责任链)
    ↓
IPostProcessor[] (后处理器数组)
    ↓
Element.PostProcessIfNeeded()
```

## IPostProcessor 接口

**位置**: [Core/PostProcessor/Abstractions/IPostProcessor.cs](../Core/PostProcessor/Abstractions/IPostProcessor.cs)

### 接口定义

```csharp
public interface IPostProcessor : IHandler
{
    PostProcessorChain Chain { get; set; }
    void Process();
}
```

### 核心属性

- **Chain**: 后处理器所属的责任链
- **Element**: 通过 IHandler 继承，获取当前处理的元素

## PostProcessor 抽象基类

**位置**: [PostProcessors/PostProcessor.cs](../PostProcessors/PostProcessor.cs)

### 类定义

```csharp
public abstract class PostProcessor : IPostProcessor
{
    public IElement Element { get; }
    public PostProcessorChain Chain { get; set; }

    protected abstract void Process();

    protected void CallNextProcessor()
    {
        if (_chain.MoveNext() && _chain.Current != null)
        {
            _chain.Current.Process();
        }
    }
}
```

### 核心方法

#### Process

后处理逻辑，由子类实现：

```csharp
protected abstract void Process();
```

#### CallNextProcessor

调用责任链中的下一个后处理器：

```csharp
protected void CallNextProcessor()
{
    if (_chain.MoveNext() && _chain.Current != null)
    {
        _chain.Current.Process();
    }
}
```

## PostProcessorChain 责任链

**位置**: [Core/PostProcessor/Models/PostProcessorChain.cs](../Core/PostProcessor/Models/PostProcessorChain.cs)

### 类定义

```csharp
public class PostProcessorChain : IEnumerable<IPostProcessor>, IEnumerator<IPostProcessor>
{
    public IPostProcessor Current { get; }
    public IPostProcessor[] PostProcessors { get; }
    public IElement Element { get; }

    public bool MoveNext() { }
    public void Reset() { }
    public IEnumerator<IPostProcessor> GetEnumerator() { }
}
```

### 特性

1. **同时实现 IEnumerable 和 IEnumerator**
2. **支持后处理器链式调用**
3. **按优先级排序**

## 后处理器优先级

使用 `PostProcessorPriorityAttribute` 控制后处理器顺序：

```csharp
public enum PostProcessorPriorityLevel
{
    VeryHigh = 0,
    High = 100,
    Medium = 200,
    Low = 300,
    VeryLow = 400,
    Super = int.MaxValue - 1  // 保留给特殊用途
}
```

### PostProcessorPriorityAttribute

**位置**: [Core/PostProcessor/Models/PostProcessorPriorityAttribute.cs](../Core/PostProcessor/Models/PostProcessorPriorityAttribute.cs)

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PostProcessorPriorityAttribute : Attribute, IPriorityAccessor
{
    public PostProcessorPriorityLevel Priority { get; }
}
```

### 优先级排序

优先级值越小，优先级越高，越早被调用。

## IPostProcessorChainResolver 接口

**位置**: [Core/Resolvers/Abstractions/IPostProcessorChainResolver.cs](../Core/Resolvers/Abstractions/IPostProcessorChainResolver.cs)

### 接口定义

```csharp
public interface IPostProcessorChainResolver : IResolver
{
    PostProcessorChain GetPostProcessorChain();
}
```

## 后处理流程

```
Element.Update()
    ↓
Element.PostProcessIfNeeded()
    ↓
    ├─→ 检查 Phases 是否包含 PendingPostProcess
    │
    └─→ PostProcessorChainResolver.GetPostProcessorChain()
         ↓
         ├─→ 查找所有适用的 PostProcessor
         └─→ 按 PostProcessorPriority 排序
              ↓
         返回 PostProcessorChain
              ↓
         PostProcessorChain.GetEnumerator()
              ↓
         第一个 PostProcessor.Process()
              ├─→ 自定义后处理逻辑
              └─→ CallNextProcessor()
                   ↓
              第二个 PostProcessor.Process()
                   ├─→ 自定义后处理逻辑
                   └─→ CallNextProcessor()
                        ↓
                   ...
```

## GroupElementPostProcessor - 分组元素后处理器

**位置**: [PostProcessors/GroupElementPostProcessor.cs](../PostProcessors/GroupElementPostProcessor.cs)

### 职责

1. **检测分组属性** - 查找带 BeginGroupAttribute 的元素
2. **创建分组元素** - 创建 GroupElement
3. **移动子元素** - 将相关子元素移动到分组中

### 分组属性

- **BeginGroupAttribute** - 开始分组（如 `[FoldoutGroup]`）
- **EndGroupAttribute** - 结束分组（如 `[EndFoldoutGroup]`）

### 工作流程

```
Element.Children (包含带 BeginGroupAttribute 的元素)
    ↓
GroupElementPostProcessor.Process()
    ↓
    ├─→ 查找下一个带 BeginGroupAttribute 的元素
    │
    ├─→ 创建 GroupElement
    │    └─→ InspectorElements.Configurator.Group()
    │         └─→ ElementFactory.CreateGroupElement()
    │
    ├─→ 查找分组范围内的子元素
    │    ├─→ 遇到 EndGroupAttribute → 结束
    │    └─→ 遇到同级 BeginGroupAttribute → 结束
    │
    └─→ 将子元素移动到 GroupElement.Children
         └─→ Element.Request() → 安全修改树结构
```

### 关键逻辑

#### 1. 查找分组元素

```csharp
private bool TryFindNextElement(ref int elementIndex, out ElementAttributeInfo beginGroupAttributeInfo)
{
    for (; elementIndex < Element.Children.Count; elementIndex++)
    {
        var child = Element.Children[elementIndex];
        foreach (var attributeInfo in child.GetAttributeInfos())
        {
            if (attributeInfo.Attribute.GetType().IsInheritsFrom<BeginGroupAttribute>())
            {
                // 检查父分组是否已处理此属性（防止无限递归）
                if (IsAttributeProcessedInParentGroups(attributeInfo))
                {
                    continue;
                }

                beginGroupAttributeInfo = attributeInfo;
                return true;
            }
        }
    }
    return false;
}
```

#### 2. 防止无限递归

当元素有多个 GroupAttribute 时，需要检查父分组是否已处理：

```csharp
private bool IsAttributeProcessedInParentGroups(ElementAttributeInfo attributeInfo)
{
    var attribute = attributeInfo.Attribute;

    // 缓存结果以避免重复的父遍历
    if (_processedAttributeCache != null && _processedAttributeCache.TryGetValue(attribute, out var cachedResult))
    {
        return cachedResult;
    }

    // 向上遍历父链
    var current = Element;
    bool found = false;
    while (current is IGroupElement groupElement)
    {
        if (groupElement.TryGetAttributeInfo(attribute, out _))
        {
            found = true;
            break;
        }
        current = groupElement.Parent;
    }

    _processedAttributeCache ??= new Dictionary<Attribute, bool>();
    _processedAttributeCache[attribute] = found;
    return found;
}
```

### 优先级

```csharp
[PostProcessorPriority(PostProcessorPriorityLevel.Super - 1)]
public class GroupElementPostProcessor : PostProcessor
```

极高的优先级，确保在其他后处理器之前执行。

## LogicalElementPostProcessor - 逻辑元素后处理器

**位置**: [PostProcessors/LogicalElementPostProcessor.cs](../PostProcessors/LogicalElementPostProcessor.cs)

### 职责

1. **处理逻辑元素** - 为 LogicalElement 执行后处理
2. **创建 LogicalChildren** - 通过 StructureResolver 创建子元素
3. **初始化子元素** - 递归初始化子元素树

### 工作流程

```
LogicalElement.PostProcessIfNeeded()
    ↓
LogicalElementPostProcessor.Process()
    ↓
    ├─→ StructureResolver.GetChildrenDefinitions()
    │
    ├─→ ElementFactory.CreateElements()
    │
    └─→ 递归初始化子元素
         └─→ child.Update()
```

## 创建自定义后处理器

### 示例：自定义后处理器

```csharp
[PostProcessorPriority(PostProcessorPriorityLevel.Medium)]
public class MyCustomPostProcessor : PostProcessor
{
    protected override void Process()
    {
        if (Element.Children == null)
        {
            CallNextProcessor();
            return;
        }

        // 自定义后处理逻辑
        // 例如：重新排序子元素、添加自定义子元素等

        // 调用下一个后处理器
        CallNextProcessor();
    }
}
```

### 注意事项

1. **使用 Request 修改树结构** - 避免在更新过程中直接修改
2. **检查 Children 是否为 null** - 不是所有元素都有子元素
3. **调用 CallNextProcessor** - 确保后处理器链继续执行

## Request 机制

后处理器通常需要修改树结构，必须使用 Request 机制：

```csharp
Element.Request(() =>
{
    // 安全地修改树结构
    Element.Children.Insert(index, newElement);
});
```

### Request 的工作原理

```
Element.Request(action)
    ↓
    ├─→ 检查是否正在绘制
    │    ├─→ 是 → 队列化操作，绘制完成后执行
    │    └─→ 否 → 立即执行
    │
    └─→ 执行 action
```

## 设计模式

### 责任链模式 (Chain of Responsibility)

- PostProcessorChain 是责任链模式的典型应用
- 每个后处理器可以处理请求或传递给下一个
- 灵活的处理顺序

### 策略模式 (Strategy Pattern)

- 每个后处理器是一种后处理策略
- 运行时选择合适的后处理器
- 通过优先级控制策略选择

## 性能优化

### 1. 缓存机制

GroupElementPostProcessor 使用缓存避免重复的父遍历：

```csharp
private Dictionary<Attribute, bool> _processedAttributeCache;
```

### 2. 早期退出

如果不需要处理，立即调用下一个后处理器：

```csharp
if (Element.Children == null)
{
    CallNextProcessor();
    return;
}
```

### 3. 延迟执行

使用 Request 机制延迟树结构修改，避免频繁的结构重组。

## 相关文档

- [解析器系统](./05-ResolverSystem.md) - PostProcessorChainResolver 的解析
- [元素系统](./02-ElementSystem.md) - IGroupElement 和 LogicalElement 的设计
- [元素树系统](./03-ElementTree.md) - Request 机制的工作原理

---

最后更新: 2026-01-03
