using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePhysics : MonoBehaviour
{
    public Vector3 previousVelocity, previousPosition;
    public float TimeofImpact; 
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
        previousVelocity = velocity;
        previousPosition = transform.position;
    
        acceleration = gravity * Vector3.down;

        velocity += acceleration * Time.deltaTime;

        transform.position += velocity * Time.deltaTime;

    }

    public void ResolveCollisionWith(PlaneScript planeScript)
    {

        // Calculate current distance from the plane
        float currentDistance = planeScript.Distance(this);
        float oldDistance = Vector3.Dot(previousPosition - planeScript.Position, planeScript.Normal) - Radius;

        // Check if the sphere was previously intersecting with the plane
        if (oldDistance > 0 && currentDistance <= 0)
        {
            // Calculate time of impact
            TimeofImpact = oldDistance / (oldDistance - currentDistance) * Time.deltaTime;

            Vector3 newPosition = previousPosition + TimeofImpact * velocity;



            // Calculate position of impact using the impact velocity
            Vector3 positionOfImpact = previousPosition + (TimeofImpact * velocity);

            // Recalculate velocity at the time of impact
            Vector3 impactVelocity = previousVelocity + acceleration * TimeofImpact;

            // Resolve collision by moving the sphere out of the plane
            transform.position = positionOfImpact - planeScript.Normal * Radius; // Move out by radius

            // Calculate new velocity after collision
            Vector3 y = Utility.parallel(impactVelocity, planeScript.Normal);
            Vector3 x = Utility.perpendicular(impactVelocity, planeScript.Normal);

            // Apply restitution to the normal component of velocity
            Vector3 newVelocity = (x - CoeficientOfRestitution * y);

            velocity = newVelocity + acceleration * (Time.deltaTime - TimeofImpact);
            transform.position += velocity * (Time.deltaTime - TimeofImpact);

            
        }


    }

    public bool isCollidingWith(SpherePhysics otherSphere)
    {
        return Vector3.Distance(otherSphere.transform.position, transform.position) < (otherSphere.Radius + Radius);
    }

    public void ResolveCollisionWith(SpherePhysics sphere2)
    {
        //calculating time of impact
        float D1 = Vector3.Distance(sphere2.transform.position, transform.position) - (sphere2.Radius + Radius);
        float oldDistance = Vector3.Distance(sphere2.previousPosition, previousPosition) - (sphere2.Radius + Radius);
       

     
        print("Distance: " + D1 + "Old Distance: " + oldDistance); 
    
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
