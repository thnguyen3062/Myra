using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using GIKCore.Bundle;

public class SC_UpgradeCard : MonoBehaviour
{
    public SC_Info_Upgrade InfoScript;
    public SC_Button_Upgrade ButtonScript;
    [SerializeField] private GameObject Card;
    private Image CardImage;
    [SerializeField] private GameObject BackCard;
    private Image BackCardImage;
    [SerializeField] private GameObject EndPoint;
    [SerializeField] private GameObject StartPoint;
    [SerializeField] private GameObject ObjectToShake;
    private CamShakeScript_UI camShakeScript;
    [SerializeField] private GameObject InwardFlow;
    [SerializeField] private GameObject AuraRing;
    [SerializeField] private GameObject InwardCircle;
    [SerializeField] private GameObject PlasmaOutward;
    [SerializeField] private GameObject CardGlow;
    [SerializeField] private GameObject Rays;
    [SerializeField] private GameObject RaysFlick;
    [SerializeField] private GameObject Wind;
    [SerializeField] private GameObject Explosion;
    [SerializeField] private GameObject AuraBack;
    [SerializeField] private GameObject FrameParticle;
    private ParticleSystem FrameParticleChild;
    [SerializeField] private GameObject RainbowParticle;
    [SerializeField] private GameObject Reward;
    private Image RewardImage;
    [SerializeField] private GameObject RewardBack;
    private Image RewardBackImage;
    private Material RewardFrameMat;
    private ParticleSystem RainbowParticleChild;
    private Image raysImg;
    private Image raysFlickImg;
    private Material RaysFlickMat;
    private Material InfMat;
    private Material InCircMat;
    private Material ExpMat;
    private Material WindMat;
    private Material BackMat;
    private Material PlasmaMat;
    private Material PlasmaOutMat;
    private Material AuraRingMat;
    private Material AuraBackMat;
    private Material CardFrameMat;
    private Material CardGlowMat;
    [SerializeField] private GameObject RainbowParticleHor;
    private ParticleSystem RainbowHorParticleChild;
    public bool isCardSeqPlaying = false;
    public Sequence CardSeq;
    public Sc_Slowmo_UI slow_mo;
    private GameObject ParticleRainbowInstance;
    private GameObject ParticleRainbowInstanceHor;
    private GameObject BlinkPrefabInstance;
    [SerializeField] private GameObject Success;
    private CanvasGroup SuccessCanvasGroup;



