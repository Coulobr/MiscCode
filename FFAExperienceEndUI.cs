using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class FFAExperienceEndUI : Menu<FFAExperienceEndUI>
	{
		public EndExperienceUserListPanel userListPanel;
		public BataroundPlaysListPanel playsListPanel;
		public TextMeshProUGUI title;
		public TextMeshProUGUI winnerText;
		public SmartButton continueBtn;
		public Sprite winnerIcon;
		public BataroundGameManager GameManager => BataroundGameManager.Instance;

		protected override void OnOpened()
		{
			base.OnOpened();

			continueBtn.onClick.RemoveAllListeners();
			continueBtn.onClick.AddListener(OnContinueClick);

			userListPanel.listGenerated = false;
			userListPanel.Open();

			User winner = BataroundGameManager.Instance.CurrentBataroundGroup.GameBattingOrder[0];
			List<User> ties = new List<User>();
			bool potentialTie = false;
			bool tie = false;

			foreach (User user in BataroundGameManager.Instance.CurrentBataroundGroup.GameBattingOrder)
			{
				if (user.BAM.TotalGameScore > winner.BAM.TotalGameScore)
				{
					winner = user;
				}
				else if (user.BAM.TotalGameScore == winner.BAM.TotalGameScore)
				{
					potentialTie = true;
				}
			}

			if (potentialTie)
			{
				foreach (User user in BataroundGameManager.Instance.CurrentBataroundGroup.GameBattingOrder)
				{
					if (user.BAM.TotalGameScore == winner.BAM.TotalGameScore)
					{
						ties.Add(user);
					}
				}
			}

			winnerText.text = tie ? "Tie!" : winner.screenName + " wins!";

			foreach (var item in userListPanel.userList.allProListItems)
			{
				if (item.user == winner)
				{
					var button = new ProColumnButtonData();
					button.icon = winnerIcon;

					item.AddColumnButton(button);
				}
			}
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			userListPanel.Close();
			BataroundGameManager.Instance.ResetExperience();
			Close();
		}

		private void OnContinueClick()
		{
			Close();
		}
	}
}

