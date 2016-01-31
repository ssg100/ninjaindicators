using System;
using System.ComponentModel;
using NinjaTrader.Indicator;

namespace Gom
{
	partial class GomNinjaTickFileManager : IDataManager, IDisposable
	{ 
		public void Initialize(string instrName, bool writeData, GomRecorderIndicator indy)
		{
			freereader();
			
			_InstrName = instrName;

		}
	}
	
}




