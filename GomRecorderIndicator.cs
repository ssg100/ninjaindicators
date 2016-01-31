#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Data;
using NinjaTrader.Cbi;
using NinjaTrader.Indicator;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
#endregion

namespace NinjaTrader.Indicator
{	
	/// <summary>
    /// Gom Recorder Indicator
    /// </summary>
    [Description("Base Recorder indicator class. Can be instantiated or not")]
	public partial class GomRecorderIndicator : Indicator
	{
		#region Variables

		private double ask = Double.MinValue, bid = Double.MaxValue;
		private bool initBidAsk;
		private bool foundbarinfile;
		private bool EOFfound;
		private int MaxVolume = Int32.MaxValue;
		private int MaxTick = Int32.MaxValue;
		private long BeginTimeTicks, EndTimeTicks;

		protected string recordingMessage { get { return GetRecordingMessage(); } }

		private int curBarVol;
		private int curBarTicks;
		private int volOverflow;

		private int timeoffset;

		private Queue<Gom.MarketDataType> tickQueue = new Queue<Gom.MarketDataType>();

		private Gom.IDataManager fm;
		private Gom.MarketDataType GMktData;
		private long GMktDataTimeTicks;
		private bool GFetchOK;

		private int Gvol2send;

		private bool splitVolume = true;
		private bool disableTime;
		private bool writeData;
		private bool writeOK = true;

		private string fileFormat;
		private int iDataManager = -1;

		private bool writable;
		private bool millisecCompliant;
		
		private bool firstRec;
		
		private double priceoffset=0.0f;
		
		private DateTime curcm=new DateTime(0L);
		
		private bool dataManagerNotDisposedOf=true;

		#endregion

		//--------------- HELPERS
		#region Helpers

		#region Time Calculations
		private void UpdateBeginEndTime()
		{
			DateTime BeginTime, EndTime, time;

			if (GMktDataTimeTicks > EndTimeTicks)
			{
				time = new DateTime(GMktDataTimeTicks);
				Bars.Session.GetNextBeginEnd(time, out BeginTime, out EndTime);
			}
			else
			{
				time = new DateTime(GMktDataTimeTicks);
				Bars.Session.GetNextBeginEnd(time.AddSeconds(1), out BeginTime, out EndTime);
			}

			BeginTimeTicks = BeginTime.Ticks;
			EndTimeTicks = EndTime.Ticks;
		}

		private void ComputeTimeOffset(DateTime tickTime)
		{
			DateTime t = DateTime.Now;

			DateTime correctedtime = t.AddMilliseconds(timeoffset);
			TimeSpan diff = (correctedtime - tickTime);

			if (diff.Seconds > 0)
			{
				if (diff.Seconds > 1)
					timeoffset -= (diff.Seconds - 1) * 1000;

				timeoffset -= (diff.Milliseconds + 1);
			}

			if (diff.Ticks < 0L)
			{
				timeoffset -= (diff.Seconds * 1000 + diff.Milliseconds);
				if (t.AddMilliseconds(timeoffset) < tickTime)
					timeoffset++;
			}
		}
		#endregion

		#region Dynamic PropertyGrid manager
		protected void ChangeProvider()
		{
			fileFormat = Gom.DataManagerList.Name[iDataManager];

			writable = Gom.DataManagerList.Writable[iDataManager];
			if (!writable)
				writeData = false;

            millisecCompliant = Gom.DataManagerList.MillisecCompliant[iDataManager];
            useMillisec = millisecCompliant;



			//custom properties
			var props = from p in typeof(GomRecorderIndicator).GetProperties()
						where p.GetCustomAttributes(typeof(Gom.SpecificTo), false).Length > 0
						select new { PropertyName = p.Name, RecorderName = ((Gom.SpecificTo)(p.GetCustomAttributes(typeof(Gom.SpecificTo), false)[0])).Name };

			foreach (var p in props)
			{
				PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())[p.PropertyName];

				bool browsableFound = false;

				foreach (var att in descriptor.Attributes)
				{
					if (att.GetType() == typeof(BrowsableAttribute))
					{
						browsableFound = true;
						break;
					}
				}

				if (browsableFound)
				{
					BrowsableAttribute browsable = (BrowsableAttribute)descriptor.Attributes[typeof(BrowsableAttribute)];
					FieldInfo isBrowsable = browsable.GetType().GetField("browsable", BindingFlags.NonPublic | BindingFlags.Instance);

					if (!p.RecorderName.Contains(fileFormat))
						isBrowsable.SetValue(browsable, false);

					else
						isBrowsable.SetValue(browsable, true);
				}
			}

