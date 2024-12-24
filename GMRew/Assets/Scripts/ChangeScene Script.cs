using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneScript : MonoBehaviour
{

    void Awake()
    {
        Cursor.visible = true;
    }
    public void PlayAgainButton()
    {

        SceneManager.LoadScene("Pool Game");
        
    }
}
