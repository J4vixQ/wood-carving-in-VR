using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Knife : MonoBehaviour
{

    public GameObject _collisionBox;
    public GameObject _searchBox;
    public Dictionary<string, GameObject> _referencePointDict = new();

    private Vector3 _position;
    private Quaternion _rotation;

    public GameObject _carvedVolume;
    public bool carving_flag;
    private Vector3 blade_direction;
    private Vector3 last_position;

    public Transform controllerTransform;
    private Quaternion original_rotation;

    void Awake()
    {
        _position = transform.position;
        _rotation = transform.rotation;

        Transform childTransform = transform.Find("CollisionBox");
        if (childTransform == null) Debug.Log("CollisionBox not found");
        _collisionBox = childTransform.gameObject;

        childTransform = transform.Find("SearchBox");
        if (childTransform == null) Debug.Log("SearchBox not found");
        _searchBox = childTransform.gameObject;

        int i = 0;
        string pointName;
        while (true) {
            pointName = "Point" + i;
            childTransform = transform.Find("ReferencePoints/" + pointName);
            if (childTransform == null) {
                Debug.Log("ReferencePoints/Point" + i + " not found. Break the loop");
                break;
            }

            _referencePointDict[pointName] = childTransform.gameObject;

            pointName = pointName + "-1";
            childTransform = transform.Find("ReferencePoints/" + pointName);
            _referencePointDict[pointName] = childTransform.gameObject;
            i += 1;
        }

        BuildCarvedMesh();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        _carvedVolume.tag = "Knife";
        last_position = transform.position;
        original_rotation = transform.rotation;
    }

    public void BuildCarvedMesh()
    {
        _carvedVolume = new GameObject("Carved Volume", typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody));
        _carvedVolume.transform.parent = this.gameObject.transform;

        // List<Vector3> vertices = new(){
        //     _referencePointDict["Point0"].transform.position,
        //     _referencePointDict["Point0-1"].transform.position,
        //     _referencePointDict["Point1"].transform.position,
        //     _referencePointDict["Point1-1"].transform.position,
        //     _referencePointDict["Point2"].transform.position,
        //     _referencePointDict["Point2-1"].transform.position,
        // };
        // List<int> triangles = new(){
        //     0, 5, 1, 5, 0, 4, 0, 1, 3, 0, 3, 2, 1, 5, 3, 0, 2, 4, 2, 5, 4, 2, 3, 5,
        // };

        List<Vector3> vertices = new(){
            _referencePointDict["Point0"].transform.position,
            _referencePointDict["Point0-1"].transform.position,
            _referencePointDict["Point1"].transform.position,
            _referencePointDict["Point1-1"].transform.position,
            _referencePointDict["Point0"].transform.position,
            _referencePointDict["Point0-1"].transform.position,
            _referencePointDict["Point2"].transform.position,
            _referencePointDict["Point2-1"].transform.position,
            _referencePointDict["Point0"].transform.position,
            _referencePointDict["Point1"].transform.position,
            _referencePointDict["Point2"].transform.position,
            _referencePointDict["Point0-1"].transform.position,
            _referencePointDict["Point1-1"].transform.position,
            _referencePointDict["Point2-1"].transform.position,
            _referencePointDict["Point1"].transform.position,
            _referencePointDict["Point1-1"].transform.position,
            _referencePointDict["Point2"].transform.position,
            _referencePointDict["Point2-1"].transform.position,
        };
        List<int> triangles = new(){
            0, 1, 3, 0, 3, 2, 4, 7, 5, 4, 6, 7, 8, 9, 10, 11, 13, 12, 14, 15, 16, 16, 15, 17
        };

        _carvedVolume.GetComponent<MeshFilter>().mesh.vertices = vertices.ToArray();
        _carvedVolume.GetComponent<MeshFilter>().mesh.triangles = triangles.ToArray();

        _carvedVolume.GetComponent<MeshRenderer>().enabled = false;
        _carvedVolume.GetComponent<MeshRenderer>().material = Resources.Load("Materials/subtle-grained-wood_albedo", typeof(Material)) as Material;

        _carvedVolume.GetComponent<MeshCollider>().sharedMesh = _carvedVolume.GetComponent<MeshFilter>().mesh;

        _carvedVolume.GetComponent<Rigidbody>().isKinematic = true;
    }

    public List<Vector3> GetRayGlobalPosList(){
        List<Vector3> output = new()
        {
            _referencePointDict["Point0"].transform.position,
            _referencePointDict["Point0-1"].transform.position,
            _referencePointDict["Point0"].transform.position,
            _referencePointDict["Point1"].transform.position,
            _referencePointDict["Point0"].transform.position,
            _referencePointDict["Point2"].transform.position,
            _referencePointDict["Point0-1"].transform.position,
            _referencePointDict["Point1-1"].transform.position,
            _referencePointDict["Point0-1"].transform.position,
            _referencePointDict["Point2-1"].transform.position,
            _referencePointDict["Point1"].transform.position,
            _referencePointDict["Point1-1"].transform.position,
            _referencePointDict["Point2"].transform.position,
            _referencePointDict["Point2-1"].transform.position,
        };

        return output;
    }

    // public List<WoodTriangle> GetGlobalTriangleList(){
    //     List<WoodTriangle> output = new()
    //     {
    //         new WoodTriangle(
    //             _referencePointDict["Point0"].transform.position,
    //             _referencePointDict["Point1"].transform.position,
    //             _referencePointDict["Point0-1"].transform.position
    //         ),
    //         new WoodTriangle(
    //             _referencePointDict["Point0-1"].transform.position,
    //             _referencePointDict["Point1"].transform.position,
    //             _referencePointDict["Point1-1"].transform.position
    //         ),
    //         new WoodTriangle(
    //             _referencePointDict["Point2"].transform.position,
    //             _referencePointDict["Point0"].transform.position,
    //             _referencePointDict["Point0-1"].transform.position
    //         ),
    //         new WoodTriangle(
    //             _referencePointDict["Point2-1"].transform.position,
    //             _referencePointDict["Point2"].transform.position,
    //             _referencePointDict["Point0-1"].transform.position
    //         ),
    //     };

    //     return output;
    // }



    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float step = 0.2f * Time.fixedDeltaTime;

        // Update the transform with contoller input

        if (controllerTransform != null)
        {
            Vector3 pos = controllerTransform.position;
            Quaternion rot = controllerTransform.rotation;
            // Do something with pos and rot
            transform.SetPositionAndRotation(pos, rot*original_rotation);
        }

        Vector3 movement = transform.position - last_position;
        //if (Input.GetKey(KeyCode.W)) { movement += new Vector3(0, 0, step); }
        //if (Input.GetKey(KeyCode.S)) { movement += new Vector3(0, 0, -step); }
        //if (Input.GetKey(KeyCode.A)) { movement += new Vector3(step, 0, 0); }
        //if (Input.GetKey(KeyCode.D)) { movement += new Vector3(-step, 0, 0); }
        //if (Input.GetKey(KeyCode.Q)) { movement += new Vector3(0, step, 0); }
        //if (Input.GetKey(KeyCode.E)) { movement += new Vector3(0, -step, 0); }
        //// transform.position = transform.position + movement;

        //_position += movement;
        //transform.position = _position;
        //transform.rotation = _rotation;

        blade_direction = _referencePointDict["Point0"].transform.position - _referencePointDict["Point0-1"].transform.position;
        carving_flag = Vector3.Dot(blade_direction, movement) > 0.0f;
        last_position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
}
