using UnityEngine;

public class BootstrapCamera : MonoBehaviour
{
    void Awake()
    {
        // シーン内で1つだけにする
        BootstrapCamera[] cams = FindObjectsOfType<BootstrapCamera>();
        if (cams.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // 必須コンポーネント
        if (GetComponent<Camera>() == null)
            gameObject.AddComponent<Camera>();

        if (GetComponent<AudioListener>() == null)
            gameObject.AddComponent<AudioListener>();
    }

    void LateUpdate()
    {
        // 自分以外に有効なCameraが現れたら退場
        Camera[] cams = Camera.allCameras;
        foreach (Camera cam in cams)
        {
            if (cam != null &&
                cam.gameObject != gameObject &&
                cam.enabled &&
                cam.gameObject.activeInHierarchy)
            {
                Destroy(gameObject);
                break;
            }
        }
    }
}
