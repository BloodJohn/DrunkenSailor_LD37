using SmartLocalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TownController : MonoBehaviour
{
    public const string sceneName = "town";
    
    public Text ScoreText;

    private float _waitTime = 0;
    private const float _maxWaitTime = 4f;

    // Use this for initialization
    void Start()
    {
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
            }
            else
            {
                CoreGame.Instance.LevelIndex++;
                SceneManager.LoadScene(BarController.sceneName);
            }
        }
    }
}