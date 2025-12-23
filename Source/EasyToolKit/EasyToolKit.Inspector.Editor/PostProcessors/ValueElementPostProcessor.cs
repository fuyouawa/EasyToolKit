using System;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class ValueElementPostProcessor<TValue> : ElementPostProcessor
    {
        protected virtual bool CanProcessValueType(Type valueType)
        {
            return true;
        }

        protected virtual bool CanProcessElement(IValueElement element)
        {
            return true;
        }

        protected abstract void Process(IValueElement element);

        protected override bool CanProcess(IElement element)
        {
            if (element is IValueElement valueElement)
            {
                var valueType = valueElement.ValueEntry.ValueType;
                return valueType == typeof(TValue) &&
                       CanProcessValueType(valueType) &&
                       CanProcessElement(valueElement);
            }
            return false;
        }

        protected sealed override void Process(IElement element)
        {
            Process((IValueElement)element);
        }
    }
}
