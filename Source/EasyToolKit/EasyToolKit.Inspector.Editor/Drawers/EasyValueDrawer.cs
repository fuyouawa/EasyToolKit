using System;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyValueDrawer<TValue> : EasyDrawer
    {
        private IValueEntry<TValue> _valueEntry;

        public new IValueElement Element => base.Element as IValueElement;

        public IValueEntry<TValue> ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    _valueEntry = Element.ValueEntry as IValueEntry<TValue>;
                }

                return _valueEntry;
            }
        }

        protected sealed override bool CanDraw(IElement element)
        {
            if (element is IValueElement valueElement)
            {
                var valueType = valueElement.ValueEntry.ValueType;
                return valueType == typeof(TValue) &&
                       CanDrawValueType(valueType) &&
                       CanDrawElement(valueElement);
            }
            return false;
        }

        protected virtual bool CanDrawValueType(Type valueType)
        {
            return true;
        }

        protected virtual bool CanDrawElement(IValueElement element)
        {
            return true;
        }
    }
}
