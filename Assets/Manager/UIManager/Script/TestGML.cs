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
    public class TestGML : MonoBehaviour
    {
        public readonly Dictionary<string, SampleGml> gmls = new Dictionary<string, SampleGml>();
        private PLATEAUInstancedCityModel[] instancedCityModels;

        private void Awake()
        {
            // InputSystemのインスタンス(初期値)
            //Plateauのデータを取得
            InitializeAsync().ContinueWithErrorCatch();
        }
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        private async Task InitializeAsync()
        {
            // 初期化UIを起動
            // initializingUi.gameObject.SetActive(true);
            // Plateauのデータを取得
            instancedCityModels = FindObjectsOfType<PLATEAUInstancedCityModel>();
            if (instancedCityModels == null || instancedCityModels.Length == 0)
            {
                return;
            }
            foreach(var instancedCityModel in instancedCityModels)
            {
                var rootDirName = instancedCityModel.name;
                Debug.Log(instancedCityModel.transform.childCount);
                for (int i = 0; i < instancedCityModel.transform.childCount; ++i)
                {
                    var go = instancedCityModel.transform.GetChild(i).gameObject;
                    // サンプルではdemを除外します。
                    if (go.name.Contains("dem")) continue;
                    var cityModel = await PLATEAUCityGmlProxy.LoadAsync(go, rootDirName);
                    if (cityModel == null) continue;
                    var gml = new SampleGml(cityModel, go);
                    gmls.Add(go.name, gml);
                    Debug.Log("yes");
                }
            }
            Debug.Log("finish");
            // isInitialiseFinish = true;
        }
    }
}