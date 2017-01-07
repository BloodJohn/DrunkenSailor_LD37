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
        CoffeBean1 = 1,
        CoffeBean2 = 2,
        CoffeBean3 = 3,
        CoffeBean4 = 4,
        Syrup1 = 5,
        Syrup2 = 6,
        Syrup3 = 7,
        Syrup4 = 8,
        Syrup5 = 9,
        Croissant = 10,
        Cupcake1 = 11,
        Cupcake2 = 12,
        Macaroon1 = 13,
        Macaroon2 = 14,
        Macaroon3 = 15,
        Macaroon4 = 16,
        Macaroon5 = 17,
        Sandwich = 18,
        MaxCount = 19
    }

    public enum CustomerType
    {
        LumberJack1 = 0,
        LumberJack2 = 1,
        LumberJack3 = 2,
        girl1 = 3,
        girl2 = 4,
        MaxCount = 5
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
    private static List<BarCustomer> oldCustomerList = new List<BarCustomer>();
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
        LiveCount = 3;
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

        var time = 0f;

        for (var i = 0; i < 10; i++)
        {
            AddNewCustomer(time, 10f);
            time += 3f;
        }

        time += 2f;

        for (var i = 0; i < 10; i++)
        {
            AddNewCustomer(time, 7f);
            time += 2f;
        }

        time += 3f;

        for (var i = 0; i < 5; i++)
        {
            AddNewCustomer(time, 6f);
            time += 2f;
        }

        for (var i = 0; i < 5; i++)
        {
            AddNewCustomer(time, 5f);
            time += 1.5f;
        }

        time += 3.5f;

        for (var i = 0; i < 5; i++)
        {
            AddNewCustomer(time, 4f);
            time += 1.5f;
        }

        for (var i = 0; i < 5; i++)
        {
            AddNewCustomer(time, 3f);
            time += 1f;
        }

        Debug.LogFormat("Последний посетитель: {0}", time);
    }

    public void TurnTime(float delta)
    {
        GameTime += delta;
    }

    /// <summary>взять вкусняшку и искать покупателя</summary>
    public bool ClickItem(GoodType itemType)
    {
        var findCustomer = false;

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

        return findCustomer;
    }

    /// <summary>слишком много ошибок!</summary>
    public bool GameLose { get { return LiveCount <= 0; } }

    /// <summary>Все клиенты ушли</summary>
    public bool GameWin
    {
        get
        {
            foreach (var customer in customerList)
                if (customer.finish >= GameTime)
                    return false;

            return true;
        }
    }

    #endregion

    #region Customer generator

    /// <summary>берем покупателя, который сейчас в баре</summary>
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


    /// <summary>добавляет нового покупателя в очередь</summary>
    private BarCustomer AddNewCustomer(float time, float delay)
    {
        CollectOldCustomers();
        var startIndex = Random.Range(0, 5);

        for (var i = 0; i < 5; i++) //пять раз пробуем пристроить нашего посетителя
        {
            var lastFinish = GetLastCustomer(startIndex) + 1f;

            if (lastFinish < time)
            {
                var wantItem = (GoodType)Random.Range((int)GoodType.CheeseCake, (int)GoodType.MaxCount);
                var visitorItem = (CustomerType)Random.Range((int)CustomerType.LumberJack1, (int)CustomerType.MaxCount);

                switch (visitorItem)
                {
                    case CustomerType.LumberJack1: //хипстер
                        break;
                    case CustomerType.LumberJack2: //мужик с топором
                        if (delay > 7f) visitorItem = CustomerType.LumberJack1;
                        break;
                    case CustomerType.LumberJack3: //мужик в шортах
                        if (delay > 3f) visitorItem = CustomerType.LumberJack1;
                        break;
                    case CustomerType.girl1: //девчонка в шортах
                        if (delay > 7f) visitorItem = CustomerType.LumberJack1;
                        break;
                    case CustomerType.girl2: //девчонка в очках и рубашке
                        if (delay > 5f) visitorItem = CustomerType.girl1;
                        break;
                }


                var item = new BarCustomer(startIndex, time, delay, wantItem, visitorItem);
                customerList.Add(item);
                return item;
            }
            else
            {
                startIndex++;
                if (startIndex >= 5) startIndex = 0;
            }
        }

        Debug.Log("Нет свободных стульев!");

        return null;
    }

    /// <summary>удаляет из очереди неактуальных покупателей</summary>
    private void CollectOldCustomers()
    {
        oldCustomerList.Clear();

        foreach (var item in customerList)
            if (!item.isView && item.finish < GameTime)
                oldCustomerList.Add(item);

        if (oldCustomerList.Count <= 0) return;

        foreach (var oldItem in oldCustomerList)
            customerList.Remove(oldItem);
    }

    /// <summary>ищет последнего покупателя на этом стуле</summary>
    private float GetLastCustomer(int index)
    {
        var result = float.MinValue;

        foreach (var customer in customerList)
            if (customer.index == index && customer.finish > result)
                result = customer.finish;

        return result;
    }

    /// <summary>модель посетителя в баре</summary>
    public class BarCustomer
    {
        public bool isView;
        public bool isSatisfied;
        public int index;
        public GoodType wantItem;
        public float start;
        public float finish;
        public CustomerType visitorItem;

        public BarCustomer(int number, float time, float delay, GoodType want, CustomerType visitor)
        {
            index = number;
            wantItem = want;
            start = time;
            finish = start + delay;
            isView = false;
            isSatisfied = false;
            visitorItem = visitor;
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

    #endregion
}
