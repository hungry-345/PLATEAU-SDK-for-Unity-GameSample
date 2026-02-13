using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Return_controller: MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("How to play 1");
    }
}
