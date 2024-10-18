using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keyMoveUpText;
    [SerializeField] private TextMeshProUGUI keyMoveDownText;
    [SerializeField] private TextMeshProUGUI keyMoveLeftText;
    [SerializeField] private TextMeshProUGUI keyMoveRightText;
    [SerializeField] private TextMeshProUGUI keyMoveInteractText;
    [SerializeField] private TextMeshProUGUI keyMoveInteractAlternateText;
    [SerializeField] private TextMeshProUGUI keyMovePauseText;

    private void Start()
    {
        GameInput.instance.OnBindingRebind += GameInput_OnBindingRebind;
        KitchenGameManager.Instance.OnStartChanged += KitchenGameManager_OnStartChanged;
        UpdateVisual();
        Show();
    }

    private void KitchenGameManager_OnStartChanged(object sender, System.EventArgs e)
    {
        if(KitchenGameManager.Instance.IsCountdownToStart())
        {
            Hide();
        }
    }

    private void GameInput_OnBindingRebind(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        keyMoveUpText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Up);
        keyMoveDownText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Down);
        keyMoveLeftText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Left);
        keyMoveRightText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Right);
        keyMoveInteractText.text = GameInput.instance.GetBindingText(GameInput.Binding.Interact);
        keyMoveInteractAlternateText.text = GameInput.instance.GetBindingText(GameInput.Binding.InteractAlternate);
        keyMovePauseText.text = GameInput.instance.GetBindingText(GameInput.Binding.Pause);
    }

    private void Show()
    {
        this.gameObject.SetActive(true);
    }
    private void Hide()
    {
        this.gameObject.SetActive(false);
    }

}
