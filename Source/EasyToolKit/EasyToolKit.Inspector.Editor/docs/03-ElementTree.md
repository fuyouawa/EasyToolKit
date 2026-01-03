# 元素树系统 (ElementTree)

## 概述

ElementTree 是 EasyToolKit.Inspector.Editor 的核心管理系统，负责管理整个 Inspector 元素的层次结构、更新、绘制和生命周期。

## IElementTree 接口

**位置**: [Core/Elements/Abstractions/IElementTree.cs](../Core/Elements/Abstractions/IElementTree.cs)

### 核心职责

1. **管理元素树层次结构** - 维护父子关系
2. **协调元素更新和绘制** - 统一调度
3. **提供元素工厂服务** - 创建新元素
4. **管理回调队列** - 支持延迟执行
5. **处理值变化应用** - 批量更新脏值

### 核心属性

```csharp
public interface IElementTree
{
    int UpdateId { get; }                    // 更新标识符（每帧递增）
    SerializedObject SerializedObject { get; } // Unity 序列化对象
    IRootElement Root { get; }               // 根元素
    IReadOnlyList<object> Targets { get; }   // 目标对象列表
    Type TargetType { get; }                 // 目标类型
    bool DrawMonoScriptObjectField { get; set; } // 是否绘制脚本字段
    IElementFactory ElementFactory { get; }  // 元素工厂
}
```

### 核心方法

```csharp
// 绘制流程
void BeginDraw();
void DrawElements();
void EndDraw();

// 回调管理
void QueueCallback(Action callback);
void QueueCallbackUntilRepaint(Action callback);

// Unity 集成
SerializedProperty GetUnityPropertyByPath(string propertyPath);
```

## ElementTree 实现

**位置**: [Core/Implementations/Elements/ElementTree.cs](../Core/Implementations/Elements/ElementTree.cs)

### 关键实现细节

#### 1. UpdateId 机制

每帧递增 `UpdateId`，防止单帧内重复执行逻辑：

```csharp
public void Update(bool forceUpdate = false)
{
    if (_lastUpdateId == SharedContext.UpdateId && !forceUpdate)
        return;

    _lastUpdateId = SharedContext.UpdateId;
    // 执行更新逻辑
}
```

**优点**:
- 避免单帧内重复更新
- 提升性能

#### 2. 脏值跟踪

使用 HashSet 跟踪需要更新的元素：

```csharp
private readonly HashSet<IValueElement> _dirtyValueElements = new HashSet<IValueElement>();

private void OnEvent(object sender, ValueDirtyEventArgs e)
{
    if (sender is IValueElement element)
    {
        _dirtyValueElements.Add(element);
    }
}
```

**优点**:
- 批量处理值变化
- 减少不必要的更新

#### 3. 回调队列

支持立即回调和延迟回调：

```csharp
// 立即执行（下一帧）
tree.QueueCallback(() => { /* ... */ });

// 延迟到重绘
tree.QueueCallbackUntilRepaint(() => { /* ... */ });
```

**优点**:
- 安全的延迟执行
- 避免在绘制过程中修改树结构

#### 4. MonoScript 支持

可选择显示/隐藏脚本引用字段：

```csharp
public bool DrawMonoScriptObjectField { get; set; }
```

#### 5. 音频过滤器集成

支持 Unity 音频过滤器的 GUI 显示。

## 元素工厂 (IElementFactory)

**位置**: [Core/Elements/Factories/Abstractions/IElementFactory.cs](../Core/Elements/Factories/Abstractions/IElementFactory.cs)

### 职责

元素工厂负责创建各种类型的元素：

```csharp
public interface IElementFactory
{
    IRootElement CreateRootElement(IRootDefinition definition);
    IValueElement CreateValueElement(IValueDefinition definition);
    ICollectionElement CreateCollectionElement(ICollectionDefinition definition);
    ICollectionItemElement CreateCollectionItemElement(ICollectionItemDefinition definition);
    IMethodElement CreateMethodElement(IMethodDefinition definition);
    IMethodParameterElement CreateMethodParameterElement(IMethodParameterDefinition definition);
    IGroupElement CreateGroupElement(IGroupDefinition definition);
}
```

### 实现

**位置**: [Core/Implementations/Elements/Factories/ElementFactory.cs](../Core/Implementations/Elements/Factories/ElementFactory.cs)

