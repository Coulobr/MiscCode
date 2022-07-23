using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitTrax;
using System;
using UnityEngine.UI;
using DG.Tweening;

namespace HitTrax.Bataround
{
    public class BataroundLandingUI : Menu<BataroundLandingUI>
    {
        public SmartButton playBtn;
        public SmartButton backBtn;
        public GameObject animationContainer;
        public GameObject mainContainer;
        public Image titleGlow;
        public Image fadeImg;
        public bool firstOpen;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            firstOpen = true;

            if (playBtn)
            {
                playBtn.onClick.RemoveAllListeners();
                playBtn.onClick.AddListener(OnClickPlay);
            }
            
            if (backBtn)
            {
                backBtn.onClick.RemoveAllListeners();
                backBtn.onClick.AddListener(OnBackClick);
            }
        }

        protected override void OnOpened()
        {
            base.OnOpened();

			//if (firstOpen)
			//{
   //             animationContainer.SetActive(true);
   //             mainContainer.SetActive(false);

   //             StartCoroutine(Co_AnimationRoutine());
   //             firstOpen = false;
   //         }
        }

		private IEnumerator Co_AnimationRoutine()
		{
            titleGlow.rectTransform.DOScaleY(1.2f, 1.5f);
            yield return new WaitForSeconds(1.5f);
            titleGlow.rectTransform.DOScaleY(1f, 1.5f);
            yield return new WaitForSeconds(1.5f);
            titleGlow.rectTransform.DOScaleY(1.2f, 1.5f);
            yield return new WaitForSeconds(1.5f);
            titleGlow.rectTransform.DOScaleY(1f, 1.5f);
            yield return new WaitForSeconds(1.5f);
            titleGlow.rectTransform.DOScaleY(1.2f, 1.5f);
            yield return new WaitForSeconds(1.5f);

            fadeImg.DOFade(1, 1.5f);
            yield return new WaitForSeconds(1.5f);
            animationContainer.SetActive(false);
            mainContainer.SetActive(true);
            fadeImg.DOFade(0, 1.5f);
        }

        private void OnBackClick()
        {
            BataroundSessionsManager.Instance.UnloadCurrentSession();
            BataroundGameManager.Instance.CloseBatAround();
        }

        public void OnClickPlay()
        {
            BataroundSessionsManager.Instance.LoadSession<GameModeSelectSession>();
            Close();
        }
    }
}