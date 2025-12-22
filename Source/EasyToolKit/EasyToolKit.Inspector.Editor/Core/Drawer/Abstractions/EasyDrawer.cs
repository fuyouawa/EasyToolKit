using System;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyDrawer : HandlerBase, IEasyDrawer
    {
        private bool _isInitialized;

        public bool SkipWhenDrawing { get; set; }

        protected virtual bool CanDraw(IElement element)
        {
            return true;
        }

        protected bool CallNextDrawer(GUIContent label)
        {
            var chain = Element.GetDrawerChain();
            if (chain.MoveNext() && chain.Current != null)
            {
                chain.Current.Draw(label);
                return true;
            }
            return false;
        }

        protected virtual void Draw(GUIContent label)
        {
        }

        protected override bool CanHandle(IElement element)
        {
            return CanDraw(element);
        }

        protected virtual void Initialize()
        {
        }

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        void IEasyDrawer.Draw(GUIContent label)
        {
            EnsureInitialize();
            Draw(label);
        }
    }
}
