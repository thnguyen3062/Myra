using UnityEngine;

public class Sc_Slowmo : MonoBehaviour
{
    [SerializeField] private float slowdownFactor = 0.05f;
    [SerializeField] private float slowdownLength = 2f;


    void Update()
    {
        Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
        Time.fixedDeltaTime += (0.01f / slowdownLength) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        Time.fixedDeltaTime = Mathf.Clamp(Time.fixedDeltaTime, 0f, 0.01f);
    }

    public void DoSlowmotion ()
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * slowdownFactor;
    }
}
                                                                                             