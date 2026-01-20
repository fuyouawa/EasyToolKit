using System;
using System.Collections.Generic;

namespace Tests.Editor.Inspector.Core
{
    /// <summary>
    /// Simple test class with a single field.
    /// </summary>
    [Serializable]
    public class SingleFieldClass
    {
        public int TestInt;
    }

    /// <summary>
    /// Test class with multiple fields of different types.
    /// </summary>
    [Serializable]
    public class MultipleFieldsClass
    {
        public int IntField;
        public float FloatField;
        public string StringField;
        public bool BoolField;
    }

    /// <summary>
    /// Test class with properties.
    /// </summary>
    [Serializable]
    public class PropertyClass
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }

    /// <summary>
    /// Test class with both fields and properties.
    /// </summary>
    [Serializable]
    public class MixedMembersClass
    {
        public int PublicField;
        private int _privateField;
        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }
    }

    /// <summary>
    /// Test class with nested object.
    /// </summary>
    [Serializable]
    public class NestedClass
    {
        public int Id;
        public SingleFieldClass NestedObject;
    }

    /// <summary>
    /// Test class with list field.
    /// </summary>
    [Serializable]
    public class ListFieldClass
    {
        public List<int> Integers;
    }
}
