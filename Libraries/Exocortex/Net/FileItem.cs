/*
    FileItem class
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

/// <summary>Represents a file from the local or a remote system.</summary>
public class FileItem : IComparable {
	/// <summary>Constructs a new FileItem object.</summary>
	public FileItem() {}
	/// <summary>Constructs a new FileItem object.</summary>
	/// <param name="ObjectName">Specifies the full path of the object in question.</param>
	/// <param name="IsDirectory">Specifies whether the specified object is a directory or a file.</param>
	/// <exceptions cref="ArgumentNullException">Thrown when the specifed Filename is Nothing (C#, VC++: null)</exceptions>
	/// <exceptions cref="ArgumentException">Thrown when there was an error querying the information of the specified object.</exceptions>
	public FileItem(string ObjectName, bool IsDirectory) {
		if (ObjectName == null) 
			throw new ArgumentNullException();
		m_IsDirectory = IsDirectory;
		if (m_IsDirectory) {
			try {
				DirectoryInfo di = new DirectoryInfo(ObjectName);
				m_FileTitle = di.Name;
				m_FileSize = 0;
				m_FilePath = di.FullName;
				m_FilePath = m_FilePath.Substring(0, m_FilePath.Length - m_FileTitle.Length);
				m_FileDate = di.LastWriteTime;
			} catch {
				throw new ArgumentException();
			}
		} else {
			try {
				FileInfo fi = new FileInfo(ObjectName);
				m_FileTitle = fi.Name;
				m_FileSize = fi.Length;
				m_FilePath = fi.DirectoryName;
				m_FileDate = fi.LastWriteTime;
			} catch {
				throw new ArgumentException();
			}
		}
	}
	/// <summary>Specifies the path of the object.</summary>
	/// <value>A String that specifies the path of the object.</value>
	/// <exceptions cref="ArgumentNullException">Thrown when the specified value is Nothing (C#, VC++: null).</exceptions>
	public string FilePath {
		get {
			return m_FilePath;
		}
		set {
			if (value == null)
				throw new ArgumentNullException();
			m_FilePath = value;
		}
	}
	/// <summary>Specifies whether the object is directory or not.</summary>
	/// <value>A Boolean that specifies whether the object is a directory or not.</value>
	public bool IsDirectory {
		get {
			return m_IsDirectory;
		}
		set {
			m_IsDirectory = value;
		}
	}
	/// <summary>Specifies the date of the object.</summary>
	/// <value>A DateTime object that specifies the date of the object.</value>
	public DateTime FileDate {
		get {
			return m_FileDate;
		}
		set {
			m_FileDate = value;
		}
	}
	/// <summary>Specifies the permissions of the file.</summary>
	/// <value>A String that specifies the permissions for the file.</value>
	/// <exceptions cref="ArgumentNullException">Thrown when the specified value is Nothing (C#, VC++: null).</exceptions>
	public string FilePermissions {
		get {
			return m_FilePermissions;
		}
		set {
			if (value == null)
				throw new ArgumentNullException();
			m_FilePermissions = value;
		}
	}
	/// <summary>Specifies the group of users the file belongs to.</summary>
	/// <value>A String that specifies the group of users the file belongs to.</value>
	/// <exceptions cref="ArgumentNullException">Thrown when the specified value is Nothing (C#, VC++: null).</exceptions>
	public string FileGroup {
		get {
			return m_FileGroup;
		}
		set {
			if (value == null)
				throw new ArgumentNullException();
			m_FileGroup = value;
		}
	}
	/// <summary>Specifies the owner of the file.</summary>
	/// <value>A String that specifies the owner of the file.</value>
	/// <exceptions cref="ArgumentNullException">Thrown when the specified value is Nothing (C#, VC++: null).</exceptions>
	public string FileOwner {
		get {
			return m_FileOwner;
		}
		set {
			if (value == null)
				throw new ArgumentNullException();
			m_FileOwner = value;
		}
	}
	/// <summary>Specifies the size of the file.</summary>
	/// <value>A Long that specifies the size of the file.</value>
	public long FileSize {
		get {
			return m_FileSize;
		}
		set {
			m_FileSize = value;
		}
	}
	/// <summary>Specifies the title of the file.</summary>
	/// <value>A String that specifies the title of the file.</value>
	/// <exceptions cref="ArgumentNullException">Thrown when the specified value is Nothing (C#, VC++: null).</exceptions>
	public string FileTitle {
		get {
			return m_FileTitle;
		}
		set {
			if (value == null)
				throw new ArgumentNullException();
			m_FileTitle = value;
		}
	}
	/// <summary>Compares this object to another FileItem object.</summary>
	/// <returns>Returns 1 if the passed FileItem object should be placed above this FileItem, -1 if the passed FileItem should be placed below this FileItem and 0 if it is the same.</returns>
	public int CompareTo(object obj) {
		if (obj == null)
			return -1;
		FileItem ct = (FileItem)obj;
		if (m_IsDirectory && !ct.IsDirectory) {
			return -1;
		} else if (!m_IsDirectory && ct.IsDirectory) {
			return 1;
		} else if (string.Compare(m_FileTitle.ToLower(), ct.FileTitle.ToLower()) > 0) {
			return 1;
		} else if (string.Compare(m_FileTitle.ToLower(), ct.FileTitle.ToLower()) < 0) {
			return -1;
		} else {
			return 0;
		}
	}
	// Private Variables
	private string m_FileTitle = "";
	private long m_FileSize = 0;
	private string m_FileOwner = "";
	private string m_FileGroup = "";
	private string m_FilePermissions = "";
	private DateTime m_FileDate;
	private bool m_IsDirectory;
	private string m_FilePath = "";
}