using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColorChange : MonoBehaviour
{
    [SerializeField] private Material bodyMaterial;
    [SerializeField] private Material armMaterial;
    [SerializeField] private Material legMaterial;

    private void Awake()
    {     
        // 色を赤に変更
        bodyMaterial.color = Color.red;
        armMaterial.color = Color.red;
        legMaterial.color = Color.red;
    }

}

