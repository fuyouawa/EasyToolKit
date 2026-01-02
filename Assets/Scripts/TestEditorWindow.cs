using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Inspector;
using EasyToolKit.Inspector.Editor;
using UnityEditor;
using UnityEngine;

public class JJBB
{
    public int aa;
}

public class TestEditorWindow : EasyEditorWindow
{
    [MenuItem("Tools/EasyToolKit/Test/Open Editor Window")]
    public static void Open()
    {
        GetWindow<TestEditorWindow>("Test Editor Window");
    }

    public int jj;
    public List<float> bbb;

    [Button]
    public void BB()
    {
        EasyEditorWindowUtility.InspectObject(new JJBB());
    }

    [Button]
    public void BBb()
    {
        EasyEditorWindowUtility.InspectObjectInDropDown(new JJBB());
    }
}
