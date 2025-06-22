using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Chunk : MonoBehaviour
{
    public delegate float AssignGridValue(Vector3 GridPos, Transform chunk_transform);
    public static AssignGridValue assignValueFunc;

    public float size = 0; // The size of the chunk in meter
    public float stepSize = 0; // The interval between each GridPoint in meter
    public Material material = null;


    public GridPoint[,,] p = null;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uv = new List<Vector2>();
    private GridCell cell = new GridCell();
    private bool allClear = false; // set it to True if there is no mesh in this chunk, which means that we can skip the collision and checking.

    private Knife _knife;
    private object _marchCubeLock = new object();
    public CarvingObject carve_object;

    private AudioSource audioSource;
    private AudioClip[] carvingClips;

    public void Setup(float size, float stepSize, Material material, CarvingObject carve_object)
    {
        this.size = size;
        this.stepSize = stepSize;
        this.material = material;
        this.carve_object = carve_object;
    }

    public void Init()
    {
        gameObject.GetComponent<BoxCollider>().size = Vector3.one * size;
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        MakeGridPoints();
        MarchCubes();
        _knife = GameObject.Find("Knife").GetComponent<Knife>();

        audioSource = gameObject.AddComponent<AudioSource>();
        carvingClips = Resources.LoadAll<AudioClip>("Audio/CarvingSounds");

    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Knife") == true)
        {
            if (this._knife.carving_flag)
            {
                if (updateGridValue(other))
                {
                    // var startTime = Time.realtimeSinceStartup;
                    this.carve_object.isUpdate = true;
                    MarchCubes();
                    // var endTime = Time.realtimeSinceStartup;
                    // Debug.Log($"MarchCubes: {endTime - startTime} seconds");
                    SendHapticFeedback();
                    PlayCarvingSound();
                }
            }
        }    
    }

    private bool updateGridValue(Collider other)
    {
        bool isUpdated = false;
        for (int x = 0; x < p.GetLength(0); x++)
        {
            for (int y = 0; y < p.GetLength(1); y++)
            {
                for (int z = 0; z < p.GetLength(2); z++)
                {
                    Vector3 globalPos = this.transform.TransformPoint(p[x, y, z].Position);
                    if (p[x, y, z].Value <= 0)
                    {
                        if (other.bounds.Contains(globalPos))
                        {
                            p[x, y, z].Value = 0.1f;
                            isUpdated = true;
                        }
                    }
                }
            }
        }

        return isUpdated;
    }

    private void MakeGridPoints()
    {
        int numberOfPoint = (int)(size / stepSize);
        p = new GridPoint[numberOfPoint, numberOfPoint, numberOfPoint];

        for (int x = 0; x < p.GetLength(0); x++)
        {
            for (int y = 0; y < p.GetLength(1); y++)
            {
                for (int z = 0; z < p.GetLength(2); z++)
                {
                    p[x, y, z] = new GridPoint();
                    p[x, y, z].Position = new Vector3(x, y, z) * stepSize - Vector3.one * size / 2;
                    p[x, y, z].Value = assignValueFunc(p[x, y, z].Position, this.transform);
                }
            }
        }
    }
    public void MarchCubes()
    {
        GameObject go = this.gameObject;
        MarchingCube.GetMesh(ref go, ref material, false);

        vertices.Clear();
        triangles.Clear();
        uv.Clear();

        /*  vertex 8 (0-7)
              E4-------------F5         7654-3210
              |               |         HGFE-DCBA
              |               |
        H7-------------G6     |
        |     |         |     |
        |     |         |     |
        |     A0--------|----B1  
        |               |
        |               |
        D3-------------C2               */

        allClear = true;
        for (int x = 0; x < p.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < p.GetLength(1) - 1; y++)
            {
                for (int z = 0; z < p.GetLength(2) - 1; z++)
                {
                    cell.Reset();
                    cell.A0 = p[x, y, z + 1];
                    cell.B1 = p[x + 1, y, z + 1];
                    cell.C2 = p[x + 1, y, z];
                    cell.D3 = p[x, y, z];
                    cell.E4 = p[x, y + 1, z + 1];
                    cell.F5 = p[x + 1, y + 1, z + 1];
                    cell.G6 = p[x + 1, y + 1, z];
                    cell.H7 = p[x, y + 1, z];

                    foreach (GridPoint gp in cell.GetPointArray())
                    {
                        if (gp.Value < 0) allClear = false;
                    }

                    MarchingCube.IsoFaces(ref cell, 0); // surfaceLevel is always 0.
                    CreateCell();
                }
            }
        }

        Vector3[] av = vertices.ToArray();
        int[] at = triangles.ToArray();
        Vector2[] au = uv.ToArray();
        MarchingCube.SetMesh(ref go, ref av, ref at, ref au);

        gameObject.GetComponent<BoxCollider>().isTrigger = !allClear;
    }
    private void CreateCell()
    {
        bool uvAlternate = false;
        for (int i = 0; i < cell.numtriangles; i++)
        {
            vertices.Add(cell.triangle[i].p[0]);
            vertices.Add(cell.triangle[i].p[1]);
            vertices.Add(cell.triangle[i].p[2]);

            triangles.Add(vertices.Count - 1);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 3);

            /*  A ------ B
                |        |
                |        |
                D ------ C  */
            if (uvAlternate == true)
            {
                uv.Add(UVCoord.A);
                uv.Add(UVCoord.C);
                uv.Add(UVCoord.D);
            }
            else
            {
                uv.Add(UVCoord.A);
                uv.Add(UVCoord.B);
                uv.Add(UVCoord.C);
            }
            uvAlternate = !uvAlternate;
        }
    }

    private void SendHapticFeedback()
    {
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightHand.isValid && rightHand.TryGetHapticCapabilities(out HapticCapabilities capabilities))
        {
            if (capabilities.supportsImpulse)
            {
                rightHand.SendHapticImpulse(0u, 0.5f, 0.1f);  // 通道0, 强度0.5, 持续0.1秒
            }
        }
    }

    private void PlayCarvingSound()
    {
        if (carvingClips != null && carvingClips.Length > 0)
        {
            var clip = carvingClips[Random.Range(0, carvingClips.Length)];
            audioSource.PlayOneShot(clip);
        }
    }

}