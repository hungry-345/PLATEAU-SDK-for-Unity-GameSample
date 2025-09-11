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
            EnvironmentScript.m_Rain = 0;
            var gameView = ViewBase.Instantiate<GameView>("GameView");
            EnvironmentScript.m_Rain = 1;
            yield return gameView.Wait();
            gameView.DestroyView();
        }
#endif
    }

}
