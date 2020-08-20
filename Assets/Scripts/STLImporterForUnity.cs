/*******************************************************************************
* 版权声明：北京润尼尔网络科技有限公司，保留所有版权
* 版本声明：v1.0.0
* 项目名称：大屏幕VR展示
* 类 名 称：BaseDataModelEntity
* 创建日期：2019-03-27 16:38:17
* 作者名称：黎特为
* CLR 版本：4.0.30319.42000
* 功能描述：STL格式文件导入
* 修改记录：
* 日期 描述 更新功能
* 更新：
* 318：取消继承Monobehaviour，没必要
******************************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class STLImporterForUnity
{
    //材质
    public Material material;
    //父节点
    public Transform rootFather;

    private int _singleTrianglesNumber = 15000;
    private List<Vector3> _vertices;
    private List<Vector3> _normals;
    private List<int> _triangles;

    public int GetVertexCount() { return _vertices.Count; }

    public void SetSTL(string path)
    {
        byte[] bytes = System.IO.File.ReadAllBytes(path);
        string _trianglescount = BitConverter.ToInt32(bytes, 80).ToString();
        int _total = int.Parse(_trianglescount);

        _vertices = new List<Vector3>();
        _normals = new List<Vector3>();
        _triangles = new List<int>();

        ReadVertex(bytes, _total);
        CreateGameObject(_total);
    }
    private void ReadVertex(byte[] bytes, int _total)
    {
        int _number = 0;
        while (_number < _total)
        {
            byte[] tempbytes = new byte[50];
            Buffer.BlockCopy(bytes, 84 + (50 * _number), tempbytes, 0, 50);
            if (bytes.Length < 50)
            {
                _number += 1;
                continue;
            }
            Vector3 vec0 = new Vector3(BitConverter.ToSingle(tempbytes, 0), BitConverter.ToSingle(tempbytes, 4), BitConverter.ToSingle(tempbytes, 8));
            Vector3 vec1 = new Vector3(BitConverter.ToSingle(tempbytes, 12), BitConverter.ToSingle(tempbytes, 16), BitConverter.ToSingle(tempbytes, 20));
            Vector3 vec2 = new Vector3(BitConverter.ToSingle(tempbytes, 24), BitConverter.ToSingle(tempbytes, 28), BitConverter.ToSingle(tempbytes, 32));
            Vector3 vec3 = new Vector3(BitConverter.ToSingle(tempbytes, 36), BitConverter.ToSingle(tempbytes, 40), BitConverter.ToSingle(tempbytes, 44));
            _normals.AddNormal(vec0);
            _triangles.AddTriangle(_vertices.AddGetIndex(vec1), _vertices.AddGetIndex(vec2), _vertices.AddGetIndex(vec3));
            _number += 1;
        }
    }

    private void CreateGameObject(int _total)
    {
        rootFather.transform.localScale = Vector3.one;
        GameObject stlObj = rootFather.gameObject;
        //stlObj.transform.localPosition = Vector3.zero;
        //stlObj.transform.localScale = Vector3.one;
        int count = _total / _singleTrianglesNumber;
        count += (_total % _singleTrianglesNumber > 0) ? 1 : 0;

        for (int i = 0; i < count; i++)
        {
            GameObject tem = new GameObject("STL:" + i.ToString());
            tem.transform.SetParent(stlObj.transform);
            tem.transform.localPosition = Vector3.zero;
            tem.transform.localScale = Vector3.one;
            MeshFilter mf = tem.AddComponent<MeshFilter>();
            MeshRenderer mr = tem.AddComponent<MeshRenderer>();

            int startIndex = i * _singleTrianglesNumber * 3;
            int length = _singleTrianglesNumber * 3;
            if ((startIndex + length) > _vertices.Count)
            {
                length = _vertices.Count - startIndex;
            }

            List<Vector3> vertices = _vertices.GetRange(startIndex, length);
            List<Vector3> normals = _normals.GetRange(startIndex, length);
            List<int> triangles = _triangles.GetRange(0, length);

            Mesh m = new Mesh();
            m.name = tem.name;
            m.vertices = vertices.ToArray();
            m.normals = normals.ToArray();
            m.triangles = triangles.ToArray();
            m.RecalculateNormals();
            mf.mesh = m;
            mr.material = material;

        }
        //ResetPos(stlObj);
        stlObj.transform.position = Vector3.zero;
        // stlObj.transform.localScale = (1f / maxSize) * Vector3.one;
        stlObj.transform.localScale = Vector3.one;
        stlObj.transform.SetParent(rootFather);

    }
    private void ResetPos(GameObject obj)
    {
        Transform parent = obj.transform;
        //Vector3 postion = parent.position;
        // Quaternion rotation = parent.rotation;
        // Vector3 scale = parent.localScale;
        parent.position = Vector3.zero;
        parent.rotation = Quaternion.Euler(Vector3.zero);
        parent.localScale = Vector3.one;


        Vector3 center = Vector3.zero;
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in renders)
        {
            center += child.bounds.center;
        }
        center /= parent.GetComponentsInChildren<Transform>().Length;
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (Renderer child in renders)
        {
            bounds.Encapsulate(child.bounds);
        }

        //parent.position = postion;
        //parent.rotation = rotation;
        //parent.localScale = scale;

        foreach (Transform t in parent)
        {
            t.position = t.position - bounds.center;
        }
        parent.transform.position = bounds.center + parent.position;

    }
    //private void SetMaxSize(Mesh m ,float maxsize)
    //{

    //    if (m.bounds.size.x > maxSize)
    //    {
    //        maxSize = m.bounds.size.x;
    //    }
    //    if (m.bounds.size.y > maxSize)
    //    {
    //        maxSize = m.bounds.size.y;
    //    }
    //    if (m.bounds.size.z > maxSize)
    //    {
    //        maxSize = m.bounds.size.z;
    //    }
    //}
}
public static class BuskitExtension
{
    public static bool IsOn(this string value)
    {
        return value == "On";
    }

    public static int AddGetIndex(this List<Vector3> vertices, Vector3 vec)
    {
        vertices.Add(vec);
        return vertices.Count - 1;
    }

    public static void AddNormal(this List<Vector3> normals, Vector3 vec)
    {
        normals.Add(vec);
        normals.Add(vec);
        normals.Add(vec);
    }

    public static void AddTriangle(this List<int> triangles, int vertex1, int vertex2, int vertex3)
    {
        triangles.Add(vertex1);
        triangles.Add(vertex2);
        triangles.Add(vertex3);
    }
}
