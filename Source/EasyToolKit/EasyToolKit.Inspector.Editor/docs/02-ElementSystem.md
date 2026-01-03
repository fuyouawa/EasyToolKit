# 元素系统 (Element System)

## 概述

元素系统是 EasyToolKit.Inspector.Editor 的核心，所有 Inspector 中的可见项都是 Element。元素系统采用树形结构，支持动态创建、销毁和重组。

## 核心接口层次

```
IElement (基础元素接口)
├── ILogicalElement (逻辑元素)
│   └── IValueElement (值元素)
│       ├── IPropertyElement (属性元素)
│       ├── IFieldElement (字段元素)
│       ├── IPropertyCollectionElement (属性集合元素)
│       ├── IFieldCollectionElement (字段集合元素)
│       └── IMethodElement (方法元素)
│           └── IMethodParameterElement (方法参数元素)
├── ICollectionItemElement (集合项元素)
├── IGroupElement (分组元素)
└── IRootElement (根元素)
```

## IElement - 基础元素接口

**位置**: [Core/Elements/Abstractions/IElement.cs](../Core/Elements/Abstractions/IElement.cs)

### 核心属性

```csharp
public interface IElement
{
    IElementDefinition Definition { get; }              // 元素定义（元数据）
    IElementSharedContext SharedContext { get; }        // 共享上下文（服务容器）
    IElement Parent { get; }                            // 当前父元素（动态）
    IElementState State { get; }                        // 元素运行时状态
    string Path { get; }                                // 层级路径
    GUIContent Label { get; set; }                      // 显示标签
    ElementPhases Phases { get; }                       // 当前生命周期阶段
    IElementList<IElement> Children { get; }            // 子元素集合
}
```

### 核心方法

```csharp
// 获取自定义属性信息
IReadOnlyList<ElementAttributeInfo> GetAttributeInfos();
bool TryGetAttributeInfo(Attribute attribute, out ElementAttributeInfo attributeInfo);

// 请求执行修改树结构的操作
bool Request(Action action, bool forceDelay = false);

// 发送消息到元素
bool Send(string messageName, object args = null);
TResult Send<TResult>(string messageName, object args = null);

// 请求刷新元素结构
bool RequestRefresh();

// 销毁元素
void Destroy();

// 更新和绘制
void Update(bool forceUpdate = false);
bool PostProcessIfNeeded();
void Draw(GUIContent label);
```

### 设计要点

#### Parent vs LogicalParent
- **Parent**: 动态父元素，随运行时修改而变化
- **LogicalParent**（ILogicalElement）: 静态父元素，由代码结构定义，不可变

#### Children vs LogicalChildren
- **Children**: 可变子元素集合，支持运行时添加/删除
- **LogicalChildren**（ILogicalElement）: 不可变子元素集合，由代码结构决定

## ILogicalElement - 逻辑元素

**位置**: [Core/Elements/Abstractions/ILogicalElement.cs](../Core/Elements/Abstractions/ILogicalElement.cs)

逻辑元素代表代码中定义的 class 或 struct 类型，具有静态的层次结构。

```csharp
public interface ILogicalElement : IElement
{
    ILogicalElement LogicalParent { get; }                              // 静态父元素
    IReadOnlyElementList<ILogicalElement> LogicalChildren { get; }     // 静态子元素
}
```

### LogicalChildren 的解析

LogicalChildren 通过 **StructureResolver** 解析获取：

```
LogicalElement
    ↓
StructureResolver.GetChildrenDefinitions()
    ↓
ElementFactory.CreateElements()
    ↓
LogicalChildren
```

### 相关文件

- **接口**: [Core/Elements/Abstractions/ILogicalElement.cs](../Core/Elements/Abstractions/ILogicalElement.cs)
- **结构解析器基类**: [Resolvers/StructureResolver/ValueStructureResolverBase.cs](../Resolvers/StructureResolver/ValueStructureResolverBase.cs)
- **集合结构解析器**: [Resolvers/StructureResolver/Collection/CollectionStructureResolver.cs](../Resolvers/StructureResolver/Collection/CollectionStructureResolver.cs)

## IValueElement - 值元素

**位置**: [Core/Elements/Abstractions/IValueElement.cs](../Core/Elements/Abstractions/IValueElement.cs)

