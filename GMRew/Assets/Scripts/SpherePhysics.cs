
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePhysics : MonoBehaviour
{
    public Vector3 previousVelocity, previousPosition;
    public float TimeofImpact;
    public Vector3 velocity, acceleration;
    public float mass = 0.8f;
    float gravity = 9.81f;
    float CoeficientOfRestitution = 0.8f;

    Vector3 newVelocity1, newVelocity2;

    public float Radius { get { return transform.localScale.x / 2.0f; } private set { transform.localScale = value * 2 * Vector3.one; } }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize velocity and acceleration at the start
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Store previous state
        previousVelocity = velocity;
        previousPosition = transform.position;

        // Update acceleration with gravity applied
        acceleration = gravity * Vector3.down;  // gravity is applied every frame

        // Update velocity based on acceleration
        velocity += acceleration * Time.deltaTime;

        // Update position based on velocity
        transform.position += velocity * Time.deltaTime;

    }
    // Method to apply external acceleration (e.g., from user input)
    public void AddForce(Vector3 force)
    {
        velocity += force * Time.deltaTime; // Apply force over time
    }



    public void ResolveCollisionWith(PlaneScript planeScript)
    {
        // Calculate the distance from the sphere to the plane
        float currentDistance = planeScript.distanceFromSphere(this);

        // DEBUG
        print("Current distance to plane: " + currentDistance);

        // If the sphere is below the plane, resolve the collision
        if (currentDistance < 0) // Sphere is below the plane, collision detected
        {
            // Calculate the amount of overlap (penetration depth)
            float overlap = Mathf.Abs(currentDistance);

            // Correct the sphere's position to resolve the overlap
            Vector3 correction = planeScript.Normal * overlap;
            transform.position += correction;  // Move sphere out of the plane

            // Reflect the velocity off the plane (use the Coefficient of Restitution)
            velocity = Utility.perpendicular(velocity, planeScript.Normal) - CoeficientOfRestitution * Utility.parallel(velocity, planeScript.Normal);

            // Ensure that velocity is pointing away from the plane (if it's still pointing into the plane)
            if (Vector3.Dot(velocity, planeScript.Normal) < 0)
            {
                velocity = Utility.perpendicular(velocity, planeScript.Normal);
            }
        }

        // After resolving the position and velocity, ensure we don't get stuck inside the plane
        transform.position = new Vector3(transform.position.x, Mathf.Max(transform.position.y, planeScript.Position.y), transform.position.z);
    }




    public bool isCollidingWithSphere(SpherePhysics otherSphere)
    {
        return Vector3.Distance(otherSphere.transform.position, transform.position) < (otherSphere.Radius + Radius);
    }

    public void ResolveCollisionWithSphere(SpherePhysics sphere2)
    {
        // Calculate time of impact
        float currentDistance = Vector3.Distance(sphere2.transform.position, transform.position) - (sphere2.Radius + Radius);
        float previousDistance = Vector3.Distance(sphere2.previousPosition, previousPosition) - (sphere2.Radius + Radius);

        float timeOfImpact = -previousDistance / (currentDistance - previousDistance) * Time.deltaTime;
        print("TOI: " + timeOfImpact + " deltaTime: " + Time.deltaTime);

        // After getting Time of Impact, calculate position of spheres at impact for both spheres
        Vector3 sphere1POI = previousPosition + velocity * timeOfImpact;
        Vector3 sphere2POI = sphere2.previousPosition + sphere2.velocity * timeOfImpact;

        // Recalculate velocity for both spheres using timeOfImpact
        Vector3 sphere1VelocityAtImpact = previousVelocity + (acceleration * timeOfImpact);
        Vector3 sphere2VelocityAtImpact = sphere2.previousVelocity + (sphere2.acceleration * timeOfImpact);

        // Normal of collision at time of impact
        Vector3 normal = (sphere1POI - sphere2POI).normalized;

        Vector3 sphere1Parallel = Utility.parallel(velocity, normal);
        Vector3 sphere1Perpendicular = Utility.perpendicular(velocity, normal);
        Vector3 sphere2Parallel = Utility.parallel(sphere2.velocity, normal);
        Vector3 sphere2Perpendicular = Utility.perpendicular(sphere2.velocity, normal);

        Vector3 u1 = sphere1Parallel;
        Vector3 u2 = sphere2Parallel;

        // Velocities after TOI parallel to the normal
        Vector3 v1 = ((mass - sphere2.mass) / (mass + sphere2.mass)) * u1 + ((sphere2.mass * 2) / (mass + sphere2.mass)) * u2;
        Vector3 v2 = (-(mass - sphere2.mass) / (mass + sphere2.mass)) * u2 + ((mass * 2) / (mass + sphere2.mass)) * u1;

        // Apply restitution to velocities
        velocity = sphere1Perpendicular + v1 * CoeficientOfRestitution;
        Vector3 sphere1VelocityAfterTOI = sphere1Perpendicular + v1 * CoeficientOfRestitution;
        Vector3 sphere2VelocityAfterTOI = sphere2Perpendicular + v2 * CoeficientOfRestitution;

        // Calculate remaining time after impact
        float timeRemaining = Time.deltaTime - timeOfImpact;

        // Update the velocities of both spheres
        velocity = sphere1VelocityAfterTOI + acceleration * timeRemaining;
        Vector3 sphere2Velocity = sphere2VelocityAfterTOI + sphere2.acceleration * timeRemaining;

        // Update this sphere's position first
        transform.position = sphere1POI + sphere1VelocityAfterTOI * timeRemaining;

        // Calculate other sphere's resolved position
        Vector3 sphere2ResolvedPosition = sphere2POI + sphere2VelocityAfterTOI * timeRemaining;

        // Check for overlap between spheres after resolution
        if (Vector3.Distance(transform.position, sphere2ResolvedPosition) < (Radius + sphere2.Radius))
        {
            // If spheres are still overlapping, adjust positions to prevent jitter
            float overlap = (Radius + sphere2.Radius) - Vector3.Distance(transform.position, sphere2ResolvedPosition);
            Vector3 direction = (transform.position - sphere2ResolvedPosition).normalized;
            transform.position += direction * overlap * 0.5f;
            sphere2.transform.position -= direction * overlap * 0.5f;
        }

        // Now resolve the position and velocity for the other sphere
        sphere2.slaveCollisionResolution(sphere2.transform.position, sphere2Perpendicular + v2 * sphere2.CoeficientOfRestitution);
    }



    private void slaveCollisionResolution(Vector3 position, Vector3 newVelocity)
    {
        transform.position = position;
        velocity = newVelocity;
    }
}
