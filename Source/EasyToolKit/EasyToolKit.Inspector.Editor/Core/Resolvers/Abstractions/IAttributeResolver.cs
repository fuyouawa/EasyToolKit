using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IAttributeResolver : IResolver
    {
        ElementAttributeInfo[] GetAttributeInfos();
    }
}
