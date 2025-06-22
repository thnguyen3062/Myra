using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEditor;
using GIKCore.UI;
using UnityEngine.Events;

namespace GIKCore.Utilities
{
    public class IUtil
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Capture Screenshot")]
        public static void CaptureScreenshot()
        {
            string label = Application.dataPath + "/screenshot" + System.DateTime.Now.Ticks + ".png";
            ScreenCapture.CaptureScreenshot(label);
            Debug.Log("Capture screenshot successful!");
        }
#endif
        public static Object LoadPrefab(string path)
        {
            return Resources.Load<Object>(path);
        }
        public static Sprite LoadSprite(string path)
        {
            return Resources.Load<Sprite>(path);
        }
        public static Sprite[] LoadSpriteMultipe(string path)
        {
            return Resources.LoadAll<Sprite>(path);
        }
        public static TextAsset LoadTextAsset(string path)
        {
            return Resources.Load<TextAsset>(path);
        }
        public static string LoadTextAsset2(string path)
        {
            TextAsset ret = LoadTextAsset(path);
            if (ret != null) return ret.ToString();
            return "";
        }
        public static AudioClip LoadAudioClip(string path)
        {
            return Resources.Load<AudioClip>(path);
        }
        public static GameObject LoadPrefabWithParent(string path, Transform parent, string label = "")
        {
            object temp = LoadPrefab(path);
            if (temp != null)
            {
                GameObject go = Object.Instantiate(LoadPrefab(path), parent) as GameObject;
                if (!string.IsNullOrEmpty(label)) go.name = label;
                return go;
            }
            else
                return null;

        }
        public static Transform LoadPrefabRecycle(string path, string assetName, Transform parent, string label = "")
        {
            if (string.IsNullOrEmpty(label)) label = assetName;
            Transform target = parent.Find(label);
            if (target == null)
            {
                GameObject go = LoadPrefabWithParent(path + assetName, parent, label);
                if (go != null)
                    target = go.transform;
            }
            if (target != null)
                target.gameObject.SetActive(true);
            else
                LogWriterHandle.WriteLog("No Target With ID " + assetName + "Object found");
            return target;
        }
        public static GameObject PrefabClone(GameObject original, Transform parent, string label = "")
        {
            GameObject go = Object.Instantiate(original, parent);
            go.name = string.IsNullOrEmpty(label) ? original.name : label;
            return go;
        }

        // ------- Some useful methods -------
        public static string UriQueryAppend(string uri, string queryToAppend)
        {
            try
            {
                System.UriBuilder uriBuilder = new System.UriBuilder(uri);
                if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
                    uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + queryToAppend;
                else uriBuilder.Query = queryToAppend;
                return uriBuilder.ToString();
            }
            catch (System.Exception e)
            {
                return uri;
            }
            finally { }
        }
        public static string UriSetParam(string uri, Dictionary<string, string> collection)
        {
            try
            {
                System.UriBuilder uriBuilder = new System.UriBuilder(uri);
                NameValueCollection qs = Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                foreach (KeyValuePair<string, string> pair in collection)
                {
                    qs.Set(pair.Key, pair.Value);
                }
                uriBuilder.Query = qs.ToString();
                return uriBuilder.ToString();
            }
            catch (System.Exception e)
            {
                return uri;
            }
            finally { }
        }
        public static string UriSetParam(string uri, string key, string value)
        {
            try
            {
                System.UriBuilder uriBuilder = new System.UriBuilder(uri);
                NameValueCollection qs = Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                qs.Set(key, value);
                uriBuilder.Query = qs.ToString();
                return uriBuilder.ToString();
            }
            catch (System.Exception e)
            {
                return uri;
            }
            finally { }
        }
        public static string UriGetParam(string uri, string key)
        {
            //try
            //{
            //    System.UriBuilder uriBuilder = new System.UriBuilder(uri);
            //    NameValueCollection qs = HttpUtility.ParseQueryString(uriBuilder.Query);
            //    return qs.Get(key);
            //}
            //catch (System.Exception e)
            //{
            //    return "";
            //}
            //finally { }
            return "";
        }

