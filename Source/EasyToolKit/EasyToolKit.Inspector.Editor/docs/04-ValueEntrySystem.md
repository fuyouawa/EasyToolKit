# 值条目系统 (Value Entry System)

## 概述

值条目系统是 EasyToolKit.Inspector.Editor 中管理值访问、状态和变更的核心系统。它提供统一的接口来读写值、跟踪值状态，并处理值变化事件。

## 系统架构

```
IValueEntry (统一接口)
├── IValueAccessor (值访问器)
├── IValueState (值状态)
└── IValueChangeHandler (值变化处理器)
```

## IValueEntry 接口

**位置**: [Core/Elements/ValueEntry/Abstractions/IValueEntry.cs](../Core/Elements/ValueEntry/Abstractions/IValueEntry.cs)

### 核心接口

```csharp
public interface IValueEntry : IValueAccessor, IValueState, IValueChangeHandler
{
    IValueElement OwnerElement { get; }
    bool IsReadOnly { get; }
    void Update();
}

public interface IValueEntry<TValue> : IValueEntry, IValueAccessor<TValue>
{
}
```

### 职责

1. **统一值访问** - 提供一致的读写接口
2. **状态管理** - 跟踪值是否已更改
3. **变更通知** - 触发值变化事件
4. **多目标支持** - 支持同时编辑多个对象

## IValueAccessor - 值访问器

**位置**: [Core/Elements/ValueEntry/Abstractions/IValueAccessor.cs](../Core/Elements/ValueEntry/Abstractions/IValueAccessor.cs)

### 接口定义

```csharp
public interface IValueAccessor
{
    int TargetCount { get; }           // 目标对象数量
    Type ValueType { get; }            // 值类型
    Type RuntimeValueType { get; }     // 运行时值类型

    // 弱类型访问
    object WeakSmartValue { get; set; }
    object GetWeakValue(int targetIndex);
    void SetWeakValue(int targetIndex, object value);
}

public interface IValueAccessor<T> : IValueAccessor
{
    // 强类型访问
    T SmartValue { get; set; }
    T GetValue(int targetIndex);
    void SetValue(int targetIndex, T value);
}
```

### SmartValue vs GetValue(index)

- **SmartValue**: 当编辑多个对象时，如果所有对象值相同，返回该值；否则返回默认值
- **GetValue(index)**: 获取指定索引的目标对象的值

### 相关文件

- **接口**: [Core/Elements/ValueEntry/Abstractions/IValueAccessor.cs](../Core/Elements/ValueEntry/Abstractions/IValueAccessor.cs)
- **实现**: [Core/Implementations/ValueEntry/ValueAccessor.cs](../Core/Implementations/ValueEntry/ValueAccessor.cs)

## IValueState - 值状态

**位置**: [Core/Elements/ValueEntry/Abstractions/IValueState.cs](../Core/Elements/ValueEntry/Abstractions/IValueState.cs)

### 接口定义

```csharp
public interface IValueState
{
    bool IsDirty { get; }        // 值是否已更改
    void MarkDirty();            // 标记为脏值
    void ClearDirty();           // 清除脏值标记
}
```

### 职责

1. **跟踪值是否已更改**
2. **管理值的脏状态**
3. **提供状态查询接口**

### 脏值机制

```
ValueEntry.SmartValue = newValue
    ↓
触发 BeforeValueChanged 事件
    ↓
更新值
    ↓
MarkDirty() → 标记为脏值
    ↓
触发 AfterValueChanged 事件
    ↓
ElementTree 检测到脏值 → 批量应用变化
    ↓
ClearDirty() → 清除脏值标记
```

### 相关文件

- **接口**: [Core/Elements/ValueEntry/Abstractions/IValueState.cs](../Core/Elements/ValueEntry/Abstractions/IValueState.cs)
- **实现**: [Core/Implementations/ValueEntry/ValueState.cs](../Core/Implementations/ValueEntry/ValueState.cs)

## IValueChangeHandler - 值变化处理器

**位置**: [Core/Elements/ValueEntry/Abstractions/IValueChangeHandler.cs](../Core/Elements/ValueEntry/Abstractions/IValueChangeHandler.cs)

### 接口定义

```csharp
public interface IValueChangeHandler
{
    event EventHandler<ValueChangedEventArgs> BeforeValueChanged;
    event EventHandler<ValueChangedEventArgs> AfterValueChanged;

    void ApplyChanges();
    void EnqueueChange(Action action);
}
```

### 事件流程

```
用户修改值
    ↓
BeforeValueChanged 事件
    ↓
执行更改
    ↓
AfterValueChanged 事件
    ↓
ApplyChanges() → 应用到目标对象
```

### 事件参数

```csharp
public class ValueChangedEventArgs : EventArgs
{
    public IValueEntry ValueEntry { get; }
    public object OldValue { get; }
    public object NewValue { get; }
}
```

### 相关文件

- **接口**: [Core/Elements/ValueEntry/Abstractions/IValueChangeHandler.cs](../Core/Elements/ValueEntry/Abstractions/IValueChangeHandler.cs)
- **实现**: [Core/Implementations/ValueEntry/ValueChangeHandler.cs](../Core/Implementations/ValueEntry/ValueChangeHandler.cs)

## ValueOperation - 值操作

值操作是 ValueEntry 实际读写值的底层机制。

### IValueOperation 接口

**位置**: [Core/Operations/Abstractions/IValueOperation.cs](../Core/Operations/Abstractions/IValueOperation.cs)

