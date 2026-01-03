# 解析器系统 (Resolver System)

## 概述

解析器系统是 EasyToolKit.Inspector.Editor 的核心扩展机制，负责解析元素的结构、操作、属性等。通过 Resolver 模式，框架可以灵活地支持各种自定义类型和逻辑。

## 解析器层次结构

```
IResolver (基础接口)
├── IStructureResolver (结构解析器)
│   ├── 获取子元素定义
│   └── 分析元素结构
│
├── IValueOperationResolver (值操作解析器)
│   ├── 提供值操作接口
│   └── 处理值读写
│
├── ICollectionElementOperationResolver (集合操作解析器)
│   ├── 集合项操作
│   └── 集合管理
│
├── IDrawerChainResolver (绘制链解析器)
│   ├── 构建绘制器链
│   └── 解析属性到绘制器
│
├── IAttributeResolver (属性解析器)
│   ├── 解析自定义属性
│   └── 提供属性信息
│
└── IPostProcessorChainResolver (后处理链解析器)
    ├── 元素后处理
    └── 树结构后处理
```

## IResolver 基础接口

```csharp
public interface IResolver
{
    // 所有 Resolver 的基础接口
}
```

## IStructureResolver - 结构解析器

**位置**: [Core/Resolvers/Abstractions/IStructureResolver.cs](../Core/Resolvers/Abstractions/IStructureResolver.cs)

### 接口定义

```csharp
public interface IStructureResolver : IResolver
{
    IElementDefinition[] GetChildrenDefinitions();
}
```

### 职责

1. **解析逻辑子结构** - 为 LogicalElement 提供子元素定义
2. **分析元素结构** - 确定元素的层次结构
3. **返回定义数组** - 供 ElementFactory 创建子元素

### 结构解析流程

```
LogicalElement.Initialize()
    ↓
StructureResolverFactory.GetResolver(valueType)
    ↓
IStructureResolver.GetChildrenDefinitions()
    ↓
返回 IElementDefinition[]
    ↓
ElementFactory.CreateElements()
    ↓
LogicalElement.LogicalChildren
```

### 实现类

#### 1. ValueStructureResolverBase

值结构解析器基类，处理单个值的结构。

**位置**: [Resolvers/StructureResolver/ValueStructureResolverBase.cs](../Resolvers/StructureResolver/ValueStructureResolverBase.cs)

**职责**:
- 处理单个值的结构
- 支持泛型类型
- 为值类型提供子元素定义

#### 2. CollectionStructureResolverBase

集合结构解析器基类，处理数组、列表等集合类型。

**位置**: [Resolvers/StructureResolver/Collection/CollectionStructureResolver.cs](../Resolvers/StructureResolver/Collection/CollectionStructureResolver.cs)

**职责**:
- 处理数组、列表等集合类型
- 管理集合项元素
- 提供集合项的定义

#### 3. ReadOnlyCollectionStructureResolver

只读集合结构解析器。

**位置**: [Resolvers/StructureResolver/Collection/ReadOnlyCollectionStructureResolver.cs](../Resolvers/StructureResolver/Collection/ReadOnlyCollectionStructureResolver.cs)

#### 4. UnityCollectionStructureResolver

Unity 序列化集合结构解析器。

**位置**: [Resolvers/StructureResolver/Collection/UnityCollectionStructureResolver.cs](../Resolvers/StructureResolver/Collection/UnityCollectionStructureResolver.cs)

#### 5. MethodStructureResolverBase

方法结构解析器基类。

**位置**: [Resolvers/StructureResolver/MethodStructureResolverBase.cs](../Resolvers/StructureResolver/MethodStructureResolverBase.cs)

**职责**:
- 处理方法元素
- 解析方法参数
- 为方法参数提供定义

### 相关文件

- **接口**: [Core/Resolvers/Abstractions/IStructureResolver.cs](../Core/Resolvers/Abstractions/IStructureResolver.cs)
- **工厂**: [Core/ResolverFactories/Abstractions/IStructureResolverFactory.cs](../Core/ResolverFactories/Abstractions/IStructureResolverFactory.cs)
- **工厂实现**: [Core/Implementations/ResolverFactories/DefaultStructureResolverFactory.cs](../Core/Implementations/ResolverFactories/DefaultStructureResolverFactory.cs)

## IValueOperationResolver - 值操作解析器

**位置**: [Core/Resolvers/Abstractions/IValueOperationResolver.cs](../Core/Resolvers/Abstractions/IValueOperationResolver.cs)

### 接口定义

```csharp
public interface IValueOperationResolver : IResolver
{
    IValueOperation GetOperation();
}
```

### 职责

