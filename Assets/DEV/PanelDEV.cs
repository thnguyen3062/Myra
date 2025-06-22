using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIEngine.Extensions;
using UIEngine.Extensions.Attribute;
using GIKCore.UI;
using GIKCore.DB;
using GIKCore.Net;
using GIKCore.Utilities;

public class PanelDEV : MonoBehaviour
{
    // Fields
    [Help("Click exactly 7 times then hold 5s")]
    [SerializeField] private GameObject m_GoContent;
    [SerializeField] private InputField m_Field1, m_Field2;    
    [SerializeField] private LongClick m_LongClick;
    [SerializeField] private bool m_AlwaysOnTop = false;

    // Values
    private int count;

    // Methods
    public void DoClick()
    {
        count++;
    }
    public void Play()
    {
        string s1 = m_Field1.text;
        string s2 = m_Field2.text;

        CheckDev(s1, s2);
    }

    private void Awake()
    {
        if (m_LongClick != null)
        {
            m_LongClick.AddOnLongClickEvent(() =>
            {
                if (count > 7) count = 0;
                if (count == 7)
                {
                    count = 0;
                    m_GoContent.SetActive(true);
                }
            });
        }
    }
    // Start is called before the first frame update
    //void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (m_AlwaysOnTop)
            transform.SetAsLastSibling();
    }

    public static bool CheckDev(string s1, string s2)
    {
        if (s1.Equals("cp") && s2.Equals("cp"))
            Toast.Show("Cp code: " + Constants.CP_CODE);
        else if (s1.Equals("pkn") && s2.Equals("pkn"))
            Toast.Show("PKN: " + Constants.PKN);
        else if (s1.Equals("vs") && s2.Equals("vs"))
            Toast.Show("Version: " + Constants.VERSION);
        else if (s1.Equals("di") && s2.Equals("di"))
            Toast.Show("Device Id: " + Constants.DEVICE_ID);
        else if (s1.Equals("dn") && s2.Equals("dn"))
            Toast.Show("Device Name: " + Constants.DEVICE_NAME);
        else if (s1.Equals("mcc") && s2.Equals("mcc"))
            Toast.Show("MCC code: " + Constants.CLIENT_MCC);
        else if (s1.Equals("dm") && s2.Equals("dm"))
        {
            IUtil.CopyToClipBoard(Config.GetUrlConfig(), Config.DOMAIN + " | Copy url config");
        }
        else if (s1.Equals("jc") && s2.Equals("jc"))
        {
            IUtil.CopyToClipBoard(GamePrefs.LastJsonConfig, "Copy last json config");
        }
        else if (s1.Equals("sv") && s2.Equals("sv"))
        {
            ServerInfo si = Config.GetServer();
            Toast.Show(si.host + ":" + si.port + "|id: " + si.id, 3f);
        }
        else if (s1.Equals("cpm") && s2.StartsWith("#"))
        {
            string[] split = s2.Split(new string[] { "#" }, System.StringSplitOptions.RemoveEmptyEntries);
            if (split.Length > 0)
            {
                Constants.CP_CODE = split[0];
                Toast.Show("Change cp code to " + Constants.CP_CODE);
            }
        }
        else if (s1.Equals("ping") && s2.Equals("ping"))
        {
            NetPing.allowLog = true;
            IUtil.CopyToClipBoard(Config.pingURL + "|" + Config.pingURL1 + "|" + Config.pingURL2, "open ping log");
        }
        else if (s1.Equals("cf") && s2.Equals("cf"))
        {
            Config.allowLog = true;
            Toast.Show("open config log");
        }
        else if (s1.Equals("ihr") && s2.Equals("ihr"))
        {
            IHttpRequest.allowLog = true;
            Toast.Show("open http request log");
        }
        else if (s1.Equals("isk") && s2.Equals("isk"))
        {
            ISocket.allowLog = true;
            Toast.Show("open socket log");
        }
        else if (s1.Equals("ifb") && s2.Equals("ifb"))
        {
            //IFacebook.allowLog = true;
            Toast.Show("open facebook log");
        }
        else if (s2.Equals(MD5CryptoServiceProvider.GetMd5String(Constants.PKN)))
        {
            if (s1.Equals("dev"))
            {
                Toast.Show("open ALL log");
                Config.allowLog = IHttpRequest.allowLog = ISocket.allowLog = true;//IFacebook.allowLog = true;
            }
            else if (s1.Equals("-r"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("SplashScene");
            }
        }
        else return false;
        return true;
    }
}
