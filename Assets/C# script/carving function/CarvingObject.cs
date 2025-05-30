using System.Collections.Generic;
using UnityEngine;

public class CarvingObject : MonoBehaviour
{
    private GameObject[,,] chunks = new GameObject[10, 10, 10];

    private float chunkSize = 0.1f;
    private float gridSize = 0.01f;
    public Material material = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // material = Resources.Load("Materials/subtle-grained-wood_albedo", typeof(Material)) as Material;
        Chunk.assignValueFunc = gripValueFunc;
        createChunks();
        GameObject dog = GameObject.Find("Dog");
        dog.transform.rotation = this.transform.rotation;

        GameObject dog_mesh = GameObject.Find("Dog/default");
        dog.transform.position += this.transform.position - dog_mesh.GetComponent<Renderer>().bounds.center;
    }

    float gripValueFunc(Vector3 gridLocalPos, Transform chunk_transform)
    {
        Vector3 globalPos = chunk_transform.TransformPoint(gridLocalPos);
        Vector3 ObjectPos = this.transform.InverseTransformPoint(globalPos);

        // Main logic to generate the initial mesh
        // float result = (ObjectPos - new Vector3(10,10,10)*(chunkSize-gridSize)/2).magnitude - 0.4f; // generate a sphere

        // generate a cube
        Vector3 diff = ObjectPos;//- new Vector3(10, 10, 10) * ((chunkSize - gridSize) / 2f);
        Vector3 absDiff = new Vector3(Mathf.Abs(diff.x), Mathf.Abs(diff.y), Mathf.Abs(diff.z));
        float result = Mathf.Max(absDiff.x, absDiff.y, absDiff.z)-0.35f;

        if (gridLocalPos == Vector3.zero)
        {
            // Debug.Log($"chunk_transform = {chunk_transform.position}");
            // Debug.Log($"globalPos = {globalPos}");
            // Debug.Log($"ObjectPos = {ObjectPos}");
            // Debug.Log($"result = {result}");
        }
        return result;
    }

    void createChunks()
    {
        for (int i = 0; i < chunks.GetLength(0); i++)
            for (int j = 0; j < chunks.GetLength(1); j++)
                for (int k = 0; k < chunks.GetLength(2); k++)
                {
                    //
                    chunks[i, j, k] = new GameObject();
                    chunks[i, j, k].transform.parent = this.gameObject.transform;
                    chunks[i, j, k].transform.localPosition = (new Vector3(i, j, k) - Vector3.one*5) * (chunkSize - gridSize);

                    chunks[i, j, k].AddComponent<Chunk>();
                    Chunk _chunk = chunks[i, j, k].GetComponent<Chunk>();
                    _chunk.Setup(chunkSize, gridSize, material);
                    _chunk.Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
