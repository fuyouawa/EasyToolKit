// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace EasyToolKit.ThirdParty.Scriban.Syntax
{
#if SCRIBAN_PUBLIC
    public
#else
    internal
#endif
    interface IScriptCustomType : IScriptCustomTypeInfo, IScriptCustomBinaryOperation, IScriptCustomUnaryOperation, IScriptConvertibleTo
    {
    }
}