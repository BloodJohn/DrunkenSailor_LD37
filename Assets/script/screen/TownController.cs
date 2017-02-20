using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TownController : MonoBehaviour
{
    public const string sceneName = "town";
    
    public Text ScoreText;

    public string[] LevelNameList;

    // Use this for initialization
    void Start()
    {
        ScoreText.text = string.Format("{0} позади. Вечер с котом. Завтра будет {1}!", CoreGame.Instance.LevelName, CoreGame.Instance.NextLevelName);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            CoreGame.Instance.LevelIndex++;
            SceneManager.LoadScene(BarController.sceneName);
        }

        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
    }
}
