using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Util;
using ChiamataLibrary;
using System;
using System.Text;
using Android.Util;
using System.Threading;


namespace BTLibrary
{
	/// <summary>
	/// Wrapper for all the bluetooth features (singleton implementation).
	/// </summary>
	public class BTManager: Handler
	{
		#region singleton implementation

		/// <summary>
		/// Instance of the <see cref="BTLibrary.BTManager"/> singleton
		/// </summary>
		private static readonly BTManager _instance = new BTManager ();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance of the <see cref="BTLibrary.BTManager"/>.</value>
		public static BTManager Instance{ get { return _instance; } }

		private BTManager ()
		{
		}

		#endregion

		#region BlueToothAdapter Management

		/// <summary>
		/// The local BlueTooth adapter.
		/// </summary>
		private static readonly BluetoothAdapter _btAdapter = BluetoothAdapter.DefaultAdapter;

		/// <summary>
		/// Indicate if the bluetooth exists.
		/// </summary>
		/// <returns><c>true</c>, if bluetooth exists, <c>false</c> otherwise.</returns>
		public bool existBluetooth ()
		{
			return _btAdapter != null;
		}

		/// <summary>
		/// Gets the local BlueTooth adapter.
		/// </summary>
		/// <returns>The BT adapter.</returns>
		internal BluetoothAdapter getBTAdapter ()
		{
			return _btAdapter;
		}

		/// <summary>
		/// Indicate if BTadapter is discovering new devices
		/// </summary>
		/// <returns><c>true</c>, if is discovering, <c>false</c> otherwise.</returns>
		public bool isDiscovering ()
		{
			return _btAdapter.IsDiscovering;
		}

		/// <summary>
		/// Indicate if BT is enabled.
		/// </summary>
		/// <returns><c>true</c>, if BT is enable <c>false</c> otherwise.</returns>
		public bool isBTEnabled ()
		{
			return _btAdapter.IsEnabled;
		}

		/// <summary>
		/// Gets the local BlueTooth name.
		/// </summary>
		/// <returns>The local BlueTooth name.</returns>
		public string GetLocalName ()
		{
			return _btAdapter.Name;
		}

		/// <summary>
		/// Gets the local BlueTooth address.
		/// </summary>
		/// <returns>The local BlueTooth address.</returns>
		internal string GetLocalAddress ()
		{
			return _btAdapter.Address;
		}

		/// <summary>
		/// Gets the remote device.
		/// </summary>
		/// <returns>The remote device.</returns>
		/// <param name="address">Remote device address.</param>
		public BluetoothDevice getRemoteDevice (string address)
		{
			if (address == null)
				return null;
			return _btAdapter.GetRemoteDevice (address);
		}

		#endregion

		#region Receiver Management

		/// <summary>
		/// The activity to register the receiver and display messages.
		/// </summary>
		private Activity _activity;

		/// <summary>
		/// The receiver to register intents.
		/// </summary>
		private BTReceiver _receiver;

		/// <summary>
		/// Registers the receiver for device discovery and the finishing of device discovery.
		/// </summary>
		public void RegisterReceiver ()
		{
			// Register for broadcasts when a device is discovered
			_receiver = new BTReceiver ();
			var filter = new IntentFilter (BluetoothDevice.ActionFound);
			_activity.ApplicationContext.RegisterReceiver (_receiver, filter);

			// Register for broadcasts when discovery has finished
			filter = new IntentFilter (BluetoothAdapter.ActionDiscoveryFinished);
			_activity.ApplicationContext.RegisterReceiver (_receiver, filter);

			filter = new IntentFilter (BluetoothDevice.ActionBondStateChanged);
			_activity.ApplicationContext.RegisterReceiver (_receiver, filter);
		}

		public void RegisterReceiverPairing ()
		{
			_receiver = new BTReceiver ();
			IntentFilter filter = new IntentFilter (BluetoothDevice.ActionPairingRequest);
			_activity.ApplicationContext.RegisterReceiver (_receiver, filter);

		}

		#endregion

		#region BlueTooth visibility and enable

