using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Follower의 코드는 Player와 비슷함
public class Follower : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;
    public ObjectManager objectManager;

    public float followDelay;
    public Vector3 followPos;
    public Transform parent;
    public Queue<Vector3> parentPos;

    void Awake()
    {
        // Queue를 통해 부모오브젝트의 포지션을 저장함
        parentPos = new Queue<Vector3>();
    }

    void Update()
    {
        UpdateFollowPos();
        Follow();
        Fire();
        Reload();
    }

    // 따라갈 위치 갱신
    void UpdateFollowPos()
    {
        // Input Position
        // 부모의 위치가 동일한 경우에는 parent.position을 업데이트 하지 않음
        // >> Player가 움직이지 않은 경우에는 Follower도 따라가지 않음
        if (!parentPos.Contains(parent.position))
            parentPos.Enqueue(parent.position);

        // Output Position
        // followDelay가 2일 때, Player가 2프레임 움직이면(멈추는 경우는 Enqueue하지 않기때문에 큐에 저장안됨) 그 때 Follower에게 Player의 처음 위치값을 반환해줌 
        // >> 즉, followDelay만큼의 딜레이를 걸어주겠다
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay) // 큐가 아직 채워지지 않은 경우 Player의 위치값을 넣어줌
            followPos = parent.position;
    }

    // Follower는 자체적으로 움직이지 않고 Player를 따라감
    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        //if (!Input.GetButton("Fire1"))
        //    return;

        if (curShotDelay < maxShotDelay)
            return;

        GameObject bullet = objectManager.ActiveObj("BulletFollower");
        bullet.transform.position = transform.position;
        bullet.transform.localScale = Vector2.one * 1.5f;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    public void ResetQueue()
    {
        parentPos.Clear();
        parentPos.Enqueue(parent.position);
    }
}
