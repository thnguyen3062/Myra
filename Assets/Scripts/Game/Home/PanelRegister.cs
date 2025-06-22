using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GIKCore;
using GIKCore.Lang;
using GIKCore.UI;
using GIKCore.Utilities;
using System.Text.RegularExpressions;

public class PanelRegister : MonoBehaviour
{
    // Fields
    [SerializeField] private TMP_InputField m_InputEmail, m_InputAccount, m_InputPass, m_InputPassRe;
    //[SerializeField] private BoxCaptcha m_BoxCaptcha;    

    // Values
    private ICallback.CallFunc onOpenPanelLogin;
    private bool agreePolicy = true;
    public string email { get; private set; }

    public string username { get; private set; }
    public string password { get; private set; }
    public string policy { get; set; }

    // Methods
    public void DoClickPolicyDetail()
    {
        //if (!string.IsNullOrEmpty(policy)) IUtil.OpenURL(policy);
        //else Game.main.socket.GetGameGuide(Constants.CODE_REGISTER);
    }
    public void DoClickPolicyAgree()
    {
        agreePolicy = !agreePolicy;        
    }
    public void DoClickBack()
    {
        SetActive(false);
        if (onOpenPanelLogin != null) onOpenPanelLogin();
    }
    public void DoClickRegister()
    {
        string m_email = m_InputEmail.text;
        string account = m_InputAccount.text;
        string pass = m_InputPass.text;
        string passRe = m_InputPassRe.text;

        if (string.IsNullOrEmpty(account)) Toast.Show(LangHandler.Get("login-9", defaultValue: "Cung cấp tài khoản"));
        else if (string.IsNullOrEmpty(pass)) Toast.Show(LangHandler.Get("login-10", defaultValue: "Cung cấp mật khẩu"));
        else if (pass.Length < 6) Toast.Show(LangHandler.Get("login-11", defaultValue: "Mật khẩu phải dài ít nhất 6 ký tự"));
        else if (!pass.Equals(passRe)) Toast.Show(LangHandler.Get("login-12", defaultValue: "Mật khẩu nhập lại không khớp"));
        // else if (!m_BoxCaptcha.Match())
        // {
        //     // do nothing
        // }
        else if(string.IsNullOrEmpty(m_email)) Toast.Show(LangHandler.Get("login-14",defaultValue: "Cung cấp email"));
        else if(!validateEmail(m_email)) Toast.Show(LangHandler.Get("login-15",defaultValue: "Invalid email"));
        else if (!agreePolicy)
            Toast.Show(LangHandler.Get("login-13", defaultValue: "Bạn phải đồng ý với chính sách bảo mật của chúng tôi"));
        else
        {
            username = account;
            password = pass;
            email = m_email;
            Game.main.socket.Register(username, password, email);            
        }
    }

public  const string MatchEmailPattern =
		@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
		+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
		+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
		+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
    
    
    	public static bool validateEmail (string email)
	{
		if (email != null)
			return Regex.IsMatch (email, MatchEmailPattern);
		else
			return false;
	}
    public void SetOnOpenPanelLogin(ICallback.CallFunc func) { onOpenPanelLogin = func; }
    public void SetActive(bool active) { gameObject.SetActive(active); }
    public void DoReset()
    {
        agreePolicy = true;        
        email=username = password = "";
        m_InputAccount.text = m_InputPass.text = m_InputPassRe.text = "";
      //  m_BoxCaptcha.DoReset();
    }
    //public void DoFalse() { m_BoxCaptcha.DoGen(); }

    // Use this for initialization
    //void Start() { }

    // Update is called once per frame
    //void Update() { }
}
