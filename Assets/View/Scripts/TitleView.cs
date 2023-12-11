using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.UIElements;
public class TitleView : ViewBase
{
    [SerializeField, Tooltip("スタートUI")] private UIDocument StartgUi;
    private bool IsClicked;
    private Button startButton;

    private void Start()
    {
        IsClicked = false;
        startButton = StartgUi.rootVisualElement.Query<Button>();
        startButton.clicked += OnButtonClicked;
    }

    private void OnButtonClicked()
    {
        IsClicked = true;
    }
    public override IEnumerator Wait()
    {
        while (true)
        {
            //�Q�[���X�^�[�g
            if (IsClicked)
            {
                yield break;
            }

            yield return null;
        }
    }

}