		/// <summary>
		/// Makes the Bluetooth visible.
		/// </summary>
		/// <param name="amount">amount of time to make the bluetooth visible.</param>
		internal void makeVisible (int amount)
		{
			//creates an Intent with action ActionRequestDiscoverable
			Intent visibleIntent = new Intent (BluetoothAdapter.ActionRequestDiscoverable);
			//insert in the intent the duration of visibility
			visibleIntent.PutExtra (BluetoothAdapter.ExtraDiscoverableDuration, amount);
			//Start the VisibilityRequest activity
			_activity.StartActivityForResult (visibleIntent, (int) EnActivityResultCode.VISIBILITY_REQUEST);
		}


		/// <summary>
		/// Enables the bluetooth.
		/// </summary>
		public void enableBluetooth ()
		{
			//if bluetooth is already enabled returns
			if (_btAdapter.IsEnabled)
				return;
			//else creates an Intent with action ActionRequestEnable
			Intent enableIntent = new Intent (BluetoothAdapter.ActionRequestEnable);
			//Starts the EnableRequest activity
			_activity.StartActivityForResult (enableIntent, (int) EnActivityResultCode.REQUEST_ENABLE_BT);
		}

		#endregion

		#region scanning and pairing

		/// <summary>
		/// The list of paired device addresses.
		/// </summary>
		private readonly List<string> _pairedDevicesList = new List<string> ();

		/// <summary>
		/// Gets a list of paired devices address.
		/// </summary>
		/// <returns>The paired devices address.</returns>
		public List<string> GetPaired ()
		{
			// Get a set of currently paired devices
			var pairedDevices = _btAdapter.BondedDevices;
			_pairedDevicesList.Clear ();
			// If there are paired devices, add each address to the List
			if (pairedDevices.Count > 0)
				foreach (var device in pairedDevices) {
					_pairedDevicesList.Add (( (BluetoothDevice) device ).Address);
				}
			else
				_pairedDevicesList.Add ("No Device Paired");

			return _pairedDevicesList;
		}

		/// <summary>
		/// Performs discovery of a new device
		/// </summary>
		public void Discovery ()
		{
			// If we're already discovering, stop it
			if (_btAdapter.IsDiscovering) {
				CancelDiscovery ();
			}
			// Request discover from BluetoothAdapter
			_btAdapter.StartDiscovery ();

		}

		/// <summary>
		/// Cancel the discovery activity
		/// </summary>
		public void CancelDiscovery ()
		{
			_btAdapter.CancelDiscovery ();
		}

		#endregion

		#region Connection Management

		/// <summary>
		/// The listen thread to accept incoming connections.
		/// </summary>
		private BTListenThread _listenThread;

		/// <summary>
		/// The connect thread to connect to a device.
		/// </summary>
		private BTConnectThread _connectThread;

		/// <summary>
		/// The list of thread to perform master read with mutiple connected slave or to perform slave read from the master.
		/// </summary>
		private readonly List<BTReadThread> _readThread = new List<BTReadThread> (MAX_BT_PLAYER);

		/// <summary>
		/// The list of thread to perform master write with multiple connected slave or to perform slave read the the master.
		/// </summary>
		private readonly List<BTWriteThread> _writeThread = new List<BTWriteThread> (MAX_BT_PLAYER);

		/// <summary>
		/// The UUID to perform connection.
		/// </summary>
		private static readonly UUID MY_UUID = UUID.FromString ("fa87c0d0-afac-11de-8a39-0800200c9a66");

		/// <summary>
		/// The name to perform connection.
		/// </summary>
		internal const string NAME = "PlayService";

		/// <summary>
		/// The state of device.
		/// </summary>
		private EnConnectionState _state;

		/// <summary>
		/// Sets the state.
		/// </summary>
		/// <param name="state">State.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		private void SetState (EnConnectionState state)
		{
			//sets the state
			_state = state;

		}

		/// <summary>
		/// Gets the state.
		/// </summary>
		/// <returns>The state.</returns>
		[MethodImpl (MethodImplOptions.Synchronized)]
		internal EnConnectionState GetState ()
		{
			return _state;
		}

