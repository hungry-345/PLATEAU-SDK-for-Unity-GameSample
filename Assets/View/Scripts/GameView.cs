using PLATEAU.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameView : ViewBase
{

    [SerializeField, Tooltip("ゲームオーバーUI")] private UIDocument GameOvergUi;
    private bool IsClicked;
    private Button startButton;

    public bool isGameClear = false;  //ゲームクリアフラグ
    public bool isGameOver = false;   //ゲームオーバーフラグ


    [SerializeField] private GameManage gameManage;
    [SerializeField] private Canvas gameEndCanvas;
    [SerializeField] private ExtendButton toTitleButton;  //タイトルに戻るボタン
    //[SerializeField] private Text gameEndText;  //ゲーム終了テキスト
    //[SerializeField] private Text scoreText;  //スコアテキスト



    void Start()
    {
        //ゲーム終了UIを非表示にする
        gameEndCanvas.enabled=false;
        //ゲーム開始
        gameManage.StartGame();
    }
    public override IEnumerator Wait()
    {
        while (true)
        {
            //ゲーム終了
            if(isGameOver||isGameClear)
            {
                //★ゾンビやアイテムを消す
                gameManage.OnEndGame();
                //★プレイヤーを操作できなくする


                //ゲーム終了UIを表示
                gameEndCanvas.enabled=true;
                //ゲームオーバーテキスト
                //gameEndText.text = "Game Over";
                if (isGameClear)
                {
                    //クリアテキスト
                    //gameEndText.text = "Game Clear!";

                    //スコア表示★スコア取得用の関数を作成する
                    //scoreText.text = "SCORE　"+ViewManager.instance.score;
                }

                //ボタン入力待ち状態にする
                while (true)
                {
                    if (toTitleButton.IsClicked)  //タイトルボタン
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
}