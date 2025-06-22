using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateExample : MonoBehaviour
{
    public GameObject parent;
    public GameObject myObject1;
    public GameObject myObject2;
    public GameObject myObject3;
    public GameObject myObject4;
    public GameObject myObject5;
    GameObject myObjectClone1;
    GameObject myObjectClone2;
    GameObject myObjectClone3;
    GameObject myObjectClone4;
    GameObject myObjectClone5;
    
    void Update()
    {
        if(Input.GetKeyDown("a"))
        {
            myObjectClone1 = Instantiate(myObject1, parent.transform) as GameObject;
            Destroy(myObjectClone1,6);
        }

        if(Input.GetKeyDown("s"))
        {
            myObjectClone2 = Instantiate(myObject2, parent.transform) as GameObject;
            Destroy(myObjectClone2,6);
        }

        if(Input.GetKeyDown("d"))
        {
            myObjectClone3 = Instantiate(myObject3, parent.transform) as GameObject;
            Destroy(myObjectClone3,6);
        }

        if(Input.GetKeyDown("f"))
        {
            myObjectClone4 = Instantiate(myObject4, parent.transform) as GameObject;
            Destroy(myObjectClone4,6);
        }

        if(Input.GetKeyDown("g"))
        {
            myObjectClone5 = Instantiate(myObject5, parent.transform) as GameObject;
            Destroy(myObjectClone5,6);
        }
    }
}
