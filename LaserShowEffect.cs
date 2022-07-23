using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class LaserShowEffect : MonoBehaviour
	{

		public List<LineRenderer> LeftLasers;
		public List<LineRenderer> RightLasers;
		public bool IsActive { get; set; }
		public float Duratiuon { get; set; }

		private int rightIndex = 0;
		private int leftIndex = 0;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				StartLaserShow(.75f, 5);
			}

			if (IsActive)
			{
				Duratiuon -= Time.deltaTime;
				if (Duratiuon <= 0f)
				{
					StopAllCoroutines();
					DOTween.KillAll();
					IsActive = false;

					foreach (var laser in LeftLasers)
					{
						laser.SetPosition(1, Vector3.zero);
						laser.enabled = false;
					}

					foreach (var laser in RightLasers)
					{
						laser.SetPosition(1, Vector3.zero);
						laser.enabled = false;
					}
				}
			}
		}

		public void StartLaserShow(float moveDuration, float totalDuration)
		{
			Duratiuon = totalDuration;
			IsActive = true;
			StartCoroutine(Co_LaserShow(moveDuration, totalDuration));
		}
		private IEnumerator Co_LaserShow(float moveDuration, float totalDuration)
		{
			print("Moving lasers");
			while (IsActive)
			{
				foreach (var laser in LeftLasers)
				{
					laser.enabled = true;

					//leftLasers = null;
					//leftLasers = RandomLeftRotation(laser, moveDuration);
					StartCoroutine(RandomLeftRotation(laser, Random.Range(moveDuration - 0.25f, moveDuration + 0.25f)));
				}

				foreach (var laser in RightLasers)
				{
					laser.enabled = true;

					//rightLasers = null;
					//rightLasers = RandomLeftRotation(laser, moveDuration);
					StartCoroutine(RandomRightRotation(laser, Random.Range(moveDuration - 0.25f, moveDuration + 0.25f)));
				}

				yield return new WaitForSeconds(moveDuration);
			}
		}

		private IEnumerator RandomRightRotation(LineRenderer lineRenderer, float time)
		{
			Vector3 randVector = new Vector3(-35, Random.Range(-1, 20f), Random.Range(-20, 20f));

			var pos = lineRenderer.GetPosition(1);
			var ease = rightIndex == 1 ? Ease.OutSine : Ease.InSine;
			Tween seq = DOTween.To(() => pos, x => pos = x, randVector, time).SetEase(ease);

			while (!seq.IsComplete())
			{
				yield return new WaitForEndOfFrame();
				lineRenderer.SetPosition(1, pos);
			}

			if (rightIndex == 1)
			{
				rightIndex = 0;
			}
			else
			{
				rightIndex = 1;
			}

			//rightLasers = RandomRightRotation(lineRenderer, time);
			//StartCoroutine(rightLasers);

			yield break;
		}

		private IEnumerator RandomLeftRotation(LineRenderer lineRenderer, float time)
		{
			Vector3 randVector = new Vector3(35f, Random.Range(-1, 20f), Random.Range(-20, 20f));

			var pos = lineRenderer.GetPosition(1);
			var ease = leftIndex == 1 ? Ease.OutSine : Ease.InSine;
			Tween seq = DOTween.To(() => pos, x => pos = x, randVector, time).SetEase(ease);

			while (!seq.IsComplete())
			{
				yield return new WaitForEndOfFrame();
				lineRenderer.SetPosition(1, pos);
			}

			if (leftIndex == 1)
			{
				leftIndex = 0;
			}
			else
			{
				leftIndex = 1;
			}

			//leftLasers = RandomRightRotation(lineRenderer, time);
			//StartCoroutine(leftLasers);

			yield break;
		}

		private void LerpVectorValue(float num, float to, float time)
		{
			Mathf.Lerp(num, to, time);
			//yield return null;
		}
	}
}

