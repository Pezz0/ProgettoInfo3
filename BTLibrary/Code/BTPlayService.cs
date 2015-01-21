using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Lang;
using Java.Util;
using ChiamataLibrary;
using System;
using System.Text;


namespace BTLibrary
{
	public class BTPlayService: Handler,IConnectable, IFindable
	{
		#region singleton implementation

		private static readonly Lazy<BTPlayService> _instance = new Lazy<BTPlayService> (() => new BTPlayService ());

		public static BTPlayService Instance{ get { return _instance.Value; } }

		private BTPlayService ()
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
		public BluetoothAdapter getBTAdapter ()
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
		/// Indicate if BT is enable.
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
		public string GetLocalAddress ()
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
		/// Registers the receiver for when a device is discovered and when discovery is finished.
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
		}

		#endregion

		#region BlueTooth visibility and enable

		/// <summary>
		/// Makes the Bluetooth visible.
		/// </summary>
		/// <param name="amount">amount of time to make the bluetooth visible.</param>
		public void makeVisible (int amount)
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
		private List<string> _pairedDevicesList;

		/// <summary>
		/// Gets a list of paired device addresses.
		/// </summary>
		/// <returns>The paired device addresses.</returns>
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
		/// Performs discovery of new device
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
		/// The listen thread for accepting incoming connection.
		/// </summary>
		private BTListenThread _listenThread;

		/// <summary>
		/// The connect thread to connect to a device.
		/// </summary>
		private BTConnectThread _connectThread;

		/// <summary>
		/// The thread to perform slave read from a master device
		/// </summary>
		private BTReadThread _readFromMasterThread;

		/// <summary>
		/// The thread to perform slave write to a master device
		/// </summary>
		private BTWriteThread _writeToMasterThread;

		/// <summary>
		/// The list of thread to perform master read with mutiple connected slave.
		/// </summary>
		private List<BTReadThread> _readFromSlaveThread;

		/// <summary>
		/// The list of thread to perform master write with multiple connected slave
		/// </summary>
		private List<BTWriteThread> _writeToSlaveThread;

		/// <summary>
		/// The UUID to perform connection.
		/// </summary>
		internal static readonly UUID MY_UUID = UUID.FromString ("fa87c0d0-afac-11de-8a39-0800200c9a66");

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
		public EnConnectionState GetState ()
		{
			return _state;
		}

		/// <summary>
		/// Performs connection as master
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
		/// Performs connection as slave
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
		/// Connecteds to Master.
		/// </summary>
		/// <param name="socket">Socket.</param>
		/// <param name="device">Device.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		internal void ConnectedToMaster (BluetoothSocket socket, BluetoothDevice device)
		{
			//sets the state to CONNECTED_SLAVE
			SetState (EnConnectionState.STATE_CONNECTED_SLAVE);

			// Start the thread to perform read service
			_readFromMasterThread = new BTReadThread (socket);

			// Start the thread to perform write service
			_writeToMasterThread = new BTWriteThread (socket);

			//sends a message to the indicate the slave connection to a device
			this.ObtainMessage ((int) EnLocalMessageType.MESSAGE_DEVICE_ADDR, (int) EnConnectionState.STATE_CONNECTED_SLAVE, -1, device.Address).SendToTarget ();

		}

		/// <summary>
		/// Indicate if is Slave in the network.
		/// </summary>
		/// <returns><c>true</c>, if is client <c>false</c> otherwise.</returns>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public bool isSlave ()
		{
			//if state is Connected_Client the caller is a client
			return _state == EnConnectionState.STATE_CONNECTED_SLAVE;

		}

