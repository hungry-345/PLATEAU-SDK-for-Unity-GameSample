using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

namespace PLATEAU.Samples
{
    public class Contact : MonoBehaviour
    {
        // パーティクルエフェクト  
        [SerializeField] private GameObject getItemParticle;    
        private GameObject getItemParticleInstance;
        private float particleDuration = 2f;

        //サウンドエフェクト
        [SerializeField] private AudioClip getItemAudioClip;
        private AudioSource getItemSound;

        private GameManage GameManageScript;
        private GameView GameViewScript; 
        private ItemManage ItemManageScript;
        private ActionManager ActionManager;

        void Start()
        {
            GameManageScript = GameObject.Find("GameManager").GetComponent<GameManage>();
            ItemManageScript = GameObject.Find("ItemManager").GetComponent<ItemManage>();
            GameViewScript = GameObject.Find("GameView").GetComponent<GameView>();
            ActionManager = GameObject.Find("PlayerArmature").GetComponent<ActionManager>();

            //サウンドエフェクト
            // アイテム取得時
            getItemSound = gameObject.AddComponent<AudioSource>();
            getItemSound.clip = getItemAudioClip;
            getItemSound.loop = false;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag("Hint"))
            {
                //UIManageスクリプトのヒント関数を発動
                GameManageScript.GetHintItem();
                // パーティクルエフェクト
                getItemParticleInstance = Instantiate(getItemParticle, this.gameObject.transform.position, Quaternion.Euler(-90, 0, 0), this.gameObject.transform);
                Destroy(getItemParticleInstance, particleDuration);
                // サウンドエフェクト
                getItemSound.Play();
                //アイテムを削除
                ItemManageScript.GetItem(hit.gameObject);
            }
            if (hit.gameObject.CompareTag("Goal"))
            {
                //救助する
                GameManageScript.SelectBuildingAction(hit.transform);
            }
            if(hit.gameObject.CompareTag("TargetFlag"))
            {
                // 救助処理
                GameManageScript.SelectBuildingAction(hit.transform);

                // TimeManage から残り時間を取得
                float remaining = FindObjectOfType<TimeManage>().RemainingTime;

                // PlayerPrefs に保存（シーンを跨いでデータを保持できる）
                PlayerPrefs.SetFloat("RemainingTime", remaining);

                // デバッグログで確認
                Debug.Log("ゴール！ 残り時間: " + remaining);

                // リザルトシーンへ遷移
                UnityEngine.SceneManagement.SceneManager.LoadScene("ResultScene");
            }

        }
    }
}