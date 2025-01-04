using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    class Sound {
        public string name;
        public AudioClip audioClip;
        public float volume;
        public bool playOnAwake;
        public bool loop;
        [HideInInspector] public AudioSource audioSource;
    }

    [SerializeField] private List<Sound> sounds;

    public static SoundManager instance;

    private void OnEnable()
    {
        print("on enable");
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Awake() {
        if (instance != null)
        {
            Debug.LogWarning("Multiple SoundManager instances found. Destroying duplicate.", this);
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var sound in sounds)
        {
            var audioSource = this.AddComponent<AudioSource>();
            audioSource.clip = sound.audioClip;
            audioSource.volume = sound.volume;
            audioSource.playOnAwake = sound.playOnAwake;
            audioSource.loop = sound.loop;
            sound.audioSource = audioSource;
        }
    }

    public void Play(string name)
    {
        foreach (var sound in sounds)
        {
            if (sound.name == name)
            {
                sound.audioSource.Play();
            }
        }
    }

    /// <summary>
    /// Plays Sound with random pitch between minPitch (inclusive) and maxPitch (inclusive)
    /// </summary>
    public void Play(string name, float minPitch, float maxPitch)
    {
        foreach (var sound in sounds)
        {
            if (sound.name == name)
            {
                sound.audioSource.pitch = Random.Range(minPitch, maxPitch);
                sound.audioSource.Play();
            }
        }
    }
}