			//writeData only if writable
			PropertyDescriptor descriptorwd = TypeDescriptor.GetProperties(this.GetType())["WriteData"];
			ReadOnlyAttribute roattrib = (ReadOnlyAttribute)descriptorwd.Attributes[typeof(ReadOnlyAttribute)];
			FieldInfo isReadOnly = roattrib.GetType().GetField("isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);

			if (!writable)
			{
				WriteData = false;
				isReadOnly.SetValue(roattrib, true);
			}
			else
				isReadOnly.SetValue(roattrib, false);

			//usemillisec only if millisec compliant
			PropertyDescriptor descriptorum = TypeDescriptor.GetProperties(this.GetType())["UseMillisec"];
			BrowsableAttribute browsableum = (BrowsableAttribute)descriptorum.Attributes[typeof(BrowsableAttribute)];
			FieldInfo fiUseMillisec = browsableum.GetType().GetField("browsable", BindingFlags.NonPublic | BindingFlags.Instance);

			if (millisecCompliant)
				fiUseMillisec.SetValue(browsableum, true);
			else
				fiUseMillisec.SetValue(browsableum, false);
		}

		#endregion

		#region Tick Management
		
		private void SendAll()
		{
			//basic event handler
			//jit will (should) remove empty methods
			if(!Historical)
				GMktData.IsNewTimeStamp=Gom.MarketDataType.TimeStampStatus.Unknown;
			
			GomOnMarketDataWithTime(GMktData.Time, GMktData.TickType, GMktData.Price, Gvol2send, FirstTickOfBar);
			GomOnMarketData(GMktData.TickType, GMktData.Price, Gvol2send, FirstTickOfBar);
			GomOnMarketData(GMktData.TickType, GMktData.Price, Gvol2send);	
			GomOnMarketData(GMktData);		
		}
		

		private void SendMarketData()
		{
			if (Gvol2send != GMktData.Volume)
			{
				int volold = GMktData.Volume;
				GMktData.Volume = Gvol2send;
				SendAll();
				GMktData.Volume = volold - Gvol2send;
			}
			else
			{
				SendAll();
				GMktData.Volume = 0;
			}

			curBarTicks++;
			curBarVol += Gvol2send;
		}

		private void SetCursorTime(DateTime time0)
		{
			fm.SetCursorTime(time0, ref GMktData);
			GMktDataTimeTicks = GMktData.Time.Ticks;
			GFetchOK = (GMktDataTimeTicks != 0L);
		}

		private void GetNextTick()
		{
			fm.GetNextTick(ref GMktData);
			GMktData.Price+=priceoffset;
			GMktDataTimeTicks = GMktData.Time.Ticks;
			GFetchOK = (GMktDataTimeTicks != 0L);
		}

