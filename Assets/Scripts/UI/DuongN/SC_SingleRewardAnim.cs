using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using GIKCore.Bundle;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class SC_SingleRewardAnim : MonoBehaviour
{
    public int Rarity = 1; //1: common, 2: rare, 3: epic, 4: legendary, 5: quà khác (vàng, cúp essense, v.v...).
    [SerializeField] private GameObject CardReward;
    [SerializeField] private int Frame;
    [SerializeField] private int TargetAmount; //Số lá bài giữa viền hiện tại và viền tiếp theo
    [SerializeField] private int Amount;
    [SerializeField] private int CurrentAmount;
    [SerializeField] private int CurrentBalance;
    private Image CardImage;
    [SerializeField] private GameObject BackCard;
    private Image BackCardImage;
    [SerializeField] private GameObject Reward;
    [SerializeField] private GameObject RewardBack;
    private Image RewardImage;
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
    public delegate void FrameEventHandler(int frame);
    public static event FrameEventHandler OnFrameStart;
    public delegate void RarityEventHandler(int rarity, int target, int amt, int currentAmt, int currentBal);
    public static event RarityEventHandler OnCardAnimationStart;
    public bool fallBack = false;



    // Start is called before the first frame update
    void OnEnable()
    {
        if (ObjectToShake != null)
        {
            camShakeScript = ObjectToShake.GetComponent<CamShakeScript_UI>();
        }

        //TurnOff(
        //    InwardFlow,
        //    AuraRing,
        //    PlasmaOutward,
        //    InwardCircle,
        //    CardReward,
        //    CardGlow,
        //    Rays,
        //    RaysFlick,
        //    Wind,
        //    Explosion,
        //    AuraBack,
        //    Reward);

        //Debug.Log("Turn Off first");

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
        // CardFrameMat = CardReward.GetComponent<Image>().material;
        CardGlowMat = CardGlow.GetComponent<Image>().material;
        PlasmaOutMat = PlasmaOutward.GetComponent<UIMeshRenderer>().UIMaterial;
        InCircMat = InwardCircle.GetComponent<UIMeshRenderer>().UIMaterial;
        CardImage = CardReward.GetComponent<Image>();
        BackCardImage = BackCard.GetComponent<Image>();
        RewardImage = Reward.GetComponent<Image>();
        RewardBackImage = RewardBack.GetComponent<Image>();
        RewardFrameMat = RewardImage.material;

        ParticleRainbowInstance = InstantiatePrefab(RainbowParticleHor, ref RainbowHorParticleChild);
        ParticleRainbowInstanceHor = InstantiatePrefab(RainbowParticle, ref RainbowParticleChild);
        BlinkPrefabInstance = InstantiatePrefab(FrameParticle, ref FrameParticleChild);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        //{
        //    if (isCardSeqPlaying)
        //    {
        //        // If the sequence is playing, complete it immediately
        //        CardSeq.Complete(withCallbacks: true);
        //        isCardSeqPlaying = false;
        //        return;
        //    }
        //    else
        //    {
        //        PlayCardAnim(Rarity);
        //        return;
        //    }
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (fallBack)
        //    {
        //        PlayCardAnim(6);
        //    }
        //    else
        //    {
        //        PlayCardAnim(Rarity);
        //    }
        //}
    }
    public SC_SingleRewardAnim SetCardReward(int _Frame, int _TargetAmount, int _Amount, int _CurrentAmount)
    {
        Frame = _Frame;
        TargetAmount = _TargetAmount;
        Amount = _Amount;
        CurrentAmount = _CurrentAmount;
        return this;
    }
    public SC_SingleRewardAnim SetGoldReward(int _Amount, int _CurrentBalance)
    {
        Amount = _Amount;
        CurrentBalance = _CurrentBalance;
        return this;
    }
    public SC_SingleRewardAnim SetDeckReward(int _TargetAmount, int _Amount, int _CurrentAmount)
    {
        Frame = 1;
        TargetAmount = _TargetAmount;
        Amount = _Amount;
        CurrentAmount = _CurrentAmount;
        return this;
    }
    public SC_SingleRewardAnim PlayCardAnim(int rarity)
    {
        if (fallBack)
        {
            OnCardAnimationStart?.Invoke(6, TargetAmount, Amount, CurrentAmount, CurrentBalance);
            OnFrameStart(Frame);
        }
        else
        {
            OnCardAnimationStart?.Invoke(rarity, TargetAmount, Amount, CurrentAmount, CurrentBalance);
            OnFrameStart(Frame);
        }

        TurnOff(
            InwardFlow,
            AuraRing,
            PlasmaOutward,
            InwardCircle,
            CardReward,
            CardGlow,
            Rays,
            RaysFlick,
            Wind,
            Explosion,
            AuraBack,
            Reward);


        if (rarity > 6 || rarity < 1)
        {
            rarity = 5;
        }

        if (isCardSeqPlaying)
        {
            return this;
        }

        isCardSeqPlaying = true;

        TurnOff(CardGlow);

        TurnOffIfInstantiated(
            ParticleRainbowInstance,
            ParticleRainbowInstanceHor,
            BlinkPrefabInstance);

        Texture2D loadedTexture = null;
        if (RewardImage != null)
            loadedTexture = (Texture2D)RewardImage.sprite.texture; //Texture cho reward không phải cards.

        CardSeq = DOTween.Sequence();

        switch (rarity)
        {
            case 1:
                {
                    TurnOffIfInstantiated(
                    ParticleRainbowInstance,
                    ParticleRainbowInstanceHor,
                    BlinkPrefabInstance);

                    //CardSeq.AppendCallback(() => TurnOn(CardReward, BackCard));
                    CardSeq.AppendCallback(() => TurnOn(CardReward, BackCard));

                    // LoadAndAssignTexture(CardFrameMat, rarity , "_GradientTex");
                    if (BackMat != null)
                        LoadAndAssignTexture(BackMat, rarity, "_GradientTex");


                    CardSeq.Append(CardReward.transform.DOLocalMove(StartPoint.transform.localPosition, 0.01f)); //Đến start point
                    CardSeq.Join(CardReward.transform.DOLocalRotate(new Vector3(0, -90, 0), 0.01f, RotateMode.Fast)); //Xoay vào vị trí

                    // CardSeq.Join(CardFrameMat.DOFloat(1 , "_Intensity" , 0.01f));
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0, "_Intensity", 0.01f));

                    CardSeq.Append(CardReward.transform.DOLocalMove(new Vector3(EndPoint.transform.localPosition.x, EndPoint.transform.localPosition.y, 0), 0.3f));
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0.2f, "_Intensity", 0.3f));

                    CardSeq.Append(CardReward.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.3f, RotateMode.Fast).SetEase(Ease.InOutBack).SetRelative(true)); // Xoay đến endpoint
                    CardSeq.AppendCallback(() => TurnOff(BackCard));
                    // CardSeq.Append(CardFrameMat.DOFloat(0 , "_Intensity" , 0.5f));
                    break;
                }
            case 2:
                {
                    TurnOffIfInstantiated(
                    ParticleRainbowInstance,
                    ParticleRainbowInstanceHor,
                    BlinkPrefabInstance);

                    // LoadAndAssignTexture(CardFrameMat, rarity , "_GradientTex");
                    if (CardGlowMat != null)
                        LoadAndAssignTexture(CardGlowMat, rarity, "_GradientTex");
                    if (BackMat != null)
                        LoadAndAssignTexture(BackMat, rarity, "_GradientTex");

                    CardSeq.AppendCallback(() => TurnOn(CardReward, CardGlow, BackCard));

                    if (CardGlowMat != null)
                        CardSeq.Join(CardGlowMat.DOFloat(0f, "_Intensity", 0.01f));//CardGlowMat

                    if (CardReward != null)
                    {
                        CardSeq.Join(CardReward.transform.DOLocalMove(StartPoint.transform.localPosition, 0.01f)); //Đến start point
                        CardSeq.Join(CardReward.transform.DOLocalRotate(new Vector3(0, -90, 0), 0.01f, RotateMode.Fast)); //Xoay vào vị trí
                    }
                    // CardSeq.Join(CardFrameMat.DOFloat(1 , "_Intensity" , 0.01f));
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0, "_Intensity", 0.01f));

                    CardSeq.AppendCallback(() => StartCoroutine(camShakeScript.Shake(1.5f, 1.5f)));

                    if (CardReward != null)
                        CardSeq.Append(CardReward.transform.DOLocalMove(new Vector3(EndPoint.transform.localPosition.x, EndPoint.transform.localPosition.y, 0), 0.3f));

                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0.7f, "_Intensity", 0.3f));

                    if (CardReward != null)
                        CardSeq.Append(CardReward.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.3f, RotateMode.Fast).SetEase(Ease.InOutBack).SetRelative(true)); // Xoay đến endpoint

                    if (CardGlowMat != null)
                        CardSeq.Join(CardGlowMat.DOFloat(1.5f, "_Intensity", 0.01f).SetDelay(0.2f));//CardGlowMat

                    CardSeq.AppendCallback(() => TurnOff(BackCard));

                    // CardSeq.Append(CardFrameMat.DOFloat(0 , "_Intensity" , 2f));
                    if (CardGlowMat != null)
                        CardSeq.Append(CardGlowMat.DOFloat(0, "_Intensity", 2f));
                    break;
                }
            case 3:
                {
                    TurnOffIfInstantiated(
                    ParticleRainbowInstance,
                    ParticleRainbowInstanceHor,
                    BlinkPrefabInstance);

                    // LoadAndAssignTexture(CardFrameMat, rarity , "_GradientTex");
                    if (CardGlowMat != null)
                        LoadAndAssignTexture(CardGlowMat, rarity, "_GradientTex");
                    if (BackMat != null)
                        LoadAndAssignTexture(BackMat, rarity, "_GradientTex");
                    if (InfMat != null)
                        LoadAndAssignTexture(InfMat, rarity, "_GradientTex");
                    if (AuraRingMat != null)
                        LoadAndAssignTexture(AuraRingMat, rarity, "_GradientTex");
                    if (AuraBackMat != null)
                        LoadAndAssignTexture(AuraBackMat, rarity, "_GradientTex");
                    if (InCircMat != null)
                        LoadAndAssignTexture(InCircMat, rarity, "_GradientTex");
                    if (ExpMat != null)
                        LoadAndAssignTexture(ExpMat, rarity, "_GradientTex");
                    if (PlasmaOutMat != null)
                        LoadAndAssignTexture(PlasmaOutMat, rarity, "_GradientTex");
                    if (RaysFlickMat != null)
                        LoadAndAssignTexture(RaysFlickMat, rarity, "_GradientTex");

                    CardSeq.AppendCallback(() => TurnOn(CardReward, InwardFlow, AuraRing, BackCard, AuraBack, RaysFlick));

                    if (CardReward != null)
                    {
                        CardSeq.Append(CardReward.transform.DOLocalMove(StartPoint.transform.localPosition, 0.01f)); //Đến start point
                        CardSeq.Join(CardReward.transform.DOLocalRotate(new Vector3(0, -90, 0), 0.01f, RotateMode.Fast)); //Xoay vào vị trí
                    }
                    // CardSeq.Join(CardFrameMat.DOFloat(1 , "_Intensity" , 0.01f));
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

                    if (AuraBack != null)
                        CardSeq.Join(AuraBack.transform.DOScale(Vector3.one, 0.01f));
                    if (raysFlickImg != null)
                        CardSeq.Join(raysFlickImg.DOFade(0, 0.01f));

                    if (CardReward != null)
                        CardSeq.Append(CardReward.transform.DOLocalMove(new Vector3(EndPoint.transform.localPosition.x, EndPoint.transform.localPosition.y - 10, 0), 0.3f)); // Di chuyển đến endpoint

                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(1.5f, "_Intensity", 0.3f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -0.2f), "_Speed_1", 0.01f));
                    }

                    if (CardReward != null)
                        CardSeq.Append(CardReward.transform.DOLocalMove(new Vector3(EndPoint.transform.localPosition.x, EndPoint.transform.localPosition.y - 5, 0), 0.2f)); //

                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(2, "_Intensity", 0.2f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -0.3f), "_Speed_1", 0.01f));
                    }
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0.3f, "_Intensity", 0.2f));

                    if (CardReward != null)
                        CardSeq.Append(CardReward.transform.DOLocalMove(EndPoint.transform.localPosition, 0.6f));

                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(5, "_Intensity", 0.6f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -0.6f), "_Speed_1", 0.01f));
                    }
                    if (AuraRingMat != null)
                    {
                        CardSeq.Join(AuraRingMat.DOFloat(5f, "_Intensity", 0.2f).SetDelay(0.4f));
                        CardSeq.Join(AuraRingMat.DOVector(new Vector2(-0.5f, -5f), "_Speed_1", 0.01f));
                    }
                    if (raysFlickImg != null)
                        CardSeq.Join(raysFlickImg.DOFade(1, 0.2f).SetDelay(0.2f));
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0.6f, "_Intensity", 0.6f));

                    //CardSeq.AppendCallback(() => TurnOn(InwardCircle, Explosion, PlasmaOutward));
                    CardSeq.AppendCallback(() => TurnOn(InwardCircle, PlasmaOutward));

                    if (InCircMat != null)
                    {
                        CardSeq.Join(InCircMat.DOFloat(0, "_Opacity", 0.01f));
                        CardSeq.Join(InCircMat.DOFloat(0, "_Intensity", 0.01f));
                    }
                    if (PlasmaMat != null)
                        CardSeq.Join(PlasmaMat.DOFloat(0, "_Opacity", 0.01f));
                    if (PlasmaOutMat != null)
                        CardSeq.Join(PlasmaOutMat.DOFloat(0, "_Intensity", 0.01f));
                    if (PlasmaOutward != null)
                        CardSeq.Join(PlasmaOutward.transform.DOScale(0, 0.01f));
                    if (ExpMat != null)
                        CardSeq.Join(ExpMat.DOFloat(0, "_Opacity", 0.01f));
                    //if (Explosion != null)
                    //    CardSeq.Join(Explosion.transform.DOScale(0, 0.01f));

                    CardSeq.AppendCallback(() => StartCoroutine(camShakeScript.Shake(2f, 2f)));
                    CardSeq.AppendCallback(() => TurnOn(CardGlow));

                    if (CardReward != null)
                        CardSeq.Append(CardReward.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.3f, RotateMode.Fast).SetEase(Ease.InOutBack).SetRelative(true)); // Xoay đến endpoint
                    if (PlasmaOutward != null)
                        CardSeq.Join(PlasmaOutward.transform.DOScale(1, 0.02f));
                    if (PlasmaOutMat != null)
                        CardSeq.Join(PlasmaOutMat.DOFloat(6, "_Intensity", 0.01f));
                    if (InCircMat != null)
                    {
                        CardSeq.Join(InCircMat.DOFloat(2.5f, "_Intensity", 0.01f));
                        CardSeq.Join(InCircMat.DOFloat(1, "_Opacity", 0.01f));
                    }

                    if (CardGlowMat != null)
                        CardSeq.Join(CardGlowMat.DOFloat(0.9f, "_Intensity", 0.01f));//CardGlowMat

                    //if (Explosion != null)
                    //    CardSeq.Join(Explosion.transform.DOScale(1, 0.02f).SetDelay(0.2f));
                    if (PlasmaMat != null)
                        CardSeq.Join(PlasmaMat.DOFloat(8f, "_Intensity", 0.01f));
                    if (AuraBackMat != null)
                        CardSeq.Join(AuraBackMat.DOFloat(1.3f, "_Intensity", 0.01f));

                    if (ExpMat != null)
                        CardSeq.Join(ExpMat.DOFloat(0.75f, "_Opacity", 0.01f));
                    if (raysFlickImg != null)
                        CardSeq.Join(raysFlickImg.DOFade(0, 0.01f));
                    if (InfMat != null)
                        CardSeq.Join(InfMat.DOFloat(0, "_Intensity", 0.1f));
                    if (ExpMat != null)
                    {
                        CardSeq.Join(ExpMat.DOFloat(0.3f, "_Opacity", 0.3f).SetDelay(0.4f));
                        CardSeq.Append(ExpMat.DOFloat(0, "_Opacity", 2f));
                    }
                    if (AuraRingMat != null)
                        CardSeq.Join(AuraRingMat.DOFloat(0, "_Intensity", 2f));
                    if (PlasmaMat != null)
                        CardSeq.Join(PlasmaMat.DOFloat(0, "_Intensity", 2f));

                    //CardSeq.AppendCallback(() => TurnOff(Explosion, InwardFlow, AuraRing, Rays, RaysFlick, Wind, PlasmaOutward, BackCard));
                    CardSeq.AppendCallback(() => TurnOff(InwardFlow, AuraRing, Rays, RaysFlick, Wind, PlasmaOutward, BackCard));
                    // CardSeq.Append(CardFrameMat.DOFloat(0 , "_Intensity" , 3f));
                    if (CardGlowMat != null)
                        CardSeq.Append(CardGlowMat.DOFloat(0.6f, "_Intensity", 4f));

                    if (PlasmaMat != null)
                    {
                        CardSeq.Join(InCircMat.DOFloat(0, "_Opacity", 3f));
                        CardSeq.Join(InCircMat.DOFloat(0, "_Intensity", 3f));
                    }
                        

                    CardSeq.AppendCallback(() => TurnOff(InwardCircle));
                    break;
                }
            case 4:
                {
                    TurnOffIfInstantiated(
                    ParticleRainbowInstance,
                    ParticleRainbowInstanceHor,
                    BlinkPrefabInstance);

                    // LoadAndAssignTexture(CardFrameMat, rarity , "_GradientTex");
                    if (CardGlowMat != null)
                        LoadAndAssignTexture(CardGlowMat, rarity, "_GradientTex");
                    if (BackMat != null)
                        LoadAndAssignTexture(BackMat, rarity, "_GradientTex");
                    if (InfMat != null)
                        LoadAndAssignTexture(InfMat, rarity, "_GradientTex");
                    if (AuraRingMat != null)
                        LoadAndAssignTexture(AuraRingMat, rarity, "_GradientTex");
                    if (AuraBackMat != null)
                        LoadAndAssignTexture(AuraBackMat, rarity, "_GradientTex");
                    if (InCircMat != null)
                        LoadAndAssignTexture(InCircMat, rarity, "_GradientTex");
                    if (ExpMat != null)
                        LoadAndAssignTexture(ExpMat, rarity, "_GradientTex");
                    if (PlasmaOutMat != null)
                        LoadAndAssignTexture(PlasmaOutMat, rarity, "_GradientTex");
                    if (WindMat != null)
                        LoadAndAssignTexture(WindMat, rarity, "_GradientTex");
                    if (RaysFlickMat != null)
                        LoadAndAssignTexture(RaysFlickMat, rarity, "_GradientTex");

                    CardSeq.AppendCallback(() => TurnOn(CardReward, InwardFlow, AuraRing, BackCard, AuraBack, Rays, RaysFlick, Wind, BlinkPrefabInstance, ParticleRainbowInstance, ParticleRainbowInstanceHor));

                    if (CardReward != null)
                    {
                        CardSeq.Append(CardReward.transform.DOLocalMove(StartPoint.transform.localPosition, 0.01f)); //Đến start point
                        CardSeq.Join(CardReward.transform.DOLocalRotate(new Vector3(0, -90, 0), 0.01f, RotateMode.Fast)); //Xoay vào vị trí
                    }
                    // CardSeq.Join(CardFrameMat.DOFloat(1 , "_Intensity" , 0.01f));
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
                    if (AuraBack != null)
                        CardSeq.Join(AuraBack.transform.DOScale(Vector3.one, 0.01f));
                    if (raysImg != null)
                        CardSeq.Join(raysImg.DOFade(0, 0.01f));
                    if (raysFlickImg != null)
                        CardSeq.Join(raysFlickImg.DOFade(0, 0.01f));
                    if (WindMat != null)
                        CardSeq.Join(WindMat.DOFloat(0, "_Intensity", 0.01f));

                    if (CardReward != null)
                        CardSeq.Append(CardReward.transform.DOLocalMove(new Vector3(EndPoint.transform.localPosition.x, EndPoint.transform.localPosition.y - 10, 0), 0.3f)); // Di chuyển đến endpoint
                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(1.5f, "_Intensity", 0.3f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -0.2f), "_Speed_1", 0.01f));
                    }
                    if (CardReward != null)
                        CardSeq.Append(CardReward.transform.DOLocalMove(new Vector3(EndPoint.transform.localPosition.x, EndPoint.transform.localPosition.y - 5, 0), 0.4f));
                    if (InfMat != null)
                    {
                        CardSeq.Join(InfMat.DOFloat(4, "_Intensity", 0.4f));
                        CardSeq.Join(InfMat.DOVector(new Vector2(-0.1f, -0.4f), "_Speed_1", 0.01f));
                    }
                        
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0.3f, "_Intensity", 0.4f));

                    CardSeq.AppendCallback(() => StopBurst(FrameParticleChild, RainbowParticleChild, RainbowHorParticleChild));
                    CardSeq.AppendCallback(() => Slow(2));

                    if (CardReward != null)
                        CardSeq.Append(CardReward.transform.DOLocalMove(EndPoint.transform.localPosition, 0.8f));
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

                    //CardSeq.AppendCallback(() => TurnOn(InwardCircle, Explosion, PlasmaOutward));
                    CardSeq.AppendCallback(() => TurnOn(InwardCircle, PlasmaOutward));

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
                    //if (Explosion != null)
                    //    CardSeq.Join(Explosion.transform.DOScale(0, 0.001f));

                    CardSeq.AppendCallback(() => StartCoroutine(camShakeScript.Shake(2.5f, 2.5f)));
                    CardSeq.AppendCallback(() => TurnOn(CardGlow));
                    CardSeq.AppendCallback(() => Slow(0.1f));

                    if (CardReward != null)
                        CardSeq.Append(CardReward.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.3f, RotateMode.Fast).SetEase(Ease.InOutBack).SetRelative(true)); // Xoay đến endpoint

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
                    //if (Explosion != null)
                    //    CardSeq.Join(Explosion.transform.DOScale(1, 0.02f).SetDelay(0.2f));
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

                    //CardSeq.AppendCallback(() => TurnOff(Explosion, InwardFlow, AuraRing, Rays, RaysFlick, Wind, PlasmaOutward, BackCard));
                    CardSeq.AppendCallback(() => TurnOff(InwardFlow, AuraRing, Rays, RaysFlick, Wind, PlasmaOutward, BackCard));

                    // CardSeq.Append(CardFrameMat.DOFloat(0 , "_Intensity" , 4f));
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
            case 5:
                {
                    TurnOffIfInstantiated(
                    ParticleRainbowInstance,
                    ParticleRainbowInstanceHor,
                    BlinkPrefabInstance);

                    CardSeq.AppendCallback(() => TurnOn(Reward, RewardBack, AuraBack));

                    if (RewardFrameMat != null)
                        LoadAndAssignTexture(RewardFrameMat, rarity, "_GradientTex");
                    if (BackMat != null)
                        LoadAndAssignTexture(BackMat, rarity, "_GradientTex");
                    if (AuraBackMat != null)
                        LoadAndAssignTexture(AuraBackMat, rarity, "_GradientTex");

                    // Texture2D loadedTexture = (Texture2D)RewardImage.sprite.texture;

                    if (loadedTexture != null && RewardFrameMat != null)
                    {
                        RewardFrameMat.SetTexture("_MainTex", loadedTexture);
                        RewardFrameMat.SetTexture("_MaskTex", loadedTexture);
                    }

                    if (Reward != null)
                    {
                        CardSeq.Append(Reward.transform.DOLocalMove(StartPoint.transform.localPosition, 0.01f)); //Đến start point
                        //CardSeq.Join(Reward.transform.DOLocalRotate(new Vector3(0, -180, 0), 0.01f, RotateMode.Fast)); //Xoay vào vị trí
                    }
                        
                    if (RewardFrameMat != null)
                    {
                        CardSeq.Join(RewardFrameMat.DOFloat(0.25f, "_Intensity", 0.01f));
                    }
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0, "_Intensity", 0.01f));
                    if (AuraBackMat != null)
                        CardSeq.Join(AuraBackMat.DOFloat(0, "_Intensity", 0.01f));

                    if (Reward != null)
                        CardSeq.Append(Reward.transform.DOLocalMove(new Vector3(EndPoint.transform.localPosition.x, EndPoint.transform.localPosition.y, 0), 0.3f));
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0.25f, "_Intensity", 0.3f));

                    //if (Reward != null)
                        //CardSeq.Append(Reward.transform.DOLocalRotate(new Vector3(0, 180, 0), 0.3f, RotateMode.Fast).SetEase(Ease.InOutBack).SetRelative(true)); // Xoay đến endpoint
                    if (AuraBackMat != null)
                        CardSeq.Join(AuraBackMat.DOFloat(1.2f, "_Intensity", 0.1f));
                    if (AuraBack != null)
                        CardSeq.Join(AuraBack.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.1f));
                    if (Reward != null)
                        CardSeq.Append(Reward.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0), 1f, 4, 0.2f));

                    CardSeq.AppendCallback(() => TurnOff(RewardBack));
                    if (RewardFrameMat != null)
                        CardSeq.Append(RewardFrameMat.DOFloat(0, "_Intensity", 3f));
                    if (AuraBackMat != null)
                        CardSeq.Join(AuraBackMat.DOFloat(0.4f, "_Intensity", 3f));

                    break;
                }
            case 6:
                {
                    TurnOffIfInstantiated(
                    ParticleRainbowInstance,
                    ParticleRainbowInstanceHor,
                    BlinkPrefabInstance);

                    CardSeq.AppendCallback(() => TurnOn(Reward, RewardBack, AuraBack));
                    if (RewardFrameMat != null)
                        LoadAndAssignTexture(RewardFrameMat, rarity, "_GradientTex");
                    if (BackMat != null)
                        LoadAndAssignTexture(BackMat, rarity, "_GradientTex");
                    if (AuraBackMat != null)
                        LoadAndAssignTexture(AuraBackMat, rarity, "_GradientTex");

                    if (loadedTexture != null)
                    {
                        RewardFrameMat.SetTexture("_MainTex", loadedTexture);
                        RewardFrameMat.SetTexture("_MaskTex", loadedTexture);
                    }

                    if (Reward != null)
                    {
                        CardSeq.Append(Reward.transform.DOLocalMove(StartPoint.transform.localPosition, 0.01f)); //Đến start point
                        //CardSeq.Join(Reward.transform.DOLocalRotate(new Vector3(0, -180, 0), 0.01f, RotateMode.Fast)); //Xoay vào vị trí
                    }
                    if (RewardFrameMat != null)
                        CardSeq.Join(RewardFrameMat.DOFloat(0.25f, "_Intensity", 0.01f));
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0, "_Intensity", 0.01f));
                    if (AuraBackMat != null)
                        CardSeq.Join(AuraBackMat.DOFloat(0, "_Intensity", 0.01f));

                    if (Reward != null)
                        CardSeq.Append(Reward.transform.DOLocalMove(new Vector3(EndPoint.transform.localPosition.x, EndPoint.transform.localPosition.y, 0), 0.3f));
                    if (BackMat != null)
                        CardSeq.Join(BackMat.DOFloat(0.25f, "_Intensity", 0.3f));
                    //if (Reward != null)
                        //CardSeq.Append(Reward.transform.DOLocalRotate(new Vector3(0, 180, 0), 0.3f, RotateMode.Fast).SetEase(Ease.InOutBack).SetRelative(true)); // Xoay đến endpoint
                    if (AuraBackMat != null)
                        CardSeq.Join(AuraBackMat.DOFloat(1.2f, "_Intensity", 0.1f));
                    if (AuraBack != null)
                        CardSeq.Join(AuraBack.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.1f));
                    if (Reward != null)
                        CardSeq.Append(Reward.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0), 1f, 4, 0.2f));

                    CardSeq.AppendCallback(() => TurnOff(RewardBack));
                    if (RewardFrameMat != null)
                        CardSeq.Append(RewardFrameMat.DOFloat(0, "_Intensity", 3f));
                    if (AuraBackMat != null)
                        CardSeq.Join(AuraBackMat.DOFloat(0.4f, "_Intensity", 3f));

                    break;
                }
            default:
                {
                    break;
                }
        }

        CardSeq.OnComplete(() =>
        {
            // Reset
            isCardSeqPlaying = false;
        });

        CardSeq.Play();

        return this;
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
            else
            {
                Debug.LogError("Particle System is not assigned.");
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
        else
        {
            Debug.LogWarning("slow_mo is null!");
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
            else
            {
                Debug.LogError("Child object doesn't have a ParticleSystem component.");
            }
        }
        else
        {
            Debug.LogError("Child object not found.");
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
        Texture tt = null;
        Texture targetTexture = BundleHandler.main.GetTexture("cardfilter", "Gradient" + a);
        if (targetTexture == null)
        {
            Texture dataNext = Resources.Load<Texture>("Pack/Images/FilterCard/Gradient" + a);
            if (dataNext != null)
                tt = dataNext;
        }
        tt = targetTexture;
        //Texture loadedTexture = Resources.Load<Texture>($"FilterCard/Gradient{a}");
        Texture loadedTexture = tt;
        //Texture loadedTexture = Resources.Load<Texture>($"Gradient{a}");

        if (loadedTexture != null)
        {
            // Explicitly cast to Texture2D
            Texture2D texture2D = (Texture2D)loadedTexture;

            if (mat != null)
            {
                // Assign the loaded texture to the main texture of the Material
                mat.SetTexture($"{param}", texture2D);

                // Debug.Log($"Texture 'Gradient{a}.png' loaded and assigned successfully.");
            }
        }
    }

}