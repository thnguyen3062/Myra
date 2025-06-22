using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIParticleExtensions;

public class SC_Button_Upgrade : MonoBehaviour
{
    private TextMeshProUGUI buttonText;
    private string textForButton;
    [SerializeField] private SC_UpgradeCard RewardObject;
    [SerializeField] private SC_Info_Upgrade InfoScriptObject;
    [SerializeField] private SC_Bar_Nonstop BonusBar;
    private Button thisButton;
    public delegate void ButtonClickEventHandler();
    public event ButtonClickEventHandler OnUpgradeButtonClick;
    [SerializeField] private GameObject Arrow;
    public delegate void WiggleEventHandlerOn();
    public event WiggleEventHandlerOn OnButtonStatusOn;
    public delegate void WiggleEventHandlerOff();
    public event WiggleEventHandlerOff OnButtonStatusOff;
    private bool barsFilled = false;


    void Start()
    {
        thisButton = transform.GetComponent<Button>();

        if (InfoScriptObject != null)
        {
            InfoScriptObject.CheckBarEvent += UpdateButtonStatus;
        }

        if (InfoScriptObject != null)
        {
            int level = InfoScriptObject.LevelVar;

            InfoScriptObject.OnLevelChanged += UpdateButtonStatus;

        }

        if (BonusBar != null)
        {
            BonusBar.BonusFlyDone += UpdateButtonStatus;
        }

        buttonText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        UpdateButtonStatus(InfoScriptObject.LevelVar);

        thisButton.onClick.AddListener(OnUpgradeButtonClickHandler);

        if (buttonText != null)
        {
            // Change the text content
        }

    }
    void OnUpgradeButtonClickHandler()
    {
        // Invoking
        OnUpgradeButtonClick?.Invoke();

        //Đoạn này để tắt nút đi khi chạm đến max level.

        List<int> differences = new List<int>();

        if (InfoScriptObject.LevelVar >= 5 || InfoScriptObject.LevelVar <= 0 || !InfoScriptObject.allBarsFilled)
        {
            thisButton.interactable = false;
            TurnOff(Arrow);
            OnButtonStatusOff?.Invoke();
        }
        else
        {
            UpdateButtonStatus(InfoScriptObject.LevelVar);
        }

    }

    public void UpdateButtonStatus(int level)
    {
        barsFilled = InfoScriptObject.allBarsFilled;

        bool animPlaying = RewardObject.isCardSeqPlaying;

        bool coinPlaying = BonusBar.isBarSeqPlaying;

        textForButton = DetermineLevel(level + 1);

        Button button = GetComponent<Button>();

        if (buttonText != null)
        {
            // Change the text content
            buttonText.text = textForButton;
        }

        if (level < 5 && level > 0 && barsFilled && !animPlaying && !coinPlaying)
        {
            button.interactable = true;

            OnButtonStatusOn?.Invoke();

            TurnOn(Arrow);
        }
        else
        {
            button.interactable = false;

            OnButtonStatusOff?.Invoke();

            TurnOff(Arrow);
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

    private string DetermineLevel(int lvl)
    {
        switch (lvl)
        {
            case 1:
                {
                    return "UPGRADE TO LEVEL 1";
                }
            case 2:
                {
                    return "UPGRADE TO LEVEL 2";
                }
            case 3:
                {
                    return "UPGRADE TO LEVEL 3";
                }
            case 4:
                {
                    return "UPGRADE TO LEVEL 4";
                }
            case 5:
                {
                    return "UPGRADE TO LEVEL 5";
                }
            case 6:
                {
                    return "THIS CARD IS AT MAX LEVEL.";
                }
            default:
                {
                    return ("UPGRADE CARD.");
                }
        }
    }

    private void OnDestroy()
    {
        if (InfoScriptObject != null)
        {
            InfoScriptObject.CheckBarEvent -= UpdateButtonStatus;
        }
        if (InfoScriptObject != null)
        {
            InfoScriptObject.OnLevelChanged -= UpdateButtonStatus;
        }
        if (BonusBar != null)
        {
            BonusBar.BonusFlyDone -= UpdateButtonStatus;
        }
    }
}