using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // 单例实例
    public AudioSource hitSFX; // 击中音效
    public AudioSource missSFX; // 错过音效
    public TMPro.TextMeshPro scoreText; // 分数显示的 TextMeshPro 文本对象
    private int comboScore; // 连击分数

    void Awake()
    {
        Instance = this; // 在 Awake 中设置单例实例
    }

    void Start()
    {
        comboScore = 0; // 初始化连击分数
        UpdateScoreText(); // 更新分数显示
    }

    public void Hit()
    {
        comboScore++; // 连击分数增加
        hitSFX.Play(); // 播放击中音效
        UpdateScoreText(); // 更新分数显示
    }

    public void Miss()
    {
        comboScore = 0; // 连击分数重置为 0
        missSFX.Play(); // 播放错过音效
        UpdateScoreText(); // 更新分数显示
    }

    private void UpdateScoreText()
    {
        scoreText.text = comboScore.ToString(); // 更新分数显示文本
    }
}