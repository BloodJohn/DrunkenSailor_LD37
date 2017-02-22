using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TownController : MonoBehaviour
{
    public const string sceneName = "town";
    
    public Text ScoreText;

    private float _waitTime = 0;
    private const float _maxWaitTime = 3f;

    // Use this for initialization
    void Start()
    {
        ScoreText.text = string.Format("Успешный {0}. Вечер с котом. Завтра будет {1}!", CoreGame.Instance.LevelName, CoreGame.Instance.NextLevelName);
    }

    // Update is called once per frame
    void Update()
    {
        _waitTime += Time.deltaTime;

        if (_waitTime < _maxWaitTime) return;

        if (Input.GetMouseButtonUp(0))
        {
            CoreGame.Instance.LevelIndex++;
            SceneManager.LoadScene(BarController.sceneName);
        }

        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
    }
}
