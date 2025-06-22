using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore.Net;

namespace GIKCore.Net
{
    public class GameListener : MonoBehaviour
    {
        public virtual bool ProcessNetData(int id, object data)
        {
            if (id == NetData.GET_KEY_DOWN)
            {
                KeyCode keyCode = (KeyCode)data;
                if (keyCode == KeyCode.Escape)
                {
                    DoEscape();
                    return true;
                }
            }
            return false;
        }

        public virtual bool ProcessHttpResponseData(int id, string data)
        {
            return false;
        }

        public virtual bool ProcessSocketData(int id, byte[] data)
        {
            return false;
        }

        protected virtual void DoEscape() { }

        // Awake is called before every other function   
        protected virtual void Awake()
        {
            Game.main.listeners.Add(this);
        }

        protected virtual void OnDestroy()
        {
            Game.main.listeners.Remove(this);
        }
    }
}