1. **提供值操作接口** - 返回 IValueOperation
2. **处理值读写** - 通过 Operation 实际读写值

### 操作解析流程

```
ValueElement.Initialize()
    ↓
ValueOperationResolverFactory.GetResolver(valueType)
    ↓
IValueOperationResolver.GetOperation()
    ↓
返回 IValueOperation
    ↓
创建 ValueEntry(IValueOperation)
    ↓
ValueElement.ValueEntry
```

### 实现类

根据 `AsUnityProperty` 标志选择不同的操作类型：

- **true**: 返回 UnityPropertyOperation
- **false**: 返回 MemberValueOperation

### 相关文件

- **接口**: [Core/Resolvers/Abstractions/IValueOperationResolver.cs](../Core/Resolvers/Abstractions/IValueOperationResolver.cs)
- **工厂**: [Core/ResolverFactories/Abstractions/IValueOperationResolverFactory.cs](../Core/ResolverFactories/Abstractions/IValueOperationResolverFactory.cs)
- **工厂实现**: [Core/Implementations/ResolverFactories/DefaultValueOperationResolverFactory.cs](../Core/Implementations/ResolverFactories/DefaultValueOperationResolverFactory.cs)

## IDrawerChainResolver - 绘制链解析器

**位置**: [Core/Resolvers/Abstractions/IDrawerChainResolver.cs](../Core/Resolvers/Abstractions/IDrawerChainResolver.cs)

### 接口定义

```csharp
public interface IDrawerChainResolver : IResolver
{
    DrawerChain GetDrawerChain();
}
```

### 职责

1. **构建绘制器链** - 为元素创建绘制器责任链
2. **解析属性到绘制器** - 根据属性信息选择绘制器

### 绘制链解析流程

```
Element.Initialize()
    ↓
DrawerChainResolverFactory.GetResolver(element)
    ↓
IDrawerChainResolver.GetDrawerChain()
    ↓
    ├─→ 查找适用的 ValueDrawer
    ├─→ 查找适用的 AttributeDrawer
    └─→ 按 DrawerPriority 排序
         ↓
返回 DrawerChain
    ↓
Element.DrawerChain
```

### 绘制器选择规则

1. **ValueDrawer** - 根据值类型选择
2. **AttributeDrawer** - 根据自定义属性选择
3. **优先级排序** - DrawerPriorityAttribute

### 相关文件

- **接口**: [Core/Resolvers/Abstractions/IDrawerChainResolver.cs](../Core/Resolvers/Abstractions/IDrawerChainResolver.cs)
- **工厂**: [Core/ResolverFactories/Abstractions/IDrawerChainResolverFactory.cs](../Core/ResolverFactories/Abstractions/IDrawerChainResolverFactory.cs)
- **工厂实现**: [Core/Implementations/ResolverFactories/DefaultDrawerChainResolverFactory.cs](../Core/Implementations/ResolverFactories/DefaultDrawerChainResolverFactory.cs)

## IAttributeResolver - 属性解析器

**位置**: [Core/Resolvers/Abstractions/IAttributeResolver.cs](../Core/Resolvers/Abstractions/IAttributeResolver.cs)

### 接口定义

```csharp
public interface IAttributeResolver : IResolver
{
    // 解析自定义属性
}
```

### 职责

1. **解析自定义属性** - 获取元素的自定义属性信息
2. **提供属性信息** - 返回 ElementAttributeInfo

### 相关文件

- **接口**: [Core/Resolvers/Abstractions/IAttributeResolver.cs](../Core/Resolvers/Abstractions/IAttributeResolver.cs)
- **工厂**: [Core/ResolverFactories/Abstractions/IAttributeResolverFactory.cs](../Core/ResolverFactories/Abstractions/IAttributeResolverFactory.cs)
- **工厂实现**: [Core/Implementations/ResolverFactories/DefaultAttributeResolverFactory.cs](../Core/Implementations/ResolverFactories/DefaultAttributeResolverFactory.cs)

## IPostProcessorChainResolver - 后处理链解析器

**位置**: [Core/Resolvers/Abstractions/IPostProcessorChainResolver.cs](../Core/Resolvers/Abstractions/IPostProcessorChainResolver.cs)

### 接口定义

```csharp
public interface IPostProcessorChainResolver : IResolver
{
    PostProcessorChain GetPostProcessorChain();
}
```

### 职责

1. **构建后处理器链** - 为元素创建后处理器责任链
2. **元素后处理** - 在元素创建后执行后处理逻辑
3. **树结构后处理** - 修改树结构（如创建分组）

### 后处理链解析流程

