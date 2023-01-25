using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackMusic : MonoBehaviour
{
    public string[] nameBGM;
    public AudioClip[] audioClip;
    public string nowBGM;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBGM(string name)
    {
        // 변경할 BGM이 현재와 같다면 함수실행 안함
        if (nowBGM.Equals(name)) return;

        for (int i = 0; i < nameBGM.Length; i++)
        {
            // BGM 배열중에서 변경할 BGM과 같은 이름을 찾았다면
            if (nameBGM[i].Equals(name))
            {
                // 변경할 BGM 재생
                audioSource.clip = audioClip[i];
                audioSource.Play();
                // nowBGM 갱신
                nowBGM = name;
            }
        }
    }
}
