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

					bs.Add (0);

					BTPlayService.Instance.ackRecieved (bs.ToArray ());
				} else if (type != EnContentType.NONE) {

					byte [] bs2 = new byte[1024];

					for (int i = 0; i < 1023; i++)
						if (i < data.GetLength (0))
							bs2 [i + 1] = data [i];
						else
							bs2 [i + 1] = 0;

					bs2 [0] = (byte) EnContentType.ACK;

					if (BTPlayService.Instance.isSlave ())
						BTPlayService.Instance.WriteToMaster (bs2);
					else
						BTPlayService.Instance.WriteToAllSlave (bs2);
				}
			}


		}
	}
}

