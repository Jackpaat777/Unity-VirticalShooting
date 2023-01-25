using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

// 적 기체 생성 관리
// 플레이어 복귀
public class GameManager : MonoBehaviour
{
    public int maxStage;
    public int stage;
    public Animator stageAnim;
    public Animator clearAnim;
    public Animator fadeAnim;
    public Transform playerPos;
    public TextMeshProUGUI endText;

    public string[] enemyObjs;
    public Transform[] spawnPoints;

    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public TextMeshProUGUI scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;
    public GameObject gameStopSet;
    public ObjectManager objectManager;

    // 적 출현에 필요한 변수들
    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    public Background[] background;

    // 게임 정지에 필요한 변수
    public bool playerStop;

    public BackMusic backMusic;

    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "BossA", "BossB", "BossC" };
        StageStart();
    }

    public void StageStart()
    {
        // Enemy Spawn File Read
        // 스테이지 시작 시 파일 불러오기
        ReadSpawnFile();

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.life = 3;
        playerLogic.transform.position = new Vector3(0, -4, 0);
        UpdateLifeIcon(playerLogic.life);
        playerLogic.isRespawnTime = false;

        // Player Reposition
        player.transform.position = playerPos.position;

        // Stage UI Load
        stageAnim.SetTrigger("On");
        // Animator 변수지만 같은 오브젝트에 Text가 있으면 불러오기 가능
        stageAnim.GetComponent<TextMeshProUGUI>().text = "Stage " + stage + "\nStart";
        clearAnim.GetComponent<TextMeshProUGUI>().text = "Stage " + stage + "\nClear!";

        // Fade In(밝아지기)
        fadeAnim.SetTrigger("In");

        // 스테이지 시작 시 현재 딜레이값 초기화
        curSpawnDelay = 0;

        // Stage시작할 때 마다 BGM 변경
        backMusic.PlayBGM("Stage" + stage);
    }

    public void StageEnd()
    {
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isRespawnTime = true;

        // Clear UI Load
        clearAnim.SetTrigger("On");

        // Fade Out(어두워지기)
        fadeAnim.SetTrigger("Out");

        // Stage Increament
        stage++;
        if (stage > maxStage)
            Invoke("GameClear", 6);
        else
            Invoke("StageStart", 6);
    }

    void ReadSpawnFile()
    {
        // 변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        // 리스폰 파일 읽기
        // as : 만약 이 파일이 TextAsset형태가 아닐 경우 그냥 null로 처리함
        // stage대로 스테이지 파일을 불러옴
        TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while (stringReader != null)
        {
            // 한 줄씩 읽기
            string line = stringReader.ReadLine();

            if (line == null)
                break;

            // 리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            // Parse : 문자열을 숫자로 형변환하기
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);

            spawnList.Add(spawnData);
        }

        // 파일을 다 읽고 난 뒤에는 반드시 파일 닫기
        stringReader.Close();

        // 첫번째 스폰 딜레이 적용
        nextSpawnDelay = spawnList[0].delay;
    }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        // 현재 스폰 딜레이가 상정된 시간값(nextSpawnDelay)을 만족하면 새로 스폰
        // 스폰이 끝났을 경우에는 실행되지 않도록 spawnEnd 추가
        if (curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;  // 적이 생성된 이후 꼭 마지막에 curDelay는 0으로 초기화 해주어야함
        }

        Player playerLogic = player.GetComponent<Player>();
        // 3자리마다 컴마 붙여주기
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }

    void SpawnEnemy()
    {
        // 보스는 따로 함수로 >> return을 해줘야 하므로 switch는 못쓸듯
        if (spawnList[spawnIndex].type == "B")
        {
            SpawnBoss(3);
            return;
        }
        else if (spawnList[spawnIndex].type == "B2")
        {
            SpawnBoss(4);
            return;
        }
        else if (spawnList[spawnIndex].type == "B3")
        {
            SpawnBoss(5);
            return;
        }

        // 이젠 랜덤하지 않고 텍스트파일에 나와있는 것처럼 적을 스폰
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
        }
        int enemyPoint = spawnList[spawnIndex].point;

        // 기존의 Instantiate를 ObjectManager의 오브젝트 풀에서 대신 만들어줌
        // enemyObjs도 기존의 GameObject형식에서 string형식으로 바꾸어 인자를 넘겨줌
        GameObject enemy = objectManager.ActiveObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;  // 포지션은 따로 추가해주기

        // 적 기체의 이동도 GameManager가 직접 관리하도록(Point에 따라 기체의 움직임이 달라져야 하므로)
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        // 플레이어오브젝트를 Enemy 스크립트의 player변수에 넘겨줌
        // 오브젝트매니저도 Enemy클래스에 넘겨줌
        enemyLogic.player = player;
        enemyLogic.objectManager = objectManager;
        enemyLogic.gameManager = this;  // 자기자신을 넘겨줄 땐 this

        // 좌우에서 오는 적은 방향을 돌려줌
        if (enemyPoint == 5 || enemyPoint == 6)         // Right Spawn
        {
            // Vector.back : z축을 시계방향으로 돌림
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
        }
        else if (enemyPoint == 7 || enemyPoint == 8)    // Left Spawn
        {
            // Vector.forward : z축을 반시계방향으로 돌림
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
        }
        else                                        // Front Spawn
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
        }

        // 리스폰 인덱스 증가
        spawnIndex++;
        // 스폰이 모두 끝나는 경우
        // List의 경우 Length가 아닌 Count를 사용
        if (spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        // 적 생성이 완료되면 다음 생성을 위한 시간 갱신
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    // 보스 생성 함수
    void SpawnBoss(int bossIdx)
    {
        int enemyPoint = spawnList[spawnIndex].point;

        GameObject boss = objectManager.ActiveObj(enemyObjs[bossIdx]);
        boss.transform.position = spawnPoints[enemyPoint].position;

        Rigidbody2D rigid = boss.GetComponent<Rigidbody2D>();
        Boss bossLogic = boss.GetComponent<Boss>();
        bossLogic.player = player;
        bossLogic.objectManager = objectManager;
        bossLogic.gameManager = this;

        rigid.velocity = new Vector2(0, bossLogic.speed * (-1));

        spawnIndex++;

        if (spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    public void UpdateLifeIcon(int life)
    {
        // UI Life Init Disable
        for (int i = 0; i < 3; i++)
        {
            lifeImage[i].color = new Color(1, 1, 1, 0);
        }

        // UI Life Active
        for (int i = 0; i < life; i++)
        {
            lifeImage[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        // UI Life Init Disable
        for (int i = 0; i < 3; i++)
        {
            boomImage[i].color = new Color(1, 1, 1, 0);
        }

        // UI Life Active
        for (int i = 0; i < boom; i++)
        {
            boomImage[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 1); // Invoke를 통해 2초 뒤에 실행해도 그 뒤의 코드는 즉시 실행됨
    }

    // RespawnPlayer 함수를 Invoke하기 위해 만들어진 함수
    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 4;
        player.SetActive(true);
        

        // isHit를 Player클래스에서 사용하지 않은 이유?
        // 리스폰 도중 플레이어가 다시 나오는 시점에 isHit를 꺼주어야하기 때문에 Invoke함수 안에 들어가 있어야함
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
        for (int i = 0; i < playerLogic.power - 6; i++)
        {
            playerLogic.followers[i].ResetQueue();
        }
    }

    public void CallExplosion(Vector3 pos, string type)
    {
        GameObject explosion = objectManager.ActiveObj("Explosion");
        Explosion explosionLogic = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;
        explosionLogic.StartExplosion(type);
    }

    public void GameOver()
    {
        endText.text = "Game Over";
        gameOverSet.SetActive(true);
    }

    public void GameClear()
    {
        Time.timeScale = 0;
        endText.text = "Game Clear!";
        gameOverSet.SetActive(true);

        // Game Clear Audio
        backMusic.PlayBGM("Clear");
    }

    public void GameContinue()
    {
        IsGameStop(false);
        gameStopSet.SetActive(false);
    }

    public void GameRetry()
    {
        IsGameStop(false);
        // Build Setting에서 Scene을 저장해두어야 ㅣLoad가 가능함
        SceneManager.LoadScene("InGameScene");
    }

    public void GoToMenu()
    {
        IsGameStop(false);
        SceneManager.LoadScene("MainMenuScene");
    }

    public void GameStop()
    {
        //게임 스탑 시에만 일시정지를 해줌
        IsGameStop(true);
        gameStopSet.SetActive(true);
    }

    // 일시정지 (player Move불가 & 시간 정지) 또는 재생
    void IsGameStop(bool isStop)
    {
        if (isStop)
        {
            Time.timeScale = 0;
            playerStop = true;
        }
        else
        {
            Time.timeScale = 1;
            playerStop = false;
        }
    }
}