    // Start is called before the first frame update
    void OnEnable()
    {
        ButtonScript.OnUpgradeButtonClick += () => PlayCardAnim(InfoScript.LevelVar);

        if (ObjectToShake != null)
        {
            camShakeScript = ObjectToShake.GetComponent<CamShakeScript_UI>();
        }

        //TurnOff(
        //    InwardFlow,
        //    AuraRing,
        //    PlasmaOutward,
        //    InwardCircle,
        //    CardGlow,
        //    Rays,
        //    RaysFlick,
        //    Wind,
        //    Explosion,
        //    AuraBack,
        //    Reward,
        //    Success);

        raysImg = Rays.GetComponent<Image>();
        raysFlickImg = RaysFlick.GetComponent<Image>();
        RaysFlickMat = raysFlickImg.material;
        InfMat = InwardFlow.GetComponent<UIMeshRenderer>().UIMaterial;
        ExpMat = Explosion.GetComponent<Image>().material;
        WindMat = Wind.GetComponent<UIMeshRenderer>().UIMaterial;
        BackMat = BackCard.GetComponent<Image>().material;
        PlasmaMat = PlasmaOutward.GetComponent<UIMeshRenderer>().UIMaterial;
        AuraRingMat = AuraRing.GetComponent<UIMeshRenderer>().UIMaterial;
        AuraBackMat = AuraBack.GetComponent<Image>().material;
        //CardFrameMat = Card.GetComponent<Image>().material;
        CardGlowMat = CardGlow.GetComponent<Image>().material;
        PlasmaOutMat = PlasmaOutward.GetComponent<UIMeshRenderer>().UIMaterial;
        InCircMat = InwardCircle.GetComponent<UIMeshRenderer>().UIMaterial;
        //CardImage = Card.GetComponent<Image>();
        BackCardImage = BackCard.GetComponent<Image>();
        RewardImage = Reward.GetComponent<Image>();
        RewardBackImage = RewardBack.GetComponent<Image>();
        RewardFrameMat = RewardImage.material;
        SuccessCanvasGroup = Success.GetComponent<CanvasGroup>();

        ParticleRainbowInstance = InstantiatePrefab(RainbowParticleHor, ref RainbowHorParticleChild);
        ParticleRainbowInstanceHor = InstantiatePrefab(RainbowParticle, ref RainbowParticleChild);
        BlinkPrefabInstance = InstantiatePrefab(FrameParticle, ref FrameParticleChild);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isCardSeqPlaying)
            {
                // If the sequence is playing, complete it immediately
                CardSeq.Complete(withCallbacks: true);
                return;
            }
        }
    }

    public void PlayCardAnim(int level)
    {
        TurnOff(InwardFlow,
            AuraRing,
            PlasmaOutward,
            InwardCircle,
            Card,
            CardGlow,
            Rays,
            RaysFlick,
            Wind,
            Explosion,
            AuraBack,
            Reward,
            Success);

        if (level > 5 || level < 1)
        {
            level = 1;
        }

        isCardSeqPlaying = true;

        TurnOff(CardGlow);

        TurnOffIfInstantiated(
            ParticleRainbowInstance,
            ParticleRainbowInstanceHor,
            BlinkPrefabInstance);

        Texture2D loadedTexture = (Texture2D)RewardImage.sprite.texture; //Texture cho reward không phải cards.

        CardSeq = DOTween.Sequence();

        switch (level)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                {
                    TurnOffIfInstantiated(
                    ParticleRainbowInstance,
                    ParticleRainbowInstanceHor,
                    BlinkPrefabInstance);

                    // LoadAndAssignTexture(CardFrameMat, level, "_GradientTex");
                    if (CardGlowMat != null)
                        LoadAndAssignTexture(CardGlowMat, level, "_GradientTex");
                    if (BackMat != null)
                        LoadAndAssignTexture(BackMat, level, "_GradientTex");
                    if (InfMat != null)
                        LoadAndAssignTexture(InfMat, level, "_GradientTex");
                    if (AuraRingMat != null)
                        LoadAndAssignTexture(AuraRingMat, level, "_GradientTex");
                    if (AuraBackMat != null)
                        LoadAndAssignTexture(AuraBackMat, level, "_GradientTex");
                    if (InCircMat != null)
                        LoadAndAssignTexture(InCircMat, level, "_GradientTex");
                    if (ExpMat != null)
                        LoadAndAssignTexture(ExpMat, level, "_GradientTex");
                    if (PlasmaOutMat != null)
                        LoadAndAssignTexture(PlasmaOutMat, level, "_GradientTex");
                    if (WindMat != null)
                        LoadAndAssignTexture(WindMat, level, "_GradientTex");
                    if (RaysFlickMat != null)
                        LoadAndAssignTexture(RaysFlickMat, level, "_GradientTex");

                    CardSeq.AppendCallback(() => TurnOn(Card, InwardFlow, AuraRing, BackCard, AuraBack, Rays, RaysFlick, Wind, BlinkPrefabInstance, ParticleRainbowInstance, ParticleRainbowInstanceHor));
                    if (Card != null)
                    {
                        CardSeq.Append(Card.transform.DOLocalMove(StartPoint.transform.localPosition, 0.01f)); //Đến start point
                        CardSeq.Join(Card.transform.DOLocalRotate(new Vector3(0, -90, 0), 0.01f, RotateMode.Fast)); //Xoay vào vị trí
                    }
                    // CardSeq.Join(CardFrameMat.DOFloat(1, "_Intensity", 0.01f));
                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(0, "_Intensity", 0.01f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -0.1f), "_Speed_1", 0.01f));
                    }
                    if (AuraRingMat != null)
                    {
                        CardSeq.Join(AuraRingMat.DOFloat(0, "_Intensity", 0.01f));
                        CardSeq.Join(AuraRingMat.DOVector(new Vector2(-0.1f, -2f), "_Speed_1", 0.01f));
                    }
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0, "_Intensity", 0.01f));
                    if (AuraBackMat != null)
                        CardSeq.Join(AuraBackMat.DOFloat(0, "_Intensity", 0.01f));
                    if (raysImg != null)
                        CardSeq.Join(raysImg.DOFade(0, 0.01f));
                    if (raysFlickImg != null)
                        CardSeq.Join(raysFlickImg.DOFade(0, 0.01f));
                    if (WindMat != null)
                        CardSeq.Join(WindMat.DOFloat(0, "_Intensity", 0.01f));

                    if (Card != null)
                        CardSeq.Append(Card.transform.DOLocalMove(new Vector3(0, EndPoint.transform.localPosition.y - 10, 0), 0.3f)); // Di chuyển đến endpoint
                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(1.5f, "_Intensity", 0.3f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -0.2f), "_Speed_1", 0.01f));
                    }
                    if (Card != null)
                        CardSeq.Append(Card.transform.DOLocalMove(new Vector3(0, EndPoint.transform.localPosition.y - 5, 0), 0.4f));
                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(4, "_Intensity", 0.4f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -0.4f), "_Speed_1", 0.01f));
                    }
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0.3f, "_Intensity", 0.4f));

                    CardSeq.AppendCallback(() => StopBurst(FrameParticleChild, RainbowParticleChild, RainbowHorParticleChild));
                    CardSeq.AppendCallback(() => Slow(2));

                    if (Card != null)
                        CardSeq.Append(Card.transform.DOLocalMove(EndPoint.transform.localPosition, 0.8f));

                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(8, "_Intensity", 0.8f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -1f), "_Speed_1", 0.01f));
                    }
                    if (AuraRingMat != null)
                    {
                        CardSeq.Join(AuraRingMat.DOFloat(8f, "_Intensity", 0.4f).SetDelay(0.4f));
                        CardSeq.Join(AuraRingMat.DOVector(new Vector2(-0.5f, -5f), "_Speed_1", 0.01f));
                    }
                    if (raysFlickImg != null)
                        CardSeq.Join(raysFlickImg.DOFade(1, 0.2f).SetDelay(0.2f));
                    if (raysImg != null)
                        CardSeq.Join(raysImg.DOFade(1, 0.2f).SetDelay(0.7f));
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(1, "_Intensity", 0.8f));
                    if (PlasmaMat != null)
                        CardSeq.Join(PlasmaMat.DOFloat(0, "_Opacity", 0.01f));

                    CardSeq.AppendCallback(() => TurnOn(InwardCircle, Explosion, PlasmaOutward, Success));
                    if (SuccessCanvasGroup != null)
                        CardSeq.Join(SuccessCanvasGroup.DOFade(0, 0.01f));
                    if (InCircMat != null)
                    {
                        CardSeq.Join(InCircMat.DOFloat(0, "_Opacity", 0.01f));
                        CardSeq.Join(InCircMat.DOFloat(0, "_Intensity", 0.01f));
                    }
                    if (PlasmaMat != null)
                        CardSeq.Join(PlasmaMat.DOFloat(0, "_Opacity", 0.01f));
                    if (PlasmaOutward != null)
                        CardSeq.Join(PlasmaOutward.transform.DOScale(0, 0.01f));
                    if (ExpMat != null)
                        CardSeq.Join(ExpMat.DOFloat(0, "_Opacity", 0.01f));
                    if (PlasmaOutMat != null)
                        CardSeq.Join(PlasmaOutMat.DOFloat(0, "_Intensity", 0.01f));
                    if (Explosion != null)
                        CardSeq.Join(Explosion.transform.DOScale(0, 0.001f));

                    CardSeq.AppendCallback(() => StartCoroutine(camShakeScript.Shake(2.5f, 2.5f)));
                    CardSeq.AppendCallback(() => TurnOn(CardGlow));
                    CardSeq.AppendCallback(() => Slow(0.1f));

                    if (Card != null)
                        CardSeq.Append(Card.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.3f, RotateMode.Fast).SetEase(Ease.InOutBack).SetRelative(true)); // Xoay đến endpoint
                    if (PlasmaOutward != null)
                        CardSeq.Join(PlasmaOutward.transform.DOScale(1, 0.02f));
                    if (PlasmaOutMat != null)
                        CardSeq.Join(PlasmaOutMat.DOFloat(8, "_Intensity", 0.01f));
                    if (InCircMat != null)
                    {
                        CardSeq.Join(InCircMat.DOFloat(2.5f, "_Intensity", 0.01f));
                        CardSeq.Join(InCircMat.DOFloat(1, "_Opacity", 0.01f));
                    }
                    if (CardGlowMat != null)
                        CardSeq.Join(CardGlowMat.DOFloat(0.9f, "_Intensity", 0.01f));//CardGlowMat
                    if (Explosion != null)
                        CardSeq.Join(Explosion.transform.DOScale(1, 0.02f).SetDelay(0.2f));
                    if (PlasmaMat != null)
                        CardSeq.Join(PlasmaMat.DOFloat(8f, "_Intensity", 0.01f));
                    if (AuraBackMat != null)
                        CardSeq.Join(AuraBackMat.DOFloat(1.5f, "_Intensity", 0.01f));
                    if (ExpMat != null)
                        CardSeq.Join(ExpMat.DOFloat(0.75f, "_Opacity", 0.01f));
                    if (WindMat != null)
                        CardSeq.Join(WindMat.DOFloat(6, "_Intensity", 0.01f));
                    if (SuccessCanvasGroup != null)
                        CardSeq.Join(SuccessCanvasGroup.DOFade(1, 0.5f));
                    if (raysImg != null)
                        CardSeq.Join(raysImg.DOFade(0, 0.5f));
                    if (raysFlickImg != null)
                        CardSeq.Join(raysFlickImg.DOFade(0, 0.01f));
                    if (InfMat != null)
                        CardSeq.Join(InfMat.DOFloat(0, "_Intensity", 0.1f));
                    if (ExpMat != null)
                    {
                        CardSeq.Join(ExpMat.DOFloat(0.3f, "_Opacity", 0.3f).SetDelay(0.4f));
                        CardSeq.Append(ExpMat.DOFloat(0, "_Opacity", 2f));
                    }
                    if (WindMat != null)
                        CardSeq.Join(WindMat.DOFloat(0, "_Intensity", 2f));
                    if (PlasmaMat != null)
                        CardSeq.Join(PlasmaMat.DOFloat(0, "_Intensity", 3f));
                    if (PlasmaOutMat != null)
                        CardSeq.Join(PlasmaOutMat.DOFloat(0, "_Intensity", 3f));
                    if (AuraRingMat != null)
                        CardSeq.Join(AuraRingMat.DOFloat(0, "_Intensity", 2f));

                    CardSeq.AppendCallback(() => TurnOff(Explosion, InwardFlow, AuraRing, Rays, RaysFlick, Wind, BackCard));

                    // CardSeq.Append(CardFrameMat.DOFloat(0, "_Intensity", 4f));
                    if (SuccessCanvasGroup != null)
                        CardSeq.Append(SuccessCanvasGroup.DOFade(0, 2f).SetDelay(2f));
                    if (CardGlowMat != null)
                        CardSeq.Join(CardGlowMat.DOFloat(0.7f, "_Intensity", 4f));
                    if (InCircMat != null)
                    {
                        CardSeq.Join(InCircMat.DOFloat(0, "_Opacity", 4f));
                        CardSeq.Join(InCircMat.DOFloat(0, "_Intensity", 4f));
                    }

                    CardSeq.AppendCallback(() => TurnOff(InwardCircle, Success, PlasmaOutward));

                    break;
                }
            case 5:
                {
                    TurnOffIfInstantiated(
                    ParticleRainbowInstance,
                    ParticleRainbowInstanceHor,
                    BlinkPrefabInstance);

                    // LoadAndAssignTexture(CardFrameMat, level, "_GradientTex");
                    if (CardGlowMat != null)
                        LoadAndAssignTexture(CardGlowMat, level, "_GradientTex");
                    if (BackMat != null)
                        LoadAndAssignTexture(BackMat, level, "_GradientTex");
                    if (InfMat != null)
                        LoadAndAssignTexture(InfMat, level, "_GradientTex");
                    if (AuraRingMat != null)
                        LoadAndAssignTexture(AuraRingMat, level, "_GradientTex");
                    if (AuraBackMat != null)
                        LoadAndAssignTexture(AuraBackMat, level, "_GradientTex");
                    if (InCircMat != null)
                        LoadAndAssignTexture(InCircMat, level, "_GradientTex");
                    if (ExpMat != null)
                        LoadAndAssignTexture(ExpMat, level, "_GradientTex");
                    if (PlasmaOutMat != null)
                        LoadAndAssignTexture(PlasmaOutMat, level, "_GradientTex");
                    if (WindMat != null)
                        LoadAndAssignTexture(WindMat, level, "_GradientTex");
                    if (RaysFlickMat != null)
                        LoadAndAssignTexture(RaysFlickMat, level, "_GradientTex");

                    CardSeq.AppendCallback(() => TurnOn(Card, InwardFlow, AuraRing, BackCard, AuraBack, Rays, RaysFlick, Wind, BlinkPrefabInstance, ParticleRainbowInstance, ParticleRainbowInstanceHor));

                    if (Card != null)
                    {
                        CardSeq.Append(Card.transform.DOLocalMove(StartPoint.transform.localPosition, 0.01f)); //Đến start point
                        CardSeq.Join(Card.transform.DOLocalRotate(new Vector3(0, -90, 0), 0.01f, RotateMode.Fast)); //Xoay vào vị trí
                    }
                    // CardSeq.Join(CardFrameMat.DOFloat(1, "_Intensity", 0.01f));
                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(0, "_Intensity", 0.01f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -0.1f), "_Speed_1", 0.01f));
                    }
                    if (AuraRingMat != null)
                    {
                        CardSeq.Join(AuraRingMat.DOFloat(0, "_Intensity", 0.01f));
                        CardSeq.Join(AuraRingMat.DOVector(new Vector2(-0.1f, -2f), "_Speed_1", 0.01f));
                    }
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0, "_Intensity", 0.01f));
                    if (AuraBackMat != null)
                        CardSeq.Join(AuraBackMat.DOFloat(0, "_Intensity", 0.01f));
                    if (raysImg != null)
                        CardSeq.Join(raysImg.DOFade(0, 0.01f));
                    if (raysFlickImg != null)
                        CardSeq.Join(raysFlickImg.DOFade(0, 0.01f));
                    if (WindMat != null)
                        CardSeq.Join(WindMat.DOFloat(0, "_Intensity", 0.01f));

                    if (Card != null)
                        CardSeq.Append(Card.transform.DOLocalMove(new Vector3(0, EndPoint.transform.localPosition.y - 10, 0), 0.3f)); // Di chuyển đến endpoint
                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(1.5f, "_Intensity", 0.3f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -0.2f), "_Speed_1", 0.01f));
                    }
                    if (Card != null)
                        CardSeq.Append(Card.transform.DOLocalMove(new Vector3(0, EndPoint.transform.localPosition.y - 5, 0), 0.4f));
                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(4, "_Intensity", 0.4f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -0.4f), "_Speed_1", 0.01f));
                    }
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0.3f, "_Intensity", 0.4f));

                    CardSeq.AppendCallback(() => StopBurst(FrameParticleChild, RainbowParticleChild, RainbowHorParticleChild));
                    CardSeq.AppendCallback(() => Slow(2));

                    if (Card != null)
                        CardSeq.Append(Card.transform.DOLocalMove(EndPoint.transform.localPosition, 0.8f));

                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(8, "_Intensity", 0.8f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -1f), "_Speed_1", 0.01f));
                    }
                    if (AuraRingMat != null)
                    {
                        CardSeq.Join(AuraRingMat.DOFloat(8f, "_Intensity", 0.4f).SetDelay(0.4f));
                        CardSeq.Join(AuraRingMat.DOVector(new Vector2(-0.5f, -5f), "_Speed_1", 0.01f));
                    }
                    if (raysFlickImg != null)
                        CardSeq.Join(raysFlickImg.DOFade(1, 0.2f).SetDelay(0.2f));
                    if (raysImg != null)
                        CardSeq.Join(raysImg.DOFade(1, 0.2f).SetDelay(0.7f));
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(1, "_Intensity", 0.8f));
                    if (PlasmaMat != null)
                        CardSeq.Join(PlasmaMat.DOFloat(0, "_Opacity", 0.01f));

                    CardSeq.AppendCallback(() => TurnOn(InwardCircle, Explosion, PlasmaOutward));
                    if (InCircMat != null)
                    {
                        CardSeq.Join(InCircMat.DOFloat(0, "_Opacity", 0.01f));
                        CardSeq.Join(InCircMat.DOFloat(0, "_Intensity", 0.01f));
                    }
                    if (PlasmaMat != null)
                        CardSeq.Join(PlasmaMat.DOFloat(0, "_Opacity", 0.01f));
                    if (PlasmaOutward != null)
                        CardSeq.Join(PlasmaOutward.transform.DOScale(0, 0.01f));
                    if (ExpMat != null)
                        CardSeq.Join(ExpMat.DOFloat(0, "_Opacity", 0.01f));
                    if (PlasmaOutMat != null)
                        CardSeq.Join(PlasmaOutMat.DOFloat(0, "_Intensity", 0.01f));
                    if (Explosion != null)
                        CardSeq.Join(Explosion.transform.DOScale(0, 0.001f));

                    CardSeq.AppendCallback(() => StartCoroutine(camShakeScript.Shake(2.5f, 2.5f)));
                    CardSeq.AppendCallback(() => TurnOn(CardGlow));
                    CardSeq.AppendCallback(() => Slow(0.1f));

                    if (Card != null)
                        CardSeq.Append(Card.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.3f, RotateMode.Fast).SetEase(Ease.InOutBack).SetRelative(true)); // Xoay đến endpoint
                    if (PlasmaOutward != null)
                        CardSeq.Join(PlasmaOutward.transform.DOScale(1, 0.02f));
                    if (PlasmaOutMat != null)
                        CardSeq.Join(PlasmaOutMat.DOFloat(8, "_Intensity", 0.01f));
                    if (InCircMat != null)
                    {
                        CardSeq.Join(InCircMat.DOFloat(2.5f, "_Intensity", 0.01f));
                        CardSeq.Join(InCircMat.DOFloat(1, "_Opacity", 0.01f));
                    }
                    if (CardGlowMat != null)
                        CardSeq.Join(CardGlowMat.DOFloat(0.9f, "_Intensity", 0.01f));//CardGlowMat
                    if (Explosion != null)
                        CardSeq.Join(Explosion.transform.DOScale(1, 0.02f).SetDelay(0.2f));
                    if (PlasmaMat != null)
                        CardSeq.Join(PlasmaMat.DOFloat(8f, "_Intensity", 0.01f));
                    if (AuraBackMat != null)
                        CardSeq.Join(AuraBackMat.DOFloat(1.5f, "_Intensity", 0.01f));
                    if (ExpMat != null)
                        CardSeq.Join(ExpMat.DOFloat(0.75f, "_Opacity", 0.01f));
                    if (WindMat != null)
                        CardSeq.Join(WindMat.DOFloat(6, "_Intensity", 0.01f));
                    if (raysImg != null)
                        CardSeq.Join(raysImg.DOFade(0, 0.5f));
                    if (raysFlickImg != null)
                        CardSeq.Join(raysFlickImg.DOFade(0, 0.01f));
                    if (InfMat != null)
                        CardSeq.Join(InfMat.DOFloat(0, "_Intensity", 0.1f));
                    if (ExpMat != null)
                    {
                        CardSeq.Join(ExpMat.DOFloat(0.3f, "_Opacity", 0.3f).SetDelay(0.4f));
                        CardSeq.Append(ExpMat.DOFloat(0, "_Opacity", 2f));
                    }
                    if (WindMat != null)
                        CardSeq.Join(WindMat.DOFloat(0, "_Intensity", 2f));
                    if (PlasmaMat != null)
                        CardSeq.Join(PlasmaMat.DOFloat(0, "_Intensity", 2f));
                    if (PlasmaOutMat != null)
                        CardSeq.Join(PlasmaOutMat.DOFloat(0, "_Intensity", 2f));
                    if (AuraRingMat != null)
                        CardSeq.Join(AuraRingMat.DOFloat(0, "_Intensity", 2f));

                    CardSeq.AppendCallback(() => TurnOff(Explosion, InwardFlow, AuraRing, Rays, RaysFlick, Wind, PlasmaOutward, BackCard));

                    // CardSeq.Append(CardFrameMat.DOFloat(0, "_Intensity", 4f));
                    if (CardGlowMat != null)
                        CardSeq.Append(CardGlowMat.DOFloat(0.7f, "_Intensity", 5f));
                    if (InCircMat != null)
                    {
                        CardSeq.Join(InCircMat.DOFloat(0, "_Opacity", 4f));
                        CardSeq.Join(InCircMat.DOFloat(0, "_Intensity", 4f));
                    }

                    CardSeq.AppendCallback(() => TurnOff(InwardCircle));

                    break;
                }
            default:
                break;
        }

        CardSeq.OnComplete(() =>
        {
            // Reset
            isCardSeqPlaying = false;
            ButtonScript.UpdateButtonStatus(InfoScript.LevelVar);

        });

        CardSeq.Play();
    }

    private void StopBurst(params ParticleSystem[] systems)
    {
        foreach (ParticleSystem sys in systems)
        {
            if (sys != null)
            {
                ParticleSystem.MainModule mainModule = sys.main;
                mainModule.loop = false;

                // Set the burst to all zeros
                ParticleSystem.Burst[] bursts = { new ParticleSystem.Burst(0, 0, 0, 0, 0) };
                sys.emission.SetBursts(bursts);
                mainModule.simulationSpeed = 1;

                // Set the loop property to true to keep particles over time
                mainModule.loop = true;
                sys.Play();
            }
        }
    }

    private void TurnOn(params GameObject[] gameObjects)
    {
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
            }
        }
    }

    private void TurnOff(params GameObject[] gameObjects)
    {
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null && obj.activeSelf)
            {
                obj.SetActive(false);
            }
        }
    }

    private IEnumerator DelayedSlow(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (slow_mo != null)
        {
            slow_mo.DoSlowmotion();
        }
    }

    private void Slow(float delay)
    {
        StartCoroutine(DelayedSlow(delay));
    }

    private GameObject InstantiatePrefab(GameObject particlePrefab, ref ParticleSystem particleChild)
    {
        GameObject particlePrefabInstance = null;  // Declare outside the if statement

        particlePrefabInstance = Instantiate(particlePrefab, EndPoint.transform);

        Transform childTransform = particlePrefabInstance.transform.GetChild(0).GetChild(0);

        if (childTransform != null)
        {
            particleChild = childTransform.GetComponent<ParticleSystem>();

            if (particleChild != null)
            {
                return particlePrefabInstance; // Return the instantiated prefab
            }
        }

        return particlePrefabInstance;
    }

    private void TurnOffIfInstantiated(params GameObject[] prefabs)
    {
        foreach (var prefab in prefabs)
        {
            if (prefab != null && prefab.activeSelf)
            {
                prefab.SetActive(false);
            }
        }
    }

    void LoadAndAssignTexture(Material mat, int a, string param)
    {
        Texture2D targetTexture = BundleHandler.main.GetTexture2D("cardfilter", "Gradient" + a);
        if (targetTexture == null)
        {
            Texture2D dataNext = Resources.Load<Texture2D>("Pack/Images/FilterCard/Gradient" + a);
            if (dataNext != null)
                targetTexture = dataNext;
        }
        //Texture loadedTexture = Resources.Load<Texture>($"FilterCard/Gradient{a}");

        if (targetTexture != null)
        {
            // Explicitly cast to Texture2D
            if (mat != null)
            {
                // Assign the loaded texture to the main texture of the Material
                mat.SetTexture($"{param}", targetTexture);
            }
        }
    }

}