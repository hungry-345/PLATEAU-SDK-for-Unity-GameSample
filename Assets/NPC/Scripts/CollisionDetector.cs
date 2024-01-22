using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
//キャラクターを検知するスクリプト
public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private TriggerEvent triggerEnter = new TriggerEvent();
    [SerializeField] private TriggerEvent triggerExit = new TriggerEvent();

    /// <summary>
    /// Is TriggerがONで他のColliderと重なっているときに呼ばれ続ける
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // InspectorタブのtriggerEnterで指定された処理を実行する
        triggerEnter.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        // InspectorタブのtriggerExitで指定された処理を実行する
        triggerExit.Invoke(other);
    }
    // UnityEventを継承したクラスに[Serializable]属性を付与することで、Inspectorウインドウ上に表示できるようになる。
    [Serializable]
    public class TriggerEvent : UnityEvent<Collider>
    {
    }
}
