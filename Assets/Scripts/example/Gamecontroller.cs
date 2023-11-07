using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gamecontroller : MonoBehaviour
{
    public List<BodyPart> bodyPrefabList;
    public Transform spawnPoint;
    public Transform nextSpawnPoint;
    public Button playButton;
    public float minX = -3.5f; // 最小Y坐标
    public float maxX = 3.5f; // 最大Y坐标

    private Vector3 mousePositionOffset; // 用于保存鼠标位置的偏移

    public GameObject playerObject; // 在Inspector中分配玩家物件

    public Transform target; // 在Inspector中分配目标对象

    private bool isMousePressed = false; // 用于标记鼠标左键是否按住
    private GameObject spawnedObject; // 用于存储通过Instantiate创建的物体

    private BodyPart previewFruit;

    private BodyPart currentFruit; // 用于存储当前生成的物体

    public TextMeshProUGUI scoreLabel;
    /// <summary>
    /// 游戏分数
    /// </summary>
    public int score;

    private BodyPart bodyPart;
    private int fruidId;
    private bool isGameOver;
    /// <summary>
    /// 当前场景中所有的水果对象
    /// </summary>
    private List<BodyPart> bodys = new List<BodyPart>();

    public float dropInterval = 3.0f;

    private float timeSinceLastDrop = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        // 一开始生成 spawnPoint 和 nextSpawnPoint 的物体
        //SpawnCurrentFruit();

    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            return;
        }
        // 让玩家对象跟随目标对象移动
        if (target != null)
        {
            Vector3 targetPosition = target.position;
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX); // 限制X坐标在minX和maxX之间
            targetPosition.y = playerObject.transform.position.y; // 保持Y坐标不变
            playerObject.transform.position = targetPosition;

            // 让spawnPoint跟随target移动
            Vector3 spawnPointPosition = spawnPoint.position;
            spawnPointPosition.x = targetPosition.x; // 使spawnPoint的X坐标等于target的X坐标
            spawnPoint.position = spawnPointPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            var mousePos = Input.mousePosition;
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // 限制放下水果的X坐标在指定的范围内，同时保持Y坐标不变
            float newX = Mathf.Clamp(worldPos.x, minX, maxX);
            Vector3 bodyPos = new Vector3(newX, spawnPoint.position.y, 0);


            if (currentFruit != null)
            {
                currentFruit.gameObject.transform.position = bodyPos;
                currentFruit.SetSimulated(true);
            }
            else
            {
                // 如果 currentFruit 为空，不执行任何操作
                Debug.Log("currentFruit is null. No action taken.");
            }

            // 将nextSpawnPoint生成下一个物体
            currentFruit = SpawnNextFruit();

        }


        timeSinceLastDrop += Time.deltaTime;

        if (timeSinceLastDrop >= dropInterval)
        {
            timeSinceLastDrop = 0.0f;
        }

    }

    // 辅助方法：将 nextSpawnPoint 的物体位置直接移动到 spawnPoint 的位置
    private void MoveNextSpawnPointToSpawnPoint()
    {
        nextSpawnPoint.position = spawnPoint.position;
    }

    // 生成当前物体
    private void SpawnCurrentFruit()
    {
        int rand = Random.Range(0, bodyPrefabList.Count);
        GameObject prefab = bodyPrefabList[rand].gameObject;

        Vector3 spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
        currentFruit = SpawnFruit(prefab, spawnPosition);
    }

    /// <summary>
    /// 生成下一個水果
    /// </summary>
    /// <returns></returns>
    private BodyPart SpawnNextFruit()
    {
        // 将 nextSpawnPoint 的物体位置直接移动到 spawnPoint 的位置
        MoveNextSpawnPointToSpawnPoint();

        int rand = Random.Range(0, bodyPrefabList.Count);
        GameObject prefab = bodyPrefabList[rand].gameObject;

        Vector3 nextSpawnPosition = new Vector3(nextSpawnPoint.position.x, nextSpawnPoint.position.y, nextSpawnPoint.position.z);
        BodyPart nextFruit = SpawnFruit(prefab, nextSpawnPosition);

        return nextFruit;
    }

    /// <summary>
    /// 生成水果
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private BodyPart SpawnFruit(GameObject prefab, Vector3 pos)
    {
        var obj = Instantiate(prefab, pos, Quaternion.identity);
        var part = obj.GetComponent<BodyPart>();
        part.SetSimulated(false);
        part.id = fruidId++;
        // 設定水果的父物件為玩家物件
        //obj.transform.parent = spawnPoint.transform;


        part.OnLevelUp = (a, b) =>
        {
            if (IsBodyExist(a) && IsBodyExist(b))
            {
                var pos1 = a.gameObject.transform.position;
                var pos2 = b.gameObject.transform.position;
                var pos = (pos1 + pos2) * 0.5f;
                RemoveFruit(a);
                RemoveFruit(b);
                AddScore(a.score);
                obj.transform.parent = null;
                var fr = SpawnFruit(a.nextLevelPrefab, pos);
                obj.transform.parent = null;
                fr.SetSimulated(true);
            }
        };

        part.OnGameOver = () =>
        {
            if (isGameOver == true)
            {
                return;
            }
            OnGameOver();
        };

        bodys.Add(part);
        return part;
    }

    /// <summary>
    /// 遊戲失敗
    /// </summary>
    private void OnGameOver()
    {
        isGameOver = true;
        playButton.gameObject.SetActive(true);

        for (int i = 0; i < bodys.Count; i++)
        {
            bodys[i].SetSimulated(false);
            AddScore(bodys[i].score);
            Destroy(bodys[i].gameObject);
        }

        bodys.Clear();
    }

    /// <summary>
    /// 重新遊戲
    /// </summary>
    public void Restart()
    {
        playButton.gameObject.SetActive(false);
        bodyPart = SpawnNextFruit();

        score = 0;
        scoreLabel.text = "0";

        isGameOver = false;
    }

    /// <summary>
    /// 移除水果
    /// </summary>
    /// <param name="body"></param>
    private void RemoveFruit(BodyPart body)
    {
        for (int i = 0; i < bodys.Count; i++)
        {
            if (bodys[i].id == body.id)
            {
                bodys.Remove(body);
                Destroy(body.gameObject);
                return;
            }
        }
    }

    /// <summary>
    /// 當水果離開
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    private bool IsBodyExist(BodyPart body)
    {
        for (int i = 0; i < bodys.Count; i++)
        {
            if (bodys[i].id == body.id)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 增加分數
    /// </summary>
    /// <param name="score"></param>
    private void AddScore(int score)
    {
        this.score += score;
        scoreLabel.text = $"{this.score}";
    }

}
