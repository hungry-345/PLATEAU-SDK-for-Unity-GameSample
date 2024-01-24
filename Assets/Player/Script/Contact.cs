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
        private ItemManage ItemManageScript;
        private ThirdPersonController ThirdPersonControllerScript;

        void Start()
        {
            ThirdPersonControllerScript = this.GetComponent<ThirdPersonController>();
            GameManageScript = GameObject.Find("GameManager").GetComponent<GameManage>();
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
            ItemManageScript = GameObject.Find("ItemManager").GetComponent<ItemManage>();
            GameViewScript = GameObject.Find("GameView").GetComponent<GameView>();
        }
        public void GameOverFunc()
        {
            ThirdPersonControllerScript.DyingMotion();
            //一番上の親（GameView）にゲームオーバーを通知
            GameViewScript.isGameOver = true ; 
        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if(hit.gameObject.tag == "Hint")
            {
                //UIManageスクリプトのヒント関数を発動
                GameManageScript.GetHintItem(hit.gameObject.name);
                //アイテムを削除
                ItemManageScript.GetItem(hit.gameObject);
            }
            if (hit.gameObject.name == "HumanBody")
            {
                GameManageScript.ContactHumanAction();
                //UIManageScript.DisplayRescuingNum();
            }
            if (hit.gameObject.name == "zombie")
            {
                ThirdPersonControllerScript.DyingMotion();
                //一番上の親（GameView）にゲームオーバーを通知
                GameViewScript.isGameOver = true ; 

                //#if UNITY_EDITOR
                //    UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
                //#else
                //    Application.Quit();//ゲームプレイ終了
                //#endif
            }
        }
    }
}