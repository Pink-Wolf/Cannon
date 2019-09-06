using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBehavior : MonoBehaviour
{
    public Vector3[] worldForces = new Vector3[] { new Vector3(0, -10, 0) };

    //Settings
    public Vector2Int worldSize = new Vector2Int(16, 16);
    public float height = 8;
    public Vector2[] roughness_worth;
    public Material groundMaterial;
    //Values
    [HideInInspector]
    public float[,] groundHeight;

    void Start()
    {
        groundHeight = new float[worldSize.x, worldSize.y];
        //Get vertices and triangles
        Vector3[] vertices = new Vector3[worldSize.x * worldSize.y];
        Vector2[] uvs = new Vector2[vertices.Length];
        Texture2D texture = new Texture2D(worldSize.x, worldSize.y);
        Color[] textureColor = new Color[vertices.Length];
        int[] triangles = new int[(worldSize.x - 1) * (worldSize.y - 1) * 6];

        int iv = 0;
        int it = 0;
        int itv = 0;
        for(int iz = 0; iz < worldSize.y; iz++)
        {
            for(int ix = 0; ix < worldSize.x; ix++)
            {
                uvs[iv] = new Vector2(ix, iz);

                vertices[iv] = new Vector3(ix, 0.5f, iz);
                for(int i = 0; i < roughness_worth.Length; i++)
                {
                    vertices[iv].y = Mathf.Lerp(vertices[iv].y,
                        Mathf.PerlinNoise(ix / 100f * roughness_worth[i].x, iz / 100f * roughness_worth[i].x)
                        , roughness_worth[i].y);
                }
                textureColor[iv] = Color.Lerp(Color.black, Color.white, vertices[iv].y);
                vertices[iv].y *= height;
                groundHeight[ix, iz] = vertices[iv].y;

                if ((iz > 0) && (ix > 0))
                {
                    triangles[it * 6 + 0] = itv;
                    triangles[it * 6 + 1] = iv - 1;
                    triangles[it * 6 + 2] = itv + 1;
                    triangles[it * 6 + 3] = iv - 1;
                    triangles[it * 6 + 4] = iv;
                    triangles[it * 6 + 5] = itv + 1;

                    it++;
                }
                if (it > 0) itv++;
                iv++;
            }
        }
        //Apply mesh
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        texture.SetPixels(textureColor);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.Apply();
        //groundMaterial.mainTexture = texture;
        meshRenderer.material = groundMaterial;
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.uv = uvs;
        mesh.RecalculateNormals();
    }
}