using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreGame : MonoBehaviour
{
    #region variables
    public static CoreGame Instance;
    /// <summary>Ключ куда мы сохраним игру</summary>
    public const string GameSaveKey = "gameSave";

    /// <summary>дней</summary>
    public float GameTime;

    private List<BarCustomer> customerList = new List<BarCustomer>();
    #endregion

    #region function

    #endregion

    #region constructor
    void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    public void RestartGame()
    {
        GameTime = 0f;
        SceneManager.LoadScene(BarController.sceneName);
    }

    public void Save()
    {
        var json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(GameSaveKey, json);
    }

    public void LoadGame()
    {
        var json = PlayerPrefs.GetString(GameSaveKey, string.Empty);

        if (string.IsNullOrEmpty(json))
        {
            RestartGame();
            return;
        }

        JsonUtility.FromJsonOverwrite(json, this);
        SceneManager.LoadScene(BarController.sceneName);
    }
    #endregion

    #region bar

    public void StartBar()
    {
        customerList.Clear();

        for (int i = 0; i < 5; i++)
        {
            var item = new BarCustomer(i)
            {
                start = i*5f,
                finish = i*5f + 10f
            };
            customerList.Add(item);

        }
    }

    public void TurnTime(float delta)
    {
        GameTime += delta;
    }

    public int HaylageBar()
    {
        return 0;
    }

    public BarCustomer GetCustomer(int index)
    {
        foreach (var item in customerList)
        {
            if (item.index == index && item.IsOnline) return item;
        }


        return null;
    }
    #endregion

    public class BarCustomer
    {
        public bool isView;
        public int index;
        public float start;
        public float finish;

        public BarCustomer(int number)
        {
            index = number;
            start = CoreGame.Instance.GameTime;
            finish = start + 10f;
            isView = false;
        }

        public bool IsOnline
        {
            get
            {
                var time = CoreGame.Instance.GameTime;
                return time > start && time < finish;
            }
        }
    }
}
