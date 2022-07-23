using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitTrax;

namespace HitTrax.Bataround 
{
	public class BataroundOrderPanel : Panel<BataroundOrderPanel>
	{
		public ObjectPooler pooler;

		public override void SetupPanel()
		{
			base.SetupPanel();
			pooler.SetupElement();
		}
	}
}

