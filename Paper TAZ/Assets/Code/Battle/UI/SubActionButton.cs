using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SubActionButton : MonoBehaviour, ISelectHandler
{
    public Image ActionImage;
    public TextMeshProUGUI ActionName;
    public TextMeshProUGUI ActionCost;
    public Button Button;

    private ActionSelectionPanel parentSelectionPanel;
    private string description;

    public void Populate(ActionSelectionPanel parentSelectionPanel, AttackData data)
    {
        Populate(parentSelectionPanel, data.Name, data.Cost.ToString(), data.Icon, data.Description);
    }

    public void Populate(ActionSelectionPanel parentSelectionPanel, string name, string cost, Sprite actionIcon, string description)
    {
        // Setup Button, Icon, Ability Points required, etc.
        this.parentSelectionPanel = parentSelectionPanel;
        ActionName.text = name;
        ActionCost.text = cost;
        ActionImage.overrideSprite = actionIcon;
        this.description = description;
    }

    public void SetSelectable(bool legalToSelect)
    {
        Button.interactable = legalToSelect;
    }

    public void OnSelect(BaseEventData eventData)
    {
        BattleUIManager.Instance.descriptionText.text = description;
        parentSelectionPanel.ActionSelected(this);
    }

    public void OnClick()
    {
        parentSelectionPanel.ChooseAction(this);
        // Report clicked back to BattleUIManager
        // TODO find a way to effectively check cost
    }
}
