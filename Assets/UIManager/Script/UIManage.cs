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
        [SerializeField, Tooltip("ベースUI")] public UIDocument BaseUi;
        [SerializeField, Tooltip("色分け（高さ）の色テーブル")] private Color[] heightColorTable;
        [SerializeField, Tooltip("色分け（使用用途）の色テーブル")] private Color[] usageColorTable;
        [SerializeField, Tooltip("選択中のオブジェクトの色")] private Color selectedColor;
        [SerializeField] public  Camera PlayerPosCamera;


        private PLATEAUInstancedCityModel[] instancedCityModels;
        private SampleCityObject selectCityObject;

        private InputScene inputActions;

        private ColorCodeType colorCodeType;
        private TimeManage TimeManageScript;
        public readonly Dictionary<string, SampleGml> gmls = new Dictionary<string, SampleGml>();
        public string SceneName; 

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

        //ゲームの初期化のための変数
        [SerializeField, Tooltip("ターゲットフラッグ")] private GameObject targetFlag;
        private bool isSetGMLdata;//GMLデータがセットされたか
        private List<string> buildingDirName; //建物名のリスト
        private System.Random rnd; //ランダム用
        KeyValuePair<string, PLATEAU.Samples.SampleCityObject> rndBuilding;　//建物データ
        public SampleAttribute correctGMLdata;　//正解の建物データ
        private GameObject goalBuilding; //正解の建物のゲームオブジェクト
        private Bounds goalBounds;　//ゴールの建物のBounds
        private Vector3 goalPos; //ゴールのPosition
        private int goalNum;
        private GameObject[] HintLst;
        public int sonarCount;

        //ゴール情報の構造体
        public struct GoalInfo
        {
            public Vector3 goalPosition;
            public string measuredheight;
            public string Usage;
            public string saboveground;
        }
        public Dictionary<string, GoalInfo> GoalAttributeDict;　//ゴールの建物用の辞書型
        public void InitializeGML()
        {
            inputActions = new InputScene();
            //Plateauのデータを取得
            InitializeAsync().ContinueWithErrorCatch();
            inputActions.Enable();
            inputActions.SelectScene.AddCallbacks(this);

            //初期化
            filterStatus = "None";
            SceneName = "MainCamera";
            displayBuildingName = "";
            PlayerPosCamera.enabled = false;
            // GameManagerの関数や変数を参照できる
            TimeManageScript = GameObject.Find("TimeManager").GetComponent<TimeManage>();

            //建物名のリストを作成
            buildingDirName = new List<string>();
            rnd = new System.Random();
            //ゴールの建物のリストを作成
            GoalAttributeDict = new Dictionary<string, GoalInfo>();
            //ゴールの数を設定
            goalNum = 5;
            //Hintのリストを作る
            HintLst = GameObject.FindGameObjectsWithTag("HintText");
            sonarCount = 5;

            //コルーチン開始(Plateauのデータの取得が終わった後の処理を実行)
            StartCoroutine(WatiForInitialise());
        }

        // InputSystemに関する関数
        // -------------------------------------------------------------------------------------------------------------
        //private void OnEnable()
        //{
        //    inputActions.Enable();
        //}

        private void OnDisable()
        {
            inputActions.Disable();
        }

        private void OnDestroy()
        {
            inputActions.Dispose();
        }
        // -------------------------------------------------------------------------------------------------------------

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
                bar1HintLabel.text = GoalAttributeDict[buildingName].saboveground;
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
                foreach(var t in correctGMLdata.GetKeyValues())
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
            ColorCode("None",nearestBuildingName);
            //フィルターに引っかかった建物の色を変える
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
        public SampleAttribute GetAttributeName(string gmlFileName, string cityObjectID)
        {
            if (gmls.TryGetValue(gmlFileName, out SampleGml gml))
            {
                if (gml.CityObjects.TryGetValue(cityObjectID, out SampleCityObject city))
                {
                    return city.Attribute;
                }
            }

            return null;
        }
        private string GetAttribute(string attributeName, SampleAttribute attribeteData)
        {
            string value = "";
            foreach (var attribute in attribeteData.GetKeyValues())
            {
                if (attribute.Key.Path.Contains(attributeName))
                {
                    value = attribute.Value;
                }
            }
            return value;
        }
        public void AddGoals(string goalName)
        {
            SelectGoal();
            GoalAttributeDict.Remove(key: goalName);
        }

        private string FindNearestGoal()
        {
            string nearestBuildingName = "";
            float nearestDistance = float.MaxValue;
            float distance = 0;
            Vector3 playerPos = GameObject.Find("PlayerArmature").transform.position;


            foreach (var goalAttribute in GoalAttributeDict)
            {
                distance = Cal2DDistance(goalAttribute.Value.goalPosition, playerPos);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestBuildingName = goalAttribute.Key;
                }
            }
            return nearestBuildingName;
        }
        private float Cal2DDistance(Vector3 point1, Vector3 point2)
        {
            Vector2 point1_2D = new Vector2(point1.x, point1.z);
            Vector2 point2_2D = new Vector2(point2.x, point2.z);
            float distance = Vector2.Distance(point1_2D, point2_2D);

            return distance;
        }

        /// <summary>
        /// アイテムを拾った時の処理
        /// </summary>
        public void DisplayHint(string itemName)
        {
            string nearestBuildingName;
            string hint;

            //表示させる建物の情報を決める
            nearestBuildingName = FindNearestGoal();

            if (itemName == "measuredheight")
            {
                hint = GoalAttributeDict[nearestBuildingName].measuredheight;
            }
            else if (itemName == "Usage")
            {
                hint = GoalAttributeDict[nearestBuildingName].Usage;
            }
            else
            {
                hint = GoalAttributeDict[nearestBuildingName].saboveground;
            }
            DisplayAnswerGML(itemName, hint, nearestBuildingName);
            //フィルターの表示
            TimeManageScript.ColorBuilding(itemName, nearestBuildingName, hint);
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

            //対象の建物のGMLデータを表示させる
            //GMLText = "";
            var selectbuildingAttribute = GetAttributeName(trans.parent.parent.name, trans.name);
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
                    bar3ScanLabel.text =  AttributeKeyValue.Value;
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

            sonarCountLabel.text = "距離 : " + sonarCount.ToString();
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
                SelectGoals();

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

                PlayerPosCamera.enabled = true; 
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

        public void HideGameUI()
        {
            BaseUi.gameObject.SetActive(false);
        }
        /// <summary>
        /// ランダムな位置に1個ゴールを設置する
        /// </summary>
        private void SelectGoal()
        {
            isSetGMLdata = false;
            while (!isSetGMLdata)
            {
                //ランダムな建物の名前を格納する
                var tmpdirName = buildingDirName[UnityEngine.Random.Range(0, buildingDirName.Count)];
                //ランダムに建物を指定
                rndBuilding = gmls[tmpdirName].CityObjects.ElementAt(rnd.Next(0, gmls[tmpdirName].CityObjects.Count));
                //ゴールの属性情報
                correctGMLdata = rndBuilding.Value.Attribute;
                isSetGMLdata = CheckGMLdata(correctGMLdata, rndBuilding.Key);
            }
            goalBuilding = GameObject.Find(rndBuilding.Key);
            goalBounds = goalBuilding.GetComponent<MeshCollider>().sharedMesh.bounds;
            goalPos = new Vector3(goalBounds.center.x + 320f, goalBounds.center.y + goalBounds.size.y, goalBounds.center.z + 380f);

            GoalInfo gmlData = new GoalInfo { goalPosition = goalPos, measuredheight = GetAttribute("measuredheight", correctGMLdata), Usage = GetAttribute("Usage", correctGMLdata), saboveground = GetAttribute("saboveground", correctGMLdata) };
            GoalAttributeDict.Add(rndBuilding.Key, gmlData);

            GenerateTargetFlag(goalPos, rndBuilding.Key);
        }
        private void GenerateTargetFlag(Vector3 flagPosition, string flagName)
        {
            GameObject flag = Instantiate(targetFlag, transform.root.gameObject.transform) as GameObject;
            flag.name = flagName;
            flag.transform.position = flagPosition;
        }

        /// <summary>
        /// ランダムな位置に複数個ゴールを設置する
        /// </summary>
        public void SelectGoals()
        {
            //citygmlからbldgに関するフォルダ情報を得る
            foreach (KeyValuePair<string, SampleGml> dir in gmls)
            {
                if (dir.Key.Contains("bldg"))
                {
                    buildingDirName.Add(dir.Key);
                }
            }

            for (int i = 0; i < goalNum; i++)
            {
                SelectGoal();
            }

            //Helperの位置を変更
            GameObject.Find("Helper").transform.position = goalPos;
        }

        /// <summary>
        /// 正解の建物として必要な要件は満たしているか
        /// </summary>
        private bool CheckGMLdata(SampleAttribute buildingData, string buildingName)
        {
            bool isSetData = false;
            bool isOverbaseHeight = false;
            bool isOversaboveground = false;
            bool isSameBuilding = false;
            string hintValue;
            string buildingHeight;
            string buildingsaboveground;
            // 必要なGMLデータがそろっているか判定
            foreach (GameObject hint in HintLst)
            {
                isSetData = false;
                hintValue = GetAttribute(hint.name, buildingData);
                if (!(hintValue == ""))
                {
                    isSetData = true;
                }
                if (!isSetData)
                {
                    break;
                }
            }

            // 建物の高さは10m以上か
            buildingHeight = GetAttribute("measuredheight", buildingData);
            if (buildingHeight == "")
            {
                buildingHeight = "-1";
            }
            if (float.Parse(buildingHeight) > 10)
            {
                isOverbaseHeight = true;
            }

            // 建物の階層は1以上か
            buildingsaboveground = GetAttribute("saboveground", buildingData);
            if (buildingsaboveground == "")
            {
                buildingsaboveground = "-1";
            }
            if (float.Parse(buildingsaboveground) > 0)
            {
                isOversaboveground = true;
            }

            // 同じ名前の建物でないか
            if (GoalAttributeDict.ContainsKey(buildingName))
            {
                isSameBuilding = true;
            }

            return isSetData && isOverbaseHeight && !isSameBuilding && isOversaboveground;
        }
        // InputSystemの入力に対する処理(OnSonar : F)
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sonarを使う時の処理
        /// </summary> 
        public void OnSonar(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                string nearestBuildingName = "";
                float distance = -1f;
                if (sonarCount > 0)
                {
                    nearestBuildingName = FindNearestGoal();

                    Vector3 playerPos = GameObject.Find("PlayerArmature").transform.position;
                    Vector3 buildingPos = GoalAttributeDict[nearestBuildingName].goalPosition;
                    distance = Cal2DDistance(playerPos, buildingPos);

                    sonarCount -= 1;

                }
                DisplayDistance(distance, sonarCount);
            }
        }
    }
}