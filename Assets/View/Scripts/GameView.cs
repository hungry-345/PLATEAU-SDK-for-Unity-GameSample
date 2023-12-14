using PLATEAU.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameView : ViewBase
{


    [SerializeField] private Canvas gameEndCanvas;
    [SerializeField] private ExtendButton retryButton;  //���g���C�{�^��
    [SerializeField] private ExtendButton toTitleButton;  //�^�C�g���ɖ߂�{�^��
    [SerializeField] private Text gameEndText;  //�Q�[���I���e�L�X�g
    [SerializeField] private Text scoreText;  //�X�R�A�e�L�X�g
    [SerializeField] private GameManage gameManage;
    [SerializeField, Tooltip("ゲームオーバーUI")] private UIDocument gameOverUI;
    private bool IsClicked; //ボタンが押されたか
    private Button toTitleButton;
    public bool isGameClear = false;  //ゲームクリアフラグ
    public bool isGameOver = false;   //ゲームオーバーフラグ

    private GameObject cursor;

    private CursorManager cursorManage;

    void Awake()
    {
        cursor = GameObject.Find("Cursor");
        cursorManage = cursor.GetComponent<CursorManager>();
    }

    void Start()
    {
                        //ゲーム開始
        gameManage.StartGame();
        cursorManage.OnInvisible();
        Cursor.lockState = CursorLockMode.Confined;
        gameEndCanvas.enabled=false;


        IsClicked = false;
        //toTitleButton = gameOverUI.rootVisualElement.Query<Button>();
        //toTitleButton.clicked += OnButtonClicked;

        //スタート時はUIを非表示にする
        gameOverUI.enabled = false;

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
                cursorManage.OnVisible();

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
                    toTitleButton = gameOverUI.rootVisualElement.Query<Button>();
                    toTitleButton.clicked += OnButtonClicked;
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