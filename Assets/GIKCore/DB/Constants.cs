namespace GIKCore.DB
{
    public class CPManager
    {
        public const string DEFAULT = "hq";
        public const string FULL = "full";
    }

    public class Constants
    {
        public static string DEVICE_ID = "";
        public static string DEVICE_NAME
        {
            get
            {
                string deviceName = UnityEngine.SystemInfo.deviceName;
                return string.IsNullOrEmpty(deviceName) ? DEVICE_ID : deviceName;
            }
        }

        public static string CLIENT_MCC = "-1";
        public static string CLIENT_IP = "";
        public static string CP_CODE = CPManager.DEFAULT;
        //version code 13   
#if UNITY_ANDROID
        public const string OS_TYPE = "android";
        public const string PLATFORM = "ANDROID";
        public const string PKN = "com.mytheriaccg.io";
        public const string VERSION = "1.108";
#elif UNITY_IOS
    public const string OS_TYPE = "IPHONE";
	public const string PLATFORM = "APPLE"; 
    public const string PKN = "com.mytheriaccg.io";
    public const string VERSION = "1.0.5";
#elif UNITY_WEBGL
    public const string OS_TYPE = "web";
    public const string PLATFORM = "web";
    public const string PKN = "com.mytheriaccg.io";
    public const string VERSION = "1.0.5";
#elif UNITY_STANDALONE
    public const string OS_TYPE = "android";//"standalone";
    public const string PLATFORM = "ANDROID";//"STANDALONE";
    public const string PKN = "com.mytheriaccg.io";
    public const string VERSION = "1.108";
#endif

        /// <summary>
        /// vietnam: 452
        /// </summary>
        /// <param name="mcc"></param>
        /// <returns></returns>
        public static bool CheckClientMCC(string mcc) { return CLIENT_MCC.Equals(mcc); }
        public static readonly string CLIENT_MCC_VN = "452";

        public enum LoginType { None, Normal, Quick, Facebook, Logout }
    }
}