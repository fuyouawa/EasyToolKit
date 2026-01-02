using System;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector
{
    public abstract class InspectorAttribute : Attribute
    {
        private string _id;

        private string Id
        {
            get
            {
                if (_id == null)
                {
                    var serializationData = new EasySerializationData();
                    EasySerialize.To(this, ref serializationData);
                    _id = GetType().FullName + "+" + Convert.ToBase64String(serializationData.BinaryData);
                }
                return _id;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Id == ((InspectorAttribute)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
