using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject player; // 玩家角色
    public GameObject[] itemPrefabs; // 多个物品预制体
    public Transform spawnPoint; // 生成点
    public TextMeshProUGUI scoreText; // 显示得分的UI Text

    private int score = 0; // 玩家得分

    // 用于跟踪不同类型物品的字典
    private Dictionary<string, int> itemLevels = new Dictionary<string, int>();

    private void Start()
    {
        scoreText.text = "Score: 0";

        // 初始化物品类型的等级
        foreach (var itemPrefab in itemPrefabs)
        {
            Item itemComponent = itemPrefab.GetComponent<Item>();
            if (!itemLevels.ContainsKey(itemComponent.itemType))
            {
                itemLevels[itemComponent.itemType] = 1;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 玩家按住鼠标左键可移动角色
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            player.transform.position = new Vector3(mousePosition.x, player.transform.position.y, 0);
        }

        if (Input.GetMouseButtonUp(0))
        {
            // 玩家放开鼠标左键生成随机物品
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            GameObject newItem = Instantiate(itemPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
            Item itemComponent = newItem.GetComponent<Item>();
            itemComponent.SetGameManager(this);
            itemComponent.itemType = itemComponent.itemType; // 设置物品类型
        }
    }

    public void MergeItems(Item item1, Item item2)
    {
        if (item1.itemType == item2.itemType)
        {
            // 合并物品
            Destroy(item1.gameObject);
            Destroy(item2.gameObject);

            // 升级物品
            itemLevels[item1.itemType]++;

            // 增加分数并更新UI
            score += itemLevels[item1.itemType] * 10;
            scoreText.text = "Score: " + score;
        }
    }
}