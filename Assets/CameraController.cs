using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 20f;
    public float scrollSpeed = 200f;
    
    Plane floor = new Plane(Vector3.up, Vector3.zero);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
