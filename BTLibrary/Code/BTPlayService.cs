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


namespace BTLibrary
{
	public class BTPlayService: IConnectable, IFindable
	{
		//TODO: rinominare tutte le classi come BT...  DONE
		//TODO: BTPlayService è un singleton(vedi board per copiare) DONE
		//TODO: aggiungere invia a tutti tranne a 1(solo da un master) TO TEST
		//TODO: interfaccia menu
		//TODO: controllare i readonly

		#region fields

		/// <summary>
		/// The activity to register the receiver and display messages.
		/// </summary>
		private Activity _activity;

		/// <summary>
		/// The handler to send messages.
		/// </summary>
		private readonly List<Handler> _handlers = new List<Handler> (4);

		/// <summary>
		/// The local BlueTooth adapter.
		/// </summary>
		private static readonly BluetoothAdapter _btAdapter = BluetoothAdapter.DefaultAdapter;

		/// <summary>
		/// The list of paired device addresses.
		/// </summary>
		private List<string> _pairedDevicesList;

		/// <summary>
		/// The receiver to register intents.
		/// </summary>
		private BTReceiver _receiver;

		/// <summary>
		/// The listen thread for accepting incoming connection.
		/// </summary>
		private BTListenThread listenThread;

		/// <summary>
		/// The connect thread to connect to a device.
		/// </summary>
		internal BTConnectThread connectThread;

		/// <summary>
		/// The connected slave thread to perform communication with a master.
		/// </summary>
		private BTConnectedThread connectedSlaveThread;

		/// <summary>
		/// The list of connected master thread to perform communication with mutiple connected slave.
		/// </summary>
		private List<BTConnectedThread> connectedMasterThread;

		/// <summary>
		/// The UUID to perform connection.
		/// </summary>
		internal static readonly UUID MY_UUID = UUID.FromString ("fa87c0d0-afac-11de-8a39-0800200c9a66");

		/// <summary>
		/// The name to perform connection.
		/// </summary>
		public const string NAME = "PlayService";

		/// <summary>
		/// The state of device.
		/// </summary>
		private ConnectionState _state;

		#endregion

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

		#region BlueToothAdapter Management

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
		/// Unregisters the receiever.
		/// </summary>
		public void UnregisterReceiever ()
		{
			_activity.ApplicationContext.UnregisterReceiver (_receiver);
		}

		/// <summary>
		/// Registers the receiver for when a device is discovered and when discovery is finished.
		/// </summary>
		public void RegisterReceiver ()
		{
			// Register for broadcasts when a device is discovered
			_receiver = new BTReceiver (_handlers [0]);
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

		#endregion

		#region scanning and pairing

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
		/// Sets the state.
		/// </summary>
		/// <param name="state">State.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		private void SetState (ConnectionState state)
		{
			//sets the state
			_state = state;
			// Give the new state to the Handler so the UI Activity can update

			_handlers.ForEach (delegate(Handler h) {
				h.ObtainMessage ((int) MessageType.MESSAGE_STATE_CHANGE, (int) state, -1).SendToTarget ();
			});

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

		/// <summary>
		/// Performs connection as master
		/// </summary>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void ConnectAsMaster ()
		{	
			//stops all existing listening thread
			StopListen ();

			// Start the thread to listen on a BluetoothServerSocket
			listenThread = new BTListenThread (NAME, MY_UUID);
			listenThread.Start ();

			//sets the state on STATE_LISTEN
			SetState (ConnectionState.STATE_LISTEN);
		}

		/// <summary>
		/// Performs connection as slave
		/// </summary>
		/// <param name="device">Device to connect.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void ConnectAsSlave (BluetoothDevice device)
		{	
			//if the device we want to connect is a slave connected to this device the connection is not performed
			for (int i = 0; i < connectedMasterThread.Count; i++) {
				if (connectedMasterThread [i]._Socket.RemoteDevice.Address.CompareTo (device.Address) == 0) {
					return;
				}
			}

			//stops all existing thread
			Stop ();

			// Start the thread to connect with the given device
			connectThread = new BTConnectThread (device, MY_UUID);
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
			connectedSlaveThread = new BTConnectedThread (socket);
			connectedSlaveThread.Start ();

			// Send the name of the connected device back to the Activity
			_handlers [0].ObtainMessage ((int) MessageType.MESSAGE_DEVICE_ADDR, device.Address).SendToTarget ();

			//sets the state to CONNECTED_SLAVE
			SetState (ConnectionState.STATE_CONNECTED_SLAVE);
		}

		/// <summary>
		/// Indicate if is Slave in the network.
		/// </summary>
		/// <returns><c>true</c>, if is client <c>false</c> otherwise.</returns>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public bool isSlave ()
		{
			//if state is Connected_Client the caller is a client
			return _state == ConnectionState.STATE_CONNECTED_SLAVE;

		}

		/// <summary>
		/// Connecteds to slave.
		/// </summary>
		/// <param name="socket">Socket.</param>
		/// <param name="device">Device.</param>
		[MethodImpl (MethodImplOptions.Synchronized)]
		internal void ConnectedToSlave (BluetoothSocket socket, BluetoothDevice device)
		{	
			connectedMasterThread.Add (new BTConnectedThread (socket));
			connectedMasterThread [connectedMasterThread.Count - 1].Start ();

			//sends a message to the activity indicates the connection to a device
			_handlers [0].ObtainMessage ((int) MessageType.MESSAGE_DEVICE_ADDR, device.Address).SendToTarget ();

			SetState (ConnectionState.STATE_CONNECTED_MASTER);
		}

		/// <summary>
		/// Gets the number of devices connected.
		/// </summary>
		/// <returns>The number of devices connected.</returns>
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
				listenThread.Cancel ();
				listenThread = null;
				listenThread = null;
			}

			SetState (ConnectionState.STATE_NONE);
		}

