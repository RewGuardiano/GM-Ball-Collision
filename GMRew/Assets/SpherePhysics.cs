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
            TimeofImpact = -oldDistance / (currentDistance - oldDistance) * Time.deltaTime;

    



            // Calculate position of impact using the impact velocity
            Vector3 positionOfImpact = previousPosition + (TimeofImpact * velocity);

            // Recalculate velocity at the time of impact
            Vector3 impactVelocity = previousVelocity + (acceleration * TimeofImpact);

            // Calculate new velocity after collision
            Vector3 y = Utility.parallel(impactVelocity, planeScript.Normal);
            Vector3 x = Utility.perpendicular(impactVelocity, planeScript.Normal);

            // Apply restitution to the normal component of velocity
            Vector3 newVelocity = (x - CoeficientOfRestitution * y);

            //Calculate velocity from impact time to time of detection (remaining time after impact) 
            float timeRemaining = Time.deltaTime - TimeofImpact;

            velocity = newVelocity + acceleration * timeRemaining;


        //check velocity is moving ball away from plane (IE same direction as normal +- 90 degrees)
            if (Vector3.Dot(velocity, planeScript.Normal) < 0)
            { 
              velocity = Utility.perpendicular(velocity, planeScript.Normal); 
            };

            transform.position = positionOfImpact + velocity * timeRemaining;



            
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
       

       float timeOfImpact = -oldDistance / (currentDistance - oldDistance) * Time.deltaTime;
          print("TOI: " + timeOfImpact + "deltaTime: " + Time.deltaTime);

          //After getting Time of Impact, calculate position of spheres at impact for both spheres. 
          Vector3 sphere1POI = previousPostion + velocity + timeOfImpact;
          Vector3 sphere2POI = sphere2.previousPosition + sphere2.velocity * timeOfImpact;

          //Recalculate Velocity for both spheres from previous postion, but using timeOfImpact instead of deltaTime
          Vector3 Sphere1VelocityAtImpact = previousVelocity + (acceleration * timeOfImpact);
          Vector3 Sphere2VelocityAtImpact = sphere2.previousVelocity + (sphere2.acceleration * timeOfImpact);

          //normal of collision at time of impact 
        Vector3 normal = (sphere1POI - sphere2POI).normalized;

        Vector3 sphere1Parallel = Utility.parallel(velocity, normal);
        Vector3 sphere1Perpendicular = Utility.perpendicular(velocity, normal);
        Vector3 sphere2Parallel = Utility.parallel(sphere2.velocity, normal);
        Vector3 sphere2Perpendicular = Utility.perpendicular(sphere2.velocity, normal);

        Vector3 u1 = sphere1Parallel;
        Vector3 u2 = sphere2Parallel;

        // velocities after TOI parrallel to the normal 
        Vector3 v1 = ((mass - sphere2.mass)/(mass + sphere2.mass)) * u1 + ((sphere2.mass*2)/(mass + sphere2.mass))*u2;
        Vector3 v2 = (-(mass - sphere2.mass) / (mass + sphere2.mass)) * u2 + ((mass * 2) / (mass + sphere2.mass)) * u1;

        velocity = sphere1Perpendicular + v1 * CoeficientOfRestitution;
        //Velocity After Time of Impact
         Vector3 sphere1VelocityAfterTOI = sphere1Perpendicular + v1 * CoeficientOfRestitution;
        Vector3 sphere2VelocityAfterTOI = sphere2Perpendicular + v2 * CoeficientOfRestitution;

        //calculate velocity from impact time to time of detection (remaining time after impact)
        float timeRemaining = Time.deltaTime - timeOfImpact;

        //Recalculate Velocities of both spheres 
        velocity = sphere1VelocityAfterTOI + acceleration * timeRemaining;
        Vector3 sphere2Velocity = sphere2VelocityAfterTOI + sphere2.acceleration * timeRemaining;

          //update this sphere1 first
        transform.position = sphere1POI + sphere1VelocityAfterTOI * timeRemaining;

        //calculate othersphere position
        Vector3 sphere2ResolvedPosition = sphere2POI + sphere2VelocityAfterTOI * timeRemaining; 

        //Checking for overlap between spheres after resolution
        If(Vector3.Distance(transform.position,sphere2ResolvedPosition) < (Radius + sphere2.Radius))
        {
            print("HELP");
        }

        sphere2.slaveCollisionResolution(sphere2.transform.position, sphere2Perpendicular + v2 * sphere2.CoeficientOfRestitution);
        //asking other sphere to change

       
    }

    private void slaveCollisionResolution(Vector3 position, Vector3 newVelocity)
    {
        transform.position = position;
        velocity = newVelocity;
    }
}
