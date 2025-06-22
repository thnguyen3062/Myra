using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using pbdson;

namespace GIKCore.Net
{
    public class ISocket
    {
        public static bool allowLog = true;

        // Define a timeout in milliseconds for each asynchronous call. If a response is not received within this 
        // timeout period, the call is aborted.
        public const int TIMEOUT_MILLISECONDS = 5000;
        // The maximum size of the data buffer to use with the asynchronous socket methods
        public const int MAX_BUFFER_SIZE = 12048;
        public const int REQUIRE_BYTE_READ_MIN = 10;

        public static readonly List<string> MAGICS = new List<string>() { "K2" };

        // Values   
        public bool isConnected = false;
        private int countConnect = 0;
        private IAbstractSocket mSocket;

        // Methods
        public static T Parse<T>(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                PbmethodSerializer x = new PbmethodSerializer();
                T proto = (T)x.Deserialize(ms, null, typeof(T));
                return proto;
            }
        }
        public byte[] SerializerByte(object proto) 
        {
            return Serializer(proto).ToArray();
        }

        private MemoryStream Serializer(object proto)
        {
            if (proto == null) return null;
            using (MemoryStream ms = new MemoryStream())
            {
                //Phai dung BmLibSerializer thay vi Protobuf.Serializer de tuong thich voi iOS
                PbmethodSerializer x = new PbmethodSerializer();
                x.Serialize(ms, proto);
                return ms;
            }
        }

        public void Connect(string host, int port)
        {
            DisconnectSocket("For New Session");

            if (allowLog) Debug.Log("------ Server ::: " + host + ":" + port);
#if UNITY_WEBGL
        Debug.Log("------- Connect to Websocket");
        mSocket = new IWebSocket(this);
#else
            Debug.Log("------- Connect to CommonSocket");
            mSocket = new ICommonSocket(this);
#endif

            mSocket.Connect(host, port);
            countConnect++;
        }
        public void DisconnectSocket(string log = "")
        {
            Debug.Log("------- Disconect Socket " + log);
            isConnected = false;
            if (mSocket != null)
                mSocket.Close();
        }
        public bool CheckSocketConnected()
        {
            if (mSocket == null)
                return false;
            return isConnected;
        }
        public bool LimitCount(bool reset = true)
        {
            if (countConnect >= 8)
            {
                if (reset) ResetCount();
                return true;
            }
            return false;
        }
        public void ResetCount() { countConnect = 0; }

        public void SocketConnectSuccess() { HandleNetData.QueueNetData(NetData.SOCKET_CONNECT_SUCCESS, null); }
        public void SocketError() { HandleNetData.QueueNetData(NetData.SOCKET_ERROR, null); }
        public void SocketDisconectRead() { HandleNetData.QueueNetData(NetData.SOCKET_DISCONNECT_READ, null); }
        public void ReceivePackageFail() { HandleNetData.QueueSocketData(IService.RECEIVE_PACKAGE_FAIL, null); }
        public void ReceivePackage(int id, byte[] data)
        {
            Game.main.netBehavior.InternetConnected();
            HandleNetData.QueueSocketData(id, data);
        }

        private void SendPacket(int id, byte[] data)
        {
            if (!CheckSocketConnected())
            {
                Debug.Log("------- Socket is not connected!");
                return;
            }

            try
            {
                List<byte> lstByteSend = new List<byte>();
                lstByteSend.AddRange(System.Text.Encoding.UTF8.GetBytes("K2"));

                byte[] bService = new byte[4];
                bService[0] = (byte)((id >> 24) & 0xFF);
                bService[1] = (byte)((id >> 16) & 0xFF);
                bService[2] = (byte)((id >> 8) & 0xFF);
                bService[3] = (byte)((id & 0xFF));
                lstByteSend.AddRange(bService);

                if (data != null)
                {
                    int length = data.Length;
                    byte[] bLength = new byte[4];

                    bLength[0] = (byte)((length >> 24) & 0xFF);
                    bLength[1] = (byte)((length >> 16) & 0xFF);
                    bLength[2] = (byte)((length >> 8) & 0xFF);
                    bLength[3] = (byte)((length & 0xFF));

                    lstByteSend.AddRange(bLength);
                    lstByteSend.AddRange(data);
                }
                else
                {
                    byte[] ZERO = { 0, 0, 0, 0 };
                    lstByteSend.AddRange(ZERO);
                }

                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = lstByteSend.ToArray();
                if (allowLog)
                {
                    Debug.Log("------- SENT PACKAGE ::: " + id);
                    //LogWriterHandle.WriteLog("------- SENT PACKAGE ::: " + id);
                }

                if (mSocket != null)
                {
                    mSocket.Send(byteData);
                    if (IService.NeedShowLoading(id))
                        Game.main.netBlock.Show();
                }
                else
                {
                    Debug.Log("------- Socket is not initialized!");
                    //LogWriterHandle.WriteLog("------- Socket is not initialized!");
                    isConnected = false;
                }
            }
            catch (Exception e)
            {
                Debug.Log("------- Error when send cardData ::: " + e.Message);
                Debug.Log("------- " + e.StackTrace);
                isConnected = false;
            }
        }
        public void SendProto(int id, object proto)
        {
            SendPacket(id, (proto == null) ? null : Serializer(proto).ToArray());
        }
    }
}
