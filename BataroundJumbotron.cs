using System.Collections;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class BataroundJumbotron : MonoBehaviour
	{

		public bool animate;
		private Transform homePlate;
		public Vector3 rightEnd;
		public Vector3 leftEnd;
		//public Vector3 rightEnd = new Vector3(-2018, 828, -3712);
		//public Vector3 leftEnd = new Vector3(2013, 828, -3753);
		private bool moveLeft;
		private bool moveRight;

		public void OnEnable()
		{
			if (animate)
			{
				homePlate = Objects.Instance.homePlate.transform;
				moveLeft = true;
				StartCoroutine(Co_JumboAnimate());
			}
		}

		private IEnumerator Co_JumboAnimate()
		{
			var wait = new WaitForEndOfFrame();
			var centerPivot = (rightEnd + leftEnd) * 0.35f;
			var centerOffset = Vector3.Distance(transform.position, homePlate.position);

			while (animate)
			{
				if (moveLeft)
				{
					var startRelativeCenter = rightEnd - centerPivot;
					var endRelativeCenter = leftEnd - centerPivot;

					var f = 1f / 1000;
					for (var i = 0f; i < 1; i += f)
					{
						transform.localPosition = Vector3.Slerp(startRelativeCenter, endRelativeCenter, i) + centerPivot;

						transform.LookAt(homePlate);
						var eulerAngles = transform.rotation.eulerAngles;
						eulerAngles.x = 0;
						eulerAngles.z = 0;
						transform.rotation = Quaternion.Euler(eulerAngles);

						yield return wait;
					}

					moveLeft = false;
					moveRight = true;
				}

				if (moveRight)
				{
					var startRelativeCenter = leftEnd - centerPivot;
					var endRelativeCenter = rightEnd - centerPivot;

					var f = 1f / 1000;
					for (var i = 0f; i < 1; i += f)
					{
						transform.localPosition = Vector3.Slerp(startRelativeCenter, endRelativeCenter, i) + centerPivot;

						transform.LookAt(homePlate);
						var eulerAngles = transform.rotation.eulerAngles;
						eulerAngles.x = 0;
						eulerAngles.z = 0;
						transform.rotation = Quaternion.Euler(eulerAngles);

						yield return wait;
					}

					moveRight = false;
					moveLeft = true;
				}
			}
		}
	}
}
