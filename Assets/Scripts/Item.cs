using UnityEngine;

public class Item : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void SetGameManager(GameManager manager)
    {
        gameManager = manager;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 物品碰撞检测
        if (other.CompareTag(tag))
        {
            // 通知GameManager进行合并
            gameManager.MergeItems(this, other.GetComponent<Item>());
            Destroy(gameObject);
        }
    }
}