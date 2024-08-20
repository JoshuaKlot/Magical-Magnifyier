using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DynamicAudio : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float maxDistance = 50f; // Maximum distance at which the audio will be completely silent
    public float minDistance = 5f; // Distance at which the audio is at its maximum volume
    public float maxVolume = 1f; // Maximum volume level of the audio source

    private AudioSource audioSource;

    void Start()
    {
        // Automatically find the player if it's not assigned in the inspector
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Get the audio source component attached to the object
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Calculate the distance between the player and the object
        float distance = Vector3.Distance(player.position, transform.position);

        // Adjust the volume based on the distance
        if (distance <= minDistance)
        {
            audioSource.volume = maxVolume;
        }
        else if (distance >= maxDistance)
        {
            audioSource.volume = 0f;
        }
        else
        {
            // Linear interpolation between minDistance and maxDistance
            float volume = Mathf.Lerp(maxVolume, 0f, (distance - minDistance) / (maxDistance - minDistance));
            audioSource.volume = volume;
        }
    }
}
