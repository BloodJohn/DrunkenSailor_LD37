using UnityEngine;

public class BarstoolController : MonoBehaviour
{
    public int index;
    public GameObject lumberman;
    public GameObject wantItem;

    public void ShowState()
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
    }
}
