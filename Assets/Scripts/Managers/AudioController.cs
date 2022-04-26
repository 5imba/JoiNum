using UnityEngine;

public enum Sound { Click, Post, Combine, Super, Achieve }

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    [Header("Click, Post, Combine, Super, Achieve")]
    [SerializeField] private AudioClip[] sounds;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Messenger<Sound>.AddListener(GameEvent.PLAY_SOUND, PlaySound);
    }

    private void OnDestroy()
    {
        Messenger<Sound>.RemoveListener(GameEvent.PLAY_SOUND, PlaySound);
    }

    public void PlaySound(Sound sound)
    {
        if (PlayerData.Sound)
        {
            audioSource.PlayOneShot(sounds[(int)sound]);
        }
    }
}