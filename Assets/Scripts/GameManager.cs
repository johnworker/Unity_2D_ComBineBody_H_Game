using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject player; // 玩家角色
    public GameObject[] itemPrefabs; // 多个物品预制体
    public Transform spawnPoint; // 生成点
    public TextMeshProUGUI scoreText; // 显示得分的UI Text

    private int score = 0; // 玩家得分

    private void Start()
    {
        scoreText.text = "Score: 0";
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
            newItem.GetComponent<Item>().SetGameManager(this); // 设置物品的GameManager引用
        }
    }

    public void MergeItems(Item item1, Item item2)
    {
        if (item1.CompareTag(item2.tag))
        {
            // 合并物品
            Destroy(item1.gameObject);
            Destroy(item2.gameObject);

            // 增加分数并更新UI
            score += 10;
            scoreText.text = "Score: " + score;
        }
    }
}