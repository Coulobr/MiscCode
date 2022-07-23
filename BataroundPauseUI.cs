using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitTrax;

namespace HitTrax.Bataround 
{
	public class BataroundPauseUI : Menu<BataroundPauseUI> 
	{
		protected override void OnOpened()
		{
			base.OnOpened();
			Objects.Instance.natTrack().paused = true;
			Time.timeScale = 0;
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			Objects.Instance.natTrack().paused = false;
			Time.timeScale = 1;
		}
	}
}