### 元素创建流程

```
IElementDefinition
    ↓
ElementFactory.CreateXxxElement(definition)
    ↓
new XxxElement(definition, sharedContext)
    ↓
Element.Initialize() → 创建 Resolver、ValueEntry、LogicalChildren
    ↓
返回 Element
```

## 元素树工厂 (IElementTreeFactory)

**位置**: [Core/Elements/Factories/Abstractions/IElementTreeFactory.cs](../Core/Elements/Factories/Abstractions/IElementTreeFactory.cs)

### 职责

创建完整的元素树：

```csharp
public interface IElementTreeFactory
{
    IElementTree CreateTree(SerializedObject serializedObject);
}
```

### 实现

**位置**: [Core/Implementations/Elements/Factories/ElementTreeFactory.cs](../Core/Implementations/Elements/Factories/ElementTreeFactory.cs)

### 树创建流程

```
SerializedObject
    ↓
TreeFactory.CreateTree()
    ↓
创建 ElementTree 实例
    ↓
创建 RootElement
    ↓
RootElement.Initialize() → 解析子元素
    ↓
完整的 ElementTree
```

## InspectorElements 静态工厂

**位置**: [Core/Elements/Extensions/InspectorElements.cs](../Core/Elements/Extensions/InspectorElements.cs)

提供对核心工厂的集中访问：

```csharp
public static class InspectorElements
{
    public static IElementConfigurator Configurator { get; }
    public static IElementTreeFactory TreeFactory { get; }
}
```

### 使用示例

```csharp
// 创建元素树
var tree = InspectorElements.TreeFactory.CreateTree(serializedObject);

// 创建元素配置
var config = InspectorElements.Configurator.Value()
    .WithName("MyProperty")
    .WithValueType(typeof(int))
    .CreateDefinition();
```

## 元素树的操作

### Request 机制

安全地修改树结构：

```csharp
element.Request(() =>
{
    // 在绘制完成后执行
    element.Children.Add(newElement);
});
```

**优点**:
- 避免在绘制过程中修改树结构
- 防止渲染不一致

### 刷新机制

请求刷新元素结构：

```csharp
element.RequestRefresh();
```

**流程**:
1. 标记元素为 `PendingRefresh`
2. 在安全时机执行刷新
3. 重新创建 Resolver 和 Children
4. 标记为 `JustRefreshed`

### 销毁机制

销毁元素：

```csharp
element.Destroy();
```

**流程**:
1. 如果元素处于空闲状态，立即销毁
2. 否则，标记为 `PendingDestroy`，延迟销毁
3. 从工厂的跟踪容器中移除
4. 释放资源

## 元素树的绘制流程

```
BeginDraw()
    ↓
    ├─→ UpdateId++
    │
    ├─→ Root.Update()
    │    └─→ 递归更新所有子元素
    │
    ├─→ ApplyDirtyValues()
    │    └─→ 批量应用值变化
    │
    └─→ 开始绘制阶段
         ↓
DrawElements()
    ↓
    ├─→ Root.Draw(label)
    │    └─→ 递归绘制所有子元素
    │         └─→ DrawerChain.Draw()
    │
    └─→ 处理回调队列
         ↓
EndDraw()
    ↓
    ├─→ 执行 QueuedCallbacks
    │
    └─→ 清理临时状态
```

## 元素共享上下文 (IElementSharedContext)

每个元素树都有一个共享上下文，所有元素共享该上下文。

### 职责

1. **服务容器** - 提供依赖注入访问
2. **UpdateId** - 每帧递增的更新标识符
3. **Resolver Factory** - 各种 Resolver 的工厂

### 相关文件

**位置**: [Core/Elements/Abstractions/IElementSharedContext.cs](../Core/Elements/Abstractions/IElementSharedContext.cs)

## 性能优化

### 1. UpdateId 防重复

防止单帧内重复执行逻辑。

### 2. 脏值跟踪

只更新发生变化的值元素。

### 3. 延迟回调

避免频繁的树结构修改。

### 4. 对象池

复用事件对象，减少 GC。

## 相关文档

- [元素系统](./02-ElementSystem.md) - 元素的详细设计
- [解析器系统](./05-ResolverSystem.md) - Resolver 的工作原理
- [绘制器系统](./06-DrawerSystem.md) - Drawer 的工作原理

---

最后更新: 2026-01-03
