/*using GooglePlayGames;
using GooglePlayGames.BasicApi;
using SmartLocalization;*/
using UnityEngine;

public class LoadController : MonoBehaviour
{
    public const string sceneName = "load";
    
    void Awake()
    {
        /*if (Application.systemLanguage == SystemLanguage.Russian)
        {
            LanguageManager.Instance.ChangeLanguage("ru");
        }
        else
        {
            LanguageManager.Instance.ChangeLanguage("en");
        }

        LanguageManager.SetDontDestroyOnLoad();*/
    }

    void Start()
    {
        GooglePlayServices();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            CoreGame.Instance.LoadGame();
        }

        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
    }

    private void GooglePlayServices()
    {
#if UNITY_ANDROID
        /*Debug.LogFormat("GooglePlayServices");
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
        });*/
#endif
    }
}
