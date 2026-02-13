using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("How to play");
    }
}
