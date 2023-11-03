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
    //public Button playButton;
    public float minX = -5.2f; // 最小Y坐标
    public float maxX = 5.2f; // 最大Y坐标

    private Vector3 mousePositionOffset; // 用于保存鼠标位置的偏移

    public GameObject playerObject; // 在Inspector中分配玩家物件

    private bool isMousePressed = false; // 用于标记鼠标左键是否按住
    private GameObject spawnedObject; // 用于存储通过Instantiate创建的物体

    private BodyPart previewFruit;

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
    private List<BodyPart> fruits = new List<BodyPart>();
    // Start is called before the first frame update
    void Start()
    {
        bodyPart = SpawnNextFruit();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            bodyPart.gameObject.transform.parent = null;
            var mousePos = Input.mousePosition;
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // 限制放下水果的X坐标在指定的范围内，同时保持Y坐标不变
            float newX = Mathf.Clamp(worldPos.x, minX, maxX);
            Vector3 fruitPos = new Vector3(newX, spawnPoint.position.y, 0);

            bodyPart.gameObject.transform.position = fruitPos;
            bodyPart.SetSimulated(true);

            bodyPart = SpawnNextFruit();
        }

        if (Input.GetMouseButton(0) && spawnedObject != null)
        {
            // 获取鼠标位置
            var mousePos = Input.mousePosition;
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // 限制预制体的X坐标在指定的范围内，同时保持Y坐标不变
            float newX = Mathf.Clamp(worldPos.x, minX, maxX);
            Vector3 prefabPos = new Vector3(newX, spawnPoint.position.y, 0);

            // 移动预制体到新位置
            spawnedObject.transform.position = prefabPos;
        }

        // 在 GetMouseButtonDown(0) 时更新预览水果位置
        if (Input.GetMouseButtonDown(0) && previewFruit != null)
        {
            // 获取鼠标位置
            var mousePos = Input.mousePosition;
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // 限制预览水果的X坐标在指定的范围内，同时保持Y坐标不变
            float newX = Mathf.Clamp(worldPos.x, minX, maxX);
            Vector3 previewFruitPos = new Vector3(newX, spawnPoint.position.y, 0);

            // 移动预览水果到新位置
            previewFruit.gameObject.transform.position = previewFruitPos;
        }
        // 放下物體
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            // 將水果的父物件設置為 null，從而將其從玩家物件的子物件中移除
            fruit.gameObject.transform.parent = null;
            var fruitPos = new Vector3(spawnPoint.transform.position.x, spawnPoint.position.y, 0);
            fruit.gameObject.transform.position = fruitPos;
            fruit.SetSimulated(true);

            // 更新當前水果為預覽水果
            fruit = SpawnNextFruit();

            // 將預覽水果的位置設置為 spawnPoint 以便預覽下一個水果
            var nextFruitPos = new Vector3(spawnPoint.transform.position.x, spawnPoint.position.y, 0);
            fruit.gameObject.transform.position = nextFruitPos;
            fruit.SetSimulated(false);
        }*/



    }

    /// <summary>
    /// 生成下一個水果
    /// </summary>
    /// <returns></returns>
    private BodyPart SpawnNextFruit()
    {
        int rand = Random.Range(0, bodyPrefabList.Count);
        GameObject prefab = bodyPrefabList[rand].gameObject;

        // 使用 nextSpawnPoint 生成預覽水果
        Vector3 previewFruitPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
        BodyPart previewFruit = SpawnFruit(prefab, previewFruitPosition);

        return previewFruit;
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
        obj.transform.parent = spawnPoint.transform;


        part.OnLevelUp = (a, b) =>
        {
            if (IsFruitExist(a) && IsFruitExist(b))
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

        fruits.Add(part);
        return part;
    }

    /// <summary>
    /// 遊戲失敗
    /// </summary>
    private void OnGameOver()
    {
        isGameOver = true;
        //playButton.gameObject.SetActive(true);

        for (int i = 0; i < fruits.Count; i++)
        {
            fruits[i].SetSimulated(false);
            AddScore(fruits[i].score);
            Destroy(fruits[i].gameObject);
        }

        fruits.Clear();
    }

    /// <summary>
    /// 重新遊戲
    /// </summary>
    public void Restart()
    {
        //playButton.gameObject.SetActive(false);
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
        for (int i = 0; i < fruits.Count; i++)
        {
            if (fruits[i].id == body.id)
            {
                fruits.Remove(body);
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
    private bool IsFruitExist(BodyPart body)
    {
        for (int i = 0; i < fruits.Count; i++)
        {
            if (fruits[i].id == body.id)
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
