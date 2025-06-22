using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIParticleExtensions;

public class SC_CurrencyReward : MonoBehaviour
{
    [SerializeField] private GameObject BeginPoint;
    [SerializeField] private int CurrentAmount;
    [SerializeField] private int amount;
    [SerializeField] private Transform EndPointCurrency;
    [SerializeField] private GameObject EndGlow;
    [SerializeField] private GameObject endPointBg;
    [SerializeField] private TextMeshProUGUI ResourceText;
    [SerializeField] private GameObject EndParticleContainer_1;
    [SerializeField] private GameObject BeginParticleContainer_2;
    [SerializeField] private GameObject coinContainer_3;
    public SC_Button_Upgrade ButtonScript_Bar;
    public SC_Info_Upgrade InfoScript;
    private int scaleDown;
    private GameObject destroyEndPoint;
    private GameObject destroyBeginPoint;
    private GameObject destroyCoin;
    private ParticleSystem coinParticle;
    private int cycle;
    private float waitBtwCycles;
    public GameObject ParticleAttractorToSet;
    private Material targetMaterial;
    private int clampAmount;
    private ParticleSystem endBurstParticle;
    private ParticleSystem beginGlowParticle;


    void Start()
    {
        targetMaterial = endPointBg.GetComponent<Image>().material;

        if (ButtonScript_Bar == null)
        {
            Debug.LogError("ButtonScript_Bar is not assigned.");
        }
        else
        {
            // Subscribe to OnUpgradeButtonClick with null checks
            ButtonScript_Bar.OnUpgradeButtonClick += () => CountResource(InfoScript.bonusAmount);
        } 
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // CountResource(InfoScript.bonusAmount);

        }
    }

    void CountResource(int bonus)
    {
    //Instantiate glow particle
        PrepareParticle( out destroyBeginPoint , BeginParticleContainer_2 , BeginPoint.transform , out beginGlowParticle);
        
    //Instantiate coin particles
        Vector3 beginPosition = new Vector3(BeginPoint.transform.localPosition.x, BeginPoint.transform.localPosition.y, 0);
        BeginPoint.transform.localPosition = beginPosition;

        PrepareParticle( out destroyCoin , coinContainer_3 , BeginPoint.transform , out coinParticle);

    //Start getting particle attractor for coin
        Coffee.UIExtensions.UIParticleAttractor attractor = ParticleAttractorToSet.GetComponent<Coffee.UIExtensions.UIParticleAttractor>();
        if (attractor != null)
        {
            // ParticleSystem particleSystemReference = beginParticleSystemChild /* Get your Particle System reference here */;
            attractor.SetParticleSystemTo(coinParticle);
        }
        else
        {
            Debug.LogError("ParticleSetter script not found on targetObject.");
        }
    //End attractor

    //Instantiate end point particles
        PrepareParticle( out destroyEndPoint , EndParticleContainer_1 , EndPointCurrency.transform , out endBurstParticle);

        BeginPoint.SetActive(true);

        //Set amounts
        amount = (amount == 0) ? 5   : amount;

        //Scale down for big amounts
        scaleDown = (amount > 20) ? amount / 10 : scaleDown; //Use this scale to limit number of currency appears.

        //Clamp amounts to be used in particle burst cycles and wait time between cycles
        clampAmount = (amount > 20) ? 20 + (int)(Mathf.Floor(amount / scaleDown)) : amount ; //Total coins appear

        cycle = clampAmount;

        waitBtwCycles = Mathf.Min(0.1f , 1.3f / cycle);

    //Coin Sequence
        Sequence coinSequence = DOTween.Sequence();
        coinSequence.AppendInterval(4f);
        coinSequence.AppendCallback(() => PlayBurstParticleSystem(coinParticle , 0f, 1f , cycle , waitBtwCycles));

        coinSequence.AppendInterval(1.5f);
        //Play particle system -----> move down
        coinSequence.AppendCallback(() => PlayBurstParticleSystem(endBurstParticle  , 0f, 5 , 10 , 0.05f));

        // Play the sequence
    coinSequence.Play();
    //End coin sequence


    //Animation for currency UI (end point)
        Sequence iconSequence = DOTween.Sequence(); //Sequence for end point
        
        iconSequence.Append(targetMaterial.DOFloat(0f, "_Intensity", 0.01f)); //Reset Intensity of background

        iconSequence.Join(EndGlow.GetComponent<Image>().DOFade(0.0f, 0.01f)); //Glow to 0

        iconSequence.AppendInterval(4.5f);

        iconSequence.Append(EndPointCurrency.DOPunchScale( new Vector3(0.1f, 0.3f ,0), 1f, 0, 0f) //Shake end point
            .SetEase(Ease.InSine)
            .SetDelay(1.5f)
            .SetLoops(1,LoopType.Yoyo));

        iconSequence.Join(endPointBg.transform.DOPunchScale(new Vector3(0.1f, 0.3f, 0), 1f, 0, 0f) //Shake background
            .SetEase(Ease.InSine)
            .SetLoops(1,LoopType.Yoyo));

        iconSequence.Join(EndGlow.GetComponent<Image>().DOFade(1.0f, 0.8f) //Start glow
            .SetLoops(1,LoopType.Yoyo));

        iconSequence.Join(targetMaterial.DOFloat(1.0f, "_Intensity", 0.5f) //Background flash
            .SetLoops(2, LoopType.Yoyo));

        iconSequence.Join(ResourceText.DOCounter(CurrentAmount, CurrentAmount + amount,1.2f)); //CountResource
        iconSequence.Join(ResourceText.transform.DOPunchScale(new Vector3(0.3f, 0.3f ,0.3f) , 0.8f, 0, 1f)); //Scale the number big when counting

        iconSequence.Append(EndGlow.GetComponent<Image>().DOFade(0.0f, 1f)); //Glow fade back to 0

        iconSequence.AppendCallback(() =>beginGlowParticle.Play());

        iconSequence.AppendInterval(4f);

        iconSequence.Play();

        iconSequence.OnComplete(() => DestroyObject(destroyEndPoint));
        iconSequence.OnComplete(() => DestroyObject(destroyBeginPoint));
        iconSequence.OnComplete(() => DestroyObject(destroyCoin));
    //End sequence.

    }

    private void PlayBurstParticleSystem(ParticleSystem sys, float delayTime, float particleAmount, int cycles, float waitBetweenCycles)
    {
        if (sys != null)
        {
            ParticleSystem.Burst[] bursts = { new ParticleSystem.Burst(delayTime, (short)particleAmount, (short)particleAmount, (short)cycles, waitBetweenCycles) };
            sys.emission.SetBursts(bursts);
            sys.Play();
        }
        else
        {
            Debug.LogError("Particle System is not assigned.");
        }
    }
    private void DestroyObject(GameObject gameObject)
    {
        Destroy(gameObject); // Destroy the game object
    }


    void PrepareParticle(out GameObject destroyContainer, GameObject particleContainer, Transform startTransform, out ParticleSystem particleSys)
    {
        particleSys = null; // Initialize the out parameter

        destroyContainer = Instantiate(particleContainer, startTransform);

        if (destroyContainer != null)
        {
            // Get the ParticleSystem component from the child
            particleSys = destroyContainer.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();

            // Check if the ParticleSystem component is found
            if (particleSys == null)
            {
                Debug.LogError("Child object doesn't have a ParticleSystem component.");
            }
        }
        else
        {
            Debug.LogError("Big particle container object not found.");
        }
    }


}