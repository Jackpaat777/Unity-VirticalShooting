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
    public bool isHit;  // 피격 중복 방지를 위한 변수
    public bool isBoomTime; // 필살기 중복 방지를 위한 변수

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

        // 활성화 되면 바로 Unbeatable함수로 이동
        Unbeatable();
        Invoke("Unbeatable", 2);
    }

    void Unbeatable()
    {
        // 처음에 활성화시 isRespawnTime이 false이므로 true로 바꿔줌
        // 이후에 2초 뒤에 Invoke를 통해 실행되면 isRespawnTime을 false로 변경
        // isRespawnTime에 맞춰서 플레이어의 색상이 바뀌게 됨
        isRespawnTime = !isRespawnTime;
        // 무적 타임 이펙트(투명)
        if (isRespawnTime)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            for (int i = 0; i < followers.Length; i++)
            {
                followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
        }
        // 무적 타임 종료(원래대로 )
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
        Reload(); //총알 생성속도가 너무 빠르기 때문에(1초에 60개) 딜레이를 걸어주는 함수
    }

    // 드래그이벤트와 플레이어의 콜라이더가 구분되어 플레이어 드래그 시 움직이지 않는 버그?
    void OnMouseDrag()
    {
        // 일시 정지 상황
        if (gameManager.playerStop)
            return;
        // 터치 두번 이상일 경우
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

    // B버튼은 뗐을 때 효과가 따로 없으므로 만들지 않음
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
        // if문으로 함수를 묶는 방법도 있지만 반대의 조건에서 return을 하는 방법도 있음(스타일 차이)
        //if (!Input.GetButton("Fire1"))
        //    return;

        //if (!isButtonA)
        //    return;

        // 현재 총알의 딜레이가 설정된 최대 총알의 딜레이를 넘지 않았을 경우 (한 발 쏘고 난 뒤 다음 총알이 나갈때까지의 딜레이시간이 아직 충족되지 않음)
        if (curShotDelay < maxShotDelay)
            return;

        // power에 따른 총알 변경
        switch (power)
        {
            case 1:
                // Instantiate : 프리펩의 오브젝트를 화면에 생성 (Destroy의 반대)
                // >> 오브젝트 풀링으로 인해 그냥 instantiate는 모두 바뀜
                GameObject bullet = objectManager.ActiveObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2: // 작은 총알 2개 발사
                // 총알이 좌/우로 두개 나가야 하므로 position에 Vector값을 추가
                GameObject bulletR = objectManager.ActiveObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                GameObject bulletL = objectManager.ActiveObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3: // 총알B 1개 발사
                GameObject bulletB = objectManager.ActiveObj("BulletPlayerB");
                bulletB.transform.position = transform.position;
                bulletB.transform.localScale = Vector2.one * 1.5f;
                Rigidbody2D rigidB = bulletB.GetComponent<Rigidbody2D>();
                rigidB.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 4: // 총알A 2개 추가 발사
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
            default: // 가장 높은 파워의 경우(단, 이 다음부터는 Follower가 등장하므로 Default로 해주어야함)
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

        // 한발 쏘고 난 뒤에 curShotDelay값을 0으로 초기화
        curShotDelay = 0;
    }

    void Reload()
    {
        // 딜레이변수는 델타타임을 계속 더하여 계산
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
        gameManager.UpdateBoomIcon(boom);   // boom 사용 시 UI로 boom 아이콘 삭제

        // 1. Boom Effect
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 1);
        // 2. Remove Enemy
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");

        for (int i = 0; i < enemiesL.Length; i++)
        {
            if (enemiesL[i].activeSelf) // 활성화된 오브젝트만 적용
            {
                Enemy enemyLogic = enemiesL[i].GetComponent<Enemy>();
                enemyLogic.OnHit(1000); // 이전에 만든 Enemy클래스 안의 피격함수를 다시 활용
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

    // 물리적 충돌이 아닌 그저 Trigger로써 사용(어차피 스크립트로 위치를 조절할 것이므로)
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
            // 무적 시간 중
            if (isRespawnTime)
                return;
            // 두번 맞기 방지
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

            // 피격 당하면 player는 잠시 비활성화
            gameObject.SetActive(false);

            // 적 또는 적 총알과 닿았을 경우
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
            {
                collision.gameObject.SetActive(false);              // 플레이어와 부딫히면 적도 파괴
                collision.transform.rotation = Quaternion.identity; // 플레이어에게 부딫혀서 비활성화된 적들의 기울기도 원상복구
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
                        gameManager.UpdateBoomIcon(boom);   // boom추가 시 UI로 boom 아이콘 추가
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

    // Trigger에 닿았을때는 bool값이 true, 닿지 않은 경우에는 bool값이 false
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
