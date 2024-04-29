using UnityEngine;

public class FixPosition : MonoBehaviour
{
    public Transform playerTransform;
    public Transform Audio;

    void Update()
    {
        if (playerTransform != null)
        {
            // Ustaw pozycjê AudioListenera na pozycji gracza
            Audio.transform.position = playerTransform.position;
        }
        else
        {
            Debug.LogWarning("Player transform not assigned in FollowPlayerAudioListener script.");
        }
    }
}