值元素表示包含具体数据的元素，如字段、属性等。

```csharp
public interface IValueElement : ILogicalElement
{
    new IValueDefinition Definition { get; }

    // 由代码结构定义的静态子元素
    IReadOnlyElementList<ILogicalElement> LogicalChildren { get; }

    // 运行时动态添加的子元素
    IElementList<IElement> Children { get; }

    // 值条目
    IValueEntry BaseValueEntry { get; }    // 基于声明类型
    IValueEntry ValueEntry { get; }        // 基于运行时类型
}
```

### ValueEntry 的创建

```
ValueElement
    ↓
ValueOperationResolver.GetOperation()
    ↓
IValueOperation (实现值读写逻辑)
    ↓
ValueEntry
```

### BaseValueEntry vs ValueEntry

- **BaseValueEntry**: 基于声明的类型（`IValueDefinition.ValueType`）
- **ValueEntry**: 基于运行时类型
  - 如果运行时类型等于声明类型，两者相同
  - 如果运行时类型是派生类型，ValueEntry 是 BaseValueEntry 的类型包装

### 相关文件

- **接口**: [Core/Elements/Abstractions/IValueElement.cs](../Core/Elements/Abstractions/IValueElement.cs)
- **实现**: [Core/Implementations/Elements/ValueElement.cs](../Core/Implementations/Elements/ValueElement.cs)
- **值操作解析器**: [Core/Resolvers/Abstractions/IValueOperationResolver.cs](../Core/Resolvers/Abstractions/IValueOperationResolver.cs)

## 其他专用元素

### ICollectionElement - 集合元素

管理数组、列表、字典等集合类型。

**相关文件**:
- **接口**: [Core/Elements/Abstractions/ICollectionElement.cs](../Core/Elements/Abstractions/ICollectionElement.cs)
- **集合结构解析器**: [Resolvers/StructureResolver/Collection/CollectionStructureResolver.cs](../Resolvers/StructureResolver/Collection/CollectionStructureResolver.cs)

### IMethodElement - 方法元素

代表可在 Inspector 中调用的方法。

**相关文件**:
- **接口**: [Core/Elements/Abstractions/IMethodElement.cs](../Core/Elements/Abstractions/IMethodElement.cs)
- **方法结构解析器**: [Resolvers/StructureResolver/MethodStructureResolverBase.cs](../Resolvers/StructureResolver/MethodStructureResolverBase.cs)

### ICollectionItemElement - 集合项元素

集合中的单个项元素。

**相关文件**:
- **接口**: [Core/Elements/Abstractions/ICollectionItemElement.cs](../Core/Elements/Abstractions/ICollectionItemElement.cs)

### IMethodParameterElement - 方法参数元素

方法元素的参数。

**相关文件**:
- **接口**: [Core/Elements/Abstractions/IMethodParameterElement.cs](../Core/Elements/Abstractions/IMethodParameterElement.cs)

## IGroupElement - 分组元素

**位置**: [Core/Elements/Abstractions/IGroupElement.cs](../Core/Elements/Abstractions/IGroupElement.cs)

分组元素是一个抽象概念，用于组织相关元素的容器，由 **PostProcessor** 创建。

```csharp
public interface IGroupElement : IElement
{
    new IGroupDefinition Definition { get; }
    ILogicalElement AssociatedElement { get; set; }
}
```

### GroupElement 的创建

```
Element.Children (包含带 BeginGroupAttribute 的元素)
    ↓
GroupElementPostProcessor.Process()
    ↓
创建 GroupElement
    ↓
将相关子元素移动到 GroupElement.Children
```

### 相关文件

- **接口**: [Core/Elements/Abstractions/IGroupElement.cs](../Core/Elements/Abstractions/IGroupElement.cs)
- **后处理器**: [PostProcessors/GroupElementPostProcessor.cs](../PostProcessors/GroupElementPostProcessor.cs)
- **后处理器文档**: [07-PostProcessorSystem.md](./07-PostProcessorSystem.md)

## IRootElement - 根元素

元素树的根节点，特殊的值元素，代表整个被检查对象。

