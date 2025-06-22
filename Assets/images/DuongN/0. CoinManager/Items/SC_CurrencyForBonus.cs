using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIParticleExtensions;

public class SC_CurrencyForBonus : MonoBehaviour
{
    [SerializeField] private GameObject BeginPoint;
    [SerializeField] private int amount;
    [SerializeField] private GameObject endPointBg;
    [SerializeField] private GameObject BeginParticleContainer_2;
    [SerializeField] private GameObject coinContainer_3;
    public SC_Button_Upgrade ButtonScript_Bar;
    public SC_Info_Upgrade InfoScript;
    private int scaleDown;
    private GameObject destroyBeginPoint;
    private GameObject destroyCoin;
    private ParticleSystem coinParticle;
    private int cycle;
    private float waitBtwCycles;
    public GameObject ParticleAttractorToSet;
    private Material targetMaterial;
    private int clampAmount;
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
            ButtonScript_Bar.OnUpgradeButtonClick += () => CountBonus(InfoScript.bonusAmount);
        } 

    //Instantiate glow particle
        PrepareParticle( out destroyBeginPoint , BeginParticleContainer_2 , BeginPoint.transform , out beginGlowParticle);
        
    //Instantiate coin particles
        Vector3 beginPosition = new Vector3(BeginPoint.transform.localPosition.x, BeginPoint.transform.localPosition.y, 0);
        BeginPoint.transform.localPosition = beginPosition;

        PrepareParticle( out destroyCoin , coinContainer_3 , BeginPoint.transform , out coinParticle);


    }


    void CountBonus(int bonus)
    {
        if(coinParticle == null)
        {
            Vector3 beginPosition = new Vector3(BeginPoint.transform.localPosition.x, BeginPoint.transform.localPosition.y, 0);
            BeginPoint.transform.localPosition = beginPosition;
            PrepareParticle( out destroyCoin , coinContainer_3 , BeginPoint.transform , out coinParticle);
        }

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
        
        coinSequence.AppendCallback(() => PlayParticle(beginGlowParticle));

        coinSequence.AppendInterval(5f);

        coinSequence.Play();

    //End coin sequence, stop glow, destroy coin
        coinSequence.OnComplete(() => StopParticle(beginGlowParticle));
        coinSequence.OnComplete(() => StopParticle(coinParticle));
        coinSequence.OnComplete(() => DestroyObject(destroyCoin));
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


    void PlayParticle(ParticleSystem particleSys)
        {
            if (particleSys != null)
            {
                particleSys.Play();
            }
        }

    void StopParticle(ParticleSystem particleSys)
    {

        if (particleSys != null)
        {
            particleSys.Stop();
        }
    }

    void OnDestroy()
    {
        DestroyObject(destroyCoin);
        DestroyObject(destroyBeginPoint);
    }

}