using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;

    // player�� objectManager�� �������� �ƴϱ� ������ ���� ���� �־��� �� ����
    // >> GameObject���� SpawnEnemy�� �� �� enemyLogic�� ���� ���� �Ѱ���
    public GameObject player;
    public GameManager gameManager;
    public ObjectManager objectManager;

    // BossB ���� �Լ� (FireBall & FireTornado)
    public int[] dirFire;
    public int dirFireIndex;
    public float roundNum;
    public bool dirChange;

    Animator anim;

    public int patternIndex;    // ������ �ε���
    public int curPatternCount; // ������ ��� ����ߴ���
    public int[] maxPatternCount; // �ش� ������ ������� ����� ������
    GameObject raiser;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // ������Ʈ�� Ȱ��ȭ�� �� ȣ��Ǵ� �����ֱ��Լ�
    void OnEnable()
    {
        patternIndex = -1;
        switch (enemyName)
        {
            case "B":
                health = 2500;
                Invoke("Stop", 2f);
                break;
            case "B2":
                health = 3000;
                Invoke("Stop", 2f);
                break;
            case "B3":
                health = 3500;
                Invoke("Stop", 2f);
                break;
        }
    }

    // ���� ���� �Լ�
    void Stop()
    {
        // ó�� ������ ��� �ÿ� Stop�Լ��� 2�� ����Ǳ� ������ ������ �ɾ���(ó������ �������� ��Ȱ��ȭ �ص� ��)
        if (!gameObject.activeSelf)
            return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        // ���� �� ���������� ���� �Լ� ����
        Invoke("BossPattern", 3);
    }

    // ���� ��������  �Լ�
    void BossAPattern()
    {
        // ���� patternIndex�� �ִ��� ��� ���� ������ 0���� �ٲ��� (�ƴ� ��� �׳� +1)
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;

        // curPatternCount�� Think���� �ʱ�ȭ
        curPatternCount = 0;

        switch (patternIndex)
        {
            case 0:
                FireForward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    void BossBPattern()
    {
        patternIndex = patternIndex == 4 ? 0 : patternIndex + 1;
        curPatternCount = 0;

        switch (patternIndex)
        {
            case 0:
                FireFollow();
                break;
            case 1:
                FireBall();
                break; ;
            case 2:
                FireTornado();
                break;
            case 3:
                FireRaiser();
                break;
            case 4:
                FireAround();
                break;
        }
    }

    void BossPattern()
    {
        if (enemyName == "B" || enemyName == "B2")
            BossAPattern();
        else if (enemyName == "B3")
            BossBPattern();
    }

    // <���� ���� ���� �Լ���>
    // ���� ���Ŀ��� �ٽ� �����Լ��� �ݺ�
    // Boss A/B ����
    void FireForward()
    {
        if (health <= 0)
            return;

        float rePattern = 0;
        int bulletSpeed = 0;
        // Fire Bullet Forward
        if (enemyName == "B")
        {
            bulletSpeed = 5;
            rePattern = 2f;
        }
        else if (enemyName == "B2" || enemyName == "B3")
        {
            bulletSpeed = 8;
            rePattern = 1f;
        }

        GameObject bulletRR = objectManager.ActiveObj("BulletBossA");
        GameObject bulletR = objectManager.ActiveObj("BulletBossA");
        GameObject bulletL = objectManager.ActiveObj("BulletBossA");
        GameObject bulletLL = objectManager.ActiveObj("BulletBossA");
        bulletRR.transform.position = transform.position + Vector3.right * 0.5f;
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
        bulletLL.transform.position = transform.position + Vector3.left * 0.5f;
        bulletRR.transform.localScale = new Vector2(1.5f, 2f);
        bulletR.transform.localScale = new Vector2(1.5f, 2f);
        bulletL.transform.localScale = new Vector2(1.5f, 2f);
        bulletLL.transform.localScale = new Vector2(1.5f, 2f);
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
        rigidRR.AddForce(Vector2.down * bulletSpeed, ForceMode2D.Impulse);
        rigidR.AddForce(Vector2.down * bulletSpeed, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * bulletSpeed, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * bulletSpeed, ForceMode2D.Impulse);

        // Bullet Audio
        AudioManager.audioManager.PlaySound("BOSSBULLETA");

        // Pattern Counting
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireForward", rePattern);  // ���� ���� ī��Ʈ�� max���� ���� ��� ������ ���� �ٽ� ����
        else
            Invoke("BossPattern", 3);
    }

    void FireShot()
    {
        if (health <= 0)
            return;

        float rePattern = 0;
        int bulletNum = 0;
        // Random Shotgun Bullet to Player
        if (enemyName == "B")
        {
            bulletNum = 5;
            rePattern = 1.5f;
        }
        else if (enemyName == "B2" || enemyName == "B3")
        {
            bulletNum = 7;
            rePattern = 0.9f;
        }

        for (int i = 0; i < bulletNum; i++)
        {
            GameObject bullet = objectManager.ActiveObj("BulletEnemyB");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position - transform.position;
            // �ణ�� �������� ���� �Ѿ˸��� �ٸ��� ��� ����
            Vector2 ranVec = new Vector2(Random.Range(-0.7f, 0.7f), Random.Range(0, 2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }

        // Bullet Audio
        AudioManager.audioManager.PlaySound("BOSSBULLETB");

        // Pattern Counting
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireShot", rePattern);
        else
            Invoke("BossPattern", 3);
    }

    void FireArc()
    {
        if (health <= 0)
            return;

        float rePattern = 0;
        if (enemyName == "B") rePattern = 0.15f;
        else if (enemyName == "B2" || enemyName == "B3") rePattern = 0.1f;

        // Fire Arc Continue Fire
        GameObject bullet = objectManager.ActiveObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity; // ���ư��� ź���̹Ƿ� ȸ������ �ʱ�ȭ

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        // Sin�Լ�/Cos�Լ��� ���� curPattern���� �����Ҽ��� �ﰢ�Լ�ó�� ������ ���Ҹ� �ݺ�
        // Mathf.Pi �� ���� �ѹ��� ������ �������� �ٽ� ������ �ڸ��� ���ƿ��� ��(curPatternCount/maxPatternCount[patternIndex]���� �׻� 0~1�̹Ƿ�)
        // Pi�� �־��־�� ���� �ε巯�� ��ä�� ����� ������ �������
        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 7 * curPatternCount / maxPatternCount[patternIndex]), -1);
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);

        // Bullet Audio
        AudioManager.audioManager.PlaySound("BOSSBULLETC");

        // Pattern Counting
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireArc", rePattern);
        else
            Invoke("BossPattern", 3);
    }

    void FireAround()
    {
        if (health <= 0)
            return;

        // �Ѿ� ������ �޸��ؼ� ������ Ʋ����
        int roundNumA = 34;
        int roundNumB = 32;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;

        for (int i = 0; i < roundNum; i++)
        {
            GameObject bullet = objectManager.ActiveObj("BulletBossB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            // i / roundNumA�� ���� 0~1���� ���� ������ �ص�, PI*2�� ���� ���� �ѷ��� �ǵ��� ������
            // x��� y�� ��� �ﰢ�Լ��� ������Ѽ� ���� ������� �������
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / roundNum),
                                         Mathf.Sin(Mathf.PI * 2 * i / roundNum));
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            // �Ѿ� ��������Ʈ�� ȸ�� �����ϱ�
            // Vector3.forward * 90�� ���������� �����־����
            Vector3 roVec = Vector3.forward * 360 * i / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(roVec);
        }

        // Bullet Audio
        AudioManager.audioManager.PlaySound("BOSSBULLETD");

        // Pattern Counting
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireAround", 1);
        else
            Invoke("BossPattern", 4);
    }

    // Boss C ���� ����
    void FireFollow()
    {
        if (health <= 0)
            return;

        GameObject bullet = objectManager.ActiveObj("BulletEnemyA");
        bullet.transform.position = transform.position + Vector3.down;
        bullet.transform.rotation = Quaternion.identity;
        bullet.transform.localScale = Vector3.one * 2f;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = player.transform.position - transform.position;
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);


        // Bullet Audio
        AudioManager.audioManager.PlaySound("BOSSBULLETB");

        // Pattern Counting
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireFollow", 0.5f);
        else
            Invoke("BossPattern", 2);
    }

    void FireBall()
    {
        if (health <= 0)
            return;

        // Random Shotgun Bullet to Player
        GameObject bullet = objectManager.ActiveObj("BulletBossC");
        bullet.transform.position = transform.position;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = new Vector2(dirFire[dirFireIndex], -3);
        rigid.AddForce(dirVec.normalized * 4, ForceMode2D.Impulse);

        dirFireIndex = dirFireIndex == 3 ? 0 : dirFireIndex + 1;

        // Bullet Audio
        AudioManager.audioManager.PlaySound("BOSSBULLETA");

        // Pattern Counting
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireBall", 0.5f);
        else
            Invoke("BossPattern", 2);
    }

    void FireTornado()
    {
        if (health <= 0)
            return;

        // roundNum�� 0.2�� ������ ���Ҹ� �ݺ�
        if (dirChange)
        {
            roundNum = roundNum + 0.2f;
            if (roundNum >= 11.8)
                dirChange = false;
        }
        else
        {
            roundNum = roundNum - 0.2f;
            if (roundNum <= 11)
                dirChange = true;
        }

        // ���� ��ä�� ������� �����̵���
        // roundNum�� �����̸����� ź�˻����̹Ƿ� �׻� ���� ������ ź�� ���� (roundNum�� float)
        for (int i = 0; i < roundNum; i++)
        {
            GameObject bullet = objectManager.ActiveObj("BulletEnemyB");
            bullet.transform.position = transform.position + Vector3.up * 0.3f;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            // Pi�� 2�谡 �ƴϹǷ� ���� ���ݸ������ ź�� ����
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * i / roundNum),
                                         Mathf.Sin(Mathf.PI * i / roundNum) * (-1));
            rigid.AddForce(dirVec.normalized * 2.5f, ForceMode2D.Impulse);
        }

        // Bullet Audio
        AudioManager.audioManager.PlaySound("BOSSBULLETC");

        // Pattern Counting
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireTornado", 0.2f);
        else
            Invoke("BossPattern", 2);
    }

    void FireRaiser()
    {
        if (health <= 0)
            return;

        raiser = objectManager.ActiveObj("BulletRaiser");

        // Raiser Audio
        AudioManager.audioManager.PlaySound("BOSSRAISER");
        Invoke("NextRaiserSound", 3);

        Invoke("EndRaiser", 8);
    }
    void EndRaiser()
    {
        raiser.SetActive(false);

        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireRaiser", 1);
        else
            Invoke("BossPattern", 1);
    }
    void NextRaiserSound()
    {
        AudioManager.audioManager.PlaySound("BOSSRAISER2");
    }

    public void OnHit(int dmg)
    {
        if (health <= 0)
            return;

        health -= dmg;

        anim.SetTrigger("OnHit");

        if (health <= 0)
        {
            // Boss Destroy Audio
            AudioManager.audioManager.PlaySound("BOSS");

            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            gameManager.CallExplosion(transform.position, "B");

            // ������ ���� ����
            CancelInvoke();

            // Stage Clear
            gameManager.StageEnd();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            // Bullet�� �ִ� dmg �������� �ҷ����� ���� ����
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            // �ǰ� �� �Ѿ� ������Ʈ ��Ȱ��ȭ
            collision.gameObject.SetActive(false);
        }
    }

}
