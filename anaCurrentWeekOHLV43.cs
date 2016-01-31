#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

#region Global Enums

public enum anaPlotAlignCW43 {Left, Right, DoNotPlot}
public enum anaSessionTypeCW43 {ETH, RTH}
public enum anaSessionCountCW43 {First, Second, Third, Auto}
public enum anaTargetTypeCW43 {Average_Weekly_Range, Average_Weekly_Expansion}
public enum anaDataLocationCW43 {Right_Format_Long, Left_Format_Long, Center_Format_Long, Right_Format_Short, Left_Format_Short, Center_Format_Short, DoNotPlot}

#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Current Open, High, Low and Fibonacci Levels.
	/// </summary>
	[Description("anaCurrentWeekOHLV43.")]
	public class anaCurrentWeekOHLV43 : Indicator
	{
		#region Variables
		private	SolidBrush[]		brushes					= { new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black), 
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black), 
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black),  
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black)};  
		private DateTime 			sessionBegin			= Cbi.Globals.MinDate;
		private DateTime 			sessionEnd 				= Cbi.Globals.MinDate;
		private DateTime			plotStart1				= Cbi.Globals.MaxDate;
		private DateTime			plotStart2				= Cbi.Globals.MaxDate;
		private DateTime			cacheSessionBeginTmp   	= Cbi.Globals.MinDate;
		private DateTime			cacheSessionEndTmp1		= Cbi.Globals.MinDate;
		private DateTime			cacheSessionEndTmp2		= Cbi.Globals.MinDate;
		private DateTime			cacheSessionDate		= Cbi.Globals.MinDate;
		private DateTime			cacheWeeklyEndDate		= Cbi.Globals.MinDate;
		private DateTime			sessionDateTmp			= Cbi.Globals.MinDate;
		private DateTime			currentDate				= Cbi.Globals.MinDate;
		private DateTime			currentWeek				= Cbi.Globals.MinDate;
		private DateTime			lastBarTimeStamp1		= Cbi.Globals.MinDate;
		private DateTime			lastBarTimeStamp2		= Cbi.Globals.MinDate;
		private DateTime			highTime				= Cbi.Globals.MinDate;
		private DateTime			lowTime					= Cbi.Globals.MinDate;
		private double				currentOpen				= 0.0;
		private	double				currentHigh				= double.MinValue;
		private	double				currentLow				= double.MaxValue;
		private double				currentMidline			= 0.0;
		private double				fib786					= 0.0;
		private double				fib618					= 0.0;
		private double				fib382					= 0.0;
		private double				fib236					= 0.0;
		private double				displaySize				= 0.0;
		private double				averageNoise1			= 0.0;
		private double				averageNoise2			= 0.0;
		private double				averageNoise			= 0.0;
		private double				averageRange1			= 0.0;
		private double				averageRange2			= 0.0;
		private double				averageRange			= 0.0;
		private double				displayedNoise			= 0.0;
		private double				displayedRange			= 0.0;
		private double				bandWidth				= 0.0;
		private double				upperNoiseTarget		= 0.0;
		private double				lowerNoiseTarget		= 0.0;
		private double				upperRangeTarget		= 0.0;
		private double				lowerRangeTarget		= 0.0;
		private bool				tickBuilt				= false;
		private bool				plotOHL					= false;
		private bool				calcOpen				= false;
		private bool				initPlot				= false;
		private bool				extendPublicHoliday		= false; 
		private bool				isEndOfWeekHoliday		= false; 
		private bool				firstDayOfPeriod		= false;
		private bool				rthOHL					= false;
		private bool				plotCurrentOpen			= false;
		private bool				plotFibonacci			= false;
		private bool				plotHiLo				= false;
		private bool				plotMidline				= false;
		private bool				plotNoiseLevels			= false;
		private bool				plotTargetLevels		= false;
		private bool				showCurrentOpen			= true;
		private bool				showFibonacci			= false;
		private bool				showHiLo				= true;
		private bool				showMidline				= false;
		private bool				showNoiseLevels			= true;
		private bool				showTargetLevels		= true;
		private bool				showNoiseBands			= true;
		private bool				showTargetBands			= true;
		private bool				showPriorPeriods		= true;
		private bool				isCurrency				= false;
		private bool				isGlobex				= false;
		private bool				targetHit				= false;
		private ArrayList			newSessionBarIdxArr2	= new ArrayList();
		private anaPlotAlignCW43	plotLabels				= anaPlotAlignCW43.Right;
		private anaDataLocationCW43 showRangeData			= anaDataLocationCW43.Center_Format_Long;
		private anaSessionTypeCW43	currentSession			= anaSessionTypeCW43.ETH;
		private anaSessionCountCW43 selectedSession			= anaSessionCountCW43.Auto;
		private anaSessionCountCW43 activeSession			= anaSessionCountCW43.Auto;
		private anaTargetTypeCW43	targetType				= anaTargetTypeCW43.Average_Weekly_Range;
		private Data.PivotRange		pivotRangeType1			= PivotRange.Daily;
		private Data.PivotRange		pivotRangeType2			= PivotRange.Weekly;
		private SolidBrush			noiseBrush				= new SolidBrush(Color.LightSteelBlue);
		private SolidBrush			targetBrush				= new SolidBrush(Color.LightSteelBlue);
		private SolidBrush			textBrush				= new SolidBrush(Color.White);
		private SolidBrush			dataBrush				= new SolidBrush(Color.Navy);
		private SolidBrush			errorBrush				= new SolidBrush(Color.White);
		private Font				labelFont				= new Font("Arial", 8);
		private Font				textFont				= new Font("Arial", 10);
		private Font				errorFont				= new Font("Arial", 10);
		private	string 				noiseText0 				= string.Empty;
		private	string 				noiseText1 				= string.Empty;
		private	string 				noiseText2 				= string.Empty;
		private	string 				noiseData0 				= string.Empty;
		private	string 				noiseData1 				= string.Empty;
		private	string 				noiseData2 				= string.Empty;
		private	string 				rangeText0 				= string.Empty;
		private	string 				rangeText1 				= string.Empty;
		private	string 				rangeText2 				= string.Empty;
		private	string 				rangeData0 				= string.Empty;
		private	string 				rangeData1 				= string.Empty;
		private	string 				rangeData2 				= string.Empty;
		private	string				errorData2				= "The Current Week OHL indicator Indicator did not find a third intraday session.";
		private	string				errorData4				= "Insufficient historical data for the Current Week OHL indicator. Please increase the chart look back period.";
		private string				errorData5				= "Insufficient historical data to calculate weekly noise and volatility. With the selected settings, the Current Week OHL indicator may require a chart lookback period of up to ";
		private string				errorData5b				= " weeks and 2 days.";
        private float				errorTextHeight			= 0;
		private	StringFormat		stringFormatFar			= new StringFormat();
		private	StringFormat		stringFormatNear		= new StringFormat();
		private	StringFormat		stringFormatCenter		= new StringFormat();
		private int					countDown				= 0;
		private int					numberOfSessions		= 1;
		private int					sessionCount			= 0;
		private int					rangePeriod				= 0;
		private int					rangePeriod1			= 4;
		private int					rangePeriod2			= 8;
		private int					weekCount				= 0;
		private int					factorNoiseBands		= 100;
		private int					factorWeeklyRange		= 100;
		private int					width					= 20;
		private int 				labelFontSize			= 9;
		private int					labelOffset				= 75;
		private int 				textFontSize			= 10;
		private int					opacityN				= 4;
		private int					opacityT				= 4;
		private int					opacityD				= 10;
		private int					displayFactor			= 1;
		private Color 				openColor				= Color.Plum;
		private Color 				highColor				= Color.Lime;
		private Color 				lowColor				= Color.Red;
		private Color 				midlineColor			= Color.PaleGoldenrod;
		private Color 				fibColor				= Color.PaleGoldenrod;
		private Color 				noiseBandColor			= Color.PaleGreen;
		private Color 				targetBandColor			= Color.MediumOrchid;
		private Color				textColor				= Color.White;
		private Color				dataTableColor			= Color.Navy;
		private int 				plot0Width 				= 1;
		private DashStyle 			dash0Style 				= DashStyle.Solid;
		private int 				plot1Width 				= 1;
		private DashStyle 			dash1Style 				= DashStyle.Solid;
		private int 				plot3Width 				= 1;
		private DashStyle 			dash3Style 				= DashStyle.Solid;
		private int 				plot8Width 				= 1;
		private DashStyle 			dash8Style 				= DashStyle.Solid;
		private DateTime			publicHoliday0			= new DateTime (2009,07,03);
		private DateTime			publicHoliday1			= new DateTime (2014,07,04);
		private DateTime			publicHoliday2			= new DateTime (2015,07,03);
		private DateTime			publicHoliday3;
		private DateTime[]			publicHoliday			= new DateTime[4];
		private List<double>		noiseList				= new List<double>();
		private List<double>		rangeList				= new List<double>();
		private DataSeries			displayedWidth;
		private DataSeries			AWN0;
		private DataSeries			AWN1;
		private DataSeries			AWN2;
		private DataSeries			AWR0;
		private DataSeries			AWR1;
		private DataSeries			AWR2;
		private BoolSeries			lowIsLast;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-Open "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-High "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-Low "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-Mid "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-78,6 "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-61,8 "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-38,2 "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"W-23,6 "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"AWN-High"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"AWN-Low"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"AWR-High"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"AWR-Low"));
			
			AutoScale						= false;
			Overlay							= true;
			PriceTypeSupported				= false;
			PlotsConfigurable				= false;
			MaximumBarsLookBack 			= MaximumBarsLookBack.Infinite;
			BarsRequired					= 0;
			ZOrder							= 1;
			stringFormatNear.Alignment 		= StringAlignment.Near;
			stringFormatCenter.Alignment 	= StringAlignment.Center;
			stringFormatFar.Alignment		= StringAlignment.Far;
			
			AWN0							= new DataSeries(this);
			AWN1 							= new DataSeries(this);
			AWN2							= new DataSeries(this);
			AWR0							= new DataSeries(this);
			AWR1							= new DataSeries(this);
			AWR2							= new DataSeries(this);
			displayedWidth					= new DataSeries(this);
			lowIsLast 						= new BoolSeries(this);
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

			displayFactor = 1;
			if (Instrument.MasterInstrument.InstrumentType == Cbi.InstrumentType.Currency || Instrument.MasterInstrument.Name == "DX"|| Instrument.MasterInstrument.Name == "6A"
				|| Instrument.MasterInstrument.Name == "6B" || Instrument.MasterInstrument.Name == "6C" ||Instrument.MasterInstrument.Name == "6E"
				|| Instrument.MasterInstrument.Name == "6J" || Instrument.MasterInstrument.Name == "6M" || Instrument.MasterInstrument.Name == "6S"
				|| Instrument.MasterInstrument.Name == "6N" || Instrument.MasterInstrument.Name == "E7" || Instrument.MasterInstrument.Name == "J7"
				|| Instrument.MasterInstrument.Name == "M6A" || Instrument.MasterInstrument.Name == "M6B" || Instrument.MasterInstrument.Name == "M6C"
				|| Instrument.MasterInstrument.Name == "M6E" || Instrument.MasterInstrument.Name == "M6J" || Instrument.MasterInstrument.Name == "M6S")
			{
				isCurrency = true;
				if (Instrument.MasterInstrument.Name == "6J")
					displayFactor = 1000000;
				else if (Instrument.MasterInstrument.Name == "USDJPY" || Instrument.MasterInstrument.Name == "EURJPY")
					displayFactor = 100;
				else
					displayFactor = 10000;
			}
			if (selectedSession == anaSessionCountCW43.Auto)
			{
				if (isCurrency)
					activeSession = anaSessionCountCW43.Third;
				else
					activeSession = anaSessionCountCW43.Second;
			}
			else 
				activeSession = selectedSession; 
			if(showRangeData == anaDataLocationCW43.Left_Format_Long || showRangeData == anaDataLocationCW43.Right_Format_Long || showRangeData == anaDataLocationCW43.Center_Format_Long) 
			{
				if (currentSession == anaSessionTypeCW43.ETH)
				{
					if(targetType== anaTargetTypeCW43.Average_Weekly_Range)
					{
						rangeText0 = "Current Weekly Range ETH = ";
						rangeText1 = "Average Weekly Range [" + rangePeriod1 + "] = ";
						rangeText2 = "Average Weekly Range [" + rangePeriod2 + "] = ";
					}
					else
					{
						rangeText0 = "Current Weekly Expansion ETH = ";
						rangeText1 = "Average Weekly Expansion [" + rangePeriod1 + "] = ";
						rangeText2 = "Average Weekly Expansion [" + rangePeriod2 + "] = ";
					}
					noiseText0 = "Current Weekly Noise ETH = ";
					noiseText1 = "Average Weekly Noise [" + rangePeriod1 + "] = ";
					noiseText2 = "Average Weekly Noise [" + rangePeriod2 + "] = ";
					rthOHL = false;
				}
				else
				{
					if(targetType== anaTargetTypeCW43.Average_Weekly_Range)
					{
						rangeText0 = "Current Weekly Range RTH = ";
						rangeText1 = "Average Weekly Range [" + rangePeriod1 + "] = ";
						rangeText2 = "Average Weekly Range [" + rangePeriod2 + "] = ";
					}
					else
					{
						rangeText0 = "Current Weekly Expansion RTH = ";
						rangeText1 = "Average Weekly Expansion [" + rangePeriod1 + "] = ";
						rangeText2 = "Average Weekly Expansion [" + rangePeriod2 + "] = ";
					}
					noiseText0 = "Current Weekly Noise RTH = ";
					noiseText1 = "Average Weekly Noise [" + rangePeriod1 + "] = ";
					noiseText2 = "Average Weekly Noise [" + rangePeriod2 + "] = ";
					rthOHL = true;
				}
			}
			else if(showRangeData == anaDataLocationCW43.Left_Format_Short || showRangeData == anaDataLocationCW43.Right_Format_Short || showRangeData == anaDataLocationCW43.Center_Format_Short) 
			{
				if (currentSession == anaSessionTypeCW43.ETH)
				{
					if(targetType== anaTargetTypeCW43.Average_Weekly_Range)
					{
						rangeText0 = "CWR ETH = ";
						rangeText1 = "AWR [" + rangePeriod1 + "] = ";
						rangeText2 = "AWR [" + rangePeriod2 + "] = ";
					}
					else
					{
						rangeText0 = "CWE ETH = ";
						rangeText1 = "AWE [" + rangePeriod1 + "] = ";
						rangeText2 = "AWE [" + rangePeriod2 + "] = ";
					}
					noiseText0 = "CWN ETH = ";
					noiseText1 = "AWN [" + rangePeriod1 + "] = ";
					noiseText2 = "AWN [" + rangePeriod2 + "] = ";
					rthOHL = false;
				}
				else
				{
					if(targetType== anaTargetTypeCW43.Average_Weekly_Range)
					{
						rangeText0 = "CWR RTH = ";
						rangeText1 = "AWR [" + rangePeriod1 + "] = ";
						rangeText2 = "AWR [" + rangePeriod2 + "] = ";
					}
					else
					{
						rangeText0 = "CWE RTH = ";
						rangeText1 = "AWE [" + rangePeriod1 + "] = ";
						rangeText2 = "AWE [" + rangePeriod2 + "] = ";
					}
					noiseText0 = "CWN RTH = ";
					noiseText1 = "AWN [" + rangePeriod1 + "] = ";
					noiseText2 = "AWN [" + rangePeriod2 + "] = ";
					rthOHL = true;
				}
			}
			else
			{
				if (currentSession == anaSessionTypeCW43.ETH)
					rthOHL = false;
				else
					rthOHL = true;
			}
			if(targetType == anaTargetTypeCW43.Average_Weekly_Range)
			{
				Plots[10].Name = "AWR-High";
				Plots[11].Name = "AWR-Low";
			}				
			else	
			{
				Plots[10].Name = "AWE-High";
				Plots[11].Name = "AWE-Low";
			}				
			labelFont = new Font ("Arial", labelFontSize);
			textFont = new Font ("Arial", textFontSize);
			textBrush = new SolidBrush(textColor);
			noiseBrush = new SolidBrush(Color.FromArgb(25 * opacityN, noiseBandColor));
			targetBrush = new SolidBrush(Color.FromArgb(25 * opacityT, targetBandColor));
			dataBrush = new SolidBrush(Color.FromArgb(25 * opacityD, dataTableColor));
			if (!showCurrentOpen || openColor == Color.Transparent)
				plotCurrentOpen = false;
			else
				plotCurrentOpen = true;
			if (!showHiLo && (highColor == Color.Transparent && lowColor == Color.Transparent))
				plotHiLo = false;
			else
				plotHiLo = true;
			if (!showMidline && midlineColor == Color.Transparent)
				plotMidline = false;
			else
				plotMidline = true;
			if (!showFibonacci || fibColor == Color.Transparent)
				plotFibonacci = false;
			else
				plotFibonacci = true;
			if (!showNoiseLevels || noiseBandColor == Color.Transparent)
				plotNoiseLevels = false;
			else
				plotNoiseLevels = true;
			if (!showTargetLevels || targetBandColor == Color.Transparent)
				plotTargetLevels = false;
			else
				plotTargetLevels = true;
			if (showCurrentOpen)
				Plots[0].Pen.Color = openColor;
			else
				Plots[0].Pen.Color = Color.Transparent;
			if (showHiLo)
			{
				Plots[1].Pen.Color = highColor;
				Plots[2].Pen.Color = lowColor;
			}
			else
			{
				Plots[1].Pen.Color = Color.Transparent;
				Plots[2].Pen.Color = Color.Transparent;
			}
			if(showMidline)
				Plots[3].Pen.Color = midlineColor;
			else
				Plots[3].Pen.Color = Color.Transparent;
			if (showFibonacci)
			{
				Plots[4].Pen.Color = fibColor;
				Plots[5].Pen.Color = fibColor;
				Plots[6].Pen.Color = fibColor;
				Plots[7].Pen.Color = fibColor;
			}
			else
			{
				Plots[4].Pen.Color = Color.Transparent;
				Plots[5].Pen.Color = Color.Transparent;
				Plots[6].Pen.Color = Color.Transparent;
				Plots[7].Pen.Color = Color.Transparent;
			}
			if (showNoiseLevels)
			{
				Plots[8].Pen.Color = noiseBandColor;
				Plots[9].Pen.Color = noiseBandColor;
			}
			else
			{
				Plots[8].Pen.Color = Color.Transparent;
				Plots[9].Pen.Color = Color.Transparent;
			}
			if(showTargetLevels)
			{
				Plots[10].Pen.Color = targetBandColor;
				Plots[11].Pen.Color = targetBandColor;
			}
			else
			{
				Plots[10].Pen.Color = Color.Transparent;
				Plots[11].Pen.Color = Color.Transparent;
			}
			Plots[0].Pen.Width = plot0Width;
			Plots[0].Pen.DashStyle = dash0Style;
			for (int i = 1; i < 3; i++)
			{
				Plots[i].Pen.Width = plot1Width;
				Plots[i].Pen.DashStyle = dash1Style;
			}
			for (int i = 3; i < 8; i++)
			{
				Plots[i].Pen.Width = plot3Width;
				Plots[i].Pen.DashStyle = dash3Style;
			}			
			for (int i = 8; i< 12; i++)
			{
				Plots[i].Pen.Width = plot8Width;
				Plots[i].Pen.DashStyle = dash8Style;
			}				
			rangePeriod = Math.Max(rangePeriod1, rangePeriod2);
			if(ChartControl != null)
			{
				errorBrush.Color = ChartControl.AxisColor;
				errorData5 = errorData5 + (rangePeriod + 1)+ errorData5b;
			}
			if (Instrument.MasterInstrument.InstrumentType == Cbi.InstrumentType.Currency && (TickSize == 0.00001 || TickSize == 0.001))
				displaySize = 5* TickSize;
			else
				displaySize = TickSize;
			if (AutoScale)
				AutoScale = false;
			if(Displacement != 0)
				Displacement = 0;
			noiseList  = new List<double>();
			rangeList  = new List<double>();
			for (int i=0; i < rangePeriod; i++)
			{
				noiseList.Add(0.0);
				rangeList.Add(0.0);
			}
			if (!Bars.BarsType.IsIntraday || BarsPeriod.Id == PeriodType.Minute || BarsPeriod.Id == PeriodType.Second)
				tickBuilt = false;
			else
				tickBuilt = true;
			countDown = 0;
			weekCount = 0;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (Bars == null)
				return; 
			if (!Data.BarsType.GetInstance(Bars.Period.Id).IsIntraday && Bars.Period.Id != PeriodType.Day)
				return;
			if (Bars.Period.Id == PeriodType.Day && Bars.Period.Value > 1)
				return;
			if (CurrentBar == 0)
			{	
				currentDate = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType1);
				currentWeek = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType2);
				sessionCount = 1;
				return;
			}

			lastBarTimeStamp1 = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType1);
			lastBarTimeStamp2 = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType2);
			if (isEndOfWeekHoliday && !rthOHL)
				lastBarTimeStamp2 = GetLastBarSessionDate(Time[0].AddDays(3), Bars, 0, pivotRangeType2);
			if (lastBarTimeStamp2 != currentWeek)
			{	
				if (countDown == 0)
				{
					Bars.Session.GetNextBeginEnd(Bars, 0, out sessionBegin, out sessionEnd);
					plotStart1 = sessionBegin;
					countDown = countDown -1;
				}	
				firstDayOfPeriod = true;
				sessionCount = 1;
				calcOpen = false;
				initPlot = false;
				targetHit = false;
				if(weekCount == rangePeriod)
				{
					Bars.Session.GetNextBeginEnd(Bars, 0, out sessionBegin, out sessionEnd);
					plotStart2 = sessionBegin;
				}
				weekCount = weekCount + 1;
				for (int i = rangePeriod - 1; i>0; i--)
				{
					noiseList[i] = noiseList[i-1];
					rangeList[i] = rangeList[i-1];
				}
				if (weekCount > 1)
				{
					noiseList [0] = Math.Min(currentHigh - currentOpen,currentOpen - currentLow);
					if(targetType== anaTargetTypeCW43.Average_Weekly_Range)
						rangeList [0] = currentHigh - currentLow;
					else
						rangeList[0] = Math.Max(currentHigh - currentOpen,currentOpen - currentLow);
				}
				if (weekCount > rangePeriod)
				{
					averageNoise1 = 0.0;
					averageRange1 = 0.0;
					for (int i=0; i<rangePeriod1; i++)
					{
						averageNoise1 = averageNoise1 + noiseList[i];
						averageRange1 = averageRange1 + rangeList[i];
					}
					averageNoise1 = averageNoise1/rangePeriod1;
					averageRange1 = averageRange1/rangePeriod1;
					averageNoise2 = 0.0;
					averageRange2 = 0.0;
					for (int i=0; i<rangePeriod2; i++)
					{
						averageNoise2 = averageNoise2 + noiseList[i];
						averageRange2 = averageRange2 + rangeList[i];
					}
					averageNoise2 = averageNoise2/rangePeriod2;
					averageRange2 = averageRange2/rangePeriod2;
					averageNoise = Math.Round(0.5*(averageNoise1 + averageNoise2)/displaySize) * displaySize;
					averageNoise1 = Math.Round(averageNoise1/displaySize) * displaySize;
					averageNoise2 = Math.Round(averageNoise2/displaySize) * displaySize;
					averageRange = Math.Round(0.5*(averageRange1 + averageRange2)/displaySize) * displaySize;
					averageRange1 = Math.Round(averageRange1/displaySize) * displaySize;
					averageRange2 = Math.Round(averageRange2/displaySize) * displaySize;
					displayedNoise = Math.Round(factorNoiseBands * averageNoise / (displaySize *100))*displaySize;
					displayedRange = Math.Round(factorWeeklyRange * averageRange / (displaySize *100))*displaySize;
					bandWidth = Math.Max(displaySize, 0.06*factorNoiseBands*averageNoise/100);
				}
				if (numberOfSessions == 1 || !rthOHL || (rthOHL && activeSession == anaSessionCountCW43.First))
				{
					calcOpen 		= true;
					initPlot 		= true;
					currentOpen		= Open[0];
					upperNoiseTarget 	= currentOpen + displayedNoise;
					lowerNoiseTarget 	= currentOpen - displayedNoise;
					if(targetType == anaTargetTypeCW43.Average_Weekly_Expansion)
					{
						upperRangeTarget 	= currentOpen + displayedRange;
						lowerRangeTarget 	= currentOpen - displayedRange;
					}
					currentHigh		= High[0];
					currentLow		= Low[0];
					highTime		= Time[0];
					lowTime			= Time[0];
				}
				currentDate = lastBarTimeStamp1;
				currentWeek = lastBarTimeStamp2;
				plotOHL = true;
			}
			else if (lastBarTimeStamp1 != currentDate || Bars.Period.Id == PeriodType.Day)
			{
				if (FirstTickOfBar)
				{
					firstDayOfPeriod = false;
					sessionCount = 1;
				}
				if (numberOfSessions == 1 || !rthOHL || (rthOHL && activeSession == anaSessionCountCW43.First))
				{
					calcOpen		= true;
					initPlot		= true;
					if (High[0] >= currentHigh)
					{
						currentHigh = High[0];
						highTime 	= Time[0];
					}
					if (Low[0] <= currentLow)
					{
						currentLow 	= Low[0];
						lowTime 	= Time[0];
					}
				}
				else
					calcOpen		= false;
				currentDate = lastBarTimeStamp1;
			}			
			else if (Bars.FirstBarOfSession)
			{
				if (FirstTickOfBar)
				{
					sessionCount = sessionCount +1;
					numberOfSessions = Math.Min(3, Math.Max(sessionCount, numberOfSessions));
				}
				if (rthOHL && firstDayOfPeriod && ((sessionCount == 1 && activeSession == anaSessionCountCW43.First) 
					|| (sessionCount == 2 && activeSession == anaSessionCountCW43.Second) 
					|| (sessionCount == 3 && activeSession == anaSessionCountCW43.Third)))
				{
					if(FirstTickOfBar)
					{
						currentOpen		= Open[0];
						upperNoiseTarget 	= currentOpen + displayedNoise;
						lowerNoiseTarget 	= currentOpen - displayedNoise;
						if(targetType == anaTargetTypeCW43.Average_Weekly_Expansion)
						{
							upperRangeTarget 	= currentOpen + displayedRange;
							lowerRangeTarget 	= currentOpen - displayedRange;
						}
						calcOpen		= true;
						initPlot 		= true;
					}
					currentHigh		= High[0];
					currentLow		= Low[0];
					highTime		= Time[0];
					lowTime			= Time[0];
				}
				else if (!rthOHL|| (rthOHL && !firstDayOfPeriod && ((sessionCount == 1 && activeSession == anaSessionCountCW43.First)
					|| (sessionCount == 2 && activeSession == anaSessionCountCW43.Second) 
					|| (sessionCount == 3 && activeSession == anaSessionCountCW43.Third))))
				{
					calcOpen		= true;
					initPlot		= true;
					if (High[0] >= currentHigh)
					{
						currentHigh = High[0];
						highTime = Time[0];
					}
					if (Low[0] <= currentLow)
					{
						currentLow = Low[0];
						lowTime = Time[0];
					}
				}
				else
					calcOpen = false;
			}				
			else if (calcOpen)
			{
				if (High[0] >= currentHigh)
				{
					currentHigh			 = High[0];
					highTime 			= Time[0];
				}
				if (Low[0] <= currentLow)
				{
					currentLow 			= Low[0];
					lowTime 			= Time[0];
				}
			}
			
			if(targetType == anaTargetTypeCW43.Average_Weekly_Range && calcOpen && weekCount > rangePeriod && !targetHit)
			{
				if(displayedRange > currentHigh - currentLow)
				{
					lowerRangeTarget = currentHigh - displayedRange;
					upperRangeTarget = currentLow + displayedRange;
				}
				else if(High[0] >= currentHigh && Low[0] <= currentLow)
				{
					lowerRangeTarget = 0.5*(High[0]+ Low[0] - displayedRange);
					upperRangeTarget = 0.5*(High[0]+ Low[0] + displayedRange);
					targetHit = true;
				}
				else if (High[0] >= currentHigh)
				{
					lowerRangeTarget = currentLow;
					upperRangeTarget = currentLow + displayedRange;
					targetHit = true;
				}
				else if (Low[0] <= currentLow)
				{
					upperRangeTarget = currentHigh;
					lowerRangeTarget = currentHigh - displayedRange;
					targetHit = true;
				}
			}

			if (plotOHL && initPlot && !(rthOHL && activeSession == anaSessionCountCW43.Third && numberOfSessions == 2)) 
			{
				CurrentHigh.Set(currentHigh);
				CurrentLow.Set(currentLow);
				CurrentOpen.Set(currentOpen);
				CurrentMidline.Set(Math.Round((currentLow + 0.5*(currentHigh-currentLow))/displaySize)* displaySize);
				if (lowTime >= highTime)				
				{	
					lowIsLast.Set(true);
					Fib786.Set(Math.Round((currentLow + 0.786*(currentHigh-currentLow))/displaySize)*displaySize); 
					Fib618.Set(Math.Round((currentLow + 0.618*(currentHigh-currentLow))/displaySize)*displaySize);
					Fib382.Set(Math.Round((currentLow + 0.382*(currentHigh-currentLow))/displaySize)*displaySize);
					Fib236.Set(Math.Round((currentLow + 0.236*(currentHigh-currentLow))/displaySize)*displaySize);
				}
				else 
				{
					lowIsLast.Set(false);
					Fib786.Set(Math.Round((currentHigh - 0.236*(currentHigh-currentLow))/displaySize)*displaySize);
					Fib618.Set(Math.Round((currentHigh - 0.382*(currentHigh-currentLow))/displaySize)*displaySize);
					Fib382.Set(Math.Round((currentHigh - 0.618*(currentHigh-currentLow))/displaySize)*displaySize);
					Fib236.Set(Math.Round((currentHigh - 0.786*(currentHigh-currentLow))/displaySize)*displaySize); 
				}
				if (weekCount > rangePeriod)
				{
					if(FirstTickOfBar)
					{
						RemoveDrawObject("errortag3");
						UWNL.Set (upperNoiseTarget);
						LWNL.Set (lowerNoiseTarget);
						UWET.Set (upperRangeTarget);
						LWET.Set (lowerRangeTarget);
						AWN1.Set (averageNoise1*displayFactor);
						AWN2.Set (averageNoise2*displayFactor);
						AWR1.Set (averageRange1*displayFactor);
						AWR2.Set (averageRange2*displayFactor);
						displayedWidth.Set(bandWidth);
					}
					AWN0.Set(Math.Round((Math.Min(currentHigh - currentOpen,currentOpen - currentLow))/displaySize)* displaySize * displayFactor);
					if(targetType == anaTargetTypeCW43.Average_Weekly_Range)
						AWR0.Set(Math.Round((currentHigh - currentLow)/displaySize)* displaySize * displayFactor);
					else
						AWR0.Set(Math.Round((Math.Max(currentHigh - currentOpen,currentOpen - currentLow))/displaySize)* displaySize * displayFactor);
				}
				else if (FirstTickOfBar)
				{
					UWNL.Reset();
					LWNL.Reset();
					UWET.Reset();
					LWET.Reset();
					AWN0.Reset();
					AWN1.Reset();
					AWN2.Reset();
					AWR0.Reset();
					AWR1.Reset();
					AWR2.Reset();
					displayedWidth.Reset();
				}
			}
			else if (FirstTickOfBar)
			{
				CurrentHigh.Reset();
   			  	CurrentLow.Reset();
				CurrentMidline.Reset();
				CurrentOpen.Reset();
				Fib786.Reset(); 
				Fib618.Reset(); 
				Fib382.Reset(); 
				Fib236.Reset();
				UWNL.Reset();
				LWNL.Reset();
				UWET.Reset();
				LWET.Reset();
				AWN0.Reset();
				AWR0.Reset();
				if (weekCount > rangePeriod && !(rthOHL && activeSession == anaSessionCountCW43.Third && numberOfSessions == 2))
				{
					AWN1.Set (averageNoise1 * displayFactor);
					AWN2.Set (averageNoise2 * displayFactor);
					AWR1.Set (averageRange1 * displayFactor);
					AWR2.Set (averageRange2 * displayFactor);
				}
				else
				{
					if (weekCount > 1 && plotOHL && rthOHL && activeSession == anaSessionCountCW43.Third && numberOfSessions == 2)
						DrawTextFixed("errortag2", errorData2, TextPosition.Center, errorBrush.Color, errorFont, Color.Transparent,Color.Transparent,0);
					AWN1.Reset();
					AWN2.Reset();
					AWR1.Reset();
					AWR2.Reset();
				}
				displayedWidth.Reset();
			}
		}


		#region Properties
		/// <summary>
		/// </summary>
		[Description("Session - ETH or RTH - used for calculating OHL")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Calculate from session")]
		public anaSessionTypeCW43 CurrentSession
		{
			get { return currentSession; }
			set { currentSession = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Session # used for calculating OHL, noise and target bands")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Select RTH session")]
		public anaSessionCountCW43 SelectedSession
		{
			get { return selectedSession; }
			set { selectedSession = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Formula used for calculating expansion targets")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Target formula")]
		public anaTargetTypeCW43 TargetType
		{
			get { return targetType; }
			set { targetType = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Option to show current open")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Show current open")]
		public bool ShowCurrentOpen 
		{
			get { return showCurrentOpen; }
			set { showCurrentOpen = value; }
		}
		
		[Description("Option to show dynamic Fibonacci Lines")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Show dynamic fibs")]
		public bool ShowFibonacci 
		{
			get { return showFibonacci; }
			set { showFibonacci = value; }
		}
		
		[Description("Option to show current weeks's high and low")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Show high/low")]
		public bool ShowHiLo 
		{
			get { return showHiLo; }
			set { showHiLo = value; }
		}
		
		[Description("Option to show current week's midline")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Show midline")]
		public bool ShowMidline 
		{
			get { return showMidline; }
			set { showMidline = value; }
		}
		
		[Description("Option to show noise levels")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Show noise levels")]
		public bool ShowNoiseLevels 
		{
			get { return showNoiseLevels; }
			set { showNoiseLevels = value; }
		}
		
		[Description("Option to show target levels based on average weekly range")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Show target levels")]
		public bool ShowTargetLevels 
		{
			get { return showTargetLevels; }
			set { showTargetLevels = value; }
		}
		
		[Description("Option to show volatility bands")]
		[Category("Display Options")]
		[Gui.Design.DisplayNameAttribute("Show noise bands")]
		public bool ShowNoiseBands
		{
			get { return showNoiseBands; }
			set { showNoiseBands = value; }
		}
		
		
		/// <summary>
		/// </summary>
		[Description("# of days used to calculate average weekly noise, expansion and range.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Lookback 1 in weeks")]
		public int RangePeriod1
		{
			get { return rangePeriod1; }
			set { rangePeriod1 = Math.Min(100,Math.Max(1, value)); }
		}

		/// <summary>
		/// </summary>
		[Description("# of days used to calculate average weekly noise, expansion and range.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Lookback2 in weeks")]
		public int RangePeriod2
		{
			get { return rangePeriod2; }
			set { rangePeriod2 = Math.Min(100,Math.Max(1, value)); }
		}

		/// <summary>
		/// </summary>
		[Description("Percent of average weekly noise used to calculate the bands")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Noise range in % of AWN")]
		public int FactorNoiseBands
		{
			get { return factorNoiseBands; }
			set { factorNoiseBands = Math.Min(400, Math.Max(10, value)); }
		}

		/// <summary>
		/// </summary>
		[Description("Percent of average weekly expansion used to calculate the bands")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Target range in % of AWE/AWR")]
		public int FactorWeeklyRange
		{
			get { return factorWeeklyRange; }
			set { factorWeeklyRange = Math.Min(400, Math.Max(10, value)); }
		}

		[Description("Option to show target bands based on average weekly range")]
		[Category("Display Options")]
		[Gui.Design.DisplayNameAttribute("Show target bands")]
		public bool ShowTargetBands
		{
			get { return showTargetBands; }
			set { showTargetBands = value; }
		}
		
		[Description("Option to show plots for prior days")]
		[Category("Display Options")]
		[Gui.Design.DisplayNameAttribute("Show prior days")]
		public bool ShowPriorPeriods 
		{
			get { return showPriorPeriods; }
			set { showPriorPeriods = value; }
		}
		
		[Description("Option to show current day's range and average ranges")]
		[Category("Display Options")]
		[Gui.Design.DisplayNameAttribute("Show range data")]
		public anaDataLocationCW43 ShowRangeData 
		{
			get { return showRangeData; }
			set { showRangeData = value; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CurrentOpen
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CurrentHigh
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CurrentLow
		{
			get { return Values[2]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CurrentMidline
		{
			get { return Values[3]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Fib786
		{
			get { return Values[4]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Fib618
		{
			get { return Values[5]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Fib382
		{
			get { return Values[6]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Fib236
		{
			get { return Values[7]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries UWNL
		{
			get { return Values[8]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries LWNL
		{
			get { return Values[9]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries UWET
		{
			get { return Values[10]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries LWET
		{
			get { return Values[11]; }
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
		[Description("Option where to plot labels")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Label position")]
		public anaPlotAlignCW43 PlotLabels
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
		[Gui.Design.DisplayNameAttribute("Labelfont size")]
		public int LabelFontSize
		{
			get { return labelFontSize; }
			set { labelFontSize = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Font size for text box.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Textfont size")]
		public int TextFontSize
		{
			get { return textFontSize; }
			set { textFontSize = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Opacity for Noise Band colors ")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Noise Band opacity")]
		public int OpacityN
		{
			get { return opacityN; }
			set { opacityN = Math.Min(10, Math.Max(0, value)); }
		}

		/// <summary>
		/// </summary>
		[Description("Opacity for Target Band colors ")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Target Band opacity")]
		public int OpacityT
		{
			get { return opacityT; }
			set { opacityT = Math.Min(10, Math.Max(0, value)); }
		}

		/// <summary>
		/// </summary>
		[Description("Opacity for data table colors ")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Data table opacity")]
		public int OpacityD
		{
			get { return opacityD; }
			set { opacityD = Math.Min(10, Math.Max(0, value)); }
		}

		[XmlIgnore]
		[Description("Select color for current open")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Color current open")]
		public Color OpenColor
		{
			get { return openColor; }
			set { openColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string OpenColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(openColor); }
			set { openColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select color for current high")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Color current high")]
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

		[XmlIgnore]
		[Description("Select color for current low")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Color current low")]
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

		[XmlIgnore]
		[Description("Select color for current midline")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Color for current midline")]
		public Color MidlineColor
		{
			get { return midlineColor; }
			set { midlineColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string MidlineColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(midlineColor); }
			set { midlineColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select color for dynamic Fibonacci lines")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Color for fib lines")]
		public Color FibColor
		{
			get { return fibColor; }
			set { fibColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string FibColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(fibColor); }
			set { fibColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select color for noise levels and noise bands")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Color for noise bands")]
		public Color NoiseBandColor
		{
			get { return noiseBandColor; }
			set { noiseBandColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string NoiseBandColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(noiseBandColor); }
			set { noiseBandColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore]
		[Description("Select color for target bands")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Color for target bands")]
		public Color TargetBandColor
		{
			get { return targetBandColor; }
			set { targetBandColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string TargetBandColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(targetBandColor); }
			set { targetBandColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore]
		[Description("Select color for text which appears in the box. ")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Color for text")]
		public Color TextColor
		{
			get { return textColor; }
			set { textColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string TextColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(textColor); }
			set { textColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select color for text box")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Color for text box")]
		public Color FillColor
		{
			get { return dataTableColor; }
			set { dataTableColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string FillColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(dataTableColor); }
			set { dataTableColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Line width for current open.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width for current open")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for current open.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style current open")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		} 
		
		/// <summary>
		/// </summary>
		[Description("Line width for high and low.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width high & low")]
		public int Plot1Width
		{
			get { return plot1Width; }
			set { plot1Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for high and low.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style high & low")]
		public DashStyle Dash1Style
		{
			get { return dash1Style; }
			set { dash1Style = value; }
		} 		

		/// <summary>
		/// </summary>
		[Description("Width for fib lines.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width fib lines")]
		public int Plot3Width
		{
			get { return plot3Width; }
			set { plot3Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for fib lines.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style fib lines")]
		public DashStyle Dash3Style
		{
			get { return dash3Style; }
			set { dash3Style = value; }
		} 
		
		/// <summary>
		/// </summary>
		[Description("Line width for noise bands.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width noise labels")]
		public int Plot8Width
		{
			get { return plot8Width; }
			set { plot8Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for noise bands.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style noise labels")]
		public DashStyle Dash8Style
		{
			get { return dash8Style; }
			set { dash8Style = value; }
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
				if (tmpWeeklyEndDate != cacheWeeklyEndDate || (rthOHL && firstDayOfPeriod && sessionCount == 1 && activeSession == anaSessionCountCW43.Second)
					|| (rthOHL && firstDayOfPeriod && sessionCount ==2 && activeSession == anaSessionCountCW43.Third)) 
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
			noiseBrush.Dispose();
			targetBrush.Dispose();
			dataBrush.Dispose();
			textBrush.Dispose();
			errorBrush.Dispose();
			foreach (SolidBrush solidBrush in brushes)
				solidBrush.Dispose();
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
			SizeF errorSize	= graphics.MeasureString(errorData2, errorFont);
			errorTextHeight	= errorSize.Height + 5;
			if (plotStart1 >= lastBarSessionEnd)
				graphics.DrawString(errorData4, errorFont, errorBrush, bounds.X + bounds.Width, bounds.Y + bounds.Height - errorTextHeight, stringFormatFar);
			else if (plotStart2 >= lastBarSessionEnd && (showNoiseLevels || showTargetLevels || showNoiseBands || showTargetBands || showRangeData != anaDataLocationCW43.DoNotPlot))
				graphics.DrawString(errorData5, errorFont, errorBrush, bounds.X + bounds.Width, bounds.Y + bounds.Height - errorTextHeight, stringFormatFar);
			
			int lastBarIndex		= this.LastBarIndexPainted;
			int firstBarIndex		= Math.Max(BarsRequired, this.FirstBarIndexPainted - 1);
			
			bool firstLoop = true;
			do
			{	
				while (lastBarIndex > firstBarIndex && !Values[0].IsValidPlot(lastBarIndex))
				{
					lastBarIndex = lastBarIndex - 1;
					firstLoop = false;
				}
				if (!Values[0].IsValidPlot(lastBarIndex))
					break;
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
				int lastBarPlotIndex 	= (CalculateOnBarClose && firstLoop && CurrentBar < ChartControl.LastBarPainted) ? lastBarIndex + 1 : lastBarIndex; 
				int firstBarPlotIndex 	= Math.Max(firstBarIndex, firstBarIdxToPaint); 
				int sessionGap			= Math.Min(10, Math.Max(3, ChartControl.BarWidth));
				int firstXtoFill		= ChartControl.GetXByBarIdx(BarsArray[0], lastBarPlotIndex);
				int lastXtoFill 		= ChartControl.GetXByBarIdx(BarsArray[0], firstBarPlotIndex) + sessionGap;
				int[] yArr 				= new int[Values.Length];
				double bandWidth		= 1.5*displayedWidth.Get(lastBarIndex);
				string[] labels			= new String [Values.Length];
				Color[]	plotColors		= new Color [Values.Length];
				
				if(lowIsLast.IsValidPlot(lastBarIndex))
				{
					bool retraceFromLow = lowIsLast.Get(lastBarIndex);
					if (retraceFromLow)
					{
						Plots[4].Name = "W-78.6";
						Plots[5].Name = "W-61.8";
						Plots[6].Name = "W-38.2";
						Plots[7].Name = "W-23.6";
					}
					else
					{
						Plots[4].Name = "W-23.6";
						Plots[5].Name = "W-38.2";
						Plots[6].Name = "W-61.8";
						Plots[7].Name = "W-78.6";
					}
				}
				
				for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
				{
					yArr[seriesCount] 	= ChartControl.GetYByValue(this, Values[seriesCount].Get(lastBarIndex));
					labels[seriesCount] = Plots[seriesCount].Name;
					plotColors[seriesCount] = Plots[seriesCount].Pen.Color;
				}
				if(plotHiLo && plotCurrentOpen)
				{
					for (int i = 1; i < 3; i++)
						if (yArr[i] == yArr[0])
						{
							labels[i] = Plots[0].Name + "/" + Plots[i].Name;
							plotColors[0] = Color.Transparent;
						}
				}
				if(plotNoiseLevels && plotMidline)
				{
					for (int i = 8; i < 10; i++)
						if (yArr[i] == yArr[3])
						{
							labels[i] = Plots[3].Name + "/" + Plots[i].Name;
							plotColors[3] = Color.Transparent;
						}
				}				
				if(plotNoiseLevels && plotFibonacci)
				{
					for (int i = 8; i < 10; i++)
						for( int j = 4; j < 8; j++)
							if (yArr[i] == yArr[j])
							{
								labels[i] = Plots[j].Name + "/" + Plots[i].Name;
								plotColors[j] = Color.Transparent;
							}
				}				
				if(plotNoiseLevels && plotCurrentOpen)
				{
					for (int i = 8; i < 10; i++)
						if (yArr[i] == yArr[0])
						{
							labels[i] = Plots[0].Name + "/" + Plots[i].Name;
							plotColors[0] = Color.Transparent;
						}
				}
				if(plotNoiseLevels && plotHiLo)
				{
					for (int i = 8; i < 10; i++)
						for( int j = 1; j < 3; j++)
							if (yArr[i] == yArr[j])
							{
								labels[i] = Plots[j].Name + "/" + Plots[i].Name;
								plotColors[j] = Color.Transparent;
							}
				}				
				if(plotTargetLevels && plotMidline)
				{
					for (int i = 10; i < 12; i++)
						if (yArr[i] == yArr[3])
						{
							labels[i] = Plots[3].Name + "/" + Plots[i].Name;
							plotColors[3] = Color.Transparent;
						}
				}				
				if(plotTargetLevels && plotFibonacci)
				{
					for (int i = 10; i < 12; i++)
						for( int j = 4; j < 8; j++)
							if (yArr[i] == yArr[j])
							{
								labels[i] = Plots[j].Name + "/" + Plots[i].Name;
								plotColors[j] = Color.Transparent;
							}
				}				
				if(plotTargetLevels && plotCurrentOpen)
				{
					for (int i = 10; i < 12; i++)
						if (yArr[i] == yArr[0])
						{
							labels[i] = Plots[0].Name + "/" + Plots[i].Name;
							plotColors[0] = Color.Transparent;
						}
				}
				if(plotTargetLevels && plotHiLo)
				{
					for (int i = 10; i < 12; i++)
						for( int j = 1; j < 3; j++)
							if (yArr[i] == yArr[j])
							{
								labels[i] = Plots[j].Name + "/" + Plots[i].Name;
								plotColors[j] = Color.Transparent;
							}
				}	
				if(plotTargetLevels && plotNoiseLevels)
				{
					for (int i = 10; i < 12; i++)
						for( int j = 8; j < 10; j++)
							if (yArr[i] == yArr[j])
							{
								labels[i] = Plots[j].Name + "/" + Plots[i].Name;
								plotColors[j] = Color.Transparent;
							}
				}	
				
				if(showNoiseBands && opacityN > 0 && Values[8].IsValidPlot(lastBarIndex))
				{
					double upperNoiseBand = Values[8].Get(lastBarIndex);
					double lowerNoiseBand = Values[9].Get(lastBarIndex);
					
					int upperNoiseBandUpper = ChartControl.GetYByValue(this, upperNoiseBand + bandWidth);
					int upperNoiseBandLower = ChartControl.GetYByValue(this, upperNoiseBand - bandWidth);
					int lowerNoiseBandUpper = ChartControl.GetYByValue(this, lowerNoiseBand + bandWidth);
					int lowerNoiseBandLower = ChartControl.GetYByValue(this, lowerNoiseBand - bandWidth);
					graphics.FillRectangle(noiseBrush, lastXtoFill, upperNoiseBandUpper, firstXtoFill - lastXtoFill, upperNoiseBandLower - upperNoiseBandUpper);
					graphics.FillRectangle(noiseBrush, lastXtoFill, lowerNoiseBandUpper, firstXtoFill - lastXtoFill, lowerNoiseBandLower - lowerNoiseBandUpper);
				}
				if(showTargetBands && opacityT > 0 && targetType == anaTargetTypeCW43.Average_Weekly_Expansion && Values[10].IsValidPlot(lastBarIndex))
				{
					double upperTargetBand = Values[10].Get(lastBarIndex);
					double lowerTargetBand = Values[11].Get(lastBarIndex);
					double targetBandWidth = 1.5*displayedWidth.Get(lastBarIndex);
					
					int upperTargetBandUpper = ChartControl.GetYByValue(this, upperTargetBand + targetBandWidth);
					int upperTargetBandLower = ChartControl.GetYByValue(this, upperTargetBand - targetBandWidth);
					int lowerTargetBandUpper = ChartControl.GetYByValue(this, lowerTargetBand + targetBandWidth);
					int lowerTargetBandLower = ChartControl.GetYByValue(this, lowerTargetBand - targetBandWidth);
					graphics.FillRectangle(targetBrush, lastXtoFill, upperTargetBandUpper, firstXtoFill - lastXtoFill, upperTargetBandLower - upperTargetBandUpper);
					graphics.FillRectangle(targetBrush, lastXtoFill, lowerTargetBandUpper, firstXtoFill - lastXtoFill, lowerTargetBandLower - lowerTargetBandUpper);
				}
					
				int maxCount = Values.Length;
				if(targetType == anaTargetTypeCW43.Average_Weekly_Range)
					maxCount = maxCount - 2;
				
				for (int seriesCount = maxCount; seriesCount < Values.Length; seriesCount++)
				{
					DataSeries		series			= (DataSeries) Values[seriesCount];
					if (!series.IsValidPlot(lastBarIndex))
						continue;
					SmoothingMode oldSmoothingMode 	= graphics.SmoothingMode;
					Gui.Chart.Plot	plot			= Plots[seriesCount];
					SolidBrush brush				= brushes[seriesCount];
					int x 							= 0;
					int firstX						= -1;
					int y 							= 0;
					int lastX						= -1;
					int lastY						= -1;
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
							if(idx > firstBarIdxToPaint)
								y 	= ChartControl.GetYByValue(this, series.Get(idx));
							else
								y = lastY;
							if(idx < lastBarIndex)								
							{
								path.AddLine(lastX - plot.Pen.Width / 2, lastY, lastX - plot.Pen.Width / 2, y);
								path.AddLine(lastX - plot.Pen.Width / 2, y, x - plot.Pen.Width / 2, y);
							}
							else if(lastX >=0)
								path.AddLine(lastX - plot.Pen.Width / 2, y, x - plot.Pen.Width / 2, y);
							lastX	= x;
							lastY	= y;
							if (idx == lastBarPlotIndex)
									firstX = x;
						}
						graphics.SmoothingMode = SmoothingMode.AntiAlias;
						Color cacheColor = plot.Pen.Color;
						plot.Pen.Color = plotColors[seriesCount]; 							
						graphics.DrawPath(plot.Pen, path);
						plot.Pen.Color = cacheColor;
						graphics.SmoothingMode = oldSmoothingMode;
						if (PlotLabels == anaPlotAlignCW43.Right)
						graphics.DrawString(labels[seriesCount], labelFont, brush, firstX + labelOffset, yArr[seriesCount] - labelFont.GetHeight() / 2, stringFormatNear);
						if (PlotLabels == anaPlotAlignCW43.Left)
						graphics.DrawString(labels[seriesCount], labelFont, brush, lastX - labelOffset, yArr[seriesCount] - labelFont.GetHeight() / 2, stringFormatFar);
					}
					
					if(showTargetBands && opacityT > 0 && targetType == anaTargetTypeCW43.Average_Weekly_Range)
					{
						using (GraphicsPath	path = new GraphicsPath())
						{
							double targetBandWidth = 0;
							if(seriesCount == maxCount)
								targetBandWidth = bandWidth;
							else if(seriesCount == maxCount + 1)
								targetBandWidth = -bandWidth;
							lastX				= -1;
							lastY				= -1;
							int last2Y			= -1;
							int lastIdx			= 0;
							for (int idx = lastBarPlotIndex; idx >= firstBarPlotIndex; idx--)    
							{
								if (idx - Displacement >= Bars.Count) 
									continue;
								else if (idx - Displacement < 0)
									break;
								x	= Math.Max(ChartControl.GetXByBarIdx(BarsArray[0], idx), lastXtoFill);
								if(idx > firstBarIdxToPaint)
									y 	= ChartControl.GetYByValue(this, series.Get(idx) - targetBandWidth);
								else
									y = lastY;
								if(idx < lastBarIndex)
								{
									if(last2Y >=0)
									{	
										path.AddLine(lastX - plot.Pen.Width / 2, last2Y, lastX - plot.Pen.Width / 2, lastY);
										path.AddLine(lastX - plot.Pen.Width / 2, lastY, x - plot.Pen.Width / 2, lastY);
									}
									else
										path.AddLine(lastX - plot.Pen.Width / 2, lastY, x - plot.Pen.Width / 2, lastY);
									last2Y = lastY;
								}
								else if(lastX >=0)
									path.AddLine(lastX - plot.Pen.Width / 2, y, x - plot.Pen.Width / 2, y);
								lastX	= x;
								lastY	= y;
								lastIdx = idx;
							}
							last2Y	= -1;
							for (int idx = lastIdx; idx <= lastBarPlotIndex; idx++)
							{
								x	= Math.Max(ChartControl.GetXByBarIdx(BarsArray[0], idx), lastXtoFill);
								if(idx == lastIdx)
								{
									if(idx > firstBarIdxToPaint)
										y 	= ChartControl.GetYByValue(this, series.Get(idx) + targetBandWidth);
									else
										y 	= ChartControl.GetYByValue(this, series.Get(idx + 1) + targetBandWidth);
									path.AddLine(lastX - plot.Pen.Width / 2, lastY, lastX - plot.Pen.Width / 2, y);
								}
								else if (idx == lastIdx + 1)
								{
									y 	= ChartControl.GetYByValue(this, series.Get(idx) + targetBandWidth);
									path.AddLine(lastX - plot.Pen.Width / 2, lastY, x - plot.Pen.Width / 2, lastY);
									path.AddLine(x - plot.Pen.Width / 2, lastY, x - plot.Pen.Width / 2, y);
									last2Y = lastY;
								}	
								else	
								{	
									y 	= ChartControl.GetYByValue(this, series.Get(idx) + targetBandWidth);
									path.AddLine(lastX - plot.Pen.Width / 2, last2Y, x - plot.Pen.Width / 2, last2Y);
									path.AddLine(x - plot.Pen.Width / 2, last2Y, x - plot.Pen.Width / 2, lastY);
									last2Y = lastY;
								}
								lastX	= x;
								lastY	= y;
							}							
							graphics.SmoothingMode = SmoothingMode.AntiAlias;
							graphics.FillPath(targetBrush, path);
							graphics.SmoothingMode = oldSmoothingMode;
						}
					}				
				}
				
				for (int seriesCount = 0; seriesCount < maxCount ; seriesCount++)
				{
					DataSeries		series			= (DataSeries) Values[seriesCount];
					if (!series.IsValidPlot(lastBarIndex))
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
						if (PlotLabels == anaPlotAlignCW43.Right)
						graphics.DrawString(labels[seriesCount], labelFont, brush, firstX + labelOffset, yArr[seriesCount] - labelFont.GetHeight() / 2, stringFormatNear);
						if (PlotLabels == anaPlotAlignCW43.Left)
						graphics.DrawString(labels[seriesCount], labelFont, brush, lastX - labelOffset, yArr[seriesCount] - labelFont.GetHeight() / 2, stringFormatFar);
					}
				}
				lastBarIndex = firstBarIdxToPaint;
				firstLoop = false;
			}
			while (showPriorPeriods && lastBarIndex > firstBarIndex); 
			
			lastBarIndex = this.LastBarIndexPainted;
			if (AWR1.IsValidPlot(lastBarIndex) && AWR2.IsValidPlot(lastBarIndex) && AWN1.IsValidPlot(lastBarIndex) && AWN2.IsValidPlot(lastBarIndex) )
			{
				double averageWN0 = 0.0;
				if(AWN0.IsValidPlot(lastBarIndex))
					averageWN0 = AWN0.Get(lastBarIndex);
				double averageWN1 = AWN1.Get(lastBarIndex);
				double averageWN2 = AWN2.Get(lastBarIndex);
				double averageWR0 = 0.0;
				if(AWR0.IsValidPlot(lastBarIndex))
					averageWR0 = AWR0.Get(lastBarIndex);
				double averageWR1 = AWR1.Get(lastBarIndex);
				double averageWR2 = AWR2.Get(lastBarIndex);
				if (TickSize == 0.03125) 
				{
					double truncN0 = Math.Truncate(averageWN0); 
					double truncN1 = Math.Truncate(averageWN1); 
					double truncN2 = Math.Truncate(averageWN2);
					double truncR0 = Math.Truncate(averageWR0); 
					double truncR1 = Math.Truncate(averageWR1); 
					double truncR2 = Math.Truncate(averageWR2);
					int fractionN0 = Convert.ToInt32(32 * Math.Abs(averageWN0 - truncN0) - 0.0001);
					int fractionN1 = Convert.ToInt32(32 * Math.Abs(averageWN1 - truncN1) - 0.0001);
					int fractionN2 = Convert.ToInt32(32 * Math.Abs(averageWN2 - truncN2) - 0.0001);
					int fractionR0 = Convert.ToInt32(32 * Math.Abs(averageWR0 - truncR0) - 0.0001);
					int fractionR1 = Convert.ToInt32(32 * Math.Abs(averageWR1 - truncR1) - 0.0001);
					int fractionR2 = Convert.ToInt32(32 * Math.Abs(averageWR2 - truncR2) - 0.0001);
					if (fractionN0 < 10)
						noiseData0 = noiseText0 + truncN0.ToString() + "'0" + fractionN0.ToString();
					else 
						noiseData0 = noiseText0 + truncN0.ToString() + "'" + fractionN0.ToString();
					if (fractionN1 < 10)
						noiseData1 = noiseText1 + truncN1.ToString() + "'0" + fractionN1.ToString();
					else 
						noiseData1 = noiseText1 + truncN1.ToString() + "'" + fractionN1.ToString();
					if (fractionN2 < 10)
						noiseData2 = noiseText2 + truncN2.ToString() + "'0" + fractionN2.ToString();
					else 
						noiseData2 = noiseText2 + truncN2.ToString() + "'" + fractionN2.ToString();
					if (fractionR0 < 10)
						rangeData0 = rangeText0 + truncR0.ToString() + "'0" + fractionR0.ToString();
					else 
						rangeData0 = rangeText0 + truncR0.ToString() + "'" + fractionR0.ToString();
					if (fractionR1 < 10)
						rangeData1 = rangeText1 + truncR1.ToString() + "'0" + fractionR1.ToString();
					else 
						rangeData1 = rangeText1 + truncR1.ToString() + "'" + fractionR1.ToString();
					if (fractionR2 < 10)
						rangeData2 = rangeText2 + truncR2.ToString() + "'0" + fractionR2.ToString();
					else 
						rangeData2 = rangeText2 + truncR2.ToString() + "'" + fractionR2.ToString();
				}
				else if (TickSize == 0.015625 || TickSize == 0.0078125)
				{
					double truncN0 = Math.Truncate(averageWN0); 
					double truncN1 = Math.Truncate(averageWN1); 
					double truncN2 = Math.Truncate(averageWN2);
					double truncR0 = Math.Truncate(averageWR0); 
					double truncR1 = Math.Truncate(averageWR1); 
					double truncR2 = Math.Truncate(averageWR2);
					int fractionN0 = Convert.ToInt32(320 * Math.Abs(averageWN0 - truncN0) - 0.0001);
					int fractionN1 = Convert.ToInt32(320 * Math.Abs(averageWN1 - truncN1) - 0.0001);
					int fractionN2 = Convert.ToInt32(320 * Math.Abs(averageWN2 - truncN2) - 0.0001);
					int fractionR0 = Convert.ToInt32(320 * Math.Abs(averageWR0 - truncR0) - 0.0001);
					int fractionR1 = Convert.ToInt32(320 * Math.Abs(averageWR1 - truncR1) - 0.0001);
					int fractionR2 = Convert.ToInt32(320 * Math.Abs(averageWR2 - truncR2) - 0.0001);
					if (fractionN0 < 10)
						noiseData0 = noiseText0 + truncN0.ToString() + "'00" + fractionN0.ToString();
					else if (fractionN0 < 100)
						noiseData0 = noiseText0 + truncN0.ToString() + "'0" + fractionN0.ToString();
					else 
						noiseData0 = noiseText0 + truncN0.ToString() + "'" + fractionN0.ToString();
					if (fractionN1 < 10)
						noiseData1 = noiseText1 + truncN1.ToString() + "'00" + fractionN1.ToString();
					else if (fractionN1 < 100)
						noiseData1 = noiseText1 + truncN1.ToString() + "'0" + fractionN1.ToString();
					else 
						noiseData1 = noiseText1 + truncN1.ToString() + "'" + fractionN1.ToString();
					if (fractionN2 < 10)
						noiseData2 = noiseText2 + truncN2.ToString() + "'00" + fractionN2.ToString();
					else if (fractionN2 < 100)
						noiseData2 = noiseText2 + truncN2.ToString() + "'0" + fractionN2.ToString();
					else 
						noiseData2 = noiseText2 + truncN2.ToString() + "'" + fractionN2.ToString();
					if (fractionR0 < 10)
						rangeData0 = rangeText0 + truncR0.ToString() + "'00" + fractionR0.ToString();
					else if (fractionR0 < 100)
						rangeData0 = rangeText0 + truncR0.ToString() + "'0" + fractionR0.ToString();
					else 
						rangeData0 = rangeText0 + truncR0.ToString() + "'" + fractionR0.ToString();
					if (fractionR1 < 10)
						rangeData1 = rangeText1 + truncR1.ToString() + "'00" + fractionR1.ToString();
					else if (fractionR1 < 100)
						rangeData1 = rangeText1 + truncR1.ToString() + "'0" + fractionR1.ToString();
					else 
						rangeData1 = rangeText1 + truncR1.ToString() + "'" + fractionR1.ToString();
					if (fractionR2 < 10)
						rangeData2 = rangeText2 + truncR2.ToString() + "'00" + fractionR2.ToString();
					else if (fractionR2 < 100)
						rangeData2 = rangeText2 + truncR2.ToString() + "'0" + fractionR2.ToString();
					else 
						rangeData2 = rangeText2 + truncR2.ToString() + "'" + fractionR2.ToString();				}
				else if (isCurrency)
				{
					noiseData0 = noiseText0 + averageWN0.ToString();	
					noiseData1 = noiseText1 + averageWN1.ToString();	
					noiseData2 = noiseText2 + averageWN2.ToString();	
					rangeData0 = rangeText0 + averageWR0.ToString();	
					rangeData1 = rangeText1 + averageWR1.ToString();	
					rangeData2 = rangeText2 + averageWR2.ToString();	
				}	
				else	
				{
					noiseData0 = noiseText0 + averageWN0.ToString(Gui.Globals.GetTickFormatString(TickSize));	
					noiseData1 = noiseText1 + averageWN1.ToString(Gui.Globals.GetTickFormatString(TickSize));	
					noiseData2 = noiseText2 + averageWN2.ToString(Gui.Globals.GetTickFormatString(TickSize));	
					rangeData0 = rangeText0 + averageWR0.ToString(Gui.Globals.GetTickFormatString(TickSize));	
					rangeData1 = rangeText1 + averageWR1.ToString(Gui.Globals.GetTickFormatString(TickSize));	
					rangeData2 = rangeText2 + averageWR2.ToString(Gui.Globals.GetTickFormatString(TickSize));	
				}
				SizeF noiseSize	= graphics.MeasureString(noiseData0, textFont);
				SizeF rangeSize	= graphics.MeasureString(rangeData0, textFont);
				if (showRangeData == anaDataLocationCW43.Left_Format_Long || showRangeData == anaDataLocationCW43.Left_Format_Short)
				{
					graphics.FillRectangle(dataBrush, bounds.X + 10, bounds.Y + 20, (rangeSize+noiseSize).Width + 55, 4.0f*textFont.Height + 20);
					graphics.DrawString(noiseData0, textFont, textBrush, bounds.X + 30, bounds.Y + 0.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(noiseData1, textFont, textBrush, bounds.X + 30, bounds.Y + 2.0f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(noiseData2, textFont, textBrush, bounds.X + 30, bounds.Y + 3.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData0, textFont, textBrush, bounds.X + noiseSize.Width + 50, bounds.Y + 0.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData1, textFont, textBrush, bounds.X + noiseSize.Width + 50, bounds.Y + 2.0f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData2, textFont, textBrush, bounds.X + noiseSize.Width + 50, bounds.Y + 3.5f*textFont.Height + 20, stringFormatNear);
				}
				else if (showRangeData == anaDataLocationCW43.Right_Format_Long || showRangeData == anaDataLocationCW43.Right_Format_Short)
				{
					graphics.FillRectangle(dataBrush, bounds.X + bounds.Width - (rangeSize + noiseSize).Width - 70, bounds.Y + 20, (rangeSize+noiseSize).Width + 55, 4.0f*textFont.Height + 20);
					graphics.DrawString(noiseData0, textFont, textBrush, bounds.X + bounds.Width - (rangeSize+noiseSize).Width - 50, bounds.Y + 0.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(noiseData1, textFont, textBrush, bounds.X + bounds.Width - (rangeSize+noiseSize).Width - 50, bounds.Y + 2.0f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(noiseData2, textFont, textBrush, bounds.X + bounds.Width - (rangeSize+noiseSize).Width - 50, bounds.Y + 3.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData0, textFont, textBrush, bounds.X + bounds.Width - rangeSize.Width - 30, bounds.Y + 0.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData1, textFont, textBrush, bounds.X + bounds.Width - rangeSize.Width - 30, bounds.Y + 2.0f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData2, textFont, textBrush, bounds.X + bounds.Width - rangeSize.Width - 30, bounds.Y + 3.5f*textFont.Height + 20, stringFormatNear);
				}
				else if (showRangeData == anaDataLocationCW43.Center_Format_Long || showRangeData == anaDataLocationCW43.Center_Format_Short)
				{
					graphics.FillRectangle(dataBrush, bounds.X + bounds.Width/2.0f - 0.5f*(rangeSize + noiseSize).Width - 30, bounds.Y + 20, (rangeSize+noiseSize).Width + 55, 4.0f*textFont.Height + 20);
					graphics.DrawString(noiseData0, textFont, textBrush, bounds.X + bounds.Width/2 - 0.5f*(rangeSize+noiseSize).Width - 10, bounds.Y + 0.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(noiseData1, textFont, textBrush, bounds.X + bounds.Width/2 - 0.5f*(rangeSize+noiseSize).Width - 10, bounds.Y + 2.0f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(noiseData2, textFont, textBrush, bounds.X + bounds.Width/2 - 0.5f*(rangeSize+noiseSize).Width - 10, bounds.Y + 3.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData0, textFont, textBrush, bounds.X + bounds.Width/2 + 0.5f*(noiseSize-rangeSize).Width + 10, bounds.Y + 0.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData1, textFont, textBrush, bounds.X + bounds.Width/2 + 0.5f*(noiseSize-rangeSize).Width + 10, bounds.Y + 2.0f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData2, textFont, textBrush, bounds.X + bounds.Width/2 + 0.5f*(noiseSize-rangeSize).Width + 10, bounds.Y + 3.5f*textFont.Height + 20, stringFormatNear);
				}
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
        private anaCurrentWeekOHLV43[] cacheanaCurrentWeekOHLV43 = null;

        private static anaCurrentWeekOHLV43 checkanaCurrentWeekOHLV43 = new anaCurrentWeekOHLV43();

        /// <summary>
        /// anaCurrentWeekOHLV43.
        /// </summary>
        /// <returns></returns>
        public anaCurrentWeekOHLV43 anaCurrentWeekOHLV43(anaSessionTypeCW43 currentSession, int factorNoiseBands, int factorWeeklyRange, int rangePeriod1, int rangePeriod2, anaSessionCountCW43 selectedSession, bool showCurrentOpen, bool showFibonacci, bool showHiLo, bool showMidline, bool showNoiseLevels, bool showTargetLevels, anaTargetTypeCW43 targetType)
        {
            return anaCurrentWeekOHLV43(Input, currentSession, factorNoiseBands, factorWeeklyRange, rangePeriod1, rangePeriod2, selectedSession, showCurrentOpen, showFibonacci, showHiLo, showMidline, showNoiseLevels, showTargetLevels, targetType);
        }

        /// <summary>
        /// anaCurrentWeekOHLV43.
        /// </summary>
        /// <returns></returns>
        public anaCurrentWeekOHLV43 anaCurrentWeekOHLV43(Data.IDataSeries input, anaSessionTypeCW43 currentSession, int factorNoiseBands, int factorWeeklyRange, int rangePeriod1, int rangePeriod2, anaSessionCountCW43 selectedSession, bool showCurrentOpen, bool showFibonacci, bool showHiLo, bool showMidline, bool showNoiseLevels, bool showTargetLevels, anaTargetTypeCW43 targetType)
        {
            if (cacheanaCurrentWeekOHLV43 != null)
                for (int idx = 0; idx < cacheanaCurrentWeekOHLV43.Length; idx++)
                    if (cacheanaCurrentWeekOHLV43[idx].CurrentSession == currentSession && cacheanaCurrentWeekOHLV43[idx].FactorNoiseBands == factorNoiseBands && cacheanaCurrentWeekOHLV43[idx].FactorWeeklyRange == factorWeeklyRange && cacheanaCurrentWeekOHLV43[idx].RangePeriod1 == rangePeriod1 && cacheanaCurrentWeekOHLV43[idx].RangePeriod2 == rangePeriod2 && cacheanaCurrentWeekOHLV43[idx].SelectedSession == selectedSession && cacheanaCurrentWeekOHLV43[idx].ShowCurrentOpen == showCurrentOpen && cacheanaCurrentWeekOHLV43[idx].ShowFibonacci == showFibonacci && cacheanaCurrentWeekOHLV43[idx].ShowHiLo == showHiLo && cacheanaCurrentWeekOHLV43[idx].ShowMidline == showMidline && cacheanaCurrentWeekOHLV43[idx].ShowNoiseLevels == showNoiseLevels && cacheanaCurrentWeekOHLV43[idx].ShowTargetLevels == showTargetLevels && cacheanaCurrentWeekOHLV43[idx].TargetType == targetType && cacheanaCurrentWeekOHLV43[idx].EqualsInput(input))
                        return cacheanaCurrentWeekOHLV43[idx];

            lock (checkanaCurrentWeekOHLV43)
            {
                checkanaCurrentWeekOHLV43.CurrentSession = currentSession;
                currentSession = checkanaCurrentWeekOHLV43.CurrentSession;
                checkanaCurrentWeekOHLV43.FactorNoiseBands = factorNoiseBands;
                factorNoiseBands = checkanaCurrentWeekOHLV43.FactorNoiseBands;
                checkanaCurrentWeekOHLV43.FactorWeeklyRange = factorWeeklyRange;
                factorWeeklyRange = checkanaCurrentWeekOHLV43.FactorWeeklyRange;
                checkanaCurrentWeekOHLV43.RangePeriod1 = rangePeriod1;
                rangePeriod1 = checkanaCurrentWeekOHLV43.RangePeriod1;
                checkanaCurrentWeekOHLV43.RangePeriod2 = rangePeriod2;
                rangePeriod2 = checkanaCurrentWeekOHLV43.RangePeriod2;
                checkanaCurrentWeekOHLV43.SelectedSession = selectedSession;
                selectedSession = checkanaCurrentWeekOHLV43.SelectedSession;
                checkanaCurrentWeekOHLV43.ShowCurrentOpen = showCurrentOpen;
                showCurrentOpen = checkanaCurrentWeekOHLV43.ShowCurrentOpen;
                checkanaCurrentWeekOHLV43.ShowFibonacci = showFibonacci;
                showFibonacci = checkanaCurrentWeekOHLV43.ShowFibonacci;
                checkanaCurrentWeekOHLV43.ShowHiLo = showHiLo;
                showHiLo = checkanaCurrentWeekOHLV43.ShowHiLo;
                checkanaCurrentWeekOHLV43.ShowMidline = showMidline;
                showMidline = checkanaCurrentWeekOHLV43.ShowMidline;
                checkanaCurrentWeekOHLV43.ShowNoiseLevels = showNoiseLevels;
                showNoiseLevels = checkanaCurrentWeekOHLV43.ShowNoiseLevels;
                checkanaCurrentWeekOHLV43.ShowTargetLevels = showTargetLevels;
                showTargetLevels = checkanaCurrentWeekOHLV43.ShowTargetLevels;
                checkanaCurrentWeekOHLV43.TargetType = targetType;
                targetType = checkanaCurrentWeekOHLV43.TargetType;

                if (cacheanaCurrentWeekOHLV43 != null)
                    for (int idx = 0; idx < cacheanaCurrentWeekOHLV43.Length; idx++)
                        if (cacheanaCurrentWeekOHLV43[idx].CurrentSession == currentSession && cacheanaCurrentWeekOHLV43[idx].FactorNoiseBands == factorNoiseBands && cacheanaCurrentWeekOHLV43[idx].FactorWeeklyRange == factorWeeklyRange && cacheanaCurrentWeekOHLV43[idx].RangePeriod1 == rangePeriod1 && cacheanaCurrentWeekOHLV43[idx].RangePeriod2 == rangePeriod2 && cacheanaCurrentWeekOHLV43[idx].SelectedSession == selectedSession && cacheanaCurrentWeekOHLV43[idx].ShowCurrentOpen == showCurrentOpen && cacheanaCurrentWeekOHLV43[idx].ShowFibonacci == showFibonacci && cacheanaCurrentWeekOHLV43[idx].ShowHiLo == showHiLo && cacheanaCurrentWeekOHLV43[idx].ShowMidline == showMidline && cacheanaCurrentWeekOHLV43[idx].ShowNoiseLevels == showNoiseLevels && cacheanaCurrentWeekOHLV43[idx].ShowTargetLevels == showTargetLevels && cacheanaCurrentWeekOHLV43[idx].TargetType == targetType && cacheanaCurrentWeekOHLV43[idx].EqualsInput(input))
                            return cacheanaCurrentWeekOHLV43[idx];

                anaCurrentWeekOHLV43 indicator = new anaCurrentWeekOHLV43();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CurrentSession = currentSession;
                indicator.FactorNoiseBands = factorNoiseBands;
                indicator.FactorWeeklyRange = factorWeeklyRange;
                indicator.RangePeriod1 = rangePeriod1;
                indicator.RangePeriod2 = rangePeriod2;
                indicator.SelectedSession = selectedSession;
                indicator.ShowCurrentOpen = showCurrentOpen;
                indicator.ShowFibonacci = showFibonacci;
                indicator.ShowHiLo = showHiLo;
                indicator.ShowMidline = showMidline;
                indicator.ShowNoiseLevels = showNoiseLevels;
                indicator.ShowTargetLevels = showTargetLevels;
                indicator.TargetType = targetType;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaCurrentWeekOHLV43[] tmp = new anaCurrentWeekOHLV43[cacheanaCurrentWeekOHLV43 == null ? 1 : cacheanaCurrentWeekOHLV43.Length + 1];
                if (cacheanaCurrentWeekOHLV43 != null)
                    cacheanaCurrentWeekOHLV43.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaCurrentWeekOHLV43 = tmp;
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
        /// anaCurrentWeekOHLV43.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaCurrentWeekOHLV43 anaCurrentWeekOHLV43(anaSessionTypeCW43 currentSession, int factorNoiseBands, int factorWeeklyRange, int rangePeriod1, int rangePeriod2, anaSessionCountCW43 selectedSession, bool showCurrentOpen, bool showFibonacci, bool showHiLo, bool showMidline, bool showNoiseLevels, bool showTargetLevels, anaTargetTypeCW43 targetType)
        {
            return _indicator.anaCurrentWeekOHLV43(Input, currentSession, factorNoiseBands, factorWeeklyRange, rangePeriod1, rangePeriod2, selectedSession, showCurrentOpen, showFibonacci, showHiLo, showMidline, showNoiseLevels, showTargetLevels, targetType);
        }

        /// <summary>
        /// anaCurrentWeekOHLV43.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaCurrentWeekOHLV43 anaCurrentWeekOHLV43(Data.IDataSeries input, anaSessionTypeCW43 currentSession, int factorNoiseBands, int factorWeeklyRange, int rangePeriod1, int rangePeriod2, anaSessionCountCW43 selectedSession, bool showCurrentOpen, bool showFibonacci, bool showHiLo, bool showMidline, bool showNoiseLevels, bool showTargetLevels, anaTargetTypeCW43 targetType)
        {
            return _indicator.anaCurrentWeekOHLV43(input, currentSession, factorNoiseBands, factorWeeklyRange, rangePeriod1, rangePeriod2, selectedSession, showCurrentOpen, showFibonacci, showHiLo, showMidline, showNoiseLevels, showTargetLevels, targetType);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// anaCurrentWeekOHLV43.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaCurrentWeekOHLV43 anaCurrentWeekOHLV43(anaSessionTypeCW43 currentSession, int factorNoiseBands, int factorWeeklyRange, int rangePeriod1, int rangePeriod2, anaSessionCountCW43 selectedSession, bool showCurrentOpen, bool showFibonacci, bool showHiLo, bool showMidline, bool showNoiseLevels, bool showTargetLevels, anaTargetTypeCW43 targetType)
        {
            return _indicator.anaCurrentWeekOHLV43(Input, currentSession, factorNoiseBands, factorWeeklyRange, rangePeriod1, rangePeriod2, selectedSession, showCurrentOpen, showFibonacci, showHiLo, showMidline, showNoiseLevels, showTargetLevels, targetType);
        }

        /// <summary>
        /// anaCurrentWeekOHLV43.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaCurrentWeekOHLV43 anaCurrentWeekOHLV43(Data.IDataSeries input, anaSessionTypeCW43 currentSession, int factorNoiseBands, int factorWeeklyRange, int rangePeriod1, int rangePeriod2, anaSessionCountCW43 selectedSession, bool showCurrentOpen, bool showFibonacci, bool showHiLo, bool showMidline, bool showNoiseLevels, bool showTargetLevels, anaTargetTypeCW43 targetType)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaCurrentWeekOHLV43(input, currentSession, factorNoiseBands, factorWeeklyRange, rangePeriod1, rangePeriod2, selectedSession, showCurrentOpen, showFibonacci, showHiLo, showMidline, showNoiseLevels, showTargetLevels, targetType);
        }
    }
}
#endregion
