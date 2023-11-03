using System;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public int id;
    public int score;
    public GameObject nextLevelPrefab;
    public Action<BodyPart, BodyPart> OnLevelUp;
    public Action OnGameOver;
    private Rigidbody2D rigid;
    /// <summary>
    /// 是否碰到红线
    /// </summary>
    private bool isTouchRedline;
    /// <summary>
    /// 和红线接触的时间
    /// </summary>
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        if (isTouchRedline == false)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer > 3)
        {
            Debug.Log("Game Over");
            OnGameOver?.Invoke();
        }
    }
    public void SetSimulated(bool b)
    {
        if (rigid == null)
        {
            rigid = GetComponent<Rigidbody2D>();
        }
        rigid.simulated = b;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        var fruit = obj.GetComponent<BodyPart>();

        if (fruit != null)
        {
            if (obj.name == gameObject.name)
            {
                if (nextLevelPrefab != null)
                {
                    // Set the parent to null first
                    obj.transform.parent = null;

                    // Handle merging, e.g., destroy or deactivate the collided fruit
                    Destroy(obj);
                    OnLevelUp?.Invoke(this, fruit);
                }
            }
        }

        if (obj.CompareTag("Fruit"))
        {
            // Set the parent to null first
            obj.transform.parent = null;

            if (obj.name == gameObject.name)
            {
                if (nextLevelPrefab != null)
                {
                    // Handle merging
                    // obj.transform.parent = null; // No need to set the parent to null again
                    OnLevelUp?.Invoke(this, fruit);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("Redline"))
        {
            Debug.Log("OnTriggerEnter2D Redline");
            isTouchRedline = true;
            // 解除連接，將父物體設置為 null
            obj.transform.parent = null;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("Redline"))
        {
            Debug.Log("OnTriggerExit2D Redline");
            isTouchRedline = false;
            timer = 0;
            obj.transform.parent = null;
        }
    }
}