		/// <summary>
		/// Connecteds to slave.
		/// </summary>
		/// <param name="socket">Socket.</param>
		/// <param name="device">Device.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		internal void ConnectedToSlave (BluetoothSocket socket, BluetoothDevice device)
		{	
			//sets the state to CONNECTED_MASTER
			SetState (EnConnectionState.STATE_CONNECTED_MASTER);

			// Start the thread to perform read service
			_readFromSlaveThread.Add (new BTReadThread (socket));

			// Start the thread to perform write service
			_writeToSlaveThread.Add (new BTWriteThread (socket));

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
			return _readFromSlaveThread.Count;
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

			if (_readFromMasterThread != null) {
				_readFromMasterThread.Cancel ();
				_readFromMasterThread = null;
			}

			if (_writeToMasterThread != null) {
				_writeToMasterThread.Cancel ();
				_writeToMasterThread = null;
			}

			for (int i = 0; i < _writeToSlaveThread.Count; i++) {
				_writeToSlaveThread [i].Cancel ();
				_writeToSlaveThread.RemoveAt (i);
			}

			for (int i = 0; i < _readFromSlaveThread.Count; i++) {
				_readFromSlaveThread [i].Cancel ();
				_readFromSlaveThread.RemoveAt (i);
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
		/// Removes a slave .
		/// </summary>
		/// <param name="address">Address of the slave to remove.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void RemoveSlave (string address)
		{
			for (int i = 0; i < _readFromSlaveThread.Count; i++) {

				if (_readFromSlaveThread [i]._socket.RemoteDevice.Address.CompareTo (address) == 0) {
					_readFromSlaveThread [i].Cancel ();
					_readFromSlaveThread.RemoveAt (i);

					_writeToSlaveThread [i].Cancel ();
					_writeToSlaveThread.RemoveAt (i);
					return;
				}
			}
		}

		/// <summary>
		/// Indicate that the connection attempt failed and notify the Activity.
		/// </summary>
		internal void ConnectionFailed ()
		{	
			//stops all existing thread
			Stop ();

		}

		/// <summary>
		/// Indicate that the connection was lost and notify the UI Activity.
		/// </summary>
		internal void ConnectionLost ()
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

		/// <summary>
		/// Gets the activity.
		/// </summary>
		/// <returns>The activity.</returns>
		public Activity getActivity ()
		{
			return _activity;
		}

		#endregion

		#region initialization

		/// <summary>
		/// The Max player bluetooth.
		/// </summary>
		public const int MAX_BT_PLAYER = 4;

		/// <summary>
		/// Initialize the specified the Play Service.
		/// </summary>
		/// <param name="activity">Activity.</param>
		/// <param name="handler">Handler.</param>
		public void Initialize (Activity activity)//, int maxplayer)
		{
			// Initialize the list of device that are already paired  
			_pairedDevicesList = new List<string> ();

			//activity to register the receiver
			_activity = activity;

			//sets the state to STATE_NONE
			_state = EnConnectionState.STATE_NONE;

			//creates an arry of connectedThread with _MAXPLAYER elements
			_readFromSlaveThread = new List<BTReadThread> (MAX_BT_PLAYER);

			_writeToSlaveThread = new List<BTWriteThread> (MAX_BT_PLAYER);

			initializeComunication ();
		}

		#endregion

		#region Handler

		public delegate void eventHandlerMessageInitialization (Message msg);

		public event eventHandlerMessageInitialization eventMessageInitialization;

		/// <summary>
		/// delegate to handle a playtime message.
		/// </summary>
		public delegate void eventHandlerPackageRecieved (Package pkg);

		/// <summary>
		/// Occurs when a playtime message is received.
		/// </summary>
		public event eventHandlerPackageRecieved eventPackageReceived;

		/// <summary>
		/// Handle the messages.
		/// </summary>
		public override void HandleMessage (Message msg)
		{
			if (msg.What == (int) EnLocalMessageType.MESSAGE_READ) {	//package message(remote)
				byte [] data = (byte []) msg.Obj;

				//Get the type of the message from the firs byte of the message
				EnPackageType type = (EnPackageType) data [0];

				if (type == EnPackageType.ACK) {
				
					char [] adr = new char[17];

					//the next 17 bytes indicete the address of the device who sends message
					for (int i = 1; i < 18; i++)
						adr [i - 1] = (char) data [i];

					string address = new string (adr);

					List<byte> bs = new List<byte> ();
					//the other bytes indicate the message (normal or playtime)
					for (int i = 18; i < data.GetLength (0); i++)
						bs.Add (data [i]);
						
					if (isSlave ()) {
						//if i am a slave i remove the message from the list of the message to send
						_writeToMasterThread.Remove (bs.ToArray ());
					} else {
						//otherwise i am the master so i remove the message only from the list of the sender
						_writeToSlaveThread.ForEach (delegate(BTWriteThread thred) {
							if (thred.Connected == address)
								thred.Remove (bs.ToArray ());
						});
					}
						

				} else if (type != EnPackageType.NONE) {

					Package pkg = Package.createPackage (data);	

					//ACK consists of the type ACK followed by the message received 
					if (BTPlayService.Instance.isSlave ())
						_writeToMasterThread.Add (pkg.getAckMessage ());
					else
						for (int i = 0; i < _writeToSlaveThread.Count; i++)
							if (_writeToSlaveThread [i] != null)
								_writeToSlaveThread [i].Add (pkg.getAckMessage ());
						
				}
				// all other messages that are not message_read (STATE_CHANGE, DEVICE_ADDRESS ecc) are initializing events
			} else if (eventMessageInitialization != null)	//local message
				eventMessageInitialization (msg);

		}

		#endregion

		#region Communication Management

		public void WriteToMaster (Package pkg)
		{
			if (_writeToMasterThread == null)
				return;

			_writeToMasterThread.Add (pkg.getMessage ());

		}

		private void WriteToSlave (Package pkg, int slave)
		{
			if (_writeToSlaveThread [slave] == null) {
				Toast.MakeText (Application.Context, "Client not connected", ToastLength.Long).Show ();
				return;
			}
			_writeToSlaveThread [slave].Add (pkg.getMessage ());

		}


		public void WriteToAllSlave (Package pkg)
		{
			byte [] message = pkg.getMessage ();
			for (int i = 0; i < _writeToSlaveThread.Count; i++) {
				if (_writeToSlaveThread [i] != null)
					_writeToSlaveThread [i].Add (message);
			}
		}

		private void initializeComunication ()
		{
			Board.Instance.eventImReady += imReady;
			Board.Instance.eventIPlaceABid += bidPlaced;
			if (!BTPlayService.Instance.isSlave ())
				Board.Instance.eventSomeonePlaceABid += bidPlaced;
			Board.Instance.eventPlaytimeStart += semeChosen;
			Board.Instance.eventIPlayACard += cardPlayed;
			if (!BTPlayService.Instance.isSlave ())
				Board.Instance.eventSomeonePlayACard += cardPlayed;
		}

		//When the Board event eventImReady happens, write to master or to all slave the message
		private void imReady ()
		{
			//the message is only one byte because the ready event doesn't need any information
			if (BTPlayService.Instance.isSlave ())
				WriteToMaster (new PackageReady ());
			else
				WriteToAllSlave (new PackageReady ());

		}

		//When the Board event eventIPlaceABid or eventSomeonePlaceABid happens, write to master or to all slave the message
		private void bidPlaced (IBid bid)
		{
			//the message is compose of the nuber of bid to control that happens in the correct board time
			// then is added the information about the bid type 
			if (BTPlayService.Instance.isSlave ())
				BTPlayService.Instance.WriteToMaster (new PackageBid (bid));
			else
				BTPlayService.Instance.WriteToAllSlave (new PackageBid (bid));
		}

		//When the Board event eventPlaytimeStart happens, write to master or to all slave the message
		private void semeChosen ()
		{
			//if this is the slave and is the caller send to master one byte that indicate the seme chosen 
			if (BTPlayService.Instance.isSlave ()) {
				if (Board.Instance.Me.Role == EnRole.CHIAMANTE)
					BTPlayService.Instance.WriteToMaster (new PackageSeme (Board.Instance.getChiamante (), Board.Instance.CalledCard.seme));
				//if this is the master send to all slave one byte that indicate the seme chosen
			} else
				BTPlayService.Instance.WriteToAllSlave (new PackageSeme (Board.Instance.getChiamante (), Board.Instance.CalledCard.seme));
		}

		//When the Board event eventIPlayACard or eventSomeonePlayACard happens, write to master or to all slave the message
		private void cardPlayed (Move move)
		{
			//the message is composed of the time where the card is played and then the information about the card
			if (BTPlayService.Instance.isSlave ())
				BTPlayService.Instance.WriteToMaster (new PackageCard (move));
			else
				BTPlayService.Instance.WriteToAllSlave (new PackageCard (move));
		}

		#endregion
	}
}