		private void OnBarUpdateHistorical()
		{
			double HighPrice = High[0];
			double LowPrice = Low[0];

			DateTime time0 = Time[0];
			long roundeddticks, time0ticks;

			curBarTicks = 0;
			GFetchOK = false;

			if (volOverflow > MaxVolume)
			{
				curBarVol = MaxVolume;
				volOverflow -= MaxVolume;
			}
			else
			{
				curBarVol = volOverflow;
				volOverflow = 0;
			}

			if ((BarsPeriod.Id == PeriodType.Second && BarsPeriod.Value > 1) || BarsPeriod.Id == PeriodType.Minute)
				time0 = time0.AddSeconds(-1);

			time0ticks = time0.Ticks;

			if (!EOFfound)
			{
				do
				{
					if (!foundbarinfile) 	// first iteration : position cursor in file
					{			
						DateTime begdate, dummydate;
						Bars.Session.GetNextBeginEnd(Time[0],out begdate,out dummydate);
						SetCursorTime(begdate);
						foundbarinfile = true;
					}
					else if (GMktData.Volume > 0)
						GFetchOK = true;
					else
					{
						do
						{
							GetNextTick();

							if ((GMktDataTimeTicks >= EndTimeTicks) && GFetchOK)
								UpdateBeginEndTime();

						} while (!disableTime && ((GMktDataTimeTicks < BeginTimeTicks) || (GMktDataTimeTicks >= EndTimeTicks)) && GFetchOK);
					}


					if (GFetchOK)
					{
						Gvol2send = GMktData.Volume;

						roundeddticks = GMktDataTimeTicks;

						if (millisecCompliant)
							roundeddticks = GMktData.Time.AddMilliseconds(-GMktData.Time.Millisecond).Ticks;

						if (roundeddticks < time0ticks)
							SendMarketData();

						else if ((roundeddticks == time0ticks) && (curBarTicks < MaxTick) &&
								(GMktData.Price <= HighPrice) && (GMktData.Price >= LowPrice) && (curBarVol < MaxVolume))
						{
							if ((!splitVolume) || (BarsPeriod.Id != PeriodType.Volume))
							{
								SendMarketData();
								volOverflow = Math.Max(0, curBarVol - MaxVolume);
							}
							else
							{
								Gvol2send = Math.Min(GMktData.Volume, Math.Max((MaxVolume - curBarVol), 0));
								SendMarketData();
							}
						}
					}
				} while ((GMktData.Volume == 0) && GFetchOK);


				if (!GFetchOK)
					EOFfound = true;
			}
		}

		private void OnBarUpdateRealTime()
		{
			if (FirstTickOfBar)
				curBarVol = 0;

			//now we empty the tick queue used in OnMarketData
			int queueCount = tickQueue.Count;

			// if COBC=true then last tick belongs to next bar
			if (CalculateOnBarClose)
				queueCount--;

			Gom.MarketDataType tcTemp;

			for (int i = 0; i < queueCount; i++)
			{
				GMktData = tickQueue.Dequeue();

				//not the same process if we have to split volume or not.
				//if we don't, we send all volume
				//we only split on volume chart, if volume is too high, if it is the last tick of the queue and if we asked for splitting

				if ((BarsPeriod.Id != PeriodType.Volume) || ((curBarVol + GMktData.Volume) <= MaxVolume) || (i < (queueCount - 1)) || !splitVolume)
				{
					Gvol2send = GMktData.Volume;
					SendMarketData();
				}
				else
				{
					//split volume 						
					Gvol2send = Math.Max(MaxVolume - curBarVol, 0);
					SendMarketData();//Math.Max(MaxVolume - curBarVol, 0), FirstTickOfBar);

					//requeue remaining volume
					//if COBC=true we have to remove the last tick or we will have an ordering problem
					if (CalculateOnBarClose)
					{
						tcTemp = tickQueue.Dequeue();
						tickQueue.Enqueue(GMktData);
						tickQueue.Enqueue(tcTemp);
					}
					else
						tickQueue.Enqueue(GMktData);
				}
			}
		}
		
