using UnityEngine;

/// <summary>
/// Handles audio in the scene. Currently only plays an audio clip when the player eats the food.
/// Is a Singleton.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ClientMusicPlayer : Singleton<ClientMusicPlayer>
{
    [SerializeField] private AudioClip nomAudioClip;

    private AudioSource _audioSource;

    public override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayNomAudioClip()
    {
        _audioSource.clip = nomAudioClip;
        _audioSource.Play();
    }
}
