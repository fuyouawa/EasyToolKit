using System;
using System.Collections.Generic;
using EasyToolKit.Serialization;

namespace Tests.Serialization
{
    /// <summary>Test enum for serialization testing.</summary>
    public enum TestEnum
    {
        OptionA = 0,
        OptionB = 1,
        OptionC = 2
    }

    /// <summary>Test data class for serialization testing.</summary>
    [Serializable]
    public class TestDataClass
    {
        public int Id;
        public string Name;
        public float Health;
        public bool IsActive;
        public UnityEngine.Vector3 Position;
        public List<int> Scores;
    }


    /// <summary>Test class with default member flags (AllFields).</summary>
    [EasySerializable]
    public class DefaultMemberFlagsClass
    {
        public int publicField;
        [UnityEngine.SerializeField] private int privateField;
        [UnityEngine.SerializeField] protected int protectedField;
        [UnityEngine.SerializeField] internal int internalField;

        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }
        protected int ProtectedProperty { get; set; }

        public DefaultMemberFlagsClass()
        {
        }

        public DefaultMemberFlagsClass(
            int publicField, int privateField, int protectedField, int internalField,
            int publicProperty, int privateProperty, int protectedProperty)
        {
            this.publicField = publicField;
            this.privateField = privateField;
            this.protectedField = protectedField;
            this.internalField = internalField;
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
            this.ProtectedProperty = protectedProperty;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
        public int GetProtectedField() => protectedField;
        public int GetPrivateProperty() => PrivateProperty;
        public int GetProtectedProperty() => ProtectedProperty;
    }

    /// <summary>Test class with PublicFields only.</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.PublicFields)]
    public class PublicFieldsOnlyClass
    {
        public int publicField;
        private int privateField;
        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }

        public PublicFieldsOnlyClass()
        {
        }

        public PublicFieldsOnlyClass(int publicField, int privateField, int publicProperty, int privateProperty)
        {
            this.publicField = publicField;
            this.privateField = privateField;
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
        public int GetPrivateProperty() => PrivateProperty;
    }

    /// <summary>Test class with PublicProperties only.</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.PublicProperties)]
    public class PublicPropertiesOnlyClass
    {
        public int publicField;
        private int privateField;
        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }

        public PublicPropertiesOnlyClass()
        {
        }

        public PublicPropertiesOnlyClass(int publicField, int privateField, int publicProperty, int privateProperty)
        {
            this.publicField = publicField;
            this.privateField = privateField;
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
        public int GetPrivateProperty() => PrivateProperty;
    }

    /// <summary>Test class with AllPublic (fields and properties).</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.AllPublic)]
    public class AllPublicClass
    {
        public int publicField;
        private int privateField;
        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }

        public AllPublicClass()
        {
        }

        public AllPublicClass(int publicField, int privateField, int publicProperty, int privateProperty)
        {
            this.publicField = publicField;
            this.privateField = privateField;
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
        public int GetPrivateProperty() => PrivateProperty;
    }

    /// <summary>Test class with NonPublicFields only.</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.NonPublicFields)]
    public class NonPublicFieldsOnlyClass
    {
        public int publicField;
        [UnityEngine.SerializeField] private int privateField;

        public NonPublicFieldsOnlyClass()
        {
        }

        public NonPublicFieldsOnlyClass(int publicField, int privateField)
        {
            this.publicField = publicField;
            this.privateField = privateField;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
    }

    /// <summary>Test class with AllFields (public and non-public).</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.AllFields)]
    public class AllFieldsClass
    {
        public int publicField;
        [UnityEngine.SerializeField] private int privateField;
        [UnityEngine.SerializeField] protected int protectedField;

        public AllFieldsClass()
        {
        }

        public AllFieldsClass(int publicField, int privateField, int protectedField)
        {
            this.publicField = publicField;
            this.privateField = privateField;
            this.protectedField = protectedField;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
        public int GetProtectedField() => protectedField;
    }

    /// <summary>Test class with AllProperties (public and non-public).</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.AllProperties)]
    public class AllPropertiesClass
    {
        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }
        protected int ProtectedProperty { get; set; }

        public AllPropertiesClass()
        {
        }

