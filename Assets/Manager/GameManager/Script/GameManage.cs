using PLATEAU.CityInfo;
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
        public struct GoalInfo
        {
            public Vector3 goalPosition;
            public string measuredheight;
            public bool isHintActive;
            public int evacueeNum;
            public int capacity;
            public GameObject buildingObj;
        }
        
        [SerializeField, Tooltip("ターゲットフラッグ")] private GameObject targetFlag;
        [SerializeField, Tooltip("ゴールマーカー")] private GameObject GoalMarker;

        //ゲーム終了時にプレイヤーを操作不能にするために取得
        [SerializeField] private PlayerInput playerInput;
        private Transform targetParent;


        public SampleAttribute correctGMLdata;
        public Dictionary<string,GoalInfo> GoalAttributeDict;
        public int sonarCount;
        public int rescuedNum;
        public int rescuingNum;

        private InputGameManage inputActions;
        //private ThirdPersonController thirdpersonController;
        private System.Random rnd;
        private UIManage UIManageScript;
        private TimeManage TimeManageScript;
        private EnemyManage EnemyManageScript;
        private ItemManage ItemManageScript;
        private NPCManager NPCManageScript;
        private GameObject[] HintLst;
        private GameObject goalBuilding;
        private List<string> buildingDirName;
        private KeyValuePair<string, PLATEAU.Samples.SampleCityObject> rndBuilding;
        private Bounds goalBounds;
        private Vector3 goalPos;
        private int goalNum;
        private int getHintNum;
        private bool isSetGMLdata;
        // -------------------------------------------------------------------------------------------------------------
        private void Awake()
        {
            inputActions = new InputGameManage();
            targetParent = GameObject.Find("52385628_bldg_6697_op.gml").transform;
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

        }

        public void StartGame()
        {
            rnd = new System.Random();
            inputActions.InputGame.AddCallbacks(this);
            
            //thirdpersonController = GameObject.Find("PlayerArmature").GetComponent<ThirdPersonController>();

            //SceneManagerからShow.csにアクセスする
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
            TimeManageScript = GameObject.Find("TimeManager").GetComponent<TimeManage>();
            EnemyManageScript = GameObject.Find("EnemyManager").GetComponent<EnemyManage>();
            ItemManageScript = GameObject.Find("ItemManager").GetComponent<ItemManage>();
            NPCManageScript= GameObject.Find("NPCManager").GetComponent<NPCManager>();

            //Hintのリストを作る
            HintLst = GameObject.FindGameObjectsWithTag("HintText");
            buildingDirName = new List<string>();
            GoalAttributeDict = new Dictionary<string, GoalInfo>();

            getHintNum = 0;
            rescuedNum = 0;
            rescuingNum = 0;
            goalNum = 3;
            sonarCount = 5;

            //GMLデータの初期化コルーチンを含む処理
            UIManageScript.InitializeUI();
            //コルーチン開始(Plateauのデータの取得が終わった後の処理を実行)
            // StartCoroutine(WatiForInitialise());
            //アイテム・NPCの初期化
            playerInput.enabled = false;
            EnemyManageScript.InitializeEnemy();
            ItemManageScript.InitializeItem();
            NPCManageScript.InitializeNPC();
            playerInput.enabled = true;

        }
        // IEnumerator WatiForInitialise()
        // {
        //     // yield return ->　ある関数が終わるまで待つ
            
        //     // yield return new WaitUntil(() => UIManageScript.IsInitialiseFinished());
            
        // }
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
        private bool CheckAttributeData(string buildingName,string buildingHeight)
        {
            bool isOverbaseHeight = false;
            bool isSameBuilding = false;

            if(buildingHeight == "")
            {
                buildingHeight = "-1";
            }
            if(float.Parse(buildingHeight) > 10)
            {
                isOverbaseHeight = true;
            }

            if(GoalAttributeDict.ContainsKey(buildingName))
            {
                isSameBuilding = true;
            }

            return isOverbaseHeight && !isSameBuilding;

        }
        private bool CheckGMLdata(SampleAttribute buildingData,string buildingName)
        {
            bool isSetData = false;
            bool isOverbaseHeight = false;
            // bool isOversaboveground = false;
            bool isSameBuilding = false;
            string hintValue;
            string buildingHeight;
            string buildingsaboveground;
            // 必要なGMLデータがそろっているか判定  Usageは沼津にはない
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

            // 建物の階層は1以上か ぬまずにはない
            // buildingsaboveground = GetAttribute("saboveground",buildingData);
            // if(buildingsaboveground == "")
            // {
            //     buildingsaboveground = "-1";
            // }
            // if(float.Parse(buildingsaboveground) > 0)
            // {
            //     isOversaboveground = true;
            // }

            // 同じ名前の建物でないか
            if(GoalAttributeDict.ContainsKey(buildingName))
            {
                isSameBuilding = true;
            }

            return isSetData && isOverbaseHeight && !isSameBuilding;
        }


        /// <summary>
        /// ランダムな位置に1個ゴールを設置する
        /// </summary>
        private void SelectGoal()
        {
            var cityObjGroups = targetParent.GetComponentsInChildren<PLATEAUCityObjectGroup>();
            int capacityNum = 0;
            bool isSetAttributeData;
            isSetGMLdata = false;
            // 全ての建物

            while(GoalAttributeDict.Count != 3)
            {
                var cityObjGroup = cityObjGroups.ElementAt(rnd.Next(0, cityObjGroups.Length));
                var target = cityObjGroup.transform;
                foreach (var cityObj in cityObjGroup.GetAllCityObjects())
                {
                    var attributes = cityObj.AttributesMap;
                    if(attributes.TryGetValue("bldg:measuredheight", out var height))
                    {
                        isSetAttributeData = CheckAttributeData(target.name,height.StringValue);
                        if(isSetAttributeData)
                        {
                            // 正解の建物のGameOBject
                            goalBuilding = GameObject.Find(target.name);
                            goalBounds = goalBuilding.GetComponent<MeshCollider>().sharedMesh.bounds;
                            goalPos = new Vector3(goalBounds.center.x+320f,goalBounds.center.y+goalBounds.size.y,goalBounds.center.z+380f);

                            //goalBuilding.tag = "Goal";

                            capacityNum =  (int)float.Parse(height.StringValue)/5;

                            GoalInfo gmlData = new GoalInfo { goalPosition = goalPos, measuredheight = height.StringValue, isHintActive=false, capacity=capacityNum,evacueeNum=0,buildingObj= goalBuilding};

                            GoalAttributeDict.Add(target.name,gmlData);
                            goalPos += new Vector3(-467.28f,0f,-1869.266f);
                            GenerateTargetFlag(goalPos,target.name);
                        }
                    }
                }
            }

            // while(!isSetGMLdata)
            // {
            //     var tmpdirName = buildingDirName[Random.Range(0,buildingDirName.Count)];
            //     //ランダムに建物を指定ript.gmls[tmpdirName]
            //     rndBuilding = UIManageScript.gmls[tmpdirName].CityObjects.ElementAt(rnd.Next(0, UIManageSc.CityObjects.Count));
            //     //ゴールの属性情報
            //     correctGMLdata = rndBuilding.Value.Attribute;
            //     isSetGMLdata = CheckGMLdata(correctGMLdata,rndBuilding.Key);
            // }
            // // goalPos
            // goalBuilding = GameObject.Find(rndBuilding.Key);
            // goalBounds = goalBuilding.GetComponent<MeshCollider>().sharedMesh.bounds;
            // goalPos = new Vector3(goalBounds.center.x+320f,goalBounds.center.y+goalBounds.size.y,goalBounds.center.z+380f);

            // //正解の建物用のタグを付ける
            // goalBuilding.tag = "Goal";

            // //Capacity
            // capacityNum =  (int)float.Parse(GetAttribute("measuredheight",correctGMLdata))/5;

            // GoalInfo gmlData = new GoalInfo { goalPosition = goalPos, measuredheight = GetAttribute("measuredheight",correctGMLdata), isHintActive=false, capacity=capacityNum,evacueeNum=0};
            // GoalAttributeDict.Add(rndBuilding.Key,gmlData);
            // goalPos += new Vector3(-467.28f,0f,-1869.266f);
            // GenerateTargetFlag(goalPos,rndBuilding.Key);
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
            
            // for(int i=0;i<goalNum;i++)
            // {
                SelectGoal();
            // }

            //正解の建物の情報を取得
            // goalBuilding = GameObject.Find(rndBuilding.Key);
            // goalBounds = goalBuilding.GetComponent<MeshCollider>().sharedMesh.bounds;
            //選ばれた建物の位置情報を取得
            // goalPos = new Vector3(goalBounds.center.x+320f,goalBounds.center.y+goalBounds.size.y,goalBounds.center.z+380f);
            
            //Helperの位置を変更
             GameObject.Find("Helper").transform.position = goalPos;
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
        public void GetHintItem()
        {          
            GoalInfo hintBuildingValue;
            string hintBuildingName = "";
            GameObject flag;
            GameObject Marker;
            //表示させる建物の情報を決める
            foreach(var goalAttribute in GoalAttributeDict)
            {
                if(!goalAttribute.Value.isHintActive)
                {
                    hintBuildingName = goalAttribute.Key;
                    hintBuildingValue = goalAttribute.Value;
                    hintBuildingValue.buildingObj.tag = "Goal";

                    UIManageScript.DisplayAnswer(hintBuildingName,hintBuildingValue.measuredheight,hintBuildingValue.capacity.ToString(),hintBuildingValue.evacueeNum.ToString());
                    break;
                }
            }
            if(hintBuildingName == "")
            {
                return;
            }
            else
            {
                var tmpGoalAttribute = GoalAttributeDict[hintBuildingName];
                tmpGoalAttribute.isHintActive = true;
                GoalAttributeDict[hintBuildingName] = tmpGoalAttribute;
            }
            flag = GameObject.Find(hintBuildingName+"flag");
            flag.GetComponent<MeshRenderer>().enabled = true;
            Marker = GameObject.Find(hintBuildingName+"Marker");
            var MarkerTexture = Marker.transform.GetChild(0).gameObject.transform;
            MarkerTexture.GetComponent<MeshRenderer>().enabled = true;

            

            // nearestBuildingName = FindNearestGoal();

            // if(itemName == "measuredheight")
            // {
            //     hint = GoalAttributeDict[nearestBuildingName].measuredheight;
            // }
            // else if(itemName == "Usage")
            // {
            //     hint = GoalAttributeDict[nearestBuildingName].Usage;
            // }
            // else
            // {
            //     hint = GoalAttributeDict[nearestBuildingName].saboveground;
            // }
            // UIManageScript.DisplayAnswerGML(itemName,hint,nearestBuildingName);

            // //フィルター関連の表示
            // TimeManageScript.ColorBuilding(itemName,nearestBuildingName,hint);
        }

        // -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// アイテムを生成する
        /// </summary>

        public void SpawnHintItem()
        {
            ItemManageScript.GenerateItem();
        }
        private void GenerateTargetFlag(Vector3 flagPosition,string flagName)
        {
            GameObject flag = Instantiate(targetFlag,transform.root.gameObject.transform) as GameObject;
            flag.name = flagName+"flag";
            flag.transform.position = flagPosition;
            flag.GetComponent<MeshRenderer>().enabled = false;

            GameObject Marker = Instantiate(GoalMarker,transform.root.gameObject.transform) as GameObject;
            Marker.name = flagName + "Marker";
            Marker.transform.localScale = new Vector3(13f, 1f, 13f);
            Marker.transform.position = new Vector3(flagPosition.x,-500,flagPosition.z);
            var MarkerTexture = Marker.transform.GetChild(0).gameObject.transform;
            MarkerTexture.GetComponent<MeshRenderer>().enabled = false;
        }

        // --------------------------------------------------------------------------------------------------------------
        //プレイヤーがゴールの建物にたどり着いた時の処理
        public void SelectBuildingAction(Transform clickedBuilding)
        {
            GoalInfo tmpGoalAttribute = GoalAttributeDict[clickedBuilding.name];

            if(rescuingNum > 0)
            {
                //建物に収容できる残り人数
                int remainingNum = tmpGoalAttribute.capacity - tmpGoalAttribute.evacueeNum;
                //救助された(建物に送り出された)人数
                int sendNum;

                //救助中の人数>残り収容人数ならば救助中の人数ー残り収容人数、そうでなければ0
                if(rescuingNum > remainingNum)
                {
                    tmpGoalAttribute.evacueeNum += remainingNum;
                    sendNum = remainingNum;
                    rescuingNum -= remainingNum;
                }
                else
                {
                    tmpGoalAttribute.evacueeNum += rescuingNum;
                    sendNum = rescuingNum;
                    rescuingNum = 0;
                }

                //救助中の人数-1
                //rescuingNum -= 1;
                //建物にはいった救助者+1
                //tmpGoalAttribute.evacueeNum += 1;

                //NPCが向かうTransformの値をセット
                Transform goalTransform = GameObject.Find(clickedBuilding.name + "flag").transform;
                //NPCを救助する
                NPCManageScript.SendBuilding(sendNum);

                //rescuedNum += 1;

                //収容人数の最後の1人が入る時
                if(tmpGoalAttribute.capacity == tmpGoalAttribute.evacueeNum)
                {
                    //ゴールの建物のタグを元に戻す
                    clickedBuilding.gameObject.tag = "Untagged";
                    UIManageScript.DeleteAnswer(clickedBuilding.name);
                    GoalAttributeDict.Remove(clickedBuilding.name);
                    
                    GameObject flag = GameObject.Find(clickedBuilding.name + "flag");
                    GameObject Marker = GameObject.Find(clickedBuilding.name + "Marker");
                    Destroy(flag);
                    Destroy(Marker);
                    // 新しいゴールの生成
                    SelectGoal();
                }
                else
                {
                    GoalAttributeDict[clickedBuilding.name] = tmpGoalAttribute;
                }

                UIManageScript.SelectCityObject(clickedBuilding);
                UIManageScript.EditMissionText();
            }
        }
        //助けた人数を追加する処理
        public void AddRescueNum()
        {
            rescuedNum++;
            UIManageScript.DisplayRescuedNum();
        }
        //現在助けている人数を追加する関数
        public void ContactHumanAction()
        {
            rescuingNum += 1;
            UIManageScript.DisplayRescuingNum();
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
        private void ResetGoals()
        {
            // 建物の色を初期化
            //ゴールタグの初期化
            foreach (var goalAttribute in GoalAttributeDict)
            {
                Renderer renderer = goalAttribute.Value.buildingObj.GetComponent<Renderer>();
                for (int i = 0; i < renderer.materials.Length; ++i)
                {
                    renderer.materials[i].color = Color.white;
                }
                goalAttribute.Value.buildingObj.tag = "Untagged";
            }

        }

        //ゲームの終了処理
        public void OnEndGame()
        {
            ResetGoals();
            GoalAttributeDict.Clear();
            EnemyManageScript.DestroyEnemy();
            ItemManageScript.DestroyItem();
            UIManageScript.PlayerPosCamera.enabled = false;          
            UIManageScript.HideGameUI();
            NPCManageScript.DestroyNPC();
            playerInput.enabled = false;

        }
    }
}
