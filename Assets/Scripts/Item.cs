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
        Item otherItem = collision.collider.GetComponent<Item>();

        // 检查碰撞的对象是否是物品，且是当前物品的两个相同实例
        if (otherItem != null && otherItem == this)
        {
            // 通知GameManager进行合并
            gameManager.MergeItems(this, otherItem);
            Destroy(gameObject);
        }
    }
}