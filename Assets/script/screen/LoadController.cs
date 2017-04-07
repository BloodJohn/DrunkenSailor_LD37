/*using GooglePlayGames;
using GooglePlayGames.BasicApi;*/

using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using SmartLocalization;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadController : MonoBehaviour
{
    public const string sceneName = "load";
    public Text hintText;
    public Text authText;

    public Sprite sountOn;
    public Sprite sountOff;
    public Image soundImage;

    void Awake()
    {
        LanguageManager.Instance.ChangeLanguage(Application.systemLanguage == SystemLanguage.Russian ? "ru" : "en");

        LanguageManager.SetDontDestroyOnLoad();
        Random.InitState(DateTime.Now.Millisecond);

        hintText.text = LanguageManager.Instance.GetTextValue("intro_hint");
        authText.text = LanguageManager.Instance.GetTextValue("intro_auth");

        soundImage.sprite = SoundManager.Instance.IsSound ? sountOn : sountOff;
    }

    void Start()
    {
        GooglePlayServices();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
    }

    public void ClickBackground()
    {
        CoreGame.Instance.LoadGame();
    }

    public void ClickSound()
    {
        SoundManager.Instance.MuteSound();
        soundImage.sprite = SoundManager.Instance.IsSound ? sountOn : sountOff;
    }

    private void GooglePlayServices()
    {
#if UNITY_ANDROID
        Debug.LogFormat("GooglePlayServices");
        var config = new PlayGamesClientConfiguration.Builder()
        // enables saving game progress.
        //.EnableSavedGames()
        // registers a callback to handle game invitations received while the game is not running.
        //.WithInvitationDelegate(< callback method >)
        // registers a callback for turn based match notifications received while the
        // game is not running.
        //.WithMatchDelegate(< callback method >)
        // require access to a player's Google+ social graph (usually not needed)
        //.RequireGooglePlus()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = false;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();


        // authenticate user:
        Social.localUser.Authenticate((bool success) =>
        {
            // handle success or failure
            if (!success)
            {
                //пишем для отладки, потом надо убрать.
                //author.text = "Social.localUser.Authenticate - failed";
            }
        });
#endif
    }
}
