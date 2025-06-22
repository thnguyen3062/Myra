using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GIKCore.Lang;
using GIKCore.UI;
using GIKCore.Utilities;

public class BoxCaptcha : MonoBehaviour
{
    // Fields
    [SerializeField] private TMP_InputField m_EditCaptcha;
    [SerializeField] private TextMeshProUGUI m_TxtCaptcha;

    // Methods
    public void DoGen()
    {
        string alphabet = "1234567890";
        string captcha = "";
        for (int i = 0; i < 3; i++)
        {
            int index = IUtil.RandomInt(0, alphabet.Length - 1);
            captcha += alphabet[index];
        }
        m_TxtCaptcha.text = captcha;
    }
    public void DoReset()
    {
        DoGen();
        m_EditCaptcha.text = "";
    }
    public bool Match()
    {
        string captcha = m_EditCaptcha.text;
        if (!captcha.Equals(m_TxtCaptcha.text))
        {
            Toast.Show(LangHandler.Get("captcha-2", "Mã bảo vệ không đúng"));
            DoGen();
            return false;
        }
        return true;
    }

    // Start is called before the first frame update
    //void Start() { }

    // Update is called once per frame
    //void Update() { }
}
