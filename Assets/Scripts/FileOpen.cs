using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileOpen : MonoBehaviour
{
    //材质
    public Material material;
    public Material material_Mesh;
    //父节点
    public Transform rootFather;
    //模型路径
    string path = Application.streamingAssetsPath + "/MyModel.stl";
    public GameObject importer;

    public bool isImporter = false;

    GameObject go;
    public void ImporterSTL()
    {
        isImporter = true;
        importer.SetActive(false);
        go = new GameObject("MyModel");

        STLImporterForUnity stlImporter = new STLImporterForUnity
        {
            rootFather = go.transform,
            material = material
        };
        stlImporter.SetSTL(path);
        go.transform.localScale = Vector3.one  * 0.01f;
        go.transform.localPosition = new Vector3(0,0,4);
        MeshInfo.Add(go, new MeshInfo() { path = path, vertexCount = stlImporter.GetVertexCount() });
    }

    public void SetMat()
    {
        if(go != null)
        {
            for(int i= 0;i<go.transform.childCount;i++)
            {
                go.transform.GetChild(i).GetComponent<MeshRenderer>().material = material_Mesh;
            }
        }
    }
}


public class MeshInfo
{
    static Dictionary<GameObject, MeshInfo> meshInfos = new Dictionary<GameObject, MeshInfo>();
    public static void Add(GameObject go, MeshInfo info)
    {
        if (meshInfos.ContainsKey(go)) return;
        foreach (var item in meshInfos.Values)
        {
            if (item.path == info.path) return;
        }
        meshInfos.Add(go, info);
    }
    public static MeshInfo Get(GameObject go)
    {
        if (meshInfos.ContainsKey(go)) return meshInfos[go];
        return null;
    }

    public string path;
    public int vertexCount;
}
