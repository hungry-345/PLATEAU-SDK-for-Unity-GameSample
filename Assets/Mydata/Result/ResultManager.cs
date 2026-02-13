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

    private const string TimeKey    = "ElapsedTime";
    private const string RankingKey = "RankingTimes";

    void Start()
    {
        if (PlayerPrefs.HasKey(TimeKey))
        {
            float elapsed = PlayerPrefs.GetFloat(TimeKey);
            TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.FloorToInt(elapsed));
            clearTimeText.text = $"{timeSpan:mm\\:ss}";

            SaveTime(elapsed);
            ShowRanking();
        }
        else
        {
            clearTimeText.text = "--:--";
            ShowRanking();
        }
    }

    void Update()
    {
        // Dキーでランキング初期化
        if (Input.GetKeyDown(KeyCode.D))
        {
            ClearRanking();
        }
    }

    private void SaveTime(float elapsed)
    {
        List<float> times = LoadRanking();
        times.Add(elapsed);

        // 速いほど上位
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

        // ランキングが空の場合
        if (times.Count == 0)
        {
            rank1Text.text = "--:--";
            rank2Text.text = "--:--";
            rank3Text.text = "--:--";
            return;
        }

        rank1Text.text = times.Count > 0 ? $"{TimeSpan.FromSeconds(times[0]):mm\\:ss}" : "--:--";
        rank2Text.text = times.Count > 1 ? $"{TimeSpan.FromSeconds(times[1]):mm\\:ss}" : "--:--";
        rank3Text.text = times.Count > 2 ? $"{TimeSpan.FromSeconds(times[2]):mm\\:ss}" : "--:--";
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
        public List<float> times;
        public RankingData(List<float> times)
        {
            this.times = times;
        }
    }
}
