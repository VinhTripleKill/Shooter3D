using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ThrowSFX : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Initialize(AudioClip clip)
    {
        if (clip == null)
        {
            Destroy(gameObject);
            return;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        audioSource.clip = clip;
        audioSource.Play();

        Destroy(
            gameObject,
            clip.length
        );
    }
}