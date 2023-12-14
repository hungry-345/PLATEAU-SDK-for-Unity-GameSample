// 正解データがある大元
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Linq;
using StarterAssets;

namespace PLATEAU.Samples
{
    public class GameManage : MonoBehaviour, InputGameManage.IInputGameActions
    {
        [SerializeField, Tooltip("ターゲットフラッグ")] private GameObject targetFlag;

        private InputGameManage inputActions;
        private UIManage UIManageScript;
        private TimeManage TimeManageScript;

        private EnemyManager enemyManager;
        private ItemManager itemManager;

        public SampleAttribute correctGMLdata;
        private GameObject goalBuilding;
        private Bounds goalBounds;
        private Vector3 goalPos;
        private System.Random rnd;
        private GameObject[] HintLst;

        public int sonarCount;
        public int rescuedNum;

        private bool isSetGMLdata;
        private int goalNum;

        KeyValuePair<string, PLATEAU.Samples.SampleCityObject> rndBuilding;
        private List<string> buildingDirName;

        //プレイヤーのコントローラー関数
        private ThirdPersonController thirdpersonController;

        public struct GoalInfo
        {
            public Vector3 goalPosition;
            public string measuredheight;
            public string Usage;
            public string saboveground;
        }


        public Dictionary<string,GoalInfo> GoalAttributeDict;
        private void Awake()
        {
            inputActions = new InputGameManage();
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

        void Start()
        {

        }

        public void StartGame()
        {
            rnd = new System.Random();
            inputActions.InputGame.AddCallbacks(this);
            //操作不能にするために取得
            thirdpersonController = GameObject.Find("PlayerArmature").GetComponent<ThirdPersonController>();

            //SceneManagerからShow.csにアクセスする
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
            TimeManageScript = GameObject.Find("TimeManager").GetComponent<TimeManage>();
            enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
            itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
            //Hintのリストを作る
            HintLst = GameObject.FindGameObjectsWithTag("HintText");
            buildingDirName = new List<string>();
            GoalAttributeDict = new Dictionary<string, GoalInfo>();

            rescuedNum = 0;
            goalNum = 5;
            sonarCount = 5;
            enemyManager.InitializeEnemy();
            itemManager.InitializeItem();
        }

        private string GetAttribute(string attributeName,SampleAttribute attribeteData)
        {
            string value = "";
            foreach(var attribute in attribeteData.GetKeyValues())
            {
                if(attribute.Key.Path.Contains(attributeName))
                {
                    value = attribute.Value;
                }
            }
            return value;
        }
        /// <summary>
        /// 正解の建物として必要な要件は満たしているか
        /// </summary>
        private bool CheckGMLdata(SampleAttribute buildingData,string buildingName)
        {
            bool isSetData = false;
            bool isOverbaseHeight = false;
            bool isOversaboveground = false;
            bool isSameBuilding = false;
            string hintValue;
            string buildingHeight;
            string buildingsaboveground;
            // 必要なGMLデータがそろっているか判定
            foreach(GameObject hint in HintLst)
            {
                isSetData = false;
                hintValue = GetAttribute(hint.name,buildingData);
                if(!(hintValue == ""))
                {
                    isSetData = true;
                }
                if(!isSetData)
                {
                    break;
                }
            }

            // 建物の高さは10m以上か
            buildingHeight = GetAttribute("measuredheight",buildingData);
            if(buildingHeight == "")
            {
                buildingHeight = "-1";
            }
            if(float.Parse(buildingHeight) > 10)
            {
                isOverbaseHeight = true;
            }

            // 建物の階層は1以上か
            buildingsaboveground = GetAttribute("saboveground",buildingData);
            if(buildingsaboveground == "")
            {
                buildingsaboveground = "-1";
            }
            if(float.Parse(buildingsaboveground) > 0)
            {
                isOversaboveground = true;
            }

            // 同じ名前の建物でないか
            if(GoalAttributeDict.ContainsKey(buildingName))
            {
                isSameBuilding = true;
            }

            return isSetData && isOverbaseHeight && !isSameBuilding && isOversaboveground;
        }


        /// <summary>
        /// ランダムな位置に1個ゴールを設置する
        /// </summary>
        private void SelectGoal()
        {
            isSetGMLdata = false;
            while(!isSetGMLdata)
            {
                var tmpdirName = buildingDirName[Random.Range(0,buildingDirName.Count)];
                //ランダムに建物を指定
                rndBuilding = UIManageScript.gmls[tmpdirName].CityObjects.ElementAt(rnd.Next(0, UIManageScript.gmls[tmpdirName].CityObjects.Count));
                //ゴールの属性情報
                correctGMLdata = rndBuilding.Value.Attribute;
                isSetGMLdata = CheckGMLdata(correctGMLdata,rndBuilding.Key);
            }
            goalBuilding = GameObject.Find(rndBuilding.Key);
            goalBounds = goalBuilding.GetComponent<MeshCollider>().sharedMesh.bounds;
            goalPos = new Vector3(goalBounds.center.x+320f,goalBounds.center.y+goalBounds.size.y,goalBounds.center.z+380f);

            GoalInfo gmlData = new GoalInfo { goalPosition = goalPos, measuredheight = GetAttribute("measuredheight",correctGMLdata), Usage = GetAttribute("Usage",correctGMLdata), saboveground = GetAttribute("saboveground",correctGMLdata)};
            GoalAttributeDict.Add(rndBuilding.Key,gmlData);

            GenerateTargetFlag(goalPos,rndBuilding.Key);
        }

        /// <summary>
        /// ランダムな位置に複数個ゴールを設置する
        /// </summary>
        public void SelectGoals()
        {
            //citygmlからbldgに関するフォルダ情報を得る
            foreach(KeyValuePair<string, SampleGml> dir in UIManageScript.gmls)
            {
                if(dir.Key.Contains("bldg"))
                {
                    buildingDirName.Add(dir.Key);
                }
            }
            
            for(int i=0;i<goalNum;i++)
            {
                SelectGoal();
            }
            // foreach(var i in GoalAttributeDict)
            // {
            //     Debug.Log(i.Key);
            // }

            //正解の建物の情報を取得
            // goalBuilding = GameObject.Find(rndBuilding.Key);
            // goalBounds = goalBuilding.GetComponent<MeshCollider>().sharedMesh.bounds;
            //選ばれた建物の位置情報を取得
            // goalPos = new Vector3(goalBounds.center.x+320f,goalBounds.center.y+goalBounds.size.y,goalBounds.center.z+380f);
            
            //Helperの位置を変更
             GameObject.Find("Helper").transform.position = goalPos;
        }
        public void AddGoals(string goalName)
        {
            SelectGoal();
            GoalAttributeDict.Remove(key: goalName);
        }

//  -------------------------------------------------------------------------------

        private float Cal2DDistance(Vector3 point1,Vector3 point2)
        {
            Vector2 point1_2D = new Vector2(point1.x,point1.z);
            Vector2 point2_2D = new Vector2(point2.x,point2.z);
            float distance = Vector2.Distance(point1_2D,point2_2D);

            return distance;
        }


        /// <summary>
        /// ゴールとプレイヤーの距離を計測する
        /// </summary>

        private string FindNearestGoal()
        {
            string nearestBuildingName = "";
            float nearestDistance = float.MaxValue;
            float distance = 0;
            Vector3 playerPos = GameObject.Find("PlayerArmature").transform.position;

            
            foreach(var goalAttribute in GoalAttributeDict)
            {
                distance = Cal2DDistance(goalAttribute.Value.goalPosition,playerPos);
                if(distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestBuildingName = goalAttribute.Key;
                }
            }
            return nearestBuildingName;
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

            if(itemName == "measuredheight")
            {
                hint = GoalAttributeDict[nearestBuildingName].measuredheight;
            }
            else if(itemName == "Usage")
            {
                hint = GoalAttributeDict[nearestBuildingName].Usage;
            }
            else
            {
                hint = GoalAttributeDict[nearestBuildingName].saboveground;
            }
            UIManageScript.DisplayAnswerGML(itemName,hint,nearestBuildingName);

            //フィルター関連の表示
            TimeManageScript.ColorBuilding(itemName,nearestBuildingName,hint);
        }

        // -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// アイテムを生成する
        /// </summary>
        public void SpawnHintItem()
        {
            itemManager.GenerateItem();
        }
        private void GenerateTargetFlag(Vector3 flagPosition,string flagName)
        {
            GameObject flag = Instantiate(targetFlag,transform.root.gameObject.transform) as GameObject;
            flag.name = flagName;
            flag.transform.position = flagPosition;
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
                if(sonarCount > 0)
                {
                    nearestBuildingName = FindNearestGoal();
                    
                    Vector3 playerPos = GameObject.Find("PlayerArmature").transform.position;
                    Vector3 buildingPos = GoalAttributeDict[nearestBuildingName].goalPosition;
                    distance = Cal2DDistance(playerPos,buildingPos);
                    
                    sonarCount -= 1;

                }
                UIManageScript.DisplayDistance(distance,sonarCount);
            }
        }
        //ゲームの終了処理
        public void OnEndGame()
        {
            enemyManager.DestroyEnemy();
            itemManager.DestroyItem();
            thirdpersonController.enabled = false;
        }
    }
}
