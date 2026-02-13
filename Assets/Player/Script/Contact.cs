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
                // ヒント取得処理
                GameManageScript.GetHintItem();

                // パーティクルエフェクト
                getItemParticleInstance = Instantiate(getItemParticle, transform.position, Quaternion.Euler(-90, 0, 0), transform);
                Destroy(getItemParticleInstance, particleDuration);

                // サウンド再生
                getItemSound.Play();

                // アイテム削除
                ItemManageScript.GetItem(hit.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Target"))
            {
                // TimeManage の経過時間を取得
                float elapsed = FindObjectOfType<TimeManage>().ElapsedTime;
                PlayerPrefs.SetFloat("ElapsedTime", elapsed);

                Debug.Log("ゴール！ 経過時間: " + elapsed);

                // リザルトシーンへ遷移
                UnityEngine.SceneManagement.SceneManager.LoadScene("ResultScene");
            }
        }
    }
}