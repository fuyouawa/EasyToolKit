// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Generic;
using EasyToolKit.ThirdParty.Scriban.Syntax;

namespace EasyToolKit.ThirdParty.Scriban.Runtime
{
    internal class ScriptPipeArguments : Stack<ScriptExpression>
    {
        public ScriptPipeArguments()
        {
        }

        public ScriptPipeArguments(int capacity) : base(capacity)
        {
        }
    }
}