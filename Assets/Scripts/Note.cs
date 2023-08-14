using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    double timeInstantiated; // 音符生成的時間戳
    public float assignedTime; // 指定的音符時間
    void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime(); // 取得音頻源的時間
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated; // 從生成到現在的時間間隔
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2)); // 計算動畫進度的參數

        if (t > 1)
        {
            Destroy(gameObject); // 如果動畫進度超過 1，銷毀音符物件
        }
        else
        {
            // 在生成位置和點擊位置之間執行線性差值，實現音符下落的動畫
            transform.localPosition = Vector3.Lerp(Vector3.up * SongManager.Instance.noteSpawnY, Vector3.up * SongManager.Instance.noteTapY, t);
            GetComponent<SpriteRenderer>().enabled = true; // 啟用音符的 SpriteRenderer，顯示音符
        }
    }
}