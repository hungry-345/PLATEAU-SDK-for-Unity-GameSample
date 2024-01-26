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
    public class UIManage : MonoBehaviour, InputScene.ISelectSceneActions
    {
        public struct BuildingInfo
        {
            public Label heightLabel;
            public Label capacityLabel;
        }
        [SerializeField, Tooltip("初期化中UI")] private UIDocument initializingUi;
        [SerializeField, Tooltip("ベースUI")] public UIDocument BaseUi;
        [SerializeField, Tooltip("色分け（高さ）の色テーブル")] private Color[] heightColorTable;
        [SerializeField, Tooltip("色分け（使用用途）の色テーブル")] private Color[] usageColorTable;
        [SerializeField, Tooltip("選択中のオブジェクトの色")] private Color selectedColor;
        [SerializeField] public Camera PlayerPosCamera;

        public readonly Dictionary<string, SampleGml> gmls = new Dictionary<string, SampleGml>();
        public bool isInitialiseFinish = false;
        public Label timeLabel;
        public Label rescuedNumLabel;
        public Label rescuingNumLabel;
        private PLATEAUInstancedCityModel[] instancedCityModels;
        private SampleCityObject selectCityObject;
        private InputScene inputActions;
        private ColorCodeType colorCodeType;
        private GameManage GameManageScript;
        private TimeManage TimeManageScript;
        private GameObject[] HintTexts;
        // UI Document のラベル
        public Dictionary<string,BuildingInfo> BuildingInfoDict = new Dictionary<string, BuildingInfo>();
        private Label sonarCountLabel;
        private Label sonarContextLabel;
        private Label filterStatusLabel;
        private Label filterContextLabel;
        private Label Shelter1HeightLabel;
        private Label Shelter1CapacityLabel;
        private Label Shelter2HeightLabel;
        private Label Shelter2CapacityLabel;
        private Label Shelter3HeightLabel;
        private Label Shelter3CapacityLabel;
        private Label MissionLabel;
        private string correctBuildingName;
        private string filterStatus;
        private string nearestBuildingName;
        // -------------------------------------------------------------------------------------------------------------
        private void Awake()
        {
            // InputSystemのインスタンス(初期値)
            inputActions = new InputScene();
            //Plateauのデータを取得
            InitializeAsync().ContinueWithErrorCatch();
        }

        // InputSystemを有効化させる
        // -------------------------------------------------------------------------------------------------------------
        private void OnEnable()
        {
            inputActions.Enable();
        }
        private void OnDisable()
        {
            inputActions.Disable();
        }
        private void OnDestroy()
        {
            inputActions.Dispose();
        }
        // -------------------------------------------------------------------------------------------------------------
        void Start()
        {
            // 他オブジェクトのスクリプトのインスタンス(GameManagerの関数や変数を参照できる)
            GameManageScript = GameObject.Find("GameManager").GetComponent<GameManage>();
            TimeManageScript = GameObject.Find("TimeManager").GetComponent<TimeManage>();
        }
        public void InitializeUI()
        {
            //InputSystemの入力を登録
            inputActions.SelectScene.AddCallbacks(this);
            //コルーチン開始(Plateauのデータの取得が終わった後の処理を実行)
            StartCoroutine(WatiForInitialise());
            //変数の初期化
            filterStatus = "None";
            correctBuildingName = "";
            PlayerPosCamera.enabled = false;

            // 共通タグのオブジェクトを一つの配列にまとめる
            HintTexts = GameObject.FindGameObjectsWithTag("HintText");
        }

        // Plateauのデータのロードに関する処理
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Plateauのデータを取得する関数
        /// </summary> 
        private async Task InitializeAsync()
        {
            // 初期化UIを起動
            initializingUi.gameObject.SetActive(true);
            // Plateauのデータを取得
            instancedCityModels = FindObjectsOfType<PLATEAUInstancedCityModel>();
            if (instancedCityModels == null || instancedCityModels.Length == 0)
            {
                return;
            }
            foreach(var instancedCityModel in instancedCityModels)
            {
                var rootDirName = instancedCityModel.name;
                //Debug.Log(instancedCityModel.transform.childCount);
                for (int i = 0; i < instancedCityModel.transform.childCount; ++i)
                {
                    var go = instancedCityModel.transform.GetChild(i).gameObject;
                    // サンプルではdemを除外します。
                    if (go.name.Contains("dem")) continue;
                    var cityModel = await PLATEAUCityGmlProxy.LoadAsync(go, rootDirName);
                    if (cityModel == null) continue;
                    var gml = new SampleGml(cityModel, go);
                    gmls.Add(go.name, gml);
                }
            }
            isInitialiseFinish = true;
        }
        /// <summary>
        /// Plateauのデータの取得が終わるまで待機する関数
        /// </summary>
        IEnumerator WatiForInitialise()
        {
            // yield return ->　ある関数が終わるまで待つ
            yield return new WaitUntil(() => IsInitialiseFinished());
        }
        /// <summary>
        /// Plateauのデータの取得が終わった後の処理を行う関数
        /// </summary> 
        private bool IsInitialiseFinished()
        {
            if(isInitialiseFinish)
            {
                // UIの切り替え
                initializingUi.gameObject.SetActive(false);
                BaseUi.gameObject.SetActive(true);
                // BaseUIのラベルを取得
                timeLabel = BaseUi.rootVisualElement.Q<Label>("Time");
                rescuedNumLabel = BaseUi.rootVisualElement.Q<Label>("Rescued_Count");
                rescuingNumLabel = BaseUi.rootVisualElement.Q<Label>("Rescuing_Count");
                Shelter1HeightLabel = BaseUi.rootVisualElement.Q<Label>("Shelter1_Height");
                Shelter1CapacityLabel = BaseUi.rootVisualElement.Q<Label>("Shelter1_Capacity");
                Shelter2HeightLabel = BaseUi.rootVisualElement.Q<Label>("Shelter2_Height");
                Shelter2CapacityLabel = BaseUi.rootVisualElement.Q<Label>("Shelter2_Capacity");
                Shelter3HeightLabel = BaseUi.rootVisualElement.Q<Label>("Shelter3_Height");
                Shelter3CapacityLabel = BaseUi.rootVisualElement.Q<Label>("Shelter3_Capacity");
                MissionLabel = BaseUi.rootVisualElement.Q<Label>("Mission_Text");
                

                EditMissionText();
                // ゴールの位置を設定する
                GameManageScript.SelectGoals();
                // // Map用のカメラを起動する
                PlayerPosCamera.enabled = true; 
            }
            return isInitialiseFinish;
        }

        // ヒントに関する関数
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// アイテムを取得した時の処理(Contact.csに参照されている)
        /// </summary>
        public void DisplayAnswer(string hintBuildingName,string hintBuildingHeight,string hintBuildingCapacity,string hintBuildingEvacuee)
        {
            Debug.Log(hintBuildingCapacity);
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
            }
            // GoalCityObject.GetComponent<Renderer>().material.color = selectedColor;
            // selectCityObject = gmls[GoalCityObject.transform.parent.parent.name].CityObjects[GoalCityObject.transform.name];
            // selectCityObject.SetMaterialColor(selectedColor);
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

        private string SetFilterText(string itemName,string attributeValue)
        {
            string filterText = "";
            if(itemName == "measuredheight")
            {
                float height = float.Parse(attributeValue);  
                if (height <= 12f)
                {
                    filterText = "高さ : ~ 12m";
                }
                else if (height > 12f && height <= 31f)
                {
                    filterText = "高さ : 13m ~ 31m";
                }
                else if (height > 31f && height <= 60f)
                {
                    filterText = "高さ : 32m ~ 60m";
                }
                else if (height > 60f && height <= 120f)
                {
                    filterText = "高さ : 61m ~ 120m";
                }
                else if (height > 120f && height <= 180f)
                {
                    filterText = "高さ : 121m ~ 180m";
                }
                else if (height > 180f)
                {
                    filterText = "高さ : 181m ~";
                }
            }
            if(itemName == "Usage")
            {
                foreach(var t in GameManageScript.correctGMLdata.GetKeyValues())
                {
                    if(t.Key.Path.Contains(itemName))
                    {
                        filterText = "使用用途 : " + t.Value;
                    }
                }
            }
            return filterText;
        }


        /// <summary>
        /// 機能しているフィルターを表示させる 
        /// </summary> 
        public void ChangeColoring(string itemName,string nearestName,string attributeValue)
        {
            // TextMeshProUGUI  displayMesh = GameObject.Find("FilterLabel").GetComponent<TextMeshProUGUI>();
            nearestBuildingName = nearestName; // 個の変数はUIManage専用の変数でここで以外変更してはいけない　<- あとで直す
            filterStatus = itemName;

            // 全ての建物の色を元に戻す
            // colorCodeType = (ColorCodeType)Enum.Parse(typeof(ColorCodeType), "None");
            ColorCode("None",nearestBuildingName);
            //フィルターに引っかかった建物の色を変える
            // colorCodeType = (ColorCodeType)Enum.Parse(typeof(ColorCodeType), itemName);
            ColorCode(itemName,nearestBuildingName);


            string filterText = SetFilterText(itemName,attributeValue); 
        }


        // 建物の選択に関する関数
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// すべての建物の色を変える
        /// </summary>
        public void ColorCode(string type,string nearestName)
        {
            colorCodeType = (ColorCodeType)Enum.Parse(typeof(ColorCodeType), type);
            //GISSample.csのColorCode関数(上の方)を参考
            foreach (var keyValue in gmls)
            {
                Color[] colorTable = null;
                switch (colorCodeType)
                {
                    case ColorCodeType.measuredheight:
                        colorTable = heightColorTable;
                        break;
                    case ColorCodeType.Usage:
                        colorTable = usageColorTable;
                        break;
                    default:
                        break;
                }
                keyValue.Value.ColorCode(colorCodeType, colorTable,nearestName);
            }
        }

        /// <summary>
        /// 目の前にある建物を返す
        /// </summary>
        private Transform Lookforward()
        {
            // 初期化
            float nearestDistance = float.MaxValue;
            Transform nearestTransform = null;
            //カメラの方向を取得
            var camera = Camera.main;
            var ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            // 方向(ray)と交わったオブジェクトの中から一番PlayerArmatureと距離の近いオブジェクトを返す
            foreach (var hit in Physics.RaycastAll(ray))
            {
                if (hit.distance <= nearestDistance && hit.transform.name.Contains("bldg"))
                {
                    nearestDistance = hit.distance;
                    nearestTransform = hit.transform;
                }
            }

            return nearestTransform;
        }

        /// <summary>
        /// gmlsから目的の建物の属性情報を選ぶ(属性情報 == Attribute)
        /// </summary>
        public SampleAttribute GetAttribute(string gmlFileName, string cityObjectID)
        {
            // gmls(gmlFileName, gml)  + gmlFileName  ->  gml
            if (gmls.TryGetValue(gmlFileName, out SampleGml gml))
            {
                // gml(ID,cityObject) + ID  -> cityObject
                if (gml.CityObjects.TryGetValue(cityObjectID, out SampleCityObject city))
                {
                    // cityobject -> attribute
                    return city.Attribute;
                }
            }

            return null;
        }

        /// <summary>
        /// UI表示の変更
        /// </summary>
        public void SelectCityObject(Transform buildingTrans)
        {
            // 目の前の建物のTransform情報を得る(GameObject -> Transform)
            //Transform trans = Lookforward();
            if (buildingTrans == null || buildingTrans.parent.parent == null)
            {
                // selectedCityObject = null;
                return;
            }

            //ゴールの建物だった場合
            //if(BuildingInfoDict.ContainsKey(buildingTrans.name))
            //{
            //GameManageScript.selectBuildingAction(trans);
            rescuedNumLabel.text = GameManageScript.rescuedNum.ToString();
            rescuingNumLabel.text = GameManageScript.rescuingNum.ToString();
            //}
            if (BuildingInfoDict.ContainsKey(buildingTrans.name))
            {
                BuildingInfoDict[buildingTrans.name].capacityLabel.text = GameManageScript.GoalAttributeDict[buildingTrans.name].evacueeNum + "/" + GameManageScript.GoalAttributeDict[buildingTrans.name].capacity;
            }
        }

        // 実行時間に依存する処理
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ゴールまでの距離を表示させる
        /// </summary> 
        public void DisplayDistance(float distance,int sonarCount)
        {
            string dist = "";
            if(distance >-1f)
            {
                dist = distance.ToString();
            }

            // sonarCountLabel.text = sonarCount.ToString();
            // sonarContextLabel.text  = "距離 : " +  dist;
        }
        // ---------------------------------------------------------------
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
                MissionLabel.text = "生存者を見つけよう!";
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
        // Plateauのデータ取得の終了待ちの関数
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ある関数が終わるまで待つ
        /// </summary> 


        // InputSystemの入力に対する処理(OnChangeCameraScene : Keyboard Q, OnChangeFilterScene : Keyboard C, OnSelectObject : Mouse Left)
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// カメラの切り替えの要求処理
        /// </summary> 
        public void OnChangeCameraScene(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                //ChangeCameraScene();
            }

        }
        /// <summary>
        /// フィルターの切り替えの要求処理
        /// </summary> 
        public void OnChangeFilterScene(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
            }
        }
        /// <summary>
        /// カメラの切り替えの要求処理
        /// </summary> 
        public void OnSelectObject(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if(isInitialiseFinish)
                {
                    //SelectCityObject();
                }
            }
        }

        public void HideGameUI()
        {
            BaseUi.gameObject.SetActive(false);
        }
    }
}