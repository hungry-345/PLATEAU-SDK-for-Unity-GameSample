using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Result2Button : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("NoResult");
    }
}