```csharp
public interface IValueOperation
{
    Type ValueType { get; }
    object GetValue(int targetIndex);
    void SetValue(int targetIndex, object value);
}
```

### 操作类型

#### 1. MemberValueOperation

通过反射访问类成员（字段/属性）。

**位置**: [Operations/MemberValueOperation.cs](../Operations/MemberValueOperation.cs)

#### 2. UnityPropertyOperation

通过 Unity SerializedProperty 访问值。

**位置**: [Operations/UnityPropertyOperation.cs](../Operations/UnityPropertyOperation.cs)

### ValueEntry 与 ValueOperation 的关系

```
ValueElement
    ↓
ValueOperationResolver.GetOperation()
    ↓
返回 IValueOperation
    ↓
创建 ValueEntry(IValueOperation)
    ↓
ValueEntry 通过 IValueOperation 读写值
```

### 相关文件

- **接口**: [Core/Operations/Abstractions/IValueOperation.cs](../Core/Operations/Abstractions/IValueOperation.cs)
- **反射操作**: [Operations/MemberValueOperation.cs](../Operations/MemberValueOperation.cs)
- **Unity操作**: [Operations/UnityPropertyOperation.cs](../Operations/UnityPropertyOperation.cs)

## 集合元素操作 (ICollectionElementOperation)

**位置**: [Core/Operations/Abstractions/ICollectionElementOperation.cs](../Core/Operations/Abstractions/ICollectionElementOperation.cs)

### 接口定义

```csharp
public interface ICollectionElementOperation
{
    Type ElementType { get; }
    int GetCount(int targetIndex);
    object GetItem(int targetIndex, int index);
    void AddItem(int targetIndex);
    void AddItems(int targetIndex, int count);
    void RemoveItem(int targetIndex, int index);
    void Clear(int targetIndex);
    void Resize(int targetIndex, int newSize);
    void DuplicateItem(int targetIndex, int index);
}
```

### 实现类

#### 1. ListElementOperation

支持可变集合的完整操作。

**位置**: [Operations/CollectionElement/ListElementOperation.cs](../Operations/CollectionElement/ListElementOperation.cs)

#### 2. ReadOnlyListElementOperation

仅提供查询功能。

**位置**: [Operations/CollectionElement/ReadOnlyListElementOperation.cs](../Operations/CollectionElement/ReadOnlyListElementOperation.cs)

#### 3. UnityCollectionOperation

处理 Unity 序列化集合。

**位置**: [Operations/Collection/UnityCollectionOperation.cs](../Operations/Collection/UnityCollectionOperation.cs)

## ValueEntry 创建流程

```
ValueElement.Initialize()
    ↓
ValueOperationResolver.GetOperation()
    ↓
    ├─→ 检查 ValueDefinition
    ├─→ 根据 AsUnityProperty 选择操作类型
    └─→ 返回 IValueOperation
         ↓
创建 ValueEntry(IValueOperation)
    ↓
ValueElement.ValueEntry = ValueEntry
```

### ValueOperationResolver

**位置**: [Core/Resolvers/Abstractions/IValueOperationResolver.cs](../Core/Resolvers/Abstractions/IValueOperationResolver.cs)

```csharp
public interface IValueOperationResolver : IResolver
{
    IValueOperation GetOperation();
}
```

### Resolver 工厂

**位置**: [Core/ResolverFactories/Abstractions/IValueOperationResolverFactory.cs](../Core/ResolverFactories/Abstractions/IValueOperationResolverFactory.cs)

```csharp
public interface IValueOperationResolverFactory
{
    IValueOperationResolver GetResolver<TValue>();
    IValueOperationResolver GetResolver(Type valueType);
}
```

### 默认实现

**位置**: [Core/Implementations/ResolverFactories/DefaultValueOperationResolverFactory.cs](../Core/Implementations/ResolverFactories/DefaultValueOperationResolverFactory.cs)

## 多目标对象编辑

ValueEntry 支持同时编辑多个对象（Unity 的多选功能）。

### SmartValue 行为

```csharp
// 所有对象值相同
valueEntry.SmartValue = 10;  // 设置所有对象的值为 10

// 对象值不同
var value = valueEntry.SmartValue;  // 返回默认值
```

### GetValue(index) 行为

```csharp
// 获取特定对象的值
var value1 = valueEntry.GetValue(0);  // 第一个对象的值
var value2 = valueEntry.GetValue(1);  // 第二个对象的值
```

## 值变化事件应用

```
用户修改值
    ↓
ValueEntry.SmartValue = newValue
    ↓
标记为脏值（IsDirty = true）
    ↓
ElementTree 检测到脏值
    ↓
批量应用变化（ApplyChanges）
    ↓
SerializedObject.ApplyModifiedProperties()
    ↓
清除脏值标记（IsDirty = false）
```

## 性能优化

### 1. 脏值跟踪

只更新发生变化的值，减少不必要的序列化操作。

### 2. 批量应用

在绘制结束后批量应用所有变化，减少序列化开销。

### 3. 延迟回调

使用 `EnqueueChange` 延迟执行值变化操作。

## 相关文档

- [元素系统](./02-ElementSystem.md) - ValueElement 的设计
- [解析器系统](./05-ResolverSystem.md) - ValueOperationResolver 的工作原理
- [元素树系统](./03-ElementTree.md) - 脏值跟踪和应用机制

---

最后更新: 2026-01-03
