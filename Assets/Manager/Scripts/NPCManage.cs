using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManage : MonoBehaviour
{
    [SerializeField, Tooltip("NPCPrefab")] private GameObject[] NPCPrefabs;
    [SerializeField, Tooltip("NPCSpawnPositions")] private Transform[] NPCSpawnPositions;
    [SerializeField, Tooltip("NPCNum")] private int NPCNum = 5;
    [SerializeField] private List<GameObject> followNPCList = new List<GameObject>();

    // 🎵 ランダム音声関連
    [Header("音声クリップ（おはよう／ちょっと待って／バイバーイ／初めまして／また明日）")]
    [SerializeField] private AudioClip[] voiceClips;

    // ランダム再生間隔設定
    [SerializeField] private float minVoiceInterval = 5f;
    [SerializeField] private float maxVoiceInterval = 15f;

    public void InitializeNPC()
    {
        GenerateNPC();
    }

    void GenerateNPC()
    {
        for (int i = 0; i < NPCNum; i++)
        {
            int r = Random.Range(0, NPCPrefabs.Length);
            GameObject npc = Instantiate(NPCPrefabs[r], NPCSpawnPositions[i].position, Quaternion.identity, this.gameObject.transform);

            AddFollowList(npc);

            // 🎤 ランダム音源をNPCに設定して、再生コルーチン開始
            AttachAndPlayRandomVoice(npc);
        }
    }

    /// <summary>
    /// NPCにランダム音源を付与し、定期的に再生する
    /// </summary>
    private void AttachAndPlayRandomVoice(GameObject npc)
    {
        if (voiceClips == null || voiceClips.Length == 0) return;

        // AudioSource がなければ追加
        AudioSource audioSource = npc.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = npc.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3Dサウンドに
            audioSource.maxDistance = 20f;
        }

        // NPC専用のコルーチンを開始（NPCごとに個別で鳴らす）
        StartCoroutine(PlayVoiceRoutine(audioSource));
    }

    /// <summary>
    /// NPCごとにランダムなタイミングで音声を再生するコルーチン
    /// </summary>
    private IEnumerator PlayVoiceRoutine(AudioSource source)
    {
        // NPCが破棄されるまでループ
        while (source != null)
        {
            yield return new WaitForSeconds(Random.Range(minVoiceInterval, maxVoiceInterval));

            if (voiceClips.Length > 0 && !source.isPlaying)
            {
                AudioClip clip = voiceClips[Random.Range(0, voiceClips.Length)];
                source.PlayOneShot(clip);
            }
        }
    }

    public void SendBuilding(int NPCNum)
    {
        for (int i = 0; i < NPCNum; i++)
        {
            int n = Random.Range(0, followNPCList.Count);
            NPCController npcController = followNPCList[n].GetComponent<NPCController>();
            // NPCController に指示を送る処理が今後追加される想定
        }
    }

    public void DestroyNPC()
    {
        foreach (Transform n in gameObject.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
        followNPCList.Clear();
    }

    public void AddFollowList(GameObject NPC)
    {
        followNPCList.Add(NPC);
    }

    public void RemoveFollowList(GameObject NPC)
    {
        followNPCList.Remove(NPC);
    }
}
