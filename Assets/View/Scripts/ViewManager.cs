using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlateauToolkit.Rendering;
public class ViewManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public static ViewManager instance = null;
    public int score;
    private void Awake()
    {
        Cursor.visible = false;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        mainCamera.gameObject.SetActive(false);
        
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
            var EnvironmentScript = GameObject.Find("Environment").GetComponent<EnvironmentController>();
            var titleView = ViewBase.Instantiate<TitleView>("TitleView");
            EnvironmentScript.m_Rain = 0;
            titleView.transform.position = Vector3.zero;
            yield return titleView.Wait();
            titleView.DestroyView();
            var gameView = ViewBase.Instantiate<GameView>("GameView");
            EnvironmentScript.m_Rain = 1;
            yield return gameView.Wait();
            gameView.DestroyView();
        }
#endif
    }

}
