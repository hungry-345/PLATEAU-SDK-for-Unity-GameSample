using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerManager : MonoBehaviour
{
    private static AudioListenerManager instance;
    private AudioListener listener;

    void Awake()
    {
        // 多重生成防止
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Audio Listener を必ず持つ
        listener = GetComponent<AudioListener>();
        if (listener == null)
        {
            listener = gameObject.AddComponent<AudioListener>();
        }
    }

    /// <summary>
    /// 実際に使うカメラに Listener を移す
    /// </summary>
    public void AttachToCamera(Camera targetCamera)
    {
        if (targetCamera == null) return;

        // 既存のカメラListenerを削除
        AudioListener camListener = targetCamera.GetComponent<AudioListener>();
        if (camListener != null)
        {
            //Destroy(camListener);
        }

        // Listenerをカメラに移動
        transform.SetParent(targetCamera.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