		/// <summary>
		/// Performs connection as master.
		/// </summary>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void ConnectAsMaster ()
		{	
			//stops listening thread
			StopListen ();

			// Start the thread to listen on a BluetoothServerSocket
			_listenThread = new BTListenThread (NAME, MY_UUID);

			//sets the state on STATE_LISTEN and send message to indicate this change
			SetState (EnConnectionState.STATE_LISTEN);
			this.ObtainMessage ((int) EnLocalMessageType.MESSAGE_STATE_CHANGE, (int) EnConnectionState.STATE_LISTEN, -1).SendToTarget ();
		}

		/// <summary>
		/// Performs connection as slave.
		/// </summary>
		/// <param name="device">Device to connect.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void ConnectAsSlave (BluetoothDevice device)
		{	
			//stops all existing thread
			Stop ();

			//sets the state to CONNECTING and send message to indicate this change
			SetState (EnConnectionState.STATE_CONNECTING);
			this.ObtainMessage ((int) EnLocalMessageType.MESSAGE_STATE_CHANGE, (int) EnConnectionState.STATE_CONNECTING, -1).SendToTarget ();

			// Start the thread to connect with the given device
			_connectThread = new BTConnectThread (device, MY_UUID);
		}

		/// <summary>
		/// Connects to Master.
		/// </summary>
		/// <param name="socket">Bluetooth socket.</param>
		/// <param name="device">Bluetooth device.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		internal void ConnectedToMaster (BluetoothSocket socket, BluetoothDevice device)
		{
			//sets the state to CONNECTED_SLAVE
			SetState (EnConnectionState.STATE_CONNECTED_SLAVE);

			// Start the thread to perform read service
			_readThread.Add (new BTReadThread (socket));

			// Start the thread to perform write service
			_writeThread.Add (new BTWriteThread (socket));

			//sends a message to the indicate the slave connection to a device
			this.ObtainMessage ((int) EnLocalMessageType.MESSAGE_DEVICE_ADDR, (int) EnConnectionState.STATE_CONNECTED_SLAVE, -1, device.Address).SendToTarget ();

		}

		/// <summary>
		/// Indicates if the current device is a slave in the network.
		/// </summary>
		/// <returns><c>true</c>, if is client <c>false</c> otherwise.</returns>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public bool isSlave ()
		{
			//if state is Connected_Client the caller is a client
			return _state == EnConnectionState.STATE_CONNECTED_SLAVE;

		}

		/// <summary>
		/// Connects to slave.
		/// </summary>
		/// <param name="socket">Bluetooth socket.</param>
		/// <param name="device">Bluetooth device.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		internal void ConnectedToSlave (BluetoothSocket socket, BluetoothDevice device)
		{	
			//sets the state to CONNECTED_MASTER
			SetState (EnConnectionState.STATE_CONNECTED_MASTER);

			// Start the thread to perform read service
			_readThread.Add (new BTReadThread (socket));

			// Start the thread to perform write service
			_writeThread.Add (new BTWriteThread (socket));

			//sends a message to the indicate the master connection to a device
			this.ObtainMessage ((int) EnLocalMessageType.MESSAGE_DEVICE_ADDR, (int) EnConnectionState.STATE_CONNECTED_MASTER, -1, device.Address).SendToTarget ();

			//stop listen thread 
			StopListen ();
		}

		/// <summary>
		/// Gets the number of devices connected.
		/// </summary>
		/// <returns>The number of devices connected.</returns>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public int getNumConnected ()
		{
			return _readThread.Count;
		}

		/// <summary>
		/// Stop all threads.
		/// </summary>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void Stop ()
		{
			if (_connectThread != null) {
				_connectThread.Cancel ();
				_connectThread = null;
			}

			for (int i = 0; i < _writeThread.Count; i++) {
				_writeThread [i].Cancel ();
				_writeThread.RemoveAt (i);
			}

			for (int i = 0; i < _readThread.Count; i++) {
				_readThread [i].Cancel ();
				_readThread.RemoveAt (i);
			}

			if (_listenThread != null) {
				_listenThread.Cancel ();
				_listenThread = null;
			}
		}

