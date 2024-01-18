using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour
{

    //��ʑJ�ڗp�̃L�����o�X
    //public CanvasGroup canvas;
    public static ViewManager instance = null;
    //�Q�[���X�R�A
    public int score;

    //�V���O���g��
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

        //�ЂƂO��View���L���b�V��
        //ViewBase pre;


        //�Q�[�����[�v
        while(true)
        {
            //�^�C�g����ʂ����[�h
            var titleView = ViewBase.Instantiate<TitleView>("TitleView");
            titleView.transform.position = Vector3.zero;
            //�^�C�g����ʕ\����
            yield return titleView.Wait();
            //�^�C�g����ʂ�j��
            titleView.DestroyView();

            //�Q�[����ʂ����[�h
            var gameView = ViewBase.Instantiate<GameView>("GameView");
            gameView.transform.position = new Vector3(160,65,1500);
            //�Q�[����ʕ\����
            yield return gameView.Wait();
            //�Q�[����ʂ�j��
            gameView.DestroyView();


            ////���U���g��ʂ����[�h
            //var resultView = ViewBase.Instantiate<ResultView>("Prefabs/ViewPrefabs/ResultView");
            ////���U���g��ʕ\����
            //yield return resultView.Wait();
            ////���U���g��ʂ�j��
            //resultView.DestroyView();

        }


#endif
    }
}
