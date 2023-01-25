using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void GameLoad()
    {
        // 일시정지로 나갔던 게임오버로 나갔던 TimeScale을 다시 되돌려줌
        Time.timeScale = 1;
        SceneManager.LoadScene("InGameScene");
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