**相关文件**:
- **接口**: [Core/Elements/Abstractions/IRootElement.cs](../Core/Elements/Abstractions/IRootElement.cs)
- **定义**: [Core/Elements/Definitions/IRootDefinition.cs](../Core/Elements/Definitions/IRootDefinition.cs)

## 元素角色 (ElementRoles)

使用标志位枚举定义元素的角色类型：

```csharp
[Flags]
public enum ElementRoles
{
    None = 0,
    Root = 1 << 0,           // 根元素
    Value = 1 << 1,          // 值元素
    Field = 1 << 2,          // 字段元素
    Property = 1 << 3,       // 属性元素
    Collection = 1 << 4,     // 集合元素
    CollectionItem = 1 << 5, // 集合项元素
    Method = 1 << 6,         // 方法元素
    Group = 1 << 7,          // 分组元素
    MethodParameter = 1 << 8 // 方法参数元素
}
```

**相关文件**: [Core/Elements/Models/ElementRoles.cs](../Core/Elements/Models/ElementRoles.cs)

## 元素生命周期 (ElementPhases)

使用标志位枚举定义元素的生命周期阶段：

```csharp
[Flags]
public enum ElementPhases
{
    None = 0,
    Drawing = 1 << 0,           // 正在绘制
    PendingRefresh = 1 << 1,    // 等待刷新
    Refreshing = 1 << 2,        // 正在刷新
    JustRefreshed = 1 << 3,     // 刚完成刷新
    Updating = 1 << 4,          // 正在更新
    PendingPostProcess = 1 << 5,// 等待后处理
    PostProcessing = 1 << 6,    // 正在后处理
    PendingDestroy = 1 << 7,    // 等待销毁
    Destroying = 1 << 8,        // 正在销毁
    Destroyed = 1 << 9,         // 已销毁
}
```

**相关文件**: [Core/Elements/Models/ElementPhases.cs](../Core/Elements/Models/ElementPhases.cs)

## 消息机制

元素之间通过消息机制进行通信。

### 发送消息

```csharp
// 无返回值
element.Send("MessageName", arguments);

// 有返回值
var result = element.Send<TResult>("MessageName", arguments);
```

### 处理消息

使用特性标记消息处理方法：

```csharp
[OnMessage("MessageName")]
private void HandleMessage(object args)
{
    // 处理消息
}
```

### 相关文件

- **消息分发器**: [Tools/Messaging/MessageDispatcherFactory.cs](../Tools/Messaging/MessageDispatcherFactory.cs)

## 配置和定义系统

### IElementConfiguration - 元素配置

定义元素的行为和属性的配置接口层次：

```
IElementConfiguration (基础)
├── IValueConfiguration (值配置)
├── IFieldConfiguration (字段配置)
├── IPropertyConfiguration (属性配置)
├── ICollectionConfiguration (集合配置)
├── ICollectionItemConfiguration (集合项配置)
├── IMethodConfiguration (方法配置)
├── IMethodParameterConfiguration (方法参数配置)
├── IGroupConfiguration (分组配置)
└── IRootConfiguration (根配置)
```

### IElementDefinition - 元素定义

通过配置创建的不可变定义对象，包含元素的元数据。

**相关文件**:
- **配置接口**: [Core/Elements/Configurations/Abstractions/](../Core/Elements/Configurations/Abstractions/)
- **定义接口**: [Core/Elements/Definitions/](../Core/Elements/Definitions/)
- **配置扩展**: [Core/Elements/Configurations/Extensions/](../Core/Elements/Configurations/Extensions/)

## 实现类

### ValueElement<T>

值元素的泛型实现基类。

**位置**: [Core/Implementations/Elements/ValueElement.cs](../Core/Implementations/Elements/ValueElement.cs)

### PropertyElement / FieldElement

属性和字段元素的实现。

**位置**:
- [Core/Implementations/Elements/PropertyElement.cs](../Core/Implementations/Elements/PropertyElement.cs)
- [Core/Implementations/Elements/FieldElement.cs](../Core/Implementations/Elements/FieldElement.cs)

## 下一步

- 了解 [元素树系统](./03-ElementTree.md) 如何管理元素
- 了解 [值条目系统](./04-ValueEntrySystem.md) 如何操作数值
- 了解 [后处理器系统](./07-PostProcessorSystem.md) 如何创建分组元素

---

最后更新: 2026-01-03
