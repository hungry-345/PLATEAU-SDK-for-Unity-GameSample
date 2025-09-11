/*using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultView : ViewBase
{
    [SerializeField] private ExtendButton toTitleButton;
    
    // ボタンサウンド
    private CursorManager cursorManager;

    void Start()
    {
        cursorManager = GameObject.Find("Cursor").GetComponent<CursorManager>();
    }

    public override IEnumerator Wait()
    {
        while (true)
        {
            // タイトルに戻る
            if (toTitleButton.IsClicked)
            {
                cursorManager.CursorSoundPlay();

                // タイトルシーンをロード
                SceneManager.LoadScene("TitleScene");
                yield break;
            }

            yield return null;
        }
    }
}
*/