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

### Extension Framework
- **MonoBehaviour Extensions** - `CallInNextFrame()` and other convenience methods
- **Collection Extensions** - LINQ-style extensions for Unity collections
- **Math Extensions** - Vector, Rect, and mathematical operations
- **Unity Object Extensions** - GameObject, Component, and Scene utilities

### Inspector System
- **Attributes** (`EasyToolKit.Inspector`) - Conditional property display, UI organization
- **Drawers** (`EasyToolKit.Inspector.Editor`) - Custom property drawers for inspector attributes

**Documentation**: For detailed information about Inspector.Editor architecture and structure, refer to [EasyToolKit.Inspector.Editor/docs/](EasyToolKit.Inspector.Editor/docs/)

## Development Guidelines

### Code Organization
- **Runtime code** goes in Core project
- **Editor code** goes in Editor projects (conditionally compiled with `UNITY_EDITOR`)
- **Inspector attributes** defined in Inspector project
- **Inspector drawers** implemented in Inspector.Editor project

### Coding Standards

#### Class Member Order

**Normal Classes** (top to bottom):
1. Constants (`const`)
2. Static readonly fields (`static readonly`)
3. Static fields (`static`)
4. Instance fields (`private`/`protected`)
5. Constructors
6. Properties
7. Methods

**Access Modifier Order**: `public` → `protected` → `private`

**Unity Classes** (top to bottom):
1. Constants/static fields
2. Serialized fields (`[SerializeField]`)
3. Private fields
4. Public properties
5. Unity lifecycle methods (in calling order)
6. Public methods
7. Private methods
8. Coroutines

**Serialized Fields Best Practices**:
- Use `[SerializeField]` with `private` access modifier
- Provide public properties for external access with validation logic

#### Member Variable Access Guidelines

1. **Pure data properties** - Prefer direct field access within class
2. **Properties with logic** - Always use property access to ensure validation/events execute

```csharp
// Pure data - direct field access OK internally
public int PlayerId => _playerId;  // Use _playerId internally

// Properties with logic - must use property access
public float Health
{
    get => _health;
    private set
    {
        _health = Mathf.Clamp(value, 0, _maxHealth);
        OnHealthChanged?.Invoke(_health);  // Logic must execute
    }
}
```

#### Naming Conventions

**General Rules**:
- Avoid abbreviations except common ones
- Use descriptive, self-explanatory names

**Collections** (plural form):

**Dictionary** (ValueByKey pattern):
- `XxxByYyy` where Xxx is value (singular unless collection), Yyy is key
- Semantic: "Xxx obtained by Yyy"

**TaskCompletionSource**: Action semantic + `Tcs` suffix

**Events**: `Xxxing` / `Xxxed` (without `On` prefix)

**Event Naming Principles**:
- Event names describe "what happened", not "what to do"
- ✅ `Closed`, `Clicked`, `Completed`
- ❌ `Close`, `Click`, `Complete`
- No `On` prefix (reserved for trigger methods)
- `Xxxing` → "in progress, can be interrupted"
- `Xxxed` → "completed, notifying result"

**Delegates**: Use suffix based on usage scenario

- **XxxHandler**: Event/message handling (`WindowClosedHandler`)
- **XxxInvoker**: Command execution (`CommandInvoker`)
- **XxxResolver**: Dependency/service resolution (`DependencyResolver`)
- **XxxFactory**: Object creation (`GameObjectFactory`)
- **XxxGetter**: Property/data retrieval (`PropertyGetter`)
- **XxxConverter**: Type conversion (`TypeConverter<TInput, TOutput>`)
- **XxxPredicate**: Condition checking (returns `bool`)
- **XxxEvaluator**: Calculation/scoring (`DamageEvaluator`)

**Comparator Naming**:
- Lambda expressions: Use `a, b` as parameters
- Public methods: Use `left, right` as parameters

#### Function Naming Conventions

**Get/Find/TryGet Prefixes**:

