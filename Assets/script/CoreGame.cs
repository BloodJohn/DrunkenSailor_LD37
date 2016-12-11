using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CoreGame : MonoBehaviour
{
    #region const
    /// <summary>Ключ куда мы сохраним игру</summary>
    public const string GameSaveKey = "gameSave";
    /// <summary>типы продуктов в нашем</summary>
    public enum GoodType
    {
        CheeseCake = 0,
        CoffeBean = 1,
    }
    #endregion

    #region variables
    public static CoreGame Instance;

    /// <summary>время в игре</summary>
    public float GameTime;
    /// <summary>сколько попыток</summary>
    public int LiveCount;
    /// <summary>сколько довольных клиентов</summary>
    public int ScoreCount;

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
        ScoreCount = 0;
        LiveCount = 10;
        SceneManager.LoadScene(BarController.sceneName);
        Random.InitState(DateTime.Now.Second);
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
            var wantItem = Random.Range((int)GoodType.CheeseCake, (int)GoodType.CoffeBean+1);

            var item = new BarCustomer(i, (GoodType)wantItem)
            {
                start = i * 5f,
                finish = i * 5f + 10f
            };
            customerList.Add(item);

        }
    }

    public void TurnTime(float delta)
    {
        GameTime += delta;
    }

    public BarCustomer GetCustomer(int index)
    {
        foreach (var item in customerList)
            if (item.index == index)
            {
                if (item.IsOnline) return item;
                if (item.isView) return item;
            }

        return null;
    }

    /// <summary>взять вкусняшку и искать покупателя</summary>
    public bool ClickItem(GoodType itemType)
    {
        var findCustomer=false;

        foreach (var item in customerList)
            if (item.isView && !item.isSatisfied && item.wantItem == itemType)
            {
                item.finish = Mathf.Min(GameTime + 0.5f, item.finish);
                item.isSatisfied = true;
                ScoreCount++;
                findCustomer = true;
                break;
            }

        if (!findCustomer) LiveCount--;

        return true;
    }

    #endregion

    /// <summary>модель посетителя в баре</summary>
    public class BarCustomer
    {
        public bool isView;
        public bool isSatisfied;
        public int index;
        public GoodType wantItem;
        public float start;
        public float finish;

        public BarCustomer(int number, GoodType want)
        {
            index = number;
            wantItem = want;
            start = CoreGame.Instance.GameTime;
            finish = start + 10f;
            isView = false;
            isSatisfied = false;
        }

        public bool IsOnline
        {
            get
            {
                var time = CoreGame.Instance.GameTime;
                return time > start && time < finish;
            }
        }

        public void UpdateStatus()
        {
            var newStatus = IsOnline;
            if (newStatus == isView) return;
            //отнимаем очки если покупатель не дождался заказа
            if (!newStatus && isView && !isSatisfied)
                CoreGame.Instance.LiveCount--;

            isView = IsOnline;
        }
    }
}
