using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    public float speed = 5f; // Movement speed multiplier
    public SpherePhysics spherePhysics; // Reference to SpherePhysics for velocity updates

    void Update()
    {
        // Get input from WASD or arrow keys
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down

        // Move the ball using its velocity
        Vector3 movement = new Vector3(horizontal, 0f, vertical) * speed;

        // Apply movement to the ball's velocity
        if (spherePhysics != null)
        {
            spherePhysics.velocity += movement * Time.deltaTime;
        }
    }

}
