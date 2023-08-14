using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Lane : MonoBehaviour
{
    public KeyCode input;
    public GameObject notePrefab;
    public DrumNote drumNote; // 选择的鼓声枚举
    public List<double> timeStamps = new List<double>();
    public List<Melanchall.DryWetMidi.Interaction.Note> spawnedNotes = new List<Melanchall.DryWetMidi.Interaction.Note>(); // 存储已生成的音符对象

    #region Private Variables

    List<GameObject> notes = new List<GameObject>();
    AudioSource audioSource;
    int spawnIndex = 0;
    int inputIndex = 0;
    #endregion
    // 映射音符号和枚举值的字典
    Dictionary<int, DrumNote> noteMappings = new Dictionary<int, DrumNote>()
    {
        { 35, DrumNote.B1 },   // bass drum
        { 36, DrumNote.C2 },
        { 38, DrumNote.D2 },   // snare drum
        { 42, DrumNote.FSharp2 },  // closed hi-hat
        { 47, DrumNote.B2 },   // tom 2
        { 43, DrumNote.G2 },   // floor tom
        { 40, DrumNote.E2 },   // crash cymbal 1
        { 45, DrumNote.A2 },   // crash cymbal 2
        { 50, DrumNote.D3 }    // ride cymbal
    };

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            // 在这里根据鼓谱事件的特征来提取时间戳，例如根据通道、音符号等
            // 以下示例假设 MIDI 鼓谱事件位于特定的通道上
            int noteNumber = note.NoteNumber;
            if (noteMappings.ContainsKey(noteNumber) && noteMappings[noteNumber] == drumNote) // 确保音符号在映射字典中并与选择的枚举匹配
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
                spawnedNotes.Add(note); // 将已生成的音符对象添加到列表
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(input))
        {
            PlaySound(); // 播放声音
        }
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
            {
                var note = Instantiate(notePrefab, transform);
                notes.Add(note);
                spawnIndex++;
            }
        }

        if (inputIndex < notes.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double marginOfError = SongManager.Instance.marginOfError;
            double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

            if (audioTime >= timeStamp - marginOfError && audioTime <= timeStamp + marginOfError)
            {
                if (Input.GetKeyDown(input))
                {
                    ScoreManager.Instance.Hit(); // 调用 ScoreManager 的 Hit() 方法
                    print($"Hit on {inputIndex} note");
                    Destroy(notes[inputIndex]);
                    inputIndex++;
                }
            }
            else if (audioTime > timeStamp + marginOfError)
            {
                ScoreManager.Instance.Miss(); // 调用 ScoreManager 的 Miss() 方法
                print($"Missed {inputIndex} note");
                inputIndex++;
            }
        }

       
    }
    private void PlaySound()
    {
        // 确保有音频源和音频剪辑
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play(); // 播放声音
        }
    }


}

public enum DrumNote
{
    Rest,
    B1,   // bass drum
    C2,
    D2,   // snare drum
    FSharp2,  // closed hi-hat
    B2,   // tom 2
    G2,   // floor tom
    E2,   // crash cymbal 1
    A2,   // crash cymbal 2
    D3    // ride cymbal
}