		private void RolloverContract()
		{
			string instrname;
			
			bool rollover=false;
			
			DateTime zonedate=TimeZoneInfo.ConvertTime(Time[0],Bars.Session.TimeZoneInfo);
			
			MasterInstrument MI=Bars.Instrument.MasterInstrument;
	
			IEnumerable<RollOver> ROCollection=MI.RollOverCollection.Cast<RollOver>();
			
			DateTime cm = ROCollection.Where(x=>x.Date<=zonedate.Date).OrderByDescending(x=>x.Date).First().ContractMonth;
					
			if (cm != curcm)
			{		
				rollover=true;
				curcm=cm;
			}
			
			if (rollover)
			{ 	
				priceoffset=0.0f;
				if (MI.MergePolicy == MergePolicy.MergeBackAdjusted)
			    	priceoffset=ROCollection.Where(x=>(x.ContractMonth>cm) && (x.ContractMonth<=Instrument.Expiry)&&!(double.IsNaN(x.Offset))).Sum(x=>x.Offset);
				
				foundbarinfile=false;
				EOFfound=false;
					
				instrname=MI.Name+" "+cm.Month.ToString("D2")+"-"+(cm.Year%100).ToString("D2");
				fm.Initialize(instrname, writeData, this);
			}

		}
		
		#endregion

		private string GetRecordingMessage()
		{
			string recmes;

			if (writeData)
				recmes = "Recording " + fileFormat + " " + ((!firstRec)?"NotNeeded":(writeOK) ? "OK" : "KO");
			else
				recmes = "Using " + fileFormat;

			if (useMillisec)
				recmes += " - Lag=" + (-timeoffset) + " ms";

			return recmes;
		}

		#endregion


		//--------------------Ninja methods. Sealed to avoid override

		protected override sealed void Initialize()
		{
			CalculateOnBarClose = false;
			BarsRequired=0;
			GomInitialize();

			if (iDataManager == -1)
			{
				iDataManager = Gom.DataManagerList.Name.IndexOf("Binary");

				if (iDataManager == -1)
					if (Gom.DataManagerList.Name.Count > 0)
						iDataManager = 0;

				if (iDataManager > -1)
					FileFormat = Gom.DataManagerList.Name[iDataManager];
			}

			#region Automatically add browsable attribute =>framework bug
			// Add Browsable attribute to custom properties
			/*
			   var props = from p in typeof(GomRecorderIndicator).GetProperties()
						   where p.GetCustomAttributes(typeof(Gom.SpecificTo), false).Length > 0
						   select p;

			   foreach (var p in props)
			   {
				   PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())[p.Name];

				   bool browsableFound = false;

				   foreach(var att in descriptor.Attributes)
				   {
					   if (att.GetType() == typeof(BrowsableAttribute))
					   {
						   browsableFound = true;
						   break;
					   }
				   }

				   if (!browsableFound)
				   {
					   BrowsableAttribute browsable = (BrowsableAttribute)descriptor.Attributes[typeof(BrowsableAttribute)];
					   System.ComponentModel.TypeDescriptor.AddAttributes(descriptor.PropertyType,browsable);

				   }
			   }
		   */
			#endregion
		}

		protected override sealed void OnStartUp()
		{			
			if (BarsPeriod.Id == PeriodType.Volume)
				MaxVolume = BarsPeriod.Value;

			if (BarsPeriod.Id == PeriodType.Tick)
				MaxTick = BarsPeriod.Value;

			fm = (Gom.IDataManager)Activator.CreateInstance(Gom.DataManagerList.Type[iDataManager]);
			fm.Initialize(BarsArray[0].Instrument.FullName, writeData, this);
			
			GomOnStartUp();
		}


		
		protected override sealed void OnBarUpdate()
		{
			GomOnBarUpdate();
			
			if(BarsInProgress==0)
			{
				if (Historical) 
				{
					MasterInstrument MI=Bars.Instrument.MasterInstrument;
					if ((Bars.FirstBarOfSession||(BarsPeriod.Id == PeriodType.Day)) && (MI.InstrumentType==InstrumentType.Future) && (MI.MergePolicy != MergePolicy.DoNotMerge)&& !writeData && !Bars.Instrument.FullName.Contains("##-##"))					
						RolloverContract();

					OnBarUpdateHistorical();
				}
				
				else
				{
					if (dataManagerNotDisposedOf)
					{
						if (!writeData)
						{
							fm.Dispose();
							fm=null;
							dataManagerNotDisposedOf=false;
						}
						
					 }
					
					OnBarUpdateRealTime();
				}

				GomOnBarUpdateDone();
			}
		}

