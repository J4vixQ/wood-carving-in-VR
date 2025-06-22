using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

public class CarvingObject : MonoBehaviour
{
    private static int nGrid = 10;
    private GameObject[,,] chunks = new GameObject[nGrid, nGrid, nGrid];

    private float chunkSize = 0.05f;
    private float gridSize = 0.005f;
    public Material material = null;

    private float interval = 1f;
    private float timer = 0f;
    private float[,,] values = null;
    private bool isTransparent = false;
    private float targetAlpha = 0.7f;
    private LinkedList<float[,,]> buffer = new LinkedList<float[,,]>();
    private static int buffer_size = 10;
    public bool isUpdate = false;


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
        // generate a sphere
        // float result = (ObjectPos - new Vector3(10,10,10)*(chunkSize-gridSize)/2).magnitude - 0.4f; 

        // generate a cube
        Vector3 diff = ObjectPos;//- new Vector3(10, 10, 10) * ((chunkSize - gridSize) / 2f);
        Vector3 absDiff = new Vector3(Mathf.Abs(diff.x), Mathf.Abs(diff.y), Mathf.Abs(diff.z));
        float result = Mathf.Max(absDiff.x, absDiff.y, absDiff.z) - 0.35f / 2;// nGrid*0.7f* (chunkSize - gridSize) / 2;

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
                    chunks[i, j, k].transform.localPosition = (new Vector3(i, j, k) - Vector3.one* nGrid/2) * (chunkSize - gridSize);

