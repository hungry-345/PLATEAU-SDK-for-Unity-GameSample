using PLATEAU.CityInfo;
using PLATEAU.Util.Async;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

namespace PLATEAU.Samples
{
    public class UIManage : MonoBehaviour
    {
        public struct BuildingInfo
        {
            public Label heightLabel;
            public Label capacityLabel;
        }
        [SerializeField, Tooltip("初期化中UI")] private UIDocument initializingUi;
        [SerializeField, Tooltip("ベースUI")] public UIDocument baseUi;
        [SerializeField, Tooltip("選択中のオブジェクトの色")] private Color selectedColor;
        [SerializeField] private AudioClip startAudioClip;
        private AudioSource startSound;
        [SerializeField] public Camera PlayerPosCamera;
        public Label timeLabel;
        public Label rescuedNumLabel;
        public Label rescuingNumLabel;
        private GameManage GameManageScript;
        private TimeManage TimeManageScript;
        // UI Document のラベル
        public Dictionary<string,BuildingInfo> BuildingInfoDict = new Dictionary<string, BuildingInfo>();
        private Label Shelter1HeightLabel;
        private Label Shelter1CapacityLabel;
        private Label Shelter2HeightLabel;
        private Label Shelter2CapacityLabel;
        private Label Shelter3HeightLabel;
        private Label Shelter3CapacityLabel;
        private Label MissionLabel;
        private Label StartText;
        private string correctBuildingName;
        private string filterStatus;
        private ActionManager actionManager;
        // -------------------------------------------------------------------------------------------------------------
        private void Awake()
        {
            actionManager = GameObject.Find("PlayerArmature").GetComponent<ActionManager>();

            startSound = gameObject.AddComponent<AudioSource>();
            startSound.clip = startAudioClip;
            startSound.loop = false;
        }

        // -------------------------------------------------------------------------------------------------------------
        public void InitializeUI()
        {
            GameManageScript = GameObject.Find("GameManager").GetComponent<GameManage>();
            TimeManageScript = GameObject.Find("TimeManager").GetComponent<TimeManage>();

            // ゴールの位置を設定する
            GameManageScript.SelectGoal();
            StartCoroutine(WatiForInitialise());

            //変数の初期化
            filterStatus = "None";
            correctBuildingName = "";
        }
        IEnumerator WatiForInitialise()
        {
            initializingUi.gameObject.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            initializingUi.gameObject.SetActive(false);
            baseUi.gameObject.SetActive(true);
            startSound.Play();

            timeLabel = baseUi.rootVisualElement.Q<Label>("Time");
            rescuedNumLabel = baseUi.rootVisualElement.Q<Label>("Rescued_Count");
            rescuingNumLabel = baseUi.rootVisualElement.Q<Label>("Rescuing_Count");
            Shelter1HeightLabel = baseUi.rootVisualElement.Q<Label>("Shelter1_Height");
            Shelter1CapacityLabel = baseUi.rootVisualElement.Q<Label>("Shelter1_Capacity");
            Shelter2HeightLabel = baseUi.rootVisualElement.Q<Label>("Shelter2_Height");
            Shelter2CapacityLabel = baseUi.rootVisualElement.Q<Label>("Shelter2_Capacity");
            Shelter3HeightLabel = baseUi.rootVisualElement.Q<Label>("Shelter3_Height");
            Shelter3CapacityLabel = baseUi.rootVisualElement.Q<Label>("Shelter3_Capacity");
            MissionLabel = baseUi.rootVisualElement.Q<Label>("Mission_Text");

            EditMissionText();
            actionManager.ChangeNormal();
            yield return new WaitForSeconds(2.0f);
            
            StartText = baseUi.rootVisualElement.Q<Label>("StartText");
            StartText.text = "";
        }