		protected override sealed void OnMarketData(MarketDataEventArgs e)
		{
			DateTime t;

			if (useMillisec)
			{
				ComputeTimeOffset(e.Time);
				t = DateTime.Now.AddMilliseconds(timeoffset);
			}
			else
				t = e.Time;
			
			if (!initBidAsk)
				initBidAsk = (ask > bid);

			if ((e.MarketDataType == MarketDataType.Last))
			{
				GMktDataTimeTicks = e.Time.Ticks;

				if (writeData && writeOK && writable && initBidAsk)
				{
					writeOK = fm.RecordTick(t, bid, ask, e.Price, (int)e.Volume);
					firstRec=true;
				}

				UpdateBeginEndTime();

				if (disableTime || ((GMktDataTimeTicks >= BeginTimeTicks) && (GMktDataTimeTicks < EndTimeTicks)))
					tickQueue.Enqueue(new Gom.MarketDataType(t, Gom.Utils.GetIntTickType(bid, ask, e.Price), e.Price, (int)e.Volume));
			}

			else if (e.MarketDataType == MarketDataType.Ask)
				ask = e.Price;

			else if (e.MarketDataType == MarketDataType.Bid)
				bid = e.Price;

		}

		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			base.Plot(graphics, bounds, min, max);

			Color ColorNeutral = Color.FromArgb(255, 255 - ChartControl.BackColor.R, 255 - ChartControl.BackColor.G, 255 - ChartControl.BackColor.B);

			using (SolidBrush brush = new SolidBrush(ColorNeutral))
			using (StringFormat SF = new StringFormat())
				graphics.DrawString(recordingMessage, ChartControl.Font, brush, bounds.Left, bounds.Bottom - 22, SF);

		}

		protected override sealed void OnTermination()
		{
			GomOnTermination();

			if (fm != null)
			{
				fm.Dispose();
				fm = null;
			}
		}

		//-------------Exposed to the client

		protected virtual void GomInitialize()
		{ }

		protected virtual void GomOnStartUp()
		{ }

		protected virtual void GomOnBarUpdate()
		{ }

		protected virtual void GomOnTermination()
		{ }

		protected virtual void GomOnBarUpdateDone()
		{ }

		protected virtual void GomOnMarketData(Gom.MarketDataType e)
		{ }


		//LEGACY METHODS : DO NOT USE
		#region Legacy Methods
		protected virtual void GomOnMarketData(TickTypeEnum tickType, double price, int volume)
		{ }

		protected virtual void GomOnMarketData(TickTypeEnum tickType, double price, int volume, bool firstTickOfBar)
		{ }

		protected virtual void GomOnMarketDataWithTime(DateTime tickTime, TickTypeEnum tickType, double price, int volume, bool firstTickOfBar)
		{ }
		#endregion


		#region Properties
		[Description("Recording Format")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Rec:Format")]
        [RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(Gom.GomDataManagerConverter))]
		public string FileFormat
		{
			get
			{
				ChangeProvider();
				return fileFormat;
			}
			set
			{
				iDataManager = Gom.DataManagerList.Name.IndexOf(value);
				ChangeProvider();
			}
		}

		[Description("Write Data")]
		[Category("Settings : Recorder")]
		[Gui.Design.DisplayName("Write Data")]
		[ReadOnly(true)]
		[Browsable(true)]
		public bool WriteData
		{
			get { return writeData; }
			set
			{
				if (writable)
					writeData = value;
				else
					writeData = false;
			}
		}

