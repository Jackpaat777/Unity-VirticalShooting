using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClips;

    // �����̴��� ���� �������� ����
    public AudioMixer audioMixer;
    public Slider sliderMaster;
    public Slider sliderBGM;
    public Slider sliderSFX;

    // �ٸ� ��ũ��Ʈ������ AudioManager�� �ٷ� ������ �� �ֵ��� static���� �����
    public static AudioManager audioManager;

    AudioSource audioSource;

    void Awake()
    {
        // AudioManager.audioManager�� ���� ��� �� ��ũ��Ʈ�� �־��ֱ� (�ٸ� ��ũ��Ʈ���� ���)
        if (AudioManager.audioManager == null)
            AudioManager.audioManager = this;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // ���� ���۵� �� �����̴��� ��ġ�� ���� ������ �����ϰ� ���� >> (�ΰ��� -> ����ȭ�� / ����ȭ�� -> �ΰ���) �� ��ȯ �� �����̴� �ʱ���ġ ����
        // slider�� ��ġ�� �������� ����
        float valueMST, valueBGM, valueSFX;
        // ����� �ͼ��� ���� ���� value�� ������ (value�� �������� �ʴ� ��� false�� ��ȯ��)
        bool resultMST = audioMixer.GetFloat("Master", out valueMST);
        bool resultBGM = audioMixer.GetFloat("BGM", out valueBGM);
        bool resultSFX = audioMixer.GetFloat("SFX", out valueSFX);

        // slider�� ��ġ ��ȯ
        if (resultMST)
            sliderMaster.value = valueMST;
        if (resultBGM)
            sliderBGM.value = valueBGM;
        if (resultSFX)
            sliderSFX.value = valueSFX;
    }

    // �����̴��� ���� �������� �Լ���
    public void MasterControl()
    {
        float sound = sliderMaster.value;

        // ���� -40�� �ּҰ����� �������־����Ƿ� -40�� �ǹ����� �ƿ� ������(������ͼ��� �Ҹ� �ּҰ��� -80)
        if (sound == -40f) audioMixer.SetFloat("Master", -80);
        // �ƴ� ��� �׳� �����̴��� ������ sound��ŭ �Ҹ� ����
        else audioMixer.SetFloat("Master", sound);
    }

    public void BGMControl()
    {
        float sound = sliderBGM.value;

        if (sound == -40f) audioMixer.SetFloat("BGM", -80);
        else audioMixer.SetFloat("BGM", sound);
    }

    public void SFXControl()
    {
        float sound = sliderSFX.value;

        if (sound == -40f) audioMixer.SetFloat("SFX", -80);
        else audioMixer.SetFloat("SFX", sound);
    }

    public void ResetAudio()
    {
        sliderMaster.value = 0;
        sliderBGM.value = -20;
        sliderSFX.value = -20;
        audioMixer.SetFloat("Master", 0);
        audioMixer.SetFloat("BGM", -20);
        audioMixer.SetFloat("SFX", -20);
    }

    public void PlaySound(string action)
    {
        switch (action)
        {
            case "ENEMYS":
                audioSource.clip = audioClips[0];
                break;
            case "ENEMYM":
                audioSource.clip = audioClips[1];
                break;
            case "ENEMYL":
                audioSource.clip = audioClips[2];
                break;
            case "BOSS":
                audioSource.clip = audioClips[3];
                break;
            case "BOSSBULLETA":
                audioSource.clip = audioClips[4];
                break;
            case "BOSSBULLETB":
                audioSource.clip = audioClips[5];
                break;
            case "BOSSBULLETC":
                audioSource.clip = audioClips[6];
                break;
            case "BOSSBULLETD":
                audioSource.clip = audioClips[7];
                break;
            case "BOSSRAISER":
                audioSource.clip = audioClips[8];
                break;
            case "BOSSRAISER2":
                audioSource.clip = audioClips[9];
                break;
            case "PLAYER":
                audioSource.clip = audioClips[10];
                break;
            case "BOOM":
                audioSource.clip = audioClips[11];
                break;
            case "ITEM":
                audioSource.clip = audioClips[12];
                break;
        }

        // PlayOneShot�� �Ҹ� ��ø�� ����
        audioSource.PlayOneShot(audioSource.clip);
    }
}
