using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    void Start()
    {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        if (rend != null)
        {
            rend.enabled = false;  // ゲーム開始後に非表示
        }
    }
}

