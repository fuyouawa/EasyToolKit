# 绘制器系统 (Drawer System)

## 概述

绘制器系统是 EasyToolKit.Inspector.Editor 的核心渲染系统，负责在 Unity Editor 中绘制各种元素。通过责任链模式，绘制器系统支持灵活的自定义渲染逻辑。

## 系统架构

```
IDrawerChainResolver
    ↓
DrawerChain (责任链)
    ↓
IEasyDrawer[] (绘制器数组)
    ↓
Element.Draw()
```

## IEasyDrawer 接口

**位置**: [Core/Drawer/Abstractions/IEasyDrawer.cs](../Core/Drawer/Abstractions/IEasyDrawer.cs)

### 接口定义

```csharp
public interface IEasyDrawer : IHandler
{
    DrawerChain Chain { get; set; }
    bool SkipWhenDrawing { get; set; }
    void Draw(GUIContent label);
}
```

### 核心属性

- **Chain**: 绘制器所属的责任链
- **SkipWhenDrawing**: 是否在绘制时跳过此绘制器（但仍需初始化）
- **Element**: 通过 IHandler 继承，获取当前绘制的元素

### IHandler 基础接口

```csharp
public interface IHandler
{
    IElement Element { get; }
    void Initialize();
}
```

## EasyDrawer 抽象基类

**位置**: [Drawers/EasyDrawer.cs](../Drawers/EasyDrawer.cs)

### 类定义

```csharp
public abstract class EasyDrawer : IEasyDrawer
{
    public IElement Element { get; }
    public DrawerChain Chain { get; set; }
    public bool SkipWhenDrawing { get; set; }

    protected abstract bool CanDraw(IElement element);
    protected virtual void Initialize() { }
    protected virtual void Draw(GUIContent label) { }

    protected bool CallNextDrawer(GUIContent label) { }
}
```

### 核心方法

#### CanDraw

判断此绘制器是否可以绘制指定元素：

```csharp
protected abstract bool CanDraw(IElement element);
```

#### Initialize

初始化绘制器，在首次使用前调用：

```csharp
protected virtual void Initialize() { }
```

#### Draw

绘制逻辑：

```csharp
protected virtual void Draw(GUIContent label)
{
    // 自定义绘制逻辑
}
```

#### CallNextDrawer

调用责任链中的下一个绘制器：

```csharp
protected bool CallNextDrawer(GUIContent label)
{
    if (_chain.MoveNext() && _chain.Current != null)
    {
        _chain.Current.Draw(label);
        return true;
    }
    return false;
}
```

## EasyValueDrawer<TValue> 泛型值绘制器

**位置**: [Drawers/EasyValueDrawer.cs](../Drawers/EasyValueDrawer.cs)

### 类定义

```csharp
public abstract class EasyValueDrawer<TValue> : EasyDrawer
{
    public new IValueElement Element { get; }
    public IValueEntry<TValue> ValueEntry { get; }

    protected virtual bool CanDrawValueType(Type valueType) { }
    protected virtual bool CanDrawElement(IValueElement element) { }
}
```

### 核心属性

- **Element**: 强类型的值元素
- **ValueEntry**: 强类型的值条目，用于访问值

### 便捷方法

#### SmartValue

快速访问值：

```csharp
protected TValue SmartValue
{
    get => ValueEntry.SmartValue;
    set => ValueEntry.SmartValue = value;
}
```

## EasyAttributeDrawer<TAttribute> 属性绘制器

**位置**: [Drawers/EasyAttributeDrawer.cs](../Drawers/EasyAttributeDrawer.cs)

### 类定义

```csharp
public abstract class EasyAttributeDrawer<TAttribute> : EasyDrawer
    where TAttribute : Attribute
{
    public TAttribute Attribute { get; }

    protected override bool CanDraw(IElement element)
    {
        return element.TryGetAttributeInfo(Attribute, out _);
    }
}
```

### 职责

- 为特定属性提供自定义绘制
- 在属性前后添加额外 UI
- 修改属性的默认行为

## DrawerChain 责任链

**位置**: [Core/Drawer/Models/DrawerChain.cs](../Core/Drawer/Models/DrawerChain.cs)

### 类定义

```csharp
public class DrawerChain : IEnumerable<IEasyDrawer>, IEnumerator<IEasyDrawer>
{
    public IEasyDrawer Current { get; }
    public IEasyDrawer[] Drawers { get; }
    public IElement Element { get; }

    public bool MoveNext() { }
    public void Reset() { }
    public IEnumerator<IEasyDrawer> GetEnumerator() { }
}
```

### 特性

1. **同时实现 IEnumerable 和 IEnumerator**
2. **自动跳过 SkipWhenDrawing 的绘制器**
3. **支持绘制器链式调用**

### 遍历流程

```
DrawerChain.GetEnumerator()
    ↓
    ├─→ MoveNext()
    │    └─→ 跳过 SkipWhenDrawing 的 Drawer
    │    └─→ 移动到下一个 Drawer
    │
    └─→ Current
         └─→ 返回当前 Drawer
```

## 绘制器优先级

使用 `DrawerPriorityAttribute` 控制绘制器顺序：

```csharp
public enum DrawerPriorityLevel
{
    VeryLow = 100,
    Low = 75,
    Medium = 50,
    High = 25,
    VeryHigh = 0
}
```

### DrawerPriorityAttribute

