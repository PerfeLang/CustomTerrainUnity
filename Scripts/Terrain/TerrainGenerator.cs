using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private int m_TerrainSizeX = 10;
    [SerializeField] private int m_TerrainSizeZ = 10;

    [Serializable]
    private class Voxel
    {
        public int Height;
        public int Index;
    }
    [HideInInspector] [SerializeField] private SerializableDictionary<Vector3, Voxel> m_HeightMap = new SerializableDictionary<Vector3, Voxel>();
    [HideInInspector] [SerializeField] private Vector3[] m_Vertices;
    [HideInInspector] [SerializeField] private Mesh m_Mesh;
    [HideInInspector] public bool Generated;

    private static readonly Vector3[] m_Diagonal = new Vector3[]
    {
        new Vector3(-0.5f, 0.0f, 0.5f),
        new Vector3(0.5f, 0.0f, 0.5f),
        new Vector3(0.5f, 0.0f, -0.5f),
        new Vector3(-0.5f, 0.0f, -0.5f),
    };
    private static readonly int[] m_Triangles = new int[]
    {
        0,
        1,
        2,
        0,
        2,
        3,
    };
    private static readonly Vector2[] m_UVs = new Vector2[]
    {
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
    };
    private static readonly Vector3[] m_DiagonalAndAdjacent = new Vector3[]
    {
        // Diagonal
        new Vector3(-1.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, -1.0f),
        new Vector3(-1.0f, 0.0f, -1.0f),

        // Adjacent
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
    };

    public void Generate()
    {
        float terrainSizeX = -(m_TerrainSizeX / 2f) + 0.5f;
        float terrainSizeZ = -(m_TerrainSizeZ / 2f) + 0.5f;
        transform.position = new Vector3(terrainSizeX, 0, terrainSizeZ);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int x = 0; x < m_TerrainSizeX; x++)
            for (int z = 0; z < m_TerrainSizeZ; z++)
                m_HeightMap.Add(new Vector3(x, 0, z), new Voxel());

        int index = 0;
        foreach (Vector3 position in m_HeightMap.Keys)
        {
            // Vertices
            Vector3[] vertices1 = new Vector3[m_Diagonal.Length];
            for (int i = 0; i < vertices1.Length; i++)
                vertices1[i] = position + m_Diagonal[i];
            m_HeightMap[position].Index = vertices.Count;
            for (int i = 0; i < vertices1.Length; i++)
            {
                List<int> diagonalHeights = new List<int>();
                foreach (Vector3 diagonal in m_Diagonal)
                    if (m_HeightMap.TryGetValue(vertices1[i] + diagonal, out Voxel voxel))
                        diagonalHeights.Add(voxel.Height);

                vertices1[i].y = Average(diagonalHeights);
            }
            vertices.AddRange(vertices1);

            // Triangles
            int[] triangles1 = new int[m_Triangles.Length];
            for (int i = 0; i < triangles1.Length; i++)
                triangles1[i] = m_Triangles[i] + (index * 4);
            triangles.AddRange(triangles1);

            // UVs
            uvs.AddRange(m_UVs);

            index++;
        }

        m_Vertices = vertices.ToArray();
        m_Mesh = new Mesh
        {
            vertices = m_Vertices,
            triangles = triangles.ToArray(),
            uv = uvs.ToArray(),
        };
        GetComponent<MeshFilter>().sharedMesh = m_Mesh;
        GetComponent<MeshCollider>().sharedMesh = m_Mesh;

        Generated = true;
    }

    private static float Average(List<int> list)
    {
        float sum = 0f;

        foreach (int integer in list)
            sum += integer;

        return sum / list.Count;
    }

    private void UpdateVoxel(Vector3 position)
    {
        for (int i = 0; i < m_Diagonal.Length; i++)
        {
            List<int> diagonalHeights = new List<int>();
            foreach (Vector3 diagonal in m_Diagonal)
                if (m_HeightMap.TryGetValue(position + m_Diagonal[i] + diagonal, out Voxel voxel))
                    diagonalHeights.Add(voxel.Height);

            m_Vertices[m_HeightMap[position].Index + i].y = Average(diagonalHeights);
        }
    }

    public void ModifyVoxel(Vector3 position, int height)
    {
        m_HeightMap[position].Height += height;
        UpdateVoxel(position);
        foreach (Vector3 diagonalAndAdjacent in m_DiagonalAndAdjacent)
        {
            Vector3 diagonalAndAdjacentPosition = position + diagonalAndAdjacent;
            if (m_HeightMap.ContainsKey(diagonalAndAdjacentPosition))
                UpdateVoxel(diagonalAndAdjacentPosition);
        }

        m_Mesh.vertices = m_Vertices;
        GetComponent<MeshCollider>().sharedMesh = m_Mesh;
    }

    public Vector3 WorldToVoxelPosition(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        position = Vector3Int.RoundToInt(position);
        position.y = 0.0f;
        return position;
    }
}