```
Element.PostProcessIfNeeded()
    ↓
PostProcessorChainResolverFactory.GetResolver(element)
    ↓
IPostProcessorChainResolver.GetPostProcessorChain()
    ↓
    ├─→ 查找适用的 PostProcessor
    └─→ 按 PostProcessorPriority 排序
         ↓
返回 PostProcessorChain
    ↓
PostProcessorChain.Process()
    ↓
    ├─→ GroupElementPostProcessor.Process()
    └─→ LogicalElementPostProcessor.Process()
```

### 相关文件

- **接口**: [Core/Resolvers/Abstractions/IPostProcessorChainResolver.cs](../Core/Resolvers/Abstractions/IPostProcessorChainResolver.cs)
- **工厂**: [Core/ResolverFactories/Abstractions/IPostProcessorChainResolverFactory.cs](../Core/ResolverFactories/Abstractions/IPostProcessorChainResolverFactory.cs)
- **工厂实现**: [Core/Implementations/ResolverFactories/DefaultPostProcessorChainResolverFactory.cs](../Core/Implementations/ResolverFactories/DefaultPostProcessorChainResolverFactory.cs)

## Resolver 优先级

使用 `ResolverPriorityAttribute` 控制解析器选择顺序：

```csharp
[ResolverPriority]
public class MyCustomResolver : SomeResolverBase
{
    // 优先级高的解析器会优先被选择
}
```

### ResolverPriorityAttribute

**位置**: [Core/Resolvers/ResolverPriorityAttribute.cs](../Core/Resolvers/ResolverPriorityAttribute.cs)

## Resolver 工厂

### IResolverFactory 基础接口

```csharp
public interface IResolverFactory
{
    // 工厂基础接口
}
```

### 具体工厂接口

- **IStructureResolverFactory** - 结构解析器工厂
- **IValueOperationResolverFactory** - 值操作解析器工厂
- **IDrawerChainResolverFactory** - 绘制链解析器工厂
- **IAttributeResolverFactory** - 属性解析器工厂
- **IPostProcessorChainResolverFactory** - 后处理链解析器工厂

### 工厂实现位置

- **DefaultStructureResolverFactory**: [Core/Implementations/ResolverFactories/DefaultStructureResolverFactory.cs](../Core/Implementations/ResolverFactories/DefaultStructureResolverFactory.cs)
- **DefaultValueOperationResolverFactory**: [Core/Implementations/ResolverFactories/DefaultValueOperationResolverFactory.cs](../Core/Implementations/ResolverFactories/DefaultValueOperationResolverFactory.cs)
- **DefaultDrawerChainResolverFactory**: [Core/Implementations/ResolverFactories/DefaultDrawerChainResolverFactory.cs](../Core/Implementations/ResolverFactories/DefaultDrawerChainResolverFactory.cs)
- **DefaultAttributeResolverFactory**: [Core/Implementations/ResolverFactories/DefaultAttributeResolverFactory.cs](../Core/Implementations/ResolverFactories/DefaultAttributeResolverFactory.cs)
- **DefaultPostProcessorChainResolverFactory**: [Core/Implementations/ResolverFactories/DefaultPostProcessorChainResolverFactory.cs](../Core/Implementations/ResolverFactories/DefaultPostProcessorChainResolverFactory.cs)

## 创建自定义 Resolver

### 1. 结构解析器

```csharp
[ResolverPriority]
public class MyCustomStructureResolver : ValueStructureResolverBase<MyCustomType>
{
    protected override IElementDefinition[] GetChildrenDefinitions()
    {
        // 返回子元素定义
        return new IElementDefinition[]
        {
            // ... 子元素定义
        };
    }
}
```

### 2. 操作解析器

```csharp
[ResolverPriority]
public class MyCustomOperationResolver : ValueOperationResolverBase<MyCustomType>
{
    protected override IValueOperation GetOperation()
    {
        // 返回自定义操作实现
        return new MyCustomOperation();
    }
}
```

## 设计模式

### 策略模式 (Strategy Pattern)

- Resolver 系统是策略模式的典型应用
- 运行时选择具体的 Resolver 实现
- 通过优先级属性控制选择

### 依赖注入 (Dependency Injection)

- Resolver 工厂通过 IElementSharedContext 注入
- 元素通过 SharedContext 访问 Resolver 工厂

## 相关文档

- [元素系统](./02-ElementSystem.md) - LogicalChildren 的解析
- [值条目系统](./04-ValueEntrySystem.md) - ValueOperation 的解析
- [绘制器系统](./06-DrawerSystem.md) - DrawerChain 的解析
- [后处理器系统](./07-PostProcessorSystem.md) - PostProcessorChain 的解析

---

最后更新: 2026-01-03
