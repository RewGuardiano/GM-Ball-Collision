using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class GoalScript : MonoBehaviour
{
    public int goalLimit = 5; // The number of goals required to change the scene
    private int currentGoalCount = 0; // Tracks the number of goals scored
    public string nextSceneName = "You Win Scene"; 

    public PlaneScript goalPlane; 
    public Text goalCounterText;

    private HashSet<SpherePhysics> scoredBalls = new HashSet<SpherePhysics>(); // Keeps track of balls that already scored

    private void Start()
    {
        // Initialize the counter text
        UpdateGoalCounterUI();
    }

    private void Update()
    {
        // Find all SpherePhysics objects in the scene
        SpherePhysics[] spheres = FindObjectsOfType<SpherePhysics>();

        foreach (SpherePhysics sphere in spheres)
        {
            // Check if the sphere is colliding with the goal plane and has not already scored
            if (goalPlane.isCollidingWith(sphere) && !scoredBalls.Contains(sphere))
            {
                HandleGoal(sphere);
            }
        }
    }

    private void HandleGoal(SpherePhysics sphere)
    {
        // Increment the goal count
        currentGoalCount++;
        scoredBalls.Add(sphere); // Mark the ball as scored to avoid counting it again
        Debug.Log($"Goal scored! Current count: {currentGoalCount}/{goalLimit}");

        // Update the UI counter
        UpdateGoalCounterUI();

        // Check if the goal limit is reached
        if (currentGoalCount >= goalLimit)
        {
            Debug.Log("Goal limit reached! Loading next scene.");
            SceneManager.LoadScene(nextSceneName);
        }
    }


    private void UpdateGoalCounterUI()
    {
        if (goalCounterText != null)
        {
            goalCounterText.text = $"Goals: {currentGoalCount}/{goalLimit}";
        }
     
    }
}
