using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarstoolController : MonoBehaviour
{
    public int index;
    public GameObject lumberman;

    public void ShowState()
    {
        var item = CoreGame.Instance.GetCustomer(index);

        if (item == null)
            lumberman.SetActive(false);
        else
        {
            lumberman.SetActive(item.IsOnline);
            item.isView = item.IsOnline;
        }
    }
}
