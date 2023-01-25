using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragEvent : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject player;
    Vector3? prevPos;
    
    // 이동한 벡터값만큼 더하는 것을 통한 구현
    void OnMouseDown()
    {
        prevPos = null;
    }

    void OnMouseDrag()
    {
        // 일시 정지 상황
        if (gameManager.playerStop)
            return;

        // 터치 두번 이상일 경우
        if (Input.touchCount > 1)
            return;

        // curPos Update
        Vector3 curPos = GetTouchPos();

        // Player Transform Update
        if (prevPos.HasValue)
        {
            player.transform.position += curPos - prevPos.Value;
        }

        // Border
        if (player.transform.position.x > 2) player.transform.position = new Vector3(2, player.transform.position.y, 0);
        if (player.transform.position.x < -2) player.transform.position = new Vector3(-2, player.transform.position.y, 0);
        if (player.transform.position.y > 4.5f) player.transform.position = new Vector3(player.transform.position.x, 4.5f, 0);
        if (player.transform.position.y < -4.5f) player.transform.position = new Vector3(player.transform.position.x, -4.5f, 0);

        // prevPos Update
        prevPos = curPos;
    }

    Vector3 GetTouchPos()
    {
        // 현재 터치 위치
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
    }
}
