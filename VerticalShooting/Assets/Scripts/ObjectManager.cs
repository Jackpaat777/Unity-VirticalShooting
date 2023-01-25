using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    // 프리펩변수값을 가져오기 위한 변수값들
    public GameObject bossAPrefab;
    public GameObject bossBPrefab;
    public GameObject bossCPrefab;
    public GameObject enemyLPrefab;
    public GameObject enemyMPrefab;
    public GameObject enemySPrefab;
    public GameObject itemCoinPrefab;
    public GameObject itemPowerPrefab;
    public GameObject itemBoomPrefab;
    public GameObject bulletPlayerAPrefab;
    public GameObject bulletPlayerBPrefab;
    public GameObject bulletPlayerCPrefab;
    public GameObject bulletEnemyAPrefab;
    public GameObject bulletEnemyBPrefab;
    public GameObject bulletBossAPrefab;
    public GameObject bulletBossBPrefab;
    public GameObject bulletBossCPrefab;
    public GameObject bulletRaiserPrefab;
    public GameObject bulletFollowerPrefab;
    public GameObject explosionPrefab;

    // 오브젝트 풀링에 사용될 오브젝트들
    GameObject[] bossA;
    GameObject[] bossB;
    GameObject[] bossC;
    GameObject[] enemyL;
    GameObject[] enemyM;
    GameObject[] enemyS;

    GameObject[] itemCoin;
    GameObject[] itemPower;
    GameObject[] itemBoom;

    GameObject[] bulletPlayerA;
    GameObject[] bulletPlayerB;
    GameObject[] bulletPlayerC;
    GameObject[] bulletEnemyA;
    GameObject[] bulletEnemyB;
    GameObject[] bulletBossA;
    GameObject[] bulletBossB;
    GameObject[] bulletBossC;
    GameObject[] bulletRaiser;

    GameObject[] bulletFollower;

    GameObject[] explosion;

    GameObject[] targetPool;    // switch문에서 해당 오브젝트배열만 사용하기 위해 필요한 배열

    void Awake()
    {
        bossA = new GameObject[1];
        bossB = new GameObject[1];
        bossC = new GameObject[1];
        enemyL = new GameObject[10];
        enemyM = new GameObject[10];
        enemyS = new GameObject[20];

        itemCoin = new GameObject[20];
        itemPower = new GameObject[10];
        itemBoom = new GameObject[10];

        bulletPlayerA = new GameObject[100];
        bulletPlayerB = new GameObject[100];
        bulletPlayerC = new GameObject[100];
        bulletEnemyA = new GameObject[100];
        bulletEnemyB = new GameObject[500];
        bulletBossA = new GameObject[50];
        bulletBossB = new GameObject[1000];
        bulletBossC = new GameObject[20];
        bulletRaiser = new GameObject[1];

        bulletFollower = new GameObject[100];

        explosion = new GameObject[20];

        // 처음부터 오브젝트 풀을 만들어놓기(게임에서 로딩이 발생하는 이유)
        Generate();
    }

    void Generate()
    {
        // 1. Enemy
        for (int i = 0; i < bossA.Length; i++)
        {
            bossA[i] = Instantiate(bossAPrefab);
            bossA[i].SetActive(false);
        }
        for (int i = 0; i < bossB.Length; i++)
        {
            bossB[i] = Instantiate(bossBPrefab);
            bossB[i].SetActive(false);
        }
        for (int i = 0; i < bossC.Length; i++)
        {
            bossC[i] = Instantiate(bossCPrefab);
            bossC[i].SetActive(false);
        }
        for (int i = 0; i < enemyL.Length; i++)
        {
            enemyL[i] = Instantiate(enemyLPrefab);  // 이번엔 이전에 한것처럼 함수의 인자로 player의 위치값같은게 필요없음
            enemyL[i].SetActive(false);
        }
        for (int i = 0; i < enemyM.Length; i++)
        {
            enemyM[i] = Instantiate(enemyMPrefab);
            enemyM[i].SetActive(false);
        }
        for (int i = 0; i < enemyS.Length; i++)
        {
            enemyS[i] = Instantiate(enemySPrefab);
            enemyS[i].SetActive(false);
        }
        // 2. Item
        for (int i = 0; i < itemCoin.Length; i++)
        {
            itemCoin[i] = Instantiate(itemCoinPrefab);
            itemCoin[i].SetActive(false);
        }
        for (int i = 0; i < itemPower.Length; i++)
        {
            itemPower[i] = Instantiate(itemPowerPrefab);
            itemPower[i].SetActive(false);
        }
        for (int i = 0; i < itemBoom.Length; i++)
        {
            itemBoom[i] = Instantiate(itemBoomPrefab);
            itemBoom[i].SetActive(false);
        }
        // 3. Bullet
        for (int i = 0; i < bulletPlayerA.Length; i++)
        {
            bulletPlayerA[i] = Instantiate(bulletPlayerAPrefab);
            bulletPlayerA[i].SetActive(false);
        }
        for (int i = 0; i < bulletPlayerB.Length; i++)
        {
            bulletPlayerB[i] = Instantiate(bulletPlayerBPrefab);
            bulletPlayerB[i].SetActive(false);
        }
        for (int i = 0; i < bulletPlayerC.Length; i++)
        {
            bulletPlayerC[i] = Instantiate(bulletPlayerCPrefab);
            bulletPlayerC[i].SetActive(false);
        }
        for (int i = 0; i < bulletEnemyA.Length; i++)
        {
            bulletEnemyA[i] = Instantiate(bulletEnemyAPrefab);
            bulletEnemyA[i].SetActive(false);
        }
        for (int i = 0; i < bulletEnemyB.Length; i++)
        {
            bulletEnemyB[i] = Instantiate(bulletEnemyBPrefab);
            bulletEnemyB[i].SetActive(false);
        }
        for (int i = 0; i < bulletBossA.Length; i++)
        {
            bulletBossA[i] = Instantiate(bulletBossAPrefab);
            bulletBossA[i].SetActive(false);
        }
        for (int i = 0; i < bulletBossB.Length; i++)
        {
            bulletBossB[i] = Instantiate(bulletBossBPrefab);
            bulletBossB[i].SetActive(false);
        }
        for (int i = 0; i < bulletBossC.Length; i++)
        {
            bulletBossC[i] = Instantiate(bulletBossCPrefab);
            bulletBossC[i].SetActive(false);
        }
        for (int i = 0; i < bulletFollower.Length; i++)
        {
            bulletFollower[i] = Instantiate(bulletFollowerPrefab);
            bulletFollower[i].SetActive(false);
        }
        for (int i = 0; i < explosion.Length; i++)
        {
            explosion[i] = Instantiate(explosionPrefab);
            explosion[i].SetActive(false);
        }
        for (int i = 0; i < bulletRaiser.Length; i++)
        {
            bulletRaiser[i] = Instantiate(bulletRaiserPrefab);
            bulletRaiser[i].SetActive(false);
        }
    }

    public GameObject ActiveObj(string type)
    {
        switch (type)
        {
            case "BossA":
                targetPool = bossA;
                break;
            case "BossB":
                targetPool = bossB;
                break;
            case "BossC":
                targetPool = bossC;
                break;
            case "EnemyL":
                targetPool = enemyL;
                break;
            case "EnemyM":
                targetPool = enemyM;
                break;
            case "EnemyS":
                targetPool = enemyS;
                break;
            case "ItemCoin":
                targetPool = itemCoin;
                break;
            case "ItemPower":
                targetPool = itemPower;
                break;
            case "ItemBoom":
                targetPool = itemBoom;
                break;
            case "BulletPlayerA":
                targetPool = bulletPlayerA;
                break;
            case "BulletPlayerB":
                targetPool = bulletPlayerB;
                break;
            case "BulletPlayerC":
                targetPool = bulletPlayerC;
                break;
            case "BulletEnemyA":
                targetPool = bulletEnemyA;
                break;
            case "BulletEnemyB":
                targetPool = bulletEnemyB;
                break;
            case "BulletBossA":
                targetPool = bulletBossA;
                break;
            case "BulletBossB":
                targetPool = bulletBossB;
                break;
            case "BulletBossC":
                targetPool = bulletBossC;
                break;
            case "BulletRaiser":
                targetPool = bulletRaiser;
                break;
            case "BulletFollower":
                targetPool = bulletFollower;
                break;
            case "Explosion":
                targetPool = explosion;
                break;
        }

        for (int i = 0; i < targetPool.Length; i++)
        {
            if (!targetPool[i].activeSelf) // 현재 배열의 오브젝트가 사용중이 아니라면
            {
                targetPool[i].SetActive(true);
                return targetPool[i];
            }
        }

        return null;
    }

    public GameObject[] GetPool(string type)
    {
        switch (type)
        {
            case "BossA":
                targetPool = bossA;
                break;
            case "BossB":
                targetPool = bossB;
                break;
            case "BossC":
                targetPool = bossC;
                break;
            case "EnemyL":
                targetPool = enemyL;
                break;
            case "EnemyM":
                targetPool = enemyM;
                break;
            case "EnemyS":
                targetPool = enemyS;
                break;
            case "ItemCoin":
                targetPool = itemCoin;
                break;
            case "ItemPower":
                targetPool = itemPower;
                break;
            case "ItemBoom":
                targetPool = itemBoom;
                break;
            case "BulletPlayerA":
                targetPool = bulletPlayerA;
                break;
            case "BulletPlayerB":
                targetPool = bulletPlayerB;
                break;
            case "BulletPlayerC":
                targetPool = bulletPlayerC;
                break;
            case "BulletEnemyA":
                targetPool = bulletEnemyA;
                break;
            case "BulletEnemyB":
                targetPool = bulletEnemyB;
                break;
            case "BulletBossA":
                targetPool = bulletBossA;
                break;
            case "BulletBossB":
                targetPool = bulletBossB;
                break;
            case "BulletBossC":
                targetPool = bulletBossC;
                break;
            case "BulletRaiser":
                targetPool = bulletRaiser;
                break;
            case "BulletFollower":
                targetPool = bulletFollower;
                break;
            case "Explosion":
                targetPool = explosion;
                break;
        }

        return targetPool;
    }
}
