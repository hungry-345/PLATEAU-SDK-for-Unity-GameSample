using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    // public Image cursorImage;              // UI Image
    //public Sprite defaultCursorSprite;
    //public Sprite highlightCursorSprite; 
    private AudioSource clickSound;
    private Vector2 mousePosition;

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
    }
    public void OnInvisible()
    {
        Cursor.visible = false;
    }

    private void Awake()
    {
        clickSound = GetComponent<AudioSource>();
    }
    public void Update()
    {
        //mousePosition = Mouse.current.position.ReadValue();
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        // cursorImage.rectTransform.position = mousePosition;
        Cursor.lockState = CursorLockMode.Confined;
        //if(Input.GetMouseButtonDown(0))
        //{
        //    clickSound.Play();
        //}
    }

    bool ShouldHighlightCursor()
    {
        return false;
    }

    public void CursorSoundPlay()
    {
        clickSound.Play();
    }
    public bool CheckClickSound()
    {
        return clickSound.isPlaying;
    }

    public float GetSoundLength()
    {
        return clickSound.clip.length;
    }
}