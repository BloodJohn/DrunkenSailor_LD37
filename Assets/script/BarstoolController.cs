using UnityEngine;

public class BarstoolController : MonoBehaviour
{
    private int oldIndex = -1;
    public int index;
    public GameObject lumberman;
    public GameObject wantItem;

    public void ShowState(BarController parent)
    {
        var item = CoreGame.Instance.GetCustomer(index);

        if (item == null)
        {
            lumberman.SetActive(false);
        }
        else
        {
            ChangeCustormer(parent, item);

            //включаем или отключаем показ дровосека
            lumberman.SetActive(item.IsOnline);
            item.UpdateStatus();
        }

        wantItem.SetActive(lumberman.activeSelf);

        if (wantItem.activeSelf)
        {
            wantItem.GetComponentInChildren<SpriteRenderer>().sprite = parent.GoodSprite[(int) item.WantItem];
        }
    }

    private void ChangeCustormer(BarController parent, CoreGame.BarCustomer item)
    {
        if (oldIndex == index) return;
        
        if (lumberman != null) Destroy(lumberman);

        var prefab = parent.CustomerPrefab[(int) item.VisitorItem];

        lumberman = Instantiate(prefab);
        lumberman.transform.parent = transform;
        lumberman.transform.localPosition = Vector3.zero;
    }

    public void Hide()
    {
        wantItem.SetActive(false);
        lumberman.SetActive(false);
    }
}