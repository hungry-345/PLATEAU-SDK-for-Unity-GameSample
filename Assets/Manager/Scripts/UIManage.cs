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
        // ゴールの建物の属性情報を保持する構造体
        public struct BuildingInfo
        {
            public Label heightLabel;
            public Label capacityLabel;
        }
        // ゴールの建物と属性情報を関連図ける辞書
        public Dictionary<string,BuildingInfo> BuildingInfoDict = new Dictionary<string, BuildingInfo>();
        // ゴールの建物の色
        [SerializeField, Tooltip("ゴールの建物の色")] private Color selectedColor;
        // UI
        [SerializeField, Tooltip("初期化中UI")] private UIDocument initializingUi;
        [SerializeField, Tooltip("ベースUI")] public UIDocument baseUi;
        // サウンドエフェクト
        [SerializeField] private AudioClip startAudioClip;
        private AudioSource startSound;
        // マップカメラ
        [SerializeField] public Camera MapCamera;
        // 参照スクリプト
        private GameManage GameManageScript;
        private TimeManage TimeManageScript;
        private ActionManager actionManager;
        // UI Document のラベル
        public Label timeLabel;
        public Label rescuingNumLabel;
        public Label rescuedNumLabel;
        private Label Shelter1HeightLabel;
        private Label Shelter1CapacityLabel;
        private Label Shelter2HeightLabel;
        private Label Shelter2CapacityLabel;
        private Label Shelter3HeightLabel;
        private Label Shelter3CapacityLabel;
        private Label MissionLabel;
        private Label StartText;
        public void InitializeUI()
        {
            actionManager = GameObject.Find("PlayerArmature").GetComponent<ActionManager>();
            GameManageScript = GameObject.Find("GameManager").GetComponent<GameManage>();
            TimeManageScript = GameObject.Find("TimeManager").GetComponent<TimeManage>();
            // スタート時のSEの初期化
            startSound = gameObject.AddComponent<AudioSource>();
            startSound.clip = startAudioClip;
            startSound.loop = false;
            // ゴールの位置を設定する
            GameManageScript.SelectGoal();
            StartCoroutine(WatiForInitialise());
        }
        IEnumerator WatiForInitialise()
        {
            // UIの切り替え
            initializingUi.gameObject.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            initializingUi.gameObject.SetActive(false);
            baseUi.gameObject.SetActive(true);
            // ラベルの初期化
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

            actionManager.ChangeNormal();
            startSound.Play();

            EditMissionText();
            // スタートテキストを2秒後に消す
            yield return new WaitForSeconds(2.0f);
            StartText = baseUi.rootVisualElement.Q<Label>("StartText");
            StartText.text = "";
        }

        /// <summary>
        /// アイテムを取得した時の処理(Contact.csに参照されている)
        /// </summary>
        public void DisplayAnswer(string hintBuildingName,string hintBuildingHeight,string hintBuildingCapacity,string hintBuildingEvacuee)
        {
            // 建物の高さ、収容可能人数、収容人数の表示
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

            // 建物の高さ、収容可能人数、収容人数の表示を消す
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
            // 建物の色を元に戻す
            for (int i = 0; i < renderer.materials.Length; ++i)
            {
                renderer.materials[i].color = Color.white;
                renderer.materials[i].DisableKeyword("_EMISSION");
            }
            // ゴールの情報を辞書から消す
            if(BuildingInfoDict.ContainsKey(deleteBuildingName))
            {
                BuildingInfoDict.Remove(deleteBuildingName);
            }
        }
        public void TouchedCityObject(Transform buildingTrans)
        {
            if (buildingTrans == null || buildingTrans.parent.parent == null)
            {
                return;
            }
            
            if (BuildingInfoDict.ContainsKey(buildingTrans.name))
            {
                BuildingInfoDict[buildingTrans.name].capacityLabel.text = GameManageScript.GoalAttributeDict[buildingTrans.name].evacueeNum + "/" + GameManageScript.GoalAttributeDict[buildingTrans.name].capacity;
            }
            DisplayRescuingNum();
            DisplayRescuedNum();
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
                MissionLabel.text = "話しかけているNPCを探せ！";
            }
        }
        public void HideGameUI()
        {
            MapCamera.enabled = false;
            baseUi.gameObject.SetActive(false);
        }
    }
}