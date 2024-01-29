using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour
{
    public static ViewManager instance = null;
    public int score;
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
        while(true)
        {
            var titleView = ViewBase.Instantiate<TitleView>("TitleView");
            titleView.transform.position = Vector3.zero;
            yield return titleView.Wait();
            titleView.DestroyView();
            var gameView = ViewBase.Instantiate<GameView>("GameView");
            yield return gameView.Wait();
            gameView.DestroyView();
        }
#endif
    }
}
