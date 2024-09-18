using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    List<SpherePhysics> spheres;
    List<PlaneScript> planes;
    

    // Start is called before the first frame update
    void Start()
    {
        spheres = FindObjectsOfType<SpherePhysics>().ToList();
        planes = FindObjectsOfType<PlaneScript>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < spheres.Count - 1; i++)
        {
            SpherePhysics sphere = spheres[i];
            foreach (PlaneScript plane in planes)
            {
                if (plane.isCollidingWith(sphere))
                {
                    sphere.ResolveCollisionWith(plane);
                }
            }

            for (int j = i + 1; j < spheres.Count; j++)
            {
               SpherePhysics sphere2 = spheres[j];
                if (sphere2.isCollidingWith(sphere))
                {
                    sphere.ResolveCollisionWith(sphere2);
                }
            }
        }
    }
}
