using System;

namespace HitTrax.Bataround
{
	public class BataroundInstance
	{
		public Id id;
		public int syncState;
		public DateTime created;
		public bool dbSync;

		public int instanceID;
		public bool hasSubscription;
		public DateTime startDate;
		public DateTime endDate;
		public int playersLoggedIn;
		public string playerNames;
		public string playerGUIDs;

		public BataroundInstance()
		{
			id = new Id();
			instanceID = id.id;
			created = DateTime.Now;
			startDate = DateTime.Now;
			endDate = DateTime.Now;
			dbSync = false;
		}
	}
}