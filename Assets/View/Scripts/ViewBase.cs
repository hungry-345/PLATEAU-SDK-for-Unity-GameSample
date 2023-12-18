using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBase : MonoBehaviour
{
    public virtual IEnumerator Wait()
    {
        yield return null;
    }

    //テスト
    //画面を破棄する
    public void DestroyView()
    {
        Destroy(this.gameObject);
    }

    /// 画面を生成する
    /// <typeparam name="T">Viewの継承</typeparam>
    /// <param name="viewName">View名</param>
    public static T Instantiate<T>(string viewName) where T : ViewBase
    {
        var prefab = Resources.Load<T>("Prefabs/ViewPrefabs/"+viewName);
        prefab.name = viewName;
        var view = Instantiate(prefab);
        view.name = viewName;
        return view;
    }

}