using UnityEngine;
using System.Collections.Generic;

public class Icosphere : MonoBehaviour
{
    private struct TriangleIndices
    {
        public int v1;
        public int v2;
        public int v3;

        public TriangleIndices(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    // return index of point in the middle of p1 and p2
    private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache)
    {
        // first check if we have it already
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (cache.TryGetValue(key, out ret))
        {
            return ret;
        }

        // not in cache, calculate it
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3
        (
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f
        );

        float bias = Vector3.Distance(middle, point1) / (Vector3.Distance(point1, middle) + Vector3.Distance(middle, point2));

        float radius = Vector3.Distance(point1, Vector3.zero) * bias + Vector3.Distance(point2, Vector3.zero) * (1 - bias);

        // add vertex makes sure point is on unit sphere
        int i = vertices.Count;
        vertices.Add(middle.normalized * radius);

        // store it, return index
        cache.Add(key, i);

        return i;
    }

    public static void Create(GameObject gameObject, int recursionLevel, float radius, int seed, float initialVariation, float variation)
    {
        Debug.Log("Creating icosphere with a recursion level of " + recursionLevel + " and a radius of " + radius);

        Random.seed = seed;

        MeshFilter filter = gameObject.GetComponent<MeshFilter>();

        if (filter == null)
        {
            filter = gameObject.AddComponent<MeshFilter>();
        }

        Mesh mesh = filter.mesh;
        mesh.Clear();

        List<Vector3> vertList = new List<Vector3>();
        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

        // create 12 vertices of a icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertList.Add(new Vector3(-1f, t, 0f).normalized + new Vector3(-1f, t, 0f).normalized * Random.Range(-initialVariation, initialVariation));
        vertList.Add(new Vector3(1f, t, 0f).normalized + new Vector3(1f, t, -0f).normalized * Random.Range(-initialVariation, initialVariation));
        vertList.Add(new Vector3(-1f, -t, 0f).normalized + new Vector3(-1f, -t, 0f).normalized * Random.Range(-initialVariation, initialVariation));
        vertList.Add(new Vector3(1f, -t, 0f).normalized + new Vector3(1f, -t, 0f).normalized * Random.Range(-initialVariation, initialVariation));

        vertList.Add(new Vector3(0f, -1f, t).normalized + new Vector3(0f, -1f, t).normalized * Random.Range(-initialVariation, initialVariation));
        vertList.Add(new Vector3(0f, 1f, t).normalized + new Vector3(0f, 1f, t).normalized * Random.Range(-initialVariation, initialVariation));
        vertList.Add(new Vector3(0f, -1f, -t).normalized + new Vector3(0f, -1f, -t).normalized * Random.Range(-initialVariation, initialVariation));
        vertList.Add(new Vector3(0f, 1f, -t).normalized + new Vector3(0f, 1f, -t).normalized * Random.Range(-initialVariation, initialVariation));

        vertList.Add(new Vector3(t, 0f, -1f).normalized + new Vector3(t, 0f, -1f).normalized * Random.Range(-initialVariation, initialVariation));
        vertList.Add(new Vector3(t, 0f, 1f).normalized + new Vector3(t, 0f, 1f).normalized * Random.Range(-initialVariation, initialVariation));
        vertList.Add(new Vector3(-t, 0f, -1f).normalized + new Vector3(-t, 0f, -1f).normalized * Random.Range(-initialVariation, initialVariation));
        vertList.Add(new Vector3(-t, 0f, 1f).normalized + new Vector3(-t, 0f, 1f).normalized * Random.Range(-initialVariation, initialVariation));


        // create 20 triangles of the icosahedron
        List<TriangleIndices> faces = new List<TriangleIndices>();

        // 5 faces around point 0
        faces.Add(new TriangleIndices(0, 11, 5));
        faces.Add(new TriangleIndices(0, 5, 1));
        faces.Add(new TriangleIndices(0, 1, 7));
        faces.Add(new TriangleIndices(0, 7, 10));
        faces.Add(new TriangleIndices(0, 10, 11));

        // 5 adjacent faces 
        faces.Add(new TriangleIndices(1, 5, 9));
        faces.Add(new TriangleIndices(5, 11, 4));
        faces.Add(new TriangleIndices(11, 10, 2));
        faces.Add(new TriangleIndices(10, 7, 6));
        faces.Add(new TriangleIndices(7, 1, 8));

        // 5 faces around point 3
        faces.Add(new TriangleIndices(3, 9, 4));
        faces.Add(new TriangleIndices(3, 4, 2));
        faces.Add(new TriangleIndices(3, 2, 6));
        faces.Add(new TriangleIndices(3, 6, 8));
        faces.Add(new TriangleIndices(3, 8, 9));

        // 5 adjacent faces 
        faces.Add(new TriangleIndices(4, 9, 5));
        faces.Add(new TriangleIndices(2, 4, 11));
        faces.Add(new TriangleIndices(6, 2, 10));
        faces.Add(new TriangleIndices(8, 6, 7));
        faces.Add(new TriangleIndices(9, 8, 1));


        // refine triangles
        for (int i = 0; i < recursionLevel; i++)
        {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();
            foreach (var tri in faces)
            {
                // replace triangle by 4 triangles
                int a = getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache);
                int b = getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache);
                int c = getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache);

                faces2.Add(new TriangleIndices(tri.v1, a, c));
                faces2.Add(new TriangleIndices(tri.v2, b, a));
                faces2.Add(new TriangleIndices(tri.v3, c, b));
                faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
            for (int n = 0; n < vertList.Count; n++)
                vertList[n] += vertList[n] * Random.Range(-(variation / Mathf.Pow(3, i + 1)), (variation / Mathf.Pow(3, i + 1)));
        }

        //        for (int i = 0; i < 100; i++)
        //            vertList[Mathf.RoundToInt(Random.Range(0, vertList.Count))] *= 1;

        mesh.vertices = vertList.ToArray();

        List<int> triList = new List<int>();
        for (int i = 0; i < faces.Count; i++)
        {
            triList.Add(faces[i].v1);
            triList.Add(faces[i].v2);
            triList.Add(faces[i].v3);
        }
        mesh.triangles = triList.ToArray();

        Vector3[] normales = new Vector3[vertList.Count];
        for (int i = 0; i < normales.Length; i++)
            normales[i] = vertList[i].normalized;


        mesh.normals = normales;

        mesh.RecalculateBounds();
        mesh.Optimize();

        Debug.Log("Finished creating icosphere with " + mesh.vertices.Length + " vertices and " + mesh.triangles.Length + " triangles");
    }
}

public struct Vertex
{
    public int originalPosition;
    public Vector3 vertex;

    public Vertex(int _originalPosition, Vector3 _vertex)
    {
        originalPosition = _originalPosition;
        vertex = _vertex;
    }
}