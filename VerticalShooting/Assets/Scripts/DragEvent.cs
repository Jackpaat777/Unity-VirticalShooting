using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragEvent : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject player;
    Vector3? prevPos;
    
    // �̵��� ���Ͱ���ŭ ���ϴ� ���� ���� ����
    void OnMouseDown()
    {
        prevPos = null;
    }

    void OnMouseDrag()
    {
        // �Ͻ� ���� ��Ȳ
        if (gameManager.playerStop)
            return;

        // ��ġ �ι� �̻��� ���
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
        // ���� ��ġ ��ġ
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
    }
}
