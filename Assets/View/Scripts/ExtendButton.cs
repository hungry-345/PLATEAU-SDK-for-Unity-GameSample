using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendButton : MonoBehaviour
{
    // Start is called before the first frame update
    public UnityEngine.UI.Button ButtonInternal;
    public UnityEngine.UI.Image ImageInternal;
    public UnityEngine.UI.Text TextInternal;
    public Sprite Sprite
    {
        get => ImageInternal?.sprite;
        set
        {
            if (ImageInternal != null)
                ImageInternal.sprite = value;
        }
    }
    public string Text
    {
        get => TextInternal?.text;
        set
        {
            if (TextInternal != null)
                TextInternal.text = value;
        }
    }
    public bool IsClicked { get; set; }

    void Start()
    {
        if (ButtonInternal != null)
        {
            ButtonInternal.onClick.AddListener(OnClick);
        }
        
    }
    void Update()
    {
        //IsClicked = false;
    }
    void OnClick()
    {
        IsClicked = true;
        Debug.Log("�{�^���N���b�N");
        
    }
}
