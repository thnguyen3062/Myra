using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GIKCore.Utilities;
using System;

namespace GIKCore.UI
{
    public class Toast : MonoBehaviour
    {
        // Fields    
        [SerializeField] private RectTransform m_RectBox;
        [SerializeField] private LayoutElement m_LayoutElm;
        [SerializeField] private TextMeshProUGUI m_TxtContent;
        [SerializeField] private Animator m_Anim;
        [SerializeField] private float yTarget = 200f;

        // Values    
        private const float MAX_WIDTH = 1000f;
        private const float DISTANCE = 1.5f;//with speed = 1f, time = 1.5f, s = v * t

        public string message { get; private set; }

        // Methods
        public void AnimComplete() { Destroy(gameObject); }

        private void FillData(string message, float duration, float yOffset = 0f)
        {
            this.message = message;

            m_TxtContent.text = message;
            if (m_TxtContent.preferredWidth > MAX_WIDTH)
                m_LayoutElm.preferredWidth = MAX_WIDTH;

            Vector2 v2 = m_RectBox.anchoredPosition;
            m_RectBox.anchoredPosition = new Vector2(v2.x, yTarget + yOffset);

            float speed = (duration == 0f) ? 1f : (DISTANCE / duration);
            m_Anim.speed = speed;
            m_Anim.Play("Play", -1, 0f);

            transform.SetAsLastSibling();
        }

        // Use this for initialization
        //void Start() { }

        // Update is called once per frame
        //void Update() { }

        private void OnDestroy()
        {
            cache.Remove(gameObject);
        }

        private static List<GameObject> cache = new List<GameObject>();
        public static void Show(string message, float duration = 2f, float yOffset = 0f)
        {
            GameObject go = null;
            foreach (GameObject tmp in cache)
            {
                Toast script = tmp.GetComponent<Toast>();
                if (script.message.Equals(message))
                {
                    go = tmp;
                    break;
                }
            }

            if (go == null)
            {
                string assetName = "ToastPrefab";
                go = IUtil.LoadPrefabWithParent("GIKCore/UI/Toast/" + assetName, Game.main.netBehavior.panelBlock);//root
                go.name = assetName;

                cache.Add(go);
            }

            go.GetComponent<Toast>().FillData(message, duration, yOffset);
        }
        public static void ShowOnTut(string message, float duration = 2f, float yOffset = 0f)
        {
            GameObject go = null;
            foreach (GameObject tmp in cache)
            {
                Toast script = tmp.GetComponent<Toast>();
                if (script.message.Equals(message))
                {
                    go = tmp;
                    break;
                }
            }

            if (go == null)
            {
                string assetName = "ToastPrefab";
                go = IUtil.LoadPrefabWithParent("GIKCore/UI/Toast/" + assetName, Game.main.netBehavior.panelBlock);//root
                go.name = assetName;

                cache.Add(go);
            }

            go.GetComponent<Toast>().FillData(message, duration, yOffset);
        }
        internal static void show(List<string> aString)
        {
            throw new NotImplementedException();
        }
    }
}