        public static void CopyToClipBoard(string value, string toast = "Copy to Clipboard")
        {
#if UNITY_WEBGL
        WebGLNative.CopyToClipboardJS(value);
#else
            TextEditor te = new TextEditor();
            te.text = value;
            te.SelectAll();
            te.Copy();
#endif

            if (!string.IsNullOrEmpty(toast)) Toast.Show(toast);
        }
        public static void InvokeEvent(params UnityEvent[] args)
        {
            foreach (UnityEvent ue in args)
                if (ue != null) ue.Invoke();
        }
        public static void InvokeEvent<T>(T data, params UnityEvent<T>[] args)
        {
            foreach (UnityEvent<T> ue in args)
            {
                if (ue != null) ue.Invoke(data);
            }
        }
        public static void AddEvent(UnityEvent ue, params UnityAction[] args)
        {
            if (ue == null)
                ue = new UnityEvent();
            foreach (UnityAction ua in args)
                ue.AddListener(ua);
        }
        public static void RemoveEvent(UnityEvent ue, params UnityAction[] args)
        {
            if (ue != null)
            {
                foreach (UnityAction ua in args)
                    ue.RemoveListener(ua);
            }
        }
        public static void AddEvent<T>(UnityEvent<T> ue, params UnityAction<T>[] args)
        {
            if (ue == null)
                ue = new UnityEvent<T>();
            foreach (UnityAction<T> ua in args)
                ue.AddListener(ua);
        }
        public static void RemoveEvent<T>(UnityEvent<T> ue, params UnityAction<T>[] args)
        {
            if (ue != null)
            {
                foreach (UnityAction<T> ua in args)
                    ue.RemoveListener(ua);
            }
        }
        public static bool IsActiveSceneSameAs(string scene)
        {
            return SceneManager.GetActiveScene().name.Equals(scene);
        }
        public static IEnumerator Delay(ICallback.CallFunc callback, float seconds = 0f)
        {
            yield return new WaitForSeconds(seconds);
            if (callback != null) callback();
        }
        public static IEnumerator DelayForEndOfFrame(ICallback.CallFunc callback)
        {
            yield return new WaitForEndOfFrame();
            if (callback != null) callback();
        }
        public static IEnumerator DelayUtil(System.Func<bool> predicate, ICallback.CallFunc callback)
        {
            yield return new WaitUntil(predicate);
            if (callback != null) callback();
        }
        public static IEnumerator LoadTexture2DFromUrl(string url, ICallback.CallFunc2<Texture2D> onLoaded)
        {
            if (string.IsNullOrEmpty(url) && onLoaded != null)
            {
                onLoaded(null);
                yield return null;
            }

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                www.timeout = 20;
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("------- " + www.error);
                }
                else
                {
                    if (onLoaded != null)
                        onLoaded(DownloadHandlerTexture.GetContent(www));
                }
            }
        }
        public static int RandomInt(int fromInclusive, int toInclusive, int time = 1)
        {
            time = Mathf.Max(time, 1);
            int result = fromInclusive;
            for (int i = 0; i < time; i++)
            {
                float tmp = fromInclusive + (toInclusive - fromInclusive) * UnityEngine.Random.value;
                result = System.Convert.ToInt32(tmp);
            }
            return result;
        }
        public static long RandomLong(long fromInclusive, long toInclusive, int time = 1)
        {
            time = Mathf.Max(time, 1);
            long result = fromInclusive;
            for (int i = 0; i < time; i++)
            {
                float tmp = fromInclusive + (toInclusive - fromInclusive) * UnityEngine.Random.value;
                result = System.Convert.ToInt64(tmp);
            }
            return result;
        }
        public static float RandomFloat(float fromInclusive, float toInclusive, int time = 1)
        {
            time = Mathf.Max(time, 1);
            float result = fromInclusive;
            for (int i = 0; i < time; i++)
            {
                float rnd = Random.value;
                result = fromInclusive + (toInclusive - fromInclusive) * rnd;
            }
            return result;
        }
        public static float ToDegree(float rad)
        {
            return rad * (180f / Mathf.PI);
        }
        public static float ToRadian(float degree)
        {
            return degree * (Mathf.PI / 180f);
        }
        public static float AngleBetweenVector2(Vector2 v1, Vector2 v2)
        {
            Vector2 dif = v2 - v1;
            float sign = (v2.y < v1.y) ? -1f : 1f;
            return Vector2.Angle(Vector2.right, dif) * sign;
        }
        public static List<int> Shuffle(int fromInclusive, int toExclusive, int time = 5)
        {
            List<int> lst = new List<int>();
            for (int i = fromInclusive; i < toExclusive; i++)
                lst.Add(i);
            int num = lst.Count;
            while (num > 1)
            {
                num--;
                int k = RandomInt(0, num, time);
                int tmp = lst[k];
                lst[k] = lst[num];
                lst[num] = tmp;
            }
            return lst;
        }
        public static List<int> ConvertListLongToListInt(List<long> lstLong)
        {
            List<int> lstInt = new List<int>();
            int numLong = lstLong.Count;
            for (int i = 0; i < numLong; i++)
            {
                int tmp = unchecked((int)lstLong[i]);// It'll throw OverflowException in checked context if the value doesn't fit in an int:
                lstInt.Add(tmp);
            }

            return lstInt;
        }
        public static List<long> ConvertListIntToListLong(List<int> lstInt)
        {
            List<long> lstLong = new List<long>();
            int numLong = lstInt.Count;
            for (int i = 0; i < numLong; i++)
            {
                lstLong.Add(lstInt[i]);
            }

            return lstLong;
        }

        public static string ConvertListLongToString(List<long> lstInt)
        {
            string dt = "";
            foreach (long l in lstInt)
                dt += l + ", ";

            return dt;
        }

        public static string ConvertListStringToString(List<string> lstStr)
        {
            string dt = "";
            foreach (string l in lstStr)
                dt += l + ", ";

            return dt;
        }

        public static List<int> GetDigitsI(string s)
        {
            List<int> ret = new List<int>();
            string[] digits = Regex.Split(s, @"\D+");
            foreach (string v in digits)
            {
                int num;
                if (int.TryParse(v, out num))
                    ret.Add(num);
            }
            return ret;
        }
        public static List<long> GetDigitsL(string s)
        {
            List<long> ret = new List<long>();
            string[] digits = Regex.Split(s, @"\D+");
            foreach (string v in digits)
            {
                long num;
                if (long.TryParse(v, out num))
                    ret.Add(num);
            }
            return ret;
        }
        public static List<float> GetDigitF(string s)
        {
            List<float> ret = new List<float>();
            string[] digits = Regex.Split(s, @"\D+");
            foreach (string v in digits)
            {
                float num;
                if (float.TryParse(v, out num))
                    ret.Add(num);
            }
            return ret;
        }

        // lấy toàn bộ số tổ hợp chập có k phần tử của một List
        public static List<List<T>> GetCombinationOfAList<T>(int k, List<T> lst)
        {
            int n = (lst == null) ? 0 : lst.Count;
            List<List<T>> result = new List<List<T>>();
            if (n > 0 && n >= k)
            {
                int[] tmp = new int[n + 1];
                ICallback.CallFunc print = () =>
                {
                    List<T> combination = new List<T>();
                    for (int i = 1; i <= k; i++)
                    {
                        int index = tmp[i] - 1;
                        combination.Add(lst[index]);
                    }
                    if (combination.Count == k)
                        result.Add(combination);
                };

                //init            
                for (int i = 1; i <= k; i++)
                    tmp[i] = i;
                int idx = 0;
                do
                {
                    print();
                    idx = k;
                    while (idx > 0 && tmp[idx] == (n - k + idx)) --idx;
                    if (idx > 0)
                    {
                        tmp[idx]++;
                        for (int i = idx + 1; i <= k; i++)
                        {
                            tmp[i] = tmp[i - 1] + 1;
                        }
                    }
                } while (idx != 0);
            }

            return result;
        }
        public static string SplitString(string content, int lenghtMax, string more = "...")
        {
            string result = "";
            string contentTrim = content.Trim();
            char[] arrText = contentTrim.ToArray();
            for (int i = 0; i < arrText.Length; i++)
            {
                if (i > lenghtMax)
                {
                    result += more;
                    break;
                }
                result += arrText[i];
            }
            return result;
        }
        public static string StringToUpperFirstChar(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
        public static string StringToUpperFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            string[] split = input.Split(' ');
            for (int i = 0; i < split.Length; i++)
            {
                split[i] = split[i].First().ToString().ToUpper() + split[i].Substring(1);
            }

            return string.Join(" ", split);
        }

        public static string FormatKoinK(long koin)
        {
            return string.Format("{0:#,##0.##}", koin);
        }
        public static string FormatKoinK(float koin)
        {
            return string.Format("{0:#,##0.##}", koin);
        }
        public static string FormatKoinAndRoundUp(long koin, long minRoundUp = 100 * 1000 * 1000, string split = "")
        {
            string suffix = "";
            long absKoin = System.Math.Abs(koin), factor = 1;

            if (absKoin >= minRoundUp)
            {
                if (absKoin >= 1000000000)
                {
                    factor = 1000000000;
                    suffix = "B";
                }
                else if (absKoin >= 1000000)
                {
                    factor = 1000000;
                    suffix = "M";
                }
                else if (absKoin >= 1000)
                {
                    factor = 1000;
                    suffix = "K";
                }
            }

            float round = (float)koin / factor;
            return string.Format("{0:#,##0.##}", round) + split + suffix;
        }
        public static void SetMapRewardSprite(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            IEnumerator coroutine = IUtil.LoadTexture2DFromUrl(url, (Texture2D tex) =>
            {
                if (tex == null) return;
                Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                GameData.main.DictRewardSprite.TryAdd(url, sprite);
            });
            Game.main.StartCoroutine(coroutine);
        }
        public static string FormatKoinAndRoundUpF(float koin, float minRoundUp = 100f * 1000f * 1000f)
        {
            string suffix = "";
            float absKoin = System.Math.Abs(koin), factor = 1f;

            if (absKoin >= minRoundUp)
            {
                if (absKoin >= 1000000000f)
                {
                    factor = 1000000000f;
                    suffix = "B";
                }
                else if (absKoin >= 1000000f)
                {
                    factor = 1000000f;
                    suffix = "M";
                }
                else if (absKoin >= 1000f)
                {
                    factor = 1000f;
                    suffix = "K";
                }
            }

            float round = koin / factor;
            return string.Format("{0:#,##0.##}", round) + suffix;
        }

        // ----------------------------------------------------------------------
        // ----------------------------------------------------------------------
        // --------------------- DEFINE OWN FUNCTION BELOW ---------------------
        // ----------------------------------------------------------------------
        // ----------------------------------------------------------------------
        public static bool IsSplashScene()
        {
            return IsActiveSceneSameAs("SplashScene");
        }
        public static string StringFormat(string provider, params object[] args)
        {
            string ret = provider;
            for (int i = 0; i < args.Length; i++)
            {
                string key = "{" + i + "}";
                string value = "" + args[i];
                ret = ret.Replace(key, value);
            }
            return ret;
        }
        /// <summary>
        /// Use Scheduler system to schedule a task that runs only once, with a delay of 0 or larger.
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="callback"></param>
        /// <param name="delay">The delay time for the first invocation, Unit: s</param>
        public static void ScheduleOnce(MonoBehaviour behaviour, ICallback.CallFunc callback, float delay = 0)
        {
            if (delay > 0) behaviour.StartCoroutine(Delay(callback, delay));
            else if (callback != null) callback();
        }
        /// <summary>
        /// Use Scheduler system to schedule a custom task
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="callback"></param>
        /// <param name="interval">The time interval between each invocation</param>
        /// <param name="repeat">The repeat count of this task, if repeat < 0 => repeat forever; if repeat >= 0 => the task will be invoked (repeat + 1) times</param>
        public static void Schedule(MonoBehaviour behaviour, ICallback.CallFunc callback, float interval, int repeat = -1)
        {
            behaviour.StartCoroutine(DelayRepeat(callback, interval, repeat));
        }
        public static IEnumerator DelayRepeat(ICallback.CallFunc callback, float interval, int repeat = -1)
        {
            if (repeat < 0)//forever
            {
                while (true)
                {
                    yield return new WaitForSeconds(interval);
                    if (callback != null) callback();
                }
            }
            else
            {
                for (int i = 0; i < repeat + 1; i++)
                {
                    yield return new WaitForSeconds(interval);
                    if (callback != null) callback();
                }
            }
        }

        public static void OpenURL(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
#if UNITY_EDITOR
            Application.OpenURL(url);
#elif UNITY_WEBGL
            WebGLNative.OpenUrlJS(url);
#elif (UNITY_ANDROID || UNITY_IOS)
            //int webview; int.TryParse(UriGetParam(url, "webview"), out webview);
            //if (webview != 0)
            //{
            //    url = UriSetParam(url, "platform", "webview");
            PopupWebview.Show(url);
            //}
            //else Application.OpenURL(url);
#else
            Application.OpenURL(url);
#endif
        }
    }
}