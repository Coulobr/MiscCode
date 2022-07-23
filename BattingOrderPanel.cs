using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class BattingOrderPanel : Panel<BattingOrderPanel>
	{
		public List<BattingOrderPlayerInfo> Players { get; set; } = new List<BattingOrderPlayerInfo>();
		public Transform teamALayout;
		public Transform teamBLayout;
		public TextMeshProUGUI teamAHeader;
		public TextMeshProUGUI teamBHeader;

		public BattingOrderPlayerInfo genericPrefab;
		public List<User> UserList { get; set; } = new List<User>();

		public void AddPlayer(User user)
		{
			if (Players == null)
			{
				Players = new List<BattingOrderPlayerInfo>();
			}

			BattingOrderPlayerInfo playerInfoObject = null;

			if (user.BataroundTeam == 0)
			{
				playerInfoObject = Instantiate(genericPrefab, teamALayout);
				playerInfoObject.User = user;
				playerInfoObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
			}
			else
			{
				playerInfoObject = Instantiate(genericPrefab, teamBLayout);
				playerInfoObject.User = user;
				playerInfoObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
			}

			if (BataroundGameManager.Instance.CurrentlyInLinas)
			{
				playerInfoObject.SetData(user);
			}
			if (BataroundGameManager.Instance.CurrentlyInAroundTheWorld)
			{
				playerInfoObject.SetData(user);
			}
			if (BataroundGameManager.Instance.CurrentlyInLaserShow)
			{
				playerInfoObject.SetData(user);
			}

			Players.Add(playerInfoObject);
		}

		public void LoadBatters(List<User> users)
		{
			if (BataroundGameManager.Instance.IsFreeForAll)
			{
				teamAHeader.text = "Batting Order";
				teamBHeader.text = "";
			}

			ClearBattingOrder();
			Players = null;
			foreach (User user in users)
			{
				AddPlayer(user);
			}
		}

		public void HighlightHitter(User user)
		{
			var group = BataroundGameManager.Instance.CurrentBataroundGroup;

			foreach (var playerInfo in Players)
			{
				if (playerInfo.User == user)
				{
					playerInfo.displayText.color = Color.red;
					playerInfo.displayText.fontStyle = TMPro.FontStyles.Bold;
				}
				else
				{
					playerInfo.displayText.color = Color.white;
					playerInfo.displayText.fontStyle = TMPro.FontStyles.Normal;
				}
			}
		}

		public void UpdateBattingOrder()
		{
			var group = BataroundGameManager.Instance.CurrentBataroundGroup;

			//foreach (var playerInfo in Players)
			//{
			//	foreach (Clinic team in group.Clinics)
			//	{
			//		foreach (User user in team.users)
			//		{
			//			if (user == playerInfo.User)
			//			{
			//				playerInfo.SetData(user.stats.BataroundGameScore);
			//			}
			//		}
			//	}
			//}
			//foreach (var batter in BataroundGameManager.Instance.CurrentBataroundGroup.GameBattingOrder)
			//{
			//	foreach (var uiPlayerInfo in Players)
			//	{
			//		if (batter == uiPlayerInfo.User)
			//		{
			//			uiPlayerInfo.SetData(batter);
			//		}
			//	}
			//}	
		}

		public void ClearBattingOrder()
		{
			foreach (var item in Players)
			{
				if (item != null)
					Destroy(item.gameObject);
			}
		}
	}
}

