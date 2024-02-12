using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PLATEAU.Samples
{
    public class TimeManage : MonoBehaviour
    {
        private float elapsedTime;
        private UIManage UIManageScript;
        private GameManage GameManageScript;
        private GameView GameViewScript;
        private bool isInitialiseFinish;
        private string itemName;
        private float generateSpan = 5f;
        public int countdownMinutes = 3;
        private float countdownSeconds;
        private Coroutine coloringCoroutine;
        private bool isCoroutine;
        
        public TimeSpan timeSpan;


        void Start()
        {
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
            GameManageScript = GameObject.Find("GameManager").GetComponent<GameManage>();
            GameViewScript = GameObject.Find("GameView").GetComponent<GameView>();

            countdownSeconds = countdownMinutes * 60f;
        }

        void Update()
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > generateSpan)
            {
                elapsedTime = 0f;
                GameManageScript.SpawnHintItem();
            }
            if(UIManageScript.isInitialiseFinish)
            {
                countdownSeconds -= Time.deltaTime;
                timeSpan = new TimeSpan(0, 0, (int)countdownSeconds);
                if(timeSpan != null && UIManageScript.timeLabel != null)
                {
                    UIManageScript.timeLabel.text = timeSpan.ToString(@"mm\:ss");
                }
            }
            if(countdownSeconds <= 0)
            {
                GameViewScript.isGameFinish = true;
            }

        }

        private IEnumerator Coloring(string gmlName,string nearestBuildingName,string hint)
        {
            isCoroutine = true;
            UIManageScript.ChangeColoring(gmlName,nearestBuildingName,hint);
            yield return new WaitForSeconds(20);
            UIManageScript.ChangeColoring("None",nearestBuildingName,hint);
            isCoroutine = false;
        }
        public void ColorBuilding(string gmlName,string nearestBuildingName,string hint)
        {
            if(isCoroutine)
            {
                StopCoroutine(coloringCoroutine);
                UIManageScript.ColorCode("None");
                isCoroutine = false;
            }
            coloringCoroutine = StartCoroutine(Coloring(gmlName,nearestBuildingName,hint));
        }
    }
}
