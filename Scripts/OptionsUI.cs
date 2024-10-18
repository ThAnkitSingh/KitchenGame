using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameInput;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;    
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button pasueButton;


    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private TextMeshProUGUI musicText;

    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlternateText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Transform pressToRebindKey;

    private Action onCloseButtonAction;

    private void Awake()
    {
        Instance = this;
        soundEffectButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisuals();
        });
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisuals();
        });
        closeButton.onClick.AddListener(() =>
        {
            Hide();
            onCloseButtonAction();
        });
        moveUpButton.onClick.AddListener(() => { RebindingKey(GameInput.Binding.Move_Up); });
        moveDownButton.onClick.AddListener(() => { RebindingKey(GameInput.Binding.Move_Down); });
        moveLeftButton.onClick.AddListener(() => { RebindingKey(GameInput.Binding.Move_Left); });
        moveRightButton.onClick.AddListener(() => { RebindingKey(GameInput.Binding.Move_Right); });
        interactButton.onClick.AddListener(() => { RebindingKey(GameInput.Binding.Interact); });
        interactAlternateButton.onClick.AddListener(() => { RebindingKey(GameInput.Binding.InteractAlternate); });
        pasueButton.onClick.AddListener(() => { RebindingKey(GameInput.Binding.Pause); });
    }
    private void ShowPressToRebindKey()
    {
        pressToRebindKey.gameObject.SetActive(true);
    }

    private void HidePressToRebindKey()
    {
        pressToRebindKey.gameObject.SetActive(false);
    }

    public void Start()
    {
        KitchenGameManager.Instance.OnUnpaused += KitchenGameManager_OnPaused;
        UpdateVisuals();
        Hide();
        HidePressToRebindKey();

    }

    private void KitchenGameManager_OnPaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UpdateVisuals()
    {
        soundEffectText.text = "Sound Effect :" + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music :" + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        moveUpText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Up);
        moveDownText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Down);
        moveLeftText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Left);
        moveRightText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Right);
        interactText.text = GameInput.instance.GetBindingText(GameInput.Binding.Interact);
        interactAlternateText.text = GameInput.instance.GetBindingText(GameInput.Binding.InteractAlternate);
        pauseText.text = GameInput.instance.GetBindingText(GameInput.Binding.Pause);
    }

    public void Show(Action onCloseButtonAction)
    {
        this.onCloseButtonAction = onCloseButtonAction;
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void RebindingKey(GameInput.Binding bindgingKey)
    {
        ShowPressToRebindKey();
        GameInput.instance.RebindingBinding(bindgingKey,() => {

            HidePressToRebindKey();
            UpdateVisuals();

        } );
    }
}
