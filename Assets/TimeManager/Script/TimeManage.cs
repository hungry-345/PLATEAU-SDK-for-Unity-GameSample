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
        private bool isInitialiseFinish;
        private string itemName;
        private float span = 0.5f;

        void Start()
        {
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
            GameManageScript = GameObject.Find("GameManager").GetComponent<GameManage>();

            StartCoroutine(WatiForInitialise());
        }

        void Update()
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > span)
            {
                elapsedTime = 0f;
                GameManageScript.SpawnHintItem();
            }

        }

        private IEnumerator Coloring(string gmlName,string attributeValue)
        {
            UIManageScript.ChangeColoring(gmlName,attributeValue);
            yield return new WaitForSeconds(20);
            UIManageScript.ChangeColoring("None",attributeValue);
        }
        public void ColorBuilding(string gmlName,string attributeValue)
        {
            if(itemName == gmlName)
            {
                UIManageScript.ChangeColoring("None",attributeValue);
            }
            itemName = gmlName;
            StartCoroutine(Coloring(gmlName,attributeValue));
        }

        IEnumerator WatiForInitialise()
        {
            // yield return ->　ある関数が終わるまで待つ
            yield return new WaitUntil(() => IsInitialiseFinished());
        }
        private bool IsInitialiseFinished()
        {
            if(UIManageScript.isInitialiseFinish)
            {
                GameManageScript.SelectGoals();
                isInitialiseFinish = true;
            }
            return isInitialiseFinish;
        }
    }
}
