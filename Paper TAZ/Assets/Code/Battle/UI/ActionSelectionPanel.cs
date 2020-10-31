using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionSelectionPanel : MonoBehaviour
{

    public TextMeshProUGUI SelectionTitleLabel;
    public RectTransform ButtonContainter;
    public SubActionButton ActionButtonPrefab;


    private List<SubActionButton> ActionButtonPool = new List<SubActionButton>();

    private int buttonsInUse = 0;

    private void ResetSelections()
    {
        for(int i = 0; i < buttonsInUse; i++)
        {
            ActionButtonPool[i].gameObject.SetActive(false);
        }
    }

    public void LoadAttacks(Fighter fighter)
    {
        ResetSelections();
        for(int i = 0; i < fighter.FighterData.AttackList.Count; i++)
        {
            // Add buttons if more are needed
            if(ActionButtonPool.Count == i)
            {
                ActionButtonPool.Add(Instantiate(ActionButtonPrefab, ButtonContainter));
                ActionButtonPool[i].transform.SetAsLastSibling();
                // Set button nav
                if (i == 0)
                {
                    ActionButtonPool[0].Button.navigation = new Navigation()
                    {
                        mode = Navigation.Mode.None
                    };
                }
                else
                {
                    ActionButtonPool[i].Button.navigation = new Navigation()
                    {
                        mode = Navigation.Mode.Explicit,
                        selectOnUp = ActionButtonPool[i - 1].Button,
                        selectOnDown = ActionButtonPool[0].Button
                    };
                }
                // adjust previous button nav
                if (i > 0)
                {
                    ActionButtonPool[i].Button.navigation = new Navigation()
                    {
                        mode = Navigation.Mode.Explicit,
                        selectOnUp = i == 1 ? ActionButtonPool[i].Button : ActionButtonPool[i - 2].Button,
                        selectOnDown = ActionButtonPool[i].Button
                    };
                }
            }
            ActionButtonPool[i].Populate(this, fighter.FighterData.AttackList[i]);

            ActionButtonPool[i].SetSelectable(
                    BattleManager.Instance.PlayerTeam.AvailableActionPoints >= fighter.FighterData.AttackList[i].Cost &&
                    BattleTargetting.ValidTargetsFoundForAction(fighter, fighter.FighterData.AttackList[i].TargetData));
        }
    }

    public void SelectAction(int index)
    {
        ActionButtonPool[index].Button.Select();
    }

    public void ActionSelected(SubActionButton button)
    {
        BattleUIManager.Instance.SetActionSelectionIndex(ActionButtonPool.IndexOf(button));
    }

    public void ChooseAction(SubActionButton chosenButton)
    {
        BattleUIManager.Instance.BeginTargettingForAction(ActionButtonPool.IndexOf(chosenButton));
    }
}
