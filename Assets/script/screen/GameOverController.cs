
using SmartLocalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public const string sceneName = "gameover";
    public const string storeURL = "https://play.google.com/store/apps/details?id=com.StarAge.FlappyBeard";
    public const string voteCountKey = "vote";

    public Text ScoreText;
    private float _waitTime = 0;
    private const float _maxWaitTime = 2f;
    private bool needVote = false;

    private void Awake()
    {
        var vote = PlayerPrefs.GetInt(voteCountKey, 0 );

        if (vote == 0 && CoreGame.Instance.LevelIndex >= 6) 
        {
            ScoreText.text = LanguageManager.Instance.GetTextValue("gameover_vote");
            needVote = true;
        }
        else
        {
            ScoreText.text = LanguageManager.Instance.GetTextValue("gameover_restart");
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();

        _waitTime += Time.deltaTime;

        if (Input.GetMouseButtonUp(0) && _waitTime > _maxWaitTime)
        {
            if (needVote)
            {
                VoteClick();
            }
            else
            {
                CoreGame.Instance.RestartGame();
            }
        }
    }

    private void VoteClick()
    {
        Application.OpenURL(storeURL);
        PlayerPrefs.SetInt(voteCountKey, 1);
        ScoreText.text = LanguageManager.Instance.GetTextValue("gameover_restart");
        needVote = false;
    }

    /*private void LeaderBoard()
    {
        var oldResult = PlayerPrefs.GetInt(GPGSIds.leaderboard_woolen_fabric, 0);
        if (CoreGame.Instance.TotalFelted <= oldResult) return;

        Social.ReportScore(CoreGame.Instance.TotalFelted, GPGSIds.leaderboard_woolen_fabric,
            (bool success) =>
            {
                if (success)
                {
                    PlayerPrefs.SetInt(GPGSIds.leaderboard_woolen_fabric, CoreGame.Instance.TotalFelted);
                    Social.ShowLeaderboardUI();
                }
            });
    }*/
}
