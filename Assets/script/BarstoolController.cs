using UnityEngine;

public class BarstoolController : MonoBehaviour
{
    public int index;
    public GameObject lumberman;
    public GameObject wantItem;
    public GameObject lumbermanTop;
    public GameObject lumbermanBottom;

    public void ShowState(BarController parent)
    {
        var item = CoreGame.Instance.GetCustomer(index);

        if (item == null)
        {
            lumberman.SetActive(false);
        }
        else
        {
            //включаем или отключаем показ дровосека
            lumberman.SetActive(item.IsOnline);
            item.UpdateStatus();
        }

        wantItem.SetActive(lumberman.activeSelf);

        if (wantItem.activeSelf)
        {
            wantItem.GetComponentInChildren<SpriteRenderer>().sprite = parent.GoodSprite[(int) item.wantItem];
        }
    }

    public void Hide()
    {
        wantItem.SetActive(false);
        lumberman.SetActive(false);
    }
}