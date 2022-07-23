using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class TeamSelectionCard : MonoBehaviour
	{
		public Image background;
		public Image playerNameBackground;
		public TextMeshProUGUI playerName;
		public SmartButton leftArrow;
		public SmartButton rightArrow;

		public User User { get; set; }
		public bool IsTeamA { get; set; }
		public bool IsTeamB { get; set; }
		public bool IsPlayerList { get; set; }

		public Sprite whiteArrow;
		public Sprite redArrow;
		public Sprite blueArrow;
		public Sprite nameBackgroundOneSide;
		public Sprite nameBackgroundTwoSide;

		public Color redTeamColor;
		public Color blueTeamColor;

		private void OnEnable()
		{
			leftArrow.onClick.AddListener(OnLeftClick);
			rightArrow.onClick.AddListener(OnRightClick);

			IsPlayerList = true;
		}

		private void OnRightClick()
		{
			if (IsPlayerList)
			{
				transform.SetParent(BataroundTeamSelectionUI.Instance.teamBlayout);
				IsTeamB = true;
				IsPlayerList = false;

				rightArrow.interactable = false;
				leftArrow.interactable = true;

				rightArrow.gameObject.SetActive(false);
				leftArrow.gameObject.SetActive(true);

				leftArrow.image.sprite = whiteArrow;
				leftArrow.transform.rotation = Quaternion.Euler(0, 0, 180);

				playerNameBackground.sprite = nameBackgroundOneSide;
				playerNameBackground.transform.rotation = Quaternion.Euler(0, 0, 180);

				background.color = redTeamColor;
				playerName.alignment = TextAlignmentOptions.Right;
			}
			else if (IsTeamA)
			{
				transform.SetParent(BataroundTeamSelectionUI.Instance.playerLayout);
				IsPlayerList = true;
				IsTeamB = false;

				rightArrow.interactable = true;
				leftArrow.interactable = true;

				rightArrow.gameObject.SetActive(true);
				leftArrow.gameObject.SetActive(true);

				rightArrow.image.sprite = redArrow;
				leftArrow.transform.rotation = Quaternion.Euler(0, 0, 0);
				leftArrow.image.sprite = blueArrow;
				leftArrow.transform.rotation = Quaternion.Euler(0, 0, 180);

				playerNameBackground.sprite = nameBackgroundTwoSide;
				playerNameBackground.transform.rotation = Quaternion.Euler(0, 0, 0);

				background.color = Color.white;
				playerName.alignment = TextAlignmentOptions.Center;
			}
		}

		private void OnLeftClick()
		{
			if (IsPlayerList)
			{
				transform.SetParent(BataroundTeamSelectionUI.Instance.teamAlayout);
				IsTeamA = true;
				IsPlayerList = false;

				leftArrow.interactable = false;
				rightArrow.interactable = true;

				rightArrow.gameObject.SetActive(true);
				leftArrow.gameObject.SetActive(false);

				rightArrow.image.sprite = whiteArrow;
				rightArrow.transform.rotation = Quaternion.Euler(0, 0, 0);

				playerNameBackground.sprite = nameBackgroundOneSide;
				playerNameBackground.transform.rotation = Quaternion.Euler(0, 0, 0);

				background.color = blueTeamColor;
				playerName.alignment = TextAlignmentOptions.Left;
			}
			else if (IsTeamB)
			{
				transform.SetParent(BataroundTeamSelectionUI.Instance.playerLayout);
				IsPlayerList = true;
				IsTeamB = false;

				rightArrow.interactable = true;
				leftArrow.interactable = true;

				rightArrow.gameObject.SetActive(true);
				leftArrow.gameObject.SetActive(true);

				rightArrow.image.sprite = redArrow;
				leftArrow.transform.rotation = Quaternion.Euler(0, 0, 0);
				leftArrow.image.sprite = blueArrow;
				leftArrow.transform.rotation = Quaternion.Euler(0, 0, 180);

				playerNameBackground.sprite = nameBackgroundTwoSide;
				playerNameBackground.transform.rotation = Quaternion.Euler(0, 0, 0);

				background.color = Color.white;
				playerName.alignment = TextAlignmentOptions.Center;
			}
		}

		public void SetData(User user)
		{
			User = user;
			playerName.text = user.screenName;
		}
	}
}
