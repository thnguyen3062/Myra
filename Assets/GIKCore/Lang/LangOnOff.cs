using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GIKCore.UI;

namespace GIKCore.Lang
{
    public class LangOnOff : MonoBehaviour
    {
        // Fields
        [SerializeField] [Tooltip("Viernamese")] private ButtonOnOff m_ButtonVI;
        [SerializeField] [Tooltip("English")] private ButtonOnOff m_ButtonEN;

        // Methods
        public void DoClick(int type)
        {
            //if (type != LangHandler.lastType)
            //    Game.main.socket.MultiLanguage(type);#comment
            DoSet(type);
        }

        private void DoSet(int type)
        {
            //m_ButtonVI.Turn(type == LangData.TYPE_VI);
            m_ButtonEN.Turn(type == LangData.TYPE_EN);
        }

        // Start is called before the first frame update
        void Awake()
        {
            //m_ButtonVI.SetOnClickCallback(x => { DoClick(LangData.TYPE_VI); });
            m_ButtonEN.SetOnClickCallback(x => { DoClick(LangData.TYPE_EN); });

            DoSet(LangHandler.lastType);
        }
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}
