using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIParticleExtensions;
using System.Linq;

public class SC_Info_Upgrade : MonoBehaviour
{
    public int requiredAmount; //Số lá bài giữa viền hiện tại và viền tiếp theo
    public int currentAmount; //Số lá bài hiện tại, lấy từ script Hương
    public List<int> CurrentAmountsCoin_Ess_Card; // Thứ tự: Card / Coin / Essence, //Lấy từ script Hương
    public List<int> RequiredAmountsCoin_Ess_Card; // Thứ tự: Card / Coin / Essence, //Lấy từ script Hương
    public List<int> currentStatIntAtk_Hp_Mana; //Lấy từ script Hương
    public List<int> newStatIntAtk_Hp_Mana; //Lấy từ script Hương
    public int Level = 1; //1: Bạc, 2: Vàng, 3: Kim cương, 4: Đen, Lấy từ script Hương.
    public int LevelVar
    {
        get { return Level; }
        set
        {
            if (Level != value)
            {
                Level = value;
                OnLevelChanged?.Invoke(Level);
            }
        }
    }
    public int BonusAmount = 195; //Lấy từ script Hương
    public int bonusAmount
    {
        get { return BonusAmount; }
        private set { BonusAmount = value; }
    }
    public delegate void LevelChangedEventHandler(int newLevel);
    public event LevelChangedEventHandler OnLevelChanged;

    private GameObject Bar;
    private TextMeshProUGUI amountText;
    private TextMeshProUGUI requiredText;
    private Color notFullColor = new Color(69f / 255f, 228f / 255f, 1f, 1f);
    private Color fullColor = new Color(45f / 255f, 1f, 1.3f / 255f, 1f);
    private GameObject Glow;
    private Color color;
    private Material barMaterial;
    private Material glowMaterial;
    private Image BarImage;
    private Image glowImage;
    private Sequence barSeq;
    private bool isBarSeqPlaying = false;
    [SerializeField] private TextMeshProUGUI CurrentLevel;
    [SerializeField] private GameObject[] BigObjectCoin_Ess_Card; // Thứ tự: Card / Coin / Essence
    [SerializeField] private GameObject[] statObjectsAtk_Hp_Mana;
    [SerializeField]  private TextMeshProUGUI[] statTMP;
    [SerializeField]  private TextMeshProUGUI[] currentStatTextsAtk_Hp_Mana;
    [SerializeField]  private TextMeshProUGUI[] newStatTextsAtk_Hp_Mana;
    [SerializeField] private GameObject bonusObject;
    [SerializeField] private TextMeshProUGUI bonusText;
    public SC_Button_Upgrade ButtonScript_Bar;
    public delegate void AllBarsFilledEventHandler(List<int> currentAmount, List<int> requiredAmount);
    public event AllBarsFilledEventHandler OnAllBarsFilled; //Khi các bars đầy, gửi sự kiện này để cho phép trừ số balance khi upgrade.
    public delegate void CheckBarEventHandler(int test);
    public event CheckBarEventHandler CheckBarEvent;
    private bool AllBarsFilled = true;
    public bool allBarsFilled
    {
        get { return AllBarsFilled; }
        private set { AllBarsFilled = value; }
    }

