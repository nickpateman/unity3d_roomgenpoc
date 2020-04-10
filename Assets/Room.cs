using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Wall
{
    Back = 0,
    Left = 1,
    Right = 2
}

[System.Serializable]
public class Adornment
{
    public string Key;
    public Wall Wall;
    public GameObject Prefab;
    public Vector3 Offset;
    public Vector3 Rotation;
    public Vector3 Scale;
}

[ExecuteInEditMode]
public class Room : MonoBehaviour
{ 
    [SerializeField] Vector3 FloorSize = new Vector3(1, 0, 1);
    [SerializeField] Material BackWallMaterial;
    [SerializeField] Material LeftWallMaterial;
    [SerializeField] Material RightWallMaterial;
    [SerializeField] Material FloorMaterial;
    [SerializeField] Adornment[] Adornments;

    private GameObject _floor;
    private GameObject _walls;
    private List<GameObject> _wallPlanes = new List<GameObject>();

    void Start()
    {
        
    }

    private void Awake()
    {
        Reset();
    }

    void Update()
    {
    }

    public void Reset()
    {
        DeleteAllChildren();
        GenerateFloor();
        GenerateWalls();
        CreateAdornments();
    }

    private void DeleteAllChildren()
    {
        foreach (Transform child in transform)
        {
            Debug.Log($"Destroying '{child.name}'.");
            child.gameObject.transform.parent = null;
            GameObject.DestroyImmediate(child.gameObject);
        }

        _wallPlanes.Clear();
    }

    private void GenerateFloor()
    {
        var floorName = $"{name}.Floor";
        _floor = GameObject.Find(floorName);
        if(_floor == null)
        {
            _floor = new GameObject(floorName);
            MeshFilter meshFilter = (MeshFilter)_floor.AddComponent(typeof(MeshFilter));
            meshFilter.mesh = CreateFloorMesh(
                floorName,
                FloorSize.x,
                FloorSize.z);
            MeshRenderer renderer = _floor.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            renderer.material = FloorMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = true;
            _floor.transform.parent = transform;
        }
    }

    private void GenerateWalls()
    {
        var wallsName = $"{name}.Walls";
        _walls = GameObject.Find(wallsName);
        if (_walls == null)
        {
            _walls = new GameObject(wallsName);
            _walls.transform.parent = transform;
        }

        var wall1Name = $"{wallsName}.Back";
        var wall1 = GameObject.Find(wall1Name);
        if (wall1 == null)
        {
            wall1 = CreateWallObject(
                wall1Name,
                FloorSize.x,
                FloorSize.y,
                _walls.transform,
                BackWallMaterial);
            _wallPlanes.Add(wall1);
        }
        else
            _wallPlanes.Add(wall1);

        var wall2Name = $"{wallsName}.Left";
        var wall2 = GameObject.Find(wall2Name);
        if (wall2 == null)
        {
            wall2 = CreateWallObject(
                wall2Name,
                FloorSize.z,
                FloorSize.y,
                _walls.transform,
                LeftWallMaterial);
            _wallPlanes.Add(wall2);
        }
        else
            _wallPlanes.Add(wall2);

        var wall3Name = $"{wallsName}.Right";
        var wall3 = GameObject.Find(wall3Name);
        if (wall3 == null)
        {
            wall3 = CreateWallObject(
                wall3Name,
                FloorSize.z,
                FloorSize.y,
                _walls.transform,
                RightWallMaterial);
            _wallPlanes.Add(wall3);
        }
        else
            _wallPlanes.Add(wall3);

        RepositionWalls();
    }

    private GameObject CreateWallObject(
        string name,
        float width,
        float height,
        Transform parent,
        Material material)
    {
        var wall = new GameObject(name);
        MeshFilter meshFilter = (MeshFilter)wall.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = CreateWallMesh(
            name,
            width,
            height);
        MeshRenderer renderer = wall.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = true;
        wall.transform.parent = parent;
        return wall;
    }


    private Mesh CreateWallMesh(
        string name,
        float width,
        float height)
    {
        Mesh m = new Mesh();
        m.name = name;
        m.vertices = new Vector3[] {
            new Vector3(-width, height, 0),
            new Vector3(width, height, 0),
            new Vector3(width, -height, 0),
            new Vector3(-width, -height, 0)
        };
        m.triangles = new int[] { 0, 1, 3, 1, 2, 3 };
        m.RecalculateNormals();

        return m;
    }

    private Mesh CreateFloorMesh(
        string name,
        float width,
        float depth)
    {
        Mesh m = new Mesh();
        m.name = name;
        m.vertices = new Vector3[] {
            new Vector3(-width, 0, depth),
            new Vector3(width, 0, depth),
            new Vector3(width, 0, -depth),
            new Vector3(-width, 0, -depth)
        };
        m.triangles = new int[] { 0, 1, 3, 1, 2, 3 };
        m.RecalculateNormals();

        return m;
    }

    private void RepositionWalls()
    {
        Debug.Log($"Resizing and positioning wall '{_wallPlanes[0]}'.");
        _wallPlanes[0].transform.localPosition = new Vector3(0, FloorSize.y, FloorSize.z);

        Debug.Log($"Resizing and positioning wall '{_wallPlanes[1]}'.");
        _wallPlanes[1].transform.localRotation = Quaternion.Euler(0, -90, 0);
        _wallPlanes[1].transform.localPosition = new Vector3(-FloorSize.x, FloorSize.y, 0);

        Debug.Log($"Resizing and positioning wall '{_wallPlanes[2]}'.");
        _wallPlanes[2].transform.localRotation = Quaternion.Euler(0, 90, 0);
        _wallPlanes[2].transform.localPosition = new Vector3(FloorSize.x, FloorSize.y, 0);
    }

    private void CreateAdornments()
    {
        foreach(var curAdornment in Adornments)
        {
            var adornmentPrefab = GameObject.Instantiate(curAdornment.Prefab);
            var wall = _wallPlanes[(int)curAdornment.Wall];
            adornmentPrefab.transform.parent = wall.transform;
            adornmentPrefab.transform.localPosition = curAdornment.Offset;
            adornmentPrefab.transform.localRotation = Quaternion.Euler(curAdornment.Rotation);
            adornmentPrefab.transform.localScale = curAdornment.Scale;
        }
    }

}
