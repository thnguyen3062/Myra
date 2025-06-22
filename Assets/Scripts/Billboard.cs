using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private bool useStaticBillboard;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void ManagedLateUpdate()
    {
        if (!useStaticBillboard)
        {
            transform.LookAt(mainCamera.transform);
        }
        else
        {
            transform.rotation = Camera.main.transform.rotation;
        }

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    void OnSpawned()
    {
        BillboardUpdateHandler.instance.Register(this);
    }

    void OnDespawned()
    {
        BillboardUpdateHandler.instance.UnRegister(this);
    }
}
