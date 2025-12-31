using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;
using UnityEngine;

public class JJBBs
{
    public int s;
    public string tsd;
    public float f;
    public double d;
    // public bool b;
    // public Vector3 v;
    // public Vector4 v4;
    // public Color cds;
    // public Color32 c32;
    // public Quaternion q;
    // public Matrix4x4 m;
    // public Transform t;
    // public GameObject g;
    // public Component cas;
    // public Object obj;
    // public Object[] array;
    // public List<Object> list;
    // public Dictionary<string, Object> dict;
    // public JJBBs nested;
}

public class TestSerializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var obj = new JJBBs()
        {
            s = 123,
            tsd = "Asda",
            f = 13,
            // g = gameObject,
            // cas = this,
        };

        EasySerializationData data = new EasySerializationData(EasyDataFormat.Binary);
        EasySerialize.To(obj, ref data);
        var sdd = EasySerialize.From<JJBBs>(ref data);

        Assert.IsTrue(obj.s == sdd.s);
        Assert.IsTrue(obj.tsd == sdd.tsd);
        Assert.IsTrue(obj.f == sdd.f);
        Assert.IsTrue(obj.d == sdd.d);
        // Assert.IsTrue(obj.b == sdd.b);
        // Assert.IsTrue(obj.v == sdd.v);
        // Assert.IsTrue(obj.v4 == sdd.v4);
        // Assert.IsTrue(obj.cds == sdd.cds);
        // Assert.IsTrue(obj.q == sdd.q);
        // Assert.IsTrue(obj.m == sdd.m);
        // Assert.IsTrue(obj.t == sdd.t);
        // Assert.IsTrue(obj.g == sdd.g);
        // Assert.IsTrue(obj.cas == sdd.cas);
        // Assert.IsTrue(obj.obj == sdd.obj);
        // Assert.IsTrue(obj.array == sdd.array);
        // Assert.IsTrue(obj.list == sdd.list);
        // Assert.IsTrue(obj.dict == sdd.dict);
        // Assert.IsTrue(obj.nested == sdd.nested);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