		/// <summary>
		/// Stops the listen Thread.
		/// </summary>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void StopListen ()
		{
			if (listenThread != null) {
				listenThread.Cancel ();
				listenThread = null;
				listenThread = null;
			}

			SetState (ConnectionState.STATE_NONE);
		}

		/// <summary>
		/// Removes a slave .
		/// </summary>
		/// <param name="address">Address of the slave to remove.</param>
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
		/// Indicate that the connection attempt failed and notify the Activity.
		/// </summary>
		internal void ConnectionFailed ()
		{	
			// Send a failure message back to the Activity
			_handlers [0].ObtainMessage ((int) MessageType.MESSAGE_TOAST, "I don't find any BlueTooth game opened on this device, Please retry").SendToTarget ();
			//stops all existing thread
			Stop ();
			//return to initial state
			SetState (ConnectionState.STATE_NONE);
		}

		/// <summary>
		/// Indicate that the connection was lost and notify the UI Activity.
		/// </summary>
		internal void ConnectionLost ()
		{	
			// Send a failure message back to the Activity
			_handlers.ForEach (delegate(Handler h) {
				h.ObtainMessage ((int) MessageType.MESSAGE_TOAST, "Device connection was lost: ").SendToTarget ();
			});

			//if device was a slave return to initial state
			//if (_state == ConnectionState.STATE_CONNECTED_SLAVE) {
			Stop ();
			SetState (ConnectionState.STATE_NONE);
			//}

			//TODO CAZZO FACCIO SE IL MASTER PERDE LA COMUNICAZIONE CON UNO SLAVE??????
		}

		#endregion

		#region Communication Management

		/// <summary>
		/// Write to the Master in an unsynchronized manner
		/// </summary>
		/// <param name='out'>
		/// The String to write.
		/// </param>
		public void WriteToMaster<T> (IBTSendable<T> bts)
		{
			WriteToMaster (bts.toByteArray ());
		}

		/// <summary>
		/// Write to the Master in an unsynchronized manner
		/// </summary>
		/// <param name='out'>
		/// The String to write.
		/// </param>
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
			WriteToSlave (bts.toByteArray (), player);
		}

		public void WriteToSlave (byte [] bts, int player)
		{

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
			tmp.Write (bts);
		}

		/// <summary>
		/// Writes to all slave except the one specified in the address.
		/// </summary>
		/// <param name="address">Address.</param>
		/// <param name="bts"> the parameter to send.</param>
		public void WriteToAllSlaveExceptOne<T> (IBTSendable<T> bts, string address)
		{
			WriteToAllSlaveExceptOne (bts.toByteArray (), address);
		}

		public void WriteToAllSlaveExceptOne (byte [] bts, string address)
		{
			BTConnectedThread tmp;
			for (int i = 0; i < connectedMasterThread.Count; i++) {
				lock (this) {
					if (connectedMasterThread [i]._Socket.RemoteDevice.Address == address || connectedMasterThread [i] == null)
						return;
					tmp = connectedMasterThread [i];
				}
				tmp.Write (bts);
			}
		}

		/// <summary>
		/// Writes to all slave.
		/// </summary>
		/// <param name="bts">the parameter to send.</param>
		/// <typeparam name="T">The type parameter.</typeparam>
		public void WriteToAllSlave<T> (IBTSendable<T> bts)
		{
			WriteToAllSlave (bts.toByteArray ());
		}

		public void WriteToAllSlave (byte [] bts)
		{

			BTConnectedThread tmp;
			for (int i = 0; i < connectedMasterThread.Count; i++) {
				lock (this) {
					if (connectedMasterThread [i] != null) {
						tmp = connectedMasterThread [i];
						tmp.Write (bts);
					}
				}

			}
		}

		#endregion

		#region handler Management

		/// <summary>
		/// Sets the handler.
		/// </summary>
		/// <param name="handler">Handler.</param>
		public void AddHandler (Handler handler)
		{
			_handlers.Add (handler);
		}

		public void RemoveHandler (Handler handler)
		{
			_handlers.Remove (handler);
		}

		public void ResetHandler ()
		{
			_handlers.Clear ();
		}

		public void forEachHandler (Action<Handler> action)
		{
			_handlers.ForEach (action);
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
			_state = ConnectionState.STATE_NONE;

			//creates an arry of connectedThread with _MAXPLAYER elements
			connectedMasterThread = new List<BTConnectedThread> ();
		}

		#endregion

	}
}