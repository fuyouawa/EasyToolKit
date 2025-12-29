using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Inspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TestInner
{
    public int jjss;
    public float ddbba;
    public Vector2 assdaad;
}

[EasyInspector]
public class TestInspector : MonoBehaviour
{
    public int jj;
    public float bb;
    public Vector2 assd;

    public TestInner inner;

    public UnityEvent kk;

    public List<int> jjjjjss;

    [FoldoutGroup("asdas", GroupCatalogue = "ddd")]
    public int kkss;
    public int kksss;

    [FoldoutGroup("asdasaadsasds", GroupCatalogue = "ddd/ssss")]
    public TestInner asddax;

    [FoldoutGroup("asdadds")]
    public int kkssdd;
    public int kksssdd;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