		/// <summary>
		/// Stops the listen Thread.
		/// </summary>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void StopListen ()
		{
			if (_listenThread != null) {
				_listenThread.Cancel ();
				_listenThread = null;
				_listenThread = null;
			}
		}

		/// <summary>
		/// Removes a slave.
		/// </summary>
		/// <param name="address">Address of the slave to remove.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void RemoveSlave (string address)
		{
			for (int i = 0; i < _readThread.Count; ++i)
				if (_readThread [i]._socket.RemoteDevice.Address == address) {
					_readThread [i].Cancel ();
					_readThread.RemoveAt (i);
					break;
				}
			for (int i = 0; i < _writeThread.Count; ++i) {
				if (_writeThread [i]._Socket.RemoteDevice.Address == address) {
					_writeThread [i].Cancel ();
					_writeThread.RemoveAt (i);
					break;
				}
			}
		}

		/// <summary>
		/// Removes the master.
		/// </summary>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void RemoveMaster ()
		{
			_readThread [0].Cancel ();
			_readThread.Clear ();

			_writeThread [0].Cancel ();
			_writeThread.Clear ();
		
		}

		/// <summary>
		/// Indicate that the connection attempt failed and notify the Activity.
		/// </summary>
		internal void ConnectionFailed ()
		{	
			//stops all existing thread
			Stop ();

		}

		#endregion

		#region Application Management

		/// <summary>
		/// Sets the activity.
		/// </summary>
		/// <param name="activity">Activity.</param>
		public void setActivity (Activity activity)
		{
			_activity = activity;
		}

		#endregion

		#region initialization

		/// <summary>
		/// The maximum number of bluetooth slots avaiable.
		/// </summary>
		public const int MAX_BT_PLAYER = 4;

		/// <summary>
		/// Initialize the specified the Play Service.
		/// </summary>
		/// <param name="activity">Activity.</param>
		public void Initialize (Activity activity)
		{
			//activity to register the receiver
			_activity = activity;

			//sets the state to STATE_NONE
			_state = EnConnectionState.STATE_NONE;

			Stop ();
		}

		#endregion

		#region Handler

		/// <summary>
		/// Delegate for the event that occours when a local message is recieved.
		/// </summary>
		public delegate void eventHandlerLocalMessageReceived (Message msg);

		/// <summary>
		/// Occurs when a local message is received.
		/// </summary>
		public event eventHandlerLocalMessageReceived eventLocalMessageReceived;

		/// <summary>
		/// Delegate or the event that occours when a playtime message is received.
		/// </summary>
		public delegate void eventHandlerPackageRecieved (PackageBase pkg);

		/// <summary>
		/// Occurs when a playtime message is received.
		/// </summary>
		public event eventHandlerPackageRecieved eventPackageReceived;

		/// <summary>
		/// Handles the messages.
		/// </summary>
		public override void HandleMessage (Message msg)
		{
			if (msg.What == (int) EnLocalMessageType.MESSAGE_READ) {	//package message(remote)
				byte [] data = (byte []) msg.Obj;

				//Get the type of the message from the firs byte of the message
				EnPackageType type = (EnPackageType) data [0];

				if (type == EnPackageType.ACK) {
						
					if (isSlave ()) {
						//if i am a slave i remove the message from the list of the message to send
						_writeThread [0].Remove (PackageBase.getMessageFromAck (data));
					} else {
						//otherwise i am the master so i remove the message only from the list of the sender
						_writeThread.ForEach (delegate(BTWriteThread thred) {
							if (thred.Connected == PackageBase.getAddressFromAck (data))
								thred.Remove (PackageBase.getMessageFromAck (data));
						});

					}
						

				} else if (type != EnPackageType.NONE) {

					PackageBase pkg = PackageBase.createPackage (data);	


					if (eventPackageReceived != null)
						eventPackageReceived (pkg);
						
					//ACK consists of the type ACK followed by the message received 
					if (BTManager.Instance.isSlave ())
						_writeThread [0].Add (pkg.getAckMessage ());
					else
						for (int i = 0; i < _writeThread.Count; i++)
							if (_writeThread [i] != null)
								_writeThread [i].Add (pkg.getAckMessage ());
						
				}
				// all other messages that are not message_read (STATE_CHANGE, DEVICE_ADDRESS ecc) are initializing events
			} else if (eventLocalMessageReceived != null)	//local message
				eventLocalMessageReceived (msg);

		}

