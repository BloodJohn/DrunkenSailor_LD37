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
        Girl1 = 3,
        Girl2 = 4,
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
    /// <summary>текущий уровень игры</summary>
    public int LevelIndex;

    private List<BarCustomer> _customerList = new List<BarCustomer>();
    private static List<BarCustomer> _oldCustomerList = new List<BarCustomer>();
    /// <summary>Уровни</summary>
    public LevelData[] Levels;
    /// <summary>название текущего уровня</summary>
    public string LevelName { get { return Levels[LevelIndex].name; } }
    /// <summary>название следующего уровня</summary>
    public string NextLevelName
    {
        get
        {
            if (LevelIndex+1 >= Levels.Length)
                return Levels[Levels.Length-1].name;

            return Levels[LevelIndex+1].name;
        }
    }
    #endregion

    #region constructor
    void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    public void RestartGame()
    {
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
        if (LevelIndex < 0) LevelIndex = 0;
        if (LevelIndex >= Levels.Length) LevelIndex = Levels.Length - 1;

        GameTime = 0f;
        ScoreCount = 0;
        LiveCount = 3;

        LoadLevel(LevelIndex);
    }

    public void TurnTime(float delta)
    {
        GameTime += delta;
    }

    /// <summary>взять вкусняшку и искать покупателя</summary>
    public bool ClickItem(GoodType itemType)
    {
        var findCustomer = false;

        foreach (var item in _customerList)
            if (item.IsView && !item.IsSatisfied && item.WantItem == itemType)
            {
                item.Finish = Mathf.Min(GameTime + 0.5f, item.Finish);
                item.IsSatisfied = true;
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
            foreach (var customer in _customerList)
                if (customer.Finish >= GameTime)
                    return false;

            return true;
        }
    }

    #endregion

    #region Customer generator

    /// <summary>берем покупателя, который сейчас в баре</summary>
    public BarCustomer GetCustomer(int index)
    {
        foreach (var item in _customerList)
            if (item.Index == index)
            {
                if (item.IsOnline) return item;
                if (item.IsView) return item;
            }

        return null;
    }

    /// <summary>Создаем всех посетителей для указанного уровня</summary>
    private void LoadLevel(int index)
    {
        var level = Levels[index];

        _customerList.Clear();
        var time = 0f;

        foreach (var group in level.GroupList)
        {
             time += group.StartPause;

            for (var i = 0; i < group.Quantity; i++)
            {
                AddNewCustomer(time, group.WaitTime);
                time += group.NextTime;
            }
        }

        Debug.LogFormat("Последний посетитель: {0}", time);
    }

    /// <summary>добавляет нового покупателя в очередь</summary>
    private BarCustomer AddNewCustomer(float time, float waitTime)
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
                        if (waitTime > 7f) visitorItem = CustomerType.LumberJack1;
                        break;
                    case CustomerType.LumberJack3: //мужик в шортах
                        if (waitTime > 3f) visitorItem = CustomerType.LumberJack1;
                        break;
                    case CustomerType.Girl1: //девчонка в шортах
                        if (waitTime > 7f) visitorItem = CustomerType.LumberJack1;
                        break;
                    case CustomerType.Girl2: //девчонка в очках и рубашке
                        if (waitTime > 5f) visitorItem = CustomerType.Girl1;
                        break;
                }


                var item = new BarCustomer(startIndex, time, waitTime, wantItem, visitorItem);
                _customerList.Add(item);
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
        _oldCustomerList.Clear();

        foreach (var item in _customerList)
            if (!item.IsView && item.Finish < GameTime)
                _oldCustomerList.Add(item);

        if (_oldCustomerList.Count <= 0) return;

        foreach (var oldItem in _oldCustomerList)
            _customerList.Remove(oldItem);
    }

    /// <summary>ищет последнего покупателя на этом стуле</summary>
    private float GetLastCustomer(int index)
    {
        var result = float.MinValue;

        foreach (var customer in _customerList)
            if (customer.Index == index && customer.Finish > result)
                result = customer.Finish;

        return result;
    }

    /// <summary>модель посетителя в баре</summary>
    public class BarCustomer
    {
        public bool IsView;
        public bool IsSatisfied;
        public int Index;
        public GoodType WantItem;
        public float Start;
        public float Finish;
        public CustomerType VisitorItem;

        public BarCustomer(int number, float time, float delay, GoodType want, CustomerType visitor)
        {
            Index = number;
            WantItem = want;
            Start = time;
            Finish = Start + delay;
            IsView = false;
            IsSatisfied = false;
            VisitorItem = visitor;
        }

        public bool IsOnline
        {
            get
            {
                var time = CoreGame.Instance.GameTime;
                return time > Start && time < Finish;
            }
        }

        public void UpdateStatus()
        {
            var newStatus = IsOnline;
            if (newStatus == IsView) return;
            //отнимаем очки если покупатель не дождался заказа
            if (!newStatus && IsView && !IsSatisfied)
                CoreGame.Instance.LiveCount--;

            IsView = IsOnline;
        }
    }

    [Serializable]
    public struct LevelData
    {
        public string name;
        public GroupData[] GroupList;

        [Serializable]
        public struct GroupData
        {
            public float StartPause;
            public int Quantity;
            public float WaitTime;
            public float NextTime;
        }
    }

    #endregion
}
