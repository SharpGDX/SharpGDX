using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	public interface IDisableable
	{
		public void setDisabled(bool isDisabled);

		public bool isDisabled();
	}
}
