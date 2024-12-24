using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float speed = 0.8f; 
    public SpherePhysics spherePhysics; // Reference to SpherePhysics for velocity updates

    void Update()
    {
        
        Cursor.visible = false;

        // Get input from WASD or arrow keys
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down

        // Calculate the movement vector based on input
        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        if (spherePhysics != null)
        {
            // Apply the movement as an acceleration rather than directly modifying velocity
            spherePhysics.AddForce(movement * speed);
        }
    }
}
