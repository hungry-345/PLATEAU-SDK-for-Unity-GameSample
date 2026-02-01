using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Result1Button : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("ResultScene");
    }
}
