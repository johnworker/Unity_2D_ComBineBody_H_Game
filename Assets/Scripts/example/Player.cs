using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    public float minX = -5.2f; // 最小X範圍
    public float maxX = 5.2f;  // 最大X範圍

    private ObjectManager objectManager;

    private void Start()
    {
        //objectManager = FindObjectOfType<ObjectManager>(); // 找到場景中的ObjectManager腳本
        //objectManager.PreviewSpawnObject();
    }

    private void Update()
    {
        // 移動玩家 (鍵盤)
        //float moveInput = Input.GetAxis("Horizontal");
        //float newX = transform.position.x + moveInput * moveSpeed * Time.deltaTime;

        // 移動玩家 (滑鼠左鍵壓著)
        if (Input.GetMouseButton(0)) // 如果鼠标左键被按住
        {
            // 获取鼠标当前的屏幕坐标
            Vector3 mousePosition = Input.mousePosition;

            // 将屏幕坐标转换为世界坐标
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // 限制玩家的新X坐标在指定的范围内
            float newX = Mathf.Clamp(worldPosition.x, minX, maxX);

            // 设置玩家的新位置
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        }
        // 限制移動範圍

        // 限制移動範圍
        // newX = Mathf.Clamp(newX, minX, maxX);
        // transform.position = new Vector3(newX, transform.position.y, transform.position.z);



        // 放下物體
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //objectManager.SpawnRandomObject(); // 在玩家位置生成物體
            //objectManager.PreviewSpawnObject();
        }
    }
}
