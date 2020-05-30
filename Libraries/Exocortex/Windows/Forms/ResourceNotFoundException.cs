using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace Exocortex.Windows.Forms {

	public class ResourceNotFoundException : ApplicationException {
		public ResourceNotFoundException( string msg ) : base( msg ) {}
	}

}
