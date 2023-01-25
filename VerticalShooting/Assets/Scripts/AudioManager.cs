using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClips;

    // 슬라이더를 통한 볼륨조절 변수
    public AudioMixer audioMixer;
    public Slider sliderMaster;
    public Slider sliderBGM;
    public Slider sliderSFX;

    // 다른 스크립트에서도 AudioManager에 바로 접근할 수 있도록 static으로 만들기
    public static AudioManager audioManager;

    AudioSource audioSource;

    void Awake()
    {
        // AudioManager.audioManager가 없을 경우 이 스크립트를 넣어주기 (다른 스크립트에서 사용)
        if (AudioManager.audioManager == null)
            AudioManager.audioManager = this;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // 씬이 시작될 때 슬라이더의 위치는 현재 볼륨과 동일하게 설정 >> (인게임 -> 메인화면 / 메인화면 -> 인게임) 씬 전환 시 슬라이더 초기위치 조정
        // slider의 위치를 지정해줄 변수
        float valueMST, valueBGM, valueSFX;
        // 오디오 믹서의 현재 값을 value에 저장함 (value가 존재하지 않는 경우 false를 반환함)
        bool resultMST = audioMixer.GetFloat("Master", out valueMST);
        bool resultBGM = audioMixer.GetFloat("BGM", out valueBGM);
        bool resultSFX = audioMixer.GetFloat("SFX", out valueSFX);

        // slider의 위치 변환
        if (resultMST)
            sliderMaster.value = valueMST;
        if (resultBGM)
            sliderBGM.value = valueBGM;
        if (resultSFX)
            sliderSFX.value = valueSFX;
    }

    // 슬라이더를 통한 볼륨조절 함수들
    public void MasterControl()
    {
        float sound = sliderMaster.value;

        // 현재 -40을 최소값으로 설정해주었으므로 -40이 되버리면 아예 꺼버림(오디오믹서의 소리 최소값은 -80)
        if (sound == -40f) audioMixer.SetFloat("Master", -80);
        // 아닐 경우 그냥 슬라이더의 설정된 sound만큼 소리 설정
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

        // PlayOneShot은 소리 중첩이 가능
        audioSource.PlayOneShot(audioSource.clip);
    }
}
