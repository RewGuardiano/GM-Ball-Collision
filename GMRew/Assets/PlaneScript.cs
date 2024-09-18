using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
    
    Vector3 point, normal;
    float distance;
    

    public Vector3 Normal { 
        get { return normal; }
        private set { normal = value.normalized;
            transform.up = normal;
        } 
    }

    internal bool isCollidingWith(SpherePhysics spherePhysics)
    {
        distance =  Vector3.Dot(spherePhysics.transform.position - point, normal);

        return distance < spherePhysics.Radius;
    }

    // Start is called before the first frame update
    void Start()
    {
        Normal = new Vector3(0, 1, 0.2f);

        point = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
