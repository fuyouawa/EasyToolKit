using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Inspector;
using UnityEngine;

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

    public List<int> jjjjjss;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
