using System;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyDrawer : IEasyDrawer
    {
        private bool _isInitialized;
        private IElement _element;
        private DrawerChain _chain;

        IElement IHandler.Element
        {
            get => _element;
            set => _element = value;
        }

        DrawerChain IEasyDrawer.Chain
        {
            get => _chain;
            set => _chain = value;
        }

        public IElement Element => _element;
        public DrawerChain Chain => _chain;
        public bool SkipWhenDrawing { get; set; }

        protected virtual bool CanDraw(IElement element)
        {
            return true;
        }

        protected bool CallNextDrawer(GUIContent label)
        {
            if (_chain.MoveNext() && _chain.Current != null)
            {
                _chain.Current.Draw(label);
                return true;
            }
            return false;
        }

        protected virtual void Draw(GUIContent label)
        {
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

        bool IHandler.CanHandle(IElement element)
        {
            return CanDraw(element);
        }

        void IEasyDrawer.Draw(GUIContent label)
        {
            EnsureInitialize();
            Draw(label);
        }
    }
}
