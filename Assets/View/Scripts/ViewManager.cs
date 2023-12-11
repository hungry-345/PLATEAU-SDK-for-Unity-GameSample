using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour
{

    //画面遷移用のキャンバス
    //public CanvasGroup canvas;
    public static ViewManager instance = null;
    //ゲームスコア
    public int score;

    //シングルトン
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    private void Start()
    {
        StartCoroutine(Flow());
    }

    IEnumerator Flow()
    {

#if RELEASE_MODE
   
#else
        //var debugView = View.Instantiate<DebugView>("Prefabs/View/DebugView");
        //yield return debugView.Wait();
        //debugView.Destroy();

        //ひとつ前のViewをキャッシュ
        //ViewBase pre;


        //ゲームループ
        while(true)
        {
            //タイトル画面をロード
            var titleView = ViewBase.Instantiate<TitleView>("TitleView");
            //タイトル画面表示中
            yield return titleView.Wait();
            //タイトル画面を破棄
            titleView.DestroyView();

            //ゲーム画面をロード
            var gameView = ViewBase.Instantiate<GameView>("GameView");
            //ゲーム画面表示中
            yield return gameView.Wait();
            //ゲーム画面を破棄
            gameView.DestroyView();


            ////リザルト画面をロード
            //var resultView = ViewBase.Instantiate<ResultView>("Prefabs/ViewPrefabs/ResultView");
            ////リザルト画面表示中
            //yield return resultView.Wait();
            ////リザルト画面を破棄
            //resultView.DestroyView();

        }


#endif
    }
}
