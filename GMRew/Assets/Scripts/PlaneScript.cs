
using System;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
    private Vector3 point;
    private Vector3 normal;
    public float distance;

    public Vector3 Normal
    {
        get { return normal; }
        private set
        {
            normal = value.normalized;
            transform.up = normal;
        }
    }

    public Vector3 Position
    {
        get { return transform.position; }
        internal set
        {
            transform.position = value;
        }
    }

    internal bool isCollidingWith(SpherePhysics spherePhysics)
    {
        distance = Vector3.Dot(spherePhysics.transform.position - point, normal);
        return distance < spherePhysics.Radius;
    }

    internal float distanceFromSphere(SpherePhysics spherePhysics)
    {
        distance = Vector3.Dot(spherePhysics.transform.position - point, normal);
        return distance - spherePhysics.Radius;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial normal based on the plane's world orientation
        normal = transform.up;
        point = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Continuously update point and normal in case of changes in the plane's position or rotation
        point = transform.position;
        normal = transform.up;
    }
}
