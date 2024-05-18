using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Adinmo;
using TMPro;

public class ZBDController: MonoBehaviour
{

    #region Public variables

    [Header("Bitcoin Counter Button")]
    // Reference to the Text component that displays the bitcoins numbers
    public Text MainBitcoinCounter;
    [Header("Cash Out Panels")]
    // Reference to the pabel warning 
    public GameObject Warning_Panel;
    // Reference to the CashOut panel
    public GameObject CashOut_Panel;
    // Reference to the error panrl component 
    public GameObject ErrorPanel;
    // Reference to the text for the Error
    public Text ErrorText;
    // Reference to the SucessPanel
    public GameObject SuccessPanel;
    // Reference to the Success Text
    public Text SuccessText;
    // the amount in dollars on the display screen
    public Text DollarText;

    [Header("Cashout Bitcoin Panel Elements")]
    // Reference to the Bitcoin amount to convert
    public Text CashoutBitcoinAmount;
    // Reference to the Slider
    public Slider Slider;
    // Reference to the Slider Amount
    public Text SlideAmount;
    // Reference to the GamerTag component 
    public InputField GamerTagInputField;
    // WidthdrawlButton
    public Button WithdrawlButton;
    // time allowed to animate the total updating
    const float ANIMATION_TIME = 1.0f;
    // remaining time in the animation
    float animationTimeRemaining = 0.0f;
    uint currentValue = 0;
    uint targetValue = 0;

    #endregion

    #region private variables
    // Color to change
    private Color SelectedColor;

    #endregion


    public void Start()
    {
        if (AdinmoManager.IsReady())
        {
            statusCallback("");
        }
        else
        {
            gameObject.SetActive(false);
            AdinmoManager.SetOnReadyCallback(statusCallback);
        }
        
    }
    public void CashOutBitcoin()
    {
        // Check if the number of Bitcoin is greater than 0 to open the withdraw panel if not open the warning panel.
        uint satsBalance = AdinmoManager.GetZBDSatsBalance();
        CashoutBitcoinAmount.text = AdinmoUtilities.ToEngineeringNotation(satsBalance);
        DollarText.text = "$"+AdinmoManager.GetZBDDollarBalance();
        GamerTagInputField.SetTextWithoutNotify(AdinmoManager.GetZBDGamerTag());
        FillSlider(satsBalance, AdinmoManager.GetZBDGamerTag());
        CashOut_Panel.SetActive(true);
            
           
       
    }

  

    //convert bitcoint amount to Slider
    public void FillSlider(uint satsBalance, string gamertag)
    {
        if (satsBalance == 0 || string.IsNullOrEmpty(gamertag))
        {
            Slider.maxValue = 0;
            Slider.minValue = 0;
            WithdrawlButton.interactable = false;
            Slider.value = 0;
            SlideAmount.text = "Withdraw 0";
        }
        else
        {
            Slider.minValue = 1;
            Slider.maxValue = satsBalance;
            WithdrawlButton.interactable = true;
            Slider.value = satsBalance;
            SlideAmount.text = "Withdraw " + AdinmoUtilities.ToEngineeringNotation(satsBalance);
        }

    }

    public void SliderUpdated()
    {
        SlideAmount.text = "Withdraw " + AdinmoUtilities.ToEngineeringNotation(Slider.value);
    }
  
    //Open the link for download the zebedee wallet
    public void GetZebedeeWallet()
    {

        Application.OpenURL(AdinmoManager.GetRewardedUrlTracker());
    }

    public void OnEnable()
    {
        AdinmoManager.SetZBDOpCallback(opCallback);
        AdinmoManager.SetZBDUpdateCallback(updateCallback);
        uint satsBalance = AdinmoManager.GetZBDSatsBalance();
        CashoutBitcoinAmount.text = AdinmoUtilities.ToEngineeringNotation(satsBalance);
        SetBitcoinCounter(satsBalance,false);
        DollarText.text = "$" + AdinmoManager.GetZBDDollarBalance();
    }