- **Get**: Expected to find target, throws if not found
  - Usually O(1) complexity
  - Target must exist

- **Find**: May need search/iteration, returns null if not found
  - Time complexity may exceed O(1)
  - Returns null on failure

- **TryGet**: Safe version, returns bool success/failure
  - Result via `out` parameter
  - Never throws for not found


**Compute/Calculate Prefixes**:

- **Compute**: Algorithmic/logical derivation (multi-step, complex logic)
  - Configuration-based derivation
  - Path planning, state evaluation

- **Calculate**: Mathematical formula-based (direct calculation)
  - Numerical formulas, ratios
  - Damage, stat growth

**Can/Is Prefixes**:

- **Can**: "Can it?" -判断操作是否可行
- **Is**: "Is it?" - 判断状态或属性

```csharp
public bool CanUseSkill(Player player, Skill skill);  // Capability check
public bool IsPlayerOnline(int playerId);               // Status check
```

**Function Suffixes**:

- **ByXxx**: "According to Xxx..."
  - `ById`, `ByName`, `ByKey`, `ByLevel`, `ByStatus`

- **AtXxx**: "At Xxx location/time..."
  - `AtPosition`, `AtCoordinate`, `AtIndex`, `AtTime`

- **OfXxx**: "Belonging to Xxx..." or "With Xxx property..."
  - `OfPlayer`, `OfTeam`, `OfType`, `OfCategory`

**When to Use Suffixes**:

Required:
- Multiple lookup keys for same entity: `GetPlayerById`, `GetPlayerByName`
- Same parameter type, different semantics: `GetItemById`, `GetItemByIndex`

Optional (omit when context is clear):
- Single lookup method in context-specific class
- Semantically unambiguous scenario

**Uncertainty Principle**: If unsure whether API expansion will cause ambiguity, use the suffix.

#### Comment Standards

**Language Principles**:

  - Must use English comments
  - All public APIs require complete English XML documentation

**XML Documentation Comments**:

Use `<see cref=""/>` to reference other types, methods, properties, or fields:

**Best Practices**:
- Always use `<see cref=""/>` instead of plain text for type references
- Enables IDE navigation and documentation generation
- Improves code maintainability

**Documentation Requirements**:

**Public APIs** - Must include:
1. `<summary>` - Functionality description
2. `<param>` - All parameters
3. `<returns>` - Return value (non-void)
4. `<remarks>` - Additional notes (if needed)
5. `<exception>` - User-relevant exceptions only

**Private Methods**:
- Simple methods with clear names: no comment needed
- Complex methods: add XML comment explaining logic

```csharp
// No comment needed - name is self-explanatory
private void ResetHealth()
{
    _health = MaxHealth;
}

/// <summary>
/// Calculates damage mitigation using diminishing returns formula.
/// Formula: Mitigation = Armor / (Armor + 100).
/// </summary>
private float CalculateDamageMitigation(float armor, float resistance)
{
    // Complex logic...
}
```

**Special Comment Tags**:

- `TODO(username): Description`
- `FIXME(username): Description`
- `HACK(username): Reason`
- `NOTE: Description`
- `[Obsolete]` attribute + XML comment

**Comment Best Practices**:
- Explain "why" not "what"
- Keep comments synchronized with code
- Use clear grammar and spelling
- Document key decisions
- Don't comment obvious code
- Don't use comments to hide code problems

## Development Workflow

1. **Build**: Build solution in Visual Studio/Rider
2. **Integration**: Assemblies automatically copied to Unity Plugins folder
3. **Testing**: Use Unity's Play Mode for runtime testing, Editor Mode for inspector testing
4. **Documentation**: XML documentation generated automatically

## Architecture Patterns Used

- **Singleton Pattern**: Managers and global services
- **Event-Driven Architecture**: Decoupled communication
- **State Pattern**: Flexible state machine implementation
- **Observer Pattern**: Data binding with change notifications
- **Extension Methods**: Unity API enhancements