using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.IO;
using GIKCore.Utilities;

namespace GIKCore.Net
{
    public class NetPing
    {
        public enum InternetStatus { None, Connected, NotConnected }

        // Values
        public static bool allowLog = false;
        private static InternetStatus status = InternetStatus.None;
        private static bool isDone = false;
        private static bool conntected = false;

        // Methods
        public static IEnumerator HttpHeadRequest(string uri, ICallback.CallFunc2<InternetStatus> callback)
        {
            using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Head(uri))
            {
                www.timeout = 5;
                yield return www.SendWebRequest();
                // responseCode = 0 => Malformed URL | Request timeout | Cannot resolve destination host | ...
                // just have responseCode (> 0) we have internet connection
                bool result = www.responseCode > 0;
                if (allowLog) LogWriterHandle.WriteLog(uri + "|" + www.error + "|" + www.responseCode);

                PingDone(result);
                callback(status);
            }
        }

        public static IEnumerator DoPing(string hostName, ICallback.CallFunc2<InternetStatus> callback)
        {
            DoPing(hostName);
            yield return new WaitUntil(() => isDone);
            callback(status);
        }

        private static void PingDone(bool result)
        {
            isDone = true;
            conntected = result;
            if (result) status = InternetStatus.Connected;
            else status = InternetStatus.NotConnected;
        }

        private static void DoPing(string hostName)
        {
            if (PingThis(hostName)) //call to check if you can make ping to that host name
                PingDone(true);
            else PingDone(false);
        }

        private static bool PingThis(string hostName)
        {
            try
            {
                //I strongly recommend to check Ping, Ping.Send & PingOptions on microsoft C# docs or other C# info source
                //in this block you configure the ping call to your host or server in order to check if there is network connection.

                //from https://forum.unity.com/threads/how-to-check-internet-connection-in-an-app.384541/
                //from https://stackoverflow.com/questions/55461884/how-to-ping-for-ipv4-only
                //from https://stackoverflow.com/questions/49069381/why-ping-timeout-is-not-working-correctly
                //and from https://stackoverflow.com/questions/2031824/what-is-the-best-way-to-check-for-internet-connectivity-using-net            

                System.Net.NetworkInformation.Ping myPing = new System.Net.NetworkInformation.Ping();
                byte[] buffer = new byte[32]; //array that contains data to be sent with the ICMP echo
                int timeout = 5000; //in milliseconds
                System.Net.NetworkInformation.PingOptions pingOptions = new System.Net.NetworkInformation.PingOptions(64, true);
                System.Net.NetworkInformation.PingReply reply = myPing.Send(GetIPAddress(hostName), timeout, buffer, pingOptions);

                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    return true;
                else if (reply.Status == System.Net.NetworkInformation.IPStatus.TimedOut)
                    return conntected;//in case of timeout, maybe have connection, but have some problem like block ping in host.server side,..., so we return last status
                else return false;
            }
            catch (Exception e)
            {
                LogWriterHandle.WriteLog(e.ToString());
                return false;
            }
            finally { } //To not get stuck in an error or exception, see "Try, Catch, Finally" docs.
        }

        /// <summary>
        /// Get the actual IP addres of your host/server like google.com
        /// </summary>
        /// <returns>
        /// IPv4 or hostName if exception
        /// </returns>
        private static string GetIPAddress(string hostName)
        {
            //Yes, I could use the "host name" or the "host IP address" direct on the ping.send method BUT!!
            //I find out and "Situation" in which due to my network setting in my PC any ping call (from script or cmd console)
            //returned the IPv6 instead of IPv4 which couse the Ping.Send thrown an exception
            //that could be the scenario for many of your users so you have to ensure this run for everyone.


            //from https://stackoverflow.com/questions/1059526/get-ipv4-addresses-from-dns-gethostentry

            IPHostEntry host;
            try
            {
                host = Dns.GetHostEntry(hostName); //Get the IP host entry from your host/server
                foreach (IPAddress ip in host.AddressList)
                {
                    //filter just the IPv4 IPs
                    //you can play around with this and get all the IP arrays (if any)
                    //and check the connection with all of then if needed
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                LogWriterHandle.WriteLog(e.ToString());
            }
            finally { }

            return hostName;
        }
    }
}