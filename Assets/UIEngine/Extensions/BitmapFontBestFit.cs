using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIEngine.Extensions
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(Text))]
    public class BitmapFontBestFit : MonoBehaviour
    {
        // Fields
        [SerializeField] private Text m_Target;
        [SerializeField] [Min(1)] private int m_Min = 15;
        [SerializeField] [Min(1)] private int m_Max = 30;

        // Values
        private string lastText = "";

        void Awake()
        {
            if (m_Target == null) m_Target = GetComponent<Text>();
        }
        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        void Update()
        {
            if (!lastText.Equals(m_Target.text))
            {
                lastText = m_Target.text;
                float wRect = m_Target.rectTransform.rect.width;
                float wText = m_Target.preferredWidth;
                if (wText > 0)
                {
                    float scale = wRect / wText;
                    int fontSize = Mathf.FloorToInt(m_Target.fontSize * scale);                    
                    if (fontSize < m_Min) fontSize = m_Min;
                    if (fontSize > m_Max) fontSize = m_Max;
                    if (fontSize != m_Target.fontSize)
                        m_Target.fontSize = fontSize;
                }
            }
        }
    }
}
