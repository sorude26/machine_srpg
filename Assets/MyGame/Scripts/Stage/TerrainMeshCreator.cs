using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMeshCreator : MonoBehaviour
{
    private enum Angle
    {
        Forward,
        Back,
        Left,
        Right
    }
    [SerializeField]
    private Material _terrainMaterial = default;
    [SerializeField]
    private MeshFilter _terrainFilter = default;
    [SerializeField]
    private MeshRenderer _terrainRenderer = default;

    private float _height = 1f;
    private float _topPos = 0f;
    private int _verTicesCount = 0;
    private Vector3[] GetVertices(float size, Angle angle)
    {
        if (angle == Angle.Forward)
        {
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(-size, _topPos - _height, size),
                new Vector3(size, _topPos - _height, size),
                new Vector3(-size, _topPos, size),
                new Vector3(size, _topPos, size)
            };
            return vertices;
        }
        else if (angle == Angle.Back)
        {
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(size, _topPos, -size),
                new Vector3(size, _topPos - _height, -size),
                new Vector3(-size, _topPos, -size),
                new Vector3(-size, _topPos - _height, -size)
            };
            return vertices;
        }
        else if (angle == Angle.Left)
        {
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(-size, _topPos, size),
                new Vector3(-size, _topPos, -size),
                new Vector3(-size, _topPos - _height, size),
                new Vector3(-size, _topPos - _height, -size)
            };
            return vertices;
        }
        else if (angle == Angle.Right)
        {
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(size, _topPos, size),
                new Vector3(size, _topPos - _height, size),
                new Vector3(size, _topPos, -size),
                new Vector3(size, _topPos - _height, -size)
            };
            return vertices;
        }
        return null;
    }
    private int[] GetTris()
    {
        int[] tris = new int[6]
        {
            1 + _verTicesCount, 2 + _verTicesCount, 0 + _verTicesCount,
            1 + _verTicesCount, 3 + _verTicesCount, 2 + _verTicesCount
        };
        return tris;
    }
    private Vector3[] GetNormals(Angle angle)
    {
        if (angle == Angle.Forward)
        {
            Vector3[] normalsa = new Vector3[4] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
            return normalsa;
        }
        else if (angle == Angle.Back)
        {
            Vector3[] normals = new Vector3[4] { Vector3.back, Vector3.back, Vector3.back, Vector3.back, };
            return normals;
        }
        else if (angle == Angle.Left)
        {
            Vector3[] normals = new Vector3[4] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
            return normals;
        }
        else if (angle == Angle.Right)
        {
            Vector3[] normals = new Vector3[4] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
            return normals;
        }
        return null;
    }
    private Vector2[] GetUv()
    {
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        return uv;
    }
    private void SetMeshData(Angle angle,float size,List<Vector3> verticesList,List<int> trianglesList,List<Vector3> normalList, List<Vector2> uvList)
    {
        foreach (var vertices in GetVertices(size / 2, angle))
        {
            verticesList.Add(vertices);
        }
        foreach (var triangele in GetTris())
        {
            trianglesList.Add(triangele);
        }
        foreach (var normal in GetNormals(angle))
        {
            normalList.Add(normal);
        }
        foreach (var uv in GetUv())
        {
            uvList.Add(uv);
        }
    }
    public void CreateTerrain(float forwrd, float back, float left, float right, float size)
    {
        _height = size;
        _verTicesCount = 0;
        float[] angles = new float[4] { forwrd, back, left, right };
        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();
        List<Vector3> normalList = new List<Vector3>();
        List<Vector2> uvList = new List<Vector2>();
        for (int i = 0; i < angles.Length; i++)
        {
            _topPos = 0;
            while (angles[i] >= 0)// 各4方向の高低差数値が0になるまで壁のmesh情報を追加する
            {
                SetMeshData((Angle)i, size, verticesList, trianglesList, normalList, uvList);
                angles[i] -= size;
                _topPos -= size;
                _verTicesCount += angles.Length;
            }
        }
        //リストに追加された頂点情報を元に地形Panelのメッシュを設定する
        if (_terrainRenderer == null)
        {
            _terrainRenderer = transform.gameObject.AddComponent<MeshRenderer>();
        }
        if (_terrainFilter == null)
        {
            _terrainFilter = transform.gameObject.AddComponent<MeshFilter>();
        }
        _terrainRenderer.material = _terrainMaterial;
        _terrainFilter.mesh = new Mesh();
        _terrainFilter.mesh.vertices = verticesList.ToArray();
        _terrainFilter.mesh.triangles = trianglesList.ToArray();
        _terrainFilter.mesh.normals = normalList.ToArray();
        _terrainFilter.mesh.uv = uvList.ToArray();
    }
}
