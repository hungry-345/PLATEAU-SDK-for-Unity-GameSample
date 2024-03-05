using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.UIElements;
public class TitleView : ViewBase
{
    [SerializeField, Tooltip("タイトルUI")] private UIDocument titleUI;
    [SerializeField, Tooltip("タイトルUI")] private UIDocument explanationUI_Page1;
    [SerializeField, Tooltip("タイトルUI")] private UIDocument explanationUI_Page2;
    [SerializeField, Tooltip("タイトルUI")] private UIDocument explanationUI_Page3;
    [SerializeField, Tooltip("タイトルUI")] private UIDocument explanationUI_Page4;
    private UIDocument explanationUI;
    private UIDocument nextExplanationUI;
    private bool isStart;
    private bool isExplatation;
    private Button startButton;
    private Button endGameButton;
    private Button explanationButton;
    private Button cancelButton;
    private Button previousButton;
    private Button nextButton;
    private int pageState;
    //ボタンサウンド
    private CursorManager cursorManager;
    

    private void Start()
    {
        isStart = false;
        startButton = titleUI.rootVisualElement.Query<Button>("StartButton");
        startButton.clicked += OnStart;
        endGameButton = titleUI.rootVisualElement.Query<Button>("EndGameButton");
        endGameButton.clicked += OnEndGame;
        explanationButton = titleUI.rootVisualElement.Query<Button>("ExplanationButton");
        explanationButton.clicked += OnExplanation;
        explanationUI = explanationUI_Page1;
        explanationUI_Page1.enabled = false;
        explanationUI_Page2.enabled = false;
        explanationUI_Page3.enabled = false;
        explanationUI_Page4.enabled = false;
        cursorManager = GameObject.Find("Cursor").GetComponent<CursorManager>();
    }

    private void OnStart()
    {
        cursorManager.CursorSoundPlay();
        isStart = true;
    }

//    private IEnumerator OnEndGame()
//    {
//        cursorManager.CursorSoundPlay();
//        yield return new WaitForSeconds(cursorManager.GetSoundLength());

//#if     UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
        
        
//#else
        
//        Application.Quit();//ゲームプレイ終了
        
//#endif
        
//    }

    private void OnEndGame()
    {
        StartCoroutine(EndGameCoroutine()); // コルーチンを開始
    }

    private IEnumerator EndGameCoroutine()
    {
        cursorManager.CursorSoundPlay(); // 音声再生を開始
        yield return new WaitForSeconds(cursorManager.GetSoundLength()); // 音声が終了するまで待機

        // ゲーム終了処理
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Unity エディタでのゲームプレイを終了
#else
    Application.Quit(); // アプリケーションを終了
#endif
    }

    private void OnExplanation()
    {
        cursorManager.CursorSoundPlay();
        cursorManager.OnVisible();
        explanationUI.enabled = !(explanationUI.enabled);
        if(!explanationUI.enabled)
        {
            explanationUI = explanationUI_Page1;
        }
        //説明ウィンドウ表示時の処理
        if (explanationUI.enabled)
        {
            pageState = 1;
            cancelButton = explanationUI.rootVisualElement.Query<Button>("CancelButton");
            nextButton = explanationUI.rootVisualElement.Query<Button>("NextButton");
            nextButton.clicked += OnExplanationNext;
            cancelButton.clicked += OnExplanation;
        }
    }
    private void OnExplanationNext()
    {
        cursorManager.CursorSoundPlay();
        cursorManager.OnVisible();
        pageState += 1;
        explanationUI.enabled = false;
        if(pageState == 2)
        {
            explanationUI = explanationUI_Page2;
            explanationUI.enabled = true;
            cancelButton = explanationUI.rootVisualElement.Query<Button>("CancelButton");
            nextButton = explanationUI.rootVisualElement.Query<Button>("NextButton");
            previousButton = explanationUI.rootVisualElement.Query<Button>("PreviousButton");
            previousButton.clicked += OnExplanationPrevious;
        }
        if(pageState == 3)
        {
            explanationUI = explanationUI_Page3;
            explanationUI.enabled = true;
            cancelButton = explanationUI.rootVisualElement.Query<Button>("CancelButton");
            nextButton = explanationUI.rootVisualElement.Query<Button>("NextButton");
            previousButton = explanationUI.rootVisualElement.Query<Button>("PreviousButton");
            previousButton.clicked += OnExplanationPrevious;
        }
        if(pageState == 4)
        {
            explanationUI = explanationUI_Page4;
            explanationUI.enabled = true;
            cancelButton = explanationUI.rootVisualElement.Query<Button>("CancelButton");
            previousButton = explanationUI.rootVisualElement.Query<Button>("PreviousButton");
            previousButton.clicked += OnExplanationPrevious;
        }
        nextButton.clicked += OnExplanationNext;
        cancelButton.clicked += OnExplanation;
    }
    private void OnExplanationPrevious()
    {
        cursorManager.CursorSoundPlay();
        cursorManager.OnVisible();
        pageState -= 1;
        explanationUI.enabled = false;
        if(pageState == 1)
        {
            explanationUI = explanationUI_Page1;
            explanationUI.enabled = true;
            cancelButton = explanationUI.rootVisualElement.Query<Button>("CancelButton");
            nextButton = explanationUI.rootVisualElement.Query<Button>("NextButton");
        }
        if(pageState == 2)
        {
            explanationUI = explanationUI_Page2;
            explanationUI.enabled = true;
            cancelButton = explanationUI.rootVisualElement.Query<Button>("CancelButton");
            nextButton = explanationUI.rootVisualElement.Query<Button>("NextButton");
            previousButton = explanationUI.rootVisualElement.Query<Button>("PreviousButton");
            previousButton.clicked += OnExplanationPrevious;
        }
        if(pageState == 3)
        {
            explanationUI = explanationUI_Page3;
            explanationUI.enabled = true;
            cancelButton = explanationUI.rootVisualElement.Query<Button>("CancelButton");
            nextButton = explanationUI.rootVisualElement.Query<Button>("NextButton");
            previousButton = explanationUI.rootVisualElement.Query<Button>("PreviousButton");
            previousButton.clicked += OnExplanationPrevious;
        }
        nextButton.clicked += OnExplanationNext;
        cancelButton.clicked += OnExplanation;
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
