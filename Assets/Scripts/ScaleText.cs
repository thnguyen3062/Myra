using GIKCore.Utilities;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScaleText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private float disappearTime;
    private Color textColor;
    private const float DISAPPER_TIME_MAX = 0.3f;
    Vector3 basePosition;
    Vector3 baseScale;
    float moveYSpeed;
    ICallback.CallFunc onComplete;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        basePosition = transform.position;
        baseScale = transform.localScale;
    }

    void OnSpawned()
    {
        if(onComplete != null)
        {
            onComplete = null;
        }
    }

    // Create Damage Popup
    public static ScaleText Create(Vector3 position, long damageAmount, Transform parent, ICallback.CallFunc callback = null)
    {
        Transform damagePopupTransform = PoolManager.Pools["ScaleText"].Spawn(Resources.Load<Transform>("Prefabs/ScaleText"), Vector3.zero, Quaternion.identity, parent );
        damagePopupTransform.position = position + new Vector3(0, 0.2f, 0);
        damagePopupTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        //damagePopupTransform.localScale = new Vector3(-1, 1, 1);

        ScaleText damagePopup = damagePopupTransform.GetComponent<ScaleText>();
        damagePopup.Setup(damageAmount, callback);

        return damagePopup;
    }

    public void Setup(long damageAmount,  ICallback.CallFunc callback)
    {
       textMesh.text = damageAmount.ToString("F0");
       textColor = Color.white;
        textMesh.fontSize = 24 ;
        moveYSpeed = 0.2f;
        disappearTime = DISAPPER_TIME_MAX;
        textMesh.color = textColor;

        if (callback != null)
        {
            onComplete = callback;
        }
    }

    private void Update()
    {
        transform.position += new Vector3(0, moveYSpeed) /*transform.forward*/ * Time.deltaTime;
        disappearTime -= Time.deltaTime;
        transform.localScale += new Vector3(0.005f, 0.005f, 0);
        if (disappearTime < 0)
        {
            // Start disappear
            float disappearSpeed = 10f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                onComplete?.Invoke();
                PoolManager.Pools["ScaleText"].Despawn(gameObject.transform);
            }
        }
    }

    void OnDespawned()
    {
        transform.localScale = baseScale;
        transform.position = basePosition;
    }
}
