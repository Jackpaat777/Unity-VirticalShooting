using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBot;
    public bool isTouchRight;
    public bool isTouchLeft;

    public int life;
    public int score;
    public float speed;
    public int maxPower;
    public int power;
    public int maxBoom;
    public int boom;
    public float maxShotDelay;
    public float curShotDelay;

    public GameObject boomEffect;
    public GameManager gameManager;
    public ObjectManager objectManager;
    public bool isHit;  // �ǰ� �ߺ� ������ ���� ����
    public bool isBoomTime; // �ʻ�� �ߺ� ������ ���� ����

    public Follower[] followers;
    public bool isRespawnTime;
    public bool isStart;

    public bool isButtonB;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (isStart)
        {
            isStart = false;
            return;
        }

        // Ȱ��ȭ �Ǹ� �ٷ� Unbeatable�Լ��� �̵�
        Unbeatable();
        Invoke("Unbeatable", 2);
    }

    void Unbeatable()
    {
        // ó���� Ȱ��ȭ�� isRespawnTime�� false�̹Ƿ� true�� �ٲ���
        // ���Ŀ� 2�� �ڿ� Invoke�� ���� ����Ǹ� isRespawnTime�� false�� ����
        // isRespawnTime�� ���缭 �÷��̾��� ������ �ٲ�� ��
        isRespawnTime = !isRespawnTime;
        // ���� Ÿ�� ����Ʈ(����)
        if (isRespawnTime)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            for (int i = 0; i < followers.Length; i++)
            {
                followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
        }
        // ���� Ÿ�� ����(������� )
        else
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            for (int i = 0; i < followers.Length; i++)
            {
                followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
    }

    void Update()
    {
        Fire();
        Boom();
        Reload(); //�Ѿ� �����ӵ��� �ʹ� ������ ������(1�ʿ� 60��) �����̸� �ɾ��ִ� �Լ�
    }

    // �巡���̺�Ʈ�� �÷��̾��� �ݶ��̴��� ���еǾ� �÷��̾� �巡�� �� �������� �ʴ� ����?
    void OnMouseDrag()
    {
        // �Ͻ� ���� ��Ȳ
        if (gameManager.playerStop)
            return;
        // ��ġ �ι� �̻��� ���
        if (Input.touchCount > 1)
            return;

        Vector3 curPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

        transform.position = curPos;

        // Border
        if (transform.position.x > 2) transform.position = new Vector3(2, transform.position.y, 0);
        if (transform.position.x < -2) transform.position = new Vector3(-2, transform.position.y, 0);
        if (transform.position.y > 4.5f) transform.position = new Vector3(transform.position.x, 4.5f, 0);
        if (transform.position.y < -4.5f) transform.position = new Vector3(transform.position.x, -4.5f, 0);
    }

    // B��ư�� ���� �� ȿ���� ���� �����Ƿ� ������ ����
    public void ButtonBDown()
    {
        isButtonB = true;
    }

    public void ButtonBUp()
    {
        isButtonB = false;
    }

    void Fire()
    {
        // if������ �Լ��� ���� ����� ������ �ݴ��� ���ǿ��� return�� �ϴ� ����� ����(��Ÿ�� ����)
        //if (!Input.GetButton("Fire1"))
        //    return;

        //if (!isButtonA)
        //    return;

        // ���� �Ѿ��� �����̰� ������ �ִ� �Ѿ��� �����̸� ���� �ʾ��� ��� (�� �� ��� �� �� ���� �Ѿ��� ������������ �����̽ð��� ���� �������� ����)
        if (curShotDelay < maxShotDelay)
            return;

        // power�� ���� �Ѿ� ����
        switch (power)
        {
            case 1:
                // Instantiate : �������� ������Ʈ�� ȭ�鿡 ���� (Destroy�� �ݴ�)
                // >> ������Ʈ Ǯ������ ���� �׳� instantiate�� ��� �ٲ�
                GameObject bullet = objectManager.ActiveObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2: // ���� �Ѿ� 2�� �߻�
                // �Ѿ��� ��/��� �ΰ� ������ �ϹǷ� position�� Vector���� �߰�
                GameObject bulletR = objectManager.ActiveObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                GameObject bulletL = objectManager.ActiveObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3: // �Ѿ�B 1�� �߻�
                GameObject bulletB = objectManager.ActiveObj("BulletPlayerB");
                bulletB.transform.position = transform.position;
                bulletB.transform.localScale = Vector2.one * 1.5f;
                Rigidbody2D rigidB = bulletB.GetComponent<Rigidbody2D>();
                rigidB.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 4: // �Ѿ�A 2�� �߰� �߻�
                GameObject bulletRR = objectManager.ActiveObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.35f;
                GameObject bulletCC = objectManager.ActiveObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;
                bulletCC.transform.localScale = Vector2.one * 1.5f;
                GameObject bulletLL = objectManager.ActiveObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.35f;
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 5:
                GameObject bulletRRR = objectManager.ActiveObj("BulletPlayerA");
                bulletRRR.transform.position = transform.position + Vector3.right * 0.55f;
                GameObject bulletRRC = objectManager.ActiveObj("BulletPlayerA");
                bulletRRC.transform.position = transform.position + Vector3.right * 0.35f;
                GameObject bulletCCC = objectManager.ActiveObj("BulletPlayerB");
                bulletCCC.transform.position = transform.position;
                bulletCCC.transform.localScale = Vector2.one * 1.5f;
                GameObject bulletLLC = objectManager.ActiveObj("BulletPlayerA");
                bulletLLC.transform.position = transform.position + Vector3.left * 0.35f;
                GameObject bulletLLL = objectManager.ActiveObj("BulletPlayerA");
                bulletLLL.transform.position = transform.position + Vector3.left * 0.55f;
                Rigidbody2D rigidRRR = bulletRRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidRRC = bulletRRC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCCC = bulletCCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLLC = bulletLLC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLLL = bulletLLL.GetComponent<Rigidbody2D>();
                rigidRRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidRRC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLLC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            default: // ���� ���� �Ŀ��� ���(��, �� �������ʹ� Follower�� �����ϹǷ� Default�� ���־����)
                GameObject bulletRRRR = objectManager.ActiveObj("BulletPlayerC");
                bulletRRRR.transform.position = transform.position + Vector3.right * 0.8f;
                GameObject bulletRRRC = objectManager.ActiveObj("BulletPlayerA");
                bulletRRRC.transform.position = transform.position + Vector3.right * 0.55f;
                GameObject bulletRRCC = objectManager.ActiveObj("BulletPlayerA");
                bulletRRCC.transform.position = transform.position + Vector3.right * 0.35f;
                GameObject bulletCCCC = objectManager.ActiveObj("BulletPlayerB");
                bulletCCCC.transform.position = transform.position;
                bulletCCCC.transform.localScale = Vector2.one * 1.5f;
                GameObject bulletLLCC = objectManager.ActiveObj("BulletPlayerA");
                bulletLLCC.transform.position = transform.position + Vector3.left * 0.35f;
                GameObject bulletLLLC = objectManager.ActiveObj("BulletPlayerA");
                bulletLLLC.transform.position = transform.position + Vector3.left * 0.55f;
                GameObject bulletLLLL = objectManager.ActiveObj("BulletPlayerC");
                bulletLLLL.transform.position = transform.position + Vector3.left * 0.8f;
                Rigidbody2D rigidRRRR = bulletRRRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidRRRC = bulletRRRC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidRRCC = bulletRRCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCCCC = bulletCCCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLLCC = bulletLLCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLLLC = bulletLLLC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLLLL = bulletLLLL.GetComponent<Rigidbody2D>();
                rigidRRRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidRRRC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidRRCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCCCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLLCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLLLC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLLLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }

        // �ѹ� ��� �� �ڿ� curShotDelay���� 0���� �ʱ�ȭ
        curShotDelay = 0;
    }

    void Reload()
    {
        // �����̺����� ��ŸŸ���� ��� ���Ͽ� ���
        curShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        //if (!Input.GetButton("Fire2"))
        //    return;

        if (!isButtonB)
            return;

        if (isBoomTime)
            return;

        if (boom == 0)
            return;

        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);   // boom ��� �� UI�� boom ������ ����

        // 1. Boom Effect
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 1);
        // 2. Remove Enemy
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");

        for (int i = 0; i < enemiesL.Length; i++)
        {
            if (enemiesL[i].activeSelf) // Ȱ��ȭ�� ������Ʈ�� ����
            {
                Enemy enemyLogic = enemiesL[i].GetComponent<Enemy>();
                enemyLogic.OnHit(1000); // ������ ���� EnemyŬ���� ���� �ǰ��Լ��� �ٽ� Ȱ��
            }
        }
        for (int i = 0; i < enemiesM.Length; i++)
        {
            if (enemiesM[i].activeSelf)
            {
                Enemy enemyLogic = enemiesM[i].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int i = 0; i < enemiesS.Length; i++)
        {
            if (enemiesS[i].activeSelf)
            {
                Enemy enemyLogic = enemiesS[i].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        // 3. Remove Enemy Bullet
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");
        GameObject[] bulletsC = objectManager.GetPool("BulletBossA");
        GameObject[] bulletsD = objectManager.GetPool("BulletBossB");
        GameObject[] bulletsS = objectManager.GetPool("BulletBossC");

        for (int i = 0; i < bulletsA.Length; i++)
        {
            if (bulletsA[i].activeSelf)
                bulletsA[i].SetActive(false);
        }
        for (int i = 0; i < bulletsB.Length; i++)
        {
            if (bulletsB[i].activeSelf)
                bulletsB[i].SetActive(false);
        }
        for (int i = 0; i < bulletsC.Length; i++)
        {
            if (bulletsC[i].activeSelf)
                bulletsC[i].SetActive(false);
        }
        for (int i = 0; i < bulletsD.Length; i++)
        {
            if (bulletsD[i].activeSelf)
                bulletsD[i].SetActive(false);
        }
        for (int i = 0; i < bulletsS.Length; i++)
        {
            if (bulletsS[i].activeSelf)
                bulletsS[i].SetActive(false);
        }
        gameManager.UpdateBoomIcon(boom);

        // Player Boom Audio
        AudioManager.audioManager.PlaySound("BOOM");
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }

    // ������ �浹�� �ƴ� ���� Trigger�ν� ���(������ ��ũ��Ʈ�� ��ġ�� ������ ���̹Ƿ�)
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBot = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet" || collision.gameObject.tag == "Raiser" || collision.gameObject.tag == "Boss")
        {
            // ���� �ð� ��
            if (isRespawnTime)
                return;
            // �ι� �±� ����
            if (isHit)
                return;

            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position, "P");

            // Player Destroy Audio
            AudioManager.audioManager.PlaySound("PLAYER");

            if (life == 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }

            // �ǰ� ���ϸ� player�� ��� ��Ȱ��ȭ
            gameObject.SetActive(false);

            // �� �Ǵ� �� �Ѿ˰� ����� ���
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
            {
                collision.gameObject.SetActive(false);              // �÷��̾�� �΋H���� ���� �ı�
                collision.transform.rotation = Quaternion.identity; // �÷��̾�� �΋H���� ��Ȱ��ȭ�� ������ ���⵵ ���󺹱�
            }
        }
        else if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":
                    score += 1000;
                    break;

                case "Power":
                    if (power == maxPower)
                        score += 500;
                    else
                    {
                        power++;
                        AddFollower(); 
                    }
                    break;

                case "Boom":
                    if (boom < maxBoom)
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);   // boom�߰� �� UI�� boom ������ �߰�
                    }
                    else
                        score += 500;
                    break;
            }

            // Item Audio
            AudioManager.audioManager.PlaySound("ITEM");

            collision.gameObject.SetActive(false);
        }
    }

    void AddFollower()
    {
        if (power == 7)
            followers[0].gameObject.SetActive(true);
        else if (power == 8)
            followers[1].gameObject.SetActive(true);
        else if (power == 9)
            followers[2].gameObject.SetActive(true);
    }

    // Trigger�� ��������� bool���� true, ���� ���� ��쿡�� bool���� false
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBot = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}
