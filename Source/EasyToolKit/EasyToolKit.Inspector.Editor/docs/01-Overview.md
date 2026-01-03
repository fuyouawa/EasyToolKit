# EasyToolKit.Inspector.Editor 项目概述

## 项目简介

`EasyToolKit.Inspector.Editor` 是一个 Unity 编辑器扩展框架，提供灵活、可扩展的自定义 Inspector 系统。该项目采用模块化设计，通过元素树、解析器、绘制器和值条目等核心组件，实现了高度可定制的属性编辑界面。

## 核心特性

### 1. 元素树架构
- 树形结构的元素管理系统
- 支持动态添加、移除、重组元素
- 自动管理元素生命周期和更新

### 2. 责任链绘制器
- 灵活的渲染处理链
- 支持绘制器优先级排序
- 可跳过特定绘制器

### 3. 多解析器系统
- 结构解析器（StructureResolver）- 解析元素的逻辑子结构
- 操作解析器（OperationResolver）- 提供值读写操作
- 属性解析器（AttributeResolver）- 解析自定义属性
- 绘制链解析器（DrawerChainResolver）- 构建绘制器链
- 后处理链解析器（PostProcessorChainResolver）- 构建后处理器链

### 4. 值条目抽象
- 统一的值访问和变更管理
- 支持多目标对象编辑
- 值变化事件通知

### 5. 消息机制
- 元素间通信支持
- 特性驱动的消息处理
- 支持带返回值的消息

### 6. 对象池优化
- 事件对象池减少 GC
- 脏值跟踪优化更新
- 延迟回调机制

## 整体架构图

```
┌─────────────────────────────────────────────────────────────────┐
│                        Entry Points                              │
│  ┌──────────────────┐         ┌──────────────────────┐          │
│  │   EasyEditor     │         │  EasyEditorWindow    │          │
│  │  (Unity Editor)  │         │   (Editor Window)    │          │
│  └────────┬─────────┘         └──────────┬───────────┘          │
└───────────┼──────────────────────────────┼──────────────────────┘
            │                              │
            ▼                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                      InspectorElements                           │
│  ┌──────────────────────┐    ┌─────────────────────────┐        │
│  │   Configurator       │    │   TreeFactory           │        │
│  │ (Element Config)     │    │ (Element Tree Creation) │        │
│  └──────────────────────┘    └───────────┬─────────────┘        │
└──────────────────────────────────────────┼──────────────────────┘
                                             │
                                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                       IElementTree                               │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │                     IRootElement                         │    │
│  │  ┌─────────┐  ┌──────────┐  ┌─────────┐  ┌─────────┐  │    │
│  │  │ Value   │  │ Property │  │ Field   │  │ Method  │  │    │
│  │  │ Element │  │ Element  │  │ Element │  │ Element │  │    │
│  │  └────┬────┘  └─────┬────┘  └────┬────┘  └────┬────┘  │    │
│  │       │             │            │             │       │    │
│  │       └─────────────┴────────────┴─────────────┘       │    │
│  │                      │                                    │    │
│  │              ┌───────▼────────┐                          │    │
│  │              │  Collection    │                          │    │
│  │              │    Element     │                          │    │
│  │              └────────────────┘                          │    │
│  └─────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
        ▼                     ▼                     ▼
┌──────────────┐    ┌───────────────┐    ┌──────────────┐
│   Resolver   │    │    Drawer     │    │ Value Entry  │
│   System     │    │    System     │    │   System     │
│              │    │               │    │              │
│ - Structure  │    │ - Chain       │    │ - Accessor   │
│ - Operation  │    │ - Attribute   │    │ - State      │
│ - Attribute  │    │ - Value       │    │ - Handler    │
└──────────────┘    └───────────────┘    └──────────────┘
```

## 核心模块关系

### ElementTree（元素树）
**位置**: [Core/Elements/Abstractions/IElementTree.cs](../Core/Elements/Abstractions/IElementTree.cs)

ElementTree 是整个系统的核心，负责：
- 管理所有元素的层次结构
- 协调元素的更新和绘制
- 提供元素工厂服务
- 管理回调队列

