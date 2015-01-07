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


namespace BTLibrary
{
	public class BTPlayService: IConnectable, IFindable
	{
		//TODO: rinominare tutte le classi come BT...  DONE
		//TODO: BTPlayService è un singleton(vedi board per copiare) DONE
		//TODO: aggiungere invia a tutti tranne a 1(solo da un master) TO TEST
		//TODO: interfaccia menu
		//TODO: controllare i readonly

		private Activity _activity;
		private Handler _handler;

		//Fields for finding devices
		private BluetoothAdapter _btAdapter;
		private List<string> _pairedDevicesList;
		private BTReceiver _receiver;

		//Fields to perform connection
		private BTListenThread listenThread;
		internal BTConnectThread connectThread;
		private BTConnectedThread connectedSlaveThread;
		private List<BTConnectedThread> connectedMasterThread;
		//private int counter = 0;

		internal static UUID MY_UUID = UUID.FromString ("fa87c0d0-afac-11de-8a39-0800200c9a66");
		public const string NAME = "PlayService";

		private ConnectionState _state;

		#region singleton implementation

		private static readonly BTPlayService _instance = new BTPlayService ();

		public static BTPlayService Instance{ get { return _instance; } }

		static BTPlayService ()
		{
		}

		private BTPlayService ()
		{
		}

		#endregion

		public void Initialize (Activity activity, Handler handler)//, int maxplayer)
		{
			// Initialize the list of device that are already paired  
			_pairedDevicesList = new List<string> ();

			//handler to communicate the new discovered devices
			_handler = handler;

			//max number of player in the network
			//_MAXPLAYER = maxplayer;

			//activity to register the receiver
			_activity = activity;

			// Get the local Bluetooth adapter
			_btAdapter = BluetoothAdapter.DefaultAdapter;

			//sets the state to STATE_NONE
			_state = ConnectionState.STATE_NONE;

			//creates an arry of connectedThread with _MAXPLAYER elements
			connectedMasterThread = new List<BTConnectedThread> ();
		}

		/// <summary>
		/// Indicate if the bluetooth exists one this device .
		/// </summary>
		/// <returns><c>true</c>, if bluetooth existe, <c>false</c> otherwise.</returns>
		public bool existBluetooth ()
		{
			return _btAdapter != null;
		}

		/// <summary>
		/// Indicate if device is discovering for new device.
		/// </summary>
		/// <returns><c>true</c>, if is discovering, <c>false</c> otherwise.</returns>
		public bool isDiscovering ()
		{
			return _btAdapter.IsDiscovering;
		}

		/// <summary>
		/// Abort unwanted discovery.
		/// </summary>
		public void CancelDiscovery ()
		{
			_btAdapter.CancelDiscovery ();
		}

		/// <summary>
		/// Unregisters the reciever.
		/// </summary>
		public void UnregisterReceiever ()
		{
			_activity.ApplicationContext.UnregisterReceiver (_receiver);
		}

		public void RegisterReceiver ()
		{
			// Register for broadcasts when a device is discovered
			_receiver = new BTReceiver (_handler);
			var filter = new IntentFilter (BluetoothDevice.ActionFound);
			_activity.ApplicationContext.RegisterReceiver (_receiver, filter);
		
			// Register for broadcasts when discovery has finished
			filter = new IntentFilter (BluetoothAdapter.ActionDiscoveryFinished);
			_activity.ApplicationContext.RegisterReceiver (_receiver, filter);
		}

		/// <summary>
		/// Indicate if the Bluetooth is enabled.
		/// </summary>
		/// <returns><c>true</c>, if Bluetooth is enabled, <c>false</c> otherwise.</returns>
		public bool isBTEnabled ()
		{
			return _btAdapter.IsEnabled;
		}

		/// <summary>
		/// Gets the remote device which as the address specified in the parameter .
		/// </summary>
		/// <returns>The remote device.</returns>
		/// <param name="address">Address of the device.</param>
		public BluetoothDevice getRemoteDevice (string address)
		{
			if (address == null)
				return null;
			return _btAdapter.GetRemoteDevice (address);
		}

		/// <summary>
		/// Gets the paired device.
		/// </summary>
		/// <returns>A list of address of paired device.</returns>
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
		/// Perfroms the discovery for new devices
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
			_activity.StartActivityForResult (visibleIntent, (int) ActivityResultCode.VISIBILITY_REQUEST);
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
			_activity.StartActivityForResult (enableIntent, (int) ActivityResultCode.REQUEST_ENABLE_BT);
		}

