using System;
using System.Collections.Generic;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;

namespace EasyToolKit.TileWorldPro
{
    public class TileBuildPipline
    {
        [LabelText("地形构建处理器管线")]
        [MetroListDrawerSettings(ShowIndexLabel = false)]
#if UNITY_EDITOR
        [ValueDropdown(nameof(ProcessorDropdownList))]
#endif
        [OdinSerialize, ShowInInspector] private List<ITileBuildProcessor> _processors;

        public void BeforeInstantiateTile(BeforeBuildTileEvent e)
        {
            foreach (var processor in _processors)
            {
                processor.OnBeforeBuildTile(e);
            }
        }

        public void AfterInstantiateTile(AfterBuildTileEvent e)
        {
            foreach (var processor in _processors)
            {
                processor.OnAfterBuildTile(e);
            }
        }

#if UNITY_EDITOR
        private ValueDropdownList<ITileBuildProcessor> ProcessorDropdownList => TileBuildProcessorUtility.GetProcessorDropdownList();
#endif
    }
}
