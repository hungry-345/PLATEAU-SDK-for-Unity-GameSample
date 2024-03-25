using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManage : MonoBehaviour
{
    [SerializeField] private Dictionary<GameObject, List<Transform>> neighborDic = new Dictionary<GameObject, List<Transform>>();
    private void Awake()
    {
        for(int i=0;i<transform.childCount;i++)
        {
            AddNeighbor(transform.GetChild(i).gameObject, transform.GetChild(i).GetComponent<RoadObject>().neighborObjects);
        }
    }
    //道路オブジェクトのTransformと，隣接する道路オブジェクトのTransformリストを登録
    public void AddNeighbor(GameObject roadObj, List<Transform> neighborTransforms)
    {
        if (neighborDic.ContainsKey(roadObj))
        {
            neighborDic[roadObj] = neighborTransforms;
        }
        else
        {
            neighborDic.Add(roadObj, neighborTransforms);
        }
    }
    // 指定された道路オブジェクトに隣接する道路オブジェクトのPositionを取得するメソッド
    //一つ前に通った道路オブジェクトは候補から除外する
    public GameObject GetRandomNeighbor(GameObject roadObj, GameObject lastVisitedRoad)
    {
        if (neighborDic.ContainsKey(roadObj) && neighborDic[roadObj]!= null)
        {
            if(roadObj.CompareTag("Road"))//道路
            {
                //既に通った道ではない道路オブジェクトを返す
                for(int i=0;i< neighborDic[roadObj].Count;i++)
                {
                    if (neighborDic[roadObj][i].gameObject == lastVisitedRoad)
                    {
                        continue;
                    }
                    else
                    {
                        return neighborDic[roadObj][i].gameObject;    
                    }
                }
            }
            else//交差点
            {
                while(true)//一つ前に通った以外の道路オブジェクトが選択されるまで繰り返す
                {
                    int randomIndex = Random.Range(0, neighborDic[roadObj].Count);
                    if (neighborDic[roadObj][randomIndex].gameObject == lastVisitedRoad) continue;
                    return neighborDic[roadObj][randomIndex].gameObject;
                }
            }
        }
        return null;
    }
    // １番近い道路オブジェクトを取得する
    public GameObject GetNearestRoadObject(Transform characterTrans)
    {
        GameObject result = null;
        float minTargetDistance = float.MaxValue;
        foreach (var target in neighborDic.Keys)
        {
            // 前回計測したオブジェクトよりも近くにあれば記録
            float targetDistance = Vector3.Distance(characterTrans.position, target.GetComponent<Renderer>().bounds.center);
            if (!(targetDistance < minTargetDistance)) continue;
            minTargetDistance = targetDistance;
            result = target;
        }
        // 最後に記録されたオブジェクトを返す
        return result;
    }
}

