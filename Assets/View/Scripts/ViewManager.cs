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
        if (instance == null)
        {
            Cursor.visible = false;
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            if (mainCamera != null)
            {
                mainCamera.gameObject.SetActive(false);
            }
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
            // Wait for Game Scene
            while (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameSample 1")
            {
                yield return null;
            }

            var EnvironmentObj = GameObject.Find("Environment");
            if (EnvironmentObj == null) 
            {
                yield return null;
                continue;
            }

            var EnvironmentScript = EnvironmentObj.GetComponent<EnvironmentController>();
            EnvironmentScript.m_Rain = 0;
            var gameView = ViewBase.Instantiate<GameView>("GameView");
            EnvironmentScript.m_Rain = 1;
            yield return gameView.Wait();
            gameView.DestroyView();
        }
#endif
    }

}
