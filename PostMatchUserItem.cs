using TMPro;
using UnityEngine;
using HitTrax.Pro;

namespace HitTrax.Bataround
{
	public class PostMatchUserItem : ProListItem
	{
		public TextMeshProUGUI orderNumber;

		public override void ExpandElement(float newHeight)
		{
			base.ExpandElement(newHeight);

			orderNumber.rectTransform.SetHeight(400f);
			orderNumber.alignment = TextAlignmentOptions.Top;

			// find and center plays list panel
			BataroundPlaysListPanel panel = null; 
			if (BataroundGameManager.Instance.IsFreeForAll)
			{
				if (FFAEndMinigameUI.Instance.isOpen)
				{
					panel = FFAEndMinigameUI.Instance.playsListPanel;
				}
				else if (FFAExperienceEndUI.Instance.isOpen)
				{
					panel = FFAExperienceEndUI.Instance.playsListPanel;
				}
			}
			else
			{
				if (TeamMinigameEndUI.Instance.isOpen)
				{
					panel = TeamMinigameEndUI.Instance.playsListPanel;
				}
				else if (TeamExperienceEndUI.Instance.isOpen)
				{
					panel = TeamExperienceEndUI.Instance.playsListPanel;
				}
			}

			if (panel != null)
			{               
				// Set position
				panel.transform.SetParent(transform, false);
				panel.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

				// Get all the user's plays from sessions
				panel.playsList.currentSession = user.stats;

				// Open and build the list
				panel.Open();
			}
		}

		public override void ShrinkElement()
		{
			base.ShrinkElement();

			orderNumber.rectTransform.SetHeight(50f);
			orderNumber.alignment = TextAlignmentOptions.Top;

			if (BataroundGameManager.Instance.IsFreeForAll)
			{
				if (FFAEndMinigameUI.Instance.isOpen)
				{
					FFAEndMinigameUI.Instance.playsListPanel.playsList.ResetElement();
					FFAEndMinigameUI.Instance.playsListPanel.Close();
				}
				else if (FFAExperienceEndUI.Instance.isOpen)
				{
					FFAExperienceEndUI.Instance.playsListPanel.playsList.ResetElement();
					FFAExperienceEndUI.Instance.playsListPanel.Close();
				}
			}
			else
			{
				if (TeamMinigameEndUI.Instance.isOpen)
				{
					TeamMinigameEndUI.Instance.playsListPanel.playsList.ResetElement();
					TeamMinigameEndUI.Instance.playsListPanel.Close();
				}
				else if (TeamExperienceEndUI.Instance.isOpen)
				{
					TeamExperienceEndUI.Instance.playsListPanel.playsList.ResetElement();
					TeamExperienceEndUI.Instance.playsListPanel.Close();
				}
			}
		}
	}
}