    void Start()
    {
        OnAllBarsFilled += DeductRequiredAmount;
        if (ButtonScript_Bar != null)
        {
            // Subscribe to OnUpgradeButtonClick with null checks
            ButtonScript_Bar.OnUpgradeButtonClick += () => IncreaseLevel(LevelVar);
            ButtonScript_Bar.OnUpgradeButtonClick += () => DeductRequiredAmount(CurrentAmountsCoin_Ess_Card, RequiredAmountsCoin_Ess_Card);
            //ButtonScript_Bar.OnUpgradeButtonClick += () => UpdateInfo(LevelVar);

            for (int i = 0; i < statObjectsAtk_Hp_Mana.Length; i++)
            {
                int index = i; // Capture the variable to avoid closure issues

                ButtonScript_Bar.OnUpgradeButtonClick += () => DisplayAttributeChanges(currentStatIntAtk_Hp_Mana[index], newStatIntAtk_Hp_Mana[index], statObjectsAtk_Hp_Mana[index], currentStatTextsAtk_Hp_Mana[index], newStatTextsAtk_Hp_Mana[index]);
            }
        }        

        //OnLevelChanged += UpdateInfo;

        DetermineLevel(LevelVar);

        for (int i = 0; i < statObjectsAtk_Hp_Mana.Length; i++)
        {
            int index = i;

            Debug.Log(currentStatIntAtk_Hp_Mana[i]);
            Debug.Log(newStatIntAtk_Hp_Mana[i]);
            Debug.Log(statObjectsAtk_Hp_Mana[i]);
            Debug.Log(newStatTextsAtk_Hp_Mana[i]);
            DisplayAttributeChanges(currentStatIntAtk_Hp_Mana[index], newStatIntAtk_Hp_Mana[index], statObjectsAtk_Hp_Mana[index], currentStatTextsAtk_Hp_Mana[index], newStatTextsAtk_Hp_Mana[index]);
        }

        //UpdateInfo(LevelVar);
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         if (isBarSeqPlaying)
    //         {
    //             // Dang chay ma click -> ket thuc luon.
    //             barSeq.Complete( withCallbacks: true );
    //             isBarSeqPlaying = false;
    //             return;
    //         }
    //     }
    // }

    public SC_Info_Upgrade LoadData()
    {
        // Initialize private arrays with the same size as statObjectsAtk_Hp_Mana vì không serialize.
        statTMP = new TextMeshProUGUI[statObjectsAtk_Hp_Mana.Length];
        currentStatTextsAtk_Hp_Mana = new TextMeshProUGUI[statObjectsAtk_Hp_Mana.Length];
        newStatTextsAtk_Hp_Mana = new TextMeshProUGUI[statObjectsAtk_Hp_Mana.Length];

        for (int i = 0; i < statObjectsAtk_Hp_Mana.Length; i++)
        {
            currentStatTextsAtk_Hp_Mana[i] = statObjectsAtk_Hp_Mana[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            newStatTextsAtk_Hp_Mana[i] = statObjectsAtk_Hp_Mana[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        for (int i = 0; i < statObjectsAtk_Hp_Mana.Length; i++)
        {
            statTMP[i] = statObjectsAtk_Hp_Mana[i].GetComponent<TextMeshProUGUI>();
        }
        for (int i = 0; i < statObjectsAtk_Hp_Mana.Length; i++)
        {
            int index = i; // Capture the variable to avoid closure issues
            DisplayAttributeChanges(currentStatIntAtk_Hp_Mana[index], newStatIntAtk_Hp_Mana[index], statObjectsAtk_Hp_Mana[index], currentStatTextsAtk_Hp_Mana[index], newStatTextsAtk_Hp_Mana[index]);
        }
        return this;
    }
    public SC_Info_Upgrade UpdateInfo(int level)
    {
        DetermineLevel(LevelVar);

        int countFull = 0;

        if (level < 5 && level > 0)
        {
            if (!isBarSeqPlaying)
            {
                isBarSeqPlaying = true; //Chinh bool true.
            }

            for (int i = 0; i < BigObjectCoin_Ess_Card.Length; i++)
            {

                //TurnOn(BigObjectCoin_Ess_Card[i].transform.parent.gameObject);

                TurnOn(BigObjectCoin_Ess_Card[i]);

                currentAmount = CurrentAmountsCoin_Ess_Card[i];
                requiredAmount = RequiredAmountsCoin_Ess_Card[i];

                if (currentAmount < requiredAmount)
                {
                    color = notFullColor;
                }
                else
                {
                    color = fullColor;
                    countFull++;
                }

                BarImage = BigObjectCoin_Ess_Card[i].transform.GetChild(2).GetComponent<Image>();
                glowImage = BigObjectCoin_Ess_Card[i].transform.GetChild(3).GetComponent<Image>();
                amountText = BigObjectCoin_Ess_Card[i].transform.GetChild(4).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                requiredText = BigObjectCoin_Ess_Card[i].transform.GetChild(4).transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                Bar = BigObjectCoin_Ess_Card[i].transform.GetChild(2).gameObject;

                barMaterial = BarImage.material;
                glowMaterial = glowImage.material;
                glowMaterial.SetFloat("_TilingX", 1f);
                glowMaterial.SetFloat("_Glow_Intensity", 0f);

                //Set mau cua  bar and texts
                BarImage.fillAmount = (float)currentAmount / requiredAmount;
                barMaterial.color = color;
                amountText.color = Color.white;
                requiredText.color = Color.white;

                //Set so luong target
                if (requiredText != null)
                {
                    requiredText.text = "/ " + (requiredAmount).ToString();
                }
               
                //Set so luong hien tai amount
                if (amountText != null)
                {
                    amountText.text = (currentAmount).ToString();
                }
                

                barSeq = DOTween.Sequence();

                barSeq.AppendCallback(() => TurnOn(Glow));
                barSeq.AppendInterval(0.5f);

                barSeq.Append(glowMaterial.DOFloat(11f, "_TilingX", 0.01f));
                barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", 0.01f));

                barSeq.Append(glowMaterial.DOFloat(Mathf.Clamp((requiredAmount / (float)(currentAmount + 1.9f)) * 1.1f, 1f, 12f), "_TilingX", 0.01f));
                barSeq.Join(BarImage.DOFillAmount((float)(currentAmount) / requiredAmount, 1f));
                barSeq.Join(BarImage.DOColor(color, 0.01f));
                barSeq.Join(amountText.DOColor(Color.white, 0.01f));
                barSeq.Join(requiredText.DOColor(Color.white, 0.01f));
                barSeq.Join(barMaterial.DOFloat(1.3f, "_Intensity_Bar", 0.01f));
                barSeq.Join(barMaterial.DOFloat(1f, "_Intensity_Bar", 1f));
                barSeq.Join(glowMaterial.DOFloat(1f, "_Glow_Intensity", 1f).SetDelay(0.8F));
                barSeq.Join(Bar.transform.DOScale(new Vector3(1f, 1.1f, 1f), 0.8f).SetLoops(2, LoopType.Yoyo));
                barSeq.Join(amountText.DOCounter(currentAmount, currentAmount, 1f));
                barSeq.Join(amountText.transform.DOScale(new Vector3(1f, 1.3f, 1f), 0.7f).SetLoops(2, LoopType.Yoyo));

                barSeq.Append(barMaterial.DOFloat(1.15f, "_Intensity_Bar", 1f));
                barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", 1f));

                if (currentAmount >= requiredAmount)
                {
                    barSeq.Join(BarImage.DOColor(fullColor, 0.5f));
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

            if (countFull < 3 || LevelVar >= 5 || LevelVar <= 0)
            {
                allBarsFilled = false;
                CheckBarEvent?.Invoke(level); //CheckButtonStatus
            }
            else
            {
                allBarsFilled = true;
                OnAllBarsFilled?.Invoke(CurrentAmountsCoin_Ess_Card.ToList(), RequiredAmountsCoin_Ess_Card.ToList());
                CheckBarEvent?.Invoke(level);
            }
        }
        else
        {
            for (int i = 0; i < BigObjectCoin_Ess_Card.Length; i++)
            {
                TurnOff(BigObjectCoin_Ess_Card[i]);
                //TurnOff(BigObjectCoin_Ess_Card[i].transform.parent.gameObject);
            }
        }
        bonusText.text = "+" + bonusAmount.ToString();
        return this;
    }
    private void DeductRequiredAmount(List<int> CurrentAmountsCoin_Ess_Card, List<int> RequiredAmountsCoin_Ess_Card)
    {
        for (int i = 0; i < CurrentAmountsCoin_Ess_Card.Count; i++)
        {
            CurrentAmountsCoin_Ess_Card[i] = CurrentAmountsCoin_Ess_Card[i] - RequiredAmountsCoin_Ess_Card[i];
        }
    }

    private void IncreaseLevel(int level)
    {
        if (Level < 5)
        {
            Level += 1;
        }
    }

    private string DetermineLevel(int lvl)
    {
        switch (lvl)
        {
            case 1:
                {
                    CurrentLevel.text = "CARD LEVEL: 1";
                    return "CARD LEVEL: 1";
                }
            case 2:
                {
                    CurrentLevel.text = "CARD LEVEL: 2";
                    return "CARD LEVEL: 2";
                }
            case 3:
                {
                    CurrentLevel.text = "CARD LEVEL: 3";
                    return "CARD LEVEL: 3";
                }
            case 4:
                {
                    CurrentLevel.text = "CARD LEVEL: 4";
                    return "CARD LEVEL: 4";
                }
            case 5:
                {
                    CurrentLevel.text = "CARD LEVEL: 5";
                    return "CARD LEVEL: 5";
                }
            case 6:
                {
                    CurrentLevel.text = "CARD LEVEL: MAXED";
                    return "CARD LEVEL: MAXED";
                    break;
                }
            default:
                {
                    return ("");
                }
        }
    }

    void DisplayAttributeChanges(int current, int newValue, GameObject attributeObject, TextMeshProUGUI textComponent1, TextMeshProUGUI textComponent2)
    {
        string currentDisplayText;
        string newDisplayText;
        TextMeshProUGUI attNameTemp;
        GameObject arrowUp = attributeObject.transform.GetChild(2).gameObject;
        GameObject arrowHor = attributeObject.transform.GetChild(3).gameObject;

        attNameTemp = attributeObject.transform.GetComponent<TextMeshProUGUI>();

        // Check if the new value is different from the current value
        if (current != newValue)
        {
            // Display both current and new attributes with green color
            attNameTemp.color = Color.green;
            attNameTemp.text = attNameTemp.text;
            currentDisplayText = $"<color=green>{current}</color>";
            newDisplayText = $"<color=green>{newValue}</color>";

            textComponent2.gameObject.SetActive(true);
            textComponent1.text = currentDisplayText;
            textComponent2.text = newDisplayText;
            textComponent2.gameObject.SetActive(true);
            arrowUp.gameObject.SetActive(true);
            arrowHor.gameObject.SetActive(true);
        }
        else
        {
            // Display either current or new attribute with the current color
            attNameTemp.color = Color.white;
            attNameTemp.text = attNameTemp.text;
            currentDisplayText = $"<color=white>{current}</color>";
            textComponent2.gameObject.SetActive(false);
            textComponent1.text = currentDisplayText;
            arrowUp.gameObject.SetActive(false);
            arrowHor.gameObject.SetActive(false);
        }
    }


    void OnDestroy()
    {
        if (ButtonScript_Bar != null)
        {
            //ButtonScript_Bar.OnUpgradeButtonClick -= () => UpdateInfo(LevelVar);
            ButtonScript_Bar.OnUpgradeButtonClick -= () => IncreaseLevel(LevelVar);

            for (int i = 0; i < statObjectsAtk_Hp_Mana.Length; i++)
            {
                int index = i; // Capture the variable to avoid closure issues
                ButtonScript_Bar.OnUpgradeButtonClick -= () => DisplayAttributeChanges(currentStatIntAtk_Hp_Mana[index], newStatIntAtk_Hp_Mana[index], statObjectsAtk_Hp_Mana[index], currentStatTextsAtk_Hp_Mana[index], newStatTextsAtk_Hp_Mana[index]);
            }
        }
        OnAllBarsFilled -= DeductRequiredAmount;
        //OnLevelChanged -= UpdateInfo;
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
}