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
    public class GameManage : MonoBehaviour
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
        private NPCManage NPCManageScript;
        private GameObject[] HintLst;
        private GameObject goalBuilding;
        private List<string> buildingDirName;
        private Bounds goalBounds;
        private Vector3 goalPos;
        private int goalNum;
        private int getHintNum;
        private bool isSetGMLdata;

        //player
        private GameObject player;
        //サウンドエフェクト
        [SerializeField] private AudioClip saveAudioClip;
        private AudioSource saveSound;

        // パーティクルエフェクト
        [SerializeField] private GameObject saveParticle;
        private GameObject saveParticleInstance;
        private float particleDuration = 2f;
        // -------------------------------------------------------------------------------------------------------------
        private void Awake()
        {
            player = GameObject.Find("PlayerArmature");
            inputActions = new InputGameManage();
            targetParent = GameObject.Find("52385628_bldg_6697_op.gml").transform;

            // 救出時
            saveSound = gameObject.AddComponent<AudioSource>();
            saveSound.clip = saveAudioClip;
            saveSound.loop = false;
        }
        public void StartGame()
        {
            rnd = new System.Random();

            
            //thirdpersonController = GameObject.Find("PlayerArmature").GetComponent<ThirdPersonController>();

            //SceneManagerからShow.csにアクセスする
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
            TimeManageScript = GameObject.Find("TimeManager").GetComponent<TimeManage>();
            EnemyManageScript = GameObject.Find("EnemyManager").GetComponent<EnemyManage>();
            ItemManageScript = GameObject.Find("ItemManager").GetComponent<ItemManage>();
            NPCManageScript= GameObject.Find("NPCManager").GetComponent<NPCManage>();

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
        /// <summary>
        /// ランダムな位置に1個ゴールを設置する
        /// </summary>
        public void SelectGoal()
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
            flag.GetComponentInChildren<ParticleSystem>().Play();
            Marker = GameObject.Find(hintBuildingName+"Marker");
            var MarkerTexture = Marker.transform.GetChild(0).gameObject.transform;
            MarkerTexture.GetComponent<MeshRenderer>().enabled = true;
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
            flag.GetComponentInChildren<ParticleSystem>().Pause();

            GameObject Marker = Instantiate(GoalMarker,transform.root.gameObject.transform) as GameObject;
            Marker.name = flagName + "Marker";
            Marker.transform.localScale = new Vector3(13f, 1f, 13f);
            Marker.transform.position = new Vector3(flagPosition.x,-500,flagPosition.z);
            var MarkerTexture = Marker.transform.GetChild(0).gameObject.transform;
            MarkerTexture.GetComponent<MeshRenderer>().enabled = false;
        }

        // --------------------------------------------------------------------------------------------------------------
        //プレイヤーがゴールの建物にたどり着いた時の処理
        public void SelectBuildingAction(Transform touchedBuilding)
        {
            GoalInfo tmpGoalAttribute = GoalAttributeDict[touchedBuilding.name];

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
                Transform goalTransform = GameObject.Find(touchedBuilding.name + "flag").transform;
                //NPCを救助する
                NPCManageScript.SendBuilding(sendNum);

                //rescuedNum += 1;

                //収容人数の最後の1人が入る時
                if(tmpGoalAttribute.capacity == tmpGoalAttribute.evacueeNum)
                {
                    //ゴールの建物のタグを元に戻す
                    touchedBuilding.gameObject.tag = "Untagged";
                    UIManageScript.DeleteAnswer(touchedBuilding.name);
                    GoalAttributeDict.Remove(touchedBuilding.name);
                    
                    GameObject flag = GameObject.Find(touchedBuilding.name + "flag");
                    GameObject Marker = GameObject.Find(touchedBuilding.name + "Marker");
                    Destroy(flag);
                    Destroy(Marker);
                    // 新しいゴールの生成
                    SelectGoal();
                }
                else
                {
                    GoalAttributeDict[touchedBuilding.name] = tmpGoalAttribute;
                }

                UIManageScript.SelectCityObject(touchedBuilding);
                UIManageScript.EditMissionText();
            }
        }
        //助けた人数を追加する処理
        public void AddRescueNum()
        {
            rescuedNum++;
            UIManageScript.DisplayRescuedNum();

            // パーティクルエフェクト
            saveParticleInstance = Instantiate(saveParticle, player.gameObject.transform.position, Quaternion.Euler(-90, 0, 0), player.gameObject.transform);
            Destroy(saveParticleInstance, particleDuration);
            // サウンドエフェクト
            saveSound.Play();
        }
        //現在助けている人数を追加する関数
        public void ContactHumanAction()
        {
            rescuingNum += 1;
            UIManageScript.DisplayRescuingNum();
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
                    renderer.materials[i].DisableKeyword("_EMISSION");
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
