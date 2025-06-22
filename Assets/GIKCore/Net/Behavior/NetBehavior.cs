using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using GIKCore.DB;
using GIKCore.UI;
using GIKCore.Utilities;
using DG.Tweening;

namespace GIKCore.Net
{
    public class NetBehavior : MonoBehaviour
    {
        // Fields
        [SerializeField] private Canvas m_Canvas;
        [SerializeField] private RectTransform m_PanelBlock;
        [SerializeField] private NetBlock m_NetBlock;
        [SerializeField] private GameObject m_BlockSocket;

        // Values  
        private bool waitPing = false;
        private bool isChangeScene = false;
        private float timeCheckConnection = 0f;
        private string nextScene = "";

        private NetPing.InternetStatus internetStatus = NetPing.InternetStatus.Connected;
        private ICallback.CallFunc onLoadSceneSuccessCB = null;

        // Methods
        // ------- CANVAS ------
        public Canvas canvas { get { return m_Canvas; } }
        public RectTransform panelBlock { get { return m_PanelBlock; } }
        public NetBlock netBlock { get { return m_NetBlock; } }
        public void SetActiveBlockSocket(bool active) { m_BlockSocket.SetActive(active); }

        // ------- Scene -------
        public void LoadScene(string nextScene, ICallback.CallFunc onLoadSceneSuccess = null, float delay = 0f, bool curtain = false)
        {
            this.nextScene = nextScene;
            onLoadSceneSuccessCB = onLoadSceneSuccess;
            isChangeScene = true;
            if (curtain) PopupLoading.Show();
            StartCoroutine(LoadScene(delay));
        }
        private IEnumerator LoadScene(float delay = 0f)
        {//for some animation load scene
            yield return new WaitForSeconds(delay);
            netBlock.Hide();            
            DOTween.KillAll();
            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            isChangeScene = false;
            if (onLoadSceneSuccessCB != null)
            {
                onLoadSceneSuccessCB();//just execute once
                onLoadSceneSuccessCB = null;
            }
        }

        // ------- Network -------
        public bool IsConnected()
        {
            return internetStatus == NetPing.InternetStatus.Connected;
        }
        public void InternetConnected()
        {
            timeCheckConnection = 0;
            internetStatus = NetPing.InternetStatus.Connected;
        }
        private void CheckReconnect(NetPing.InternetStatus status)
        {
            waitPing = false;
            timeCheckConnection = 0;

            if (internetStatus != status)
            {
                LogWriterHandle.WriteLog("Check Reconnect: " + internetStatus + "|" + status);
                internetStatus = status;
                if (!Game.main.socket.tryConnect)
                    Game.main.socket.tryConnect = true;
            }

            if (Game.main.socket.tryConnect)
            {
                Game.main.socket.tryConnect = false;
                Game.main.socket.Connect();
            }
        }
        private void CheckConnection(float deltaTime)
        {
            if (!waitPing)
            {
                timeCheckConnection += deltaTime;
                if (timeCheckConnection >= 5f)
                {
                    timeCheckConnection = 0f;
                    //Check if the device cannot reach the internet at all (that means if the "cable", "WiFi", etc. is connected or not)
                    //if not, don't waste your time.
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        LogWriterHandle.WriteLog("Internet Not Connected");
                        CheckReconnect(NetPing.InternetStatus.NotConnected);
                    }
                    else
                    {
                        //It could be a network connection but not internet access so you have to ping your host / server to be sure.
                        waitPing = true;
                        StartCoroutine(NetPing.HttpHeadRequest(Config.pingURL, s =>
                        {                            
                            if (s == NetPing.InternetStatus.NotConnected && !string.IsNullOrEmpty(Config.pingURL1))
                            {                               
                                StartCoroutine(NetPing.HttpHeadRequest(Config.pingURL1, s1 =>
                                {                                    
                                    if (s1 == NetPing.InternetStatus.NotConnected && !string.IsNullOrEmpty(Config.pingURL2))
                                    {
                                        StartCoroutine(NetPing.HttpHeadRequest(Config.pingURL2, s2 =>
                                        {
                                            CheckReconnect(s2);
                                        }));
                                    }
                                    else CheckReconnect(s1);
                                }));
                            }
                            else CheckReconnect(s);
                        }));
                    }
                }
            }
        }

        // ------- Callback From Native -------
#if UNITY_WEBGL
    public void WebGLPasted(string s)
    {
        Game.main.QueueNetData(NetData.WEBGL_PASTED, s);
    }
    public void WebGLGetDeviceId(string s)
    {
        PopupDeviceUUID.Set(s);
    }
#endif

        // Use this for initialization
        void Awake()
        {
            if (Game.main.netBehavior == null)
            {
                Game.main.netBehavior = this;
                DontDestroyOnLoad(transform.root.gameObject);                

                SceneManager.sceneLoaded += OnSceneLoaded;
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
                Application.targetFrameRate = 45;
#if (UNITY_ANDROID || UNITY_IOS)
                Application.targetFrameRate = 45;
#else

                Application.targetFrameRate = 60;
#endif
            }
            else
            {
                Destroy(gameObject);
            }
        }

        //void Start() { }

        // Update is called once per frame
        void Update()
        {
            if (isChangeScene) return;            
            
            // detect keydown
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                LogWriterHandle.WriteLog("Hello World");
#if (UNITY_ANDROID || UNITY_IOS)
                HandleNetData.SendNetDataToListener(NetData.GET_KEY_DOWN, KeyCode.Escape);
#elif UNITY_WEBGL
            if (Screen.fullScreen) Screen.fullScreen = false;
#endif
            }

            if (Keyboard.current.f11Key.wasPressedThisFrame)
            {
#if UNITY_WEBGL
            if (!Screen.fullScreen) Screen.fullScreen = true;
#endif
            }
        }

        void FixedUpdate()
        {
            if (isChangeScene) return;

            // network        
            CheckConnection(Time.deltaTime);
            HandleNetData.HandleQueue();

        }

        void OnApplicationQuit()
        {
            LogWriterHandle.WriteLog("------- On Application Quit");
            Game.main.socket.DisconnectSocket(" For Quit");
            StopAllCoroutines();
        }

        private void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 40, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 80;
            style.normal.textColor = Color.white; 
            ServerInfo server = Config.GetServer();
            string typeSV = "";
            if(server.host == "45.77.245.14")
            {
                typeSV = "D.";
            }
            else if(server.host == "139.180.153.32")
            {
                typeSV = "T.";
            }
            else if (server.host == "137.220.60.146")
            {
                typeSV = "R.";
            }
            string text = "  V: " + typeSV + Application.version + "2409";
            GUI.Label(rect, text, style);
        }
    }

}