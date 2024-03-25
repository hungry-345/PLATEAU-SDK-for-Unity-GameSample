using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

namespace PLATEAU.Samples
{
    public class Contact : MonoBehaviour
    {
        // パーティクルエフェクト
        [SerializeField] private GameObject saveParticle;
        [SerializeField] private GameObject getItemParticle;
        private GameObject saveParticleInstance;
        private GameObject getItemParticleInstance;
        private float particleDuration = 2f;
        //サウンドエフェクト
        [SerializeField] private AudioClip getItemAudioClip;
        private AudioSource getItemSound;
        [SerializeField] private AudioClip saveAudioClip;
        private AudioSource saveSound;

        private GameManage GameManageScript;
        private GameView GameViewScript;
        private UIManage UIManageScript;
        private ItemManage ItemManageScript;
        private ThirdPersonController ThirdPersonControllerScript;
        private ActionManager ActionManager;

        void Start()
        {
            ThirdPersonControllerScript = this.GetComponent<ThirdPersonController>();
            GameManageScript = GameObject.Find("GameManager").GetComponent<GameManage>();
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
            ItemManageScript = GameObject.Find("ItemManager").GetComponent<ItemManage>();
            GameViewScript = GameObject.Find("GameView").GetComponent<GameView>();
            ActionManager = GameObject.Find("PlayerArmature").GetComponent<ActionManager>();

            //サウンドエフェクト
            // アイテム取得時
            getItemSound = gameObject.AddComponent<AudioSource>();
            getItemSound.clip = getItemAudioClip;
            getItemSound.loop = false;
            // 救出時
            saveSound = gameObject.AddComponent<AudioSource>();
            saveSound.clip = saveAudioClip;
            saveSound.loop = false;
        }
        public void GameOverFunc()
        {
            ActionManager.state = ActionManager.State.Died;
            //ThirdPersonControllerScript.DyingMotion();
            //一番上の親（GameView）にゲームオーバーを通知
            GameViewScript.isGameOver = true ; 
        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if(hit.gameObject.CompareTag ("Hint"))
            {
                //UIManageスクリプトのヒント関数を発動
                GameManageScript.GetHintItem();
                // パーティクルエフェクト
                getItemParticleInstance = Instantiate(getItemParticle,this.gameObject.transform.position,Quaternion.Euler(-90,0,0),this.gameObject.transform);
                Destroy(getItemParticleInstance,particleDuration);
                // サウンドエフェクト
                getItemSound.Play();
                //アイテムを削除
                ItemManageScript.GetItem(hit.gameObject);
            }
            if(hit.gameObject.CompareTag("Goal"))
            {
                //救助する
                GameManageScript.SelectBuildingAction(hit.transform);
                // パーティクルエフェクト
                saveParticleInstance = Instantiate(saveParticle,this.gameObject.transform.position,Quaternion.Euler(-90,0,0),this.gameObject.transform);
                Destroy(saveParticleInstance,particleDuration);
                // サウンドエフェクト
                saveSound.Play();
            }
        }
    }
}