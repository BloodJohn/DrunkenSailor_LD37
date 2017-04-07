using SmartLocalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TownController : MonoBehaviour
{
    public const string sceneName = "town";
    private const string tweetKey = "tweetCount";
    
    public Text ScoreText;

    private float _waitTime = 0;
    private const float _maxWaitTime = 4f;
    private int tweetCount;

    // Use this for initialization
    void Start()
    {
        tweetCount = PlayerPrefs.GetInt(tweetKey, 0);

        ScoreText.text = string.Format(LanguageManager.Instance.GetTextValue("town_successful"), 
            CoreGame.Instance.LevelName, CoreGame.Instance.NextLevelName);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();

        _waitTime += Time.deltaTime;

        if (Input.GetMouseButtonUp(0))
        {
            if (_waitTime < _maxWaitTime)
            {
                var index = (int)Random.Range(1, 39);
                ScoreText.text = LanguageManager.Instance.GetTextValue(string.Format("tweet_{0}", index));
                tweetCount++;
            }
            else
            {
                TweetAchievement();

                CoreGame.Instance.LevelIndex++;
                SceneManager.LoadScene(BarController.sceneName);
            }
        }
    }

    /// <summary>окончание первой недели</summary>
    private void TweetAchievement()
    {
        PlayerPrefs.SetInt(tweetKey, tweetCount);
        if (tweetCount < 10) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_10_tweets)) return;

        Social.ReportProgress(GPGSIds.achievement_10_tweets, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_10_tweets, 100);
            }
        });
    }
}