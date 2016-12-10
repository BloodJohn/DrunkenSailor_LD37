//using SmartLocalization;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BarController : MonoBehaviour
{
    #region variables
    public const string sceneName = "bar";

    /// <summary>места для посадки клиентов</summary>
    public BarstoolController[] clientList;

    public Text liveText;
    #endregion

    #region unity
    public void Awake()
    {
        CoreGame.Instance.StartBar();

        ShowStats();

        Random.InitState(DateTime.Now.Second);
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

        for (var i = 0; i < clientList.Length; i++)
            clientList[i].ShowState();

        liveText.text = string.Format("{0}/{1}",CoreGame.Instance.ScoreCount, CoreGame.Instance.LiveCount);
    }

    private void CheeseCakeClick(Vector2 point, ItemButton clickItem)
    {
        Debug.LogFormat("CheeseCakeClick [{0},{1}]", point.x, point.y);
        var result = CoreGame.Instance.ClickItem(clickItem.ItemType);

        if (result)
        {
            var item = (GameObject) Instantiate(clickItem.ItemPrefab, transform);
            item.transform.localPosition = new Vector3(point.x, point.y, -0.01f);
            Destroy(item, 3f);
        }
    }

    /*private void CreateSheep()
    {
        var cnt = 0;
        var height = Camera.allCameras[0].orthographicSize;
        var width = height * Camera.allCameras[0].aspect * height;

        while (cnt < 100)
        {
            cnt++;
            var point = new Vector2(Random.Range(-width, width), Random.Range(-height, height));

            var hit = Physics2D.Raycast(point, Vector2.zero);
            if (hit.transform != null)
            {
                if (hit.transform.name.Contains("land"))
                {
                    var item = (GameObject)Instantiate(lumbermanPrefab, transform);
                    item.transform.position = new Vector3(point.x, point.y, 0f);
                    return;
                }
            }
        }
    }*/
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
