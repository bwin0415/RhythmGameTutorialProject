using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using System;

public class SongManager : MonoBehaviour
{
    // 用於控制音樂的各種參數和組件的變數
    public static SongManager Instance;
    public AudioSource audioSource;
    public Lane[] lanes;
    public float songDelayInSeconds; // 歌曲延遲時間（秒）
    public double marginOfError; // 容錯範圍，以秒為單位
    public int inputDelayInMilliseconds; // 輸入延遲時間（毫秒）

    // MIDI 文件的位置和音符相關的資訊
    public string fileLocation;
    public float noteTime;
    public float noteSpawnY;
    public float noteTapY;

    // 靜態的 MIDI 文件實例，用於在整個類別中共用
    public static MidiFile midiFile;

    // 遊戲開始時執行
    void Start()
    {
        Instance = this;
        // 根據檔案位置的類型，從本地或網頁讀取 MIDI 文件
        if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://"))
        {
            StartCoroutine(ReadFromWebsite());
        }
        else
        {
            ReadFromFile();
        }
    }

    // 從網頁讀取 MIDI 文件
    private IEnumerator ReadFromWebsite()
    {
        // 使用 UnityWebRequest 從網頁獲取檔案內容
        using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileLocation))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                byte[] results = www.downloadHandler.data;
                using (var stream = new MemoryStream(results))
                {
                    // 讀取 MIDI 文件並從中提取音符資訊
                    midiFile = MidiFile.Read(stream);
                    GetDataFromMidi();
                }
            }
        }
    }

    // 從本地檔案讀取 MIDI
    private void ReadFromFile()
    {
        // 讀取 MIDI 文件並從中提取音符資訊
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        GetDataFromMidi();
    }

    // 提取 MIDI 文件中的音符資訊
    public void GetDataFromMidi()
    {
        // 獲取所有音符事件，包括鼓譜
        var drums = midiFile.GetNotes();
        var drumArray = new Melanchall.DryWetMidi.Interaction.Note[drums.Count];
        drums.CopyTo(drumArray, 0);

        // 將提取的音符資訊傳遞給每個 Lane 物件
        foreach (var lane in lanes)
        {
            lane.SetTimeStamps(drumArray);
        }

        // 延遲一段時間後開始播放音樂
        Invoke(nameof(StartSong), songDelayInSeconds);
    }

    // 獲取每個四分音符的時間分辨率
    public float GetTicksPerQuarterNote()
    {
        var timeDivision = midiFile.TimeDivision;
        int ticksPerQuarterNote = 0;

        if (timeDivision is TicksPerQuarterNoteTimeDivision)
        {
            var ticksPerQuarterNoteTimeDivision = timeDivision as TicksPerQuarterNoteTimeDivision;
            ticksPerQuarterNote = ticksPerQuarterNoteTimeDivision.TicksPerQuarterNote;
        }

        return ticksPerQuarterNote; // 返回時間分辨率的值
    }

    // 開始播放音樂
    public void StartSong()
    {
        audioSource.Play();
    }

    // 獲取音頻源的時間
    public static double GetAudioSourceTime()
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    // Update 函數在每一幀調用
    void Update()
    {

    }
}