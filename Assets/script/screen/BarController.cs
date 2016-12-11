//using SmartLocalization;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    #region variables
    public const string sceneName = "bar";

    /// <summary>Баристо оттдыхает</summary>
    public GameObject BaristoRelax;
    /// <summary>места для посадки клиентов</summary>
    public BarstoolController[] СlientList;
    /// <summary>Список всех спрайтов</summary>
    public Sprite[] GoodSprite;
    /// <summary>Анимация товара при клике</summary>
    public GameObject ItemPrefab;

    public Text LiveText;
    #endregion

    #region unity
    public void Awake()
    {
        CoreGame.Instance.StartBar();

        ShowStats();

        //for (var i = 0; i < CoreGame.Instance.SheepCount; i++) CreateSheep();

        //longhouseButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("winter_button");
    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
        //if (CoreGame.Instance.DayCount <= 0) return;

        if (CoreGame.Instance.GameWin)
        {
            BaristoRelax.SetActive(true);

            if (Input.GetMouseButtonDown(0)) RestartClick();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
                if (hit.transform != null)
                {
                    var itemButton = hit.transform.gameObject.GetComponent<ItemButton>();
                    if (itemButton != null) CheeseCakeClick(hit.point, itemButton);

                }
            }

            CoreGame.Instance.TurnTime(Time.deltaTime);
            ShowStats();

            BaristoRelax.SetActive(false);
        }
    }

    public void RestartClick()
    {
        CoreGame.Instance.RestartGame();
    }
    #endregion

    #region stuff
    private void ShowStats()
    {
        if (CoreGame.Instance==null) return;

        for (var i = 0; i < СlientList.Length; i++)
            СlientList[i].ShowState(this);

        LiveText.text = string.Format("{0}/{1}",CoreGame.Instance.ScoreCount, CoreGame.Instance.LiveCount);
    }

    private void CheeseCakeClick(Vector2 point, ItemButton clickItem)
    {
        var result = CoreGame.Instance.ClickItem(clickItem.ItemType);

        if (result)
        {
            var item = (GameObject) Instantiate(ItemPrefab, transform);
            item.transform.localPosition = new Vector3(point.x, point.y, -0.01f);
            item.GetComponentInChildren<SpriteRenderer>().sprite = GoodSprite[(int) clickItem.ItemType];
            Destroy(item, 3f);
        }
    }
    #endregion

    #region achievements

    /// <summary>большой улов</summary>
    private void BigFishAchievement(int fishing)
    {
        /*if (fishing < 3) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_big_fish)) return;

        Social.ReportProgress(GPGSIds.achievement_big_fish, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_big_fish, 100);
            }
        });*/
    }
    #endregion
}
