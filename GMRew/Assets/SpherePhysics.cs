using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePhysics : MonoBehaviour
{
    Vector3 velocity, acceleration;
    public float mass = 1.0f;
    float gravity = 9.81f;
    float CoeficientOfRestitution = 0.8f;

    Vector3 newVelocity1, newVelocity2; 
    
    public float Radius { get { return transform.localScale.x / 2.0f; } private set { transform.localScale = value * 2 * Vector3.one; } }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        acceleration = gravity * Vector3.down;

        velocity += acceleration * Time.deltaTime;

        transform.position += velocity * Time.deltaTime;

    }

    public void ResolveCollisionWith(PlaneScript planeScript)
    {
        transform.position -= velocity * Time.deltaTime;

        Vector3 y = Utility.parallel(velocity, planeScript.Normal);
        Vector3 x = Utility.perpendicular(velocity, planeScript.Normal);

        Vector3 newVelocity = (x - CoeficientOfRestitution * y);

        velocity = newVelocity;
    }

    public bool isCollidingWith(SpherePhysics otherSphere)
    {
        return Vector3.Distance(otherSphere.transform.position, transform.position) < (otherSphere.Radius + Radius);
    }

    public void ResolveCollisionWith(SpherePhysics sphere2)
    {
        Vector3 normal = (transform.position - sphere2.transform.position).normalized;

        Vector3 sphere1Parallel = Utility.parallel(velocity, normal);
        Vector3 sphere1Perpendicular = Utility.perpendicular(velocity, normal);
        Vector3 sphere2Parallel = Utility.parallel(sphere2.velocity, normal);
        Vector3 sphere2Perpendicular = Utility.perpendicular(sphere2.velocity, normal);

        Vector3 u1 = sphere1Parallel;
        Vector3 u2 = sphere2Parallel;


        Vector3 v1 = ((mass - sphere2.mass)/(mass + sphere2.mass)) * u1 + ((sphere2.mass*2)/(mass + sphere2.mass))*u2;
        Vector3 v2 = (-(mass - sphere2.mass) / (mass + sphere2.mass)) * u2 + ((mass * 2) / (mass + sphere2.mass)) * u1;

        velocity = sphere1Perpendicular + v1 * CoeficientOfRestitution;

        sphere2.slaveCollisionResolution(sphere2.transform.position, sphere2Perpendicular + v2 * sphere2.CoeficientOfRestitution);
        //asking other sphere to change
    }

    private void slaveCollisionResolution(Vector3 position, Vector3 newVelocity)
    {
        transform.position = position;
        velocity = newVelocity;
    }
}
