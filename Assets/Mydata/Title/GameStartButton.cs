using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButton : MonoBehaviour
{
    private void Start()
    {
        EventSystemSetup.SetupEventSystem();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnClick()
    {
        SceneManager.LoadScene("GameSample 1");
    }
}