        public AllPropertiesClass(int publicProperty, int privateProperty, int protectedProperty)
        {
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
            this.ProtectedProperty = protectedProperty;
        }

        // Getter methods for testing non-public members
        public int GetPrivateProperty() => PrivateProperty;
        public int GetProtectedProperty() => ProtectedProperty;
    }

    /// <summary>Test class with All members.</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.All)]
    public class AllMembersClass
    {
        public int publicField;
        [UnityEngine.SerializeField] private int privateField;
        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }

        public AllMembersClass()
        {
        }

        public AllMembersClass(int publicField, int privateField, int publicProperty, int privateProperty)
        {
            this.publicField = publicField;
            this.privateField = privateField;
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
        public int GetPrivateProperty() => PrivateProperty;
    }

    /// <summary>Test class with RequireSerializeFieldOnNonPublic enabled.</summary>
    [EasySerializable(RequireSerializeFieldOnNonPublic = true)]
    public class RequireSerializeFieldClass
    {
        public int publicField;
        [UnityEngine.SerializeField] private int privateFieldWithAttribute;
        private int privateFieldWithoutAttribute;

        public RequireSerializeFieldClass()
        {
        }

        public RequireSerializeFieldClass(int publicField, int privateFieldWithAttribute,
            int privateFieldWithoutAttribute)
        {
            this.publicField = publicField;
            this.privateFieldWithAttribute = privateFieldWithAttribute;
            this.privateFieldWithoutAttribute = privateFieldWithoutAttribute;
        }

        // Getter methods for testing non-public members
        public int GetPrivateFieldWithAttribute() => privateFieldWithAttribute;
        public int GetPrivateFieldWithoutAttribute() => privateFieldWithoutAttribute;
    }

    /// <summary>Test class with RequireSerializeFieldOnNonPublic disabled.</summary>
    [EasySerializable(RequireSerializeFieldOnNonPublic = false)]
    public class NotRequireSerializeFieldClass
    {
        public int publicField;
        private int privateField;

        public NotRequireSerializeFieldClass()
        {
        }

        public NotRequireSerializeFieldClass(int publicField, int privateField)
        {
            this.publicField = publicField;
            this.privateField = privateField;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
    }

    /// <summary>Base class with AllocInherit disabled.</summary>
    [EasySerializable(AllocInherit = false)]
    public class BaseClassNoInherit
    {
        public int baseField;

        public BaseClassNoInherit(int value)
        {
            baseField = value;
        }
    }

    /// <summary>Derived class from BaseClassNoInherit (should not inherit).</summary>
    public class DerivedFromNoInherit : BaseClassNoInherit
    {
        public int derivedField;

        public DerivedFromNoInherit(int baseValue, int derivedValue) : base(baseValue)
        {
            derivedField = derivedValue;
        }
    }

    /// <summary>Base class with AllocInherit enabled.</summary>
    [EasySerializable(AllocInherit = true, MemberFlags = SerializableMemberFlags.AllPublic)]
    public class BaseClassWithInherit
    {
        public int baseField;
        private int basePrivateField;

        public BaseClassWithInherit(int publicValue, int privateValue)
        {
            baseField = publicValue;
            basePrivateField = privateValue;
        }

        // Getter methods for testing non-public members
        public int GetBasePrivateField() => basePrivateField;
    }

    /// <summary>Derived class from BaseClassWithInherit (should inherit).</summary>
    public class DerivedFromWithInherit : BaseClassWithInherit
    {
        public int derivedField;

        public DerivedFromWithInherit(int basePublicValue, int basePrivateValue, int derivedValue)
            : base(basePublicValue, basePrivateValue)
        {
            derivedField = derivedValue;
        }
    }

    /// <summary>Base class without attribute.</summary>
    public class BaseClassNoAttribute
    {
        public int baseField;

        public BaseClassNoAttribute(int value)
        {
            baseField = value;
        }
    }

    /// <summary>Derived class with attribute.</summary>
    [EasySerializable]
    public class DerivedWithAttribute : BaseClassNoAttribute
    {
        public int derivedField;

        public DerivedWithAttribute(int baseValue, int derivedValue) : base(baseValue)
        {
            derivedField = derivedValue;
        }
    }
}
