using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void GameLoad()
    {
        // �Ͻ������� ������ ���ӿ����� ������ TimeScale�� �ٽ� �ǵ�����
        Time.timeScale = 1;
        SceneManager.LoadScene("InGameScene");
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
