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
        // 카메라의 사이즈마다 배경이 안보이는 경우가 달라지므로 이를 변수값에 직접 가져와서 넣어줌
        // 2를 곱해주어야 실제 카메라에 보이는 높이를 알 수 있음
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
            // position은 글로벌 기준이어서 localPosition을 사용해 위치를 옮겨줌
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * 10;

            // Cursor Index Change
            // 2(s) 1 0(e) 순서대로 움직이다가 0(s) 2 1(e) 순서가 되어버리면 
            int startIndexSave = startIndex;
            startIndex = endIndex;
            // 후위 연산자는 해당 라인 다음에 연산이 적용되므로 반드시 전위 연산자 사용
            // 시작인덱스가 -1이 되면 endIndex값에 2를 넣어주기
            endIndex = startIndexSave - 1 == -1 ? sprites.Length - 1 : startIndexSave - 1;
        }
    }
}
