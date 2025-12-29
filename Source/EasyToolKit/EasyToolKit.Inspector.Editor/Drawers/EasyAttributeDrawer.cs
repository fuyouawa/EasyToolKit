using System;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyAttributeDrawer<TAttribute> : EasyDrawer
        where TAttribute : Attribute
    {
        private TAttribute _attribute;

        public new ILogicalElement Element => (ILogicalElement)base.Element;

        public TAttribute Attribute
        {
            get
            {
                if (_attribute == null)
                {
                    _attribute = Element.GetAttribute<TAttribute>();
                }

                return _attribute;
            }
        }

        protected override bool CanDraw(IElement element)
        {
            if (element is ILogicalElement logicalElement)
            {
                return CanDrawElement(logicalElement);
            }
            return false;
        }

        protected virtual bool CanDrawElement(ILogicalElement element)
        {
            return true;
        }
    }

    public abstract class EasyAttributeDrawer<TAttribute, TValue> : EasyAttributeDrawer<TAttribute>
        where TAttribute : Attribute
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
    }
}
