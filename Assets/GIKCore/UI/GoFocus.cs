using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIKCore.UI
{
    public class GoFocus : MonoBehaviour
    {
        // Fields
        [SerializeField] private GameObject m_GoFocus;
        [SerializeField] private string m_Id;

        // Methods
        public string id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        public int idInt
        {
            get
            {
                int r = -1;
                int.TryParse(id, out r);
                return r;
            }
        }

        public void SetActive(bool active) { gameObject.SetActive(active); }
        public void Focus() { m_GoFocus.SetActive(true); }
        public void Blur() { m_GoFocus.SetActive(false); }
        public void Set(bool focus)
        {
            if (focus) Focus();
            else Blur();
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}