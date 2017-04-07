//using SmartLocalization;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    #region variables
    public const string sceneName = "bar";

    /// <summary>Баристо за стойкой Gameplay</summary>
    public GameObject BaristoWork;
    /// <summary>Баристо оттдыхает Win</summary>
    public GameObject BaristoRelax;
    /// <summary>Баристо грустит Lose</summary>
    public GameObject BaristoDepressed;
    /// <summary>места для посадки клиентов</summary>
    public BarstoolController[] СlientList;
    /// <summary>Список всех спрайтов товаров</summary>
    public Sprite[] GoodSprite;
    public Sprite VrongSprite;
    /// <summary>Список всех префабов клиентов</summary>
    public GameObject[] CustomerPrefab;
    /// <summary>Анимация товара при клике</summary>
    public GameObject ItemPrefab;


    /// <summary>жизни</summary>
    public GameObject[] liveList;
    /// <summary>потерянные жизни</summary>
    public GameObject[] liveoffList;

    /// <summary>счетчик клиентов</summary>
    public Text LiveText;

    private float _waitTime = float.MinValue;
    private const float _maxWaitTime = 2f;


    private const string gameKey = "gameCount";
    private int gameCount;
    private const string customerKey = "customerCount";
    private int customerCount;
    #endregion

    #region unity
    public void Awake()
    {
        CoreGame.Instance.StartBar();

        ShowStats();
    }

    public void Start()
    {
        gameCount = PlayerPrefs.GetInt(gameKey, 0);
        customerCount = PlayerPrefs.GetInt(customerKey, 0);
        StartGameAchievement();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
        if (_waitTime >= 0f)
        {
            _waitTime += Time.deltaTime;
            if (_waitTime < _maxWaitTime) return;
            WaitRestart();
            return;
        }
        if (CoreGame.Instance.GameWin)
        {
            ShowWin();
            return;
        }
        if (CoreGame.Instance.GameLose)
        {
            ShowLose();
            return;
        }

        if (Input.touchSupported)
        {
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began) //|| touch.phase == TouchPhase.Moved
                {
                    Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
                    CheckMouseClick(mouseWorldPos);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CheckMouseClick(mouseWorldPos);
            }
        }

        ShowGame();

    }
    #endregion

    #region stuff
    private void ShowStats()
    {
        if (CoreGame.Instance == null) return;

        for (var i = 0; i < СlientList.Length; i++)
            СlientList[i].ShowState(this);

        LiveText.text = string.Format("{0}", CoreGame.Instance.ScoreCount);

        for (var i = 0; i < liveList.Length; i++)
        {
            var fullLive = CoreGame.Instance.LiveCount > i;

            liveList[i].SetActive(fullLive);
            liveoffList[i].SetActive(!fullLive);
        }
    }

    private void CheeseCakeClick(Vector2 point, ItemButton clickItem)
    {
        var result = CoreGame.Instance.ClickItem(clickItem.ItemType);

        if (result)
        {
            var item = (GameObject)Instantiate(ItemPrefab, transform);
            item.transform.localPosition = new Vector3(point.x, point.y, -0.01f);
            item.GetComponentInChildren<SpriteRenderer>().sprite = GoodSprite[(int)clickItem.ItemType];
            Destroy(item, 3f);

            CustomerAchievement();
        }
        else
        {
            var item = (GameObject)Instantiate(ItemPrefab, transform);
            item.transform.localPosition = new Vector3(point.x, point.y, -0.01f);
            item.GetComponentInChildren<SpriteRenderer>().sprite = VrongSprite;
            Destroy(item, 3f);
        }
    }

    private void ShowWin()
    {
        BaristoRelax.SetActive(true);
        BaristoDepressed.SetActive(false);
        BaristoWork.SetActive(false);

        foreach (var client in СlientList) client.Hide();
        _waitTime = 0f;

        FirstWeekAchievement();
    }

    private void ShowLose()
    {
        BaristoDepressed.SetActive(true);
        BaristoRelax.SetActive(false);
        BaristoWork.SetActive(false);

        foreach (var client in СlientList) client.Hide();
        _waitTime = 0f;

        FirstGameAchievement();
    }

    private void ShowGame()
    {
        CoreGame.Instance.TurnTime(Time.deltaTime);
        ShowStats();

        BaristoWork.SetActive(true);
        BaristoRelax.SetActive(false);
        BaristoDepressed.SetActive(false);
    }

    private void WaitRestart()
    {
        if (Input.touchSupported)
        {
            foreach (var touch in Input.touches)
                if (touch.phase == TouchPhase.Began)
                    Restart();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                Restart();
        }
    }

    private void Restart()
    {
        if (CoreGame.Instance.GameLose)
        {
            CoreGame.Instance.GameOver();
        }
        else
        {
            SceneManager.LoadScene(TownController.sceneName);
        }
    }

    private void CheckMouseClick(Vector2 mouseWorldPos)
    {
        var hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        if (hit.transform != null)
        {
            var itemButton = hit.transform.gameObject.GetComponent<ItemButton>();
            if (itemButton != null) CheeseCakeClick(hit.point, itemButton);

        }
    }
    #endregion

    #region achievements
    /// <summary>окончание первой игры</summary>
    private void FirstGameAchievement()
    {
        if (PlayerPrefs.HasKey(GPGSIds.achievement_first_game)) return;

        Social.ReportProgress(GPGSIds.achievement_first_game, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_first_game, 100);
            }
        });
    }

    /// <summary>окончание первой недели</summary>
    private void FirstWeekAchievement()
    {
        if (CoreGame.Instance.LevelIndex<6) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_first_weekend)) return;

        Social.ReportProgress(GPGSIds.achievement_first_weekend, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_first_weekend, 100);
            }
        });
    }

    /// <summary>начало 10 игры</summary>
    private void StartGameAchievement()
    {
        if (CoreGame.Instance.LevelIndex>0) return;
        gameCount++;
        PlayerPrefs.SetInt(gameKey,gameCount);
        if (gameCount<10) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_10_games)) return;

        Social.ReportProgress(GPGSIds.achievement_10_games, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_10_games, 100);
            }
        });
    }

    /// <summary>10 посетителей</summary>
    private void CustomerAchievement()
    {
        customerCount++;
        PlayerPrefs.SetInt(customerKey, customerCount);
        if (customerCount < 10) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_10_customer)) return;

        Social.ReportProgress(GPGSIds.achievement_10_customer, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_10_customer, 100);
            }
        });
    }
    #endregion
}
