using System;
using TMPro;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class PlayerInfoCard : MonoBehaviour
	{
		public SmartButton deleteBtn;
		public TextMeshProUGUI playerName;
		public TMP_Dropdown playerDifficultyDropdown;
		public TMP_Dropdown handinessDropdown;
		[HideInInspector] public string userGuid;

		public BataroundMinigame.MinigameDifficulty Difficulty { get; set; }
		public int Handiness { get; set; }
		
		private void OnEnable()
		{
			if (deleteBtn)
			{
				deleteBtn.onClick.RemoveAllListeners();
				deleteBtn.onClick.AddListener(OnDeleteClick);
			}

			if (playerDifficultyDropdown)
			{
				playerDifficultyDropdown.onValueChanged.RemoveAllListeners();
				playerDifficultyDropdown.onValueChanged.AddListener(OnDifficultyChange);
			}

			if (handinessDropdown)
			{
				handinessDropdown.onValueChanged.RemoveAllListeners();
				handinessDropdown.onValueChanged.AddListener(OnHandinessChange);
			}
		}

		private void OnDifficultyChange(int val)
		{
			switch (val)
			{
				case 0:
					Difficulty = BataroundMinigame.MinigameDifficulty.Easy;
					break;
				case 1:
					Difficulty = BataroundMinigame.MinigameDifficulty.Medium;
					break;
				case 2:
					Difficulty = BataroundMinigame.MinigameDifficulty.Hard;
					break;
			}
		}

		private void OnHandinessChange(int val)
		{
			Handiness = val;
		}

		private void OnDeleteClick()
		{
			NumPlayersUI.Instance.RemovePlayer(this);
			Destroy(gameObject);
		}
	}
}

