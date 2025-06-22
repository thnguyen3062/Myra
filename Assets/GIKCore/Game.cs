using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore.Net;
using GIKCore.UI;
using GIKCore.Utilities;
using GIKCore.Lang;

namespace GIKCore
{
    public class Game
    {
        private static Game instance;
        public static Game main
        {
            get
            {
                if (instance == null) instance = new Game();
                return instance;
            }
        }

        public Game()
        {
            socket = new SocketSession();
            http = new HttpSession();
        }

        // ------- Net -------
        public List<GameListener> listeners = new List<GameListener>();
        public NetBehavior netBehavior { get; set; }
        public SocketSession socket;
        public HttpSession http;
        public IPurchaser IAP;
#if !UNITY_STANDALONE
        public IFacebook FB;
        public IGoogle google;
#endif

        // ------- UI -------
        public CameraAdjust cam { get; set; }
        public ICanvas canvas { get; set; }
        public NetBlock netBlock { get { return netBehavior.netBlock; } }
        public void SetActiveBlockSocket(bool active) { netBehavior.SetActiveBlockSocket(active); }

        // ------ Other ------
        public LangFontCollector fontCollector { get; set; }

        // Methods
        public void LoadScene(string nextscene, ICallback.CallFunc onLoadSceneSuccess = null, float delay = 0f, bool curtain = false)
        {
            if (netBehavior != null) netBehavior.LoadScene(nextscene, onLoadSceneSuccess, delay, curtain);
        }

        public void StartCoroutine(IEnumerator routine)
        {
            if (netBehavior != null) netBehavior.StartCoroutine(routine);
        }
        public void StopCoroutine(IEnumerator routine)
        {
            if (netBehavior != null) netBehavior.StopCoroutine(routine);
        }
    }
}