                    chunks[i, j, k].AddComponent<Chunk>();
                    Chunk _chunk = chunks[i, j, k].GetComponent<Chunk>();
                    _chunk.Setup(chunkSize, gridSize, material, this);
                    _chunk.Init();
        }

        int total_point_number = (int)(chunkSize / gridSize) * nGrid - 2 * (nGrid - 1);
        values = new float[total_point_number, total_point_number, total_point_number];
        // Save the init status
        mergeChunkValues();
        addValues();
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > interval)
        {
            InputDevice deviceL = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

            if (isUpdate)
            {
                var startTime = Time.realtimeSinceStartup;
                addValues();
                var endTime = Time.realtimeSinceStartup;
                Debug.Log($"addValues: {endTime - startTime} seconds");

                startTime = Time.realtimeSinceStartup;
                mergeChunkValues();
                endTime = Time.realtimeSinceStartup;
                Debug.Log($"mergeChunkValues: {endTime - startTime} seconds");

                startTime = Time.realtimeSinceStartup;
                bool flag = filterValues();
                endTime = Time.realtimeSinceStartup;
                Debug.Log($"filterValues: {endTime - startTime} seconds");

                if (flag)
                {
                    startTime = Time.realtimeSinceStartup;
                    assignValueToChunk();
                    endTime = Time.realtimeSinceStartup;
                    Debug.Log($"assignValueToChunk: {endTime - startTime} seconds");
                }
                isUpdate = false;
            }

            // Undo function

            if (deviceL.isValid && deviceL.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed))
            {
                if (gripPressed)
                {
                    float[,,] last_values = getLastBuffer();
                    if (last_values is null)
                    {
                        Debug.Log($"No data to undo!!");
                    }
                    else
                    {
                        Debug.Log($"Undo!!");
                        Array.Copy(last_values, values, values.Length);
                        assignValueToChunk();
                    }
                }
            }
            //if (Input.GetKey(KeyCode.U))
            //{

            //    float[,,] last_values = getLastBuffer();
            //    if (last_values is null)
            //    {
            //        Debug.Log($"No data to undo!!");
            //    }
            //    else
            //    {
            //        Debug.Log($"Undo!!");
            //        Array.Copy(last_values, values, values.Length);
            //        assignValueToChunk();
            //    }

            //}

            if (deviceL.isValid && deviceL.TryGetFeatureValue(CommonUsages.menuButton, out bool menuPressed))
            {
                if (menuPressed)
                {
                    isTransparent = !isTransparent;
                    setAlpha(isTransparent ? targetAlpha : 1f);
                }
            }

            //if (Input.GetKey(KeyCode.P))
            //{
            //    isTransparent = !isTransparent;
            //    setAlpha(isTransparent ? targetAlpha : 1f);
            //}

            timer = 0.0f;
        }
    }

    // Updated
    private void addValues()
    {
        Debug.Log("add Values");
        if (buffer.Count >= buffer_size)
        {
            buffer.RemoveFirst();
        }
        float[,,] new_buffer_data = new float[values.GetLength(0), values.GetLength(1), values.GetLength(2)];
        Array.Copy(values, new_buffer_data, values.Length);

        buffer.AddLast(new_buffer_data);
    }

    private float[,,] getLastBuffer()
    {
        if (buffer.Count > 0)
        {
            float[,,] output = buffer.Last.Value;
            buffer.RemoveLast();
            return output;
        }
        else
        {
            return null;
        }

    }

    private void setAlpha(float alpha)
    {
        for (int k = 0; k < chunks.GetLength(2); k++)
            for (int j = 0; j < chunks.GetLength(1); j++)
                for (int i = 0; i < chunks.GetLength(0); i++)
                {
                    Chunk s_chunk = chunks[i, j, k].GetComponent<Chunk>();
                    Color color = s_chunk.material.color;
                    color.a = alpha;
                    s_chunk.material.color = color;
                }
    }
    private void mergeChunkValues()
    {
        int chunk_point_num = (int)(chunkSize / gridSize) - 2;
        for (int k = 0; k < chunks.GetLength(2); k++)
            for (int j = 0; j < chunks.GetLength(1); j++)
                for (int i = 0; i < chunks.GetLength(0); i++)
                {
                    Chunk s_chunk = chunks[i, j, k].GetComponent<Chunk>();
                    for (int chunk_k = 0; chunk_k < s_chunk.p.GetLength(2); chunk_k++)
                        for (int chunk_j = 0; chunk_j < s_chunk.p.GetLength(1); chunk_j++)
                            for (int chunk_i = 0; chunk_i < s_chunk.p.GetLength(0); chunk_i++)
                            {
                                values[i * chunk_point_num + chunk_i, j * chunk_point_num + chunk_j, k * chunk_point_num + chunk_k] = s_chunk.p[chunk_i, chunk_j, chunk_k].Value;
                            }
                }
    }

    private bool filterValues()
    {
        int sizeX = values.GetLength(0);
        int sizeY = values.GetLength(1);
        int sizeZ = values.GetLength(2);

        Vector3Int center = new Vector3Int(sizeX / 2, sizeY / 2, sizeZ / 2);

        bool[,,] visited = new bool[sizeX, sizeY, sizeZ];
        Queue<Vector3Int> queue = new Queue<Vector3Int>();


        // flood fill from center
        queue.Enqueue(center);
        visited[center.x, center.y, center.z] = true;

        List<Vector3Int> directions = new List<Vector3Int>
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1),
        };

        while (queue.Count > 0)
        {
            var p = queue.Dequeue();

            for (int d = 0; d < 6; d++)
            {
                int nx = p.x + directions[d].x;
                int ny = p.y + directions[d].y;
                int nz = p.z + directions[d].z;

                if (nx >= 0 && nx < sizeX &&
                    ny >= 0 && ny < sizeY &&
                    nz >= 0 && nz < sizeZ &&
                    !visited[nx, ny, nz] &&
                    values[nx, ny, nz] < 0)
                {
                    visited[nx, ny, nz] = true;
                    queue.Enqueue(new Vector3Int(nx, ny, nz));
                }
            }
        }

        // remove other voxel
        bool isUpdated = false;
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (!visited[x, y, z] && values[x, y, z] < 0)
                    {
                        values[x, y, z] = 0.1f;
                        isUpdated = true;
                    }
                }
            }
        }
        return isUpdated;
    }

    private void assignValueToChunk()
    {
        int chunk_point_num = (int)(chunkSize / gridSize) - 2;
        for (int k = 0; k < chunks.GetLength(2); k++)
            for (int j = 0; j < chunks.GetLength(1); j++)
                for (int i = 0; i < chunks.GetLength(0); i++)
                {
                    bool isUpdated = false;
                    Chunk s_chunk = chunks[i, j, k].GetComponent<Chunk>();
                    for (int chunk_k = 0; chunk_k < s_chunk.p.GetLength(2); chunk_k++)
                        for (int chunk_j = 0; chunk_j < s_chunk.p.GetLength(1); chunk_j++)
                            for (int chunk_i = 0; chunk_i < s_chunk.p.GetLength(0); chunk_i++)
                            {
                                if ((s_chunk.p[chunk_i, chunk_j, chunk_k].Value > 0 &&
                                    values[i * chunk_point_num + chunk_i, j * chunk_point_num + chunk_j, k * chunk_point_num + chunk_k] <= 0) ||
                                    (s_chunk.p[chunk_i, chunk_j, chunk_k].Value <= 0 &&
                                    values[i * chunk_point_num + chunk_i, j * chunk_point_num + chunk_j, k * chunk_point_num + chunk_k] > 0))
                                {
                                    s_chunk.p[chunk_i, chunk_j, chunk_k].Value = values[i * chunk_point_num + chunk_i, j * chunk_point_num + chunk_j, k * chunk_point_num + chunk_k];
                                    isUpdated = true;
                                }
                            }
                    if (isUpdated)
                    {
                        s_chunk.MarchCubes();
                    }
                }
    }

    public bool saveToFile(string fileName)
    {
        bool result = false;

        if (values is null) return false;

        try
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                writer.Write(chunkSize);
                writer.Write(gridSize);

                Int32 x = values.GetLength(0);
                Int32 y = values.GetLength(1);
                Int32 z = values.GetLength(2);
                writer.Write(x);
                writer.Write(y);
                writer.Write(z);
                for (int i = 0; i < x; i++)
                    for (int j = 0; j < y; j++)
                        for (int k = 0; k < z; k++)
                            writer.Write(values[i, j, k]);

                result = true;
            }
        }
        catch (Exception)
        {

        }

        return result;
    }

    public bool loadFromFile(string fileName)
    {
        bool result = false;
        // check if file exists
        if (File.Exists(fileName))
        {
            float tempChunkSize, tempGridSize;
            int x, y, z;
            float[,,] tempValues;
            // read data from file
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    tempChunkSize = reader.ReadSingle();
                    tempGridSize = reader.ReadSingle();
                    x = reader.ReadInt32();
                    y = reader.ReadInt32();
                    z = reader.ReadInt32();
                    tempValues = new float[x, y, z];
                    for (int i = 0; i < x; i++)
                        for (int j = 0; j < y; j++)
                            for (int k = 0; k < z; k++)
                                tempValues[i, j, k] = reader.ReadSingle();

                }
            }
            catch (Exception)
            {
                return false;
            }

            // clear old data
            for (int k = 0; k < chunks.GetLength(2); k++)
                for (int j = 0; j < chunks.GetLength(1); j++)
                    for (int i = 0; i < chunks.GetLength(0); i++)
                    {
                        Destroy(chunks[i, j, k]);
                    }

            // assign new data 
            chunkSize = tempChunkSize;
            gridSize = tempGridSize;
            createChunks();
            buffer.Clear();
            values = tempValues;
            addValues();
            assignValueToChunk();
            result = true;
        }

        return result;
    }
}
