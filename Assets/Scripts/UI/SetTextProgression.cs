using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetTextProgression : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    // Start is called before the first frame update
    public void SetData(string a)
    {
        m_Text.text = a;
    }    
}
