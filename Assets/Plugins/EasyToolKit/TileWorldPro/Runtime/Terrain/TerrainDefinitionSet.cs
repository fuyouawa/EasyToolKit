using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;

namespace EasyToolKit.TileWorldPro
{
    [ShowOdinSerializedPropertiesInInspector]
    public class TerrainDefinitionSet : IEnumerable<TerrainDefinition>
    {
        [OdinSerialize]
        [LabelText("地形定义表")]
#if UNITY_EDITOR
        [ValueDropdown(nameof(TerrainDefinitionItemDropdownList))]
#endif
        [MetroListDrawerSettings(ShowIndexLabel = false)]
        private List<TerrainDefinitionNode> _nodes;

        private Dictionary<Guid, TerrainDefinition> _definitionsCacheByGuid;

        public IReadOnlyList<TerrainDefinitionNode> Nodes => _nodes;

        public IEnumerator<TerrainDefinition> GetEnumerator()
        {
            foreach (var item in _nodes)
            {
                if (item is TerrainDefinition definition)
                {
                    yield return definition;
                }
                else if (item is TerrainDefinitionGroup group)
                {
                    foreach (var definition2 in group)
                    {
                        yield return definition2;
                    }
                }
            }
        }

        public IEnumerable<TerrainDefinitionPathInfo> EnumeratePathInfo()
        {
            foreach (var item in _nodes)
            {
                foreach (var pathInfo in EnumeratePathInfoImpl(item, new List<string>()))
                {
                    yield return pathInfo;
                }
            }
        }

        public TerrainDefinition GetByGuid(Guid guid)
        {
            if (TryGetByGuid(guid, out var definition))
            {
                return definition;
            }
            throw new KeyNotFoundException($"Could not find terrain definition by guid '{guid}'");
        }

        public bool TryGetByGuid(Guid guid, out TerrainDefinition definition)
        {
            _definitionsCacheByGuid ??= new Dictionary<Guid, TerrainDefinition>();
            if (_definitionsCacheByGuid.TryGetValue(guid, out definition))
            {
                return true;
            }

            foreach (var def in this)
            {
                if (def.Guid == guid)
                {
                    definition = def;
                    _definitionsCacheByGuid[guid] = def;
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<TerrainDefinitionPathInfo> EnumeratePathInfoImpl(TerrainDefinitionNode item, List<string> currentPath)
        {
            currentPath.Add(item.Name);

            if (item is TerrainDefinition definition)
            {
                yield return new TerrainDefinitionPathInfo
                {
                    Definition = definition,
                    Path = new List<string>(currentPath)
                };
            }
            else if (item is TerrainDefinitionGroup group)
            {
                foreach (var childItem in group.Nodes)
                {
                    foreach (var pathInfo in EnumeratePathInfoImpl(childItem, currentPath))
                    {
                        yield return pathInfo;
                    }
                }
            }

            currentPath.RemoveAt(currentPath.Count - 1); // 回溯
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#if UNITY_EDITOR
        private static readonly ValueDropdownList<TerrainDefinitionNode> TerrainDefinitionItemDropdownList = new()
        {
            new DelayedValueDropdownItem("分组", () => new TerrainDefinitionGroup()),
            new DelayedValueDropdownItem("地形", () => new TerrainDefinition()),
            new DelayedValueDropdownItem("复合地形", () => new TerrainDefinition(true)),
        };
#endif
    }
}
