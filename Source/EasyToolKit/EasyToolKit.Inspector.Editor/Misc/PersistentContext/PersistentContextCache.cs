using System;
using System.Collections.Generic;
using System.IO;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Core.Editor.Internal;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEditor;
using UnityEngine;
using SerializationUtility = EasyToolKit.ThirdParty.OdinSerializer.SerializationUtility;

namespace EasyToolKit.Inspector.Editor
{
    class PersistentContextDirectory : Dictionary<string, GlobalPersistentContext>
    {
    }

    [InitializeOnLoad]
    public class PersistentContextCache : Singleton<PersistentContextCache>
    {
        private static readonly string CacheFileName = "PersistentContextCache.bytes";
        private static string s_cacheFilePath;
        private static string s_cacheDirectory;

        private static string CacheDirectory
        {
            get
            {
                if (s_cacheDirectory == null)
                    s_cacheDirectory = EditorAssetPaths.GetModuleTemporaryDirectory("Inspector");
                return s_cacheDirectory;
            }
        }
        private static string CacheFilePath
        {
            get
            {
                if (s_cacheFilePath == null)
                    s_cacheFilePath = CacheDirectory + "/" + CacheFileName;
                return s_cacheFilePath;
            }
        }

        private PersistentContextDirectory _cache;
        private float _cacheMemorySize;
        private bool _initialized = false;
        private DateTime _lastSaveTime = DateTime.MinValue;
        private double _lastUpdateTime;

        static PersistentContextCache()
        {
            UnityEditorEventUtility.DelayAction(() => Instance.EnsureInitialize());
        }

        private PersistentContextCache()
        {
            _cache = new PersistentContextDirectory();
        }

        private void EnsureInitialize()
        {
            if (_initialized) return;
            _initialized = true;

            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
            AppDomain.CurrentDomain.DomainUnload -= OnDomainUnload;
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            LoadCache();
        }

        public GlobalPersistentContext<TValue> GetContext<TValue>(string key, out bool isNew)
        {
            EnsureInitialize();

            if (_cache.TryGetValue(key, out var originCtx) && originCtx is GlobalPersistentContext<TValue> castedCtx)
            {
                isNew = false;
                return castedCtx;
            }

            var context = GlobalPersistentContext<TValue>.Create();
            _cache[key] = context;
            isNew = true;
            return context;
        }

        private void OnDomainUnload(object sender, EventArgs e)
        {
            SaveCache();
        }

        private void OnUpdate()
        {
            if (EditorApplication.timeSinceStartup - _lastUpdateTime < 1.0)
                return;
            _lastUpdateTime = EditorApplication.timeSinceStartup;
            //TODO clear unused cache
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            var now = DateTime.Now;
            if (now - _lastSaveTime > TimeSpan.FromSeconds(1))
            {
                _lastSaveTime = now;
                SaveCache();
            }
        }

        void LoadCache()
        {
            var fileInfo = new FileInfo(CacheFilePath);
            try
            {
                if (!fileInfo.Exists)
                {
                    _cache.Clear();
                    return;
                }
                using var fileStream = fileInfo.OpenRead();
                _cache = SerializationUtility.DeserializeValue<PersistentContextDirectory>(fileStream, DataFormat.Binary);
                _cache ??= new PersistentContextDirectory();
                _cacheMemorySize = fileInfo.Length;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void SaveCache()
        {
            var fileInfo = new FileInfo(CacheFilePath);
            try
            {
                if (_cache.Count == 0)
                {
                    if (!fileInfo.Exists)
                        return;
                    DeleteCache();
                    return;
                }

                if (!Directory.Exists(CacheDirectory))
                    Directory.CreateDirectory(CacheDirectory);
                using var fileStream = fileInfo.OpenWrite();
                SerializationUtility.SerializeValue(_cache, fileStream, DataFormat.Binary, out var unityReferences);
                if (unityReferences.IsNotNullOrEmpty())
                {
                    Debug.Log($"Reference unity objects({string.Join(", ", unityReferences)}) in persistent context is not supported.");
                }
                _cacheMemorySize = fileInfo.Length;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void DeleteCache()
        {
            _cacheMemorySize = 0;
            _cache.Clear();
            if (File.Exists(CacheFilePath))
                File.Delete(CacheFilePath);
        }
    }
}
