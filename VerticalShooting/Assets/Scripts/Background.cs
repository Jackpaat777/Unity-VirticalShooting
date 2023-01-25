using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed;
    public int startIndex;
    public int endIndex;
    public GameObject[] sprites;

    float viewHeight;

    void Awake()
    {
        // ī�޶��� ������� ����� �Ⱥ��̴� ��찡 �޶����Ƿ� �̸� �������� ���� �����ͼ� �־���
        // 2�� �����־�� ���� ī�޶� ���̴� ���̸� �� �� ����
        viewHeight = Camera.main.orthographicSize * 2;
    }

    void Update()
    {
        Move();
        Scrolling();
    }

    void Move()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }

    void Scrolling()
    {
        if (sprites[endIndex].transform.position.y < viewHeight * (-1))
        {
            // Sprites ReUse
            Vector3 backSpritePos = sprites[startIndex].transform.localPosition;
            Vector3 frontSpritePos = sprites[endIndex].transform.localPosition;
            // position�� �۷ι� �����̾ localPosition�� ����� ��ġ�� �Ű���
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * 10;

            // Cursor Index Change
            // 2(s) 1 0(e) ������� �����̴ٰ� 0(s) 2 1(e) ������ �Ǿ������ 
            int startIndexSave = startIndex;
            startIndex = endIndex;
            // ���� �����ڴ� �ش� ���� ������ ������ ����ǹǷ� �ݵ�� ���� ������ ���
            // �����ε����� -1�� �Ǹ� endIndex���� 2�� �־��ֱ�
            endIndex = startIndexSave - 1 == -1 ? sprites.Length - 1 : startIndexSave - 1;
        }
    }
}
