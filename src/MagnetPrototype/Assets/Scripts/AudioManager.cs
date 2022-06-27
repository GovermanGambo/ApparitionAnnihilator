using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField][Range(0, 1)] private float musicVolume;
    [SerializeField] private bool autoplay = true;
    [Header("Sound")]
    [SerializeField] private AudioRegistry audioRegistry;
    [SerializeField][Range(0, 1)] private float soundVolume;

    public float MusicVolume
    {
        get => musicSource.volume;
        set => musicSource.volume = musicVolume * value;
    }
    
    public float SoundVolume
    {
        get => soundSource.volume;
        set => soundSource.volume = soundVolume * value;
    }
    
    private AudioSource musicSource;
    private AudioSource soundSource;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Tried to create a second instance of AudioManager, which is a singleton.");
            return;
        }

        Instance = this;
        
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        soundSource = gameObject.AddComponent<AudioSource>();
        soundSource.volume = soundVolume;
    }

    private void Start()
    {
        if (!autoplay)
        {
            return;
        }
        
        musicSource.clip = backgroundMusic;
        StartMusic();
    }

    public void StartMusic()
    {
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySound(string name, float volume = 1.0f)
    {
        AudioClip clip = null;

        foreach (var sound in audioRegistry)
        {
            if (sound.Name == name)
            {
                clip = sound.Clip;
            }
        }

        if (clip == null)
        {
            Debug.LogWarning($"No sound with name {name} was found.");
            return;
        }
        
        soundSource.PlayOneShot(clip, volume);
    }
}

