using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UIEngine.Extensions.Attribute;
using GIKCore.DB;

public class SV1000Control : MonoBehaviour
{
    // Fields    
    [Help("sv1000 == true ? Active : InActive")]
    [SerializeField] private List<GameObject> m_LstActive = new List<GameObject>();
    [Space]    
    [Help("sv1000 == true ? InActive : Active")]
    [SerializeField] private List<GameObject> m_LstInActive = new List<GameObject>();
    [SerializeField] private UnityEvent m_OnEnable;

    // Methods
    void Awake()
    {
        foreach (GameObject go in m_LstActive)
        {
            go.SetActive(Config.sv1000);
        }
        foreach (GameObject go in m_LstInActive)
        {
            go.SetActive(!Config.sv1000);
        }
    }

    // Start is called before the first frame update
    //void Start() { }

    // Update is called once per frame
    //void Update() { }

    void OnEnable()
    {
        if (m_OnEnable != null) m_OnEnable.Invoke();
    }
}
