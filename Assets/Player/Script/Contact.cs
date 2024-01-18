using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

namespace PLATEAU.Samples
{
    public class Contact : MonoBehaviour
    {
        private GameManage GameManageScript;
        private GameView GameViewScript;
        private UIManage UIManageScript;
        private ThirdPersonController ThirdPersonControllerScript;

        //★GameViewスクリプトを参照する
        void Start()
        {
            ThirdPersonControllerScript = this.GetComponent<ThirdPersonController>();
            GameManageScript = GameObject.Find("GameManager").GetComponent<GameManage>();
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
            GameViewScript = GameObject.Find("GameView").GetComponent<GameView>();
        }
        public void GameOverFunc()
        {
                ThirdPersonControllerScript.DyingMotion();
                //★一番上の親（GameView）にゲームオーバーを通知
                GameViewScript.isGameOver = true ; 
        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if(hit.gameObject.tag == "Hint")
            {
                //UIManageスクリプトのヒント関数を発動
                GameManageScript.GetHintItem(hit.gameObject.name);
                //アイテムを削除
                Destroy(hit.gameObject);
            }
            if(hit.gameObject.name == "HumanBody")
            {
                Debug.Log("Contact");
                GameManageScript.ContactHumanAction();
                UIManageScript.DisplayRescuingNum();
            }
            if(hit.gameObject.name == "zombie")
            {
                ThirdPersonControllerScript.DyingMotion();
                //★一番上の親（GameView）にゲームオーバーを通知
                GameViewScript.isGameOver = true ; 

                //#if UNITY_EDITOR
                //    UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
                //#else
                //    Application.Quit();//ゲームプレイ終了
                //#endif
            }
            // if(hit.gameObject.tag == "TargetFlag")
            // {
            //     //★一番上の親（GameView）にゲームクリアを通知
            //     // GameViewScript.isGameClear = true;
            //     //★スコアを加算（例）TODO:直接値を変えるのは望ましくないため、ViewManager側でスコア加算用の関数を作成する
            //     GameManageScript.rescuedNum += int.Parse(GameManageScript.GoalAttributeDict[hit.gameObject.name].saboveground);
            //     UIManageScript.rescuedNumLabel.text = GameManageScript.rescuedNum.ToString();

            //     Destroy(hit.gameObject);
            //     GameManageScript.AddGoals(hit.gameObject.name);
            //     //#if UNITY_EDITOR
            //     //    UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
            //     //#else
            //     //    Application.Quit();//ゲームプレイ終了
            //     //#endif
            // }

        }
    }
}