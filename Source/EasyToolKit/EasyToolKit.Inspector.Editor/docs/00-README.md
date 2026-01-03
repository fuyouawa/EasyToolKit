# EasyToolKit.Inspector.Editor 架构设计文档

本文档目录详细描述了 `EasyToolKit.Inspector.Editor` 项目的架构设计、模块关系和实现细节。

## 文档导航

### 核心概念

- **[01-Overview](./01-Overview.md)** - 项目概述和整体架构
  - 核心特性
  - 整体架构图
  - 设计模式概览

### 核心系统

- **[02-ElementSystem](./02-ElementSystem.md)** - 元素系统
  - IElement 基础接口
  - IValueElement 值元素
  - ILogicalElement 逻辑元素
  - IGroupElement 分组元素
  - 元素角色和生命周期

- **[03-ElementTree](./03-ElementTree.md)** - 元素树系统
  - ElementTree 管理
  - 元素工厂（ElementFactory）
  - 配置和定义系统
  - InspectorElements 静态工厂

- **[04-ValueEntrySystem](./04-ValueEntrySystem.md)** - 值条目系统
  - IValueEntry 接口
  - IValueAccessor 值访问器
  - IValueState 值状态
  - IValueChangeHandler 值变化处理
  - ValueOperation 值操作

### 解析和绘制

- **[05-ResolverSystem](./05-ResolverSystem.md)** - 解析器系统
  - IStructureResolver 结构解析器
  - IValueOperationResolver 值操作解析器
  - IAttributeResolver 属性解析器
  - 解析器工厂和优先级

- **[06-DrawerSystem](./06-DrawerSystem.md)** - 绘制器系统
  - IEasyDrawer 接口
  - DrawerChain 责任链
  - EasyDrawer 基类
  - EasyValueDrawer 泛型值绘制器
  - 绘制器优先级

- **[07-PostProcessorSystem](./07-PostProcessorSystem.md)** - 后处理器系统
  - IPostProcessor 接口
  - PostProcessorChain 处理链
  - GroupElementPostProcessor 分组后处理器
  - LogicalElementPostProcessor 逻辑元素后处理器

### 扩展指南

- **创建自定义绘制器** - 参见 [06-DrawerSystem](./06-DrawerSystem.md)
- **创建自定义解析器** - 参见 [05-ResolverSystem](./05-ResolverSystem.md)
- **创建自定义元素** - 参见 [02-ElementSystem](./02-ElementSystem.md)

## 快速开始

### 理解模块关系

```
ElementTree (元素树管理)
    ↓
    ├─→ Element (元素)
    │    ├─→ ValueElement (值元素)
    │    │    └─→ ValueEntry (值条目) → Operation (操作) → OperationResolver
    │    │
    │    └─→ LogicalElement (逻辑元素)
    │         └─→ LogicalChildren (逻辑子元素) → StructureResolver
    │
    ├─→ Drawer (绘制器) → DrawerChain → DrawerChainResolver
    │
    └─→ GroupElement (分组元素) → PostProcessor → PostProcessorChainResolver
```

### 核心流程

1. **元素创建流程**: InspectorElements → Configurator → Definition → ElementFactory → Element
2. **结构解析流程**: LogicalElement → StructureResolver → LogicalChildren
3. **值操作流程**: ValueElement → ValueEntry → OperationResolver → Operation
4. **绘制流程**: Element → DrawerChainResolver → DrawerChain → Drawer.Draw()
5. **后处理流程**: Element → PostProcessorChainResolver → PostProcessorChain → PostProcessor.Process()

## 设计模式

- **工厂模式** - ElementFactory, ResolverFactory
- **责任链模式** - DrawerChain, PostProcessorChain
- **策略模式** - Resolver系统, Drawer系统
- **观察者模式** - 值变化事件
- **建造者模式** - ElementConfiguration
- **组合模式** - Element树形结构
- **对象池模式** - 事件池

## 相关文件

- [../../CLAUDE.md](../../CLAUDE.md) - 项目开发规范和指南
- [ArchitectureDesign.md](../ArchitectureDesign.md) - 原始架构文档（已归档）

---

最后更新: 2026-01-03
