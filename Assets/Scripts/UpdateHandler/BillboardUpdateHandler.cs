using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardUpdateHandler : MonoBehaviour
{
    public static BillboardUpdateHandler instance;
    [HideInInspector] public List<Billboard> lstBillboard = new List<Billboard>();

    private void Awake()
    {
        instance = this;
    }

    private void LateUpdate()
    {
        lstBillboard.ForEach(b =>
        {
            b.ManagedLateUpdate();
        });
    }

    public void Register(Billboard bill)
    {
        lstBillboard.Add(bill);
    }

    public void UnRegister(Billboard bill)
    {
        lstBillboard.Remove(bill);
    }
}
