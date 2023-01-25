using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;

    // player와 objectManager는 프리펩이 아니기 때문에 직접 값을 넣어줄 수 없음
    // >> GameObject에서 SpawnEnemy를 할 때 enemyLogic을 통해 값을 넘겨줌
    public GameObject player;
    public GameManager gameManager;
    public ObjectManager objectManager;

    // BossB 관련 함수 (FireBall & FireTornado)
    public int[] dirFire;
    public int dirFireIndex;
    public float roundNum;
    public bool dirChange;

    Animator anim;

    public int patternIndex;    // 패턴의 인덱스
    public int curPatternCount; // 패턴을 몇번 사용했는지
    public int[] maxPatternCount; // 해당 패턴을 몇번까지 사용할 것인지
    GameObject raiser;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // 컴포넌트가 활성화될 때 호출되는 생명주기함수
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

    // 보스 전용 함수
    void Stop()
    {
        // 처음 생성과 사용 시에 Stop함수가 2번 적용되기 때문에 조건을 걸어줌(처음부터 프리펩을 비활성화 해도 됨)
        if (!gameObject.activeSelf)
            return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        // 멈춘 뒤 패턴적용을 위한 함수 실행
        Invoke("BossPattern", 3);
    }

    // 보스 공격패턴  함수
    void BossAPattern()
    {
        // 지금 patternIndex가 최대인 경우 다음 패턴은 0으로 바꿔줌 (아닌 경우 그냥 +1)
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;

        // curPatternCount는 Think에서 초기화
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

    // <보스 공격 패턴 함수들>
    // 패턴 이후에는 다시 패턴함수로 반복
    // Boss A/B 패턴
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
            Invoke("FireForward", rePattern);  // 아직 패턴 카운트가 max보다 작을 경우 동일한 패턴 다시 실행
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
            // 약간의 랜덤값을 통해 총알마다 다르게 쏘도록 해줌
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
        bullet.transform.rotation = Quaternion.identity; // 돌아가는 탄알이므로 회전값도 초기화

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        // Sin함수/Cos함수를 통해 curPattern값이 증가할수록 삼각함수처럼 증가와 감소를 반복
        // Mathf.Pi 를 통해 한번의 패턴이 끝날때면 다시 원래의 자리로 돌아오게 됨(curPatternCount/maxPatternCount[patternIndex]값은 항상 0~1이므로)
        // Pi를 넣어주어야 더욱 부드러운 부채꼴 모양의 패턴이 만들어짐
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

        // 총알 개수를 달리해서 각도를 틀어줌
        int roundNumA = 34;
        int roundNumB = 32;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;

        for (int i = 0; i < roundNum; i++)
        {
            GameObject bullet = objectManager.ActiveObj("BulletBossB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            // i / roundNumA를 통해 0~1값을 각각 지정해 준뒤, PI*2를 통해 원의 둘레가 되도록 곱해줌
            // x축과 y축 모두 삼각함수를 적용시켜서 원의 모양으로 만들어줌
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / roundNum),
                                         Mathf.Sin(Mathf.PI * 2 * i / roundNum));
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            // 총알 스프라이트의 회전 적용하기
            // Vector3.forward * 90는 보정값으로 더해주어야함
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

    // Boss C 전용 패턴
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

        // roundNum은 0.2씩 증가와 감소를 반복
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

        // 절반 부채꼴 모양으로 움직이도록
        // roundNum의 개수미만으로 탄알생성이므로 항상 같은 개수의 탄알 생성 (roundNum은 float)
        for (int i = 0; i < roundNum; i++)
        {
            GameObject bullet = objectManager.ActiveObj("BulletEnemyB");
            bullet.transform.position = transform.position + Vector3.up * 0.3f;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            // Pi의 2배가 아니므로 원의 절반모양으로 탄막 생성
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

            // 보스의 패턴 정지
            CancelInvoke();

            // Stage Clear
            gameManager.StageEnd();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            // Bullet에 있는 dmg 변수값을 불러오기 위해 선언
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            // 피격 시 총알 오브젝트 비활성화
            collision.gameObject.SetActive(false);
        }
    }

}
