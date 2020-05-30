/*
    DataConnection class
      written in C#                              Version: 1.0
      by The KPD-Team                            Date: 2002/02/06
      Copyright © 2002                           Comments to: KPDTeam@allapi.net
                                                 URL: http://www.allapi.net/


   You are free to use this class file in your own applications,
   but you are expressly forbidden from selling or otherwise
   distributing this code as source without prior written consent.
   This includes both posting samples on a web site or otherwise
   reproducing it in text or html format.
 
   Although much care has gone into the programming of this class
   file, The KPD-Team does not accept any responsibility for damage
   caused by possible errors in this class and/or by misuse of this
   class.
*/

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

/// <summary>Represents a data connection for an FTP session.</summary>
/// <remarks>This class can be used to upload/download a file or to upload/download a string to a buffer.</remarks>
/// <remarks>Although it is used in the FtpConnection class, it is not FTP dependant; hence, it can be used for other protocols as well.</remarks>
public class DataConnection {
	/// <summary>Enumerates all the possible values for the file mode.</summary>
	public enum FileModes {
		Overwrite = 0,
		Append = 1
	}
	/// <summary>Enumerates all the possible values for the strean direction.</summary>
	public enum StreamDirections {
		Download = 0,
		Upload = 1
	}
	// Event delegates
	public delegate void DataReceivedHandler(int ByteLen);
	public delegate void DataSentHandler(int ByteLen);
	// Invoke the DataReceived event
	protected virtual void OnDataReceived(int ByteLen) {
		if (DataReceived != null)
			DataReceived(ByteLen);
	}
	// Invoke the DataSent event
	protected virtual void OnDataSent(int ByteLen) {
		if (DataSent != null)
			DataSent(ByteLen);
	}
	/// <summary>Raised when the class receives new data.</summary>
	public event DataReceivedHandler DataReceived;
	/// <summary>Raised when the class sent new data.</summary>
	public event DataSentHandler DataSent;
	/// <summary>Specifies the byte where to append from.</summary>
	/// <remarks>Byte 0 is the first byte in the stream; if you want to download/upload the entire file, you should set this value to 0.</remarks>
	/// <remarks>If you specify a negative value for this property, it will be changed to 0.</remarks>
	/// <value>A Long that specifies the byte where to append from.</value>
	public long AppendFrom {
        	get {
            		return m_AppendFrom;
        	}
		set {
			if (value < 0)
				m_AppendFrom = 0;
			else
				m_AppendFrom = value;
		}
	}
	/// <summary>Specifies the filemode.</summary>
	/// <value>A member of the FileModes Enum that specifies the filemode.</value>
	public FileModes FileMode {
        	get {
            		return m_FileMode;
        	}
		set {
			m_FileMode = value;
		}
	}
	/// <summary>Specifies the filename.</summary>
	/// <value>A String that specifies the filename.</value>
	/// <exceptions cref="ArgumentNullException">Thrown when the specified value is Nothing (C#, VC++: null)</exceptions>
	public string Filename {
        	get {
            		return m_Filename;
        	}
		set {
			if (value == null)
				throw new ArgumentNullException();
			m_Filename = value;
		}
	}
	/// <summary>Returns the port the connection is using.</summary>
	/// <value>An Integer that specifies the port the connection is using.</value>
	public int Port {
        	get {
            		return m_Port;
        	}
	}
	/// <summary>Specifies the remote IPEndPoint.</summary>
	/// <value>An IPEndPoint that specifies the remote end point.</value>
	/// <exceptions cref="ArgumentNullException">Thrown when the specified value is Nothing (C#, VC++: null)</exceptions>
	public IPEndPoint RemoteEndPoint {
        	get {
            		return m_RemoteEndPoint;
        	}
        	set {
			if (value == null)
				throw new ArgumentNullException();
            		m_RemoteEndPoint = value;
        	}
	}
	/// <summary>Specifies the direction of the stream.</summary>
	/// <value>A member of the StreamDirections Enum that specifies the direction of the stream.</value>
	public StreamDirections StreamDirection {
        	get {
            		return m_StreamDirection;
        	}
		set {
			m_StreamDirection = value;
		}
	}
	/// <summary>Specifies whether to download to a file or not.</summary>
	/// <value>A Boolean that specifies whether to download to a file or not.</value>
	public bool DownloadToFile {
		get {
			return m_DownloadToFile;
		}
		set {
			m_DownloadToFile = value;
		}
	}
	/// <summary>Returns the number of transferred bytes.</summary>
	/// <value>A Long that specifies the number of transferred bytes.</value>
	public long TransferredBytes {
        	get {
            		return m_TransferredBytes;
        	}
	}
	/// <summary>Specifies the data that has been downloaded or has to be uploaded.</summary>
	/// <value>A String that specifies the data that has been downloaded or has to be uploaded.</value>
	/// <exceptions cref="ArgumentNullException">Thrown when the specified value is Nothing (C#, VC++: null)</exceptions>
	public string Data {
        	get {
            		return m_Data;
        	}
		set {
			if (value == null)
				throw new ArgumentNullException();
			m_Data = value;
		}
	}
	/// <summary>Returns the local IPEndPoint.</summary>
	/// <value>An IPEndPoint that specifies the local end point.</value>
	public IPEndPoint GetLocalEndPoint {
		get {
			return (IPEndPoint)m_DataSocket.LocalEndPoint;
		}
	}
	/// <summary>Creates or opens the local data file.</summary>
	/// <exceptions cref="IOException">Thrown when there was an error while creating/opening the local data file.</exceptions>
	public void CreateDataFile() {
		if (m_Filename == "") {
			m_DownloadToFile = false;
		} else if (m_DownloadToFile) {
			try {
				if (m_StreamDirection == StreamDirections.Download) {
					m_FileStream = new FileStream(m_Filename, System.IO.FileMode.OpenOrCreate);
				} else {
					m_FileStream = new FileStream(m_Filename, System.IO.FileMode.Open);
				}
				if ((m_FileMode == FileModes.Append) && (m_AppendFrom > 0)) {
					m_FileStream.Seek(m_AppendFrom, SeekOrigin.Begin);
					m_TransferredBytes = m_FileStream.Position;
				}
			} catch {
				throw new IOException();
			}
		}
	}
	/// <summary>Connects to the remote host.</summary>
	/// <exceptions cref="IOException">Thrown when there was an error while creating/opening the local data file.</exceptions>
	/// <exceptions cref="SocketException">Thrown when there was an error while connecting to the remote host.</exceptions>
	public void Connect() {
		m_TransferredBytes = 0;
		m_Passive = true;
		CreateDataFile();
		try {
			m_DataSocket = new Socket(AddressFamily.Unspecified, SocketType.Stream, ProtocolType.Tcp);
			m_DataSocket.Connect(m_RemoteEndPoint);
		} catch {
			throw new SocketException();
		}
	}
	/// <summary>Opens a port and starts listening for connections.</summary>
	/// <exceptions cref="IOException">Thrown when there was an error while creating/opening the local data file.</exceptions>
	/// <exceptions cref="SocketException">Thrown when there was an error creating the socket.</exceptions>
	public void Listen() {
		m_TransferredBytes = 0;
		m_Passive = false;
		CreateDataFile();
		try {
			m_DataSocket = new Socket(AddressFamily.Unspecified, SocketType.Stream, ProtocolType.Tcp);
			IPAddress localIP  = Dns.Resolve(Dns.GetHostName()).AddressList[0];
			m_DataSocket.Bind(new IPEndPoint(localIP, 0));
			m_Port = ((IPEndPoint)m_DataSocket.LocalEndPoint).Port;
			m_DataSocket.Listen(0);
		} catch {
			throw new SocketException();
		}
	}
	/// <summary>Starts receiving data from the socket.</summary>
	/// <exceptions cref="SocketException">Thrown when there was an error with the socket.</exceptions>
	/// <exceptions cref="IOException">Thrown when there was an error while creating/opening the local data file.</exceptions>
	public void ReceiveFromSocket() {
		Socket theSocket;
		try {
			if (m_Passive)
				theSocket = m_DataSocket;
			else
				theSocket = m_DataSocket.Accept();
		} catch {
			throw new SocketException();
		}
		try {
			int Ret;
			byte [] Buffer = new byte[1024];
			do {
				Ret = theSocket.Receive(Buffer);
				if (Ret > 0) {
					if (m_DownloadToFile)
						m_FileStream.Write(Buffer, 0, Ret);
					else
						m_Data += ASCII.GetString(Buffer, 0, Ret);
					m_TransferredBytes = m_TransferredBytes + Ret;
					OnDataReceived(Ret);
				}
			} while (Ret != 0);
		} catch (IOException ioe) {
			throw ioe;
		} catch {
			throw new SocketException();
		} finally {
			try {
				theSocket.Shutdown(SocketShutdown.Both);
			} catch {}
			theSocket.Close();
		}
	}
	/// <summary>Starts sending data to the socket.</summary>
	/// <exceptions cref="SocketException">Thrown when there was an error with the socket.</exceptions>
	/// <exceptions cref="IOException">Thrown when there was an error while creating/opening the local data file.</exceptions>
	public void SendToSocket() {
		Socket theSocket;
		try {
			if (m_Passive)
				theSocket = m_DataSocket;
			else
				theSocket = m_DataSocket.Accept();
		} catch {
			throw new SocketException();
		}
		try {
			int Ret;
			byte [] Buffer = new byte[1024];
			byte [] StringBuf;
			do {
				if (m_DownloadToFile) {
					m_FileStream.Seek(m_TransferredBytes, SeekOrigin.Begin);
					Ret = m_FileStream.Read(Buffer, 0, 1024);
				} else {
					if (m_TransferredBytes + 1024 < m_Data.Length)
						StringBuf = ASCII.GetBytes(m_Data.Substring((int)(m_TransferredBytes), 1024));
					else
						StringBuf = ASCII.GetBytes(m_Data.Substring((int)(m_TransferredBytes)));
					Ret = StringBuf.Length;
					if (StringBuf != null)
						Array.Copy(StringBuf, Buffer, Ret);
				}
				Ret = theSocket.Send(Buffer, Ret, SocketFlags.None);
				m_TransferredBytes = m_TransferredBytes + Ret;
				OnDataSent(Ret);
			} while (Ret != 0);
		} catch (IOException ioe) {
			throw ioe;
		} catch {
			throw new SocketException();
		} finally {
			try {
				theSocket.Shutdown(SocketShutdown.Both);
			} catch {}
			theSocket.Close();
		}
	}
	/// <summary>Closes the socket.</summary>
	public void Close() {
		try {
			m_DataSocket.Shutdown(SocketShutdown.Both);
		} catch {}
		m_DataSocket.Close();
		try {
			if (m_FileStream != null)
				m_FileStream.Close();
		} catch {}
	}
	// Private variables
	private long m_AppendFrom = 0;
	private FileModes m_FileMode;
	private string m_Filename = "";
	private int m_Port = 0;
	private StreamDirections m_StreamDirection;
	private bool m_DownloadToFile;
	private Socket m_DataSocket;
	private string m_Data = "";
	private long m_TransferredBytes;
	private ASCIIEncoding ASCII = new ASCIIEncoding();
	private IPEndPoint m_RemoteEndPoint;
	private FileStream m_FileStream;
	private bool m_Passive;
}