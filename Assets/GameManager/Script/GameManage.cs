using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using StarterAssets;

//ゲームマネージャー
namespace PLATEAU.Samples
{
    public class GameManage : MonoBehaviour
    {
        private UIManage UIManageScript;
        private TimeManage TimeManageScript;
        private EnemyManager enemyManager;
        private ItemManager itemManager;
        public int rescuedNum;

        //プレイヤーのコントローラー関数
        private ThirdPersonController thirdpersonController;
        //ゲームの開始処理
        public void StartGame()
        {
            //rnd = new System.Random();
            //inputActions.InputGame.AddCallbacks(this);
            //ゲーム開始/終了時にプレイヤーを操作不能にするために取得
            thirdpersonController = GameObject.Find("PlayerArmature").GetComponent<ThirdPersonController>();

            //SceneManagerから各マネージャースクリプトにアクセスする
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
            TimeManageScript = GameObject.Find("TimeManager").GetComponent<TimeManage>();
            enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
            itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();

            rescuedNum = 0;

            UIManageScript.InitializeGML();
            enemyManager.InitializeEnemy();
            itemManager.InitializeItem();
        }
        /// <summary>
        /// アイテムを生成する
        /// </summary>
        public void SpawnHintItem()
        {
            itemManager.GenerateItem();
        }

        //ゲームの終了処理
        public void EndGame()
        {
            enemyManager.DestroyEnemy();
            itemManager.DestroyItem();
            UIManageScript.PlayerPosCamera.enabled = false;          
            UIManageScript.HideGameUI();
            thirdpersonController.enabled = false;
        }
    }
}