        /// <summary>
        /// アイテムを取得した時の処理(Contact.csに参照されている)
        /// </summary>
        public void DisplayAnswer(string hintBuildingName,string hintBuildingHeight,string hintBuildingCapacity,string hintBuildingEvacuee)
        {
            if(Shelter1HeightLabel.text == "")
            {
                Shelter1HeightLabel.text = hintBuildingHeight;
                Shelter1CapacityLabel.text = hintBuildingEvacuee + "/" + hintBuildingCapacity;
                BuildingInfo buildingInfo = new BuildingInfo {heightLabel=Shelter1HeightLabel,capacityLabel=Shelter1CapacityLabel};
                BuildingInfoDict.Add(hintBuildingName,buildingInfo);
            }
            else if(Shelter2HeightLabel.text == "")
            {
                Shelter2HeightLabel.text = hintBuildingHeight;
                Shelter2CapacityLabel.text = hintBuildingEvacuee + "/" + hintBuildingCapacity;
                BuildingInfo buildingInfo = new BuildingInfo {heightLabel=Shelter2HeightLabel,capacityLabel=Shelter2CapacityLabel};
                BuildingInfoDict.Add(hintBuildingName,buildingInfo);
            }
            else if(Shelter3HeightLabel.text == "")
            {
                Shelter3HeightLabel.text = hintBuildingHeight;
                Shelter3CapacityLabel.text = hintBuildingEvacuee + "/" + hintBuildingCapacity;
                BuildingInfo buildingInfo = new BuildingInfo {heightLabel=Shelter3HeightLabel,capacityLabel=Shelter3CapacityLabel};
                BuildingInfoDict.Add(hintBuildingName,buildingInfo);
            }

            // 建物の色を変更
            GameObject building = GameObject.Find(hintBuildingName);
            Renderer renderer = building.GetComponent<Renderer>();
            for (int i = 0; i < renderer.materials.Length; ++i)
            {
                renderer.materials[i].color = selectedColor;

                renderer.materials[i].EnableKeyword("_EMISSION");

                renderer.materials[i].SetColor("_EmissionColor", selectedColor);
               
            }
            // ミッションのメッセージの変更
            EditMissionText();
        }
        public void DeleteAnswer(string deleteBuildingName)
        {
            GameObject building = GameObject.Find(deleteBuildingName);
            Renderer renderer = building.GetComponent<Renderer>();
            for (int i = 0; i < renderer.materials.Length; ++i)
            {
                renderer.materials[i].color = Color.white;
                renderer.materials[i].DisableKeyword("_EMISSION");
            }

            if(Shelter1HeightLabel.text == BuildingInfoDict[deleteBuildingName].heightLabel.text)
            {
                Shelter1HeightLabel.text = "";
                Shelter1CapacityLabel.text = "";
            }
            else if(Shelter2HeightLabel.text == BuildingInfoDict[deleteBuildingName].heightLabel.text)
            {
                Shelter2HeightLabel.text = "";
                Shelter2CapacityLabel.text = "";
            }
            else if(Shelter3HeightLabel.text == BuildingInfoDict[deleteBuildingName].heightLabel.text)
            {
                Shelter3HeightLabel.text = "";
                Shelter3CapacityLabel.text = "";
            }
            if(BuildingInfoDict.ContainsKey(deleteBuildingName))
            {
                BuildingInfoDict.Remove(deleteBuildingName);
            }

        }
        public void SelectCityObject(Transform buildingTrans)
        {
            if (buildingTrans == null || buildingTrans.parent.parent == null)
            {
                return;
            }
            rescuedNumLabel.text = GameManageScript.rescuedNum.ToString();
            rescuingNumLabel.text = GameManageScript.rescuingNum.ToString();
            if (BuildingInfoDict.ContainsKey(buildingTrans.name))
            {
                BuildingInfoDict[buildingTrans.name].capacityLabel.text = GameManageScript.GoalAttributeDict[buildingTrans.name].evacueeNum + "/" + GameManageScript.GoalAttributeDict[buildingTrans.name].capacity;
            }
        }
        public void DisplayRescuedNum()
        {
            rescuedNumLabel.text = GameManageScript.rescuedNum.ToString();
        }
        public void DisplayRescuingNum()
        {
            rescuingNumLabel.text = GameManageScript.rescuingNum.ToString();
            EditMissionText();
        }
        public void EditMissionText()
        {
            if(GameManageScript.rescuingNum == 0)
            {
                MissionLabel.text = "要救助者を見つけよう!";
            }
            else if(BuildingInfoDict.Count == 0)
            {
                MissionLabel.text = "紙飛行機を拾って避難場所の位置を調べよう!";
            }
            else
            {
                MissionLabel.text = "避難場所に連れて行こう!";
            }
        }
        public void HideGameUI()
        {
            baseUi.gameObject.SetActive(false);
        }
    }
}