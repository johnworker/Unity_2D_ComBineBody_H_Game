using UnityEngine;

public class Item : MonoBehaviour
{
    private GameManager gameManager;
    public string itemType; // 物品类型
    public int itemLevel; // 物品等级

    private void Start()
    {
        gameManager = GameObject.Find("遊戲管理器").GetComponent<GameManager>();
    }

    public void SetGameManager(GameManager manager)
    {
        gameManager = manager;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 物品碰撞检测
        if (collision.collider.CompareTag(tag))
        {
            // 通知GameManager进行合并
            gameManager.MergeItems(this, collision.collider.GetComponent<Item>());
            Destroy(gameObject);
        }
    }
}