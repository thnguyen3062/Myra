using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIEngine.Extensions
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(Text))]    
    public class Unicode : MonoBehaviour
    {
        // Fields
        [SerializeField] private Text m_Text;
        [Tooltip("with unicode number: u+25b2 => unicode point: 25b2")]
        [SerializeField] private string m_UnicodePoint;

        // Methods
        /// <summary>
        /// param is point of an unicode character, ex: with unicode number: u+25b2 => unicode point: 25b2
        /// </summary>
        /// <param name="unicodePoint"></param>
        /// <returns></returns>
        public int ParseToUTF32(string unicodePoint)
        {
            return int.Parse(unicodePoint, System.Globalization.NumberStyles.HexNumber);
        }

        public string ConvertFromUTF32(int utf32)
        {
            return char.ConvertFromUtf32(utf32);
        }

        /// <summary>
        /// param is an unicode character with format like '\u25b2'
        /// </summary>
        /// <param name="unicodeNumber"></param>
        public void SetUnicode(char unicodeNumber)
        {
            m_Text.text = unicodeNumber.ToString();
        }

        public void SetCodePoint(string unicodePoint)
        {
            int utf32 = ParseToUTF32(unicodePoint);
            string unicodeString = ConvertFromUTF32(utf32);
            m_Text.text = unicodeString;
        }

        // Use this for initialization
        void Awake()
        {
            if (m_Text == null) m_Text = GetComponent<Text>();
            SetCodePoint(m_UnicodePoint);
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
