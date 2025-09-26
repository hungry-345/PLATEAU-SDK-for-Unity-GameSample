using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [Header("UI テキスト")]
    [SerializeField] private TextMeshProUGUI clearTimeText; // 今回のクリアタイム
    [SerializeField] private TextMeshProUGUI rank1Text;     // 1位
    [SerializeField] private TextMeshProUGUI rank2Text;     // 2位
    [SerializeField] private TextMeshProUGUI rank3Text;     // 3位

    private const string TimeKey = "ElapsedTime";    
    private const string RankingKey = "RankingTimes";  

    void Start()
    {
        if (PlayerPrefs.HasKey(TimeKey))
        {
            // 1. 秒数を取得
            float elapsed = PlayerPrefs.GetFloat(TimeKey);
            TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.FloorToInt(elapsed));
            clearTimeText.text = $"{timeSpan:mm\\:ss}";


            // 3. ランキングに保存
            SaveTime(elapsed);

            // 4. ランキングをUIに表示
            ShowRanking();
        }
        else
        {
            clearTimeText.text = "タイムデータがありません。";
        }
    }

    private void SaveTime(float elapsed)
    {
        List<float> times = LoadRanking();
        times.Add(elapsed);
        times.Sort();

        if (times.Count > 5)
            times = times.GetRange(0, 5);

        string json = JsonUtility.ToJson(new RankingData(times));
        PlayerPrefs.SetString(RankingKey, json);
        PlayerPrefs.Save();
    }

    private List<float> LoadRanking()
    {
        if (!PlayerPrefs.HasKey(RankingKey))
            return new List<float>();

        string json = PlayerPrefs.GetString(RankingKey);
        RankingData data = JsonUtility.FromJson<RankingData>(json);
        return data != null ? data.times : new List<float>();
    }

    private void ShowRanking()
    {
        List<float> times = LoadRanking();

        // UIにランキングを反映
        rank1Text.text = times.Count > 0 ? $"1位: {TimeSpan.FromSeconds(times[0]):mm\\:ss}" : "1位: --:--";
        rank2Text.text = times.Count > 1 ? $"2位: {TimeSpan.FromSeconds(times[1]):mm\\:ss}" : "2位: --:--";
        rank3Text.text = times.Count > 2 ? $"3位: {TimeSpan.FromSeconds(times[2]):mm\\:ss}" : "3位: --:--";
    }

    [Serializable]
    private class RankingData
    {
        public List<float> times;
        public RankingData(List<float> times) { this.times = times; }
    }
}
