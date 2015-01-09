using System;
using Android.OS;
using System.Collections.Generic;

namespace BTLibrary
{
	public class BTAckHandler:Handler
	{

		public override void HandleMessage (Message msg)
		{
			if (msg.What == (int) MessageType.MESSAGE_READ) {

				byte [] data = (byte []) msg.Obj;

				EnContentType type = (EnContentType) data [0];

				if (type == EnContentType.ACK) {
					List<byte> bs = new List<byte> ();

					for (int i = 1; i < data.GetLength (0); i++)
						bs.Add (data [i]);

					BTPlayService.Instance.ackRecieved (bs.ToArray ());
				} else {
					if (BTPlayService.Instance.isSlave ()) {
						List<byte> bs = new List<byte> ();

						bs.Add ((byte) EnContentType.ACK);

						for (int i = 0; i < data.GetLength (0); i++)
							bs.Add (data [i]);

						BTPlayService.Instance.WriteToMaster (bs.ToArray ());
					}
				}
			}


		}
	}
}

