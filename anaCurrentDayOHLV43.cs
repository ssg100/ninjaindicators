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

public enum anaPlotAlignCD43 {Left, Right, DoNotPlot}
public enum anaSessionTypeCD43 {ETH, RTH}
public enum anaSessionCountCD43 {First, Second, Third, Auto}
public enum anaTargetTypeCD43 {Average_Daily_Range, Average_Daily_Expansion}
public enum anaDataLocationCD43 {Right_Format_Long, Left_Format_Long, Center_Format_Long, Right_Format_Short, Left_Format_Short, Center_Format_Short, DoNotPlot}

#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Current Open, High, Low and Fibonacci Levels.
	/// </summary>
	[Description("anaCurrentDayOHLV43.")]
	public class anaCurrentDayOHLV43 : Indicator
	{
		#region Variables
		private	SolidBrush[]		brushes					= { new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black), 
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black), 
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black),  
															new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black),  
															new SolidBrush(Color.Black)};  
		private DateTime 			sessionBegin			= Cbi.Globals.MinDate;
		private DateTime 			sessionEnd 				= Cbi.Globals.MinDate;
		private DateTime			plotStart1				= Cbi.Globals.MaxDate;
		private DateTime			plotStart2				= Cbi.Globals.MaxDate;
		private DateTime			cacheSessionBeginTmp   	= Cbi.Globals.MinDate;
		private DateTime			cacheSessionEndTmp		= Cbi.Globals.MinDate;
		private DateTime			cacheSessionDate		= Cbi.Globals.MinDate;
		private DateTime			sessionDateTmp			= Cbi.Globals.MinDate;
		private DateTime			currentDate				= Cbi.Globals.MinDate;
		private DateTime			lastBarTimeStamp1		= Cbi.Globals.MinDate;
		private double				currentOpen				= 0.0;
		private	double				currentHigh				= double.MinValue;
		private	double				currentLow				= double.MaxValue;
		private double				currentClose			= 0.0;
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
		private bool				rthOHL					= false;
		private bool				plotCurrentOpen			= false;
		private bool				plotFibonacci			= false;
		private bool				plotHiLo				= false;
		private bool				plotMidline				= false;
		private bool				plotNoiseLevels			= false;
		private bool				plotTargetLevels		= false;
		private bool				plotCurrentClose		= true;
		private bool				showCurrentOpen			= true;
		private bool				showFibonacci			= false;
		private bool				showHiLo				= true;
		private bool				showMidline				= false;
		private bool				showNoiseLevels			= true;
		private bool				showTargetLevels		= true;
		private bool				showNoiseBands			= true;
		private bool				showTargetBands			= true;
		private bool				showPriorPeriods		= true;
		private bool				showCurrentClose		= true;
		private bool				includeAfterSession		= true;
		private bool				isCurrency				= false;
		private bool				isGlobex				= false;
		private bool				targetHit				= false;
		private ArrayList			newSessionBarIdxArr1	= new ArrayList();
		private anaPlotAlignCD43	plotLabels				= anaPlotAlignCD43.Right;
		private anaDataLocationCD43 showRangeData			= anaDataLocationCD43.Right_Format_Long;
		private anaSessionTypeCD43	currentSession			= anaSessionTypeCD43.RTH;
		private anaSessionCountCD43 selectedSession			= anaSessionCountCD43.Auto;
		private anaSessionCountCD43 activeSession			= anaSessionCountCD43.Auto;
		private anaTargetTypeCD43	targetType				= anaTargetTypeCD43.Average_Daily_Range;
		private Data.PivotRange		pivotRangeType1			= PivotRange.Daily;
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
		private string				errorData1				= "The Current Day OHL indicator can only be displayed on intraday charts.";
		private	string				errorData2				= "The Current Day OHL indicator Indicator did not find a third intraday session.";
		private	string				errorData4				= "Insufficient historical data for the Current Day OHL indicator. Please increase the chart look back period.";
		private string				errorData5				= "Insufficient historical data to calculate daily noise and volatility. With the selected settings, the Current Day OHL indicator may require a chart lookback period equivalent to ";
		private string				errorData5b				= " business days.";
        private float				errorTextHeight			= 0;
		private	StringFormat		stringFormatFar			= new StringFormat();
		private	StringFormat		stringFormatNear		= new StringFormat();
		private	StringFormat		stringFormatCenter		= new StringFormat();
		private int					countDown				= 0;
		private int					numberOfSessions		= 1;
		private int					sessionCount			= 0;
		private int					rangePeriod				= 0;
		private int					rangePeriod1			= 10;
		private int					rangePeriod2			= 20;
		private int					dayCount				= 0;
		private int					highBar					= 0;
		private int					lowBar					= 0;
		private int					factorNoiseBands		= 100;
		private int					factorDailyRange		= 100;
		private int					width					= 20;
		private int 				labelFontSize			= 8;
		private int					labelOffset				= 15;
		private int 				textFontSize			= 10;
		private int					opacityN				= 4;
		private int					opacityT				= 4;
		private int					opacityD				= 10;
		private int					displayFactor			= 1;
		private Color 				openColor				= Color.Plum;
		private Color 				highColor				= Color.Lime;
		private Color 				lowColor				= Color.Red;
		private Color 				closeColor				= Color.Yellow;
		private Color 				midlineColor			= Color.PaleGoldenrod;
		private Color 				fibColor				= Color.PaleGoldenrod;
		private Color 				noiseBandColor			= Color.LightSkyBlue;
		private Color 				targetBandColor			= Color.MediumVioletRed;
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
		private DateTime			publicHoliday0			= new DateTime (2009,01,19);
		private DateTime			publicHoliday1			= new DateTime (2009,02,16);
		private DateTime			publicHoliday2			= new DateTime (2009,05,25);
		private DateTime			publicHoliday3			= new DateTime (2009,07,03);
		private DateTime			publicHoliday4			= new DateTime (2009,09,07);
		private DateTime			publicHoliday5			= new DateTime (2009,11,26);
		private DateTime			publicHoliday6			= new DateTime (2010,01,18);
		private DateTime			publicHoliday7			= new DateTime (2010,02,15);
		private DateTime			publicHoliday8			= new DateTime (2010,05,31);
		private DateTime			publicHoliday9			= new DateTime (2010,07,05);
		private DateTime			publicHoliday10			= new DateTime (2010,09,06);
		private DateTime			publicHoliday11			= new DateTime (2010,11,25);
		private DateTime			publicHoliday12			= new DateTime (2011,01,17);
		private DateTime			publicHoliday13			= new DateTime (2011,02,21);
		private DateTime			publicHoliday14			= new DateTime (2011,05,30);
		private DateTime			publicHoliday15			= new DateTime (2011,07,04);
		private DateTime			publicHoliday16			= new DateTime (2011,09,05);
		private DateTime			publicHoliday17			= new DateTime (2011,11,24);
		private DateTime			publicHoliday18			= new DateTime (2012,01,16);
		private DateTime			publicHoliday19			= new DateTime (2012,02,20);
		private DateTime			publicHoliday20			= new DateTime (2012,05,28);
		private DateTime			publicHoliday21			= new DateTime (2012,07,04);
		private DateTime			publicHoliday22			= new DateTime (2012,09,03);
		private DateTime			publicHoliday23			= new DateTime (2012,11,22);
		private DateTime			publicHoliday24			= new DateTime (2013,01,21);
		private DateTime			publicHoliday25			= new DateTime (2013,02,18);
		private DateTime			publicHoliday26			= new DateTime (2013,05,27);
		private DateTime			publicHoliday27			= new DateTime (2013,07,04);
		private DateTime			publicHoliday28			= new DateTime (2013,09,02);
		private DateTime			publicHoliday29			= new DateTime (2013,11,28);
		private DateTime			publicHoliday30			= new DateTime (2014,01,20);
		private DateTime			publicHoliday31			= new DateTime (2014,02,17);
		private DateTime			publicHoliday32			= new DateTime (2014,05,26);
		private DateTime			publicHoliday33			= new DateTime (2014,07,04);
		private DateTime			publicHoliday34			= new DateTime (2014,09,01);
		private DateTime			publicHoliday35			= new DateTime (2014,11,27);
		private DateTime			publicHoliday36			= new DateTime (2015,01,19);
		private DateTime			publicHoliday37			= new DateTime (2015,02,16);
		private DateTime			publicHoliday38			= new DateTime (2015,05,25);
		private DateTime			publicHoliday39			= new DateTime (2015,07,03);
		private DateTime			publicHoliday40			= new DateTime (2015,09,07);
		private DateTime			publicHoliday41			= new DateTime (2015,11,26);
		private DateTime[]			publicHoliday			= new DateTime [42];
		private List<double>		noiseList				= new List<double>();
		private List<double>		rangeList				= new List<double>();
		private DataSeries			displayedWidth;
		private DataSeries			ADN0;
		private DataSeries			ADN1;
		private DataSeries			ADN2;
		private DataSeries			ADR0;
		private DataSeries			ADR1;
		private DataSeries			ADR2;
		private BoolSeries			lowIsLast;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"D-Open "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"D-High "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"D-Low "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"D-Mid "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"D-78,6 "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"D-61,8 "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"D-38,2 "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"D-23,6 "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"D-Close "));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"ADN-High"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"ADN-Low"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"ADR-High"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"ADR-Low"));
			
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
			
			ADN0							= new DataSeries(this);
			ADN1 							= new DataSeries(this);
			ADN2							= new DataSeries(this);
			ADR0							= new DataSeries(this);
			ADR1							= new DataSeries(this);
			ADR2							= new DataSeries(this);
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
				publicHoliday[4] = publicHoliday4;
				publicHoliday[5] = publicHoliday5;
				publicHoliday[6] = publicHoliday6;
				publicHoliday[7] = publicHoliday7;
				publicHoliday[8] = publicHoliday8;
				publicHoliday[9] = publicHoliday9;
				publicHoliday[10] = publicHoliday10;
				publicHoliday[11] = publicHoliday11;
				publicHoliday[12] = publicHoliday12;
				publicHoliday[13] = publicHoliday13;
				publicHoliday[14] = publicHoliday14;
				publicHoliday[15] = publicHoliday15;
				publicHoliday[16] = publicHoliday16;
				publicHoliday[17] = publicHoliday17;
				publicHoliday[18] = publicHoliday18;
				publicHoliday[19] = publicHoliday19;
				publicHoliday[20] = publicHoliday20;
				publicHoliday[21] = publicHoliday21;
				publicHoliday[22] = publicHoliday22;
				publicHoliday[23] = publicHoliday23;
				publicHoliday[24] = publicHoliday24;
				publicHoliday[25] = publicHoliday25;
				publicHoliday[26] = publicHoliday26;
				publicHoliday[27] = publicHoliday27;
				publicHoliday[28] = publicHoliday28;
				publicHoliday[29] = publicHoliday29;
				publicHoliday[30] = publicHoliday30;
				publicHoliday[31] = publicHoliday31;
				publicHoliday[32] = publicHoliday32;
				publicHoliday[33] = publicHoliday33;
				publicHoliday[34] = publicHoliday34;
				publicHoliday[35] = publicHoliday35;
				publicHoliday[36] = publicHoliday36;
				publicHoliday[37] = publicHoliday37;
				publicHoliday[38] = publicHoliday38;
				publicHoliday[39] = publicHoliday39;
				publicHoliday[40] = publicHoliday40;
				publicHoliday[41] = publicHoliday41;
			}			
			else for(int i=0; i<42; i++)
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
			if (selectedSession == anaSessionCountCD43.Auto)
			{
				if (isCurrency)
					activeSession = anaSessionCountCD43.Third;
				else
					activeSession = anaSessionCountCD43.Second;
			}
			else 
				activeSession = selectedSession; 
			if(showRangeData == anaDataLocationCD43.Left_Format_Long || showRangeData == anaDataLocationCD43.Right_Format_Long || showRangeData == anaDataLocationCD43.Center_Format_Long) 
			{
				if (currentSession == anaSessionTypeCD43.ETH)
				{
					if(targetType== anaTargetTypeCD43.Average_Daily_Range)
					{
						rangeText0 = "Current Daily Range ETH = ";
						rangeText1 = "Average Daily Range [" + rangePeriod1 + "] = ";
						rangeText2 = "Average Daily Range [" + rangePeriod2 + "] = ";
					}
					else
					{
						rangeText0 = "Current Daily Expansion ETH = ";
						rangeText1 = "Average Daily Expansion [" + rangePeriod1 + "] = ";
						rangeText2 = "Average Daily Expansion [" + rangePeriod2 + "] = ";
					}
					noiseText0 = "Current Daily Noise ETH = ";
					noiseText1 = "Average Daily Noise [" + rangePeriod1 + "] = ";
					noiseText2 = "Average Daily Noise [" + rangePeriod2 + "] = ";
					rthOHL = false;
				}
				else
				{
					if(targetType== anaTargetTypeCD43.Average_Daily_Range)
					{
						rangeText0 = "Current Daily Range RTH = ";
						rangeText1 = "Average Daily Range [" + rangePeriod1 + "] = ";
						rangeText2 = "Average Daily Range [" + rangePeriod2 + "] = ";
					}
					else
					{
						rangeText0 = "Current Daily Expansion RTH = ";
						rangeText1 = "Average Daily Expansion [" + rangePeriod1 + "] = ";
						rangeText2 = "Average Daily Expansion [" + rangePeriod2 + "] = ";
					}
					noiseText0 = "Current Daily Noise RTH = ";
					noiseText1 = "Average Daily Noise [" + rangePeriod1 + "] = ";
					noiseText2 = "Average Daily Noise [" + rangePeriod2 + "] = ";
					rthOHL = true;
				}
			}
			else if(showRangeData == anaDataLocationCD43.Left_Format_Short || showRangeData == anaDataLocationCD43.Right_Format_Short || showRangeData == anaDataLocationCD43.Center_Format_Short) 
			{
				if (currentSession == anaSessionTypeCD43.ETH)
				{
					if(targetType== anaTargetTypeCD43.Average_Daily_Range)
					{
						rangeText0 = "CDR ETH = ";
						rangeText1 = "ADR [" + rangePeriod1 + "] = ";
						rangeText2 = "ADR [" + rangePeriod2 + "] = ";
					}
					else
					{
						rangeText0 = "CDE ETH = ";
						rangeText1 = "ADE [" + rangePeriod1 + "] = ";
						rangeText2 = "ADE [" + rangePeriod2 + "] = ";
					}
					noiseText0 = "CDN ETH = ";
					noiseText1 = "ADN [" + rangePeriod1 + "] = ";
					noiseText2 = "ADN [" + rangePeriod2 + "] = ";
					rthOHL = false;
				}
				else
				{
					if(targetType== anaTargetTypeCD43.Average_Daily_Range)
					{
						rangeText0 = "CDR RTH = ";
						rangeText1 = "ADR [" + rangePeriod1 + "] = ";
						rangeText2 = "ADR [" + rangePeriod2 + "] = ";
					}
					else
					{
						rangeText0 = "CDE RTH = ";
						rangeText1 = "ADE [" + rangePeriod1 + "] = ";
						rangeText2 = "ADE [" + rangePeriod2 + "] = ";
					}
					noiseText0 = "CDN RTH = ";
					noiseText1 = "ADN [" + rangePeriod1 + "] = ";
					noiseText2 = "ADN [" + rangePeriod2 + "] = ";
					rthOHL = true;
				}
			}
			else
			{
				if (currentSession == anaSessionTypeCD43.ETH)
					rthOHL = false;
				else
					rthOHL = true;
			}
			if(targetType == anaTargetTypeCD43.Average_Daily_Range)
			{
				Plots[11].Name = "ADR-High";
				Plots[12].Name = "ADR-Low";
			}				
			else	
			{
				Plots[11].Name = "ADE-High";
				Plots[12].Name = "ADE-Low";
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
			if (!showCurrentClose || closeColor == Color.Transparent)
				plotCurrentClose = false;
			else
				plotCurrentClose = true;
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
			if(showCurrentClose)
				Plots[8].Pen.Color = closeColor;
			else
				Plots[8].Pen.Color = Color.Transparent;
			if (showNoiseLevels)
			{
				Plots[9].Pen.Color = noiseBandColor;
				Plots[10].Pen.Color = noiseBandColor;
			}
			else
			{
				Plots[9].Pen.Color = Color.Transparent;
				Plots[10].Pen.Color = Color.Transparent;
			}
			if(showTargetLevels)
			{
				Plots[11].Pen.Color = targetBandColor;
				Plots[12].Pen.Color = targetBandColor;
			}
			else
			{
				Plots[11].Pen.Color = Color.Transparent;
				Plots[12].Pen.Color = Color.Transparent;
			}
			Plots[0].Pen.Width = plot0Width;
			Plots[0].Pen.DashStyle = dash0Style;
			for (int i = 1; i < 3; i++)
			{
				Plots[i].Pen.Width = plot1Width;
				Plots[i].Pen.DashStyle = dash1Style;
			}
			for (int i = 3; i < 9; i++)
			{
				Plots[i].Pen.Width = plot3Width;
				Plots[i].Pen.DashStyle = dash3Style;
			}			
			for (int i = 9; i< 13; i++)
			{
				Plots[i].Pen.Width = plot8Width;
				Plots[i].Pen.DashStyle = dash8Style;
			}				
			rangePeriod = Math.Max(rangePeriod1, rangePeriod2);
			if(ChartControl != null)
			{
				errorBrush.Color = ChartControl.AxisColor;
				errorData5 = errorData5 + (rangePeriod + 2) + errorData5b;
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
			dayCount = 0;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (Bars == null)
				return; 
			if (!Data.BarsType.GetInstance(Bars.Period.Id).IsIntraday)
			{
				DrawTextFixed("errortag1", errorData1, TextPosition.Center, errorBrush.Color, errorFont, Color.Transparent,Color.Transparent,0);
				return;
			}
			if (CurrentBar == 0)
			{	
				currentDate = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType1);
				sessionCount = 1;
				return;
			}

			lastBarTimeStamp1 = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType1);
			if (lastBarTimeStamp1 != currentDate)
			{	
				if (countDown == 0)
				{
					Bars.Session.GetNextBeginEnd(Bars, 0, out sessionBegin, out sessionEnd);
					plotStart1 = sessionBegin;
					countDown = -1;
				}
				if (!extendPublicHoliday)
				{
					if(dayCount == rangePeriod)
					{
						Bars.Session.GetNextBeginEnd(Bars, 0, out sessionBegin, out sessionEnd);
						plotStart2 = sessionBegin;
					}
					dayCount = dayCount + 1;
					for (int i = rangePeriod - 1; i>0; i--)
					{
						noiseList[i] = noiseList[i-1];
						rangeList[i] = rangeList[i-1];
					}
					if (dayCount > 1)
					{
						noiseList [0] = Math.Min(currentHigh - currentOpen,currentOpen - currentLow);
						if(targetType == anaTargetTypeCD43.Average_Daily_Range)
							rangeList [0] = currentHigh - currentLow;
						else
							rangeList[0] = Math.Max(currentHigh - currentOpen,currentOpen - currentLow);
					}
					if (dayCount > rangePeriod)
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
						displayedRange = Math.Round(factorDailyRange * averageRange / (displaySize *100))*displaySize;
						bandWidth = Math.Max(displaySize, 0.08*factorNoiseBands*averageNoise/100);
					}
				}
				sessionCount = 1;
				calcOpen = false;
				initPlot = false;
				targetHit = false;
				if (numberOfSessions == 1 || !rthOHL || (rthOHL && activeSession == anaSessionCountCD43.First))
				{
					calcOpen = true;
					initPlot = true;
					if (!extendPublicHoliday || rthOHL)
					{
						currentOpen			= Open[0];
						upperNoiseTarget 	= currentOpen + displayedNoise;
						lowerNoiseTarget 	= currentOpen - displayedNoise;
						if(targetType == anaTargetTypeCD43.Average_Daily_Expansion)
						{
							upperRangeTarget 	= currentOpen + displayedRange;
							lowerRangeTarget 	= currentOpen - displayedRange;
						}
						currentHigh			= High[0];
						currentLow			= Low[0];
						currentClose		= Close[0];
						highBar				= CurrentBar;
						lowBar				= CurrentBar;
					}
					else
					{
						if (High[0] >= currentHigh)
						{
							currentHigh 		= High[0];
							highBar 			= CurrentBar;
						}
						if (Low[0] <= currentLow)
						{
							currentLow 			= Low[0];
							lowBar 				= CurrentBar;
						}
						currentClose		= Close[0];
					}
				}
				currentDate = lastBarTimeStamp1;
				plotOHL = true;
			}
			else if (FirstTickOfBar && Bars.FirstBarOfSession)
			{
				sessionCount = sessionCount + 1;
				numberOfSessions = Math.Min(3, Math.Max(sessionCount, numberOfSessions));
				if (!rthOHL)
				{
					if (High[0] >= currentHigh)
					{
						currentHigh = High[0];
						highBar = CurrentBar;
					}
					if (Low[0] <= currentLow)
					{
						currentLow = Low[0];
						lowBar = CurrentBar;
					}
					currentClose		= Close[0];
				}
				else if (rthOHL && ((sessionCount == 1 && activeSession == anaSessionCountCD43.First) || (sessionCount == 2 
					&& activeSession == anaSessionCountCD43.Second) || (sessionCount == 3 && activeSession == anaSessionCountCD43.Third)))
				{
					currentOpen			= Open[0];
					upperNoiseTarget 	= currentOpen + displayedNoise;
					lowerNoiseTarget 	= currentOpen - displayedNoise;
					if(targetType == anaTargetTypeCD43.Average_Daily_Expansion)
					{
						upperRangeTarget 	= currentOpen + displayedRange;
						lowerRangeTarget 	= currentOpen - displayedRange;
					}
					calcOpen			= true;
					initPlot 			= true;
					currentHigh			= High[0];
					currentLow			= Low[0];
					currentClose		= Close[0];
					highBar				= CurrentBar;
					lowBar				= CurrentBar;
				}
				else
					calcOpen = false;
			}
			else if (calcOpen)
			{
				if (High[0] >= currentHigh)
				{
					currentHigh			= High[0];
					highBar 			= CurrentBar;
				}
				if (Low[0] <= currentLow)
				{
					currentLow 			= Low[0];
					lowBar				= CurrentBar;
				}
				currentClose		= Close[0];
			}
			
			if(targetType == anaTargetTypeCD43.Average_Daily_Range && calcOpen && dayCount > rangePeriod && !targetHit)
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

			if (plotOHL && initPlot && !(rthOHL && activeSession == anaSessionCountCD43.Third && numberOfSessions == 2) &&
				!(rthOHL && !includeAfterSession && activeSession == anaSessionCountCD43.Second && sessionCount>2) &&
				!(rthOHL && !includeAfterSession && activeSession == anaSessionCountCD43.First && sessionCount>1))
			{
				CurrentOpen.Set(currentOpen);
				CurrentHigh.Set(currentHigh);
				CurrentLow.Set(currentLow);
				CurrentMidline.Set(Math.Round((currentLow + 0.5*(currentHigh-currentLow))/displaySize)* displaySize);
				if(rthOHL && ((activeSession == anaSessionCountCD43.First && sessionCount > 1) || (activeSession == anaSessionCountCD43.Second && sessionCount > 2)))
					CurrentClose.Set(currentClose);
				else
					CurrentClose.Set(0);
				if (lowBar >= highBar)				
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
				if (dayCount > rangePeriod)
				{
					if(FirstTickOfBar)
					{
						RemoveDrawObject("errortag3");
						UDNL.Set (Math.Round(upperNoiseTarget/displaySize)*displaySize);
						LDNL.Set (Math.Round(lowerNoiseTarget/displaySize)*displaySize);
						UDET.Set (Math.Round(upperRangeTarget/displaySize)*displaySize);
						LDET.Set (Math.Round(lowerRangeTarget/displaySize)*displaySize);
						ADN1.Set (Math.Round(averageNoise1*displayFactor/displaySize)*displaySize);
						ADN2.Set (Math.Round(averageNoise2*displayFactor/displaySize)*displaySize);
						ADR1.Set (Math.Round(averageRange1*displayFactor/displaySize)*displaySize);
						ADR2.Set (Math.Round(averageRange2*displayFactor/displaySize)*displaySize);
						displayedWidth.Set(bandWidth);
					}
					ADN0.Set(Math.Round((Math.Min(currentHigh - currentOpen,currentOpen - currentLow))/displaySize)* displaySize * displayFactor);
					if(targetType == anaTargetTypeCD43.Average_Daily_Range)
						ADR0.Set(Math.Round((currentHigh - currentLow)/displaySize)* displaySize * displayFactor);
					else
						ADR0.Set(Math.Round((Math.Max(currentHigh - currentOpen,currentOpen - currentLow))/displaySize)* displaySize * displayFactor);
				}
				else if (FirstTickOfBar)
				{
					UDNL.Reset();
					LDNL.Reset();
					UDET.Reset();
					LDET.Reset();
					ADN0.Reset();
					ADN1.Reset();
					ADN2.Reset();
					ADR0.Reset();
					ADR1.Reset();
					ADR2.Reset();
					displayedWidth.Reset();
				}
			}
			else if (FirstTickOfBar)
			{
				CurrentHigh.Reset();
   			  	CurrentLow.Reset();
				CurrentMidline.Reset();
				CurrentOpen.Reset();
				CurrentClose.Reset();
				Fib786.Reset(); 
				Fib618.Reset(); 
				Fib382.Reset(); 
				Fib236.Reset();
				UDNL.Reset();
				LDNL.Reset();
				UDET.Reset();
				LDET.Reset();
				ADN0.Reset();
				ADR0.Reset();
				if (dayCount > rangePeriod && !(rthOHL && activeSession == anaSessionCountCD43.Third && numberOfSessions == 2))
				{
					ADN1.Set (Math.Round(averageNoise1*displayFactor/displaySize)*displaySize);
					ADN2.Set (Math.Round(averageNoise2*displayFactor/displaySize)*displaySize);
					ADR1.Set (Math.Round(averageRange1*displayFactor/displaySize)*displaySize);
					ADR2.Set (Math.Round(averageRange2*displayFactor/displaySize)*displaySize);
				}
				else
				{
					if (dayCount > 1 && plotOHL && rthOHL && activeSession == anaSessionCountCD43.Third && numberOfSessions == 2)
						DrawTextFixed("errortag2", errorData2, TextPosition.Center, errorBrush.Color, errorFont, Color.Transparent,Color.Transparent,0);
					ADN1.Reset();
					ADN2.Reset();
					ADR1.Reset();
					ADR2.Reset();
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
		public anaSessionTypeCD43 CurrentSession
		{
			get { return currentSession; }
			set { currentSession = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Session # used for calculating OHL, noise and target bands")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Select RTH session")]
		public anaSessionCountCD43 SelectedSession
		{
			get { return selectedSession; }
			set { selectedSession = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Formula used for calculating expansion targets")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Target formula")]
		public anaTargetTypeCD43 TargetType
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
		
		/// <summary>
		/// </summary>
		[Description("Option to show regular close until the end of the trading day")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Show regular close")]
		public bool ShowCurrentClose 
		{
			get { return showCurrentClose; }
			set { showCurrentClose = value; }
		}
		
		[Description("Option to show dynamic Fibonacci Lines")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Show dynamic fibs")]
		public bool ShowFibonacci 
		{
			get { return showFibonacci; }
			set { showFibonacci = value; }
		}
		
		[Description("Option to show current day's high and low")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Show high/low")]
		public bool ShowHiLo 
		{
			get { return showHiLo; }
			set { showHiLo = value; }
		}
		
		[Description("Option to show current day's midline")]
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
		
		[Description("Option to show target levels based on average daily range")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Show target levels")]
		public bool ShowTargetLevels 
		{
			get { return showTargetLevels; }
			set { showTargetLevels = value; }
		}
		
		[Description("Option to extend plots in RTH mode until the end of the trading day")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Extend RTH plots until close")]
		public bool IncludeAfterSession
		{
			get { return includeAfterSession; }
			set { includeAfterSession = value; }
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
		[Description("# of days used to calculate average daily noise, expansion and range.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Lookback 1 in days")]
		public int RangePeriod1
		{
			get { return rangePeriod1; }
			set { rangePeriod1 = Math.Min(100,Math.Max(1, value)); }
		}

		/// <summary>
		/// </summary>
		[Description("# of days used to calculate average daily noise, expansion and range.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Lookback 2 in days")]
		public int RangePeriod2
		{
			get { return rangePeriod2; }
			set { rangePeriod2 = Math.Min(100,Math.Max(1, value)); }
		}

		/// <summary>
		/// </summary>
		[Description("Percent of average daily noise used to calculate the bands")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Noise range in % of ADN")]
		public int FactorNoiseBands
		{
			get { return factorNoiseBands; }
			set { factorNoiseBands = Math.Min(400, Math.Max(10, value)); }
		}

		/// <summary>
		/// </summary>
		[Description("Percent of average daily expansion used to calculate the bands")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Target range in % of ADE/ADR")]
		public int FactorDailyRange
		{
			get { return factorDailyRange; }
			set { factorDailyRange = Math.Min(400, Math.Max(10, value)); }
		}

		[Description("Option to show target bands based on average daily range")]
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
		public anaDataLocationCD43 ShowRangeData 
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
		public DataSeries CurrentClose
		{
			get { return Values[8]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries UDNL
		{
			get { return Values[9]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries LDNL
		{
			get { return Values[10]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries UDET
		{
			get { return Values[11]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries LDET
		{
			get { return Values[12]; }
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
		public anaPlotAlignCD43 PlotLabels
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
		[Description("Select color for regular close")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Color regular close")]
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

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 05 /no trade date")]
		public DateTime PublicHoliday4
		{
			get { return publicHoliday4;}
			set { publicHoliday4 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 06 /no trade date")]
		public DateTime PublicHoliday5
		{
			get { return publicHoliday5;}
			set { publicHoliday5 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 07 /no trade date")]
		public DateTime PublicHoliday6
		{
			get { return publicHoliday6;}
			set { publicHoliday6 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 08 /no trade date")]
		public DateTime PublicHoliday7
		{
			get { return publicHoliday7;}
			set { publicHoliday7 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 09 /no trade date")]
		public DateTime PublicHoliday8
		{
			get { return publicHoliday8;}
			set { publicHoliday8 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 10 /no trade date")]
		public DateTime PublicHoliday9
		{
			get { return publicHoliday9;}
			set { publicHoliday9 = value;}
		}	
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 11 /no trade date")]
		public DateTime PublicHoliday10
		{
			get { return publicHoliday10;}
			set { publicHoliday10 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 12 /no trade date")]
		public DateTime PublicHoliday11
		{
			get { return publicHoliday11;}
			set { publicHoliday11 = value;}
		}	
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 13 /no trade date")]
		public DateTime PublicHoliday12
		{
			get { return publicHoliday12;}
			set { publicHoliday12 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 14 /no trade date")]
		public DateTime PublicHoliday13
		{
			get { return publicHoliday13;}
			set { publicHoliday13 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 15 /no trade date")]
		public DateTime PublicHoliday14
		{
			get { return publicHoliday14;}
			set { publicHoliday14 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 16 /no trade date")]
		public DateTime PublicHoliday15
		{
			get { return publicHoliday15;}
			set { publicHoliday15 = value;}
		}	
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 17 /no trade date")]
		public DateTime PublicHoliday16
		{
			get { return publicHoliday16;}
			set { publicHoliday16 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 18 /no trade date")]
		public DateTime PublicHoliday17
		{
			get { return publicHoliday17;}
			set { publicHoliday17 = value;}
		}	
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 19 /no trade date")]
		public DateTime PublicHoliday18
		{
			get { return publicHoliday18;}
			set { publicHoliday18 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 20 /no trade date")]
		public DateTime PublicHoliday19
		{
			get { return publicHoliday19;}
			set { publicHoliday19 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 21 /no trade date")]
		public DateTime PublicHoliday20
		{
			get { return publicHoliday20;}
			set { publicHoliday20 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 22 /no trade date")]
		public DateTime PublicHoliday21
		{
			get { return publicHoliday21;}
			set { publicHoliday21 = value;}
		}	
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 23 /no trade date")]
		public DateTime PublicHoliday22
		{
			get { return publicHoliday22;}
			set { publicHoliday22 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 24 /no trade date")]
		public DateTime PublicHoliday23
		{
			get { return publicHoliday23;}
			set { publicHoliday23 = value;}
		}		
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 25 /no trade date")]
		public DateTime PublicHoliday24
		{
			get { return publicHoliday24;}
			set { publicHoliday24 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 26 /no trade date")]
		public DateTime PublicHoliday25
		{
			get { return publicHoliday25;}
			set { publicHoliday25 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 27 /no trade date")]
		public DateTime PublicHoliday26
		{
			get { return publicHoliday26;}
			set { publicHoliday26 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 28 /no trade date")]
		public DateTime PublicHoliday27
		{
			get { return publicHoliday27;}
			set { publicHoliday27 = value;}
		}	
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 29 /no trade date")]
		public DateTime PublicHoliday28
		{
			get { return publicHoliday28;}
			set { publicHoliday28 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 30 /no trade date")]
		public DateTime PublicHoliday29
		{
			get { return publicHoliday29;}
			set { publicHoliday29 = value;}
		}	
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 31 /no trade date")]
		public DateTime PublicHoliday30
		{
			get { return publicHoliday30;}
			set { publicHoliday30 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 32 /no trade date")]
		public DateTime PublicHoliday31
		{
			get { return publicHoliday31;}
			set { publicHoliday31 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 33 /no trade date")]
		public DateTime PublicHoliday32
		{
			get { return publicHoliday32;}
			set { publicHoliday32 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 34 /no trade date")]
		public DateTime PublicHoliday33
		{
			get { return publicHoliday33;}
			set { publicHoliday33 = value;}
		}	
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 35 /no trade date")]
		public DateTime PublicHoliday34
		{
			get { return publicHoliday34;}
			set { publicHoliday34 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 36 /no trade date")]
		public DateTime PublicHoliday35
		{
			get { return publicHoliday35;}
			set { publicHoliday35 = value;}
		}

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 37 /no trade date")]
		public DateTime PublicHoliday36
		{
			get { return publicHoliday36;}
			set { publicHoliday36 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 38 /no trade date")]
		public DateTime PublicHoliday37
		{
			get { return publicHoliday37;}
			set { publicHoliday37 = value;}
		}
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 39 /no trade date")]
		public DateTime PublicHoliday38
		{
			get { return publicHoliday38;}
			set { publicHoliday38 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 40 /no trade date")]
		public DateTime PublicHoliday39
		{
			get { return publicHoliday39;}
			set { publicHoliday39 = value;}
		}	
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 41 /no trade date")]
		public DateTime PublicHoliday40
		{
			get { return publicHoliday40;}
			set { publicHoliday40 = value;}
		}	

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement Next Day For Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 42 /no trade date")]
		public DateTime PublicHoliday41
		{
			get { return publicHoliday41;}
			set { publicHoliday41 = value;}
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
			if (time > cacheSessionEndTmp) 
			{
				extendPublicHoliday = false;
				for (int i =0; i<42; i++)
				{
					if (publicHoliday[i].Date == sessionDateTmp)
						extendPublicHoliday = true;
				}
				if (Bars.BarsType.IsIntraday)
					sessionDateTmp = Bars.GetTradingDayFromLocal(time);
				else
					sessionDateTmp = time.Date;
				if(cacheSessionDate != sessionDateTmp || (rthOHL && sessionCount == 1 && activeSession == anaSessionCountCD43.Second)
					|| (rthOHL && sessionCount ==2 && activeSession == anaSessionCountCD43.Third)) 
				{
					cacheSessionDate = sessionDateTmp;
					if ((!extendPublicHoliday || rthOHL) && (newSessionBarIdxArr1.Count == 0 
						|| (newSessionBarIdxArr1.Count > 0 && CurrentBar > (int) newSessionBarIdxArr1[newSessionBarIdxArr1.Count - 1])))
							newSessionBarIdxArr1.Add(CurrentBar);
				}
				Bars.Session.GetNextBeginEnd(bars, barsAgo, out cacheSessionBeginTmp, out cacheSessionEndTmp); 
				if(tickBuilt)
					cacheSessionEndTmp = cacheSessionEndTmp.AddSeconds(-1);
			}
			return sessionDateTmp;
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
			SizeF errorSize = graphics.MeasureString(errorData4, errorFont);
			errorTextHeight	= errorSize.Height + 5;
			if (plotStart1 >= lastBarSessionEnd)
				graphics.DrawString(errorData4, errorFont, errorBrush, bounds.X + bounds.Width, bounds.Y + bounds.Height - errorTextHeight, stringFormatFar);
			else if (plotStart2 >= lastBarSessionEnd && (showNoiseLevels || showTargetLevels || showNoiseBands || showTargetBands || showRangeData != anaDataLocationCD43.DoNotPlot))
				graphics.DrawString(errorData5, errorFont, errorBrush, bounds.X + bounds.Width, bounds.Y + bounds.Height - errorTextHeight, stringFormatFar);
			
			int lastBarIndex	= this.LastBarIndexPainted;
			int firstBarIndex	= Math.Max(BarsRequired, this.FirstBarIndexPainted - 1);
			
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
				for (int i = newSessionBarIdxArr1.Count - 1; i >= 0; i--)
				{
					int prevSessionBreakIdx = (int) newSessionBarIdxArr1[i];
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
						Plots[4].Name = "D-78.6";
						Plots[5].Name = "D-61.8";
						Plots[6].Name = "D-38.2";
						Plots[7].Name = "D-23.6";
					}
					else
					{
						Plots[4].Name = "D-23.6";
						Plots[5].Name = "D-38.2";
						Plots[6].Name = "D-61.8";
						Plots[7].Name = "D-78.6";
					}
				}
				
				for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
				{
					yArr[seriesCount] 	= ChartControl.GetYByValue(this, Values[seriesCount].Get(lastBarIndex));
					labels[seriesCount] = Plots[seriesCount].Name;
					plotColors[seriesCount] = Plots[seriesCount].Pen.Color;
				}
				if(plotCurrentOpen && plotCurrentClose)
				{
					if (yArr[0] == yArr[8])
					{
						labels[8] = Plots[0].Name + "/" + Plots[8].Name;
						plotColors[0] = Color.Transparent;
					}
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
				if(plotHiLo && plotCurrentClose)
				{
					for (int i = 1; i < 3; i++)
						if (yArr[i] == yArr[8])
						{
							labels[i] = Plots[i].Name + "/" + Plots[8].Name;
							plotColors[8] = Color.Transparent;
						}
				}
				if(plotNoiseLevels && plotMidline)
				{
					for (int i = 9; i < 11; i++)
						if (yArr[i] == yArr[3])
						{
							labels[i] = Plots[3].Name + "/" + Plots[i].Name;
							plotColors[3] = Color.Transparent;
						}
				}				
				if(plotNoiseLevels && plotFibonacci)
				{
					for (int i = 9; i < 11; i++)
						for( int j = 4; j < 8; j++)
							if (yArr[i] == yArr[j])
							{
								labels[i] = Plots[j].Name + "/" + Plots[i].Name;
								plotColors[j] = Color.Transparent;
							}
				}				
				if(plotNoiseLevels && plotCurrentOpen)
				{
					for (int i = 9; i < 11; i++)
						if (yArr[i] == yArr[0])
						{
							labels[i] = Plots[0].Name + "/" + Plots[i].Name;
							plotColors[0] = Color.Transparent;
						}
				}
				if(plotNoiseLevels && plotHiLo)
				{
					for (int i = 9; i < 11; i++)
						for( int j = 1; j < 3; j++)
							if (yArr[i] == yArr[j])
							{
								labels[i] = Plots[j].Name + "/" + Plots[i].Name;
								plotColors[j] = Color.Transparent;
							}
				}				
				if(plotNoiseLevels && plotCurrentClose)
				{
					for (int i = 9; i < 11; i++)
						if (yArr[i] == yArr[8])
						{
							labels[i] = Plots[8].Name + "/" + Plots[i].Name;
							plotColors[8] = Color.Transparent;
						}
				}
				if(plotTargetLevels && plotMidline)
				{
					for (int i = 11; i < 13; i++)
						if (yArr[i] == yArr[3])
						{
							labels[i] = Plots[3].Name + "/" + Plots[i].Name;
							plotColors[3] = Color.Transparent;
						}
				}				
				if(plotTargetLevels && plotFibonacci)
				{
					for (int i = 11; i < 13; i++)
						for( int j = 4; j < 8; j++)
							if (yArr[i] == yArr[j])
							{
								labels[i] = Plots[j].Name + "/" + Plots[i].Name;
								plotColors[j] = Color.Transparent;
							}
				}				
				if(plotTargetLevels && plotCurrentOpen)
				{
					for (int i = 11; i < 13; i++)
						if (yArr[i] == yArr[0])
						{
							labels[i] = Plots[0].Name + "/" + Plots[i].Name;
							plotColors[0] = Color.Transparent;
						}
				}
				if(plotTargetLevels && plotHiLo)
				{
					for (int i = 11; i < 13; i++)
						for( int j = 1; j < 3; j++)
							if (yArr[i] == yArr[j])
							{
								labels[i] = Plots[j].Name + "/" + Plots[i].Name;
								plotColors[j] = Color.Transparent;
							}
				}	
				if(plotTargetLevels && plotCurrentClose)
				{
					for (int i = 11; i < 13; i++)
						if (yArr[i] == yArr[8])
						{
							labels[i] = Plots[8].Name + "/" + Plots[i].Name;
							plotColors[8] = Color.Transparent;
						}
				}
				if(plotTargetLevels && plotNoiseLevels)
				{
					for (int i = 11; i < 13; i++)
						for( int j = 9; j < 11; j++)
							if (yArr[i] == yArr[j])
							{
								labels[i] = Plots[j].Name + "/" + Plots[i].Name;
								plotColors[j] = Color.Transparent;
							}
				}	
				
				if(showNoiseBands && opacityN > 0 && Values[9].IsValidPlot(lastBarIndex))
				{
					double upperNoiseBand = Values[9].Get(lastBarIndex);
					double lowerNoiseBand = Values[10].Get(lastBarIndex);
					
					int upperNoiseBandUpper = ChartControl.GetYByValue(this, upperNoiseBand + bandWidth);
					int upperNoiseBandLower = ChartControl.GetYByValue(this, upperNoiseBand - bandWidth);
					int lowerNoiseBandUpper = ChartControl.GetYByValue(this, lowerNoiseBand + bandWidth);
					int lowerNoiseBandLower = ChartControl.GetYByValue(this, lowerNoiseBand - bandWidth);
					graphics.FillRectangle(noiseBrush, lastXtoFill, upperNoiseBandUpper, firstXtoFill - lastXtoFill, upperNoiseBandLower - upperNoiseBandUpper);
					graphics.FillRectangle(noiseBrush, lastXtoFill, lowerNoiseBandUpper, firstXtoFill - lastXtoFill, lowerNoiseBandLower - lowerNoiseBandUpper);
				}
				if(showTargetBands && opacityT > 0 && targetType == anaTargetTypeCD43.Average_Daily_Expansion && Values[11].IsValidPlot(lastBarIndex))
				{
					double upperTargetBand = Values[11].Get(lastBarIndex);
					double lowerTargetBand = Values[12].Get(lastBarIndex);
					double targetBandWidth = 1.5*displayedWidth.Get(lastBarIndex);
					
					int upperTargetBandUpper = ChartControl.GetYByValue(this, upperTargetBand + targetBandWidth);
					int upperTargetBandLower = ChartControl.GetYByValue(this, upperTargetBand - targetBandWidth);
					int lowerTargetBandUpper = ChartControl.GetYByValue(this, lowerTargetBand + targetBandWidth);
					int lowerTargetBandLower = ChartControl.GetYByValue(this, lowerTargetBand - targetBandWidth);
					graphics.FillRectangle(targetBrush, lastXtoFill, upperTargetBandUpper, firstXtoFill - lastXtoFill, upperTargetBandLower - upperTargetBandUpper);
					graphics.FillRectangle(targetBrush, lastXtoFill, lowerTargetBandUpper, firstXtoFill - lastXtoFill, lowerTargetBandLower - lowerTargetBandUpper);
				}
					
				int maxCount = Values.Length;
				if(targetType == anaTargetTypeCD43.Average_Daily_Range)
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
						if (PlotLabels == anaPlotAlignCD43.Right)
						graphics.DrawString(labels[seriesCount], labelFont, brush, firstX + labelOffset, yArr[seriesCount] - labelFont.GetHeight() / 2, stringFormatNear);
						if (PlotLabels == anaPlotAlignCD43.Left)
						graphics.DrawString(labels[seriesCount], labelFont, brush, lastX - labelOffset, yArr[seriesCount] - labelFont.GetHeight() / 2, stringFormatFar);
					}
					
					if(showTargetBands && opacityT > 0 && targetType == anaTargetTypeCD43.Average_Daily_Range)
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
						if (PlotLabels == anaPlotAlignCD43.Right)
						graphics.DrawString(labels[seriesCount], labelFont, brush, firstX + labelOffset, yArr[seriesCount] - labelFont.GetHeight() / 2, stringFormatNear);
						if (PlotLabels == anaPlotAlignCD43.Left)
						graphics.DrawString(labels[seriesCount], labelFont, brush, lastX - labelOffset, yArr[seriesCount] - labelFont.GetHeight() / 2, stringFormatFar);
					}
				}
				lastBarIndex = firstBarIdxToPaint;
				firstLoop = false;
			}
			while (showPriorPeriods && lastBarIndex > firstBarIndex); 

			lastBarIndex	= this.LastBarIndexPainted;
			if (ADR1.IsValidPlot(lastBarIndex) && ADR2.IsValidPlot(lastBarIndex) && ADN1.IsValidPlot(lastBarIndex) && ADN2.IsValidPlot(lastBarIndex))			
			{
				double averageDN0 = 0.0;
				if(ADN0.IsValidPlot(lastBarIndex))
					averageDN0 = ADN0.Get(lastBarIndex);
				double averageDN1 = ADN1.Get(lastBarIndex);
				double averageDN2 = ADN2.Get(lastBarIndex);
				double averageDR0 = 0.0;
				if(ADR0.IsValidPlot(lastBarIndex))
					averageDR0 = ADR0.Get(lastBarIndex);
				double averageDR1 = ADR1.Get(lastBarIndex);
				double averageDR2 = ADR2.Get(lastBarIndex);
				if (TickSize == 0.03125) 
				{
					double truncN0 = Math.Truncate(averageDN0); 
					double truncN1 = Math.Truncate(averageDN1); 
					double truncN2 = Math.Truncate(averageDN2);
					double truncR0 = Math.Truncate(averageDR0); 
					double truncR1 = Math.Truncate(averageDR1); 
					double truncR2 = Math.Truncate(averageDR2);
					int fractionN0 = Convert.ToInt32(32 * Math.Abs(averageDN0 - truncN0) - 0.0001);
					int fractionN1 = Convert.ToInt32(32 * Math.Abs(averageDN1 - truncN1) - 0.0001);
					int fractionN2 = Convert.ToInt32(32 * Math.Abs(averageDN2 - truncN2) - 0.0001);
					int fractionR0 = Convert.ToInt32(32 * Math.Abs(averageDR0 - truncR0) - 0.0001);
					int fractionR1 = Convert.ToInt32(32 * Math.Abs(averageDR1 - truncR1) - 0.0001);
					int fractionR2 = Convert.ToInt32(32 * Math.Abs(averageDR2 - truncR2) - 0.0001);
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
					double truncN0 = Math.Truncate(averageDN0); 
					double truncN1 = Math.Truncate(averageDN1); 
					double truncN2 = Math.Truncate(averageDN2);
					double truncR0 = Math.Truncate(averageDR0); 
					double truncR1 = Math.Truncate(averageDR1); 
					double truncR2 = Math.Truncate(averageDR2);
					int fractionN0 = Convert.ToInt32(320 * Math.Abs(averageDN0 - truncN0) - 0.0001);
					int fractionN1 = Convert.ToInt32(320 * Math.Abs(averageDN1 - truncN1) - 0.0001);
					int fractionN2 = Convert.ToInt32(320 * Math.Abs(averageDN2 - truncN2) - 0.0001);
					int fractionR0 = Convert.ToInt32(320 * Math.Abs(averageDR0 - truncR0) - 0.0001);
					int fractionR1 = Convert.ToInt32(320 * Math.Abs(averageDR1 - truncR1) - 0.0001);
					int fractionR2 = Convert.ToInt32(320 * Math.Abs(averageDR2 - truncR2) - 0.0001);
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
						rangeData2 = rangeText2 + truncR2.ToString() + "'" + fractionR2.ToString();				
				}
				else if (isCurrency)
				{
					noiseData0 = noiseText0 + averageDN0.ToString();	
					noiseData1 = noiseText1 + averageDN1.ToString();	
					noiseData2 = noiseText2 + averageDN2.ToString();	
					rangeData0 = rangeText0 + averageDR0.ToString();	
					rangeData1 = rangeText1 + averageDR1.ToString();	
					rangeData2 = rangeText2 + averageDR2.ToString();	
				}	
				else	
				{
					noiseData0 = noiseText0 + averageDN0.ToString(Gui.Globals.GetTickFormatString(TickSize));	
					noiseData1 = noiseText1 + averageDN1.ToString(Gui.Globals.GetTickFormatString(TickSize));	
					noiseData2 = noiseText2 + averageDN2.ToString(Gui.Globals.GetTickFormatString(TickSize));	
					rangeData0 = rangeText0 + averageDR0.ToString(Gui.Globals.GetTickFormatString(TickSize));	
					rangeData1 = rangeText1 + averageDR1.ToString(Gui.Globals.GetTickFormatString(TickSize));	
					rangeData2 = rangeText2 + averageDR2.ToString(Gui.Globals.GetTickFormatString(TickSize));	
				}
				SizeF noiseSize	= graphics.MeasureString(noiseData0, textFont);
				SizeF rangeSize	= graphics.MeasureString(rangeData0, textFont);
				if (showRangeData == anaDataLocationCD43.Left_Format_Long || showRangeData == anaDataLocationCD43.Left_Format_Short)
				{
					graphics.FillRectangle(dataBrush, bounds.X + 10, bounds.Y + 20, (rangeSize+noiseSize).Width + 55, 4.0f*textFont.Height + 20);
					graphics.DrawString(noiseData0, textFont, textBrush, bounds.X + 30, bounds.Y + 0.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(noiseData1, textFont, textBrush, bounds.X + 30, bounds.Y + 2.0f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(noiseData2, textFont, textBrush, bounds.X + 30, bounds.Y + 3.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData0, textFont, textBrush, bounds.X + noiseSize.Width + 50, bounds.Y + 0.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData1, textFont, textBrush, bounds.X + noiseSize.Width + 50, bounds.Y + 2.0f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData2, textFont, textBrush, bounds.X + noiseSize.Width + 50, bounds.Y + 3.5f*textFont.Height + 20, stringFormatNear);
				}
				else if (showRangeData == anaDataLocationCD43.Right_Format_Long || showRangeData == anaDataLocationCD43.Right_Format_Short)
				{
					graphics.FillRectangle(dataBrush, bounds.X + bounds.Width - (rangeSize + noiseSize).Width - 70, bounds.Y + 20, (rangeSize+noiseSize).Width + 55, 4.0f*textFont.Height + 20);
					graphics.DrawString(noiseData0, textFont, textBrush, bounds.X + bounds.Width - (rangeSize+noiseSize).Width - 50, bounds.Y + 0.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(noiseData1, textFont, textBrush, bounds.X + bounds.Width - (rangeSize+noiseSize).Width - 50, bounds.Y + 2.0f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(noiseData2, textFont, textBrush, bounds.X + bounds.Width - (rangeSize+noiseSize).Width - 50, bounds.Y + 3.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData0, textFont, textBrush, bounds.X + bounds.Width - rangeSize.Width - 30, bounds.Y + 0.5f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData1, textFont, textBrush, bounds.X + bounds.Width - rangeSize.Width - 30, bounds.Y + 2.0f*textFont.Height + 20, stringFormatNear);
					graphics.DrawString(rangeData2, textFont, textBrush, bounds.X + bounds.Width - rangeSize.Width - 30, bounds.Y + 3.5f*textFont.Height + 20, stringFormatNear);
				}
				else if (showRangeData == anaDataLocationCD43.Center_Format_Long || showRangeData == anaDataLocationCD43.Center_Format_Short)
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
        private anaCurrentDayOHLV43[] cacheanaCurrentDayOHLV43 = null;

        private static anaCurrentDayOHLV43 checkanaCurrentDayOHLV43 = new anaCurrentDayOHLV43();

        /// <summary>
        /// anaCurrentDayOHLV43.
        /// </summary>
        /// <returns></returns>
        public anaCurrentDayOHLV43 anaCurrentDayOHLV43(anaSessionTypeCD43 currentSession, int factorDailyRange, int factorNoiseBands, bool includeAfterSession, int rangePeriod1, int rangePeriod2, anaSessionCountCD43 selectedSession, bool showCurrentClose, bool showCurrentOpen, bool showFibonacci, bool showHiLo, bool showMidline, bool showNoiseLevels, bool showTargetLevels, anaTargetTypeCD43 targetType)
        {
            return anaCurrentDayOHLV43(Input, currentSession, factorDailyRange, factorNoiseBands, includeAfterSession, rangePeriod1, rangePeriod2, selectedSession, showCurrentClose, showCurrentOpen, showFibonacci, showHiLo, showMidline, showNoiseLevels, showTargetLevels, targetType);
        }

        /// <summary>
        /// anaCurrentDayOHLV43.
        /// </summary>
        /// <returns></returns>
        public anaCurrentDayOHLV43 anaCurrentDayOHLV43(Data.IDataSeries input, anaSessionTypeCD43 currentSession, int factorDailyRange, int factorNoiseBands, bool includeAfterSession, int rangePeriod1, int rangePeriod2, anaSessionCountCD43 selectedSession, bool showCurrentClose, bool showCurrentOpen, bool showFibonacci, bool showHiLo, bool showMidline, bool showNoiseLevels, bool showTargetLevels, anaTargetTypeCD43 targetType)
        {
            if (cacheanaCurrentDayOHLV43 != null)
                for (int idx = 0; idx < cacheanaCurrentDayOHLV43.Length; idx++)
                    if (cacheanaCurrentDayOHLV43[idx].CurrentSession == currentSession && cacheanaCurrentDayOHLV43[idx].FactorDailyRange == factorDailyRange && cacheanaCurrentDayOHLV43[idx].FactorNoiseBands == factorNoiseBands && cacheanaCurrentDayOHLV43[idx].IncludeAfterSession == includeAfterSession && cacheanaCurrentDayOHLV43[idx].RangePeriod1 == rangePeriod1 && cacheanaCurrentDayOHLV43[idx].RangePeriod2 == rangePeriod2 && cacheanaCurrentDayOHLV43[idx].SelectedSession == selectedSession && cacheanaCurrentDayOHLV43[idx].ShowCurrentClose == showCurrentClose && cacheanaCurrentDayOHLV43[idx].ShowCurrentOpen == showCurrentOpen && cacheanaCurrentDayOHLV43[idx].ShowFibonacci == showFibonacci && cacheanaCurrentDayOHLV43[idx].ShowHiLo == showHiLo && cacheanaCurrentDayOHLV43[idx].ShowMidline == showMidline && cacheanaCurrentDayOHLV43[idx].ShowNoiseLevels == showNoiseLevels && cacheanaCurrentDayOHLV43[idx].ShowTargetLevels == showTargetLevels && cacheanaCurrentDayOHLV43[idx].TargetType == targetType && cacheanaCurrentDayOHLV43[idx].EqualsInput(input))
                        return cacheanaCurrentDayOHLV43[idx];

            lock (checkanaCurrentDayOHLV43)
            {
                checkanaCurrentDayOHLV43.CurrentSession = currentSession;
                currentSession = checkanaCurrentDayOHLV43.CurrentSession;
                checkanaCurrentDayOHLV43.FactorDailyRange = factorDailyRange;
                factorDailyRange = checkanaCurrentDayOHLV43.FactorDailyRange;
                checkanaCurrentDayOHLV43.FactorNoiseBands = factorNoiseBands;
                factorNoiseBands = checkanaCurrentDayOHLV43.FactorNoiseBands;
                checkanaCurrentDayOHLV43.IncludeAfterSession = includeAfterSession;
                includeAfterSession = checkanaCurrentDayOHLV43.IncludeAfterSession;
                checkanaCurrentDayOHLV43.RangePeriod1 = rangePeriod1;
                rangePeriod1 = checkanaCurrentDayOHLV43.RangePeriod1;
                checkanaCurrentDayOHLV43.RangePeriod2 = rangePeriod2;
                rangePeriod2 = checkanaCurrentDayOHLV43.RangePeriod2;
                checkanaCurrentDayOHLV43.SelectedSession = selectedSession;
                selectedSession = checkanaCurrentDayOHLV43.SelectedSession;
                checkanaCurrentDayOHLV43.ShowCurrentClose = showCurrentClose;
                showCurrentClose = checkanaCurrentDayOHLV43.ShowCurrentClose;
                checkanaCurrentDayOHLV43.ShowCurrentOpen = showCurrentOpen;
                showCurrentOpen = checkanaCurrentDayOHLV43.ShowCurrentOpen;
                checkanaCurrentDayOHLV43.ShowFibonacci = showFibonacci;
                showFibonacci = checkanaCurrentDayOHLV43.ShowFibonacci;
                checkanaCurrentDayOHLV43.ShowHiLo = showHiLo;
                showHiLo = checkanaCurrentDayOHLV43.ShowHiLo;
                checkanaCurrentDayOHLV43.ShowMidline = showMidline;
                showMidline = checkanaCurrentDayOHLV43.ShowMidline;
                checkanaCurrentDayOHLV43.ShowNoiseLevels = showNoiseLevels;
                showNoiseLevels = checkanaCurrentDayOHLV43.ShowNoiseLevels;
                checkanaCurrentDayOHLV43.ShowTargetLevels = showTargetLevels;
                showTargetLevels = checkanaCurrentDayOHLV43.ShowTargetLevels;
                checkanaCurrentDayOHLV43.TargetType = targetType;
                targetType = checkanaCurrentDayOHLV43.TargetType;

                if (cacheanaCurrentDayOHLV43 != null)
                    for (int idx = 0; idx < cacheanaCurrentDayOHLV43.Length; idx++)
                        if (cacheanaCurrentDayOHLV43[idx].CurrentSession == currentSession && cacheanaCurrentDayOHLV43[idx].FactorDailyRange == factorDailyRange && cacheanaCurrentDayOHLV43[idx].FactorNoiseBands == factorNoiseBands && cacheanaCurrentDayOHLV43[idx].IncludeAfterSession == includeAfterSession && cacheanaCurrentDayOHLV43[idx].RangePeriod1 == rangePeriod1 && cacheanaCurrentDayOHLV43[idx].RangePeriod2 == rangePeriod2 && cacheanaCurrentDayOHLV43[idx].SelectedSession == selectedSession && cacheanaCurrentDayOHLV43[idx].ShowCurrentClose == showCurrentClose && cacheanaCurrentDayOHLV43[idx].ShowCurrentOpen == showCurrentOpen && cacheanaCurrentDayOHLV43[idx].ShowFibonacci == showFibonacci && cacheanaCurrentDayOHLV43[idx].ShowHiLo == showHiLo && cacheanaCurrentDayOHLV43[idx].ShowMidline == showMidline && cacheanaCurrentDayOHLV43[idx].ShowNoiseLevels == showNoiseLevels && cacheanaCurrentDayOHLV43[idx].ShowTargetLevels == showTargetLevels && cacheanaCurrentDayOHLV43[idx].TargetType == targetType && cacheanaCurrentDayOHLV43[idx].EqualsInput(input))
                            return cacheanaCurrentDayOHLV43[idx];

                anaCurrentDayOHLV43 indicator = new anaCurrentDayOHLV43();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CurrentSession = currentSession;
                indicator.FactorDailyRange = factorDailyRange;
                indicator.FactorNoiseBands = factorNoiseBands;
                indicator.IncludeAfterSession = includeAfterSession;
                indicator.RangePeriod1 = rangePeriod1;
                indicator.RangePeriod2 = rangePeriod2;
                indicator.SelectedSession = selectedSession;
                indicator.ShowCurrentClose = showCurrentClose;
                indicator.ShowCurrentOpen = showCurrentOpen;
                indicator.ShowFibonacci = showFibonacci;
                indicator.ShowHiLo = showHiLo;
                indicator.ShowMidline = showMidline;
                indicator.ShowNoiseLevels = showNoiseLevels;
                indicator.ShowTargetLevels = showTargetLevels;
                indicator.TargetType = targetType;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaCurrentDayOHLV43[] tmp = new anaCurrentDayOHLV43[cacheanaCurrentDayOHLV43 == null ? 1 : cacheanaCurrentDayOHLV43.Length + 1];
                if (cacheanaCurrentDayOHLV43 != null)
                    cacheanaCurrentDayOHLV43.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaCurrentDayOHLV43 = tmp;
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
        /// anaCurrentDayOHLV43.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaCurrentDayOHLV43 anaCurrentDayOHLV43(anaSessionTypeCD43 currentSession, int factorDailyRange, int factorNoiseBands, bool includeAfterSession, int rangePeriod1, int rangePeriod2, anaSessionCountCD43 selectedSession, bool showCurrentClose, bool showCurrentOpen, bool showFibonacci, bool showHiLo, bool showMidline, bool showNoiseLevels, bool showTargetLevels, anaTargetTypeCD43 targetType)
        {
            return _indicator.anaCurrentDayOHLV43(Input, currentSession, factorDailyRange, factorNoiseBands, includeAfterSession, rangePeriod1, rangePeriod2, selectedSession, showCurrentClose, showCurrentOpen, showFibonacci, showHiLo, showMidline, showNoiseLevels, showTargetLevels, targetType);
        }

        /// <summary>
        /// anaCurrentDayOHLV43.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaCurrentDayOHLV43 anaCurrentDayOHLV43(Data.IDataSeries input, anaSessionTypeCD43 currentSession, int factorDailyRange, int factorNoiseBands, bool includeAfterSession, int rangePeriod1, int rangePeriod2, anaSessionCountCD43 selectedSession, bool showCurrentClose, bool showCurrentOpen, bool showFibonacci, bool showHiLo, bool showMidline, bool showNoiseLevels, bool showTargetLevels, anaTargetTypeCD43 targetType)
        {
            return _indicator.anaCurrentDayOHLV43(input, currentSession, factorDailyRange, factorNoiseBands, includeAfterSession, rangePeriod1, rangePeriod2, selectedSession, showCurrentClose, showCurrentOpen, showFibonacci, showHiLo, showMidline, showNoiseLevels, showTargetLevels, targetType);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// anaCurrentDayOHLV43.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaCurrentDayOHLV43 anaCurrentDayOHLV43(anaSessionTypeCD43 currentSession, int factorDailyRange, int factorNoiseBands, bool includeAfterSession, int rangePeriod1, int rangePeriod2, anaSessionCountCD43 selectedSession, bool showCurrentClose, bool showCurrentOpen, bool showFibonacci, bool showHiLo, bool showMidline, bool showNoiseLevels, bool showTargetLevels, anaTargetTypeCD43 targetType)
        {
            return _indicator.anaCurrentDayOHLV43(Input, currentSession, factorDailyRange, factorNoiseBands, includeAfterSession, rangePeriod1, rangePeriod2, selectedSession, showCurrentClose, showCurrentOpen, showFibonacci, showHiLo, showMidline, showNoiseLevels, showTargetLevels, targetType);
        }

        /// <summary>
        /// anaCurrentDayOHLV43.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaCurrentDayOHLV43 anaCurrentDayOHLV43(Data.IDataSeries input, anaSessionTypeCD43 currentSession, int factorDailyRange, int factorNoiseBands, bool includeAfterSession, int rangePeriod1, int rangePeriod2, anaSessionCountCD43 selectedSession, bool showCurrentClose, bool showCurrentOpen, bool showFibonacci, bool showHiLo, bool showMidline, bool showNoiseLevels, bool showTargetLevels, anaTargetTypeCD43 targetType)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaCurrentDayOHLV43(input, currentSession, factorDailyRange, factorNoiseBands, includeAfterSession, rangePeriod1, rangePeriod2, selectedSession, showCurrentClose, showCurrentOpen, showFibonacci, showHiLo, showMidline, showNoiseLevels, showTargetLevels, targetType);
        }
    }
}
#endregion
