using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject[] objectPrefabs; // 這裡的物體預置件應包含A、B、C、D、E
    public Transform spawnPoint;
    private GameObject previewObject; // 預覽物體實例

    public GameObject tempObject;


    private List<GameObject> objectsInScene = new List<GameObject>();
    private int[] objectScores = { 1, 2, 3, 4, 5 }; // 每種物體的分數

    private int score = 0;

    private void Start()
    {
    }

    /// <summary>
    /// 生成點預覽生成隨機物件
    /// </summary>
    public void PreviewSpawnObject()
    {
        int randomIndex = Random.Range(0, objectPrefabs.Length);

        // 實例化預覽物體，使用與newObject相同的預置物
        previewObject = Instantiate(objectPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);

        // 如果需要，可以設定預覽物體為透明或半透明，以便它只是一個預覽
    }

    /// <summary>
    /// 生成隨機物件
    /// </summary>
    public void SpawnRandomObject()
    {
        if (previewObject != null)
        {
            // 如果預覽物體存在，刪除它
            Destroy(previewObject);
        }

        int randomIndex = Random.Range(0, objectPrefabs.Length);
        tempObject = Instantiate(objectPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        objectsInScene.Add(tempObject);

        tempObject.GetComponent<Rigidbody>().isKinematic = false;
    }
    /// <summary>
    ///合併方法
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject objectA = collision.gameObject;
        GameObject objectB = collision.otherCollider.gameObject;

        if (objectA.name == objectB.name)
        {
            int indexA = System.Array.IndexOf(objectPrefabs, objectA);
            int indexB = System.Array.IndexOf(objectPrefabs, objectB);

            // 檢查是否為相同的物體並且相鄰
            if (indexA != -1 && indexB != -1 && Mathf.Abs(indexA - indexB) == 1)
            {
                // 合併成功，刪除舊物體
                Destroy(objectA);
                Destroy(objectB);

                // 根據合成規則更新物體
                if (indexB < objectPrefabs.Length - 1)
                {
                    // 找到下一個合併的物體
                    GameObject nextObjectPrefab = objectPrefabs[indexB + 1];

                    // 從場景中刪除所有原始物體
                    for (int i = objectsInScene.Count - 1; i >= 0; i--)
                    {
                        if (objectsInScene[i].name == objectA.name || objectsInScene[i].name == objectB.name)
                        {
                            objectsInScene.RemoveAt(i);
                        }
                    }

                    // 生成新物體
                    GameObject newObject = Instantiate(nextObjectPrefab, spawnPoint.position, Quaternion.identity);
                    objectsInScene.Add(newObject);
                }
            }
        }
    }
}
