using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class TerrainDefinitionGroup : TerrainDefinitionNode, IEnumerable<TerrainDefinition>
    {
        [EndFoldoutGroup]
        [LabelText("地形定义表")]
#if UNITY_EDITOR
        [ValueDropdown(nameof(TerrainDefinitionNodeDropdownList))]
#endif
        [ListDrawerSettings(ShowIndexLabel = false)]
        [OdinSerialize] private List<TerrainDefinitionNode> _nodes;

        public override TerrainDefinitionNodeType NodeType => TerrainDefinitionNodeType.Group;
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


#if UNITY_EDITOR
        private static readonly ValueDropdownList<TerrainDefinitionNode> TerrainDefinitionNodeDropdownList = new()
        {
            new DelayedValueDropdownItem("分组", () => new TerrainDefinitionGroup()),
            new DelayedValueDropdownItem("地形", () => new TerrainDefinition()),
            new DelayedValueDropdownItem("复合地形", () => new TerrainDefinition(true)),
        };
#endif
    }
}
