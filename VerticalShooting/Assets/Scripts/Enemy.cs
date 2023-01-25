using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;
    public Sprite[] sprites;

    public float maxShotDelay;
    public float curShotDelay;
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;
    // player�� objectManager�� �������� �ƴϱ� ������ ���� ���� �־��� �� ����
    // >> GameObject���� SpawnEnemy�� �� �� enemyLogic�� ���� ���� �Ѱ���
    public GameObject player;
    public GameManager gameManager;
    public ObjectManager objectManager;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // ������Ʈ�� Ȱ��ȭ�� �� ȣ��Ǵ� �����ֱ��Լ�
    void OnEnable()
    {
        switch (enemyName)
        {
            case "L":
                health = 50;
                break;
            case "M":
                health = 15;
                break;
            case "S":
                health = 3;
                break;
        }
    }

    // ==================�Ϲ� �� ���� �Լ�(���� ����)=====================
    void Update()
    {
        Fire();
        Reload();
    }

    void Fire()
    {
        // ���� �Ѿ��� �����̰� ������ �ִ� �Ѿ��� �����̸� ���� �ʾ��� ��� (�� �� ��� �� �� ���� �Ѿ��� ������������ �����̽ð��� ���� �������� ����)
        if (curShotDelay < maxShotDelay)
            return;

        // M�� �Ѿ��� ���� �ʰ� �΋H���� �͸�
        if (enemyName == "S")
        {
            GameObject bullet = objectManager.ActiveObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            // �÷��̾��� ��ġ���� ���� ��ġ�� ���� ������ ���� ���� (�� �� ������ ���Ͱ�)
            Vector3 dirVec = player.transform.position - transform.position;
            // dirVec�� �������Ͱ� �ƴ� �Ϲݺ����̹Ƿ� �������ͷ� ������� �� speed�� ���Ͽ� ����ϱ� ���ؼ��� normalized ���ֱ�
            rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);
        }
        else if (enemyName == "L")
        {
            GameObject bulletR = objectManager.ActiveObj("BulletEnemyB");
            GameObject bulletL = objectManager.ActiveObj("BulletEnemyB");
            bulletR.transform.position = transform.position;
            bulletL.transform.position = transform.position;
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
            // �� ��ġ�� �¿찪�� �����̹Ƿ� ��ȣ�� �����ֱ�
            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);
            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
        }

        // �ѹ� ��� �� �ڿ� curShotDelay���� 0���� �ʱ�ȭ
        curShotDelay = 0;
    }

    void Reload()
    {
        // �����̺����� ��ŸŸ���� ��� ���Ͽ� ���
        curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)
    {
        // �ι��� ���ÿ� �¾� �������� 2�� ����ϴ� ��츦 ����
        if (health <= 0)
            return;

        health -= dmg;

        // �ǰݴ��� ��� ��������Ʈ ����
        spriteRenderer.sprite = sprites[1];
        // �ٽ� ���󺹱��� ��쿡�� ���� �ξ ��������Ʈ ����
        Invoke("ReturnSprite", 0.1f);

        if (health <= 0)
        {
            // player�� �ٷ� ������� �ʰ� ���� ������ �����Ͽ� ȣ���ϴ� ����?
            // player�� �׳� GameObject���̹Ƿ� PlayerŬ���� ���� ������ ����� �� ����
            // ��� player���� �ȿ� PlayerŬ������ �� ������Ʈ�� �����Ƿ� �̸� ���� GetComponent�� ����
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            // Random Ratio Item Drop
            ItemDropRatio(enemyName);

            // Enemy Destroy Audio
            if (enemyName == "S")
                AudioManager.audioManager.PlaySound("ENEMYS");
            else if (enemyName == "M")
                AudioManager.audioManager.PlaySound("ENEMYM");
            else if (enemyName == "L")
                AudioManager.audioManager.PlaySound("ENEMYL");

            // ������Ʈ Ǯ�� ����ϰ� �����Ƿ� Destroy�� �ƴ� ��Ȱ��ȭ�� �����ֱ�
            // ������Ʈ�� �ٽ� ���� �� rotate�� ���·� ������ ���� �����ϱ� ���� ������ ���Ŀ��� ���� ����
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            gameManager.CallExplosion(transform.position, enemyName);   // enemyName�� �Ѱ��ָ� �ٷ� explosion�� Ÿ���� ��������

        }
    }

    // ������ ��� Ȯ��
    void ItemDropRatio(string name)
    {
        int noItem = 0;
        int coin = 0;
        int power = 0;
        int boom = 0;

        // Ȯ�� �й�
        // 5,3,1,1  3,3,2,2   1,3,3,3
        if (name == "S")
        {
            noItem = 5;
            coin = 8;
            power = 9;
            boom = 10;
            Debug.Log("S");
        }
        else if (name == "M")
        {
            noItem = 3;
            coin = 6;
            power = 8;
            boom = 10;
            Debug.Log("M");
        }
        else if (name == "L")
        {
            noItem = 1;
            coin = 4;
            power = 7;
            boom = 10;
            Debug.Log("L");
        }


        // Ȯ�� ����
        int ran = Random.Range(0, 10);
        if (ran < noItem)        // Not Item
        {
            Debug.Log("<No Item>");
        }
        else if (ran < coin)     // Coin
        {
            Debug.Log("<Coin>");
            GameObject itemCoin = objectManager.ActiveObj("ItemCoin");
            itemCoin.transform.position = transform.position;
        }
        else if (ran < power)   // Power
        {
            Debug.Log("<Power>");
            GameObject itemPower = objectManager.ActiveObj("ItemPower");
            itemPower.transform.position = transform.position;
        }
        else if (ran < boom)    // Boom
        {
            Debug.Log("<Boom>");
            GameObject itemBoom = objectManager.ActiveObj("ItemBoom");
            itemBoom.transform.position = transform.position;
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet")
        {
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            // Bullet�� �ִ� dmg �������� �ҷ����� ���� ����
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            // �ǰ� �� �Ѿ� ������Ʈ ��Ȱ��ȭ
            collision.gameObject.SetActive(false);
        }
    }

}
