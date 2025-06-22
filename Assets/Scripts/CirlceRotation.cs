using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirlceRotation : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotateSpeed));
    }
}
