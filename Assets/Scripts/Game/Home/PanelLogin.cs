using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GIKCore.UI;
using GIKCore.DB;
using GIKCore.Utilities;
using GIKCore;
using GIKCore.Lang;

public class PanelLogin : MonoBehaviour
{
    // Fields
    [SerializeField] private TMP_InputField m_InputAccount, m_InputPwd;
    [SerializeField] private ButtonOnOffTMP m_BtnRememberMe;
    [SerializeField] private bool m_FullVersion = true;

    // Values
    private ICallback.CallFunc onOpenPanelRegister;

    // Methods
    public void DoClickRemember()
    {
        bool on = !GamePrefs.Remember;
        GamePrefs.Remember = on;
        if (m_BtnRememberMe != null) m_BtnRememberMe.Turn(on);
    }
    public void DoClickLogin()
    {
#if UNITY_EDITOR
        //string rt = "eyJhbGciOiJIUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICIxYjU5MzIwNC1lOTAzLTRiNzctYjA0NC1lMjdiNWY2ZTEwZGYifQ.eyJpYXQiOjE2NTAzMzI5NDksImp0aSI6IjRmZWYwYWU0LWQyNjktNDk2Yi04MTE3LWVkN2I3NDZlYzI0MCIsImlzcyI6Imh0dHBzOi8vYXV0aC5teXRoZXJpYS5pby9yZWFsbXMvbXl0aCIsImF1ZCI6Imh0dHBzOi8vYXV0aC5teXRoZXJpYS5pby9yZWFsbXMvbXl0aCIsInN1YiI6IjdhNzFkOTM4LTllNTItNGQ5Yi1iMDcxLThkYjM5NGIxMWE0NCIsInR5cCI6Ik9mZmxpbmUiLCJhenAiOiJteXRoYXBwIiwibm9uY2UiOiI5NmJhZTkzYi1hZjg5LTQwNzAtYmY2Mi0wODU3OTNmOGFhMWEiLCJzZXNzaW9uX3N0YXRlIjoiZjY0NmQ4OWYtNzRkMi00NGZmLWI3MWMtYjIwNzQyODI1OThjIiwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBvZmZsaW5lX2FjY2VzcyBlbWFpbCIsInNpZCI6ImY2NDZkODlmLTc0ZDItNDRmZi1iNzFjLWIyMDc0MjgyNTk4YyJ9.-l_McRkwY15o0hOZQhS460xZ_UFM158OTe8wo30dIYA";
        string at = "eyJhbGciOiJIUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICIxYjU5MzIwNC1lOTAzLTRiNzctYjA0NC1lMjdiNWY2ZTEwZGYifQ.eyJleHAiOjE2NDk5MDgzODYsImlhdCI6MTY0OTkwNjU4NiwianRpIjoiMzhlZGU3NDYtY2VlMi00NzdjLThiMDEtOWQzYmQ3NzdmOTA3IiwiaXNzIjoiaHR0cHM6Ly9hdXRoLm15dGhlcmlhLmlvL3JlYWxtcy9teXRoIiwiYXVkIjoiaHR0cHM6Ly9hdXRoLm15dGhlcmlhLmlvL3JlYWxtcy9teXRoIiwic3ViIjoiNjJhMDZkZjktNGE0ZC00N2EyLThjYTYtZDBjNDY1MmRjZjdmIiwidHlwIjoiUmVmcmVzaCIsImF6cCI6Im15dGhhcHAiLCJub25jZSI6ImViYTNmNmQ3LTc2MmMtNGI0MC1hOGEwLTc0ZTQwYmJhYzdkYyIsInNlc3Npb25fc3RhdGUiOiJkOTJiYzg3ZS1mOGI3LTRhNTEtODI5OC1mOTJjZGJjN2Q0NTgiLCJzY29wZSI6Im9wZW5pZCBwcm9maWxlIGVtYWlsIiwic2lkIjoiZDkyYmM4N2UtZjhiNy00YTUxLTgyOTgtZjkyY2RiYzdkNDU4In0.Bq2OoejEwv_OF39CJhChgd2YAw9hKgyJuYwxVr9aBwo";
        //string rt = "eyJhbGciOiJIUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICIxYjU5MzIwNC1lOTAzLTRiNzctYjA0NC1lMjdiNWY2ZTEwZGYifQ.eyJpYXQiOjE2NTA1MTIwODksImp0aSI6IjE3NThkZDViLTJiNTQtNDc4Yi1hYmQ5LTVjMzBiN2QwZWJhMSIsImlzcyI6Imh0dHBzOi8vYXV0aC5teXRoZXJpYS5pby9yZWFsbXMvbXl0aCIsImF1ZCI6Imh0dHBzOi8vYXV0aC5teXRoZXJpYS5pby9yZWFsbXMvbXl0aCIsInN1YiI6ImUwZmQ5MmVjLTBmNzctNDk3ZS1hYmIxLWFlN2RhZmE0MGNhZiIsInR5cCI6Ik9mZmxpbmUiLCJhenAiOiJteXRoYXBwIiwibm9uY2UiOiIyM2VjMGQxNC1hMjIxLTQwOGQtODYwYS1kMDJhMWVjNGI2MTEiLCJzZXNzaW9uX3N0YXRlIjoiNTIyYTRlZTctZmI1Yi00ODk1LWE3MDQtMzRlZjJhMWU3ZTFlIiwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBvZmZsaW5lX2FjY2VzcyBlbWFpbCIsInNpZCI6IjUyMmE0ZWU3LWZiNWItNDg5NS1hNzA0LTM0ZWYyYTFlN2UxZSJ9.mBYjqmidByC4Xhd3LJwN7nAppnzi2uRlKJer2j767-k";
        string rt = "eyJhbGciOiJIUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICIxYjU5MzIwNC1lOTAzLTRiNzctYjA0NC1lMjdiNWY2ZTEwZGYifQ.eyJpYXQiOjE2NTA4NzkyMzQsImp0aSI6ImM1NGExNjM3LTY2MTQtNGU0NC1iZDE5LWFmNjdlMDBlOWNjZSIsImlzcyI6Imh0dHBzOi8vYXV0aC5teXRoZXJpYS5pby9yZWFsbXMvbXl0aCIsImF1ZCI6Imh0dHBzOi8vYXV0aC5teXRoZXJpYS5pby9yZWFsbXMvbXl0aCIsInN1YiI6Ijc5MGFkODI1LWFlNjktNDM3MS04NWY2LWJiY2JhOTIwNTg0MCIsInR5cCI6Ik9mZmxpbmUiLCJhenAiOiJteXRoYXBwIiwibm9uY2UiOiJiOWZiMmI1Zi0xZjQyLTRhMjMtODE2NS05NWYyOGVmMzlkMzciLCJzZXNzaW9uX3N0YXRlIjoiMWEwMTFmYjQtNmRhMC00ZmRjLWFkOTYtM2FiYTIyNjZkMjkzIiwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBvZmZsaW5lX2FjY2VzcyBlbWFpbCIsInNpZCI6IjFhMDExZmI0LTZkYTAtNGZkYy1hZDk2LTNhYmEyMjY2ZDI5MyJ9.ohgemO0vy-aWrEV1jOzC-Og8boPguaGWAtOXsH3qies";
        // white list
        //string rt = "eyJhbGciOiJIUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICIxYjU5MzIwNC1lOTAzLTRiNzctYjA0NC1lMjdiNWY2ZTEwZGYifQ.eyJpYXQiOjE2NTA2ODEzNjEsImp0aSI6IjZlMjc5MTRlLWZkMDUtNDA5OS04MWNiLTJjM2Q1YjhjMjZjMCIsImlzcyI6Imh0dHBzOi8vYXV0aC5teXRoZXJpYS5pby9yZWFsbXMvbXl0aCIsImF1ZCI6Imh0dHBzOi8vYXV0aC5teXRoZXJpYS5pby9yZWFsbXMvbXl0aCIsInN1YiI6IjYyYTA2ZGY5LTRhNGQtNDdhMi04Y2E2LWQwYzQ2NTJkY2Y3ZiIsInR5cCI6Ik9mZmxpbmUiLCJhenAiOiJteXRoYXBwIiwibm9uY2UiOiJhNDU5N2U3MC0zMTBjLTRjMmUtOGUwZS0wNjg2MWM2MmRhNWIiLCJzZXNzaW9uX3N0YXRlIjoiOTI0ZDQ2ZGQtMWQyMi00NzY4LWI3YjgtOWQwOWQwNjI3ODM1Iiwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBvZmZsaW5lX2FjY2VzcyBlbWFpbCIsInNpZCI6IjkyNGQ0NmRkLTFkMjItNDc2OC1iN2I4LTlkMDlkMDYyNzgzNSJ9.6DyYJ96NLGWTzmaD80HUDPTq67shRMC0Q-wJuKQSaeM";
        Game.main.socket.LoginBlockchain(at, rt);
#elif UNITY_ANDROID
        PopupWebview.Show("https://openid.helitech-solutions.com/");
#endif
        ///*test
        //string usn = m_InputAccount.text;
        //string pwd = m_InputPwd.text;

        //if (PanelDEV.CheckDev(usn, pwd))
        //{
        //    //do something with dev
        //}
        //else if (!m_FullVersion && (string.IsNullOrEmpty(usn) || string.IsNullOrEmpty(pwd)))
        //    PopupLogin.Show();
        //else PopupLogin.LoginNormal(usn, pwd);
        //*/
    }
    public void DoClickGuest() { Toast.Show(LangHandler.Get("toast-8", "Coming Soon!")); }
    public void DoClickFacebook()
    {
        //PopupLogin.LoginFacebook();
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    public void DoClickTwitter() { Toast.Show(LangHandler.Get("toast-8", "Coming Soon!")); }
    public void DoOpenRegisterInner()
    {
        SetActive(false);
        if (onOpenPanelRegister != null) onOpenPanelRegister();
        //Toast.Show("Coming Soon!");
    }
    public void DoOpenRegisterOuter() { PopupLogin.Show(PopupLogin.Type.Register); }
    public void DoClickForgotPwd()
    {
        //PopupRecoverPwd.Show();
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    public void SetOnOpenPanelRegister(ICallback.CallFunc func) { onOpenPanelRegister = func; }
    public void SetActive(bool active) { gameObject.SetActive(active); }
    public void DoReset()
    {
        m_InputAccount.text = GamePrefs.Username;
        m_InputPwd.text = GamePrefs.Password;
        if (m_BtnRememberMe != null) m_BtnRememberMe.Turn(GamePrefs.Remember);
    }

    // Use this for initialization
    void Start()
    {
        DoReset();
    }

    // Update is called once per frame
    //void Update() { }
}
