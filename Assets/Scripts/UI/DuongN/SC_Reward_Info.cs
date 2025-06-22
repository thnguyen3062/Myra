using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIParticleExtensions;

public class SC_Reward_Info : MonoBehaviour
{
    [SerializeField] private GameObject Bar;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI targetText;
    private GameObject AmountBigObj;
    [SerializeField] private GameObject XAmountBg;
    [SerializeField] private GameObject XAmount;
    private TextMeshProUGUI XAmtText;
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] public TextMeshProUGUI rewardNameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private GameObject BalanceBigObj;

    private Color notFullColor = new Color(69f / 255f, 228f / 255f, 1f, 1f);
    private Color fullColor = new Color(45f / 255f, 1f, 1.3f / 255f, 1f);
    [SerializeField] private GameObject Glow;
    [SerializeField] private GameObject Arrow;
    [SerializeField] private GameObject NextUpgrade;
    private TextMeshProUGUI NextUpgradeTxt;

    [SerializeField] private GameObject UpgradeAvailable;
    private Color color;

    private Material material;
    private Material glowMaterial;
    private Image image;
    private Image glowImage;
    private GameObject rarityObject;
    private GameObject rewardNameObject;
    private Sequence barSeq;
    private bool isBarSeqPlaying = false;
    private int dataRarity;
    private int dataTargetAmount;
    private int dataAmount;
    private int dataCurrentAmount;
    private int dataCurrentBalance;


    void Start()
    {

        image = Bar.GetComponent<Image>();
        glowImage = Glow.GetComponent<Image>();
        rarityObject = rarityText.transform.parent.gameObject;
        rewardNameObject = rewardNameText.transform.parent.gameObject;
        AmountBigObj = UpgradeAvailable.transform.parent.gameObject;
        NextUpgradeTxt = NextUpgrade.GetComponent<TextMeshProUGUI>();


        material = image.material;
        glowMaterial = glowImage.material;
        if (glowMaterial != null)
        {
            glowMaterial.SetFloat("_TilingX", 1f);
            glowMaterial.SetFloat("_Glow_Intensity", 0f);
        }
            
        TurnOff(Arrow, XAmount, XAmountBg, NextUpgrade, UpgradeAvailable, rarityObject, rewardNameObject);



        if (XAmount != null)
        {
            XAmtText = XAmount.GetComponent<TextMeshProUGUI>();

        }
        else
        {
            Debug.LogError("amountText reference is not set in the inspector.");
        }


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isBarSeqPlaying)
            {
                // Dang chay ma click -> ket thuc luon.
                isBarSeqPlaying = false;
                barSeq.Complete(withCallbacks: true);
                return;
            }
        }
    }

    void PlayBar(int rarity, int targetAmount, int amount, int currentAmount, int currentBalance)
    {
        TurnOff(Arrow, XAmount, XAmountBg, UpgradeAvailable, UpgradeAvailable, rarityObject, NextUpgrade);

        if (!isBarSeqPlaying)
        {
            isBarSeqPlaying = true; //Chinh bool true.
        }

        if (currentAmount + amount < targetAmount)
        {
            color = notFullColor;
        }
        else
        {
            color = fullColor;
        }

        //Chon mau khi xuat hien
        if (currentAmount < targetAmount)
        {
            color = notFullColor;
        }
        else
        {
            color = fullColor;
        }
        //Set mau cua  bar and texts
        if (image != null)
        {
            if (targetAmount == 0)
                image.fillAmount = 1;
            else
                image.fillAmount = (float)currentAmount / targetAmount;
        }
        if (material != null)
            material.color = color;

        if (amountText != null)
            amountText.color = Color.white;

        if (targetText != null)
            targetText.color = Color.white;

        //Set so luong target
        if (targetText != null)
        {
            targetText.text = "/ " + targetAmount.ToString();
        }
        else
        {
            Debug.LogError("amountText reference is not set in the inspector.");
        }
        //Set so luong hien tai amount
        if (amountText != null)
        {
            amountText.text = currentAmount.ToString();
        }
        else
        {
            Debug.LogError("amountText reference is not set in the inspector.");
        }

        //Set so luong hien tai balance
        if (balanceText != null)
        {
            balanceText.text = currentBalance.ToString();
        }
        else
        {
            Debug.LogError("amountText reference is not set in the inspector.");
        }


        barSeq = DOTween.Sequence();

        if ((int)rarity > 6 || rarity < 1)
        {
            rarity = 5;
        }

        switch (rarity)
        {
            case 1:
                {
                    barSeq.AppendCallback(() => TurnOn(AmountBigObj, Bar, Glow));
                    barSeq.AppendCallback(() => TurnOff(BalanceBigObj));

                    barSeq.AppendInterval(0.5f);

                    barSeq.AppendCallback(() => TurnOn(rarityObject, rewardNameObject));
                    barSeq.AppendCallback(() => DetermineRarity(rarity));
                    if (glowMaterial != null)
                    {
                        barSeq.Append(glowMaterial.DOFloat(11f, "_TilingX", 0.01f));
                        barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", 0.01f));
                    }
                        
                    barSeq.AppendCallback(() => TurnOn(XAmount, XAmountBg));
                    barSeq.AppendCallback(() => MultiplyAmount(XAmount, amount, 2));
                    if (glowMaterial != null)
                        barSeq.Append(glowMaterial.DOFloat(Mathf.Clamp((targetAmount / (float)(currentAmount + amount + 0.3f)) * 1.1f, 1f, 12f), "_TilingX", 0.01f));
                    barSeq.Join(image.DOFillAmount((float)(currentAmount + amount) / targetAmount, 1f));
                    barSeq.Join(image.DOColor(color, 0.01f));
                    barSeq.Join(amountText.DOColor(Color.white, 0.01f));
                    barSeq.Join(targetText.DOColor(Color.white, 0.01f));
                    if (material != null)
                    {
                        barSeq.Join(material.DOFloat(1.3f, "_Intensity_Bar", 0.01f));
                        barSeq.Join(material.DOFloat(1f, "_Intensity_Bar", 1f));
                    }
                        

                    //barSeq.Join(XAmountBg.transform.DOLocalMove(new Vector3(-545f, -232f, 0f), 0.01f));
                    //barSeq.Join(XAmount.transform.DOLocalMove(new Vector3(-545f, -232f, 0f), 0.01f));
                    barSeq.Join(XAmount.transform.DOScale(new Vector3(1f, 1.2f, 0f), 0.25f).SetLoops(2, LoopType.Yoyo).SetDelay(0.5f));
                    barSeq.Join(XAmtText.DOFade(1f, 0.3f));
                    if (glowMaterial != null)
                        barSeq.Join(glowMaterial.DOFloat(1f, "_Glow_Intensity", 1f).SetDelay(0.8F));
                    barSeq.Join(Bar.transform.DOScale(new Vector3(1f, 1.1f, 1f), 0.8f).SetLoops(2, LoopType.Yoyo));
                    barSeq.Join(amountText.DOCounter(currentAmount, currentAmount + amount, 1f));
                    barSeq.Join(amountText.transform.DOScale(new Vector3(1f, 1.3f, 1f), 0.7f).SetLoops(2, LoopType.Yoyo));


                    if (currentAmount + amount >= targetAmount && targetAmount != 0)
                    {

                        barSeq.Append(image.DOColor(fullColor, 0.5f));
                        barSeq.AppendCallback(() => TurnOn(Arrow, UpgradeAvailable));
                    }
                    else
                    {
                        barSeq.AppendCallback(() => TurnOn(NextUpgrade));
                    }

                    if (material != null)
                        barSeq.Append(material.DOFloat(1.15f, "_Intensity_Bar", 1f));
                    if (glowMaterial != null)
                        barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", 1f));

                    break;
                }
            case 2:
                {
                    barSeq.AppendCallback(() => TurnOn(AmountBigObj, Bar, Glow));
                    barSeq.AppendCallback(() => TurnOff(BalanceBigObj));

                    barSeq.AppendInterval(1f);

                    barSeq.AppendCallback(() => TurnOn(rarityObject, rewardNameObject));
                    barSeq.AppendCallback(() => DetermineRarity(rarity));
                    if(glowMaterial != null)
                    {
                        barSeq.Append(glowMaterial.DOFloat(11f, "_TilingX", 0.01f));
                        barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", 0.01f));
                    }
                    barSeq.AppendCallback(() => TurnOn(XAmount, XAmountBg));
                    barSeq.AppendCallback(() => MultiplyAmount(XAmount, amount, 2));
                    if(glowMaterial != null)
                    {
                        barSeq.Append(glowMaterial.DOFloat(Mathf.Clamp((targetAmount / (float)(currentAmount + amount + 0.3f)) * 1.1f, 1f, 12f), "_TilingX", 0.01f));
                    }
                    barSeq.Join(image.DOFillAmount((float)(currentAmount + amount) / targetAmount, 1f));
                    barSeq.Join(image.DOColor(color, 0.01f));
                    barSeq.Join(amountText.DOColor(Color.white, 0.01f));
                    barSeq.Join(targetText.DOColor(Color.white, 0.01f));
                    if(material != null)
                    {
                        barSeq.Join(material.DOFloat(1.3f, "_Intensity_Bar", 0.01f));
                        barSeq.Join(material.DOFloat(1f, "_Intensity_Bar", 1f));
                    }
                    //barSeq.Join(XAmountBg.transform.DOLocalMove(new Vector3(-545f, -232f, 0f), 0.01f));
                    //barSeq.Join(XAmount.transform.DOLocalMove(new Vector3(-545f, -232f, 0f), 0.01f));
                    barSeq.Join(XAmount.transform.DOScale(new Vector3(1f, 1.2f, 0f), 0.25f).SetLoops(2, LoopType.Yoyo).SetDelay(0.5f));
                    barSeq.Join(XAmtText.DOFade(1f, 0.3f));
                    if (glowMaterial != null)
                        barSeq.Join(glowMaterial.DOFloat(1f, "_Glow_Intensity", 1f).SetDelay(0.8F));
                    barSeq.Join(Bar.transform.DOScale(new Vector3(1f, 1.1f, 1f), 0.8f).SetLoops(2, LoopType.Yoyo));
                    barSeq.Join(amountText.DOCounter(currentAmount, currentAmount + amount, 1f));
                    barSeq.Join(amountText.transform.DOScale(new Vector3(1f, 1.3f, 1f), 0.7f).SetLoops(2, LoopType.Yoyo));

                    if (currentAmount + amount >= targetAmount && targetAmount != 0)
                    {
                        barSeq.Append(image.DOColor(fullColor, 0.5f));
                        barSeq.AppendCallback(() => TurnOn(Arrow, UpgradeAvailable));
                    }
                    else
                    {
                        barSeq.AppendCallback(() => TurnOn(NextUpgrade));
                    }
                    if (material != null)
                        barSeq.Append(material.DOFloat(1.15f, "_Intensity_Bar", 1f));
                    if (glowMaterial != null)
                        barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", 1f));

                    break;
                }
            case 3:
                {
                    barSeq.AppendCallback(() => TurnOn(AmountBigObj, Bar, Glow));
                    barSeq.AppendCallback(() => TurnOff(BalanceBigObj));

                    barSeq.AppendInterval(2f);

                    barSeq.AppendCallback(() => TurnOn(rarityObject, rewardNameObject));
                    barSeq.AppendCallback(() => DetermineRarity(rarity));
                    if(glowMaterial != null)
                    {
                        barSeq.Append(glowMaterial.DOFloat(11f, "_TilingX", 0.01f));
                        barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", 0.01f));
                    }
                    barSeq.AppendCallback(() => TurnOn(XAmount, XAmountBg));
                    barSeq.AppendCallback(() => MultiplyAmount(XAmount, amount, 2));
                    if (glowMaterial != null)
                        barSeq.Append(glowMaterial.DOFloat(Mathf.Clamp((targetAmount / (float)(currentAmount + amount + 0.3f)) * 1.1f, 1f, 12f), "_TilingX", 0.01f));
                    barSeq.Join(image.DOFillAmount((float)(currentAmount + amount) / targetAmount, 1f));
                    barSeq.Join(image.DOColor(color, 0.01f));
                    barSeq.Join(amountText.DOColor(Color.white, 0.01f));
                    barSeq.Join(targetText.DOColor(Color.white, 0.01f));
                    if (material != null)
                    {
                        barSeq.Join(material.DOFloat(1.3f, "_Intensity_Bar", 0.01f));
                        barSeq.Join(material.DOFloat(1f, "_Intensity_Bar", 1f));
                    }
                        
                    //barSeq.Join(XAmountBg.transform.DOLocalMove(new Vector3(-545f, -232f, 0f), 0.01f));
                    //barSeq.Join(XAmount.transform.DOLocalMove(new Vector3(-545f, -232f, 0f), 0.01f));
                    barSeq.Join(XAmount.transform.DOScale(new Vector3(1f, 1.2f, 0f), 0.25f).SetLoops(2, LoopType.Yoyo).SetDelay(0.5f));
                    barSeq.Join(XAmtText.DOFade(1f, 0.3f));
                    if (glowMaterial != null)
                        barSeq.Join(glowMaterial.DOFloat(1f, "_Glow_Intensity", 1f).SetDelay(0.8F));
                    barSeq.Join(Bar.transform.DOScale(new Vector3(1f, 1.1f, 1f), 0.8f).SetLoops(2, LoopType.Yoyo));
                    barSeq.Join(amountText.DOCounter(currentAmount, currentAmount + amount, 1f));
                    barSeq.Join(amountText.transform.DOScale(new Vector3(1f, 1.3f, 1f), 0.7f).SetLoops(2, LoopType.Yoyo));

                    if (currentAmount + amount >= targetAmount && targetAmount != 0)
                    {

                        barSeq.Append(image.DOColor(fullColor, 0.5f));
                        barSeq.AppendCallback(() => TurnOn(Arrow, UpgradeAvailable));
                    }
                    else
                    {
                        barSeq.AppendCallback(() => TurnOn(NextUpgrade));
                    }
                    if (material != null)
                        barSeq.Append(material.DOFloat(1.15f, "_Intensity_Bar", 1f));
                    if (glowMaterial != null)
                        barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", 1f));


                    break;
                }
            case 4:
                {
                    barSeq.AppendCallback(() => TurnOn(AmountBigObj, Bar, Glow));
                    barSeq.AppendCallback(() => TurnOff(BalanceBigObj));

                    barSeq.AppendInterval(3.5f);

                    barSeq.AppendCallback(() => TurnOn(rarityObject, rewardNameObject));
                    barSeq.AppendCallback(() => DetermineRarity(rarity));
                    if (glowMaterial != null)
                    {
                        barSeq.Append(glowMaterial.DOFloat(11f, "_TilingX", 0.01f));
                        barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", 0.01f));
                    }
                        
                    barSeq.AppendCallback(() => TurnOn(XAmount, XAmountBg));
                    barSeq.AppendCallback(() => MultiplyAmount(XAmount, amount, 2));
                    if (glowMaterial != null)
                        barSeq.Append(glowMaterial.DOFloat(Mathf.Clamp((targetAmount / (float)(currentAmount + amount + 0.3f)) * 1.1f, 1f, 12f), "_TilingX", 0.01f));
                    barSeq.Join(image.DOFillAmount((float)(currentAmount + amount) / targetAmount, 1f));
                    barSeq.Join(image.DOColor(color, 0.01f));
                    barSeq.Join(amountText.DOColor(Color.white, 0.01f));
                    barSeq.Join(targetText.DOColor(Color.white, 0.01f));
                    if (material != null)
                    {
                        barSeq.Join(material.DOFloat(1.3f, "_Intensity_Bar", 0.01f));
                        barSeq.Join(material.DOFloat(1f, "_Intensity_Bar", 1f));
                    }
                        
                    //barSeq.Join(XAmountBg.transform.DOLocalMove(new Vector3(-545f, -232f, 0f), 0.01f));
                    //barSeq.Join(XAmount.transform.DOLocalMove(new Vector3(-545f, -232f, 0f), 0.01f));
                    barSeq.Join(XAmount.transform.DOScale(new Vector3(1f, 1.2f, 0f), 0.25f).SetLoops(2, LoopType.Yoyo).SetDelay(0.5f));
                    barSeq.Join(XAmtText.DOFade(1f, 0.3f));
                    if (glowMaterial != null)
                        barSeq.Join(glowMaterial.DOFloat(1f, "_Glow_Intensity", 1f).SetDelay(0.8F));
                    barSeq.Join(Bar.transform.DOScale(new Vector3(1f, 1.1f, 1f), 0.8f).SetLoops(2, LoopType.Yoyo));
                    barSeq.Join(amountText.DOCounter(currentAmount, currentAmount + amount, 1f));
                    barSeq.Join(amountText.transform.DOScale(new Vector3(1f, 1.3f, 1f), 0.7f).SetLoops(2, LoopType.Yoyo));

                    if (currentAmount + amount >= targetAmount && targetAmount != 0)
                    {

                        barSeq.Append(image.DOColor(fullColor, 0.5f));
                        barSeq.AppendCallback(() => TurnOn(Arrow, UpgradeAvailable));
                    }
                    else
                    {
                        barSeq.AppendCallback(() => TurnOn(NextUpgrade));
                    }
                    if (material != null)
                        barSeq.Append(material.DOFloat(1.15f, "_Intensity_Bar", 1f));
                    if (glowMaterial != null)
                        barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", 1f));


                    break;
                }
            case 5:
                {
                    barSeq.AppendCallback(() => TurnOff(AmountBigObj, Bar, Glow));

                    barSeq.AppendInterval(0.5f);
                    barSeq.AppendCallback(() => TurnOn(BalanceBigObj, rarityObject, rewardNameObject, XAmount, XAmountBg));
                    barSeq.AppendCallback(() => DetermineRarity(rarity));
                    barSeq.AppendCallback(() => MultiplyAmount(XAmount, amount, 2));
                    barSeq.Join(XAmtText.DOFade(0f, 0.01f));
                    barSeq.AppendInterval(0.5f);
                    //barSeq.Join(XAmountBg.transform.DOLocalMove(new Vector3(-545f, -34f, 0f), 0.01f));
                    //barSeq.Join(XAmount.transform.DOLocalMove(new Vector3(-545f, -34f, 0f), 0.01f));
                    barSeq.Join(XAmount.transform.DOScale(new Vector3(1.05f, 1.2f, 0f), 0.25f).SetLoops(2, LoopType.Yoyo).SetDelay(0.5f));
                    barSeq.Join(XAmtText.DOFade(1f, 0.3f));

                    barSeq.Join(balanceText.DOCounter(currentBalance, currentBalance + amount, 1f));
                    barSeq.Join(balanceText.transform.DOScale(new Vector3(1f, 1.3f, 1f), 0.7f).SetLoops(2, LoopType.Yoyo));

                    break;
                }

            case 6:
                {
                    barSeq.AppendCallback(() => TurnOff(AmountBigObj, Bar, Glow));

                    barSeq.AppendInterval(0.5f);

                    barSeq.AppendCallback(() => TurnOn(BalanceBigObj, rarityObject, XAmount, XAmountBg));
                    barSeq.AppendCallback(() => DetermineRarity(rarity));
                    barSeq.AppendCallback(() => MultiplyAmount(XAmount, amount, 2));

                    barSeq.Join(XAmtText.DOFade(0f, 0.01f));
                    barSeq.AppendInterval(0.5f);
                    rewardNameText.text = "Coins for duplicate cards.";
                    //barSeq.Join(XAmountBg.transform.DOLocalMove(new Vector3(-545f, -34f, 0f), 0.01f));
                    //barSeq.Join(XAmount.transform.DOLocalMove(new Vector3(-545f, -34f, 0f), 0.01f));
                    barSeq.Join(XAmount.transform.DOScale(new Vector3(1.05f, 1.2f, 0f), 0.25f).SetLoops(2, LoopType.Yoyo).SetDelay(0.5f));
                    barSeq.Join(XAmtText.DOFade(1f, 0.3f));

                    barSeq.Join(balanceText.DOCounter(currentBalance, currentBalance + amount, 1f));
                    barSeq.Join(balanceText.transform.DOScale(new Vector3(1f, 1.3f, 1f), 0.7f).SetLoops(2, LoopType.Yoyo));

                    break;
                }

            default:
                Debug.LogError("Invalid animation type");
                break;
        }

        barSeq.OnComplete(() =>
        {
            // Chinh bool lai thanh false
            if (isBarSeqPlaying)
            {
                isBarSeqPlaying = false;
            }
        });

        barSeq.Play();
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

    private void MultiplyAmount(GameObject obj, int amt, int a)
    {
        if (obj != null)
        {
            TextMeshProUGUI textComponent = obj.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = "x" + amt.ToString();
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on the GameObject.");
            }
        }
        else
        {
            Debug.LogError("GameObject is null for MultiplyAmount.");
        }
    }
    private void DetermineRarity(int rare)
    {
        switch (rare)
        {
            case 1:
                rarityText.text = "Rarity: Common";
                break;
            case 2:
                rarityText.text = "Rarity: Rare";
                break;
            case 3:
                rarityText.text = "Rarity: Epic";
                break;
            case 4:
                rarityText.text = "Rarity: Legendary";
                break;
            case 5:
                rarityText.text = "Resources";
                break;
            case 6:
                rarityText.text = "You owned all possible cards. You get fallback coins instead.";
                break;
            default:
                break;
        }
    }

    private void DetermineFrame(int fr)
    {
        if(NextUpgradeTxt != null)
        {
            switch (fr)
            {
                case 1:
                    NextUpgradeTxt.text = ($"Next Upgrade: LEVEL 2");
                    break;
                case 2:
                    NextUpgradeTxt.text = ($"Next Upgrade: LEVEL 3");
                    break;
                case 3:
                    NextUpgradeTxt.text = ($"Next Upgrade: LEVEL 4");
                    break;
                case 4:
                    NextUpgradeTxt.text = ($"Next Upgrade: LEVEL 5");
                    break;
                case 5:
                    NextUpgradeTxt.text = ("CARD LEVEL MAXED.");
                    break;
                default:
                    break;
            }
        }
    }
    private void OnEnable()
    {
        // Subscribe to the event when this script is enabled
        SC_SingleRewardAnim.OnCardAnimationStart += PlayBar;
        SC_SingleRewardAnim.OnFrameStart += DetermineFrame;
    }
    private void OnDisable()
    {
        // Unsubscribe from the event when this script is disabled or destroyed
        SC_SingleRewardAnim.OnCardAnimationStart -= PlayBar;
        SC_SingleRewardAnim.OnFrameStart -= DetermineFrame;
    }
}