using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    public Image cursorImage;              // UI Image
    //public Sprite defaultCursorSprite;
    //public Sprite highlightCursorSprite; 
    private AudioSource clickSound;

    public void OnEnable()
    {
        InputSystem.EnableDevice(Mouse.current);
        Cursor.visible = false;
    }

    public void OnDisable()
    {
        InputSystem.DisableDevice(Mouse.current);
    }

    public void OnVisible()
    {
        cursorImage.enabled = true;
    }
    public void OnInvisible()
    {
        cursorImage.enabled = false;
    }

    private void Awake()
    {
        clickSound = GetComponent<AudioSource>();
    }
    public void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        cursorImage.rectTransform.position = mousePosition;
        Cursor.lockState = CursorLockMode.Confined;
        if(Input.GetMouseButtonDown(0))
        {
            clickSound.Play();
        }
    }

    bool ShouldHighlightCursor()
    {
        return false;
    }
}