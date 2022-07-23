using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class BataroundAttemptPoolable : PoolableObject
	{
		public Image ballImg;
		public override void ClearElement()
		{
			base.ClearElement();
			// ballImg.enabled = true;
		}
	}
}

