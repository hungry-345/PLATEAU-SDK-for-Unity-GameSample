using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBase : MonoBehaviour
{
    public virtual IEnumerator Wait()
    {
        yield return null;
    }

    //‰æ–Ê‚ğ”jŠü‚·‚é
    public void DestroyView()
    {
        Debug.Log("Destroy");
        Destroy(this.gameObject);
    }

    /// ‰æ–Ê‚ğ¶¬‚·‚é
    /// <typeparam name="T">View‚ÌŒp³</typeparam>
    /// <param name="viewName">View–¼</param>
    public static T Instantiate<T>(string viewName) where T : ViewBase
    {
        var prefab = Resources.Load<T>("Prefabs/ViewPrefabs/"+viewName);
        prefab.name = viewName;
        var view = Instantiate(prefab);
        view.name = viewName;
        return view;
    }

}