### Element（元素）
**位置**: [Core/Elements/Abstractions/IElement.cs](../Core/Elements/Abstractions/IElement.cs)

所有 Inspector 元素的基础接口，包括：
- **IValueElement** - 值元素（字段、属性等）
- **ILogicalElement** - 逻辑元素（class/struct类型）
- **ICollectionElement** - 集合元素（数组、列表等）
- **IMethodElement** - 方法元素
- **IGroupElement** - 分组元素（抽象概念）

### ValueEntry（值条目）
**位置**: [Core/Elements/ValueEntry/Abstractions/IValueEntry.cs](../Core/Elements/ValueEntry/Abstractions/IValueEntry.cs)

ValueElement 的值访问和变更管理：
- **IValueAccessor** - 值读写访问
- **IValueState** - 值状态管理
- **IValueChangeHandler** - 值变化事件处理

### Resolver（解析器）
**位置**: [Core/Resolvers/Abstractions/](../Core/Resolvers/Abstractions/)

各种解析器的基类：
- **IStructureResolver** - 解析逻辑子结构
- **IValueOperationResolver** - 提供值操作
- **IAttributeResolver** - 解析自定义属性
- **IDrawerChainResolver** - 构建绘制器链
- **IPostProcessorChainResolver** - 构建后处理器链

### Drawer（绘制器）
**位置**: [Drawers/](../Drawers/)

负责元素的渲染：
- **IEasyDrawer** - 绘制器接口
- **DrawerChain** - 绘制器责任链
- **EasyValueDrawer<T>** - 泛型值绘制器基类
- **EasyAttributeDrawer<T>** - 属性绘制器基类

## 设计模式概览

### 1. 工厂模式（Factory Pattern）
- **IElementFactory** - 创建元素
- **IElementTreeFactory** - 创建元素树
- **各种 Resolver Factory** - 创建解析器

### 2. 责任链模式（Chain of Responsibility）
- **DrawerChain** - 绘制器链
- **PostProcessorChain** - 后处理器链

### 3. 策略模式（Strategy Pattern）
- Resolver 系统 - 运行时选择具体实现
- Drawer 系统 - 优先级控制选择
- Operation 系统 - 不同类型的操作策略

### 4. 观察者模式（Observer Pattern）
- 值变化事件（BeforeValueChanged, AfterValueChanged）
- 元素生命周期事件

### 5. 建造者模式（Builder Pattern）
- ElementConfiguration 系统 - 流式配置

### 6. 组合模式（Composite Pattern）
- Element 树形结构 - 统一处理单个和组合对象

### 7. 对象池模式（Object Pool Pattern）
- 元素事件池 - 减少 allocations

## 项目依赖

### 内部依赖
- **EasyToolKit.Core** - 核心功能（事件系统、工具类）
- **EasyToolKit.Core.Editor** - 编辑器工具
- **EasyToolKit.Inspector** - Inspector 属性定义

### 外部依赖
- **UnityEditor** - Unity 编辑器 API
- **OdinSerializer** - 序列化库
- **UnityEngine** - Unity 引擎模块

## 入口点

### EasyEditor
**位置**: [Entries/EasyEditor.cs](../Entries/EasyEditor.cs)

Unity Editor 的自定义 Inspector 扩展入口。

### EasyEditorWindow
**位置**: [Entries/EasyEditorWindow.cs](../Entries/EasyEditorWindow.cs)

Editor Window 的入口，用于创建独立的编辑器窗口。

## 性能优化特性

### 1. UpdateId 机制
每帧递增 UpdateId，防止单帧内重复执行逻辑。

### 2. 脏值跟踪
使用 HashSet 跟踪需要更新的元素，批量应用变化。

### 3. 延迟回调
支持延迟到下一帧或重绘时执行的回调。

### 4. Request 机制
安全地修改树结构，避免在绘制过程中修改导致的问题。

### 5. 对象池
使用事件池减少 allocations，提升性能。

## 下一步

- 了解 [元素系统](./02-ElementSystem.md) 的详细设计
- 了解 [元素树系统](./03-ElementTree.md) 的工作原理
- 了解 [值条目系统](./04-ValueEntrySystem.md) 的值管理机制

---

最后更新: 2026-01-03
