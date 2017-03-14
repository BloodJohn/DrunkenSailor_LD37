using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public bool IsSound;

    [SerializeField]
    private GameObject Music;
    /// <summary>Фонт для всех текстовых полей в игре</summary>
    /*[SerializeField]
    private Font TextFont;*/

    private static readonly string SoundKey = "muteSound";

    void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;

        Application.targetFrameRate = 10;

        IsSound = PlayerPrefs.GetInt(SoundKey, 100) > 0;
    }

    /*public void SetFontScene()
    {
        foreach (var item in GameObject.FindObjectsOfType<Text>())
        {
            item.font = TextFont;
        }
    }*/

    public void MuteSound()
    {
        if (!IsSound)
        {
            IsSound = true;
            PlayMusic();
        }
        else
        {
            IsSound = false;
            StopSound();
        }

        PlayerPrefs.SetInt(SoundKey, IsSound ? 100 : 0);
    }

    public void StopSound()
    {
        Music.SetActive(false);
    }

    public void PlayMusic()
    {
        if (!IsSound) return;
        Music.SetActive(true);
    }
}