    public void OnDisable()
    {
        AdinmoManager.SetZBDOpCallback(null);
        AdinmoManager.SetZBDUpdateCallback(null);
    }

    void statusCallback(string message)
    {
        gameObject.SetActive(AdinmoManager.SupportsZebedee);
        AdinmoManager.SetZBDOpCallback(opCallback);
        AdinmoManager.SetZBDUpdateCallback(updateCallback);

        uint satsBalance = AdinmoManager.GetZBDSatsBalance();
        CashoutBitcoinAmount.text = AdinmoUtilities.ToEngineeringNotation(satsBalance);
        SetBitcoinCounter(satsBalance,false);
        DollarText.text = "$"+AdinmoManager.GetZBDDollarBalance();


    }
    void opCallback(bool success, string gamertag, uint balanceSats, float balanceChangeSats, float balanceDollars, string feedback)
    {
        AdinmoManager.LogInfo("zbd op callback success=" + success + ", gamerTag=" + gamertag + ", balanceSats=" + balanceSats + ", balanceDollars=" + balanceDollars + ", balanceChange=" + balanceChangeSats + ", feeedbacl=" + feedback);
        CashoutBitcoinAmount.text = AdinmoUtilities.ToEngineeringNotation(balanceSats);
        SetBitcoinCounter(balanceSats);
        DollarText.text = "$"+AdinmoManager.GetZBDDollarBalance();
        GamerTagInputField.interactable = false;
        GamerTagInputField.text = gamertag;
        GamerTagInputField.interactable = true;
        FillSlider(balanceSats, gamertag);
        if (!string.IsNullOrEmpty(feedback))
        {
            if(success)
            {
                CashOut_Panel.SetActive(false);
                SuccessPanel.SetActive(true);
                SuccessText.text = feedback;

            }
            else
            {
                ErrorPanel.SetActive(true);
                ErrorText.text = feedback;
            }
        }


    }

    void updateCallback(bool success, string gamertag, uint balanceSats, float balanceChangeSats, float balanceDollars, string feedback)
    {
        AdinmoManager.LogInfo("zbd update callback success=" + success + ", gamerTag=" + gamertag + ", balanceSats=" + balanceSats + ", balanceDollars=" + balanceDollars + ", balanceChange=" + balanceChangeSats + ", feeedback=" + feedback);
        SetBitcoinCounter(balanceSats);
    }

    public void EditGamerTag(string oldTag)
    {
        AdinmoManager.SetZBDGamerTag(GamerTagInputField.text);
    }

    public void ClickWithdraw()
    {
        WithdrawlButton.interactable = false;
        AdinmoManager.ZBDCashOut(GamerTagInputField.text, (int)Slider.value * 1000);

    }

    private void Update()
    {
        if(animationTimeRemaining>0)
        {
            animationTimeRemaining -= Time.deltaTime;
            if(animationTimeRemaining>0)
            {
                uint displayValue = (uint)Mathf.Lerp(targetValue, currentValue, animationTimeRemaining / ANIMATION_TIME);
                MainBitcoinCounter.text = AdinmoUtilities.ToEngineeringNotation(displayValue);
            }
            else
            {
                currentValue = targetValue;
                MainBitcoinCounter.text = AdinmoUtilities.ToEngineeringNotation(targetValue);
                animationTimeRemaining = 0.0f;
            }

        }
    }

    void SetBitcoinCounter(uint newAmount, bool animated=true)
    {
        if (newAmount != currentValue)
        {
            if (!animated)
            {
                currentValue = newAmount;
                MainBitcoinCounter.text = AdinmoUtilities.ToEngineeringNotation(newAmount);
            }
            else
            {
                targetValue = newAmount;
                animationTimeRemaining = ANIMATION_TIME;
            }
        }
    }    

}