**位置**: [Core/Drawer/Models/DrawerPriorityAttribute.cs](../Core/Drawer/Models/DrawerPriorityAttribute.cs)

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DrawerPriorityAttribute : Attribute, IPriorityAccessor
{
    public DrawerPriorityLevel Priority { get; }
}
```

### 优先级排序

优先级值越小，优先级越高，越早被调用。

## DrawerChainResolver

**位置**: [Core/Resolvers/Abstractions/IDrawerChainResolver.cs](../Core/Resolvers/Abstractions/IDrawerChainResolver.cs)

### 接口定义

```csharp
public interface IDrawerChainResolver : IResolver
{
    DrawerChain GetDrawerChain();
}
```

### 解析流程

```
Element.Initialize()
    ↓
DrawerChainResolver.GetDrawerChain()
    ↓
    ├─→ 查找所有适用的 ValueDrawer
    │    └─→ CanDrawValueType() 或 CanDrawElement()
    │
    ├─→ 查找所有适用的 AttributeDrawer
    │    └─→ element.TryGetAttributeInfo()
    │
    └─→ 按 DrawerPriority 排序
         ↓
返回 DrawerChain
```

## 绘制流程

```
Element.Draw(label)
    ↓
DrawerChain.GetEnumerator()
    ↓
    ├─→ 第一个 Drawer.Draw(label)
    │    ├─→ 自定义绘制逻辑
    │    └─→ CallNextDrawer(label)
    │         ↓
    │    └─→ 第二个 Drawer.Draw(label)
    │         ├─→ 自定义绘制逻辑
    │         └─→ CallNextDrawer(label)
    │              ↓
    │         └─→ 第三个 Drawer.Draw(label)
    │              └─→ ...
```

## 创建自定义绘制器

### 1. 值类型绘制器

```csharp
public class MyCustomTypeDrawer : EasyValueDrawer<MyCustomType>
{
    protected override void Initialize()
    {
        base.Initialize();
        // 初始化逻辑
    }

    protected override bool CanDrawValueType(Type valueType)
    {
        return valueType == typeof(MyCustomType);
    }

    protected override void Draw(GUIContent label)
    {
        // 自定义绘制逻辑
        var value = ValueEntry.SmartValue;

        // 绘制 UI
        EditorGUILayout.LabelField(label, new GUIContent(value.ToString()));

        // 调用下一个绘制器（可选）
        // CallNextDrawer(label);
    }
}
```

### 2. 属性绘制器

```csharp
public class MyAttributeDrawer : EasyAttributeDrawer<MyAttribute>
{
    protected override void Initialize()
    {
        base.Initialize();
        // 初始化逻辑
    }

    protected override void Draw(GUIContent label)
    {
        // 在属性前绘制
        EditorGUILayout.HelpBox("My Attribute", MessageType.Info);

        // 调用下一个绘制器
        CallNextDrawer(label);
    }
}
```

### 3. 优先级控制

```csharp
[DrawerPriority(DrawerPriorityLevel.VeryHigh)]
public class HighPriorityDrawer : EasyValueDrawer<int>
{
    // ...
}
```

## 内置绘制器

### 值绘制器

位置: [Drawers/Value/](../Drawers/Value/)

- **Number** - 数值类型（Int8, Int16, Int32, Int64, Float, Double）
- **StringDrawer** - 字符串
- **BooleanDrawer** - 布尔值
- **EnumDrawer** - 枚举
- **ColorDrawer** - 颜色
- **Vector2/3/4Drawer** - 向量
- **Vector2/3IntDrawer** - 整数向量
- **GuidDrawer** - GUID
- **UnityObjectDrawer** - Unity 对象引用
- **UnityPropertyDrawer** - Unity 序列化属性

### 属性绘制器

位置: [Drawers/Attribute/](../Drawers/Attribute/)

- **RequiredAttributeDrawer** - 必填字段
- **HideLabelAttributeDrawer** - 隐藏标签
- **HideInInspectorAttributeDrawer** - 隐藏字段
- **SpaceAttributeDrawer** - 间距
- **FolderPathAttributeDrawer** - 文件路径选择
- **RangeAttribute** 范围绘制器（多种数值类型）

## 设计模式

### 责任链模式 (Chain of Responsibility)

- DrawerChain 是责任链模式的典型应用
- 每个绘制器可以处理请求或传递给下一个
- 灵活的处理顺序

### 策略模式 (Strategy Pattern)

- 每个绘制器是一种绘制策略
- 运行时选择合适的绘制器
- 通过优先级控制策略选择

## 性能优化

### 1. SkipWhenDrawing

某些绘制器只需要初始化，不需要实际绘制：

```csharp
public class MyDrawer : EasyDrawer
{
    protected override void Initialize()
    {
        // 只执行一次的初始化逻辑
    }

    public override bool SkipWhenDrawing => true;
}
```

### 2. 延迟初始化

绘制器在首次使用时才初始化，减少不必要的初始化开销。

### 3. 绘制器缓存

DrawerChain 在元素初始化时创建并缓存，避免每次绘制都重新解析。

## 相关文档

- [解析器系统](./05-ResolverSystem.md) - DrawerChainResolver 的解析
- [元素系统](./02-ElementSystem.md) - Element.Draw() 的调用
- [值条目系统](./04-ValueEntrySystem.md) - ValueEntry 的使用

---

最后更新: 2026-01-03
