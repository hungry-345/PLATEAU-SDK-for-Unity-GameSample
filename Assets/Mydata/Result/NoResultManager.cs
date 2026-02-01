using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoResultManager : MonoBehaviour
{
    [Header("UI テキスト")]
    [SerializeField] private TextMeshProUGUI clearDistanceText; // 今回の距離
    [SerializeField] private TextMeshProUGUI rank1Text;         // 1位
    [SerializeField] private TextMeshProUGUI rank2Text;         // 2位
    [SerializeField] private TextMeshProUGUI rank3Text;         // 3位

    private const string RankingKey = "FailDistanceRanking";

    void Start()
    {
        float distance = DistanceChecker.LastDistanceAtTimeUp;

        // 今回の距離表示
        clearDistanceText.text = $"{distance:F1} m";

        // ランキングに保存
        SaveDistance(distance);

        // ランキング表示
        ShowRanking();
    }

    void Update()
    {
        // Dキーでランキング初期化
        if (Input.GetKeyDown(KeyCode.D))
        {
            ClearRanking();
        }
    }

    private void SaveDistance(float distance)
    {
        List<float> distances = LoadRanking();
        distances.Add(distance);

        // 「近いほど上位」なので昇順
        distances.Sort();

        if (distances.Count > 5)
            distances = distances.GetRange(0, 5);

        string json = JsonUtility.ToJson(new RankingData(distances));
        PlayerPrefs.SetString(RankingKey, json);
        PlayerPrefs.Save();
    }

    private List<float> LoadRanking()
    {
        if (!PlayerPrefs.HasKey(RankingKey))
            return new List<float>();

        string json = PlayerPrefs.GetString(RankingKey);
        RankingData data = JsonUtility.FromJson<RankingData>(json);
        return data != null ? data.distances : new List<float>();
    }

    private void ShowRanking()
    {
        List<float> distances = LoadRanking();

        // ランキングが空の場合
        if (distances.Count == 0)
        {
            rank1Text.text = "-- m";
            rank2Text.text = "-- m";
            rank3Text.text = "-- m";
            return;
        }

        rank1Text.text = distances.Count > 0 ? $"{distances[0]:F1} m" : "-- m";
        rank2Text.text = distances.Count > 1 ? $"{distances[1]:F1} m" : "-- m";
        rank3Text.text = distances.Count > 2 ? $"{distances[2]:F1} m" : "-- m";
    }

    private void ClearRanking()
    {
        PlayerPrefs.DeleteKey(RankingKey);
        PlayerPrefs.Save();

        ShowRanking();
    }

    [Serializable]
    private class RankingData
    {
        public List<float> distances;
        public RankingData(List<float> distances)
        {
            this.distances = distances;
        }
    }
}
