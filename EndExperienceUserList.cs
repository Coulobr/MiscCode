using HitTrax.Pro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class EndExperienceUserList : ProList
	{
		public List<User> sortedUserList;
	
		protected override void BuildListInstant()
		{
			base.BuildListInstant();
			var obj = Objects.Instance;

			sortedUserList = new List<User>(BataroundGameManager.Instance.CurrentBataroundGroup.GameBattingOrder);
			sortedUserList = sortedUserList.OrderByDescending(item => item.BAM.TotalGameScore).ToList();


			foreach (User user in sortedUserList)
			{
				var newItem = ActivateProListItem(GetFreeProListItem()) as PostMatchUserItem;

				for (int j = 0; j < BataroundGameManager.Instance.CurrentBataroundGroup.GameBattingOrder.Count; j++)
				{
					if (newItem.user == BataroundGameManager.Instance.CurrentBataroundGroup.GameBattingOrder[j])
					{
						newItem.orderNumber.text = (j + 1).ToString();
						break;
					}
				}

				//Button placeholder column
				if (BataroundGameManager.Instance.IsFreeForAll)
				{
					newItem.AddColumn(" ", Color.white, TextAlignmentOptions.Left, null, null, null, 60f, columnHeight);
				}

				newItem.AddColumn(user.screenName, Color.white, TextAlignmentOptions.Left, null, null, null, 200f, columnHeight);
				//newItem.AddColumn((1 + BataroundGameManager.Instance.CurrentBataroundGroup.GameBattingOrder.IndexOf(user)).ToString(), Color.white, TextAlignmentOptions.Center, null, null, null, 200, columnHeight);
				newItem.AddColumn(user.BAM.TotalLinasScore.ToString(), Color.white, TextAlignmentOptions.Center, null, null, null, 200f, columnHeight);
				newItem.AddColumn(user.BAM.TotalATWScore.ToString(), Color.white, TextAlignmentOptions.Center, null, null, null, 200f, columnHeight);
				newItem.AddColumn(user.BAM.TotalSmallBallScore.ToString(), Color.white, TextAlignmentOptions.Center, null, null, null, 200f, columnHeight);
				newItem.AddColumn(user.BAM.TotalLaserShowScore.ToString(), Color.white, TextAlignmentOptions.Center, null, null, null, 200f, columnHeight);
				newItem.AddColumn(user.BAM.TotalWalkOffScore.ToString(), Color.white, TextAlignmentOptions.Center, null, null, null, 200f, columnHeight);
				newItem.AddColumn(user.BAM.TotalBataroundMinigameScore.ToString(), Color.white, TextAlignmentOptions.Center, null, null, null, 200f, columnHeight);

				newItem.AddColumn(Globals.FormatVel((float)user.stats.avgExitVel, false, false), Color.white, TextAlignmentOptions.Center, null, null, null, 200f, columnHeight);
				newItem.AddColumn(Globals.FormatDistInt(user.stats.avgDistance, false, false), Color.white, TextAlignmentOptions.Center, null, null, null, 200f, columnHeight);
				newItem.AddColumn(Globals.FormatInt(user.stats.avgElevation), Color.white, TextAlignmentOptions.Center, null, null, null, 200f, columnHeight);

				var totalScore = user.BAM.TotalLinasScore
					+ user.BAM.TotalATWScore 
					+ user.BAM.TotalSmallBallScore
					+ user.BAM.TotalLaserShowScore
					+ user.BAM.TotalWalkOffScore
					+ user.BAM.TotalBataroundMinigameScore;
				newItem.AddColumn(totalScore.ToString(), new Color(1, 216 / 255f, 0), TextAlignmentOptions.Center, null, null, null, 200f, columnHeight);

				newItem.interactButton.GetComponent<Image>().color = user.BataroundTeam == 0 ? Globals.jerseyBlue : Globals.jerseyRed;
				newItem.user = user;
				newItem.OnToggled += OnItemToggled;
			}
		}

		public void RemoveAllColumnHeaders()
		{
			for (var i = columnHeaders.Count - 1; i >= 0; --i)
			{
				RemoveColumnHeader(columnHeaders[i]);
			}
		}

		protected override IEnumerator BuildListCR()
		{
			yield return null;
		}
	}
}

