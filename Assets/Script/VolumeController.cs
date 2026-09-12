using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("输入音量")]
    public Slider inputVolumeSlider;

    [Header("输出音量")]
    public Slider outputVolumeSlider;

    // 当前输入音量
    public float InputVolume { get; private set; }

    // 当前输出音量
    public float OutputVolume { get; private set; }

    void Start()
    {
        InputVolume = inputVolumeSlider.value;
        OutputVolume = outputVolumeSlider.value;

        inputVolumeSlider.onValueChanged.AddListener(SetInputVolume);
        outputVolumeSlider.onValueChanged.AddListener(SetOutputVolume);
    }

    void SetInputVolume(float value)
    {
        InputVolume = value;

        Debug.Log("输入音量：" + InputVolume);
    }

    void SetOutputVolume(float value)
    {
        OutputVolume = value;

        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.volume = value;
        }

        Debug.Log("输出音量：" + OutputVolume);
    }

}
