using GIKCore.Sound;
using GIKCore.Utilities;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTime;
    private Color textColor;
    //private const float DISAPPER_TIME_MAX = 0.3f;
    private const float DISAPPER_TIME_MAX = 1f;
    Vector3 basePosition;
    Vector3 baseScale;
    float moveYSpeed;
    PopupType typeText;
    public ICallback.CallFunc onComplete;

    private void Awake()
    {
        //textMesh = transform.GetChild(0).GetComponent<TextMeshPro>();
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
    public static DamagePopup Create(Vector3 position, long text, PopupType type, ICallback.CallFunc callback = null)
    {
        Transform damagePopupTransform;
        if (type == PopupType.Bonus)
        {
           damagePopupTransform = PoolManager.Pools["DamagePopup"].Spawn(Resources.Load<Transform>("Prefabs/DamagePopup"), Vector3.zero, Quaternion.identity);
        }
        else
        {
            damagePopupTransform = PoolManager.Pools["DamagePopup"].Spawn(Resources.Load<Transform>("Prefabs/Dmg"), Vector3.zero, Quaternion.identity);
        }
        damagePopupTransform.position = position + new Vector3(0, 0.2f, 0);
        damagePopupTransform.localRotation = Quaternion.Euler(new Vector3(-70, 0, 0));
        //damagePopupTransform.localScale = new Vector3(-1, 1, 1);

        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(text, type, callback);

        return damagePopup;
    }

    public void Setup(long damageAmount, PopupType type, ICallback.CallFunc callback)
    {
        typeText = type;
        if (type == PopupType.Bonus)
        {
            textMesh = transform.GetComponent<TextMeshPro>();
        }
        else
        {
            textMesh = transform.GetChild(0).GetComponent<TextMeshPro>();
        }
        textMesh.text = (type == PopupType.Damage ? "-" : "+") + damageAmount.ToString("F0");
        textColor = type == PopupType.Damage ? Color.white : Color.green;
        textMesh.fontSize = type == PopupType.Damage ? 9 : 2;
        //moveYSpeed = 0.2f;
        moveYSpeed = 0.4f;
        disappearTime = DISAPPER_TIME_MAX;
        textMesh.color = textColor;
        if(type==PopupType.Bonus)
        {
            SoundHandler.main.PlaySFX("Buff", "sounds");
        }    
        if (callback != null)
        {
            onComplete = callback;
        }
    }

    private void Update()
    {
        if (typeText == PopupType.Bonus)
        {


            transform.position += new Vector3(0, moveYSpeed) /*transform.forward*/ * Time.deltaTime;
            disappearTime -= Time.deltaTime;
            //transform.localScale += new Vector3(0.005f, 0.005f, 0);
            transform.localScale += new Vector3(0.001f, 0.001f, 0);
            if (disappearTime < 0)
            {
                // Start disappear
                //float disappearSpeed = 10f;
                float disappearSpeed = 5f;
                textColor.a -= disappearSpeed * Time.deltaTime;
                textMesh.color = textColor;
                if (textColor.a < 0)
                {
                    onComplete?.Invoke();
                    PoolManager.Pools["DamagePopup"].Despawn(gameObject.transform);
                }
            }
        }
    }

    void OnDespawned()
    {
        transform.localScale = baseScale;
        transform.position = basePosition;
    }
}
