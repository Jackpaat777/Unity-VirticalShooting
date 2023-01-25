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
    // player와 objectManager는 프리펩이 아니기 때문에 직접 값을 넣어줄 수 없음
    // >> GameObject에서 SpawnEnemy를 할 때 enemyLogic을 통해 값을 넘겨줌
    public GameObject player;
    public GameManager gameManager;
    public ObjectManager objectManager;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 컴포넌트가 활성화될 때 호출되는 생명주기함수
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

    // ==================일반 적 관련 함수(보스 포함)=====================
    void Update()
    {
        Fire();
        Reload();
    }

    void Fire()
    {
        // 현재 총알의 딜레이가 설정된 최대 총알의 딜레이를 넘지 않았을 경우 (한 발 쏘고 난 뒤 다음 총알이 나갈때까지의 딜레이시간이 아직 충족되지 않음)
        if (curShotDelay < maxShotDelay)
            return;

        // M은 총알을 쏘지 않고 부딫히는 것만
        if (enemyName == "S")
        {
            GameObject bullet = objectManager.ActiveObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            // 플레이어의 위치에서 적의 위치를 빼준 값으로 벡터 설정 (두 점 사이의 벡터값)
            Vector3 dirVec = player.transform.position - transform.position;
            // dirVec은 단위벡터가 아닌 일반벡터이므로 단위벡터로 만들어준 뒤 speed를 곱하여 사용하기 위해서는 normalized 해주기
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
            // 적 위치의 좌우값이 기준이므로 괄호로 묶어주기
            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);
            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
        }

        // 한발 쏘고 난 뒤에 curShotDelay값을 0으로 초기화
        curShotDelay = 0;
    }

    void Reload()
    {
        // 딜레이변수는 델타타임을 계속 더하여 계산
        curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)
    {
        // 두발을 동시에 맞아 아이템을 2번 드랍하는 경우를 방지
        if (health <= 0)
            return;

        health -= dmg;

        // 피격당한 경우 스프라이트 변경
        spriteRenderer.sprite = sprites[1];
        // 다시 원상복구할 경우에는 텀을 두어서 스프라이트 변경
        Invoke("ReturnSprite", 0.1f);

        if (health <= 0)
        {
            // player를 바로 사용하지 않고 따로 변수를 선언하여 호출하는 이유?
            // player는 그냥 GameObject값이므로 Player클래스 내부 변수를 사용할 수 없음
            // 대신 player변수 안에 Player클래스가 들어간 오브젝트가 있으므로 이를 통해 GetComponent를 해줌
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

            // 오브젝트 풀을 사용하고 있으므로 Destroy가 아닌 비활성화를 시켜주기
            // 오브젝트가 다시 나올 때 rotate된 상태로 나오는 것을 방지하기 위해 삭제한 직후에는 방향 복구
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            gameManager.CallExplosion(transform.position, enemyName);   // enemyName을 넘겨주면 바로 explosion의 타입을 결정해줌

        }
    }

    // 아이템 드랍 확률
    void ItemDropRatio(string name)
    {
        int noItem = 0;
        int coin = 0;
        int power = 0;
        int boom = 0;

        // 확률 분배
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


        // 확률 적용
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
            // Bullet에 있는 dmg 변수값을 불러오기 위해 선언
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            // 피격 시 총알 오브젝트 비활성화
            collision.gameObject.SetActive(false);
        }
    }

}
