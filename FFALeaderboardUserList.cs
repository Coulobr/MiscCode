using HitTrax.Pro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class FFALeaderboardUserList : ProList
	{
		public List<User> sortedUserList;

		public override void ResetElement()
		{
			base.ResetElement();
		}

		protected override void BuildListInstant()
		{
			base.BuildListInstant();
			var obj = Objects.Instance;
			var gameManager = BataroundGameManager.Instance;

			sortedUserList = new List<User>(gameManager.CurrentBataroundGroup.GameBattingOrder);
			if (gameManager.CurrentlyInLinas)
			{
				sortedUserList = sortedUserList.OrderByDescending(item => item.BAM.TotalLinasScore).ToList();
			}
			if (gameManager.CurrentlyInAroundTheWorld)
			{
				sortedUserList = sortedUserList.OrderByDescending(item => item.BAM.AroundTheWorldBAM).ToList();
			}
			if (gameManager.CurrentlyInLaserShow)
			{
				sortedUserList = sortedUserList.OrderByDescending(item => item.BAM.TotalLaserShowScore).ToList();
			}
			if (gameManager.CurrentlyInSmallBall)
			{
				sortedUserList = sortedUserList.OrderByDescending(item => item.BAM.TotalSmallBallScore).ToList();
			}
			if (gameManager.CurrentlyInWalkOff)
			{
				sortedUserList = sortedUserList.OrderByDescending(item => item.BAM.TotalWalkOffScore).ToList();
			}
			if (gameManager.CurrentlyInBataround)
			{
				sortedUserList = sortedUserList.OrderByDescending(item => (item.BAM.TotalBataroundRoundsWon + item.BAM.TotalBataroundMinigameScore)).ToList();
			}

			for (int i = 0; i < sortedUserList.Count; i++)
			{
				var newItem = ActivateProListItem(GetFreeProListItem());
				var user = sortedUserList[i];

				newItem.AddColumn(user.screenName, Color.white, TextAlignmentOptions.Left, null, null, null, 100, columnHeight);
				newItem.AddColumn((1 + gameManager.CurrentBataroundGroup.GameBattingOrder.IndexOf(user)).ToString(), Color.white, TextAlignmentOptions.Center, null, null, null, 60, columnHeight);

				var currentScore = 0;
				if (gameManager.CurrentlyInLinas)
				{
					currentScore = user.BAM.TotalLinasScore;
				}
				if (gameManager.CurrentlyInAroundTheWorld)
				{
					currentScore = user.BAM.TotalATWScore;
				}
				if (gameManager.CurrentlyInLaserShow)
				{
					currentScore = user.BAM.TotalLaserShowScore;
				}
				if (gameManager.CurrentlyInSmallBall)
				{
					currentScore = user.BAM.TotalSmallBallScore;
				}
				if (gameManager.CurrentlyInWalkOff)
				{
					currentScore = user.BAM.TotalWalkOffScore;
				}
				if (gameManager.CurrentlyInBataround)
				{
					currentScore = user.BAM.TotalBataroundRoundsWon + user.BAM.TotalBataroundMinigameScore;
				}
				newItem.AddColumn(currentScore.ToString(), Color.white, TextAlignmentOptions.Center, null, null, null, 60, columnHeight);

				var totalScore = user.BAM.TotalLinasScore
					+ user.BAM.TotalATWScore
					+ user.BAM.TotalLaserShowScore
					+ user.BAM.TotalLinasScore
					+ user.BAM.TotalWalkOffScore
					+ user.BAM.TotalSmallBallScore;
				newItem.AddColumn(totalScore.ToString(), new Color(1, 216 / 255f, 0), TextAlignmentOptions.Center, null, null, null, 60, columnHeight);
			}
		}

		/// <summary>
		/// Refresh after every hit
		/// </summary>
		public override void RefreshElement()
		{
			base.RefreshElement();
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
