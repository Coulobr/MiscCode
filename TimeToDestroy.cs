using UnityEngine;

namespace HitTrax.Bataround
{
	public class TimeToDestroy : MonoBehaviour
	{
		public float timeToDestroy;
		private float timer = 0f;

		void Update()
		{
			timer += Time.deltaTime;
			if (timer > timeToDestroy)
			{
				Destroy(gameObject);
			}
		}
	}
}


