using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Adornment
{
    public enum Wall
    {
        Back = 0,
        Left = 1,
        Right = 2
    }

    public string Key;
    public Wall ParentWall;
    public GameObject Prefab;
    public Vector3 Offset;
    public Vector3 Rotation;
    public Vector3 Scale;
}
