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
        // ������ BGM�� ����� ���ٸ� �Լ����� ����
        if (nowBGM.Equals(name)) return;

        for (int i = 0; i < nameBGM.Length; i++)
        {
            // BGM �迭�߿��� ������ BGM�� ���� �̸��� ã�Ҵٸ�
            if (nameBGM[i].Equals(name))
            {
                // ������ BGM ���
                audioSource.clip = audioClip[i];
                audioSource.Play();
                // nowBGM ����
                nowBGM = name;
            }
        }
    }
}
