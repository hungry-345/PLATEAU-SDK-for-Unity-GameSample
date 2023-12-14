using PLATEAU.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameView : ViewBase
{
    [SerializeField, Tooltip("ゲームオーバーUI")] private UIDocument gameOverUI;
    private bool IsClicked; //ボタンが押されたか
    private Button toTitleButton;

    public bool isGameClear = false;  //ゲームクリアフラグ
    public bool isGameOver = false;   //ゲームオーバーフラグ


    [SerializeField] private GameManage gameManage;
    //s[SerializeField] private Canvas gameEndCanvas;
    //[SerializeField] private ExtendButton toTitleButton;  //タイトルに戻るボタン
    //[SerializeField] private Text gameEndText;  //ゲーム終了テキスト
    //[SerializeField] private Text scoreText;  //スコアテキスト



    void Start()
    {
        Debug.Log("Game View Start");
        //ゲーム開始
        gameManage.StartGame();

        IsClicked = false;
        toTitleButton = gameOverUI.rootVisualElement.Query<Button>();
        toTitleButton.clicked += OnButtonClicked;

        //スタート時はUIを非表示にする
        gameOverUI.enabled = false;

        //ゲーム終了UIを非表示にする
        //gameEndCanvas.enabled=false;
    }

    public override IEnumerator Wait()
    {
        while (true)
        {
            //ゲーム終了
            if(isGameOver||isGameClear)
            {
                //ゾンビやアイテムを消す・プレイヤーを操作できなくする
                gameManage.OnEndGame();

                //ゲームオーバーテキスト
                //gameEndText.text = "Game Over";
                if (isGameClear)
                {
                    //クリアテキスト
                    //gameEndText.text = "Game Clear!";

                    //スコア表示★スコア取得用の関数を作成する
                    //scoreText.text = "SCORE　"+ViewManager.instance.score;
                }
                else
                {
                    //ゲームオーバーUIを表示
                    gameOverUI.enabled = true;
                }

                //ボタン入力待ち状態にする
                while (true)
                {
                    if (IsClicked)  //タイトルボタン
                    {
                        //ゲーム終了
                        yield break;
                    }

                    yield return null;
                }

            }
            yield return null;
        }
    }
    private void OnButtonClicked()
    {
        IsClicked = true;
    }
}