#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Collections;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

#region Global Enums

public enum anaPivotStylesPW43 {Floor, Wide, Fibonacci, Jackson, Camarilla, Woodie}
public enum anaSessionTypePW43 {DailyBars, ETH, RTH, Auto, UserDefined}
public enum anaSessionCountPW43 {First, Second, Third}
public enum anaCalcModePW43 {DailyClose, IntradayClose} 
public enum anaPlotAlignPW43 {Left, Right, LeftAll, RightAll, DoNotPlot}

#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Pivot Points.
	/// </summary>
	[Description("Weekly Session Pivots.")]
	public class anaPivotsWeeklyV43: Indicator
	{
		#region Variables
		private	SolidBrush[]		brushes					= { new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black), 
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black), 
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black),  
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black),  
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black),
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black),
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black),
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black),
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black)};
		private	DateTime 			sessionBegin 			= Cbi.Globals.MinDate;
		private	DateTime 			sessionEnd 				= Cbi.Globals.MinDate;
		private	DateTime 			plotStart		 		= Cbi.Globals.MaxDate;
		private DateTime			cacheSessionBeginTmp   	= Cbi.Globals.MinDate;
		private DateTime			cacheSessionEndTmp1		= Cbi.Globals.MinDate;
		private DateTime			cacheSessionEndTmp2		= Cbi.Globals.MinDate;
		private DateTime			cacheSessionDate		= Cbi.Globals.MinDate;
		private DateTime			sessionDateTmp			= Cbi.Globals.MinDate;
		private DateTime			cacheWeeklyEndDate		= Cbi.Globals.MinDate;
		private DateTime			currentDate				= Cbi.Globals.MinDate;
		private DateTime			priorDate				= Cbi.Globals.MinDate;
		private DateTime			currentWeek				= Cbi.Globals.MinDate;
		private DateTime			lastBarTimeStamp1		= Cbi.Globals.MinDate;
		private DateTime			lastBarTimeStamp2		= Cbi.Globals.MinDate;
		private	double				currentHigh				= double.MinValue;
		private	double				currentLow				= double.MaxValue;
		private	double				currentClose			= 0.0;
		private double				high					= 0.0;
		private double				low						= 0.0;
		private double				close					= 0.0;
		private	double				volSum					= 0.0;
		private	double				volPriceSum				= 0.0;
		private	double				currentVolSum			= 0.0;
		private	double				currentVolPriceSum		= 0.0;
		private double				displaySize				= 0.0;
		private Bars				dailyBars				= null;
		private bool				tickBuilt				= false;
		private bool				showMidpivots			= false;
		private bool				showPriorPeriods		= true;
		private bool				existsHistDailyData		= false;
		private bool				isDailyDataLoaded		= false;
		private bool				plotPivots				= false;
		private bool				calcOpen				= false;
		private bool				firstDayOfPeriod		= false;
		private bool				extendPublicHoliday		= false;
		private bool				isEndOfWeekHoliday		= false; 
		private bool				noDailyData				= false;
		private bool				noCurrentDailyBar		= false;
		private bool				isCurrency				= false;
		private bool				isGlobex				= false;
		private ArrayList			newSessionBarIdxArr2	= new ArrayList();
		private anaPlotAlignPW43	plotLabels				= anaPlotAlignPW43.RightAll;
		private anaPivotStylesPW43  pivotFormula			= anaPivotStylesPW43.Floor;
		private anaSessionTypePW43	pivotSession			= anaSessionTypePW43.Auto;
		private anaSessionTypePW43	selectedSession			= anaSessionTypePW43.ETH;
		private anaSessionCountPW43 activeSession			= anaSessionCountPW43.Second;
		private anaCalcModePW43		calcMode				= anaCalcModePW43.DailyClose;
		private Data.PivotRange		pivotRangeType1			= PivotRange.Daily;
		private Data.PivotRange		pivotRangeType2			= PivotRange.Weekly;
		private double				range					= 0;
		private	double				pp						= 0.0;
		private	double				r1						= 0.0;
		private	double				r2						= 0.0;
		private	double				r3						= 0.0;
		private	double				r4						= 0.0;
		private	double				r5						= 0.0;
		private	double				s1						= 0.0;
		private	double				s2						= 0.0;
		private	double				s3						= 0.0;
		private	double				s4						= 0.0;
		private	double				s5						= 0.0;
		private	double				rmid					= 0.0;
		private	double				r12mid					= 0.0;
		private	double				r23mid					= 0.0;
		private	double				r34mid					= 0.0;
		private	double				r45mid					= 0.0;
		private	double				smid					= 0.0;
		private	double				s12mid					= 0.0;
		private	double				s23mid					= 0.0;
		private	double				s34mid					= 0.0;
		private	double				s45mid					= 0.0;
		private double				centralPivot			= 0.0;
		private double				directionalPivot		= 0.0;
		private double				previousVWAP			= 0.0;
		private double				previousHigh			= 0.0;
		private double				previousLow				= 0.0;
		private double				previousClose			= 0.0;
		private SolidBrush			pivotBrush				= new SolidBrush(Color.LimeGreen);			
		private SolidBrush			centralBrush			= new SolidBrush(Color.DarkGreen);			
		private SolidBrush			jacksonBrush			= new SolidBrush(Color.SlateGray);			
		private SolidBrush			errorBrush				= new SolidBrush(Color.White);
		private Pen					pivotLinePen			= null;
		private Font				labelFont				= new Font("Arial", 8);
		private Font				errorFont				= new Font("Arial", 10);
		private string				errorData2				= "No daily data found. Please reload daily data or use CalcFromIntradayData.";
		private	string				errorData3				= "The prior close detected by the Weekly Pivots indicator is not within the range of the selected RTH session.";
		private	string				errorData4				= "Insufficient historical data for the Weekly Pivots indicator. Please increase chart look back period.";
        private float				errorTextHeight			= 0;
		private	StringFormat		stringFormatCenter		= new StringFormat();
		private	StringFormat		stringFormatFar			= new StringFormat();
		private	StringFormat		stringFormatNear		= new StringFormat();
		private double				userDefinedClose		= 0.0;
		private double				userDefinedHigh			= 0.0;
		private double				userDefinedLow			= 0.0;
		private int					countDown				= 1;
		private int					numberOfSessions		= 1;
		private int					sessionCount			= 0;
		private int					width					= 20;
		private int 				labelFontSize			= 8;
		private int 				labelOffset				= 75;
		private int					opacityPR				= 4;
		private int					opacityJZ				= 4;
		private int 				plot0Width 				= 1;
		private DashStyle 			dash0Style				= DashStyle.Solid;
		private int 				plot1Width 				= 1;
		private DashStyle 			dash1Style				= DashStyle.DashDot;
		private int 				plot2Width 				= 2;
		private DashStyle 			dash2Style				= DashStyle.Dot;
		private Color 				pivotColor				= Color.MediumSpringGreen;
		private Color 				resistanceColor 		= Color.YellowGreen;
		private Color 				supportColor	 		= Color.YellowGreen;
		private Color				midpivotColor			= Color.YellowGreen;
		private Color				highColor				= Color.Lime;
		private Color				lowColor				= Color.OrangeRed;	
		private Color				closeColor				= Color.Yellow;
		private Color				centralPivotColor		= Color.MediumSpringGreen;
		private Color				directionalPivotColor	= Color.MediumSpringGreen;
		private Color				pivotRangeColor			= Color.LimeGreen;
		private Color				centralRangeColor		= Color.DarkGreen;
		private Color				jacksonZonesColor		= Color.SlateGray;
		private Color				vwapColor				= Color.HotPink;
		private DateTime			publicHoliday0			= new DateTime (2009,07,03);
		private DateTime			publicHoliday1			= new DateTime (2014,07,04);
		private DateTime			publicHoliday2			= new DateTime (2015,07,03);
		private DateTime			publicHoliday3;
		private DateTime[]			publicHoliday			= new DateTime[4];
		
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-PP"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-R1"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-S1"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-R2"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-S2"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-R3"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-S3"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-R4"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-S4"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-R5"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-S5"));
			
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-RMid"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-SMid"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-R12Mid"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-S12Mid"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-R23Mid"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-S23Mid"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-R34Mid"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-S34Mid"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-R45Mid"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-S45Mid"));
	
			Add(new Plot(new Pen(Color.Gray,2), PlotStyle.Line,"PW-H"));
			Add(new Plot(new Pen(Color.Gray,2), PlotStyle.Line,"PW-L"));
			Add(new Plot(new Pen(Color.Gray,2), PlotStyle.Line,"PW-C"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-CP"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-DP"));

			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"PW-VWAP"));

			AutoScale						= false;
			Overlay							= true;
			PriceTypeSupported				= false;
			PlotsConfigurable				= false;
			MaximumBarsLookBack				= MaximumBarsLookBack.Infinite;
			BarsRequired					= 0;
			ZOrder							= 1;
			stringFormatCenter.Alignment 	= StringAlignment.Center;
			stringFormatFar.Alignment		= StringAlignment.Far;
			stringFormatNear.Alignment 		= StringAlignment.Near;
		}

		/// <summary>
		/// </summary>
		protected override void OnStartUp()
		{
			if (Instrument.MasterInstrument.InstrumentType == InstrumentType.Future &&
				(Instrument.MasterInstrument.Name == "EMD" ||Instrument.MasterInstrument.Name == "ES" || Instrument.MasterInstrument.Name == "NQ" 
				||Instrument.MasterInstrument.Name == "YM"||Instrument.MasterInstrument.Name == "GE" ||Instrument.MasterInstrument.Name == "SR" 
				||Instrument.MasterInstrument.Name == "UB"||Instrument.MasterInstrument.Name == "ZB" ||Instrument.MasterInstrument.Name == "ZF"
				||Instrument.MasterInstrument.Name == "ZN"||Instrument.MasterInstrument.Name == "ZQ" ||Instrument.MasterInstrument.Name == "ZT"
				||Instrument.MasterInstrument.Name == "6A"||Instrument.MasterInstrument.Name == "6B" ||Instrument.MasterInstrument.Name == "6C"
				||Instrument.MasterInstrument.Name == "6E"||Instrument.MasterInstrument.Name == "6J" ||Instrument.MasterInstrument.Name == "6M"
				||Instrument.MasterInstrument.Name == "6N"||Instrument.MasterInstrument.Name == "6S" ||Instrument.MasterInstrument.Name == "E7"
				||Instrument.MasterInstrument.Name == "J7"||Instrument.MasterInstrument.Name == "M6A" ||Instrument.MasterInstrument.Name == "M6B" 
				||Instrument.MasterInstrument.Name == "M6C"||Instrument.MasterInstrument.Name == "M6E" ||Instrument.MasterInstrument.Name == "M6J"
				||Instrument.MasterInstrument.Name == "M6S"||Instrument.MasterInstrument.Name == "CL" ||Instrument.MasterInstrument.Name == "EH"
				||Instrument.MasterInstrument.Name == "GC"||Instrument.MasterInstrument.Name == "HG" ||Instrument.MasterInstrument.Name == "HO"
				||Instrument.MasterInstrument.Name == "NG"||Instrument.MasterInstrument.Name == "QG" ||Instrument.MasterInstrument.Name == "QM"
				||Instrument.MasterInstrument.Name == "RB"||Instrument.MasterInstrument.Name == "SI" ||Instrument.MasterInstrument.Name == "YG" 
				||Instrument.MasterInstrument.Name == "YI"||Instrument.MasterInstrument.Name == "GF" ||Instrument.MasterInstrument.Name == "GPB"
				||Instrument.MasterInstrument.Name == "HE"||Instrument.MasterInstrument.Name == "LE" ||Instrument.MasterInstrument.Name == "YC"
				||Instrument.MasterInstrument.Name == "YK"||Instrument.MasterInstrument.Name == "YW" ||Instrument.MasterInstrument.Name == "ZC"
				||Instrument.MasterInstrument.Name == "ZE"||Instrument.MasterInstrument.Name == "ZL" ||Instrument.MasterInstrument.Name == "ZM"
				||Instrument.MasterInstrument.Name == "ZO"||Instrument.MasterInstrument.Name == "ZR" ||Instrument.MasterInstrument.Name == "ZS"
				||Instrument.MasterInstrument.Name == "ZW"))				
				isGlobex = true;
			if (isGlobex)
			{
				publicHoliday[0] = publicHoliday0;
				publicHoliday[1] = publicHoliday1;
				publicHoliday[2] = publicHoliday2;
				publicHoliday[3] = publicHoliday3;
			}						
			else for(int i=0; i<4; i++)
				publicHoliday[i] = Cbi.Globals.MinDate;
			if (Instrument.MasterInstrument.InstrumentType == Cbi.InstrumentType.Currency || Instrument.MasterInstrument.Name == "DX"|| Instrument.MasterInstrument.Name == "6A"
				|| Instrument.MasterInstrument.Name == "6B" || Instrument.MasterInstrument.Name == "6C" ||Instrument.MasterInstrument.Name == "6E"
				|| Instrument.MasterInstrument.Name == "6J" || Instrument.MasterInstrument.Name == "6M" || Instrument.MasterInstrument.Name == "6S"
				|| Instrument.MasterInstrument.Name == "6N" || Instrument.MasterInstrument.Name == "E7" || Instrument.MasterInstrument.Name == "J7"
				|| Instrument.MasterInstrument.Name == "M6A" || Instrument.MasterInstrument.Name == "M6B" || Instrument.MasterInstrument.Name == "M6C"
				|| Instrument.MasterInstrument.Name == "M6E" || Instrument.MasterInstrument.Name == "M6J" || Instrument.MasterInstrument.Name == "M6S")
				isCurrency = true;
			
			if (pivotSession == anaSessionTypePW43.Auto && isCurrency)
				selectedSession = anaSessionTypePW43.ETH;
			else if(pivotSession == anaSessionTypePW43.Auto)
				selectedSession = anaSessionTypePW43.RTH;
			else
				selectedSession = pivotSession;
			labelFont = new Font ("Arial", labelFontSize);
			if (PivotFormula == anaPivotStylesPW43.Jackson)
			{
				Plots[0].Name = "W-PP";
				Plots[1].Name = "W-R1";
				Plots[2].Name = "W-S1";
				Plots[3].Name = "W-RB1";
				Plots[4].Name = "W-SB1";
				Plots[5].Name = "W-R2";
				Plots[6].Name = "W-S2";
				Plots[7].Name = "W-RB2";
				Plots[8].Name = "W-SB2";
				Plots[9].Name = "W-R3";
				Plots[10].Name = "W-S3";
				jacksonBrush = new SolidBrush(Color.FromArgb(25*opacityJZ, jacksonZonesColor));
			}
			else
			{
				Plots[0].Name = "W-PP";
				Plots[1].Name = "W-R1";
				Plots[2].Name = "W-S1";
				Plots[3].Name = "W-R2";
				Plots[4].Name = "W-S2";
				Plots[5].Name = "W-R3";
				Plots[6].Name = "W-S3";
				Plots[7].Name = "W-R4";
				Plots[8].Name = "W-S4";
				Plots[9].Name = "W-R5";
				Plots[10].Name = "W-S5";
			}
			pivotBrush = new SolidBrush(Color.FromArgb(25*opacityPR, pivotRangeColor));
			centralBrush = new SolidBrush(Color.FromArgb(25*opacityPR, centralRangeColor));
			pivotLinePen = new Pen(Color.FromArgb(Math.Min(40*opacityPR,255), pivotRangeColor),1);
			pivotLinePen.DashStyle = DashStyle.Dot;
			Plots[0].Pen.Color = pivotColor;
			Plots[1].Pen.Color = resistanceColor;
			Plots[2].Pen.Color = supportColor;
			Plots[3].Pen.Color = resistanceColor;
			Plots[4].Pen.Color = supportColor;
			Plots[5].Pen.Color = resistanceColor;
			Plots[6].Pen.Color = supportColor;
			Plots[7].Pen.Color = resistanceColor;
			Plots[8].Pen.Color = supportColor;
			Plots[9].Pen.Color = resistanceColor;
			Plots[10].Pen.Color = supportColor;
			Plots[11].Pen.Color = midpivotColor;
			Plots[12].Pen.Color = midpivotColor;
			Plots[13].Pen.Color = midpivotColor;
			Plots[14].Pen.Color = midpivotColor;
			Plots[15].Pen.Color = midpivotColor;
			Plots[16].Pen.Color = midpivotColor;
			Plots[17].Pen.Color = midpivotColor;
			Plots[18].Pen.Color = midpivotColor;
			Plots[19].Pen.Color = midpivotColor;
			Plots[20].Pen.Color = midpivotColor;
			Plots[21].Pen.Color = highColor;
			Plots[22].Pen.Color = lowColor;
			Plots[23].Pen.Color = closeColor;
			Plots[24].Pen.Color = centralPivotColor;
			Plots[25].Pen.Color = directionalPivotColor;
			Plots[26].Pen.Color = vwapColor;
			
			for (int i = 0; i < 11; i++)
			{
				Plots[i].Pen.Width = plot0Width;
				Plots[i].Pen.DashStyle = dash0Style;
			}
			for (int i = 11; i < 21; i++)
			{
				Plots[i].Pen.Width = plot1Width;
				Plots[i].Pen.DashStyle = dash1Style;
			}
			for (int i = 21; i < 24; i++)
			{
				Plots[i].Pen.Width = plot2Width;
				Plots[i].Pen.DashStyle = dash2Style;
			}			
			for (int i = 24; i < 27; i++)
			{
				Plots[i].Pen.Width = plot0Width;
				Plots[i].Pen.DashStyle = dash0Style;
			}			
			if(ChartControl != null)
				errorBrush.Color = ChartControl.AxisColor;
			if (Instrument.MasterInstrument.InstrumentType == Cbi.InstrumentType.Currency && (TickSize == 0.00001 || TickSize == 0.001))
				displaySize = 5* TickSize;
			else
				displaySize = TickSize;
			if (AutoScale)
				AutoScale = false;
			if (!Bars.BarsType.IsIntraday || BarsPeriod.Id == PeriodType.Minute || BarsPeriod.Id == PeriodType.Second)
				tickBuilt = false;
			else
				tickBuilt = true;
			existsHistDailyData = false;
			countDown = 1;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (Bars == null)
				return; 
			if (!Bars.BarsType.IsIntraday && Bars.Period.Id != PeriodType.Day)
				return;
			if (Bars.Period.Id == PeriodType.Day && Bars.Period.Value > 1)
				return;
			
			if (CurrentBar == 0)
			{
				if (selectedSession == anaSessionTypePW43.UserDefined)
				{
					currentHigh			= userDefinedHigh;
					currentLow			= userDefinedLow;
					currentClose		= userDefinedClose;
				}
				else
				{
					currentDate = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType1);
					currentWeek = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType2);
					sessionCount = 1;
					if (selectedSession == anaSessionTypePW43.DailyBars && currentDate.DayOfWeek == DayOfWeek.Monday)
						countDown = 0;
				}
				return;
			}
			if(FirstTickOfBar && Bars.BarsType.IsIntraday && (selectedSession == anaSessionTypePW43.DailyBars || (selectedSession != anaSessionTypePW43.UserDefined && calcMode == anaCalcModePW43.DailyClose)))
			{	
				if(!isDailyDataLoaded)
				{
					Enabled = false;
					System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GetBarsNow));
					return;
				}
				if (!existsHistDailyData)
				{
					DrawTextFixed("errortag2", errorData2, TextPosition.Center, errorBrush.Color, errorFont, Color.Transparent,Color.Transparent,0);
					return;
				}	

				IBar dailyBar;
				if (existsHistDailyData) 
				{
					noCurrentDailyBar = false;
					noDailyData = false;
					dailyBar = dailyBars.Get(dailyBars.GetBar(currentDate));
					if (dailyBar.Time.Date != currentDate)
					{
						noCurrentDailyBar = true;
						dailyBar = dailyBars.Get(dailyBars.GetBar(priorDate));
						if (dailyBar.Time.Date != priorDate)
						{
							noDailyData = true;
						}	
					}
					if(selectedSession == anaSessionTypePW43.DailyBars)
					{
						high = dailyBar.High;
						low = dailyBar.Low;
					}
					close = dailyBar.Close;
				} 
				else 
					dailyBar = null;
			}

			if(!existsHistDailyData)
			{
				high = High[0];
				low = Low[0];
				close = Close[0];
			}
			else if (selectedSession != anaSessionTypePW43.DailyBars)
			{
				high = High[0];
				low = Low[0];
			}

			lastBarTimeStamp1 = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType1);
			lastBarTimeStamp2 = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType2);
			if (isEndOfWeekHoliday && selectedSession != anaSessionTypePW43.RTH)
				lastBarTimeStamp2 = GetLastBarSessionDate(Time[0].AddDays(3), Bars, 0, pivotRangeType2);
			if (lastBarTimeStamp2 != currentWeek || selectedSession == anaSessionTypePW43.UserDefined)
			{
				if (countDown == 0)
				{
					Bars.Session.GetNextBeginEnd(Bars, 0, out sessionBegin, out sessionEnd);
					plotStart = sessionBegin;
				}	
				countDown = countDown -1;
				firstDayOfPeriod = true;
				sessionCount = 1;
				calcOpen = false;
				if (countDown < 0 && existsHistDailyData)
				{	
					currentClose = close;
					if (selectedSession == anaSessionTypePW43.DailyBars)
					{
						currentHigh = Math.Max(currentHigh, high);
						currentLow = Math.Min(currentLow, low);
					}
				}
				priorDate = currentDate;
				currentDate = lastBarTimeStamp1;
				currentWeek = lastBarTimeStamp2;
				if ((countDown < 0) || selectedSession == anaSessionTypePW43.UserDefined)
				{
					range = (currentHigh - currentLow);						
					if (PivotFormula == anaPivotStylesPW43.Fibonacci)
					{
						pp				= (currentHigh + currentLow + currentClose)/ 3;
						r1				= pp + 0.382*range;
						s1				= pp - 0.382*range;
						r2				= pp + 0.618*range;
						s2				= pp - 0.618*range;
						r3				= pp + range;
						s3				= pp - range;
						r4				= pp + 1.382*range;
						s4				= pp - 1.382*range;
						r5				= pp + 1.618*range;
						s5 				= pp - 1.618*range;
					}
					else if (PivotFormula == anaPivotStylesPW43.Jackson)
					{
						pp				= (currentHigh + currentLow + currentClose)/ 3;
						r1				= pp + 0.500*range;
						s1				= pp - 0.500*range;
						r2				= pp + 0.618*range;
						s2				= pp - 0.618*range;
						r3				= pp + range;
						s3				= pp - range;
						r4				= pp + 1.382*range;
						s4				= pp - 1.382*range;
						r5				= pp + 1.618*range;
						s5 				= pp - 1.618*range;
					}
					else if (PivotFormula == anaPivotStylesPW43.Camarilla)
					{
						pp				= (currentHigh + currentLow + currentClose)/ 3;
						r1				= currentClose + 1.1/12*range;
						s1				= currentClose - 1.1/12*range;
						r2				= currentClose + 1.1/6*range;
						s2				= currentClose - 1.1/6*range;
						r3				= currentClose + 1.1/4*range;
						s3				= currentClose - 1.1/4*range;
						r4				= currentClose + 1.1/2*range;
						s4				= currentClose - 1.1/2*range;
						if(currentLow != 0)
						{
							r5			= currentClose * currentHigh / currentLow; 
							s5 			= 2 * currentClose - r5;
						}
					}
					else
					{
						if (PivotFormula == anaPivotStylesPW43.Floor || PivotFormula == anaPivotStylesPW43.Wide)
							pp			= (currentHigh + currentLow + currentClose) / 3;
						else if (PivotFormula == anaPivotStylesPW43.Woodie)
							pp			= (currentHigh + currentLow + 2*currentClose) / 4;
						
						r1				= 2 * pp - currentLow;
						s1				= 2 * pp - currentHigh;
						r2				= pp + range;
						s2				= pp - range;
						if (PivotFormula == anaPivotStylesPW43.Floor || PivotFormula == anaPivotStylesPW43.Woodie)
						{
							r3			= r1 + range;
							s3			= s1 - range;
							r4			= r3 + (pp - currentLow);
							s4			= s3 - (currentHigh - pp);
							r5			= r4 + (pp - currentLow);
							s5			= s4 - (currentHigh - pp);
						}
						else if (PivotFormula == anaPivotStylesPW43.Wide)
						{
							r3			= r2 + range;
							s3			= s2 - range;
							r4			= r3 + range;
							s4			= s3 - range;
							r5			= r4 + range;
							s5			= s4 - range;
						}
						rmid			= 0.5*(pp+r1);
						smid			= 0.5*(pp+s1);
						r12mid			= 0.5*(r1+r2);
						s12mid			= 0.5*(s1+s2);
						r23mid			= 0.5*(r2+r3);
						s23mid			= 0.5*(s2+s3);
						r34mid			= 0.5*(r3+r4);
						s34mid			= 0.5*(s3+s4);
						r45mid			= 0.5*(r4+r5);
						s45mid			= 0.5*(s4+s5);
					}
					pp = Math.Round(pp/displaySize)* displaySize;
					r1 = Math.Round(r1/displaySize)* displaySize;
					s1 = Math.Round(s1/displaySize)* displaySize;
					r2 = Math.Round(r2/displaySize)* displaySize;
					s2 = Math.Round(s2/displaySize)* displaySize;
					r3 = Math.Round(r3/displaySize)* displaySize;
					s3 = Math.Round(s3/displaySize)* displaySize;
					r4 = Math.Round(r4/displaySize)* displaySize;
					s4 = Math.Round(s4/displaySize)* displaySize;
					r5 = Math.Round(r5/displaySize)* displaySize;
					s5 = Math.Round(s5/displaySize)* displaySize;
					if (showMidpivots && PivotFormula != anaPivotStylesPW43.Camarilla && 
						PivotFormula != anaPivotStylesPW43.Fibonacci && PivotFormula != anaPivotStylesPW43.Jackson)
					{
						rmid = Math.Round(rmid/displaySize)* displaySize;
						smid = Math.Round(smid/displaySize)* displaySize;
						r12mid = Math.Round(r12mid/displaySize)* displaySize;
						s12mid = Math.Round(s12mid/displaySize)* displaySize;
						r23mid = Math.Round(r23mid/displaySize)* displaySize;
						s23mid = Math.Round(s23mid/displaySize)* displaySize;
						r34mid = Math.Round(r34mid/displaySize)* displaySize;
						s34mid = Math.Round(s34mid/displaySize)* displaySize;
						r45mid = Math.Round(r45mid/displaySize)* displaySize;
						s45mid = Math.Round(s45mid/displaySize)* displaySize;
					}
					previousHigh		= currentHigh;
					previousLow			= currentLow;
					previousClose		= currentClose;
					centralPivot		= Math.Round(((currentHigh + currentLow) / 2 + 0.01*displaySize)/displaySize)*displaySize;
					directionalPivot	= Math.Round((2*pp - centralPivot)/displaySize)* displaySize;
					if (volSum > double.Epsilon)
						previousVWAP 	= Math.Round(currentVolPriceSum/(currentVolSum*displaySize))* displaySize;
					else
						previousVWAP	= 0.0;
					if (noDailyData)
						plotPivots = false;
					else 
						plotPivots = true;
				}	
				if (selectedSession != anaSessionTypePW43.UserDefined)
				{
					if (Bars.BarsType.IsIntraday && (!existsHistDailyData || (existsHistDailyData && selectedSession != anaSessionTypePW43.DailyBars))) 
					{
						if (numberOfSessions == 1 || selectedSession == anaSessionTypePW43.ETH || (selectedSession == anaSessionTypePW43.RTH && activeSession == anaSessionCountPW43.First))
						{
							if (!existsHistDailyData)	 
								currentClose		= close; 
							currentHigh			= high;
							currentLow			= low;
							volSum				= 0.0;
							volPriceSum			= 0.0;
							currentVolSum		= volSum + Volume[0];
							currentVolPriceSum	= volPriceSum + Typical[0]*Volume[0];
							calcOpen			= true;
						}
						else 
						{	
							volSum				= 0.0;
							volPriceSum			= 0.0;
							currentVolPriceSum 	= 0.0;
							currentVolSum		= 0.0;
							calcOpen			= false;
						}
					}
					else if (Bars.BarsType.IsIntraday)
					{	
						currentHigh 		= double.MinValue;
						currentLow 			= double.MaxValue;
						volSum				= 0.0;
						volPriceSum			= 0.0;
						currentVolSum		= volSum + Volume[0];
						currentVolPriceSum	= volPriceSum + Typical[0]*Volume[0];
						calcOpen			= true;
					}
					else 
					{
						currentClose	= close;
						currentHigh		= high;
						currentLow		= low;
						calcOpen		= false;
					}
				}
			}
			else if (selectedSession != anaSessionTypePW43.UserDefined && (lastBarTimeStamp1 != currentDate || Bars.Period.Id == PeriodType.Day))
			{
				firstDayOfPeriod = false;
				sessionCount = 1;
				priorDate = currentDate;
				currentDate = lastBarTimeStamp1;
				if (Bars.BarsType.IsIntraday && existsHistDailyData && pivotSession == anaSessionTypePW43.DailyBars) 
				{
					if (!noCurrentDailyBar)
					{
						currentHigh 	= Math.Max(currentHigh, high);
						currentLow 		= Math.Min(currentLow, low);
					}
					volSum				= currentVolSum;
					volPriceSum			= currentVolPriceSum;
					currentVolSum		= volSum + Volume[0];
					currentVolPriceSum	= volPriceSum + Typical[0]*Volume[0];
					calcOpen			= true;
				}
				else if (Bars.BarsType.IsIntraday)
				{
					if (numberOfSessions == 1 || pivotSession == anaSessionTypePW43.ETH || (pivotSession == anaSessionTypePW43.RTH && activeSession == anaSessionCountPW43.First))
					{
						if (!existsHistDailyData)	
							currentClose		= close;
						currentHigh 		= Math.Max(currentHigh, high);
						currentLow 			= Math.Min(currentLow, low);
						volSum				= currentVolSum;
						volPriceSum			= currentVolPriceSum;
						currentVolSum		= volSum + Volume[0];
						currentVolPriceSum	= volPriceSum + Typical[0]*Volume[0];
						calcOpen			= true;
					}
					else
						calcOpen			= false;
				}				
				else if (countDown < 1)
				{
					currentClose	= close;
					currentHigh 	= Math.Max(currentHigh, high);
					currentLow 		= Math.Min(currentLow, low);
				}	
			}
			else if (selectedSession != anaSessionTypePW43.UserDefined && Bars.BarsType.IsIntraday && Bars.FirstBarOfSession)
			{
				if (FirstTickOfBar)
				{
					sessionCount = sessionCount + 1;
					numberOfSessions = Math.Min(3, Math.Max(sessionCount, numberOfSessions));
				}
				if (existsHistDailyData && selectedSession == anaSessionTypePW43.DailyBars) 
				{
					if (FirstTickOfBar)
					{
						volSum			= currentVolSum;
						volPriceSum		= currentVolPriceSum;
					}
					currentVolSum		= volSum + Volume[0];
					currentVolPriceSum	= volPriceSum + Typical[0]*Volume[0];
					calcOpen			= true;
				}	
				else if (numberOfSessions == 1 || selectedSession == anaSessionTypePW43.ETH)
				{
					if (!existsHistDailyData)	
						currentClose		= close;
					currentHigh			= Math.Max(currentHigh, high);
					currentLow			= Math.Min(currentLow, low);
					if (FirstTickOfBar)
					{
						volSum			= currentVolSum;
						volPriceSum		= currentVolPriceSum;
					}
					currentVolSum		= volSum + Volume[0];
					currentVolPriceSum	= volPriceSum + Typical[0]*Volume[0];
					calcOpen			= true;
				}
				else if (firstDayOfPeriod && ((sessionCount == 1 && activeSession == anaSessionCountPW43.First) || (sessionCount == 2 
					&& activeSession == anaSessionCountPW43.Second) || (sessionCount == 3 && activeSession == anaSessionCountPW43.Third)))
				{
					if (!existsHistDailyData)	
						currentClose	= close;
					currentHigh			= high;
					currentLow			= low;
					if (FirstTickOfBar)
					{
						volSum			= 0.0;
						volPriceSum		= 0.0;
					}
					currentVolSum		= volSum + Volume[0];
					currentVolPriceSum	= volPriceSum + Typical[0]*Volume[0];
					calcOpen			= true;
				}
				else if ((sessionCount == 1 && activeSession == anaSessionCountPW43.First)||(sessionCount == 2 && 
					activeSession == anaSessionCountPW43.Second) || (sessionCount == 3 && activeSession == anaSessionCountPW43.Third))
				{
					if (!existsHistDailyData)	
						currentClose	= close;
					currentHigh		= Math.Max(currentHigh, high);
					currentLow		= Math.Min(currentLow, low);
					if (FirstTickOfBar)
					{
						volSum			= currentVolSum;
						volPriceSum		= currentVolPriceSum;
					}
					currentVolSum		= volSum + Volume[0];
					currentVolPriceSum	= volPriceSum + Typical[0]*Volume[0];
					calcOpen			= true;
				}
				else 
					calcOpen = false;
			}
			else if (selectedSession != anaSessionTypePW43.UserDefined && Bars.BarsType.IsIntraday && calcOpen)
			{	
				if (selectedSession != anaSessionTypePW43.DailyBars) 
				{
					if (!existsHistDailyData)
						currentClose	= close;
					currentHigh		= Math.Max(currentHigh, high);
					currentLow		= Math.Min(currentLow, low);
				}
				if (FirstTickOfBar)
				{
					volSum			= currentVolSum;
					volPriceSum		= currentVolPriceSum;
				}
				currentVolSum		= volSum + Volume[0];
				currentVolPriceSum	= volPriceSum + Typical[0]*Volume[0];
			}
			else if(!Bars.BarsType.IsIntraday)
			{
				currentClose	= close;
				currentHigh 	= Math.Max(currentHigh, high);
				currentLow 		= Math.Min(currentLow, low);
			}	
			
			if(FirstTickOfBar)
			{
				if(plotPivots && !(selectedSession == anaSessionTypePW43.RTH && activeSession == anaSessionCountPW43.Third && numberOfSessions == 2))
				{
					PP.Set(pp);
					R1.Set(r1);
					S1.Set(s1);
					R2.Set(r2);
					S2.Set(s2);
					R3.Set(r3);
					S3.Set(s3);
					R4.Set(r4);
					S4.Set(s4);
					R5.Set(r5);
					S5.Set(s5);
					if (showMidpivots && PivotFormula != anaPivotStylesPW43.Camarilla && 
						PivotFormula != anaPivotStylesPW43.Fibonacci && PivotFormula != anaPivotStylesPW43.Jackson)
					{
						RMid.Set(rmid);
						SMid.Set(smid);
						R12Mid.Set(r12mid);
						S12Mid.Set(s12mid);
						R23Mid.Set(r23mid);
						S23Mid.Set(s23mid);
						R34Mid.Set(r34mid);
						S34Mid.Set(s34mid);
						R45Mid.Set(r45mid);
						S45Mid.Set(s45mid);
					}	
					PreviousHigh.Set(previousHigh);
					PreviousLow.Set(previousLow);
					PreviousClose.Set(previousClose);
					if(PivotFormula != anaPivotStylesPW43.Camarilla)
					{
						CentralPivot.Set(centralPivot);
						DirectionalPivot.Set(directionalPivot);
					}
					else
					{
						CentralPivot.Reset();
						DirectionalPivot.Reset();
					}
					if (Instrument.MasterInstrument.InstrumentType != Cbi.InstrumentType.Index && Instrument.MasterInstrument.InstrumentType != Cbi.InstrumentType.Currency 
						&& previousVWAP > double.Epsilon && selectedSession != anaSessionTypePW43.UserDefined && Bars.BarsType.IsIntraday)
						PVWAP.Set(previousVWAP);
					else
						PVWAP.Reset();
				}
				else
				{	
					PP.Reset();
					R1.Reset();
					S1.Reset();
					R2.Reset();
					S2.Reset();
					R3.Reset();
					S3.Reset();
					R4.Reset();
					S4.Reset();
					R5.Reset();
					S5.Reset();
					RMid.Reset();
					SMid.Reset();
					R12Mid.Reset();
					S12Mid.Reset();
					R23Mid.Reset();
					S23Mid.Reset();
					R34Mid.Reset();
					S34Mid.Reset();
					R45Mid.Reset();
					S45Mid.Reset();
					PreviousHigh.Reset();
					PreviousLow.Reset();
					PreviousClose.Reset();
					CentralPivot.Reset();
					DirectionalPivot.Reset();
					PVWAP.Reset();
				}
				if (existsHistDailyData && selectedSession == anaSessionTypePW43.RTH 
					&& (PreviousClose[0] < PreviousLow[0] || PreviousClose[0] > PreviousHigh[0]))
					DrawTextFixed("errortag3", errorData3, TextPosition.Center, errorBrush.Color, errorFont, Color.Transparent,Color.Transparent,0);
				else
					DrawTextFixed("errortag3", "", TextPosition.Center, errorBrush.Color, errorFont, Color.Transparent,Color.Transparent,0);
			}
		}

		#region Properties

		/// <summary>
		/// </summary>
		[Description("Pivot Formula Type")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Pivot formula")]
		public anaPivotStylesPW43 PivotFormula
		{
			get { return pivotFormula; }
			set { pivotFormula = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Session - ETH or RTH - used for calculating pivots")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Calculate from session")]
		public anaSessionTypePW43 PivotSession
		{
			get { return pivotSession; }
			set { pivotSession = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Session # used for calculating RTH pivots")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Select session (RTH only)")]
		public anaSessionCountPW43 ActiveSession
		{
			get { return activeSession; }
			set { activeSession = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Select the settlement or close price used for calculating pivots.")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Settlement/Close")]
		public anaCalcModePW43 CalcMode
		{
			get { return calcMode; }
			set { calcMode = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Option to show MidPivots")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Show midpivots")]
		public bool ShowMidpivots 
		{
			get { return showMidpivots; }
			set { showMidpivots = value; }
		}

		[Description("Option to show plots for prior days")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Show prior days")]
		public bool ShowPriorPeriods 
		{
			get { return showPriorPeriods; }
			set { showPriorPeriods = value; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries PP
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries R1
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries S1
		{
			get { return Values[2]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries R2
		{
			get { return Values[3]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries S2
		{
			get { return Values[4]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries R3
		{
			get { return Values[5]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries S3
		{
			get { return Values[6]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries R4
		{
			get { return Values[7]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries S4
		{
			get { return Values[8]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries R5
		{
			get { return Values[9]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries S5
		{
			get { return Values[10]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries RMid
		{
			get { return Values[11]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries SMid
		{
			get { return Values[12]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries R12Mid
		{
			get { return Values[13]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries S12Mid
		{
			get { return Values[14]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries R23Mid
		{
			get { return Values[15]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries S23Mid
		{
			get { return Values[16]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries R34Mid
		{
			get { return Values[17]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries S34Mid
		{
			get { return Values[18]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries R45Mid
		{
			get { return Values[19]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries S45Mid
		{
			get { return Values[20]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries PreviousHigh
		{
			get { return Values[21]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries PreviousLow
		{
			get { return Values[22]; }
		}
			
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries PreviousClose
		{
			get { return Values[23]; }
		}
			
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CentralPivot
		{
			get { return Values[24]; }
		}
			
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries DirectionalPivot
		{
			get { return Values[25]; }
		}
			
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries PVWAP
		{
			get { return Values[26]; }
		}

		/// <summary>
		/// close value for user defined pivots calculation
		/// </summary>
		[Description("User defined prior session close value used for pivots calculation.")]
		[GridCategory("\rUser defined values")]
		[Gui.Design.DisplayNameAttribute("User defined close")]
		public double UserDefinedClose
		{
			get { return userDefinedClose; }
			set { userDefinedClose = value; }
		}

		/// <summary>
		/// high value for user defined pivots calculation
		/// </summary>
		[Description("User defined prior session high value used for pivots calculation.")]
		[GridCategory("\rUser defined values")]
		[Gui.Design.DisplayNameAttribute("User defined high")]
		public double UserDefinedHigh
		{
			get { return userDefinedHigh; }
			set { userDefinedHigh = value; }
		}

		/// <summary>
		/// low value for user defined pivots calculation
		/// </summary>
		[Description("User defined prior session low value used for pivots calculation.")]
		[GridCategory("\rUser defined values")]
		[Gui.Design.DisplayNameAttribute("User defined low")]
		public double UserDefinedLow
		{
			get { return userDefinedLow; }
			set { userDefinedLow = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Option where to plot labels")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Label position")]
		public anaPlotAlignPW43 PlotLabels
		{
			get { return plotLabels; }
			set { plotLabels = value; }
		}
			
		/// <summary>
		/// </summary>
		[Description("Label distance from line.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Label offset")]
		public int LabelOffset
		{
			get { return labelOffset; }
			set { labelOffset = Math.Max(1, value); }
		}
	
		/// <summary>
		/// </summary>
		[Description("Font size for labels.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Label font size")]
		public int LabelFontSize
		{
			get { return labelFontSize; }
			set { labelFontSize = Math.Max(1, value); }
		}
	
		/// <summary>
		/// </summary>
		[Description("Length of the plots as # of bars.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Max. plot length # bars")]
		public int Width
		{
			get { return width; }
			set { width = Math.Max(0, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Width for pivots.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width pivots")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for pivots.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style pivots")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		} 

		/// <summary>
		/// </summary>
		[Description("Width for midpivots.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width midpivots")]
		public int Plot1Width
		{
			get { return plot1Width; }
			set { plot1Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for midpivots.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style midpivots")]
		public DashStyle Dash1Style
		{
			get { return dash1Style; }
			set { dash1Style = value; }
		} 
		
		/// </summary>
		[Description("Width for high, low and close.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width high, low, close")]
		public int Plot2Width
		{
			get { return plot2Width; }
			set { plot2Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for high, low and close.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style high, low, close")]
		public DashStyle Dash2Style
		{
			get { return dash2Style; }
			set { dash2Style = value; }
		} 
		
		/// <summary>
		/// </summary>
		[Description("Opacity for weekly pivot range")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Opacity pivot range")]
		public int OpacityPR
		{
			get { return opacityPR; }
			set { opacityPR = Math.Min(Math.Max(0, value),10); }
		}

		/// <summary>
		/// </summary>
		[Description("Opacity for weekly Jackson Zones")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Opacity Jackson Zones")]
		public int OpacityJZ
		{
			get { return opacityJZ; }
			set { opacityJZ = Math.Min(Math.Max(0, value),10); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for main pivot")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Main pivot")]
		public Color PivotColor
		{
			get { return pivotColor; }
			set { pivotColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string PivotColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(pivotColor); }
			set { pivotColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for resistance pivots")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Resistance")]
		public Color ResistanceColor
		{
			get { return resistanceColor; }
			set { resistanceColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string ResistanceColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(resistanceColor); }
			set { resistanceColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for support pivots")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Support")]
		public Color SupportColor
		{
			get { return supportColor; }
			set { supportColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string SupportColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(supportColor); }
			set { supportColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for midpivots")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Midpivots")]
		public Color MidpivotColor
		{
			get { return midpivotColor; }
			set { midpivotColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string MidpivotColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(midpivotColor); }
			set { midpivotColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for prior high")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Prior high")]
		public Color HighColor
		{
			get { return highColor; }
			set { highColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string HighColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(highColor); }
			set { highColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for prior low")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Prior low")]
		public Color LowColor
		{
			get { return lowColor; }
			set { lowColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string LowColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(lowColor); }
			set { lowColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for prior close")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Prior close")]
		public Color CloseColor
		{
			get { return closeColor; }
			set { closeColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string CloseColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(closeColor); }
			set { closeColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for central pivot")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Central pivot")]
		public Color CentralPivotColor
		{
			get { return centralPivotColor; }
			set { centralPivotColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string CentralPivotColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(centralPivotColor); }
			set { centralPivotColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for directional pivot")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Directional pivot")]
		public Color DirectionalPivotColor
		{
			get { return directionalPivotColor; }
			set { directionalPivotColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DirectionalPivotColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(directionalPivotColor); }
			set { directionalPivotColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for weekly pivot range")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Pivot range")]
		public Color PivotRangeColor
		{
			get { return pivotRangeColor; }
			set { pivotRangeColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string PivotRangeColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(pivotRangeColor); }
			set { pivotRangeColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for weekly central range")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Central range")]
		public Color CentralRangeColor
		{
			get { return centralRangeColor; }
			set { centralRangeColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string CentralRangeColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(centralRangeColor); }
			set { centralRangeColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for weekly Jackson Zones")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Jackson Zones")]
		public Color JacksonZonesColor
		{
			get { return jacksonZonesColor; }
			set { jacksonZonesColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string JacksonZonesColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(jacksonZonesColor); }
			set { jacksonZonesColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for prior VWAP")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Prior VWAP")]
		public Color VWAPColor
		{
			get { return vwapColor; }
			set { vwapColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string VWAPColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(vwapColor); }
			set { vwapColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
        [Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 01 /no trade date")]
		public DateTime PublicHoliday0
		{
			get { return publicHoliday0;}
			set { publicHoliday0 = value;}
		}	
	
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
        [Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 02 /no trade date")]
		public DateTime PublicHoliday1
		{
			get { return publicHoliday1;}
			set { publicHoliday1 = value;}
		}	
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 03 /no trade date")]
		public DateTime PublicHoliday2
		{
			get { return publicHoliday2;}
			set { publicHoliday2 = value;}
		}	
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 04 /no trade date")]
		public DateTime PublicHoliday3
		{
			get { return publicHoliday3;}
			set { publicHoliday3 = value;}
		}	
		#endregion

		#region Miscellaneous

		public override string FormatPriceMarker(double price)
		{
			double trunc = Math.Truncate(price);
			int fraction = 0;
			string priceMarker = "";
			if (TickSize == 0.03125) 
			{
				fraction = Convert.ToInt32(32 * Math.Abs(price - trunc));	
				if (fraction < 10)
					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
				else if(fraction == 32)
				{	
					trunc = trunc + 1;
					fraction = 0;
					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
				}	
				else 
					priceMarker = trunc.ToString() + "'" + fraction.ToString();
			}
			else if (TickSize == 0.015625)
			{
				fraction = 5 * Convert.ToInt32(64 * Math.Abs(price - trunc));	
				if (fraction < 10)
					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
				else if (fraction < 100)
					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
				else if(fraction == 320)
				{	
					trunc = trunc + 1;
					fraction = 0;
					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
				}	
				else	
					priceMarker = trunc.ToString() + "'" + fraction.ToString();
			}
			else if (TickSize == 0.0078125)
			{
				fraction = Convert.ToInt32(Math.Truncate(2.5 * Convert.ToInt32(128 * Math.Abs(price - trunc))));	
				if (fraction < 10)
					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
				else if (fraction < 100)
					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
				else if(fraction == 320)
				{	
					trunc = trunc + 1;
					fraction = 0;
					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
				}	
				else	
					priceMarker = trunc.ToString() + "'" + fraction.ToString();
			}
			else
				priceMarker = price.ToString(Gui.Globals.GetTickFormatString(TickSize));
			return priceMarker;
		}		
		
		/// <summary>
		/// Have the .GetBars() call in a seperate thread. Reason: NT internal pool locking would not work if called in main thread.
		/// </summary>
		/// <param name="state"></param>
		private void GetBarsNow(object state)
		{
			if (Disposed)
				return;

			dailyBars			= Data.Bars.GetBars(Bars.Instrument, new Period(PeriodType.Day, 1, Bars.Period.MarketDataType), Bars.From, Bars.To, (Session) Bars.Session.Clone(), Data.Bars.SplitAdjust, Data.Bars.DividendAdjust);
			existsHistDailyData	= (dailyBars.Count <= 1) ? false : true;
			isDailyDataLoaded	= true;
			Enabled				= true;

			Cbi.Globals.SynchronizeInvoke.AsyncInvoke(new System.Windows.Forms.MethodInvoker(InvalidateNow), null);
		}

		private DateTime GetLastBarSessionDate(DateTime time, Data.Bars bars, int barsAgo, PivotRange pivotRange)
		{
			if (pivotRange == PivotRange.Daily && time > cacheSessionEndTmp1)
			{
				isEndOfWeekHoliday = false;
				if (Bars.BarsType.IsIntraday)
					sessionDateTmp = Bars.GetTradingDayFromLocal(time);
				else
					sessionDateTmp = time.Date;
				for (int i =0; i<4; i++)
				{
					if (publicHoliday[i].Date == sessionDateTmp)
						isEndOfWeekHoliday = true;
				}
				if(cacheSessionDate != sessionDateTmp) 
					cacheSessionDate = sessionDateTmp;
				Bars.Session.GetNextBeginEnd(bars, barsAgo, out cacheSessionBeginTmp, out cacheSessionEndTmp1);
				if(tickBuilt)
					cacheSessionEndTmp1 = cacheSessionEndTmp1.AddSeconds(-1);
			}
			else if (pivotRange == PivotRange.Weekly && time > cacheSessionEndTmp2) 
			{
				extendPublicHoliday = false;
				for (int i =0; i<4; i++)
				{
					if (publicHoliday[i].Date == sessionDateTmp)
						extendPublicHoliday = true;
				}
				if (Bars.BarsType.IsIntraday)
					sessionDateTmp = Bars.GetTradingDayFromLocal(time);
				else
					sessionDateTmp = time.Date;
				DateTime tmpWeeklyEndDate = RoundUpTimeToPeriodTime(sessionDateTmp, PivotRange.Weekly);
				if(extendPublicHoliday)
					tmpWeeklyEndDate = RoundUpTimeToPeriodTime(sessionDateTmp.AddDays(1), PivotRange.Weekly);
				if (tmpWeeklyEndDate != cacheWeeklyEndDate)
				{
					cacheWeeklyEndDate = tmpWeeklyEndDate;
					if (newSessionBarIdxArr2.Count == 0 || (newSessionBarIdxArr2.Count > 0 && CurrentBar > (int) newSessionBarIdxArr2[newSessionBarIdxArr2.Count - 1]))
						newSessionBarIdxArr2.Add(CurrentBar);
				}
				Bars.Session.GetNextBeginEnd(bars, barsAgo, out cacheSessionBeginTmp, out cacheSessionEndTmp2);
				if(tickBuilt)
					cacheSessionEndTmp2 = cacheSessionEndTmp2.AddSeconds(-1);
			}	
			if (pivotRange == PivotRange.Daily)
				return sessionDateTmp;
			else
				return RoundUpTimeToPeriodTime(sessionDateTmp, PivotRange.Weekly);
		}

		private DateTime RoundUpTimeToPeriodTime(DateTime time, PivotRange pivotRange)
		{
			if (pivotRange == PivotRange.Weekly)
			{
				DateTime periodStart = time.AddDays((6 - (((int) time.DayOfWeek) + 1) % 7));
				return periodStart.Date.AddDays(System.Math.Ceiling(System.Math.Ceiling(time.Date.Subtract(periodStart.Date).TotalDays) / 7) * 7).Date;
			}
			else if (pivotRange == PivotRange.Monthly)
			{
				DateTime result = new DateTime(time.Year, time.Month, 1); 
				return result.AddMonths(1).AddDays(-1);
			}
			else
				return time;
		}
		
		internal void InvalidateNow()
		{
			if (Disposed || ChartControl == null)
				return;

			ChartControl.Invalidate(true);
		}

		/// <summary>
        /// Overload this method to handle the termination of an indicator. Use this method to dispose of any resources vs overloading the Dispose() method.
		/// </summary>
		protected override void OnTermination()
		{
			if (dailyBars != null)
				dailyBars.Dispose();

			pivotBrush.Dispose();
			centralBrush.Dispose();
			jacksonBrush.Dispose();
			errorBrush.Dispose();
			foreach (SolidBrush solidBrush in brushes)
				solidBrush.Dispose();
			pivotLinePen.Dispose();
			stringFormatCenter.Dispose();
			stringFormatFar.Dispose();	
			stringFormatNear.Dispose();
		}
	
		/// <summary>
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="bounds"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			if (Bars == null || ChartControl == null || this.LastBarIndexPainted < BarsRequired)
				return;

			DateTime lastBarSessionBegin = Cbi.Globals.MinDate;
			DateTime lastBarSessionEnd = Cbi.Globals.MinDate;
			Bars.Session.GetNextBeginEnd(Bars.Get(this.LastBarIndexPainted).Time, out lastBarSessionBegin, out lastBarSessionEnd);
			SizeF errorSize	= graphics.MeasureString(errorData4, errorFont);
			errorTextHeight	= errorSize.Height + 5;
			if (Bars.Count > 0 && plotStart.Date > lastBarSessionBegin.Date && selectedSession != anaSessionTypePW43.UserDefined)
				graphics.DrawString(errorData4, errorFont, errorBrush, bounds.X + bounds.Width, bounds.Y + bounds.Height - errorTextHeight, stringFormatFar);

			int lastBarIndex		= this.LastBarIndexPainted;
			int firstBarIndex		= Math.Max(BarsRequired, this.FirstBarIndexPainted - 1);

			bool firstLoop = true;
			do
			{	
				if (!Values[0].IsValidPlot(lastBarIndex)) 
					return;
				int	firstBarIdxToPaint	= -1;
				for (int i = newSessionBarIdxArr2.Count - 1; i >= 0; i--)
				{
					int prevSessionBreakIdx = (int) newSessionBarIdxArr2[i];
					if (prevSessionBreakIdx <= lastBarIndex)
					{
						firstBarIdxToPaint 	= prevSessionBreakIdx - 1;
						break;
					}
				}
				int lastBarPlotIndex 	= (Bars.BarsType.IsIntraday && CalculateOnBarClose && firstLoop && CurrentBar < ChartControl.LastBarPainted) ? lastBarIndex + 1 : lastBarIndex; 
				int firstBarPlotIndex 	= Math.Max(firstBarIndex, firstBarIdxToPaint); 
				int sessionGap			= Math.Min(10, Math.Max(3, ChartControl.BarWidth));
				int firstXtoFill		= ChartControl.GetXByBarIdx(BarsArray[0], lastBarPlotIndex);
				int lastXtoFill 		= ChartControl.GetXByBarIdx(BarsArray[0], firstBarPlotIndex) + sessionGap;
				int[] yArr 				= new int[Values.Length];
				string[] labels			= new String [Values.Length];
				Color[]	plotColors		= new Color [Values.Length];
				
				for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
				{
					yArr[seriesCount] 	= ChartControl.GetYByValue(this, Values[seriesCount].Get(lastBarIndex));
					labels[seriesCount] = Plots[seriesCount].Name;
					plotColors[seriesCount] = Plots[seriesCount].Pen.Color;
				}
				if(yArr[26] == yArr[23] && vwapColor != Color.Transparent && closeColor != Color.Transparent)
				{
					labels[26] = "";
					plotColors[26] = Color.Transparent;
					if(yArr[0] != yArr[23])
						labels[23] = Plots[23].Name + "/" + Plots[26].Name;
				}
				if(yArr[26] == yArr[24] && vwapColor != Color.Transparent && centralPivotColor != Color.Transparent)
				{
					labels[26] = "";
					plotColors[26] = Color.Transparent;
					if(yArr[0] != yArr[24])
						labels[24] = Plots[24].Name + "/" + Plots[26].Name;
				}
				if(yArr[26] == yArr[25] && vwapColor != Color.Transparent && directionalPivotColor != Color.Transparent)
				{
					labels[26] = "";
					plotColors[26] = Color.Transparent;
					if(yArr[0] != yArr[25])
						labels[25] = Plots[25].Name + "/" + Plots[26].Name;
				}
				if(yArr[26] == yArr[0] && vwapColor != Color.Transparent && directionalPivotColor != Color.Transparent)
				{
					labels[26] = "";
					plotColors[26] = Color.Transparent;
					if(yArr[0] != yArr[23])
						labels[0] = Plots[0].Name + "/" + Plots[26].Name;
				}
				for(int i = 25; i >= 24; i--)
				{
					if(yArr[i] == yArr[0] && pivotColor != Color.Transparent)
					{
						labels[i] = "";
						plotColors[i] = Color.Transparent;
						if(yArr[0] != yArr[23])
							labels[0] = Plots[0].Name + "/" + Plots[i].Name;
					}
					else if (yArr[i] == yArr[23] && closeColor != Color.Transparent)
					{
						labels[i] = "";
						plotColors[i] = Color.Transparent;
						labels[23] = Plots[23].Name + "/" + Plots[i].Name;
					}	
				}
				if (yArr[23] == yArr[0] && closeColor != Color.Transparent && pivotColor != Color.Transparent)
				{
					labels[23] = Plots[23].Name+"/" + Plots[0].Name; 
					labels[0] = "";
					plotColors[0] = Color.Transparent;
				}
				if (yArr[23] == yArr[21] && closeColor != Color.Transparent && highColor != Color.Transparent)
				{
					labels[23] = Plots[23].Name+"/" + Plots[21].Name; 
					labels[21] = "";
					plotColors[21] = Color.Transparent;
				}
				if (yArr[23] == yArr[22] && closeColor != Color.Transparent && lowColor != Color.Transparent)
				{
					labels[23] = Plots[23].Name+"/" + Plots[22].Name; 
					labels[22] = "";
					plotColors[22] = Color.Transparent;
				}
				if (yArr[21] == yArr[1] && highColor != Color.Transparent && resistanceColor != Color.Transparent)
				{
					labels[21] = Plots[21].Name+"/" + Plots[1].Name; 
					labels[1] = "";
					plotColors[1] = Color.Transparent;
				}
				if (yArr[21] == yArr[3] && highColor != Color.Transparent && resistanceColor != Color.Transparent)
				{
					labels[21] = Plots[21].Name+"/" + Plots[3].Name; 
					labels[3] = "";
					plotColors[3] = Color.Transparent;
				}
				if (yArr[22] == yArr[2] && lowColor != Color.Transparent && supportColor != Color.Transparent)
				{
					labels[22] = Plots[22].Name+"/" + Plots[2].Name; 
					labels[2] = "";
					plotColors[2] = Color.Transparent;
				}
				if (yArr[22] == yArr[4] && lowColor != Color.Transparent && supportColor != Color.Transparent)
				{
					labels[22] = Plots[22].Name+"/" + Plots[4].Name; 
					labels[4] = "";
					plotColors[4] = Color.Transparent;
				}
				if(showMidpivots)
				{
					for(int i = 11; i <= 14; i++)
						for(int j = 21; j <= 26; j++)
							if (yArr[i] == yArr[j])
							{
								labels[i] = "";
								plotColors[i] = Color.Transparent;
							}
				}
				
				if(PivotFormula != anaPivotStylesPW43.Camarilla)
				{	
					if(opacityPR > 0 && yArr[25] > yArr[24])
					{
						graphics.DrawLine(pivotLinePen, lastXtoFill, yArr[0], firstXtoFill, yArr[0]);
						graphics.FillRectangle(pivotBrush, lastXtoFill, yArr[0], firstXtoFill - lastXtoFill, yArr[25]-yArr[0]);
						graphics.FillRectangle(centralBrush, lastXtoFill, yArr[24], firstXtoFill - lastXtoFill, yArr[0]-yArr[24]);
					}
					else if(opacityPR > 0 && yArr[25] < yArr[24])
					{
						graphics.DrawLine(pivotLinePen, lastXtoFill, yArr[0], firstXtoFill, yArr[0]);
						graphics.FillRectangle(pivotBrush, lastXtoFill, yArr[25], firstXtoFill - lastXtoFill, yArr[0]-yArr[25]);
						graphics.FillRectangle(centralBrush, lastXtoFill, yArr[0], firstXtoFill - lastXtoFill, yArr[24]-yArr[0]);
					}
					else if (opacityPR > 0)
					{	
						graphics.DrawLine(pivotLinePen, lastXtoFill, yArr[0], firstXtoFill, yArr[0]);
						graphics.FillRectangle(pivotBrush, lastXtoFill, Math.Min(yArr[24],yArr[25]) - 5, firstXtoFill - lastXtoFill, 10);
					}
				}
				if (PivotFormula == anaPivotStylesPW43.Jackson && opacityJZ > 0)
				{
					graphics.FillRectangle(jacksonBrush, lastXtoFill, yArr[3], firstXtoFill - lastXtoFill, yArr[1]-yArr[3]); 
					graphics.FillRectangle(jacksonBrush, lastXtoFill, yArr[2], firstXtoFill - lastXtoFill, yArr[4]-yArr[2]); 
					graphics.FillRectangle(jacksonBrush, lastXtoFill, yArr[7], firstXtoFill - lastXtoFill, yArr[5]-yArr[7]); 
					graphics.FillRectangle(jacksonBrush, lastXtoFill, yArr[6], firstXtoFill - lastXtoFill, yArr[8]-yArr[6]); 
				}
				for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
				{
					if (!Values[seriesCount].IsValidPlot(lastBarIndex)) 
						continue;
					SmoothingMode oldSmoothingMode 	= graphics.SmoothingMode;
					Gui.Chart.Plot	plot			= Plots[seriesCount];
					SolidBrush		brush			= brushes[seriesCount];
					int x 							= 0;
					int firstX						= -1;
					int lastX						= -1;
					using (GraphicsPath	path = new GraphicsPath()) 
					{
						if (brush.Color != plotColors[seriesCount])	
							brush = new SolidBrush(plotColors[seriesCount]);
						for (int idx = lastBarPlotIndex; idx >= Math.Max(firstBarPlotIndex, lastBarPlotIndex - Width); idx--)
						{
							if (idx - Displacement >= Bars.Count) 
								continue;
							else if (idx - Displacement < 0)
								break;
							x	= Math.Max(ChartControl.GetXByBarIdx(BarsArray[0], idx), lastXtoFill);
							if (lastX >= 0)
								path.AddLine(lastX - plot.Pen.Width / 2, yArr[seriesCount], x - plot.Pen.Width / 2, yArr[seriesCount]);
							lastX	= x;
							if (idx == lastBarPlotIndex)
									firstX = x;
						}
						graphics.SmoothingMode = SmoothingMode.AntiAlias;
						Color cacheColor = plot.Pen.Color;
						plot.Pen.Color = plotColors[seriesCount]; 							
						graphics.DrawPath(plot.Pen, path);
						plot.Pen.Color = cacheColor;
						graphics.SmoothingMode = oldSmoothingMode;
						if (PlotLabels == anaPlotAlignPW43.RightAll || (firstLoop && PlotLabels == anaPlotAlignPW43.Right))
						graphics.DrawString(labels[seriesCount], labelFont, brush, firstX + labelOffset, yArr[seriesCount] - labelFont.GetHeight() / 2, stringFormatNear);
						if (PlotLabels == anaPlotAlignPW43.LeftAll || (firstLoop && PlotLabels == anaPlotAlignPW43.Left))
						graphics.DrawString(labels[seriesCount], labelFont, brush, lastX - labelOffset, yArr[seriesCount] - labelFont.GetHeight() / 2, stringFormatFar);
					}
				}
				lastBarIndex = firstBarIdxToPaint;
				firstLoop = false;
			}
			while (showPriorPeriods && lastBarIndex > firstBarIndex); 
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
        private anaPivotsWeeklyV43[] cacheanaPivotsWeeklyV43 = null;

        private static anaPivotsWeeklyV43 checkanaPivotsWeeklyV43 = new anaPivotsWeeklyV43();

        /// <summary>
        /// Weekly Session Pivots.
        /// </summary>
        /// <returns></returns>
        public anaPivotsWeeklyV43 anaPivotsWeeklyV43(anaSessionCountPW43 activeSession, anaCalcModePW43 calcMode, anaPivotStylesPW43 pivotFormula, anaSessionTypePW43 pivotSession, bool showMidpivots, bool showPriorPeriods, double userDefinedClose, double userDefinedHigh, double userDefinedLow)
        {
            return anaPivotsWeeklyV43(Input, activeSession, calcMode, pivotFormula, pivotSession, showMidpivots, showPriorPeriods, userDefinedClose, userDefinedHigh, userDefinedLow);
        }

        /// <summary>
        /// Weekly Session Pivots.
        /// </summary>
        /// <returns></returns>
        public anaPivotsWeeklyV43 anaPivotsWeeklyV43(Data.IDataSeries input, anaSessionCountPW43 activeSession, anaCalcModePW43 calcMode, anaPivotStylesPW43 pivotFormula, anaSessionTypePW43 pivotSession, bool showMidpivots, bool showPriorPeriods, double userDefinedClose, double userDefinedHigh, double userDefinedLow)
        {
            if (cacheanaPivotsWeeklyV43 != null)
                for (int idx = 0; idx < cacheanaPivotsWeeklyV43.Length; idx++)
                    if (cacheanaPivotsWeeklyV43[idx].ActiveSession == activeSession && cacheanaPivotsWeeklyV43[idx].CalcMode == calcMode && cacheanaPivotsWeeklyV43[idx].PivotFormula == pivotFormula && cacheanaPivotsWeeklyV43[idx].PivotSession == pivotSession && cacheanaPivotsWeeklyV43[idx].ShowMidpivots == showMidpivots && cacheanaPivotsWeeklyV43[idx].ShowPriorPeriods == showPriorPeriods && Math.Abs(cacheanaPivotsWeeklyV43[idx].UserDefinedClose - userDefinedClose) <= double.Epsilon && Math.Abs(cacheanaPivotsWeeklyV43[idx].UserDefinedHigh - userDefinedHigh) <= double.Epsilon && Math.Abs(cacheanaPivotsWeeklyV43[idx].UserDefinedLow - userDefinedLow) <= double.Epsilon && cacheanaPivotsWeeklyV43[idx].EqualsInput(input))
                        return cacheanaPivotsWeeklyV43[idx];

            lock (checkanaPivotsWeeklyV43)
            {
                checkanaPivotsWeeklyV43.ActiveSession = activeSession;
                activeSession = checkanaPivotsWeeklyV43.ActiveSession;
                checkanaPivotsWeeklyV43.CalcMode = calcMode;
                calcMode = checkanaPivotsWeeklyV43.CalcMode;
                checkanaPivotsWeeklyV43.PivotFormula = pivotFormula;
                pivotFormula = checkanaPivotsWeeklyV43.PivotFormula;
                checkanaPivotsWeeklyV43.PivotSession = pivotSession;
                pivotSession = checkanaPivotsWeeklyV43.PivotSession;
                checkanaPivotsWeeklyV43.ShowMidpivots = showMidpivots;
                showMidpivots = checkanaPivotsWeeklyV43.ShowMidpivots;
                checkanaPivotsWeeklyV43.ShowPriorPeriods = showPriorPeriods;
                showPriorPeriods = checkanaPivotsWeeklyV43.ShowPriorPeriods;
                checkanaPivotsWeeklyV43.UserDefinedClose = userDefinedClose;
                userDefinedClose = checkanaPivotsWeeklyV43.UserDefinedClose;
                checkanaPivotsWeeklyV43.UserDefinedHigh = userDefinedHigh;
                userDefinedHigh = checkanaPivotsWeeklyV43.UserDefinedHigh;
                checkanaPivotsWeeklyV43.UserDefinedLow = userDefinedLow;
                userDefinedLow = checkanaPivotsWeeklyV43.UserDefinedLow;

                if (cacheanaPivotsWeeklyV43 != null)
                    for (int idx = 0; idx < cacheanaPivotsWeeklyV43.Length; idx++)
                        if (cacheanaPivotsWeeklyV43[idx].ActiveSession == activeSession && cacheanaPivotsWeeklyV43[idx].CalcMode == calcMode && cacheanaPivotsWeeklyV43[idx].PivotFormula == pivotFormula && cacheanaPivotsWeeklyV43[idx].PivotSession == pivotSession && cacheanaPivotsWeeklyV43[idx].ShowMidpivots == showMidpivots && cacheanaPivotsWeeklyV43[idx].ShowPriorPeriods == showPriorPeriods && Math.Abs(cacheanaPivotsWeeklyV43[idx].UserDefinedClose - userDefinedClose) <= double.Epsilon && Math.Abs(cacheanaPivotsWeeklyV43[idx].UserDefinedHigh - userDefinedHigh) <= double.Epsilon && Math.Abs(cacheanaPivotsWeeklyV43[idx].UserDefinedLow - userDefinedLow) <= double.Epsilon && cacheanaPivotsWeeklyV43[idx].EqualsInput(input))
                            return cacheanaPivotsWeeklyV43[idx];

                anaPivotsWeeklyV43 indicator = new anaPivotsWeeklyV43();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ActiveSession = activeSession;
                indicator.CalcMode = calcMode;
                indicator.PivotFormula = pivotFormula;
                indicator.PivotSession = pivotSession;
                indicator.ShowMidpivots = showMidpivots;
                indicator.ShowPriorPeriods = showPriorPeriods;
                indicator.UserDefinedClose = userDefinedClose;
                indicator.UserDefinedHigh = userDefinedHigh;
                indicator.UserDefinedLow = userDefinedLow;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaPivotsWeeklyV43[] tmp = new anaPivotsWeeklyV43[cacheanaPivotsWeeklyV43 == null ? 1 : cacheanaPivotsWeeklyV43.Length + 1];
                if (cacheanaPivotsWeeklyV43 != null)
                    cacheanaPivotsWeeklyV43.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaPivotsWeeklyV43 = tmp;
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
        /// Weekly Session Pivots.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaPivotsWeeklyV43 anaPivotsWeeklyV43(anaSessionCountPW43 activeSession, anaCalcModePW43 calcMode, anaPivotStylesPW43 pivotFormula, anaSessionTypePW43 pivotSession, bool showMidpivots, bool showPriorPeriods, double userDefinedClose, double userDefinedHigh, double userDefinedLow)
        {
            return _indicator.anaPivotsWeeklyV43(Input, activeSession, calcMode, pivotFormula, pivotSession, showMidpivots, showPriorPeriods, userDefinedClose, userDefinedHigh, userDefinedLow);
        }

        /// <summary>
        /// Weekly Session Pivots.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaPivotsWeeklyV43 anaPivotsWeeklyV43(Data.IDataSeries input, anaSessionCountPW43 activeSession, anaCalcModePW43 calcMode, anaPivotStylesPW43 pivotFormula, anaSessionTypePW43 pivotSession, bool showMidpivots, bool showPriorPeriods, double userDefinedClose, double userDefinedHigh, double userDefinedLow)
        {
            return _indicator.anaPivotsWeeklyV43(input, activeSession, calcMode, pivotFormula, pivotSession, showMidpivots, showPriorPeriods, userDefinedClose, userDefinedHigh, userDefinedLow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Weekly Session Pivots.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaPivotsWeeklyV43 anaPivotsWeeklyV43(anaSessionCountPW43 activeSession, anaCalcModePW43 calcMode, anaPivotStylesPW43 pivotFormula, anaSessionTypePW43 pivotSession, bool showMidpivots, bool showPriorPeriods, double userDefinedClose, double userDefinedHigh, double userDefinedLow)
        {
            return _indicator.anaPivotsWeeklyV43(Input, activeSession, calcMode, pivotFormula, pivotSession, showMidpivots, showPriorPeriods, userDefinedClose, userDefinedHigh, userDefinedLow);
        }

        /// <summary>
        /// Weekly Session Pivots.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaPivotsWeeklyV43 anaPivotsWeeklyV43(Data.IDataSeries input, anaSessionCountPW43 activeSession, anaCalcModePW43 calcMode, anaPivotStylesPW43 pivotFormula, anaSessionTypePW43 pivotSession, bool showMidpivots, bool showPriorPeriods, double userDefinedClose, double userDefinedHigh, double userDefinedLow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaPivotsWeeklyV43(input, activeSession, calcMode, pivotFormula, pivotSession, showMidpivots, showPriorPeriods, userDefinedClose, userDefinedHigh, userDefinedLow);
        }
    }
}
#endregion
