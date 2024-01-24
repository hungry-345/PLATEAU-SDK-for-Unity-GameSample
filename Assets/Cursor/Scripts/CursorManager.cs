using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    public Image cursorImage;              // UI Image
    //public Sprite defaultCursorSprite;
    //public Sprite highlightCursorSprite; 

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

    public void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        cursorImage.rectTransform.position = mousePosition;
    }

    bool ShouldHighlightCursor()
    {
        return false;
    }
}