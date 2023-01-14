using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tempUI : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

    }
}