		/// <summary>
		/// Indicate if is Slave in the network.
		/// </summary>
		/// <returns><c>true</c>, if is client <c>false</c> otherwise.</returns>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public bool isSlave ()
		{
			//if state is Connected_Client the caller is a client
			if (_state == ConnectionState.STATE_CONNECTED_SLAVE)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Sets the state.
		/// </summary>
		/// <param name="state">State.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		private void SetState (ConnectionState state)
		{
			//sets the state
			_state = state;
			// Give the new state to the Handler so the UI Activity can update
			_handler.ObtainMessage ((int) MessageType.MESSAGE_STATE_CHANGE, (int) state, -1).SendToTarget ();
		}

		[MethodImpl (MethodImplOptions.Synchronized)]
		public void setHandler (Handler handler)
		{
			_handler = handler;
		}

		/// <summary>
		/// Gets the state.
		/// </summary>
		/// <returns>The state.</returns>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public ConnectionState GetState ()
		{
			return _state;
		}


		public string GetLocalName ()
		{
			return _btAdapter.Name;
		}

		public string GetLocalAddress ()
		{
			return _btAdapter.Address;
		}

		/// <summary>
		/// Connects as master in the network.
		/// </summary>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void ConnectAsMaster ()
		{	
			//stops all existing thread
			StopListen ();

			// Start the thread to listen on a BluetoothServerSocket
			listenThread = new BTListenThread (this, NAME, MY_UUID);
			listenThread.Start ();

			//sets the state on STATE_LISTEN
			SetState (ConnectionState.STATE_LISTEN);
		}

		/// <summary>
		/// Connects as client to the server specified in the parameter.
		/// </summary>
		/// <param name="device">Master to connect.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void ConnectAsSlave (BluetoothDevice device)
		{	
			//if the device we want to connect is a slave connected to this device the connection is not performed
			for (int i = 0; i < connectedMasterThread.Count; i++) {
				if (connectedMasterThread [i]._Socket.RemoteDevice.Address.CompareTo (device.Address) == 0) {
					var msg = _handler.ObtainMessage ((int) MessageType.MESSAGE_TOAST, "Unable to connect device");
					_handler.SendMessage (msg);

					return;
				}

			}
			//stops all existing thread
			Stop ();
			// Start the thread to connect with the given device
			connectThread = new BTConnectThread (device, this, MY_UUID);
			connectThread.Start ();

			//sets the state to CONNECTING
			SetState (ConnectionState.STATE_CONNECTING);
		}

		/// <summary>
		/// Connecteds to Master.
		/// </summary>
		/// <param name="socket">Socket.</param>
		/// <param name="device">Device.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		internal void ConnectedToMaster (BluetoothSocket socket, BluetoothDevice device)
		{
			//stops all existing thread
			Stop ();

			// Start the thread to manage the connection and perform transmissions
			connectedSlaveThread = new BTConnectedThread (socket, this);
			connectedSlaveThread.Start ();

			// Send the name of the connected device back to the Activity
			_handler.ObtainMessage ((int) MessageType.MESSAGE_DEVICE_ADDR, device.Address).SendToTarget ();


			//sets the state to CONNECTED_SLAVE
			SetState (ConnectionState.STATE_CONNECTED_SLAVE);
		}

		/// <summary>
		/// Connecteds to slave.
		/// </summary>
		/// <param name="socket">Socket.</param>
		/// <param name="device">Device.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		internal void ConnectedToSlave (BluetoothSocket socket, BluetoothDevice device)
		{	
			// Cancel the thread that completed the connection
			if (connectThread != null) {
				connectThread.Cancel ();
				connectThread = null;
			}
				
			// Cancel the listen thread because we only want to connect to _MAXPLAYER device
			/*if (listenThread != null && counter >= _MAXPLAYER - 1) {
				listenThread.Cancel ();
				listenThread = null;
			}*/

			// Start the thread to manage the connection and perform transmissions
			//if (counter < _MAXPLAYER) {
			connectedMasterThread.Add (new BTConnectedThread (socket, this));
			connectedMasterThread [connectedMasterThread.Count - 1].Start ();


			//sends a message to the activity indicates the connection to a device
			_handler.ObtainMessage ((int) MessageType.MESSAGE_DEVICE_ADDR, device.Address).SendToTarget ();

			SetState (ConnectionState.STATE_CONNECTED_MASTER);


			//if there are _MAXPLAYER connections sets the stete to CONNECTED_MASTER, otherwise rest in LISTEN
			/*if (counter == _MAXPLAYER)
					SetState (ConnectionState.STATE_CONNECTED_MASTER);
				else
					SetState (ConnectionState.STATE_LISTEN);*/
			//} else {
			//ConnectionFailed ();
			//}
		}

		[MethodImpl (MethodImplOptions.Synchronized)]
		public int getNumConnected ()
		{
			return connectedMasterThread.Count;
		}

		/// <summary>
		/// Stop all threads.
		/// </summary>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void Stop ()
		{
			if (connectThread != null) {
				connectThread.Cancel ();
				connectThread = null;
			}

			if (connectedSlaveThread != null) {
				connectedSlaveThread.Cancel ();
				connectedSlaveThread = null;
			}

			for (int i = 0; i < connectedMasterThread.Count; i++) {
				connectedMasterThread [i].Cancel ();
				connectedMasterThread.RemoveAt (i);
			}
			connectedMasterThread.Clear ();

			if (listenThread != null) {
				BTListenThread tmp = listenThread;
				tmp.Cancel ();
				tmp = null;
				listenThread = null;
			}
		}

		[MethodImpl (MethodImplOptions.Synchronized)]
		public void StopListen ()
		{

			if (listenThread != null) {
				BTListenThread tmp = listenThread;
				tmp.Cancel ();
				tmp = null;
				listenThread = null;
			}
			/*if (counter > 0)
				SetState (ConnectionState.STATE_CONNECTED_MASTER);*/
			SetState (ConnectionState.STATE_NONE);
		}

		[MethodImpl (MethodImplOptions.Synchronized)]
		public void RemoveSlave (string address)
		{
			for (int i = 0; i < connectedMasterThread.Count; i++) {
				if (connectedMasterThread [i]._Socket.RemoteDevice.Address.CompareTo (address) == 0) {
					connectedMasterThread [i].Cancel ();
					connectedMasterThread.RemoveAt (i);
					return;
				}
			}
		}

		/// <summary>
		/// Write to the Master in an unsynchronized manner
		/// </summary>
		/// <param name='out'>
		/// The String to write.
		/// </param>
		public void WriteToMaster<T> (IBTSendable<T> bts)
		{

			byte [] msg = bts.toByteArray ();

			// Create temporary ConnectedThread
			BTConnectedThread tmp;
			// Synchronize a copy of the ConnectedThread
			lock (this) {
				if (_state != ConnectionState.STATE_CONNECTED_SLAVE)
					return;

				tmp = connectedSlaveThread;
			}
			// Perform the write unsynchronized
			tmp.Write (msg);

		}

		public void WriteToMaster (byte [] bts)
		{
		
			// Create temporary ConnectedThread
			BTConnectedThread tmp;
			// Synchronize a copy of the ConnectedThread
			lock (this) {
				if (_state != ConnectionState.STATE_CONNECTED_SLAVE)
					return;

				tmp = connectedSlaveThread;
			}
			// Perform the write unsynchronized
			tmp.Write (bts);

		}

		/// <summary>
		/// Write to the Slave in an unsynchronized manner
		/// </summary>
		/// <param name='out'>
		/// The String to write.
		/// </param>
		/// <param name="player">
		/// The slave we want to write
		/// </param>
		public void WriteToSlave<T> (IBTSendable<T> bts, int player)
		{
			byte [] msg = bts.toByteArray ();
			// Create temporary ConnectedThread
			BTConnectedThread tmp;
			// Synchronize a copy of the ConnectedThread
			lock (this) {

				if (connectedMasterThread [player] == null) {
					Toast.MakeText (Application.Context, "Client not connected", ToastLength.Long).Show ();
					return;
				}
				tmp = connectedMasterThread [player];
			}
			// Perform the write unsynchronized
			tmp.Write (msg);
		}

		/// <summary>
		/// Writes to all slave except the one specified in the address.
		/// </summary>
		/// <param name="address">Address.</param>
		public void WriteToAllSlaveExceptOne<T> (IBTSendable<T> bts, string address)
		{
			byte [] msg = bts.toByteArray ();
			BTConnectedThread tmp;
			for (int i = 0; i < connectedMasterThread.Count; i++) {
				lock (this) {
					if (connectedMasterThread [i]._Socket.RemoteDevice.Address == address || connectedMasterThread [i] == null)
						return;
					tmp = connectedMasterThread [i];
				}
				tmp.Write (msg);
			}
		}

		public void WriteToAllSlave<T> (IBTSendable<T> bts)
		{
			byte [] msg = bts.toByteArray ();
			BTConnectedThread tmp;
			for (int i = 0; i < connectedMasterThread.Count; i++) {
				lock (this) {
					if (connectedMasterThread [i] != null) {
						tmp = connectedMasterThread [i];
						tmp.Write (msg);
					}
				}
				
			}
		}

		/// <summary>
		/// Indicate that the connection attempt failed and notify the Activity.
		/// </summary>
		internal void ConnectionFailed ()
		{	
			// Send a failure message back to the Activity
			var msg = _handler.ObtainMessage ((int) MessageType.MESSAGE_TOAST, "Unable to connect device");
			_handler.SendMessage (msg);
			Stop ();
			SetState (ConnectionState.STATE_NONE);
			//ConnectAsMaster ();
		}

		/// <summary>
		/// Indicate that the connection was lost and notify the UI Activity.
		/// </summary>
		internal void ConnectionLost ()
		{	
			// Send a failure message back to the Activity
			var msg = _handler.ObtainMessage ((int) MessageType.MESSAGE_TOAST, "Device connection was lost");
			_handler.SendMessage (msg);

//			if (connectThread == null) {
//				SetState (ConnectionState.STATE_LISTEN);
//				ConnectAsMaster ();
//			}
			if (_state == ConnectionState.STATE_CONNECTED_SLAVE) {
				SetState (ConnectionState.STATE_NONE);
				Stop ();
			}

			//TODO CAZZO FACCIO SE IL MASTER PERDE LA COMUNICAZIONE CON UNO SLAVE??????
		}

		public BluetoothAdapter getBTAdapter ()
		{
			return _btAdapter;
		}

		public Handler getHandler ()
		{
			return _handler;
		}
		/// <summary>
		/// Receiver class.
		/// </summary>


		/// <summary>
		/// Listen thread calss.
		/// </summary>

	}
}