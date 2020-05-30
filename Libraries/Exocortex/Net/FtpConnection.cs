/*
    FtpConnection class
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
using System.Text;
using System.Net;
using System.Net.Sockets;

/// <summary>Represents a connection to an FTP server.</summary>
public class FtpConnection {
	/// <summary>Enumerates the different stream modes.</summary>
	public enum StreamModes {
		ASCII = 65,
		Binary = 73
	}
	// Event delegates
	public delegate void ConnectedHandler();
	public delegate void ConnectionFailedHandler();
	public delegate void DisconnectedHandler();
	public delegate void CommandSentHandler(string Command);
	public delegate void CommandCompletedHandler();
	public delegate void ReceivedReplyHandler(string Reply, short ReplyNumber, short ReplyType);
	public delegate void NewDirectoryListingHandler();
	// Methods to invoke the events
	protected virtual void OnConnected() {
		if (Connected != null)
			Connected();
	}
	protected virtual void OnConnectionFailed() {
		if (ConnectionFailed != null)
			ConnectionFailed();
	}
	protected virtual void OnDisconnected() {
		if (Disconnected != null)
			Disconnected();
	}
	protected virtual void OnCommandSent(string Command) {
		if (CommandSent != null)
			CommandSent(Command);
	}
	protected virtual void OnCommandCompleted() {
		if (CommandCompleted != null)
			CommandCompleted();
	}
	protected virtual void OnReceivedReply(string Reply, short ReplyNumber, short ReplyType) {
		if (ReceivedReply != null)
			ReceivedReply(Reply, ReplyNumber, ReplyType);
	}
	protected virtual void OnNewDirectoryListing() {
		if (NewDirectoryListing != null)
			NewDirectoryListing();
	}
	/// <summary>Raised when the object successfully connected to the remote host.</summary>
	public event ConnectedHandler Connected;
	/// <summary>Raised when the connection to the remote host failed or when the connection to the host is lost.</summary>
	public event ConnectionFailedHandler ConnectionFailed;
	/// <summary>Raised when the connection to the remote host is closed.</summary>
	public event DisconnectedHandler Disconnected;
	/// <summary>Raised when an FTP command has been sent to the remote host.</summary>
	public event CommandSentHandler CommandSent;
	/// <summary>Raised when a command was successfully completed.</summary>
	/// <remarks>Do note that this does *not* mean the command has been successful.</remarks>
	public event CommandCompletedHandler CommandCompleted;
	/// <summary>Raised when a the object received a reply from the remote host.</summary>
	public event ReceivedReplyHandler ReceivedReply;
	/// <summary>Raised when a new directory listing is available.</summary>
	public event NewDirectoryListingHandler NewDirectoryListing;
	/// <summary>Specifies the remote port to use when the object connects to the remote host.</summary>
	/// <value>An Integer that specifies the remote port to use when the object connects to the remote host.</value>
	public int Port {
        	get {
            		return remotePort;
        	}
		set {
			remotePort = value;
		}
	}
	/// <summary>Specifies the remote host to connect to.</summary>
	/// <value>A String that specifies the remote host to connect to.</value>
	/// <exceptions cref="ArgumentNullException">Thrown when the specified value is Nothing (C#, VC++: null)<exceptions>
	public string Hostname {
        	get {
            		return remoteAddress;
        	}
		set {
			if (value == null)
				throw new ArgumentNullException();
			remoteAddress = value;
		}
	}
	/// <summary>Specifies the Username to use when connecting to a remote host.</summary>
	/// <value>A String that specifies the Username to use when connecting to a remote host.</value>
	/// <exceptions cref="ArgumentNullException">Thrown when the specified value is Nothing (C#, VC++: null)<exceptions>
	public string Username {
        	get {
            		return logonName;
        	}
		set {
			if (value == null)
				throw new ArgumentNullException();
			logonName = value;
		}
	}
	/// <summary>Specifies the Password to use when connecting to a remote host.</summary>
	/// <value>A String that specifies the Password to use when connecting to a remote host.</value>
	/// <exceptions cref="ArgumentNullException">Thrown when the specified value is Nothing (C#, VC++: null)<exceptions>
	public string Password {
        	get {
            		return logonPass;
        	}
		set {
			if (value == null)
				throw new ArgumentNullException();
			logonPass = value;
		}
	}
	/// <summary>Specifies the WorkDir of the FTP connection.</summary>
	/// <value>A String that specifies the WorkDir of the FTP connection.</value>
	/// <exceptions cref="ArgumentNullException">Thrown when the specified value is Nothing (C#, VC++: null)<exceptions>
	public string WorkDir {
		get {
			return remoteDirectory;
		}
		set {
			if (value == null)
				throw new ArgumentNullException();
			if (IsConnected)
				if (!remoteDirectory.Equals(value))
					ChangeWorkDir(value);
			else
				remoteDirectory = value;
		}
	}
	/// <summary>Returns a Boolean that specifies whether the object is connected to a remote host or not.</summary>
	/// <value>A Boolean that specifies whether the object is connected to a remote host or not.</value>
	public bool IsConnected {
		get {
			return isNowConnected;
		}
	}
	/// <summary>Specifies whether to use passive transfers or not.</summary>
	/// <value>A Boolean that specifies whether to use passive transfers or not.</value>
	public bool Passive {
		get {
			return passiveTransfers;
		}
		set {
			passiveTransfers = value;
		}
	}
	/// <summary>Connects to the remote host and logs on.</summary>
	public void Connect() {
		try {
			clientSocket = new Socket(AddressFamily.Unspecified, SocketType.Stream, ProtocolType.Tcp);
			clientSocket.Connect(new IPEndPoint(Dns.Resolve(remoteAddress).AddressList[0], remotePort));
			isNowConnected = true;
			OnConnected();
		} catch {
			isNowConnected = false;
			OnConnectionFailed();
			return;
		}
		WaitForResponse();
		SendCommand("USER " + logonName);
		if (lastResponseType == 4 || lastResponseType == 5) {
			isNowConnected = false;
			return;
		}
		SendCommand("PASS " + logonPass);
		if (lastResponseType == 4 || lastResponseType == 5) {
			isNowConnected = false;
			return;
		}
		if (remoteDirectory == "")
			PrintWorkDir();
		else
			ChangeWorkDir(remoteDirectory);
	}
	/// <summary>Logs off and disconnects from the remote host.</summary>
	public void Disconnect() {
		if (!isNowConnected)
			return;
		SendCommand("QUIT");
		try {
			clientSocket.Shutdown(SocketShutdown.Both);
		} finally {
			clientSocket.Close();
			isNowConnected = false;
		}
		OnDisconnected();
	}
	/// <summary>Starts receiving data from the command socket, until it has received a complete reply.</summary>
	private void WaitForResponse() {
		if (!isNowConnected)
			return;
		byte [] tempBuffer = new byte[1024];
		int retBytes;
		string retString = "";
		do {
			try {
				retBytes = clientSocket.Receive(tempBuffer);
			} catch {
				break;
			}
			retString += ASCII.GetString(tempBuffer, 0, retBytes);
		} while(!IsValidResponse(retString));
		lastResponse = retString;
		if (retString.Length >= 3)
			lastResponseNumber = short.Parse(retString.Substring(0, 3));
		else
			lastResponseNumber = 0;
		lastResponseType = (short)(Math.Floor(lastResponseNumber / 100));
		OnReceivedReply(lastResponse, lastResponseNumber, lastResponseType);
	}
	/// <summary>Checks whether a string is a valid reply or not.</summary>
	/// <param name="Input">The string that has to be checked for validity.</param>
	private bool IsValidResponse(string Input) {
		string [] Lines = Input.Split('\n');
		if (Lines.Length > 1) {
			try {
				if (Lines[Lines.Length - 2].Replace("\r", "").Substring(3, 1).Equals(" "))
					 return true;
			} catch {
				return false;
			}
		}
		return false;
	}
	/// <summary>Sends the specified command to the server.</summary>
	/// <param name="Command">The command to be sent to the server.</param>
	private void SendCommand(string Command) {
		try {
			clientSocket.Send(ASCII.GetBytes(Command + "\r\n"));
			if (Command.Length >= 4 && Command.Substring(0, 4).ToUpper().Equals("PASS")) {
				OnCommandSent("PASS ********");
			} else {
				OnCommandSent(Command);
			}
			WaitForResponse();
		} catch {
			OnConnectionFailed();
			lastResponseNumber = 0;
			lastResponseType = 0;
			lastResponse = "";
		}
	}
	/// <summary>Returns the last response number.</summary>
	/// <returns>A Short that represents the last response number.</returns>
	public short GetLastResponseNumber() {
		return lastResponseNumber;
	}
	/// <summary>Returns the last response.</summary>
	/// <returns>A String that represents the last response.</returns>
	public string GetLastResponse() {
		return lastResponse;
	}
	/// <summary>Aborts an FTP transfer.</summary>
	public void Abort() {
		if (!IsConnected)
			return;
		SendCommand("ABOR");
		OnCommandCompleted();
	}
	/// <summary>Changes the current workdir to the specified directory.</summary>
	/// <param name="NewDirectory">The new directory.<param>
	public void ChangeWorkDir(string NewDirectory) {
		if (!IsConnected)
			return;
		SendCommand("CWD " + NewDirectory);
		PrintWorkDir();
	}
	/// <summary>Changes the current workdir to the parent directory.</summary>
	public void ChangeToUpDir() {
		if (!IsConnected)
			return;
		SendCommand("CDUP");
		OnCommandCompleted();
	}
	/// <summary>Changes the permissions of a file or directory.</summary>
	/// <param name="Filename">The filename(s) or directory name(s) to modify.<param>
	/// <param name="NewMode">The new permissions for the specified file(s)/dir(s).<param>
	public void ChMod(string Filename, string NewMode) {
		if (!IsConnected)
			return;
		SendCommand("SITE chmod " + NewMode + " "+ Filename);
		OnCommandCompleted();
	}
	/// <summary>Sends a custom command to the server.</summary>
	/// <param name="Command">The custom command to send.<param>
	public void CustomCommand(string Command) {
		if (!IsConnected)
			return;
		SendCommand(Command);
		OnCommandCompleted();
	}
	/// <summary>Deletes a file from the server.</summary>
	/// <param name="Filename">The file to delete.<param>
	public void Delete(string Filename){
		if (!IsConnected)
			return;
		SendCommand("DELE " + Filename);
		OnCommandCompleted();
	}
	/// <summary>Retrieves information about the FTP server.</summary>
	public void GetSystemInfo() {
		if (!IsConnected)
			return;
		SendCommand("SYST");
		OnCommandCompleted();
	}
	/// <summary>Retrieves help from the server.</summary>
	public void Help() {
		if (!IsConnected)
			return;
		SendCommand("HELP");
		OnCommandCompleted();
	}
	/// <summary>Retrieves help from the server about a specific command.</summary>
	/// <param name="CommandName">The command to get help for.<param>
	public void Help(string CommandName) {
		if (!IsConnected)
			return;
		SendCommand("HELP " + CommandName);
		OnCommandCompleted();
	}
	/// <summary>Creates a directory on the server.</summary>
	/// <param name="DirectoryName">The name of the new directory.<param>
	public void MakeDir(string DirectoryName) {
		if (!IsConnected)
			return;
		SendCommand("MKD " + DirectoryName);
		OnCommandCompleted();
	}
	/// <summary>Does nothing.</summary>
	/// <remarks>This command is used to keep connections from timing out.</remarks>
	public void NoOperation() {
		if (!IsConnected)
			return;
		SendCommand("NOOP");
		OnCommandCompleted();
	}
	/// <summary>Retrieves the current work dir.</summary>
	public void PrintWorkDir() {
		if (!IsConnected)
			return;
		SendCommand("PWD");
		int startPos = lastResponse.IndexOf("\"");
		int endPos;
		try {
			if (startPos >= 0) {
				endPos  = lastResponse.IndexOf("\"", startPos + 1);
				if (endPos >= 0)
					remoteDirectory = lastResponse.Substring(startPos + 1, endPos - startPos - 1);
				else
					remoteDirectory = lastResponse.Substring(4);
			} else
				remoteDirectory = lastResponse.Substring(4);
		} catch {}
		OnCommandCompleted();
	}
	/// <summary>Removes a directory from the server.</summary>
	/// <param name="DirectoryName">The directory to remove.<param>
	public void RemoveDir(string DirectoryName) {
		if (!IsConnected)
			return;
		SendCommand("RMD " + DirectoryName);
		OnCommandCompleted();
	}
	/// <summary>Renames a file on the server.</summary>
	/// <param name="OriginalName">The name of the original file.<param>
	/// <param name="NewName">The new name of the file.<param>
	public void Rename(string OriginalName, string NewName) {
		if (!IsConnected)
			return;
		SendCommand("RNFR " + OriginalName);
		if (lastResponseType == 3)
			SendCommand("RNTO " + NewName);
		OnCommandCompleted();
	}
	/// <summary>Executes a site command on the server.</summary>
	/// <param name="Command">The site command to execute.<param>
	public void SiteCommand(string Command) {
		if (!IsConnected)
			return;
		SendCommand("SITE " + Command);
		OnCommandCompleted();
	}
	/// <summary>Gets the status of the server.</summary>
	public void Status() {
		if (!IsConnected)
			return;
		SendCommand("STAT");
		OnCommandCompleted();
	}
	/// <summary>Retrieves the directory listing of the remote path.</summary>
	/// <returns>Returns True if the request was successful, False otherwise.</returns>
	public bool GetList() {
		if (!IsConnected)
			return false;
		SendCommand("TYPE A");
		if (!CreateDataSocket(DataConnection.StreamDirections.Download)) {
			OnCommandCompleted();
			return false;
		}
		if (!passiveTransfers) {
			IPEndPoint MyEndPoint = dataSocket.GetLocalEndPoint;
			SendCommand("PORT " + MyEndPoint.Address.ToString().Replace(".", ",") + "," + ((int)Math.Floor(MyEndPoint.Port / 256)).ToString() + "," + (MyEndPoint.Port % 256).ToString());
		}
		SendCommand("LIST");
		if (lastResponseType != 4 && lastResponseType != 5) {
			try {
				dataSocket.ReceiveFromSocket();
			} catch {
				dataSocket.Close();
				WaitForResponse();
				OnCommandCompleted();
				return false;
			}
			WaitForResponse();
		}
		ParseDirList(dataSocket.Data);
		dataSocket.Close();
		OnNewDirectoryListing();
		OnCommandCompleted();
		return true;
	}
	/// <summary>Downloads a file from the remote server.</summary>
	/// <param name="RemoteFile">Specifies the remote file to download.</param>
	/// <param name="LocalFile">Specifies the local file to download to.</param>
	/// <returns>Returns True if the request was successful, False otherwise.</returns>
	public bool DownloadFile(string RemoteFile, string LocalFile) {
		return DownloadFile(RemoteFile, LocalFile, StreamModes.Binary, DataConnection.FileModes.Overwrite, 0);
	}
	/// <summary>Downloads a file from the remote server.</summary>
	/// <param name="RemoteFile">Specifies the remote file to download.</param>
	/// <param name="LocalFile">Specifies the local file to download to.</param>
	/// <param name="StreamMode">Specifies the transfer type.</param>
	/// <returns>Returns True if the request was successful, False otherwise.</returns>
	public bool DownloadFile(string RemoteFile, string LocalFile, StreamModes StreamMode) {
		return DownloadFile(RemoteFile, LocalFile, StreamMode, DataConnection.FileModes.Overwrite, 0);
	}
	/// <summary>Downloads a file from the remote server.</summary>
	/// <param name="RemoteFile">Specifies the remote file to download.</param>
	/// <param name="LocalFile">Specifies the local file to download to.</param>
	/// <param name="FileMode">Specifies the file mode.</param>
	/// <param name="AppendFrom">Specifies where to append from.</param>
	/// <returns>Returns True if the request was successful, False otherwise.</returns>
	public bool DownloadFile(string RemoteFile, string LocalFile, DataConnection.FileModes FileMode, long AppendFrom) {
		return DownloadFile(RemoteFile, LocalFile, StreamModes.Binary, DataConnection.FileModes.Overwrite, AppendFrom);
	}
	/// <summary>Downloads a file from the remote server.</summary>
	/// <param name="RemoteFile">Specifies the remote file to download.</param>
	/// <param name="LocalFile">Specifies the local file to download to.</param>
	/// <param name="StreamMode">Specifies the transfer type.</param>
	/// <param name="FileMode">Specifies the file mode.</param>
	/// <param name="AppendFrom">Specifies where to append from.</param>
	/// <returns>Returns True if the request was successful, False otherwise.</returns>
	public bool DownloadFile(string RemoteFile, string LocalFile, StreamModes StreamMode, DataConnection.FileModes FileMode, long AppendFrom) {
		SendCommand("TYPE " + Convert.ToChar(StreamMode));
		if (!CreateDataSocket(LocalFile, FileMode, DataConnection.StreamDirections.Download, AppendFrom)) {
			OnCommandCompleted();
			return false;
		}
		if (!passiveTransfers) {
			IPEndPoint MyEndPoint = dataSocket.GetLocalEndPoint;
			SendCommand("PORT " + MyEndPoint.Address.ToString().Replace(".", ",") + "," + ((int)Math.Floor(MyEndPoint.Port / 256)).ToString() + "," + (MyEndPoint.Port % 256).ToString());
		}
		if (FileMode == DataConnection.FileModes.Append && AppendFrom > 0) {
			SendCommand("REST " + AppendFrom.ToString());
		}
		SendCommand("RETR " + RemoteFile);
		if (lastResponseType != 4 && lastResponseType != 5) {
			try {
				dataSocket.ReceiveFromSocket();
			} catch {
				dataSocket.Close();
				WaitForResponse();
				OnCommandCompleted();
				return false;
			}
			WaitForResponse();
		}
		dataSocket.Close();
		OnCommandCompleted();
		return true;
	}
	/// <summary>Uploads a file to the remote server.</summary>
	/// <param name="LocalFile">Specifies the local file to upload.</param>
	/// <param name="RemoteFile">Specifies the remote file to upload to.</param>
	/// <returns>Returns True if the request was successful, False otherwise.</returns>
	public bool UploadFile(string LocalFile, string RemoteFile) {
		return UploadFile(LocalFile, RemoteFile, StreamModes.Binary, DataConnection.FileModes.Overwrite, 0);
	}
	/// <summary>Uploads a file to the remote server.</summary>
	/// <param name="LocalFile">Specifies the local file to upload.</param>
	/// <param name="RemoteFile">Specifies the remote file to upload to.</param>
	/// <param name="StreamMode">Specifies the transfer type.</param>
	/// <returns>Returns True if the request was successful, False otherwise.</returns>
	public bool UploadFile(string LocalFile, string RemoteFile, StreamModes StreamMode) {
		return UploadFile(LocalFile, RemoteFile, StreamMode, DataConnection.FileModes.Overwrite, 0);
	}
	/// <summary>Uploads a file to the remote server.</summary>
	/// <param name="LocalFile">Specifies the local file to upload.</param>
	/// <param name="RemoteFile">Specifies the remote file to upload to.</param>
	/// <param name="FileMode">Specifies the file mode.</param>
	/// <param name="AppendFrom">Specifies where to resume from.</param>
	/// <returns>Returns True if the request was successful, False otherwise.</returns>
	public bool UploadFile(string LocalFile, string RemoteFile, DataConnection.FileModes FileMode, long AppendFrom) {
		return UploadFile(LocalFile, RemoteFile, StreamModes.Binary, FileMode, AppendFrom);
	}
	/// <summary>Uploads a file to the remote server.</summary>
	/// <param name="LocalFile">Specifies the local file to upload.</param>
	/// <param name="RemoteFile">Specifies the remote file to upload to.</param>
	/// <param name="StreamMode">Specifies the transfer type.</param>
	/// <param name="FileMode">Specifies the file mode.</param>
	/// <param name="AppendFrom">Specifies where to resume from.</param>
	/// <returns>Returns True if the request was successful, False otherwise.</returns>
	public bool UploadFile(string LocalFile, string RemoteFile, StreamModes StreamMode, DataConnection.FileModes FileMode, long AppendFrom) {
		SendCommand("TYPE " + Convert.ToChar(StreamMode));
		if (!CreateDataSocket(LocalFile, FileMode, DataConnection.StreamDirections.Upload, AppendFrom)) {
			OnCommandCompleted();
			return false;
		}
		if (!passiveTransfers) {
			IPEndPoint MyEndPoint = dataSocket.GetLocalEndPoint;
			SendCommand("PORT " + MyEndPoint.Address.ToString().Replace(".", ",") + "," + ((int)Math.Floor(MyEndPoint.Port / 256)).ToString() + "," + (MyEndPoint.Port % 256).ToString());
		}
		if (FileMode == DataConnection.FileModes.Append && AppendFrom > 0) {
			SendCommand("REST " + AppendFrom.ToString());
			SendCommand("APPE " + RemoteFile);
		} else
			SendCommand("STOR " + RemoteFile);
		if (lastResponseType != 4 && lastResponseType != 5) {
			try {
				dataSocket.SendToSocket();
			} catch {
				dataSocket.Close();
				WaitForResponse();
				OnCommandCompleted();
				return false;
			}
			WaitForResponse();
		}
		dataSocket.Close();
		OnCommandCompleted();
		return true;
	}
	/// <summary>Creates a data socket.</summary>
	/// <param name="StreamDirection">Specifies the direction of the stream.</param>
	/// <returns>Returns True if it was successfully created, False otherwise.</returns>
	private bool CreateDataSocket(DataConnection.StreamDirections StreamDirection) {
		return CreateDataSocket("", DataConnection.FileModes.Overwrite, StreamDirection, 0);
	}
	/// <summary>Creates a data socket.</summary>
	/// <param name="Filename">Specifies the local filename.</param>
	/// <param name="Filemode">Specifies the file mode.</param>
	/// <param name="StreamDirection">Specifies the direction of the stream.</param>
	/// <param name="AppendFrom">Specifies where to append from.</param>
	/// <returns>Returns True if it was successfully created, False otherwise.</returns>
	private bool CreateDataSocket(string Filename, DataConnection.FileModes Filemode, DataConnection.StreamDirections StreamDirection, long AppendFrom) {
		try {
			dataSocket = new DataConnection();
			dataSocket.Filename = Filename;
			dataSocket.DownloadToFile = (Filename != "");
			dataSocket.AppendFrom = AppendFrom;
			dataSocket.FileMode = Filemode;
			dataSocket.StreamDirection = StreamDirection;
			dataSocket.DataSent += new DataConnection.DataSentHandler(DataSent);
			dataSocket.DataReceived += new DataConnection.DataReceivedHandler(DataReceived);
			if (passiveTransfers) {
				SendCommand("PASV");
				if (lastResponseType == 2) {
					int BeginPos = lastResponse.IndexOf("(");
					int EndPos = lastResponse.IndexOf(")", BeginPos + 1);
					if (BeginPos > 0 && EndPos > 0) {
						string [] Output = lastResponse.Substring(BeginPos + 1, EndPos - BeginPos - 1).Split(',');
						if (Output.Length == 6) {
							passiveEndPoint = new IPEndPoint(IPAddress.Parse(Output[0] + "." + Output[1] + "." + Output[2] + "." + Output[3]), int.Parse(Output[4]) * 256 + int.Parse(Output[5]));
							dataSocket.RemoteEndPoint = passiveEndPoint;
							dataSocket.Connect();
						}
					}
				}
			} else
				dataSocket.Listen();
		} catch {
			return false;
		}
		return true;
	}
	/// <summary>Parses a directory listing.</summary>
	/// <param name="DirList">The directory listing to parse.</param>
	// permissions   number   owner   group   filesize   month   day   hour/year   name
	private void ParseDirList(string DirList) {
		string [] Lines = DirList.Split('\n');
		string [] Items;
		int Cnt;
		DateTime tempDate;
		string [] HourMin;
		string Year;
		remoteFileCount = 0;
		if (DirList == null)
			return;
		for (Cnt = 0; Cnt < Lines.Length - 1; Cnt++) {
			Items = RemoveRedundantItems(Lines[Cnt].Trim().Split(' '));
			if (Items.Length > 9) {
				Items[8] = string.Join(" ", Items, 8, Items.Length - 8);
				string [] temp1 = new string[9];
				if (Items != null)
					Array.Copy(Items, temp1, 9);
				Items = temp1;
			}
			if (Items.Length == 9) {
				FileItem [] temp2 = new FileItem [remoteFileCount + 1];
				if (remoteFiles != null)
					Array.Copy(remoteFiles, temp2, remoteFileCount);
				remoteFiles = temp2;
				remoteFiles[remoteFileCount] = new FileItem();
				remoteFiles[remoteFileCount].FilePermissions = Items[0].Substring(1);
				remoteFiles[remoteFileCount].FileOwner = Items[2];
				remoteFiles[remoteFileCount].FileGroup = Items[3];
				remoteFiles[remoteFileCount].FileSize = long.Parse(Items[4]);
				if (Items[7].IndexOf(":") >= 0) {
					HourMin = Items[7].Split(':');
					Year = "2001";
				} else {
					HourMin = new string[2];
					HourMin[0] = "0";
					HourMin[1] = "0";
					Year = Items[7];
				}
				tempDate = new DateTime(int.Parse(Year), MonthToNumber(Items[5]), int.Parse(Items[6]), int.Parse(HourMin[0]), int.Parse(HourMin[1]), 0);
				remoteFiles[remoteFileCount].FileDate = tempDate;
				remoteFiles[remoteFileCount].FileTitle = Items[8];
				remoteFiles[remoteFileCount].FilePath = remoteDirectory;
				remoteFiles[remoteFileCount].IsDirectory = (Items[0].ToLower()[0] == 'd');
				remoteFileCount = remoteFileCount + 1;
			}
		}
		Array.Sort(remoteFiles);
	}
	/// <summary>Returns the number of a month.</summary>
	/// <param name="Input">The month.</param>
	/// <returns>The number of the specified month.</returns>
	private int MonthToNumber(string Input) {
		switch (Input.ToLower()) {
        		case "jan": return 1;
        		case "feb": return 2;
        		case "mar": return 3;
        		case "apr": return 4;
        		case "may": return 5;
        		case "jun": return 6;
        		case "jul": return 7;
        		case "aug": return 8;
        		case "sep": return 9;
        		case "oct": return 10;
        		case "nov": return 11;
        		case "dec": return 12;
        		default: return int.Parse(Input);
    		}
	}
	/// <summary>Removes the redundant items from an array.</summary>
	/// <param name="Input">The array with the redundant items.</param>
	/// <returns>The cleaned up array.</returns>
	private string[] RemoveRedundantItems(string [] Input) {
		int Cnt, GoodItems = 0;
		string [] ReturnArray = new string[Input.Length];
		for (Cnt = 0; Cnt < Input.Length; Cnt++) {
			if (Input[Cnt] != "") {
				ReturnArray[GoodItems] = Input[Cnt];
				GoodItems = GoodItems + 1;
			}
		}
		string [] temp1 = new string[GoodItems];
		Array.Copy(ReturnArray, temp1, GoodItems);
		return temp1;
	}
	/// <summary>Retrieves the directory listing.</summary>
	/// <returns>The remote directory listing.</returns>
	public FileItem[] GetDirectoryListing() {
		return remoteFiles;
	}
	/// <summary>Retrieves the number of remote files and directories.</summary>
	/// <returns>The number of remote files and directories.</returns>
	public int GetDirectoryListCount() {
		return remoteFileCount;
	}
	// Two methods that can be used by you
	private void DataReceived(int BytesLength) {
		/* Do Nothing
		   If you want to be notified when the dataSocket receives some bytes,
		   add your handler here
		*/
	}
	private void DataSent(int BytesLength) {
		/* Do Nothing
		   If you want to be notified when the dataSocket sends some bytes,
		   add your handler here
		*/
	}
	// Private variables
	private Socket clientSocket;
	private int remotePort = 21;
	private string remoteAddress = "";
	private string logonName = "anonymous";
	private string logonPass = "user@";
	private ASCIIEncoding ASCII = new ASCIIEncoding();
	private bool isNowConnected = false;
	private bool passiveTransfers = false;
	private short lastResponseNumber = 0;
	private short lastResponseType = 0;
	private string lastResponse = "";
	private string remoteDirectory = "";
	private DataConnection dataSocket;
	private IPEndPoint passiveEndPoint;
	private FileItem[] remoteFiles;
	private int remoteFileCount = 0;
}