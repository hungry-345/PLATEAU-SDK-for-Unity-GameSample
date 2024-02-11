using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
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
    public GameObject GetRandomNeighbor(GameObject roadObj)
    {
        if (neighborDic.ContainsKey(roadObj) && neighborDic[roadObj]!= null)
        {
            int randomIndex = Random.Range(0, neighborDic[roadObj].Count);
            return neighborDic[roadObj][randomIndex].gameObject; 
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

