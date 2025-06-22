namespace GIKCore.Net
{
    public class NetData
    {
        public const int SOCKET_CONNECT_SUCCESS = -1;
        public const int SOCKET_ERROR = -2;
        public const int SOCKET_DISCONNECT_READ = -3;
        public const int SOCKET_SERVER_NULL = -4;

        public const int CANVAS_ADJUST = 0;        
        public const int GET_KEY_DOWN = 1;
        public const int GET_DEVICE_UUID = 2;
        public const int WEBGL_PASTED = 3;
        public const int CLOSE_POPUP = 4;

        public const int IAP_INIT_PASS = 10;
        public const int IAP_PURCHASE_SUCCESS = 11;
        public const int IAP_PURCHASE_FAIL = 12;


        public const int DOWNLOAD_RES_STARTED = 100;
        public const int DOWNLOAD_RES_PROGRESS = 101;
        public const int DOWNLOAD_RES_FINISHED = 102;
        public const int DOWNLOAD_RES_FAILED = 103;

        public const int CARD_CHANGE_STAGE = 200;
        public const int CARD_UPDATE_MATRIX = 201;
        public const int Card_UPDATE_ULTI_STAGE = 202;

        public const int RECEIVE_SHOP_PACK_DATA = 301;
        public const int RECEIVE_SHOP_VALUE_DATA = 302;
        public const int GO_TO_OPEN_PACK = 303;
        public const int SHOP_CLICK_ITEM = 304;
        public const int HOME_LOAD_MAP_LEVEL = 305;
        public const int DO_CLICK_OPEN_HOME_LEVEL = 306;
        public const int RECEIVE_LOCAL_BALANCE = 307;
        public const int PREVIEW_CARD = 308;

        public const int ACTION_OPEN_SCENE = 401;


        public int id;
        public object data;

        public NetData(int id, object data = null)
        {
            this.id = id;
            this.data = data;
        }
    }

    public class SocketData
    {
        public int id;
        public byte[] data;

        public SocketData(int id, byte[] data)
        {
            this.id = id;
            this.data = data;
        }
    }

    public class HttpResponseData
    {
        public const string ERROR = "Error message: ";
        public const string ERROR_CONNECT_HOST = "Cannot connect to destination host";
        public const string ERROR_RESOLVE_HOST = "Cannot resolve destination host";

        public const int GET_CONFIG = 1;
        public const int GET_RESOURCE = 2;
        public const int GET_FACEBOOK_ID = 3;
        public const int GET_DOMAIN_STEP_1 = 4;
        public const int GET_DOMAIN_STEP_2 = 5;

        public int id;
        public string data;

        public HttpResponseData(int id, string data)
        {
            this.id = id;
            this.data = data;
        }

        public static bool IsErrorOrEmpty(string data)
        {
            return string.IsNullOrEmpty(data) || IsError(data);
        }
        public static bool IsError(string data)
        {
            return data.Contains(ERROR) || data.Equals(ERROR_CONNECT_HOST) || data.Equals(ERROR_RESOLVE_HOST);
        }
    }
}