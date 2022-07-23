using System;
using System.Collections;
using HitTrax.Pro.Biomechanics;
using UnityEngine;
using HitTrax.Pro;

namespace HitTrax.Bataround
{
    public class BataroundUserList : ProList
    {
        [Header("User List References")] public bool requireOnlineUserID;
        public bool showCloudSeatAssignments;

        // [HideInInspector] public bool showAgeColumn;
        private bool separateThreadCallbackFinished;
        private Action tempOnCompleted;

        private void Update()
        {
            if (separateThreadCallbackFinished)
            {
                OnCallbackCompleted();
            }
        }

        private void AddUserToLineUp(User user)
        {
            NumPlayersUI.Instance.AddPlayer(user);
            NumPlayersUI.Instance.userListPanel.Close();
        }

        public override void BuildList(Action onCompleted = null)
        {
            // We need to override the build list function here because the SQL.CheckPasswordResetAsync() happens in a separate thread
            // so we need to wait until it's finished and then execute the callback from the Update function.
            // Otherwise, this function will generally not be overwritten.

            if (asynchronous)
            {
                ResetElement();

				if (refreshGO)
				{
					refreshGO.SetActive(true);
				}

				if (onCompleted != null)
                {
                    tempOnCompleted += onCompleted;
                }

                if (requireOnlineUserID)
                {
					// Even though this function is automatically called in Sql.GetUsersAll() we still want to manually call this whenever the login panel
					// is refreshed so the user can see when an account has been verified
					var complete = Objects.Instance.platformAPI.GetFacilityVerifiedCloudUsers();
				}

                Objects.Instance.sql().CheckPasswordResetAsync(() =>
                {
                    // This callback is called inside of a separate thread (for some reason), so we can't perform the tasks we need unless we're in the main thread
                    separateThreadCallbackFinished = true;
                });
            }
        }

        protected override IEnumerator BuildListCR()
        {
            var waitTime = new WaitForSeconds(0.05f);
            var users = Objects.Instance.state.users;
            var createdCount = 0;

            for (var i = 0; i < users.Count; ++i)
            {
                if (!users[i].active || !users[i].isRegisteredUser)
                {
                    continue;
                }

                var newItem = ActivateProListItem(GetFreeProListItem()) as UserListItem;

                try
                {
                    newItem.user = users[i];

                    // User's Name
                    newItem.AddColumn($"{users[i].FullName}", Color.white, TMPro.TextAlignmentOptions.Left, null, null, null, 175, columnHeight);

                    var sport = users[i].GetSport();                  
                    // Skill Level
                    newItem.AddColumn($"{GameType.LevelName(sport, users[i].GetSkillLevel())}", Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 175, columnHeight);

                    // Handiness
                    string bats = newItem.user.bats == 1 ? "Right" : "Left";
                    if (newItem.user.bats == 0) {
                        bats = "UDF";
					}
                    newItem.AddColumn($"{bats}", Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 75, columnHeight);

                    // Add user button
                    newItem.AddColumnButton((buttonData) => { AddUserToLineUp(newItem.user); }, newItem.smartButtonTemplate.image.sprite, Color.white, null, Color.white, users[i], null, null, false, false, 50f, 50f);
                }
                catch (Exception e)
                {
                    DeactivateProListItem(newItem);
                    Utility.LogError(e, GetType());
                }

                if (createdCount >= batchCount)
                {
                    createdCount = 0;
                    yield return waitTime;
                }
                else
                {
                    createdCount++;
                }
            }

            if (manuallyActivateGameObjects)
            {
                ActivateAllGameObjects();
            }

            OnBuildListCompleted?.Invoke();
            buildListCR = null;
        }

        private void OnCallbackCompleted()
        {
            // This can only be active for a single frame, or this function will keep being called (due to a weird async issue)
            separateThreadCallbackFinished = false;
            base.BuildList(tempOnCompleted);
            tempOnCompleted = null;
        }
    }
}