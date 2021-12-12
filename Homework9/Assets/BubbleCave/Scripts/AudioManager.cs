using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager Instance;

    [Header ("Audio Sources")]
    public AudioSource efxSource;
    public AudioSource musicSource;

    [Header ("Background Music")]
    public AudioClip bgMusic;

    [Header("Sound Effects")]
    public AudioClip buttonClick;
    public AudioClip bubbleStick;
    public AudioClip bubblePop;
    public AudioClip blackHole;
    public AudioClip fire;
    public AudioClip bestScore;
    public AudioClip gameOver;
    public AudioClip great;
   
    private bool muteMusic;
    private bool muteEfx;

	void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
	}

    //load saved sound preferences
	void Start ()
    {
        muteMusic = PlayerPrefs.GetInt("MuteMusic") == 1 ? true : false;
        muteEfx = PlayerPrefs.GetInt("MuteEfx") == 1 ? true : false;
    }
	
    //play background music clip
    public void PlayMusic(AudioClip clip)
    {
        if (muteMusic)
            return;

        musicSource.clip = clip;
        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    //stop background music
    private void StopMusic()
    {
        musicSource.Stop();
    }

    //play sound clip
    public void PlayEffects(AudioClip clip)
    {
        if (muteEfx)
            return;
        
        efxSource.PlayOneShot(clip);
    }

    //mute background music
    public void MuteMusic()
    {
        if (muteMusic)
        {
            muteMusic = false;
            PlayMusic(bgMusic);
            PlayerPrefs.SetInt("MuteMusic", 0);
        }
        else
        {
            muteMusic = true;
            StopMusic();
            PlayerPrefs.SetInt("MuteMusic", 1);
        }
    }

    //mute sound effects
    public void MuteEfx()
    {
        if (muteEfx)
            PlayerPrefs.SetInt("MuteEfx", 0);
        else
            PlayerPrefs.SetInt("MuteEfx", 1);

        muteEfx = !muteEfx;
    }

    public bool IsMusicMute()
    {
        return muteMusic;
    }

    public bool IsEfxMute()
    {
        return muteEfx;
    }
}
