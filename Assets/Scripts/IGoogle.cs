#if !UNITY_STANDALONE
using Firebase.Auth;
using GIKCore;
using GIKCore.Utilities;
using Google;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class IGoogle : MonoBehaviour
{

    private void Awake()
    {
        Game.main.google = this;

        //configuration = new GoogleSignInConfiguration
        //{
        //    WebClientId = webClientId,
        //    RequestEmail = true,
        //    RequestIdToken = true
        //};
    }

    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
                Debug.Log("MYTHERIA: FIREBASE INIT SUCCESS");
            else
                Debug.Log("MYTHERIA: FIREBASE INIT FAILED " + dependencyStatus);
        });
    }

    private string webClientId = "803875045544-hpk6oifodc363cmsve0o95bgr2p9s36b.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;

    private ICallback.CallFunc2<string> OnFailed;
    private ICallback.CallFunc2<string> OnSucceeded;
    private ICallback.CallFunc OnCanceled;

    public void OnSignIn(ICallback.CallFunc2<string> success, ICallback.CallFunc canceled, ICallback.CallFunc2<string> failed)
    {
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestEmail = true,
            RequestIdToken = true
        };
        //GoogleSignIn.Configuration = configuration;
        //GoogleSignIn.Configuration.UseGameSignIn = false;
        //GoogleSignIn.Configuration.RequestIdToken = true;
        //GoogleSignIn.Configuration.RequestEmail = true;

        AddStatusText("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
        OnFailed = failed;
        OnSucceeded = success;
        OnCanceled = canceled;
    }

    public void OnSignOut()
    {
        AddStatusText("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        AddStatusText("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator();
            if (enumerator.MoveNext())
            {
                GoogleSignIn.SignInException error =
                        (GoogleSignIn.SignInException)enumerator.Current;
                AddStatusText("Got Error: " + error.Status + " " + error.Message);
            }
            else
            {
                AddStatusText("Got Unexpected Exception?!?" + task.Exception);
            }
        }
        else if (task.IsCanceled)
        {
            AddStatusText("Canceled");
        }
        else
        {
            AddStatusText("Welcome: " + task.Result.DisplayName + "!");
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            //OnSucceeded?.Invoke(task.Result.IdToken);
            Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
            auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
            {
                if (authTask.IsCanceled)
                {
                    OnCanceled?.Invoke();
                }
                else if (authTask.IsFaulted)
                {
                    OnFailed?.Invoke(authTask.Result.ToString());
                }
                else
                {
                    OnSucceeded?.Invoke(task.Result.IdToken);
                }
            });

        }
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddStatusText("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently()
              .ContinueWith(OnAuthenticationFinished);
    }


    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        AddStatusText("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }

    private List<string> messages = new List<string>();
    void AddStatusText(string text)
    {
        if (messages.Count == 5)
        {
            messages.RemoveAt(0);
        }
        messages.Add(text);
        string txt = "";
        foreach (string s in messages)
        {
            txt += "\n" + s;
        }
        Debug.Log("GOOGLE SIGNIN: " + text);
    }
}
#endif
