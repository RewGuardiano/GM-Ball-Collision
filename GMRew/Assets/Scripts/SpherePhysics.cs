
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePhysics : MonoBehaviour
{
    public Vector3 previousVelocity, previousPosition;
    public float TimeofImpact;
    public Vector3 velocity, acceleration;
    public float mass = 1.0f;
    float gravity = 9.81f;
    float CoeficientOfRestitution = 0.8f;

    Vector3 newVelocity1, newVelocity2;

    public float Radius { get { return transform.localScale.x / 2.0f; } private set { transform.localScale = value * 2 * Vector3.one; } }

    // Start is called before the first frame update
    void Start()
    {
        acceleration = gravity * Vector3.down;
    }

    // Update is called once per frame
    void Update()
    {

       // Store previous state
    previousVelocity = velocity;
    previousPosition = transform.position;

        // Update velocity
        acceleration = Vector3.zero; // Reset acceleration
        velocity += acceleration * Time.deltaTime;

    // Update position
    transform.position += velocity * Time.deltaTime;


    }



    public void ResolveCollisionWith(PlaneScript planeScript)
    {
        float currentDistance = planeScript.distanceFromSphere(this);
        float previousDistance = Vector3.Dot(previousPosition - planeScript.Position, planeScript.Normal) - Radius;

        // DEBUG
        print("Distance: " + currentDistance + " Old Distance: " + previousDistance);

        // Step 1) To check dividing by zero
        float timeOfImpact = -previousDistance / (currentDistance - previousDistance) * Time.deltaTime;
        // DEBUG print("TOI: " + timeOfImpact + " deltaTime: " + Time.deltaTime);

        // Step 2)
        Vector3 positionOfImpact = previousPosition + (timeOfImpact * velocity);

        // Recalculate velocity using timeOfImpact
        Vector3 velocityAtImpact = previousVelocity + (acceleration * timeOfImpact);

        // Step 3) Resolve Collision
        Vector3 normalComponent = Utility.parallel(velocityAtImpact, planeScript.Normal);
        Vector3 perpendicularComponent = Utility.perpendicular(velocityAtImpact, planeScript.Normal);

        Vector3 newVelocity = (perpendicularComponent - CoeficientOfRestitution * normalComponent);

        // Calculate remaining time after impact
        float timeRemaining = Time.deltaTime - timeOfImpact;

        velocity = newVelocity + acceleration * timeRemaining;

        // Check velocity is moving ball away from plane (IE same direction as normal +- 90 degrees)
        if (Vector3.Dot(velocity, planeScript.Normal) < 0)
        {
            velocity = Utility.perpendicular(velocity, planeScript.Normal);
        }

        transform.position = positionOfImpact + velocity * timeRemaining;
    }



    public bool isCollidingWithSphere(SpherePhysics otherSphere)
    {
        return Vector3.Distance(otherSphere.transform.position, transform.position) < (otherSphere.Radius + Radius);
    }

    public void ResolveCollisionWithSphere(SpherePhysics sphere2)
    {


        //calculate time of impact
        float currentDistance = Vector3.Distance(sphere2.transform.position, transform.position) - (sphere2.Radius + Radius);
        float previousDistance = Vector3.Distance(sphere2.previousPosition, previousPosition) - (sphere2.Radius + Radius);


        float timeOfImpact = -previousDistance / (currentDistance - previousDistance) * Time.deltaTime;
        print("TOI: " + timeOfImpact + "deltaTime: " + Time.deltaTime);

        //After getting Time of Impact, calculate position of spheres at impact for both spheres. 
        Vector3 sphere1POI = previousPosition + velocity * timeOfImpact;
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
        Vector3 v1 = ((mass - sphere2.mass) / (mass + sphere2.mass)) * u1 + ((sphere2.mass * 2) / (mass + sphere2.mass)) * u2;
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
        if (Vector3.Distance(transform.position, sphere2ResolvedPosition) < (Radius + sphere2.Radius))
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
