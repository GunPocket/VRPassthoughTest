using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingFromDistance : MonoBehaviour
{
    public Camera camera;
    private float speed = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToCamera = Vector3.Distance(transform.position, camera.transform.position);
        if (distanceToCamera > 1.2)
        {
            transform.Rotate(Vector3.back * speed * Time.deltaTime);
        } 
    }
}
