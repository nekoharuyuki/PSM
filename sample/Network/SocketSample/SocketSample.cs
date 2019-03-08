/* PlayStation(R)Mobile SDK 1.21.01
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

using System.Net;
using System.Net.Sockets;

namespace Sample
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
        private byte[] sendBuffer = new byte[8];
		private byte[] recvBuffer = new byte[8];

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
        public bool DataExchange()
		{
			try 
			{
				try
				{
					enterCriticalSection();
					byte[] ArrayX	= BitConverter.GetBytes(myPosition.X);
					byte[] ArrayY = BitConverter.GetBytes(myPosition.Y);
					ArrayX.CopyTo(sendBuffer, 0);
					ArrayY.CopyTo(sendBuffer, ArrayX.Length);
					
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
								networkPosition.X = BitConverter.ToSingle(recvBuffer, 0);
								networkPosition.Y = BitConverter.ToSingle(recvBuffer, 4);
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
								networkPosition.X = BitConverter.ToSingle(recvBuffer, 0);
								networkPosition.Y = BitConverter.ToSingle(recvBuffer, 4);
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
public static class SocketSample
{
	private static SampleButton serverButton;
	private static SampleButton clientButton;
	private static GraphicsContext graphics;
		
	private static LocalTCPConnection	tcpServer;
	private static LocalTCPConnection	tcpClient;
	
	static bool loop = true;
		
	public static void Main(string[] args)
	{
		tcpServer = new LocalTCPConnection(true, 11000);
		tcpClient = new LocalTCPConnection(false, 11000);
			
		tcpServer.Listen();
//		tcpClient.Connect();
			
		Init();

		while (loop) {
			SystemEvents.CheckEvents();
			Thread.Sleep(100);
			Update();
			Render();
		}

		Term();
	}

	public static bool Init()
	{
		graphics = new GraphicsContext();
		SampleDraw.Init(graphics);
			
		serverButton = new SampleButton(32, 32, 128, 48);
		clientButton = new SampleButton(32 + (SampleDraw.Width / 2), 32, 128, 48);

		serverButton.SetText(tcpServer.buttonString);
		clientButton.SetText(tcpClient.buttonString);

		return true;
	}

	/// Terminate
	public static void Term()
	{
		SampleDraw.Term();
		graphics.Dispose();
			
		serverButton.Dispose();
		clientButton.Dispose();
	}

	public static bool Update()
	{
		SampleDraw.Update();

		tcpServer.DataExchange();
		tcpClient.DataExchange();
			
		return true;
	}
		
	/**
	 * Display status
	 */
	public static void drawStatus(LocalTCPConnection LocalTCP)
	{
		const int BoxWH		= 32;
		const int BoxHalfWH	= BoxWH / 2;
		int   drawBaseX = (LocalTCP.IsServer) ? 0 : (SampleDraw.Width / 2);

		uint[] colorTable = {0xffff0000,
							 0xff0000ff};
		Sce.PlayStation.Core.Vector2[] Pos = new Sce.PlayStation.Core.Vector2[] { LocalTCP.MyPosition, LocalTCP.networkPosition };
   		for(int i = 0 ; i < Pos.Length; i++)
   		{
			SampleDraw.FillRect(colorTable[i], (int)Pos[i].X - BoxHalfWH + drawBaseX, (int)Pos[i].Y - BoxHalfWH, BoxWH, BoxWH);
		}
   		
		SampleDraw.DrawText("Status : " + LocalTCP.statusString, 0xffffffff, 176 + drawBaseX, 48);
		SampleDraw.ClearSprite();
	}
		
	public static bool Render()
	{
		graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
		graphics.Clear();
		serverButton.SetText(tcpServer.buttonString);
		clientButton.SetText(tcpClient.buttonString);

		List<TouchData> touchDataList = Touch.GetData(0);
			
		// Modify status
		if (serverButton.TouchDown(touchDataList)){
			tcpServer.ChangeStatus();
		}
		if (clientButton.TouchDown(touchDataList)){
			tcpClient.ChangeStatus();
		}
		/*	
		uint[] colorTable = {0xffff0000,
							 0xff00ff00,
							 0xff0000ff,
							 0xffffff00};
		*/
		foreach (var touchData in touchDataList) {
			/*
						if (touchData.Status == TouchStatus.Down ||
							touchData.Status == TouchStatus.Move) {

							int pointX = (int)((touchData.X + 0.5f) * Graphics2D.Width);
							int pointY = (int)((touchData.Y + 0.5f) * Graphics2D.Height);
							int radius = (int)(touchData.Force * 32);
							int colorId = touchData.ID % colorTable.Length;

							Graphics2D.FillCircle(colorTable[colorId], pointX, pointY, radius);
					
						}
			*/
			if (touchData.Status == TouchStatus.Down){
				int pointX = (int)((touchData.X + 0.5f) * SampleDraw.Width);
				int pointY = (int)((touchData.Y + 0.5f) * SampleDraw.Height);
				if(pointX < (SampleDraw.Width / 2)){
					tcpServer.SetMyPosition(pointX, pointY);
				}
				else{
					tcpClient.SetMyPosition(pointX - (SampleDraw.Width / 2), pointY);
				}
			}
				
				
		}
		
		// Display button and status
		serverButton.Draw();
		clientButton.Draw();
		drawStatus(tcpServer);
		drawStatus(tcpClient);

		SampleDraw.DrawText("Socket Sample", 0xffffffff, 0, 0);
		graphics.SwapBuffers();

		return true;
	}

}

} // Sample
