using System;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector
{
    public abstract class InspectorAttribute : Attribute
    {
        private string _base64;

        private string Base64
        {
            get
            {
                if (_base64 == null)
                {
                    var serializationData = new EasySerializationData();
                    EasySerialize.To(this, ref serializationData);
                    _base64 = Convert.ToBase64String(serializationData.BinaryData);
                }
                return _base64;
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

            return Base64 == ((InspectorAttribute)obj).Base64;
        }

        public override int GetHashCode()
        {
            return Base64.GetHashCode();
        }
    }
}
