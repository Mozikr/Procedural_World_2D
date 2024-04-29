using UnityEngine;

public class FixPosition : MonoBehaviour
{
    public Transform playerTransform;
    public Transform Audio;

    void Update()
    {
        if (playerTransform != null)
        {
            Audio.transform.position = playerTransform.position;
        }
    }
}