		#endregion

		#region Communication Management

		/// <summary>
		/// Writes to master.
		/// </summary>
		/// <param name="pkg">Package to send to master.</param>
		public void WriteToMaster (PackageBase pkg)
		{
			if (_writeThread.Count == 0)
				return;

			_writeThread [0].Add (pkg.getMessage ());

		}

		/// <summary>
		/// Writes to all slaves.
		/// </summary>
		/// <param name="pkg">Pakage to send to all slaves.</param>
		public void WriteToAllSlave (PackageBase pkg)
		{
			byte [] message = pkg.getMessage ();
			for (int i = 0; i < _writeThread.Count; i++) {
				if (_writeThread [i] != null)
					_writeThread [i].Add (message);
			}
		}

		/// <summary>
		/// Initializes the communication.
		/// </summary>
		public void initializeCommunication ()
		{
			Board.Instance.eventIPlaceABid += bidPlaced;
			if (!isSlave ())
				Board.Instance.eventSomeonePlaceABid += bidPlaced;
			Board.Instance.eventPlaytimeStart += semeChosen;
			Board.Instance.eventIPlayACard += cardPlayed;
			if (!isSlave ())
				Board.Instance.eventSomeonePlayACard += cardPlayed;
		}

		/// <summary>
		/// Sends the BID message to the master (if i'm slave) or to all slaves (if i'm master).
		/// </summary>
		/// <remarks>See <see cref="BTLibrary.PackageBid"/> for further informations about the message contents.</remarks>
		/// <param name="bid">The bid to send.</param>
		private void bidPlaced (BidBase bid)
		{
			//When the Board event eventIPlaceABid or eventSomeonePlaceABid occours, write to master (if i'm slave) or to all slave (if i'm master) the message.

			if (BTManager.Instance.isSlave ())
				BTManager.Instance.WriteToMaster (new PackageBid (bid));
			else
				BTManager.Instance.WriteToAllSlave (new PackageBid (bid));
		}

		/// <summary>
		/// Sends the SEME message to the master (if i'm slave and caller) or to all slaves (if i'm master).
		/// </summary>
		/// <remarks>See <see cref="BTLibrary.PackageSeme"/> for further informations about the message contents.</remarks>
		private void semeChosen ()
		{
			// When the Board event eventPlaytimeStart happens, write to master or to all slave the message.
			// If i'm the slave and the caller send to master one Byte that indicate the seme chosen.
			if (BTManager.Instance.isSlave ()) {
				if (Board.Instance.Me.Role == EnRole.CHIAMANTE)
					BTManager.Instance.WriteToMaster (new PackageSeme (Board.Instance.GetChiamante (), Board.Instance.CalledCard.seme));
				// If i'm the master send to all slaves one Byte that indicate the seme chosen.
			} else
				BTManager.Instance.WriteToAllSlave (new PackageSeme (Board.Instance.GetChiamante (), Board.Instance.CalledCard.seme));
		}

		/// <summary>
		/// Sends the MOVE message to the master (if i'm slave and caller) or to all slaves (if i'm master).
		/// </summary>
		/// <remarks>See <see cref="BTLibrary.PackageCard"/> for further informations about the message contents.</remarks>
		/// <param name="move">Move.</param>
		private void cardPlayed (Player player, Card card)
		{
			// When the Board event eventIPlayACard or eventSomeonePlayACard occours, write to master (if i'm slave) or to all slaves (if i'm master) the message.
			if (BTManager.Instance.isSlave ())
				BTManager.Instance.WriteToMaster (new PackageCard (player, card));
			else
				BTManager.Instance.WriteToAllSlave (new PackageCard (player, card));
		}

		#endregion
	}
}