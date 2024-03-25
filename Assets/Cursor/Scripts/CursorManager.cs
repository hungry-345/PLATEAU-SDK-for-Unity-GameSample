using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    private AudioSource clickSound;
    //カーソルが表示されているか
    public bool isVisible;

    public void OnEnable()
    {
        InputSystem.EnableDevice(Mouse.current);
    }
    public void OnDisable()
    {
        InputSystem.DisableDevice(Mouse.current);
    }
    public void OnVisible()
    {
        Cursor.visible = true;
        isVisible = true;
    }
    public void OnInvisible()
    {
        Cursor.visible = false;
        isVisible = false;
    }

    private void Awake()
    {
        clickSound = GetComponent<AudioSource>();
        isVisible = false;
    }
    public void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void CursorSoundPlay()
    {
        clickSound.Play();
    }
    public float GetSoundLength()
    {
        return clickSound.clip.length;
    }
}