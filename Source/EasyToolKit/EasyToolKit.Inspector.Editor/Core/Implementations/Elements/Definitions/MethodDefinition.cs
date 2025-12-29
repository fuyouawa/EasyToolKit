using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Method definition implementation for function handling in the inspector.
    /// Provides metadata for methods that can be invoked or displayed in the inspector interface.
    /// </summary>
    public sealed class MethodDefinition : ElementDefinition, IMethodDefinition
    {
        /// <summary>
        /// Gets or sets the <see cref="MethodInfo"/> that describes the method.
        /// </summary>
        public MethodInfo MethodInfo { get; set; }

        /// <inheritdoc/>
        public MemberInfo MemberInfo => MethodInfo;
    }
}
