using UnityEngine;

public class ObjectDropper : MonoBehaviour
{
    public GameObject objectPrefab;
    public Transform spawnPoint;
    public float dropInterval = 2.0f;

    private float timeSinceLastDrop = 0.0f;

    private void Update()
    {
        timeSinceLastDrop += Time.deltaTime;

        if (timeSinceLastDrop >= dropInterval)
        {
            DropObject();
            timeSinceLastDrop = 0.0f;
        }
    }

    private void DropObject()
    {
        Vector3 spawnPosition = spawnPoint.position;
        Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
    }
}
