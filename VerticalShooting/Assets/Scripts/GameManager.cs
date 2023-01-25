using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

// �� ��ü ���� ����
// �÷��̾� ����
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

    // �� ������ �ʿ��� ������
    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    public Background[] background;

    // ���� ������ �ʿ��� ����
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
        // �������� ���� �� ���� �ҷ�����
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
        // Animator �������� ���� ������Ʈ�� Text�� ������ �ҷ����� ����
        stageAnim.GetComponent<TextMeshProUGUI>().text = "Stage " + stage + "\nStart";
        clearAnim.GetComponent<TextMeshProUGUI>().text = "Stage " + stage + "\nClear!";

        // Fade In(�������)
        fadeAnim.SetTrigger("In");

        // �������� ���� �� ���� �����̰� �ʱ�ȭ
        curSpawnDelay = 0;

        // Stage������ �� ���� BGM ����
        backMusic.PlayBGM("Stage" + stage);
    }

    public void StageEnd()
    {
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isRespawnTime = true;

        // Clear UI Load
        clearAnim.SetTrigger("On");

        // Fade Out(��ο�����)
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
        // ���� �ʱ�ȭ
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        // ������ ���� �б�
        // as : ���� �� ������ TextAsset���°� �ƴ� ��� �׳� null�� ó����
        // stage��� �������� ������ �ҷ���
        TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while (stringReader != null)
        {
            // �� �پ� �б�
            string line = stringReader.ReadLine();

            if (line == null)
                break;

            // ������ ������ ����
            Spawn spawnData = new Spawn();
            // Parse : ���ڿ��� ���ڷ� ����ȯ�ϱ�
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);

            spawnList.Add(spawnData);
        }

        // ������ �� �а� �� �ڿ��� �ݵ�� ���� �ݱ�
        stringReader.Close();

        // ù��° ���� ������ ����
        nextSpawnDelay = spawnList[0].delay;
    }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        // ���� ���� �����̰� ������ �ð���(nextSpawnDelay)�� �����ϸ� ���� ����
        // ������ ������ ��쿡�� ������� �ʵ��� spawnEnd �߰�
        if (curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;  // ���� ������ ���� �� �������� curDelay�� 0���� �ʱ�ȭ ���־����
        }

        Player playerLogic = player.GetComponent<Player>();
        // 3�ڸ����� �ĸ� �ٿ��ֱ�
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }

    void SpawnEnemy()
    {
        // ������ ���� �Լ��� >> return�� ����� �ϹǷ� switch�� ������
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

        // ���� �������� �ʰ� �ؽ�Ʈ���Ͽ� �����ִ� ��ó�� ���� ����
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

        // ������ Instantiate�� ObjectManager�� ������Ʈ Ǯ���� ��� �������
        // enemyObjs�� ������ GameObject���Ŀ��� string�������� �ٲپ� ���ڸ� �Ѱ���
        GameObject enemy = objectManager.ActiveObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;  // �������� ���� �߰����ֱ�

        // �� ��ü�� �̵��� GameManager�� ���� �����ϵ���(Point�� ���� ��ü�� �������� �޶����� �ϹǷ�)
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        // �÷��̾������Ʈ�� Enemy ��ũ��Ʈ�� player������ �Ѱ���
        // ������Ʈ�Ŵ����� EnemyŬ������ �Ѱ���
        enemyLogic.player = player;
        enemyLogic.objectManager = objectManager;
        enemyLogic.gameManager = this;  // �ڱ��ڽ��� �Ѱ��� �� this

        // �¿쿡�� ���� ���� ������ ������
        if (enemyPoint == 5 || enemyPoint == 6)         // Right Spawn
        {
            // Vector.back : z���� �ð�������� ����
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
        }
        else if (enemyPoint == 7 || enemyPoint == 8)    // Left Spawn
        {
            // Vector.forward : z���� �ݽð�������� ����
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
        }
        else                                        // Front Spawn
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
        }

        // ������ �ε��� ����
        spawnIndex++;
        // ������ ��� ������ ���
        // List�� ��� Length�� �ƴ� Count�� ���
        if (spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        // �� ������ �Ϸ�Ǹ� ���� ������ ���� �ð� ����
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    // ���� ���� �Լ�
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
        Invoke("RespawnPlayerExe", 1); // Invoke�� ���� 2�� �ڿ� �����ص� �� ���� �ڵ�� ��� �����
    }

    // RespawnPlayer �Լ��� Invoke�ϱ� ���� ������� �Լ�
    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 4;
        player.SetActive(true);
        

        // isHit�� PlayerŬ�������� ������� ���� ����?
        // ������ ���� �÷��̾ �ٽ� ������ ������ isHit�� ���־���ϱ� ������ Invoke�Լ� �ȿ� �� �־����
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
        // Build Setting���� Scene�� �����صξ�� ��Load�� ������
        SceneManager.LoadScene("InGameScene");
    }

    public void GoToMenu()
    {
        IsGameStop(false);
        SceneManager.LoadScene("MainMenuScene");
    }

    public void GameStop()
    {
        //���� ��ž �ÿ��� �Ͻ������� ����
        IsGameStop(true);
        gameStopSet.SetActive(true);
    }

    // �Ͻ����� (player Move�Ұ� & �ð� ����) �Ǵ� ���
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
