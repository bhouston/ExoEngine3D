/*
    Test Application for the FtpConnection class
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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;

public class TestApp {
	private static FtpConnection Client;
	static void Download(string [] Input) {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		string SaveFile;
		if (Input == null) {
			Console.WriteLine("Invalid parameters");
			return;
		}
		if (Input.Length > 1)
			Input[0] = string.Join(" ", Input);
		Console.Write("Save file to: ");
		SaveFile = Console.ReadLine();
		Client.DownloadFile(Input[0], SaveFile);
	}
	static void Upload(string [] Input) {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		string SaveFile;
		if (Input == null) {
			Console.WriteLine("Invalid parameters");
			return;
		}
		if (Input.Length > 1)
			Input[0] = string.Join(" ", Input);
		Console.Write("Save on server as: ");
		SaveFile = Console.ReadLine();
		Client.UploadFile(Input[0], SaveFile, FtpConnection.StreamModes.Binary);
	}
	static void Custom(string [] Input) {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		if (Input == null) {
			Console.WriteLine("Invalid parameters");
			return;
		}
		if (Input.Length > 1)
			Input[0] = string.Join(" ", Input);
		Client.CustomCommand(Input[0]);
	}
	static void ChMod(string [] Input) {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		if (Input == null || Input.Length != 2) {
			Console.WriteLine("Invalid parameters");
			return;
		}
		Client.ChMod(Input[0], Input[1]);
	}
	static void Rename(string [] Input) {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		string RenameTo;
		if (Input.Length > 1)
			Input[0] = String.Join(" ", Input);
		Console.Write("Rename file to: ");
		RenameTo = Console.ReadLine();
		Client.Rename(Input[0], RenameTo);
	}
	static void RemoveDir(string [] Input) {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		if (Input == null) {
			Console.WriteLine("Invalid parameters");
			return;
		}
		Client.RemoveDir(Input[0]);
	}
	static void MakeDir(string [] Input) {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		if (Input == null) {
			Console.WriteLine("Invalid parameters");
			return;
		}
		Client.MakeDir(Input[0]);
	}
	static void Delete(string [] Input) {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		if (Input == null) {
			Console.WriteLine("Invalid parameters");
			return;
		}
		Client.Delete(Input[0]);
	}
	static void Status() {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		Client.Status();
	}
	static void SysInfo() {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		Client.GetSystemInfo();
	}
	static void ToUpDir() {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		Client.ChangeToUpDir();
	}
	static void Help() {
		Console.WriteLine("List of all available commands:");
		Console.WriteLine("  cd ([dir])");
		Console.WriteLine("  chmod [file] [mode]");
		Console.WriteLine("  custom [custom command]");
		Console.WriteLine("  del [file]");
		Console.WriteLine("  disconnect");
		Console.WriteLine("  download [remote file]");
		Console.WriteLine("  help");
		Console.WriteLine("  md [dir]");
		Console.WriteLine("  mkdir [dir]");
		Console.WriteLine("  open [IP/hostname] ([port]) (passive)");
		Console.WriteLine("  rd [dir]");
		Console.WriteLine("  ren [file]");
		Console.WriteLine("  status");
		Console.WriteLine("  sysinfo");
		Console.WriteLine("  upload [local file]");
	}
	static void Open(string [] Input) {
		if (Input == null) {
			Console.WriteLine("Invalid parameters");
			return;
		}
		string User , Pass;
		Console.Write("login: ");
		User = Console.ReadLine();
		if (User == "") {
			User = "Anonymous";
			Pass = "no@email.com";
		} else {
			Console.Write("password: ");
			Pass = Console.ReadLine();
			if (Pass == "")
				Pass = "no@email.com";
		}
		Client.Username = User;
		Client.Password = Pass;
		if (Input.Length >= 1)
			Client.Hostname = Input[0];
		else
			Client.Hostname = "localhost";
		if (Input.Length >= 2)
			Client.Port = int.Parse(Input[1]);
		else
			Client.Port = 21;
		if (Input.Length >= 3) {
			if (Input[2].ToLower() == "passive")
				Client.Passive = true;
			else
				Client.Passive = false;
		}
		Client.Disconnect();
		Client.WorkDir = "";
		Client.Connect();
	}
	static void Dir(string [] Input) {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		int Cnt;
		FileItem [] FileList;
		Client.GetList();
		FileList = Client.GetDirectoryListing();
		Console.WriteLine("Directory of " + Client.WorkDir + "\r\n");
		for (Cnt = 0; Cnt < Client.GetDirectoryListCount(); Cnt++) {
			Console.Write(FileList[Cnt].FileDate.ToString("yyyy/MM/dd") + "  " + FileList[Cnt].FileDate.ToString("hh:mm"));
			if (FileList[Cnt].IsDirectory)
				Console.Write("        <DIR>   ");
			else
				Console.Write(LongToString(FileList[Cnt].FileSize, 14) + "  ");
			Console.WriteLine(FileList[Cnt].FileTitle);
		}
		Console.WriteLine("");
	}
	static void Cd(string [] Input) {
		if (!Client.IsConnected) {
			Console.WriteLine("Please connect to a server first.");
			return;
		}
		if (Input != null && Input.Length > 1)
			Input[0] = string.Join(" ", Input);
		if (Input != null && Input.Length >= 1) {
			if (Input[0] == "..") {
				ToUpDir();
			} else {
				Client.ChangeWorkDir(Input[0]);
			}
		} else
			Client.PrintWorkDir();
	}
	static void Main() {
		string Input, Command;
		string [] Parameters;
		Console.WriteLine("\r\n     Welcome to NetFTP!\r\n     ------------------");
		Client = new FtpConnection();
		Client.ReceivedReply += new FtpConnection.ReceivedReplyHandler(ReceivedReply);
		Client.Connected += new FtpConnection.ConnectedHandler(Connected);
		Client.Disconnected += new FtpConnection.DisconnectedHandler(Disconnected);
		Client.ConnectionFailed += new FtpConnection.ConnectionFailedHandler(ConnectionFailed);
		Client.CommandSent += new FtpConnection.CommandSentHandler(CommandSent);
		Console.WriteLine("\r\n FTP Client ready [type 'help' for help (duh)]");
		Console.Write("\r\n>");
		Input = Console.ReadLine();
		while (!Input.ToLower().Equals("exit")) {
			Parameters = null;
			Input = Input.Trim();
			if (Input.IndexOf(' ') >= 0) {
				Command = Input.Substring(0, Input.IndexOf(' ')).ToLower();
				Parameters = RemoveRedundantItems(Input.Substring(Input.IndexOf(' ') + 1).Split(' '));
			} else 
				Command = Input.ToLower();
			switch (Command) {
				case "help":
					Help();
					break;
				case "open":
					Open(Parameters);
					break;
				case "dir":
					Dir(Parameters);
					break;
				case "cd":
					Cd(Parameters);
					break;
				case "cd..":
					ToUpDir();
					break;
				case "status":
					Status();
					break;
				case "sysinfo":
					SysInfo();
					break;
				case "del":
					Delete(Parameters);
					break;
				case "md":
					MakeDir(Parameters);
					break;
				case "mkdir":
					MakeDir(Parameters);
					break;
				case "rd":
					RemoveDir(Parameters);
					break;
				case "ren":
					Rename(Parameters);
					break;
				case "chmod":
					ChMod(Parameters);
					break;
				case "custom":
					Custom(Parameters);
					break;
				case "upload":
					Upload(Parameters);
					break;
				case "download":
					Download(Parameters);
					break;
				case "disconnect":
					Client.Disconnect();
					break;
				case "reconnect":
					Client.Disconnect();
					Client.Connect();
					break;
				default:
					Console.WriteLine("Invalid command");
					break;
			}
			Console.Write(">");
			Input = Console.ReadLine();
		}
		Client.Disconnect();
	}
	public static void ReceivedReply(string Reply, short ReplyNumber, short ReplyType) {
		Console.Write(Reply);
	}
	public static void Connected() {
		Console.WriteLine("Connected");
	}
	public static void Disconnected() {
		Console.WriteLine("Disconnected");
	}
	public static void ConnectionFailed() {
		Console.WriteLine("Connection Failed");
	}
	public static void CommandSent(string Command) {
		Console.WriteLine(Command);
	}
	private static string[] RemoveRedundantItems(string [] Input) {
		int Cnt, GoodItems = 0;
		string [] ReturnArray = new string[Input.Length];
		for(Cnt = 0; Cnt < Input.Length; Cnt++) {
			if (Input[Cnt] != ""){
				ReturnArray[GoodItems] = Input[Cnt];
				GoodItems = GoodItems + 1;
			}
		}
		if (GoodItems > 0) {
			string [] temp1 = new string[GoodItems];
			Array.Copy(ReturnArray, temp1, GoodItems);
			return ReturnArray;
		} else
			return null;
	}
	static string LongToString(long Input, int StringLength) {
		string RetVal = Input.ToString("#,##0");
		if (RetVal.Length > StringLength)
			RetVal = RetVal.Substring(0, StringLength);
		else if(RetVal.Length < StringLength)
			RetVal = RetVal.PadLeft(StringLength);
		return RetVal;
	}
}