using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColorChange : MonoBehaviour
{
    [SerializeField] private Material bodyMaterial;
    [SerializeField] private Material armMaterial;
    [SerializeField] private Material legMaterial;

    private Color originalBodyColor;
    private Color originalArmColor;
    private Color originalLegColor;

    private void Awake()
    {
        // 元の色を保存
        originalBodyColor = bodyMaterial.color;
        originalArmColor = armMaterial.color;
        originalLegColor = legMaterial.color;

        // 色を赤に変更
        bodyMaterial.color = Color.red;
        armMaterial.color = Color.red;
        legMaterial.color = Color.red;
    }

    private void OnDestroy()
    {
        // 元の色に戻す
        ResetMaterialColors();
    }

    private void OnApplicationQuit()
    {
        // アプリケーション終了時にも元の色に戻す
        ResetMaterialColors();
    }

    private void OnDisable()
    {
        ResetMaterialColors();
    }

    private void ResetMaterialColors()
    {
        // nullチェックを忘れずに
        if (bodyMaterial != null) bodyMaterial.color = originalBodyColor;
        if (armMaterial != null) armMaterial.color = originalArmColor;
        if (legMaterial != null) legMaterial.color = originalLegColor;
    }
}

