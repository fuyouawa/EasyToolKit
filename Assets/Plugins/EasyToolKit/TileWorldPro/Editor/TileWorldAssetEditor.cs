using System;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    [CustomEditor(typeof(TileWorldAsset))]
    public class TileWorldAssetEditor : EasyEditor
    {
        public static bool IsInDesigner = false;

        private static ITileWorldDataStore s_temporaryDataStore;
        private static readonly GUIContent TempContent = new GUIContent();

        private TileWorldAsset _target;

        private IElement _baseRangeElement;
        private IElement _tileSizeElement;
        private IElement _chunkSizeElement;
        private IElement _terrainDefinitionSetElement;
        private IElement _dataStoreElement;
        private bool _isInitialized = false;

        private LocalPersistentContext<bool> _dataStoreExpanded;

        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (TileWorldAsset)target;

            _dataStoreExpanded = Tree.Root.GetPersistentContext(nameof(_dataStoreExpanded), true);
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw();

            if (!_isInitialized)
            {
                _baseRangeElement = Tree.Root.Children!["_baseRange"];
                _tileSizeElement = Tree.Root.Children["_tileSize"];
                _chunkSizeElement = Tree.Root.Children["_chunkSize"];
                _terrainDefinitionSetElement = Tree.Root.Children["_terrainDefinitionSet"];
                _dataStoreElement = Tree.Root.Children["_dataStore"];
                _isInitialized = true;
            }

            _baseRangeElement.Draw();
            _tileSizeElement.Draw();
            _chunkSizeElement.Draw();

            if (IsInDesigner)
            {
                _terrainDefinitionSetElement.Draw();
            }

            var dataStoreNames = TileWorldDataStoreUtility.GetDataStoreNamesCache();
            int selectedDataStoreIndex = -1;
            if (_target.DataStore != null)
            {
                var dataStoreName = TileWorldDataStoreUtility.GetDataStoreNameByType(_target.DataStore.GetType());
                selectedDataStoreIndex = Array.IndexOf(dataStoreNames, dataStoreName);
            }

            EditorGUI.BeginChangeCheck();
            selectedDataStoreIndex = EasyEditorGUI.ValueDropdown(TempContent.SetText("数据存储方案"), selectedDataStoreIndex,
                dataStoreNames, (index, value) => new GUIContent(value));
            if (EditorGUI.EndChangeCheck())
            {
                if (selectedDataStoreIndex != -1)
                {
                    var dataStoreType =
                        TileWorldDataStoreUtility.GetDataStoreTypeByName(dataStoreNames[selectedDataStoreIndex]);
                    s_temporaryDataStore = _target.DataStore;
                    _target.DataStore = Activator.CreateInstance(dataStoreType) as ITileWorldDataStore;
                }
                else
                {
                    _target.DataStore = null;
                }

                _dataStoreElement.Refresh();
            }

            if (_target.DataStore != null)
            {
                EasyEditorGUI.BeginBox();
                EasyEditorGUI.BeginBoxHeader();
                _dataStoreExpanded.Value = EasyEditorGUI.Foldout(_dataStoreExpanded.Value,
                    TempContent.SetText($"数据存储 - {dataStoreNames[selectedDataStoreIndex]}"));
                EasyEditorGUI.EndBoxHeader();

                if (_dataStoreExpanded.Value)
                {
                    _dataStoreElement.Draw(null);
                }

                EasyEditorGUI.EndBox();

                if (GUILayout.Button("清理无效地形"))
                {
                    _target.ClearInvalidTerrains();
                    _dataStoreElement.CastValue().ValueEntry.MarkDirty();
                }

                if (s_temporaryDataStore != null)
                {
                    if (GUILayout.Button("转移数据"))
                    {
                        _target.DataStore.TransferData(s_temporaryDataStore);
                        s_temporaryDataStore.Dispose();
                        s_temporaryDataStore = null;
                    }
                }
            }

            Tree.EndDraw();
        }
    }
}