		[Description("Disable Tick Time Filtering")]
		[Category("Settings : Recorder")]
		[Gui.Design.DisplayName("Disable Time Filter")]
		public bool DisableTime
		{
			get { return disableTime; }
			set { disableTime = value; }
		}

		[Description("Split Volume on constant volume bars")]
		[Category("Settings : Recorder")]
		[Gui.Design.DisplayName("Split Volume")]
		public bool SplitVolume
		{
			get { return splitVolume; }
			set { splitVolume = value; }
		}

		[Description("Use Millisec Mode (Time[0] is replaced by Now")]
		[Category("Settings : Recorder")]
		[Gui.Design.DisplayName("Use Millisec")]
		[Browsable(true)]
		public bool UseMillisec
		{
			get { return useMillisec; }
			set
			{
				if (millisecCompliant)
					useMillisec = value;
				else
					useMillisec = false;
			}
		}

		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private GomRecorderIndicator[] cacheGomRecorderIndicator = null;

        private static GomRecorderIndicator checkGomRecorderIndicator = new GomRecorderIndicator();

        /// <summary>
        /// Base Recorder indicator class. Can be instantiated or not
        /// </summary>
        /// <returns></returns>
        public GomRecorderIndicator GomRecorderIndicator(string fileFormat)
        {
            return GomRecorderIndicator(Input, fileFormat);
        }

        /// <summary>
        /// Base Recorder indicator class. Can be instantiated or not
        /// </summary>
        /// <returns></returns>
        public GomRecorderIndicator GomRecorderIndicator(Data.IDataSeries input, string fileFormat)
        {
            if (cacheGomRecorderIndicator != null)
                for (int idx = 0; idx < cacheGomRecorderIndicator.Length; idx++)
                    if (cacheGomRecorderIndicator[idx].FileFormat == fileFormat && cacheGomRecorderIndicator[idx].EqualsInput(input))
                        return cacheGomRecorderIndicator[idx];

            lock (checkGomRecorderIndicator)
            {
                checkGomRecorderIndicator.FileFormat = fileFormat;
                fileFormat = checkGomRecorderIndicator.FileFormat;

                if (cacheGomRecorderIndicator != null)
                    for (int idx = 0; idx < cacheGomRecorderIndicator.Length; idx++)
                        if (cacheGomRecorderIndicator[idx].FileFormat == fileFormat && cacheGomRecorderIndicator[idx].EqualsInput(input))
                            return cacheGomRecorderIndicator[idx];

                GomRecorderIndicator indicator = new GomRecorderIndicator();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FileFormat = fileFormat;
                Indicators.Add(indicator);
                indicator.SetUp();

                GomRecorderIndicator[] tmp = new GomRecorderIndicator[cacheGomRecorderIndicator == null ? 1 : cacheGomRecorderIndicator.Length + 1];
                if (cacheGomRecorderIndicator != null)
                    cacheGomRecorderIndicator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGomRecorderIndicator = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Base Recorder indicator class. Can be instantiated or not
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomRecorderIndicator GomRecorderIndicator(string fileFormat)
        {
            return _indicator.GomRecorderIndicator(Input, fileFormat);
        }

        /// <summary>
        /// Base Recorder indicator class. Can be instantiated or not
        /// </summary>
        /// <returns></returns>
        public Indicator.GomRecorderIndicator GomRecorderIndicator(Data.IDataSeries input, string fileFormat)
        {
            return _indicator.GomRecorderIndicator(input, fileFormat);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Base Recorder indicator class. Can be instantiated or not
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomRecorderIndicator GomRecorderIndicator(string fileFormat)
        {
            return _indicator.GomRecorderIndicator(Input, fileFormat);
        }

        /// <summary>
        /// Base Recorder indicator class. Can be instantiated or not
        /// </summary>
        /// <returns></returns>
        public Indicator.GomRecorderIndicator GomRecorderIndicator(Data.IDataSeries input, string fileFormat)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GomRecorderIndicator(input, fileFormat);
        }
    }
}
#endregion
