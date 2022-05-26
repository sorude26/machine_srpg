using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMeshCreator : MonoBehaviour
{
    [SerializeField]
    private Material _terrainMaterial = default;

    private float height = 1;
    private float top = 0;

    private Vector3[] GetVertices(float size, int a)
    {
        if (a == 1)
        {
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(size, top, -size),
                new Vector3(size, top - height, -size),
                new Vector3(-size, top, -size),
                new Vector3(-size, top - height, -size)
            };
            return vertices;
        }
        else if (a == 2)
        {
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(-size, top, size),
                new Vector3(-size, top, -size),
                new Vector3(-size, top - height, size),
                new Vector3(-size, top - height, -size)
            };
            return vertices;
        }
        else if (a == 3)
        {
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(size, top, size),
                new Vector3(size, top - height, size),
                new Vector3(size, top, -size),
                new Vector3(size, top - height, -size)
            };
            return vertices;
        }
        else
        {
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(-size, top - height, size),
                new Vector3(size, top - height, size),
                new Vector3(-size, top, size),
                new Vector3(size, top, size)
            };
            return vertices;
        }
    }
    private int[] GetTris()
    {
        int[] tris = new int[6]
        {
            1, 2, 0,
            1, 3, 2
        };
        return tris;
    }
    private Vector3[] GetNormals(int a)
    {
        if (a == 1)
        {
            Vector3[] normals = new Vector3[4] { Vector3.back, Vector3.back, Vector3.back, Vector3.back, };
            return normals;
        }
        else if (a == 2)
        {
            Vector3[] normals = new Vector3[4] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
            return normals;
        }
        else if (a == 3)
        {
            Vector3[] normals = new Vector3[4] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
            return normals;
        }
        else
        {
            Vector3[] normalsa = new Vector3[4] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
            return normalsa;
        }
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
    private void CreatePanel(float size, int angle)
    {
        var obj = new GameObject($"(Comb:{angle})");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        var combRenderer = obj.AddComponent<MeshRenderer>();
        combRenderer.material = _terrainMaterial;
        var meshFilters = obj.AddComponent<MeshFilter>();
        meshFilters.mesh = new Mesh();
        meshFilters.mesh.vertices = GetVertices(size / 2, angle);
        meshFilters.mesh.triangles = GetTris();
        meshFilters.mesh.normals = GetNormals(angle);
        meshFilters.mesh.uv = GetUv();
    }
    public void CreateTerrain(float forwrd, float back, float left, float right, float size)
    {
        height = size;
        top = 0;
        while (forwrd > 0)
        {
            CreatePanel(size, 0);
            forwrd -= size;
            top -= size;
        }
        top = 0;
        while (back > 0)
        {
            CreatePanel(size, 1);
            back -= size;
            top -= size;
        }
        top = 0;
        while (left > 0)
        {
            CreatePanel(size, 2);
            left -= size;
            top -= size;
        }
        top = 0;
        while (right > 0)
        {
            CreatePanel(size, 3);
            right -= size;
            top -= size;
        }
    }
}
