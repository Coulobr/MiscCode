using System;
using System.Collections;
using UnityEngine;
using HitTrax.Pro;
using System.Collections.Generic;

#region ReSharper Comments
// ReSharper disable MergeSequentialChecks
#endregion

namespace HitTrax.Bataround
{
    public class BataroundPlaysList : ProList
    {
        //[Header("Button Sprite References")] public Sprite videoPreviewSprite;
        //public Sprite inactiveBookmarkSprite;
        //public Sprite activeBookmarkSprite;
        //public Sprite warningSprite;

        public User currentUser;
        public Stats currentSession;

        protected override IEnumerator BuildListCR()
        {
            //if (currentSession == null)
            //{
            //    Utility.LogError("The Plays List does not have a current session and cannot continue, cancelling...", GetType());

            //    if (refreshGO)
            //    {
            //        refreshGO.SetActive(false);
            //    }

            //    buildListCR = null;
            //    OnBuildListCompleted?.Invoke();
            //    yield break;
            //}

            var waitTime = new WaitForSeconds(0.05f);
            var obj = Objects.Instance;
            yield return null;
            var createdCount = 0;

            for (var i = currentSession.plays.Count - 1; i >= 0; --i)
            {
                var currentPlay = currentSession.plays[i];

                if (currentPlay == null)
                {
                    continue;
                }

                var newItem = ActivateProListItem(GetFreeProListItem());

                try
                {
                    newItem.play = currentPlay;

                    // Index
                    newItem.AddColumn($"{i + 1}", Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 65f, columnHeight);
                    
                    // Exit Velocity
                    newItem.AddColumn(obj.FormatVel(currentPlay.exitBallVel.magnitude, false), Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 250f, columnHeight);

                    // Launch Angle
                    newItem.AddColumn(Globals.FormatDegrees(currentPlay.elevation), Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 250f, columnHeight);

                    // Distance
                    newItem.AddColumn(obj.FormatDist(currentPlay.groundDist), Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 250f, columnHeight);

                    // Result
                    newItem.AddColumn(currentPlay.ResultStringShort(currentPlay.result), Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 250f, columnHeight);

                    // Bat Around Minigame
                    var minigame = currentPlay.BatAroundMinigame;
					switch (minigame)
					{
						case BatAroundMinigame.Linas:
                            newItem.AddColumn("Linas", Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 250f, columnHeight);
                            break;
						case BatAroundMinigame.AroundTheWorld:
                            newItem.AddColumn("Around the World", Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 250f, columnHeight);
                            break;
						case BatAroundMinigame.LaserShow:
                            newItem.AddColumn("Laser Show", Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 250f, columnHeight);
                            break;
						case BatAroundMinigame.WalkOff:
                            newItem.AddColumn("Walk Off", Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 250f, columnHeight);
                            break;
						case BatAroundMinigame.SmallBall:
                            newItem.AddColumn("Small Ball", Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 250f, columnHeight);
                            break;
						case BatAroundMinigame.BatAround:
                            newItem.AddColumn("Bat Around", Color.white, TMPro.TextAlignmentOptions.Center, null, null, null, 250f, columnHeight);
                            break;
					}

                   // newItem.OnToggled += OnItemToggled;

                    //var canAnalyze = currentPlay.BiomechanicsDataUsable;
                    //var analysisAccuracy = currentPlay.BiomechanicsAccuracy;

                    //if (currentPlay.BiomechanicsDownloaded && analysisAccuracy <= 75)
                    //{
                    //    // The warning icon
                    //    newItem.AddColumnButton((buttonData) =>
                    //    {
                    //        var popupTitle = canAnalyze ? "Data Potentially Inaccurate" : "Data Unusable";
                    //        var popupDescription = canAnalyze ? "The data analyzed for this play may potentially be inaccurate. Would you like to delete the biomechanics data for this play?" : "The data analyzed for this play has multiple issues. Would you like to delete the biomechanics data for this play?";
                    //        ProPopupUI.Open();
                    //        ProPopupUI.Instance.UpdateElement(popupTitle, popupDescription, false, true)
                    //            .UpdateTextOptions("Yes", "No")
                    //            .UpdateButtonOptions(() =>
                    //            {
                    //                // Delete the data and remove the reference to the hit ID from the play data
                    //                buttonData.play.DeleteBiomechanicsData();
                    //            }, () =>
                    //            {
                    //                // Do nothing, just close the popup
                    //            });
                    //    }, warningSprite, canAnalyze ? BiomechanicsColors.Orange : BiomechanicsColors.Red, null, Color.white, null, currentPlay, null, false, false, 30f, 30f);
                    //}


                    // The video preview button
                    // newItem.AddColumnButton((buttonData) => { buttonData.parent.SetSelectionState(!buttonData.parent.selected); }, videoPreviewSprite, Color.white, null, Color.white, null, currentPlay, null, false, false, 30f, 30f);

                    // The video upload button
                    //var uploadButton = newItem.AddColumnButton(null, null, Color.white, null, Color.white, currentUser, currentPlay, currentSession, false, false, 30f, 30f);
                    //var playAnalysis = uploadButton.smartButtonObject.gameObject.AddComponent<PlayAnalysisButton>();
                    //playAnalysis.SetupElement();
                    //playAnalysis.UpdateElement(currentPlay);
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

            if (refreshGO)
            {
                refreshGO.SetActive(false);
            }

            if (manuallyActivateGameObjects)
            {
                ActivateAllGameObjects();
            }

            buildListCR = null;
            OnBuildListCompleted?.Invoke();
        }
    }
}