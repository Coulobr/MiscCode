using TMPro;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class BattingOrderPlayerInfo : MonoBehaviour
	{

		public TextMeshProUGUI displayText;
		public User User { get; set; }

		public void SetData(User user)
		{
			if (BataroundGameManager.Instance.CurrentlyInLinas)
			{
				displayText.text = $"{user.displayName} | Score: {user.BAM.TotalLinasScore} | Bonus: {user.BAM.TotalBataroundBonusPoints}/4 | Total EXP: {user.BAM.TotalBataroundSessionEXP}";
			}
			if (BataroundGameManager.Instance.CurrentlyInAroundTheWorld)
			{
				displayText.text = $"{user.displayName} | Score: {user.BAM.TotalATWScore} | Bonus: {user.BAM.TotalBataroundBonusPoints}/4 | Total EXP: {user.BAM.TotalBataroundSessionEXP}";
			}
			if (BataroundGameManager.Instance.CurrentlyInLaserShow)
			{
				displayText.text = $"{user.displayName} | Score: {user.BAM.TotalLaserShowScore} | Bonus: {user.BAM.TotalBataroundBonusPoints}/4 | Total EXP: {user.BAM.TotalBataroundSessionEXP}";
			}
		}
	}

}
