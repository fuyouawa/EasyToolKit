using System;
using System.Collections.Generic;
using EasyToolKit.Inspector.Attributes;
using EasyToolKit.TileWorldPro;
using UnityEngine;

[Serializable]
[MetroBoxGroup("asssdnbbb")]
public class TestInner
{
    public int jjss;
    public float ddbba;
    public Vector2 assdaad;
}

[Serializable]
public class TestBase
{
    public int BaseInt;
}

[Serializable]
public class TestDerive1 : TestBase
{
    public float DeriveFloat1;
}

[Serializable]
public class TestDerive2 : TestBase
{
    public float DeriveFloat2;
}

[Serializable]
public class TestInner1
{
    public TestInner2 inner2;
    public TestInner2 inner21;
    public TestInner2 inner22;
    public TestInner2 inner23;
}

[Serializable]
public class TestInner2
{
    public TestInner3 inner3;
    public TestInner3 inner31;
    public TestInner3 inner32;
    public TestInner3 inner33;
}

[Serializable]
public class TestInner3
{
    public TestInner4 inner4;
    public TestInner4 inner41;
    public TestInner4 inner42;
    public TestInner4 inner43;
}

[Serializable]
public class TestInner4
{
    public int int3;
    public float float3;
    public Vector2 vector2;
    public string string1;
    public string string2;
    public string string3;
    public string string4;
}

[EasyInspector]
[ShowOdinSerializedPropertiesInInspector]
public class TestInspector : MonoBehaviour
{
    public List<int> ss;


    [ShowInInspector]
    public TerrainDefinitionGroup Group;

    // public TestInner1 inner1;
//     [LabelText("地形定义表")]
// #if UNITY_EDITOR
//     [ValueDropdown(nameof(TerrainDefinitionItemDropdownList))]
// #endif
//     [MetroListDrawerSettings(ShowIndexLabel = false)]
//     public List<TerrainDefinitionNode> Nodes;

    // public int jj;
    // public float bb;
    // public Vector2 assd;
    //
    // [FoldoutGroup("asda34534s")]
    // public TestInner inner2;
    //
    // public TestInner inner;
    //
    // public UnityEvent kk;
    //
    // public List<int> jjjjjss;
    //
    // [FoldoutGroup("asdas", GroupCatalogue = "ddd")]
    // public int kkss;
    // public int kksss;
    //
    // [FoldoutBoxGroup("asdasaadsasds", GroupCatalogue = "ddd/ssss")]
    // public TestInner asddax;
    //
    // [FoldoutGroup("asdadds")]
    // public int kkssdd;
    // public int kksssdd;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private ValueDropdownList<TestBase> GetTestBaseDropdown()
    {
        var total = new ValueDropdownList<TestBase>();
        total.AddDelayed("Derive1", () => new TestDerive1());
        total.AddDelayed("Derive2", () => new TestDerive2());
        return total;
    }

#if UNITY_EDITOR
    private static readonly ValueDropdownList<TerrainDefinitionNode> TerrainDefinitionItemDropdownList = new()
    {
        new DelayedValueDropdownItem("分组", () => new TerrainDefinitionGroup()),
        new DelayedValueDropdownItem("地形", () => new TerrainDefinition()),
        new DelayedValueDropdownItem("复合地形", () => new TerrainDefinition(true)),
    };
#endif
}
