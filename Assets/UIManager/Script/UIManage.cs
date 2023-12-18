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
        [SerializeField, Tooltip("初期化中UI")] private UIDocument initializingUi;
        [SerializeField, Tooltip("ベースUI")] private UIDocument BaseUi;
        [SerializeField, Tooltip("色分け（高さ）の色テーブル")] private Color[] heightColorTable;
        [SerializeField, Tooltip("色分け（使用用途）の色テーブル")] private Color[] usageColorTable;
        [SerializeField, Tooltip("選択中のオブジェクトの色")] private Color selectedColor;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera goalCamera;


        private PLATEAUInstancedCityModel[] instancedCityModels;
        private SampleCityObject selectCityObject;
        private InputScene inputActions;
        private ColorCodeType colorCodeType;
        private GameObject[] HintTexts;
        private GameObject[] FilterContents;
        private GameManage GameManageScript;
        private TimeManage TimeManageScript;
        private GameObject displaySelectGML;
        public readonly Dictionary<string, SampleGml> gmls = new Dictionary<string, SampleGml>();
        private string GMLText;
        public string SceneName; 
        private bool isSetColorCodeType;


        public bool isInitialiseFinish = false;

        public Label timeLabel;
        public Label rescuedNumLabel;
        public Label sonarCountLabel;
        public Label sonarContextLabel;
        public Label filterStatusLabel;
        public Label filterContextLabel;
        public Label bar1ScanLabel;
        public Label bar1HintLabel;
        public Label bar2ScanLabel;
        public Label bar2HintLabel;
        public Label bar3ScanLabel;
        public Label bar3HintLabel;
        
        private string displayBuildingName;

        private string filterStatus;
        private string nearestBuildingName;



        private void Awake()
        {
            inputActions = new InputScene();

            //Plateauのデータを取得
            InitializeAsync().ContinueWithErrorCatch();  
        }

        // InputSystemに関する関数
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
            inputActions.SelectScene.AddCallbacks(this);

            //コルーチン開始(Plateauのデータの取得が終わった後の処理を実行)
            StartCoroutine(WatiForInitialise());

            //初期化
            filterStatus = "None";
            SceneName = "MainCamera";
            displayBuildingName = "";
            mainCamera.enabled = true;
            goalCamera.enabled = false;
                // GameManagerの関数や変数を参照できる
            GameManageScript = GameObject.Find("GameManager").GetComponent<GameManage>();
            TimeManageScript = GameObject.Find("TimeManager").GetComponent<TimeManage>();
                // 共通タグのオブジェクトを一つの配列にまとめる
            HintTexts = GameObject.FindGameObjectsWithTag("HintText");
        }

        // Plateauのデータに関する関数
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Plateauのデータを取得する
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
            // 初期化UIを消去
            isInitialiseFinish = true;
        }

        // ヒントに関する関数
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// アイテムを取得した時の処理(Contact.csに参照されている)
        /// </summary>
        public void DisplayAnswerGML(string itemName,string hint,string buildingName)
        {
            if(displayBuildingName != buildingName)
            {
                bar1HintLabel.text = GameManageScript.GoalAttributeDict[buildingName].saboveground;
            }
            //正解の建物のGMLデータを表示させる


            if(itemName == "measuredheight")
            {
                bar2HintLabel.text = hint;
            }
            if(itemName == "Usage")
            {
                bar3HintLabel.text = hint;
            }


            displayBuildingName = buildingName;
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
            // ownHintLstの中のIndexで指定された候補を返す
            if(itemName == "None")
            {
                filterContextLabel.text = "";
                filterStatusLabel.text = "OFF";
            }
            else
            {
                filterStatusLabel.text = "ON";
                filterContextLabel.text = filterText;
            }
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
                // if(hit.distance <= nearestDistance)
                // {
                //     Debug.Log(hit.transform.name);
                // }
                // if (hit.distance <= nearestDistance && hit.transform.name != "PlayerArmature" && !hit.transform.name.Contains("dem") && !hit.transform.name.Contains("zombie"))
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
        /// 建物を選択した時の処理
        /// </summary>
        private void SelectCityObject()
        {
            // 目の前の建物のTransform情報を得る(GameObject -> Transform)
            var trans = Lookforward();
            if(trans == null || trans.parent.parent == null)
            {
                // selectedCityObject = null;
                return;
            }

            // 建物の色を変える
            //選択された状態の建物の色を元に戻す
            colorCodeType = (ColorCodeType)Enum.Parse(typeof(ColorCodeType), filterStatus);
            ColorCode(filterStatus,nearestBuildingName);
            // 選択した建物の色を変更する
            selectCityObject = gmls[trans.parent.parent.name].CityObjects[trans.name];
            selectCityObject.SetMaterialColor(selectedColor);
            // selectedCityObject =gmls[trans.parent.parent.name].CityObjects[trans.name];
            // if(selectCityObject != selectedCityObject)
            // {
            // }

            //対象の建物のGMLデータを表示させる
            GMLText = "";
            var selectbuildingAttribute = GetAttribute(trans.parent.parent.name, trans.name);
            var AttributeKeyValues = selectbuildingAttribute.GetKeyValues();

            bar1ScanLabel.text = "Unknown";
            foreach(var AttributeKeyValue in AttributeKeyValues)
            {
                if(AttributeKeyValue.Key.Path.Contains("measuredheight"))
                {
                    bar2ScanLabel.text = AttributeKeyValue.Value;
                }
                if(AttributeKeyValue.Key.Path.Contains("Usage"))
                {
                    bar3HintLabel.text =  AttributeKeyValue.Value;
                }
                foreach(GameObject HintContent in HintTexts)
                {
                    if(AttributeKeyValue.Key.Path.Contains(HintContent.name))
                    {
                        GMLText += HintContent.name + "\n" + AttributeKeyValue.Value + "\n";
                    }
                }
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

            sonarCountLabel.text = sonarCount.ToString();
            sonarContextLabel.text  = dist;
        }

        
        // Plateauのデータ取得の終了待ちの関数
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ある関数が終わるまで待つ
        /// </summary> 
        IEnumerator WatiForInitialise()
        {
            // yield return ->　ある関数が終わるまで待つ
            yield return new WaitUntil(() => IsInitialiseFinished());
        }
        /// <summary>
        /// Plateauのデータの取得が終わった後の処理
        /// </summary> 
        private bool IsInitialiseFinished()
        {
            if(isInitialiseFinish)
            {
                GameManageScript.SelectGoals();

                initializingUi.gameObject.SetActive(false);

                BaseUi.gameObject.SetActive(true);
                timeLabel = BaseUi.rootVisualElement.Q<Label>("Time");
                rescuedNumLabel = BaseUi.rootVisualElement.Q<Label>("Rescued_Count");

                sonarCountLabel = BaseUi.rootVisualElement.Q<Label>("Sonar_Count");
                sonarContextLabel = BaseUi.rootVisualElement.Q<Label>("Sonar_Context");

                filterStatusLabel = BaseUi.rootVisualElement.Q<Label>("Filter_Status");
                filterContextLabel = BaseUi.rootVisualElement.Q<Label>("Filter_Context");

                bar1ScanLabel = BaseUi.rootVisualElement.Q<Label>("Bar1_Scan");
                bar1HintLabel = BaseUi.rootVisualElement.Q<Label>("Bar1_Hint");
                bar2ScanLabel = BaseUi.rootVisualElement.Q<Label>("Bar2_Scan");
                bar2HintLabel = BaseUi.rootVisualElement.Q<Label>("Bar2_Hint");
                bar3ScanLabel = BaseUi.rootVisualElement.Q<Label>("Bar3_Scan");
                bar3HintLabel = BaseUi.rootVisualElement.Q<Label>("Bar3_Hint");

                GameObject PlayerPosCamera = GameObject.Find("PlayerPositionCamera");
                PlayerPosCamera.SetActive(true); 
            }
            return isInitialiseFinish;
        }


        // InputSystemの入力に対する処理(OnChangeCameraScene : Keyboard Q, OnChangeFilterScene : Keyboard C, OnSelectObject : Mouse Left)
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// カメラの切り替えの要求処理
        /// </summary> 
        public void OnChangeCameraScene(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                // ChangeCameraScene();
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
                    SelectCityObject();
                }
            }
        }
    }
}