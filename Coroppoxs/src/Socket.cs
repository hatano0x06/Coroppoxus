/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Threading;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace AppRpg
{
	
	/**
	 * SocketListenerInterface
	 */
	interface ISocketListener
	{
		/**
		 * Accept
		 */
		void OnAccept(IAsyncResult AsyncResult);
		/**
		 * Connect
		 */
		void OnConnect(IAsyncResult AsyncResult);
		/**
		 * Receive
		 */
		void OnReceive(IAsyncResult AsyncResult);
		/**
		 * Send
		 */
		void OnSend(IAsyncResult AsyncResult);
	}

	/**
	 * SocketEventCallback
	 */
	class SocketEventCallback
	{
		/**
		 * AcceptCallback
		 */
		public static void AcceptCallback(IAsyncResult AsyncResult) 
		{
			LocalTCPConnection Server = (LocalTCPConnection)AsyncResult.AsyncState;
			Server.OnAccept(AsyncResult);
		}

		/**
		 * ConnectCallback
		 */
		public static void ConnectCallback(IAsyncResult AsyncResult)
		{
			LocalTCPConnection Client = (LocalTCPConnection)AsyncResult.AsyncState;
			Client.OnConnect(AsyncResult);
		}
		/**
		 * ReceiveCallback
		 */
		public static void ReceiveCallback(IAsyncResult AsyncResult)
		{
			LocalTCPConnection TCPs = (LocalTCPConnection)AsyncResult.AsyncState;
			TCPs.OnReceive(AsyncResult);
		}

		/**
		 * SendCallback
		 */
		public static void SendCallback(IAsyncResult AsyncResult)
		{
			LocalTCPConnection TCPs = (LocalTCPConnection)AsyncResult.AsyncState;
			TCPs.OnSend(AsyncResult);
		}
	}
	
	/**
	 * Class for SocketTCP local connection
	 */
	public class LocalTCPConnection : ISocketListener
	{
		/**
		 * Status
		 */
		public enum Status
		{
			kNone,		
			kListen,	// Listen or connecting
			kConnected,	
			kUnknown
		}
/*
		using (CriticalSection CS = new CriticalSection(syncObject))
		{
		
		}
		public class CriticalSection : IDisposable
		{
			private object syncObject = null;
			public CriticalSection(object SyncObject)
			{
				syncObject = SyncObject;
				Monitor.Enter(syncObject);
			}

			public virtual void Dispose()
			{
				Monitor.Exit(syncObject);
				syncObject = null;
			}
		}
*/
        /**
         * Object for exclusive  socket access
         */
        private object syncObject = new object();
		/**
		 * Enter critical section
		 */
		private void enterCriticalSection() 
		{
			Monitor.Enter(syncObject);
		}
		/**
		 * Leave critical section
		 */
		private void leaveCriticalSection() 
		{
			Monitor.Exit(syncObject);
		}

		/**
		 * Get status
		 * 
		 * @return Status
		 */
		public Status StatusType
		{
			get
			{
				try
				{
					enterCriticalSection();
					if (Socket == null){
						return Status.kNone;
					}
					else{
						if (IsServer){
							if(ClientSocket == null){
								return Status.kListen;
							}
							return Status.kConnected;
						}
						else{
							if(IsConnect == false){
								return Status.kListen;
							}
							return Status.kConnected;
						}
					}
				}
				finally
				{
					leaveCriticalSection();
				}
			}
		}

        /**
         * Get status as string
         * 
         * @return status string
         */
        public string statusString
		{
			get
			{
				switch (StatusType)
				{
					case Status.kNone:
						return "None";

					case Status.kListen:
						if (IsServer){
							return "Listen";
						}
						else{
							return "Connecting";
						}

					case Status.kConnected:
						return "Connected";
				}
				return "Unknown";
			}
		}

		/**
		 * Get the button string based on status
		 * 
		 * @return button string
		 */
		public string buttonString
		{
			get
			{
				switch (StatusType)
				{
					case Status.kNone:
						if (IsServer){
							return "Listen";
						}
						else{
							return "Connect";
						}
					case Status.kListen:
						return "Disconnect";
					case Status.kConnected:
						return "Disconnect";
				}
				return "Unknown";
			}
		}

        /**
		 * Process the button that lets us change the status based on 
		 * current status 
         */
        public void ChangeStatus()
		{
			switch(StatusType)
			{
				case	Status.kNone:
					if (IsServer){
						Listen();
					}
					else{
						Connect();
					}
					break;

				case	Status.kListen:
					Disconnect();
					break;
				
				case	Status.kConnected:
					Disconnect();
					break;
			}
		}

        /**
         * transceiver buffer
         */
        private byte[] sendBuffer = new byte[50];
		private byte[] recvBuffer = new byte[20];

		/**
		 * Our position or the other party's
		 */
		private Sce.PlayStation.Core.Vector2 myPosition		= new Sce.PlayStation.Core.Vector2(128, 256);
		public	Sce.PlayStation.Core.Vector2 MyPosition
		{
			get { return myPosition; }
		}
		public	void	SetMyPosition(float X, float Y)
		{
			myPosition.X = X;
			myPosition.Y = Y;
		}
		
		public Sce.PlayStation.Core.Vector2 networkPosition	= new Sce.PlayStation.Core.Vector2(256, 256);
		
		public string getRecieve;		
		public string Recieve{
			get{return this.getRecieve;}			
		}
		public Sce.PlayStation.Core.Vector2 NetworkPosition
		{
			get { return networkPosition; }
		}
		
		/**
		 * Are we connected
		 */
		private bool isConnect = false;
		public bool IsConnect
		{
					get	{	return isConnect; }
			private set	{	this.isConnect = value;	}
		}

        /**
         * Socket  Listen when server  Server connect when client
         */
        private Socket socket;
		public  Socket Socket 
		{
			get	{	return socket;	}
		}

		/**
		 * Client socket when server
		 */
		private Socket clientSocket;
		public Socket ClientSocket
		{
					get	{	return clientSocket;	}
			private set	{	this.clientSocket = value;	}
		}

		/**
		 * Is this a server
		 */
		private bool isServer;
		public bool IsServer
		{
			get	{	return isServer;	}
		}

		/**
		 * Port number
		 */
		private UInt16 port;
		public UInt16 Port
		{
			get	{	return port;	}
		}

		/**
		 * Constructor
		 */
		public LocalTCPConnection(bool IsServer, UInt16 Port)
		{
			isServer  = IsServer;
			port      = Port;
		}

        /**
         * Listen
         * Can only be executed when server
         */
        public bool Listen()
		{
			if (isServer == false) {
				return false;
			}
			try
			{
				enterCriticalSection();
				if (socket == null) {
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					// IPEndPoint EP = new IPEndPoint(IPAddress.Any, port);
					IPEndPoint EP = new IPEndPoint(IPAddress.Loopback, port);
					socket.Bind(EP);
					socket.Listen(1);
					socket.BeginAccept(new AsyncCallback(SocketEventCallback.AcceptCallback), this);
				}
			}
			finally
			{
				leaveCriticalSection();
			}
			return true;
		}

        /**
         * Connect to the local host server
         * 
         * Can only be executed when client
         */
        public bool Connect() 
		{
			if (isServer == true){
				return false;
			}
			try
			{
				enterCriticalSection();
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
/*				
				IPAddress LocalIP = null;
				IPHostEntry IPInfo = Dns.GetHostEntry("localhost");
				foreach (IPAddress Info in IPInfo.AddressList)
				{
					if (Info.AddressFamily == AddressFamily.InterNetwork){
						LocalIP = Info;
						break;
					}
				}
				IPEndPoint EP = new IPEndPoint(LocalIP, port);
*/				
				IPEndPoint EP = new IPEndPoint(IPAddress.Loopback, port);
				socket.BeginConnect(EP, new AsyncCallback(SocketEventCallback.ConnectCallback), this);
			}
			finally
			{
				leaveCriticalSection();
			}
			return true;
		}

		/**
		 * Disconnect
		 */
		public void Disconnect() 
		{
			try
			{
				enterCriticalSection();
				if (socket != null){
					if (IsServer){
						Console.WriteLine("Disconnect Server");
						if (clientSocket != null){
							clientSocket.Close();
							// clientSocket.Shutdown(SocketShutdown.Both);
							clientSocket = null;
						}
					}
					else{
						Console.WriteLine("Disconnect Client");
					}
					//  socket.Shutdown(SocketShutdown.Both);
					socket.Close();
					socket		= null;
					IsConnect	= false;
				}
			}
			finally
			{
				leaveCriticalSection();
			}
		}

        /**
         * Data transceiver 
         */
        public bool DataSend(Int32 senddata)
		{
			try 
			{
				try
				{

					enterCriticalSection();
					byte[] Array = BitConverter.GetBytes(senddata);
					
                    
					Array.CopyTo(sendBuffer, 0);
					
					if (isServer){
						if (clientSocket == null || IsConnect == false){
							return false;
						}
						clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
						clientSocket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);
					}
					else{
						if (socket == null || IsConnect == false){
							return false;
						}
						socket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
						socket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);
					}
				}
				finally
				{
					leaveCriticalSection();
				}
			}
			catch(System.Net.Sockets.SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
					Console.WriteLine("DataExchange 切断検出");
					Disconnect();
				}
				Console.WriteLine("ExchangeError " + e.ToString());
			}
			return true;
		}

		public bool DataSend(string senddata)
		{
			try 
			{
				try
				{
					enterCriticalSection();

					byte[] Array = Encoding.Unicode.GetBytes(senddata);
//					byte[] Array = sjisEnc.GetBytes(senddata);
//					byte[] Array = cp932.GetBytes(senddata);
                    
					Array.CopyTo(sendBuffer, 0);
					
					if (isServer){
						if (clientSocket == null || IsConnect == false){
							return false;
						}
						clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
						clientSocket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);
					}
					else{
						if (socket == null || IsConnect == false){
							return false;
						}
						socket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
						socket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);
					}
					
				}
				finally
				{
					leaveCriticalSection();
				}
			}
			catch(System.Net.Sockets.SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
					Console.WriteLine("DataExchange 切断検出");
					Disconnect();
				}
				Console.WriteLine("ExchangeError " + e.ToString());
			}
			return true;
		}


		/***
		 * Accept
		 */
		public void OnAccept(IAsyncResult AsyncResult)
		{
			try
			{
				try
				{
					enterCriticalSection();
					if (Socket != null){
						ClientSocket = Socket.EndAccept(AsyncResult);
						Console.WriteLine("Accept " + ClientSocket.RemoteEndPoint.ToString());
						IsConnect = true;
					}
				}
				finally
				{
					leaveCriticalSection();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			Console.WriteLine("OnAccept");
		}
		/***
		 * Connect
		 */
		public void OnConnect(IAsyncResult AsyncResult)
		{
			try
			{
				try
				{
					enterCriticalSection();
					if (Socket != null){
						// Complete the connection.
						Socket.EndConnect(AsyncResult);
						Console.WriteLine("Connect " + Socket.RemoteEndPoint.ToString());
						IsConnect = true;
					}
				}
				finally
				{
					leaveCriticalSection();
				}
			}
			catch (System.Net.Sockets.SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionRefused){
					Disconnect();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			Console.WriteLine("OnConnect");
		}
		
		/**
		 * Receive
		 */
		public void OnReceive(IAsyncResult AsyncResult)
		{
			int Len = 0;
			try
			{
				try
				{
					enterCriticalSection();
					if (IsServer){
						if (ClientSocket != null){
							Len = ClientSocket.EndReceive(AsyncResult);
							// 切断
							if (Len <= 0){
								Disconnect();
							}
							else{
								getRecieve = Encoding.Unicode.GetString(recvBuffer);
							}
						}
					}
					else{
						if (Socket != null){
							Len = Socket.EndReceive(AsyncResult);
							// 切断
							if (Len <= 0){
								Disconnect();
							}
							else{
								getRecieve = Encoding.Unicode.GetString(recvBuffer);
							}
						}
					}
				}
				finally
				{
					leaveCriticalSection();
				}
			}
			catch (System.Net.Sockets.SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
					Console.WriteLine("ReceiveCallback 切断検出");
					Disconnect();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			Console.WriteLine("OnReceive");
		}
		
		/**
		 * Send
		 */
		public void OnSend(IAsyncResult AsyncResult)
		{
			int Len = 0;
//			int a = 0;
			try
			{
				try
				{
					enterCriticalSection();
					if (IsServer){
						if (ClientSocket != null){
							Len = ClientSocket.EndSend(AsyncResult);
						}
					}
					else{
						if (Socket != null){
							Len = Socket.EndSend(AsyncResult);
						}
					}
                    // Disconnection detection should go here...
					if (Len <= 0){
						// send error
					}
				}
				finally
				{
					leaveCriticalSection();
				}
			}
			catch (System.Net.Sockets.SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
					Console.WriteLine("SendCallback 切断検出");
					Disconnect();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			Console.WriteLine("OnSend");
		}
	};
	
/**
 * SocketSample
 */
public class SocketSample
{
	private LocalTCPConnection	tcpClient;
    private static SocketSample instance = new SocketSample();
		
    /// インスタンスの取得
    public static SocketSample GetInstance()
    {
        return instance;
    }		
		
	public void Init(){
		tcpClient = new LocalTCPConnection(false, 2001);
	}
		
	public void start(){
		tcpClient.ChangeStatus();
	}
		
	public void sendData(int id, int tweetNumber){
		tcpClient.DataSend(id.ToString()+tweetNumber.ToString());
	}
		
	public string getString(int tweetNumber){
		tcpClient.DataSend("5"+tweetNumber.ToString());
		return tcpClient.Recieve;
	}		
}

} // Sample
