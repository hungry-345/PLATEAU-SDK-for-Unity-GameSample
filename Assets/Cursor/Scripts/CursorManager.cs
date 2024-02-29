using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    // public Image cursorImage;              // UI Image
    //public Sprite defaultCursorSprite;
    //public Sprite highlightCursorSprite; 
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

    public void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        // cursorImage.rectTransform.position = mousePosition;
        Cursor.lockState = CursorLockMode.Confined;
    }

    bool ShouldHighlightCursor()
    {
        return false;
    }
}