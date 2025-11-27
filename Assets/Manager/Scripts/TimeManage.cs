using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PLATEAU.Samples
{
    public class TimeManage : MonoBehaviour
    {
        private float elapsedTime;     // 全体の経過時間
        private float spawnTimer;      // アイテム生成管理用
        private UIManage UIManageScript;
        private GameManage GameManageScript;
        private GameView GameViewScript;

        private float generateSpan = 5f;  // アイテム生成間隔
        public int countdownMinutes = 1;
        public float countdownSeconds;

        public TimeSpan timeSpan;

        public float ElapsedTime => elapsedTime; // 経過時間を外部から参照可能

        void Start()
        {
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
            GameManageScript = GameObject.Find("GameManager").GetComponent<GameManage>();
            GameViewScript = GameObject.Find("GameView").GetComponent<GameView>();

            countdownSeconds = 1 * 60f + 5; // 1分5秒（仮）
        }

        void Update()
        {
            // 全体の経過時間
            elapsedTime += Time.deltaTime;

            // 👇 経過時間をログに出す（mm:ss形式でも出せる）
            TimeSpan elapsedSpan = TimeSpan.FromSeconds(elapsedTime);
            //Debug.Log($"[TimeManage] 経過時間: {elapsedSpan:mm\\:ss} ({elapsedTime:F2} 秒)");

            // アイテム生成のためのカウント
            spawnTimer += Time.deltaTime;
            if (spawnTimer > generateSpan)
            {
                spawnTimer = 0f;
                GameManageScript.SpawnHintItem();
            }

            // 残り時間処理
            countdownSeconds -= Time.deltaTime;
            timeSpan = new TimeSpan(0, 0, (int)countdownSeconds);

            if (UIManageScript.timeLabel != null)
            {
                UIManageScript.timeLabel.text = timeSpan.ToString(@"mm\:ss");
            }

            if (countdownSeconds <= 0)
            {
                // まだ距離を保存していなければ保存
                if (DistanceChecker.Instance != null)
                {
                    DistanceChecker.Instance.SaveDistanceAtTimeUp();
                }

                GameViewScript.isGameFinish = true;
                countdownSeconds = 0f;
                timeSpan = TimeSpan.Zero;

                if (UIManageScript.timeLabel != null)
                {
                    UIManageScript.timeLabel.text = timeSpan.ToString(@"mm\:ss");
                }

                // シーン移動
                SceneManager.LoadScene("NoResult");
            }

        }
    }
}
