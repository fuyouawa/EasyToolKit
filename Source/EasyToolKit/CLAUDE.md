# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

EasyToolKit is a Unity tooling framework organized as a Visual Studio solution with four C# projects:

- **EasyToolKit.Core** - Core runtime functionality (event system, state machine, data binding, utilities)
- **EasyToolKit.Core.Editor** - Editor extensions for core functionality
- **EasyToolKit.Inspector** - Inspector attribute definitions
- **EasyToolKit.Inspector.Editor** - Inspector drawer implementations

## Build System

### Building the Solution
- Target framework: `netstandard2.1`
- Build the solution in Visual Studio/Rider: `EasyToolKit.sln`
- Post-build events automatically copy assemblies to Unity's Plugins folder: `../../Assets/Plugins/EasyToolKit/Assemblies/`
- Documentation generation is enabled (XML docs)

### Dependencies
- **OdinSerializer**: Serialization library
- **xxHash**: Hashing library
- Unity engine modules (CoreModule, AnimationModule, AudioModule, etc.)

## Architecture

### Core Framework Components

**Event System** (`EasyToolKit.Core.Tools.Event`)
- `EasyEventManager` - Singleton event manager with type-safe event handling
- `IEasyEventListener`, `IEasyEventTrigger` - Event interfaces
- Thread-safe event trigger wrappers

**State Machine** (`EasyToolKit.Core.Tools.StateMachine`)
- `StateMachine<T>` - Generic state machine implementation
- `AbstractState`, `AbstractStateBehaviour` - Base state classes
- `FluentState` - Fluent API for state configuration

**Singleton Pattern** (`EasyToolKit.Core.Tools.Singleton`)
- `ScriptableObjectSingleton<T>` - ScriptableObject-based singletons
- Automatic asset path management and editor/runtime loading

**Data Binding** (`EasyToolKit.Core.Tools.Binding`)
- `BindableValue<T>`, `BindableList<T>`, `BindableArray<T>`
- Observable collections with change notifications

### Extension Framework
- **MonoBehaviour Extensions** - `CallInNextFrame()` and other convenience methods
- **Collection Extensions** - LINQ-style extensions for Unity collections
- **Math Extensions** - Vector, Rect, and mathematical operations
- **Unity Object Extensions** - GameObject, Component, and Scene utilities

### Inspector System
- **Attributes** (`EasyToolKit.Inspector.Attributes`) - Conditional property display, UI organization
- **Drawers** (`EasyToolKit.Inspector.Editor.Drawers`) - Custom property drawers for inspector attributes

## Development Guidelines

### Code Organization
- **Runtime code** goes in Core project
- **Editor code** goes in Editor projects (conditionally compiled with `UNITY_EDITOR`)
- **Inspector attributes** defined in Inspector project
- **Inspector drawers** implemented in Inspector.Editor project

### Coding Standards
**Class Member Order** (top to bottom):
1. Constants (`const`)
2. Static readonly fields (`static readonly`)
3. Static fields (`static`)
4. Instance fields (`private`/`protected`)
5. Constructors
6. Properties
7. Methods

**Access Modifier Order**: `public` → `protected` → `private`

**Unity Class Special Order**:
1. Constants/static fields
2. Serialized fields (`[SerializeField]`)
3. Private fields
4. Public properties
5. Unity lifecycle methods (in calling order)
6. Public methods
7. Private methods
8. Coroutines

### Naming Conventions
- **Events**: `Xxxing`/`Xxxed` (without `On` prefix)
- **Delegates**: `XxxHandler` suffix
- **Dictionary**: `ValueByKey` pattern (e.g., `playerById`)
- **TaskCompletionSource**: Action semantic + `Tcs` suffix

### Function Prefix Rules
- **Get**: Expected to find target, throws exception if not found
- **Find**: May need search/iteration, returns null if not found
- **TryGet**: Safe version, returns bool success/failure
- **Compute**: Algorithmic derivation with complex logic
- **Calculate**: Mathematical formula-based calculation

## Development Workflow

1. **Build**: Build solution in Visual Studio/Rider
2. **Integration**: Assemblies automatically copied to Unity Plugins folder
3. **Testing**: Use Unity's Play Mode for runtime testing, Editor Mode for inspector testing
4. **Documentation**: XML documentation generated automatically

## Key Files for Reference

- `EasyToolKit.Core/Tools/Singleton/ScriptableObjectSingleton.cs` - Singleton pattern
- `EasyToolKit.Core/Tools/Event/EasyEventManager.cs` - Event system
- `EasyToolKit.Core/Tools/StateMachine/StateMachine.cs` - State machine
- `EasyToolKit.Inspector/Attributes/ShowIfAttribute.cs` - Conditional inspector attributes
- `EasyToolKit.Inspector.Editor/Drawers/EasyDrawer.cs` - Base drawer class

## Architecture Patterns Used

- **Singleton Pattern**: Managers and global services
- **Event-Driven Architecture**: Decoupled communication
- **State Pattern**: Flexible state machine implementation
- **Observer Pattern**: Data binding with change notifications
- **Extension Methods**: Unity API enhancements

## Unity Integration

- Assemblies built as separate DLLs for modular loading
- Editor assemblies conditionally compiled with `UNITY_EDITOR` directives
- ScriptableObject-based singletons for asset management
- Main thread dispatching for UI operations
- Serialization support via OdinSerializer