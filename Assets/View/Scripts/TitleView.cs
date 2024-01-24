using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.UIElements;
public class TitleView : ViewBase
{
    [SerializeField, Tooltip("タイトルUI")] private UIDocument titleUI;
    [SerializeField, Tooltip("タイトルUI")] private UIDocument explanationUI;
    private bool isStart;
    private bool isExplatation;
    private Button startButton;
    private Button explanationButton;
    private Button cancelButton;

    private void Start()
    {
        isStart = false;
        startButton = titleUI.rootVisualElement.Query<Button>("StartButton");
        startButton.clicked += OnStart;
        explanationButton = titleUI.rootVisualElement.Query<Button>("ExplanationButton");
        explanationButton.clicked += OnExplanation;
        explanationUI.enabled = false;
    }

    private void OnStart()
    {
        isStart = true;
    }
    private void OnExplanation()
    {
        explanationUI.enabled = !(explanationUI.enabled);
        //説明ウィンドウ表示時の処理
        if (explanationUI.enabled)
        {
            cancelButton = explanationUI.rootVisualElement.Query<Button>("CancelButton");
            cancelButton.clicked += OnExplanation;
        }
    }
    public override IEnumerator Wait()
    {
        while (true)
        {
            //ボタン入力待ち状態にする
            if (isStart)
            {
                yield break;
            }

            yield return null;
        }
    }

}
