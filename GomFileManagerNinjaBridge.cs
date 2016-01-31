using System;
using System.ComponentModel;
using NinjaTrader.Indicator;

namespace Gom
{
	abstract partial class FileManager : IDataManager, IDisposable
	{ 
		public void Initialize(string instrName, bool writeData, GomRecorderIndicator indy)
		{
			freereader();
			
			_tickSize = indy.BarsArray[0].Instrument.MasterInstrument.TickSize;
			_InstrName = instrName;
			_writeData = writeData;
			
			if (IsWritable)
				_fileMode = indy.FileMode;
			else
				_fileMode = Gom.FileModeType.SingleFile;

            _useMillisec = indy.useMillisec;
		}
	}
	
}


namespace NinjaTrader.Indicator
{
	public partial class GomRecorderIndicator : Indicator
	{
		private Gom.FileModeType fileMode = Gom.FileModeType.OnePerDay;
		public bool useMillisec=true;
		
		[Description("Recording mode : 1 big file or split per day")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Rec:Recording Mode")]
		[Gom.SpecificTo("Binary", "Flat", "Short", "Millisec")]
		[Browsable(true)]
		public Gom.FileModeType FileMode
		{
			get { return fileMode; }
			set { fileMode = value; }
		}
	}
	
}


