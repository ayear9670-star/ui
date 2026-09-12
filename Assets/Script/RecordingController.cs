using System.Collections.Generic;
using UnityEngine;

public class RecordingController : MonoBehaviour
{
    [Header("音量控制")]
    public VolumeController volumeController;

    private AudioSource audioSource;

    private AudioClip recordingClip;
    private AudioClip finalRecordingClip;

    private string microphoneDevice;

    private bool isRecording = false;
    private bool isPaused = false;

    // 保存每一段录音的数据
    private List<float[]> recordedSegments = new List<float[]>();

    private int recordingFrequency = 44100;
    private int recordingChannels = 1;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // 开始录音 / 继续录音
    public void StartRecording()
    {
        if (!isRecording)
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("没有检测到麦克风！");
                return;
            }

            microphoneDevice = Microphone.devices[0];

            recordedSegments.Clear();

            StartMicrophoneRecording();

            isRecording = true;
            isPaused = false;

            Debug.Log("开始录音");
        }
        else if (isPaused)
        {
            StartMicrophoneRecording();

            isPaused = false;

            Debug.Log("继续录音");
        }
        else
        {
            Debug.Log("当前正在录音，不需要重复开始");
        }
    }

    // 暂停 / 继续
    public void PauseRecording()
    {
        if (!isRecording)
        {
            Debug.Log("当前没有正在进行的录音");
            return;
        }

        if (!isPaused)
        {
            // 真正结束当前这一段麦克风录音
            SaveCurrentSegment();

            isPaused = true;

            Debug.Log("暂停录音");
        }
        else
        {
            // 再次点击后重新开始录音
            StartMicrophoneRecording();

            isPaused = false;

            Debug.Log("继续录音");
        }
    }

    // 停止录音
    public void StopRecording()
    {
        if (!isRecording)
        {
            Debug.Log("当前没有正在进行的录音");
            return;
        }

        // 如果当前不是暂停状态，先保存最后一段
        if (!isPaused)
        {
            SaveCurrentSegment();
        }

        isRecording = false;
        isPaused = false;

        if (recordedSegments.Count == 0)
        {
            Debug.LogError("没有获取到有效的录音数据！");
            return;
        }

        // 计算所有片段的总长度
        int totalSamples = 0;

        foreach (float[] segment in recordedSegments)
        {
            totalSamples += segment.Length;
        }

        // 合并所有录音片段
        float[] allSamples = new float[totalSamples];

        int currentIndex = 0;

        foreach (float[] segment in recordedSegments)
        {
            segment.CopyTo(allSamples, currentIndex);
            currentIndex += segment.Length;
        }

        // 根据输入音量调整录音音量
        float inputVolume = 1f;

        if (volumeController != null)
        {
            inputVolume = volumeController.InputVolume;
        }

        for (int i = 0; i < allSamples.Length; i++)
        {
            allSamples[i] *= inputVolume;
        }

        // 创建最终录音 AudioClip
        int totalFrameCount = totalSamples / recordingChannels;

        finalRecordingClip = AudioClip.Create(
        "MyRecording",
        totalFrameCount,
        recordingChannels,
        recordingFrequency,
        false
        );

        finalRecordingClip.SetData(allSamples, 0);

        float duration =
        (float)totalFrameCount / recordingFrequency;

        Debug.Log("停止录音");
        Debug.Log("实际录音时间：" + duration + " 秒");
        Debug.Log("录音片段数量：" + recordedSegments.Count);
        Debug.Log("录音已经合并到内存中的 AudioClip");

        // 播放最终录音
        if (audioSource != null)
        {
            audioSource.clip = finalRecordingClip;

            if (volumeController != null)
            {
                audioSource.volume = volumeController.OutputVolume;
            }

            audioSource.Play();

            Debug.Log("开始播放录音");
        }
        else
        {
            Debug.LogError("没有找到 Audio Source！");
        }

        recordedSegments.Clear();
    }

    // 开始一段新的麦克风录音
    private void StartMicrophoneRecording()
    {
        recordingClip = Microphone.Start(
        microphoneDevice,
        false,
        300,
        recordingFrequency
        );

        recordingChannels = recordingClip.channels;
    }

    // 保存当前正在录制的片段
    private void SaveCurrentSegment()
    {
        if (recordingClip == null)
        {
            return;
        }

        int position = Microphone.GetPosition(microphoneDevice);

        Microphone.End(microphoneDevice);

        if (position <= 0)
        {
            Debug.Log("当前录音片段没有有效数据");
            return;
        }

        int sampleCount = position * recordingClip.channels;

        float[] samples = new float[sampleCount];

        recordingClip.GetData(samples, 0);

        recordedSegments.Add(samples);

        Debug.Log("已保存一个录音片段，长度：" + position + " 帧");

        recordingClip = null;
    }

    // 获取当前输入音量
    public float GetInputVolume()
    {
        if (volumeController == null)
        {
            return 1f;
        }

        return volumeController.InputVolume;
    }
}
