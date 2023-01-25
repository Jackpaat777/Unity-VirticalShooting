using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Follower�� �ڵ�� Player�� �����
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
        // Queue�� ���� �θ������Ʈ�� �������� ������
        parentPos = new Queue<Vector3>();
    }

    void Update()
    {
        UpdateFollowPos();
        Follow();
        Fire();
        Reload();
    }

    // ���� ��ġ ����
    void UpdateFollowPos()
    {
        // Input Position
        // �θ��� ��ġ�� ������ ��쿡�� parent.position�� ������Ʈ ���� ����
        // >> Player�� �������� ���� ��쿡�� Follower�� ������ ����
        if (!parentPos.Contains(parent.position))
            parentPos.Enqueue(parent.position);

        // Output Position
        // followDelay�� 2�� ��, Player�� 2������ �����̸�(���ߴ� ���� Enqueue���� �ʱ⶧���� ť�� ����ȵ�) �� �� Follower���� Player�� ó�� ��ġ���� ��ȯ���� 
        // >> ��, followDelay��ŭ�� �����̸� �ɾ��ְڴ�
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay) // ť�� ���� ä������ ���� ��� Player�� ��ġ���� �־���
            followPos = parent.position;
    }

    // Follower�� ��ü������ �������� �ʰ� Player�� ����
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
