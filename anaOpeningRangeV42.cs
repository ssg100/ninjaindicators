
#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Collections;
using System.Linq;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

#region Global Enums

public enum anaPlotAlignOR42 {Left, Right, DoNotPlot}
public enum anaSessionCountOR42 {First, Second, Third, Auto}
public enum anaPreSessionOR42 {AsianSession, EuropeanSession, Full}

#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// This indicator displays opening range and pre-session range for the selected session.
	/// </summary>
	[Description("This indicator displays opening range and pre-session range for the selected session.")]
	public class anaOpeningRangeV42 : Indicator
	{
		#region Variables
		private	SolidBrush[]			brushes						= { new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black), 
																	new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black)}; 
		private DateTime				plotStart					= Cbi.Globals.MaxDate;
		private DateTime				cacheSessionBeginTmp   		= Cbi.Globals.MinDate;
		private DateTime				cacheSessionEndTmp			= Cbi.Globals.MinDate;
		private DateTime				cacheSessionDate			= Cbi.Globals.MinDate;
		private DateTime				sessionDateTmp				= Cbi.Globals.MinDate;
		private DateTime				currentDate					= Cbi.Globals.MinDate;
		private DateTime				lastBarTimeStamp1			= Cbi.Globals.MinDate;
		private DateTime				sessionStartTimeLocal		= Cbi.Globals.MinDate;
		private DateTime				sessionEndTimeLocal			= Cbi.Globals.MinDate;
		private DateTime				sessionStartTime			= Cbi.Globals.MinDate;
		private DateTime				openingRangeStartTimeLocal	= Cbi.Globals.MinDate;
		private DateTime				openingRangeStartTime		= Cbi.Globals.MinDate;
		private DateTime  				instrumentSessionDate		= Cbi.Globals.MinDate;
		private DateTime				asianStartTimeLocal			= Cbi.Globals.MinDate;
		private DateTime 				asianEndTimeLocal			= Cbi.Globals.MinDate;
		private DateTime				europeanStartTimeLocal		= Cbi.Globals.MinDate;
		private DateTime 				europeanEndTimeLocal		= Cbi.Globals.MinDate;
		private DateTime				priorTickTime				= Cbi.Globals.MinDate;
		private TimeSpan				openingPeriod				= new TimeSpan(0,30,0);
		private TimeSpan				asianStartTime				= new TimeSpan(8,0,0);
		private TimeSpan 				asianEndTime				= new TimeSpan(15,0,0);
		private TimeSpan				europeanStartTime			= new TimeSpan(8,0,0);
		private TimeSpan 				europeanEndTime				= new TimeSpan(13,0,0);
		private TimeSpan				tradingDayStartTime			= new TimeSpan(0,0,0);
		private TimeSpan				selectedStartTime			= new TimeSpan(0,0,0);
		private TimeSpan				openingPeriodOffset			= new TimeSpan(0,0,0);
		private double					currentOpen					= 0.0;
		private	double					openingRangeHigh			= double.MinValue;
		private	double					openingRangeLow				= double.MaxValue;
		private	double					openingRangeMidline			= double.MaxValue;
		private	double					preSessionHigh				= double.MinValue;
		private	double					preSessionLow				= double.MaxValue;
		private	double					asianSessionHigh			= double.MinValue;
		private	double					asianSessionLow				= double.MaxValue;
		private	double					europeanSessionHigh			= double.MinValue;
		private	double					europeanSessionLow			= double.MaxValue;
		private	double					nightSessionHigh			= double.MinValue;
		private	double					nightSessionLow				= double.MaxValue;
		private double					displaySize					= 0.0;
		private SolidBrush				errorBrush					= new SolidBrush(Color.Red);
		private Font					labelFont					= new Font("Arial", 8);
		private Font					errorFont					= new Font("Arial", 10);
		private string					errorData1					= "Opening range can only be displayed on intraday charts.";
		private	string					errorData2					= "Insufficient historical data to calculate opening range of current session. Please increase chart look back period.";
        private float					errorTextWidth				= 0;
        private float					errorTextHeight				= 0;
		private bool					tickBuilt					= false;
		private bool					firstSession				= false;
		private bool					plotOHL						= false;
		private bool					isCurrency					= false;
		private bool					nightSessionOpen			= false;
		private bool					asianSessionOpen			= false;
		private bool					europeanSessionOpen			= false;
		private bool					sessionOpen					= false;
		private bool					initOpeningRange			= false;
		private bool					initPreSession				= false;
		private bool					initNightSession			= false;
		private bool					initAsian					= false;
		private bool					initEuropean				= false;
		private bool					asian						= false;
		private bool					european					= false;
		private bool					plotCurrentOpen				= false;
		private bool					plotPreSession				= false;
		private bool					showCurrentOpen				= true;
		private bool					showPreSession				= true;
		private bool					showPriorPeriods			= true;
		private bool					applySessionOffset			= false;
		private bool					extendOpeningRange			= true;
		private bool					runningSession				= false;
		private ArrayList				newSessionBarIdxArr1		= new ArrayList();
		private anaPlotAlignOR42		plotLabels					= anaPlotAlignOR42.Right;
		private anaSessionCountOR42		selectedSession				= anaSessionCountOR42.Auto;
		private anaSessionCountOR42 	activeSession				= anaSessionCountOR42.Second;
		private anaPreSessionOR42		selectedPreSession			= anaPreSessionOR42.Full;	
		private SolidBrush				openingRangeFillBrush		= new SolidBrush(Color.RoyalBlue);
		private	StringFormat			stringFormatFar				= new StringFormat();
		private	StringFormat			stringFormatNear			= new StringFormat();
		private	StringFormat			stringFormatCenter			= new StringFormat();
		private int						shiftPeriod					= 0;
		private int						countDown					= 0;
		private int						numberOfSessions			= 1;
		private int						sessionCount				= 0;
		private int 					labelFontSize				= 8;
		private int						labelOffset					= 15;
		private int						opacity						= 3;
		private Color					openingRangeColor			= Color.CornflowerBlue;
		private Color					openingMidColor				= Color.RoyalBlue;
		private Color					openingRangeFillColor		= Color.RoyalBlue;
		private Color					regOpenColor				= Color.SkyBlue;
		private Color					preSessionColor				= Color.LightSteelBlue;
		private Color					developingColor				= Color.Gray;
		private int 					plot0Width 					= 2;
		private DashStyle 				dash0Style 					= DashStyle.Solid;
		private int 					plot2Width 					= 1;
		private DashStyle 				dash2Style 					= DashStyle.Dash;
		private int 					plot3Width 					= 1;
		private DashStyle 				dash3Style 					= DashStyle.Dot;
		private int 					plot4Width 					= 1;
		private DashStyle 				dash4Style 					= DashStyle.Solid;
		private TimeZoneInfo			cetZone						= TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
		private TimeZoneInfo			tstZone						= TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");	
		
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"OR-High"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"OR-Low"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"OR-Mid"));
			Add(new Plot(new Pen(Color.Gray,2), PlotStyle.Line,"Reg-Open"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"Pre-High"));
			Add(new Plot(new Pen(Color.Gray,1), PlotStyle.Line,"Pre-Low"));
			
			AutoScale						= false;
			Overlay							= true;
			PlotsConfigurable				= false;
			MaximumBarsLookBack 			= MaximumBarsLookBack.Infinite;
			BarsRequired					= 0;
			ZOrder							= 1;
			stringFormatNear.Alignment 		= StringAlignment.Near;
			stringFormatCenter.Alignment 	= StringAlignment.Center;
			stringFormatFar.Alignment		= StringAlignment.Far;
		}
		
		protected override void OnStartUp()
		{
			if (Instrument.MasterInstrument.InstrumentType == Cbi.InstrumentType.Currency || Instrument.MasterInstrument.Name == "DX"|| Instrument.MasterInstrument.Name == "6A"
				|| Instrument.MasterInstrument.Name == "6B" || Instrument.MasterInstrument.Name == "6C" ||Instrument.MasterInstrument.Name == "6E"
				|| Instrument.MasterInstrument.Name == "6J" || Instrument.MasterInstrument.Name == "6M" || Instrument.MasterInstrument.Name == "6S"
				|| Instrument.MasterInstrument.Name == "6N" || Instrument.MasterInstrument.Name == "E7" || Instrument.MasterInstrument.Name == "J7"
				|| Instrument.MasterInstrument.Name == "M6A" || Instrument.MasterInstrument.Name == "M6B" || Instrument.MasterInstrument.Name == "M6C"
				|| Instrument.MasterInstrument.Name == "M6E" || Instrument.MasterInstrument.Name == "M6J" || Instrument.MasterInstrument.Name == "M6S")
				isCurrency = true;
			
			if (selectedSession == anaSessionCountOR42.Auto)
			{
				if (isCurrency)
					activeSession = anaSessionCountOR42.Third;
				else
					activeSession = anaSessionCountOR42.Second;
			}
			else 
				activeSession = selectedSession; 
			
			if (Instrument.MasterInstrument.Name == "FDAX" || Instrument.MasterInstrument.Name == "FESX" ||Instrument.MasterInstrument.Name == "FGBL" ||
				Instrument.MasterInstrument.Name == "FGBM")
			{
				asian	 	= false;
				european 	= false;
				Plots[4].Name = "Pre-High";
				Plots[5].Name = "Pre-Low";
			}	
			else if (selectedPreSession == anaPreSessionOR42.AsianSession)
			{
				asian = true;
				european = false;
				if (asian)
				{
					Plots[4].Name = "AS-High";
					Plots[5].Name = "AS-Low";
				}
			}
			else if (selectedPreSession == anaPreSessionOR42.EuropeanSession)
			{
				asian = false;
				european = true;
				if (european)
				{
					Plots[4].Name = "EU-High";
					Plots[5].Name = "EU-Low";
				}
			}
			else if (selectedPreSession == anaPreSessionOR42.Full)
			{
				asian	 	= false;
				european 	= false;
				Plots[4].Name = "Pre-High";
				Plots[5].Name = "Pre-Low";
			}
			labelFont = new Font ("Arial", labelFontSize);
			if(ChartControl != null)
				errorBrush.Color = ChartControl.AxisColor;
			openingRangeFillBrush = new SolidBrush(Color.FromArgb(25*opacity, openingRangeFillColor));
			Plots[0].Pen.Width = plot0Width;
			Plots[0].Pen.DashStyle = dash0Style;
			Plots[1].Pen.Width= plot0Width;
			Plots[1].Pen.DashStyle = dash0Style;
			Plots[2].Pen.Width = plot2Width;
			Plots[2].Pen.DashStyle = dash2Style;
			Plots[3].Pen.Width = plot3Width;
			Plots[3].Pen.DashStyle = dash3Style;
			Plots[4].Pen.Width= plot4Width;
			Plots[4].Pen.DashStyle = dash4Style;
			Plots[5].Pen.Width = plot4Width;
			Plots[5].Pen.DashStyle = dash4Style;
			Plots[0].Pen.Color = openingRangeColor;
			Plots[1].Pen.Color = openingRangeColor;
			Plots[2].Pen.Color = openingMidColor;
			Plots[3].Pen.Color = regOpenColor;
			Plots[4].Pen.Color = preSessionColor;
			Plots[5].Pen.Color = preSessionColor;
			
			if (!showCurrentOpen || regOpenColor == Color.Transparent)
				plotCurrentOpen = false;
			else
				plotCurrentOpen = true;
			if(!showPreSession || preSessionColor == Color.Transparent)
				plotPreSession = false;
			else
				plotPreSession = true;
			if (AutoScale)
				AutoScale = false;
			if (Instrument.MasterInstrument.InstrumentType == Cbi.InstrumentType.Currency && (TickSize == 0.00001 || TickSize == 0.001))
				displaySize = 5* TickSize;
			else
				displaySize = TickSize;
			shiftPeriod = TimeSpan.Compare(openingPeriodOffset, new TimeSpan(0,0,0));
			if(shiftPeriod < 0 && activeSession == anaSessionCountOR42.First)
			{
				openingPeriodOffset = new TimeSpan(0,0,0);
				shiftPeriod = 0;
			}	
			if (BarsPeriod.Id == PeriodType.Minute || BarsPeriod.Id == PeriodType.Second)
				tickBuilt = false;
			else
				tickBuilt = true;
			countDown = 1;
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
				currentDate = GetLastBarSessionDate(Time[0], Bars, 0);
				sessionCount = 1;
				return;
			}
			
			if (Bars.FirstBarOfSession && FirstTickOfBar)
			{
				BarsArray[0].Session.GetNextBeginEnd(Bars, 0, out sessionStartTimeLocal, out sessionEndTimeLocal);
				sessionStartTime = TimeZoneInfo.ConvertTime(sessionStartTimeLocal, TimeZoneInfo.Local, Bars.Session.TimeZoneInfo);
				instrumentSessionDate = TimeZoneInfo.ConvertTime(sessionEndTimeLocal,TimeZoneInfo.Local, Bars.Session.TimeZoneInfo).Date;
				if (european)
				{
					europeanStartTimeLocal = TimeZoneInfo.ConvertTime(instrumentSessionDate + EuropeanStartTime, cetZone, TimeZoneInfo.Local);
					europeanEndTimeLocal = TimeZoneInfo.ConvertTime(instrumentSessionDate + EuropeanEndTime, cetZone, TimeZoneInfo.Local);
				}
				if (asian)
				{
					asianStartTimeLocal = TimeZoneInfo.ConvertTime(instrumentSessionDate + AsianStartTime, tstZone, TimeZoneInfo.Local);
					asianEndTimeLocal = TimeZoneInfo.ConvertTime(instrumentSessionDate + AsianEndTime, tstZone, TimeZoneInfo.Local);
				}
			}
			
			lastBarTimeStamp1 = GetLastBarSessionDate(Time[0], Bars, 0);
			if (lastBarTimeStamp1 != currentDate)
			{	
				if (countDown == 1)
				{
					plotStart = sessionStartTimeLocal;
					countDown = 0;
				}
				else
					countDown = -1;
				sessionCount = 1;
				runningSession = false;
				if (Time[0] >= plotStart.AddDays(1) && numberOfSessions == 1)
					firstSession = true;
				else	
					firstSession = false;
				if ((activeSession == anaSessionCountOR42.First || firstSession) && shiftPeriod > 0)
				{
					applySessionOffset = true;
					openingRangeStartTimeLocal = sessionStartTimeLocal.Add(openingPeriodOffset);
					openingRangeStartTime = (TimeZoneInfo.ConvertTime(sessionStartTimeLocal, TimeZoneInfo.Local, Bars.Session.TimeZoneInfo)).Add(openingPeriodOffset);
				}
				else
					applySessionOffset = false;
				if (lastBarTimeStamp1.Date.DayOfWeek != DayOfWeek.Monday || countDown < 0) 
					tradingDayStartTime = new TimeSpan(sessionStartTime.Hour, sessionStartTime.Minute,0);
				if (activeSession == anaSessionCountOR42.First || numberOfSessions == 1 && 
					(lastBarTimeStamp1.Date.DayOfWeek != DayOfWeek.Monday || countDown < 0))
				{
					if(applySessionOffset)
						selectedStartTime = new TimeSpan(openingRangeStartTime.Hour, openingRangeStartTime.Minute, 0);
					else
						selectedStartTime = new TimeSpan(sessionStartTime.Hour, sessionStartTime.Minute, 0);
				}
				if ((activeSession == anaSessionCountOR42.First || firstSession) && shiftPeriod == 0)
				{
					nightSessionOpen 	= false;
					initNightSession	= false;
					asianSessionOpen	= false;
					initAsian			= false;
					europeanSessionOpen = false;
					initEuropean		= false;
					currentOpen			= Open[0];
					openingRangeHigh	= High[0];
					openingRangeLow		= Low[0];
					sessionOpen 		= true;
					initOpeningRange 	= true;
				}
				else
				{
					nightSessionHigh 	= High[0];
					nightSessionLow 	= Low[0];
					nightSessionOpen 	= true;
					initNightSession	= true;
					if((tickBuilt && Time[0] >= asianStartTimeLocal) || (!tickBuilt && Time[0] > asianStartTimeLocal))
					{
						asianSessionHigh 	= High[0];
						asianSessionLow 	= Low[0];
						asianSessionOpen	= true;
						initAsian 			= true;
					}
					else
					{
						asianSessionOpen 	= false;
						initAsian 			= false;
					}
					if((tickBuilt && Time[0] >= europeanStartTimeLocal) || (!tickBuilt && Time[0] > europeanStartTimeLocal))
					{
						europeanSessionHigh = High[0];
						europeanSessionLow 	= Low[0];
						europeanSessionOpen	= true;
						initEuropean 		= true;
					}
					else
					{
						europeanSessionOpen = false;
						initEuropean 		= false;
					}
					sessionOpen 		= false;
					initOpeningRange 	= false;
					if (activeSession == anaSessionCountOR42.Second && shiftPeriod != 0)
					{
						applySessionOffset = true;
						openingRangeStartTimeLocal = sessionEndTimeLocal.Add(openingPeriodOffset);
						openingRangeStartTime = (TimeZoneInfo.ConvertTime(sessionEndTimeLocal, TimeZoneInfo.Local, Bars.Session.TimeZoneInfo)).Add(openingPeriodOffset);
					}
				}
				currentDate = lastBarTimeStamp1;
				plotOHL = true;
			}
			else if ((!applySessionOffset && FirstTickOfBar && Bars.FirstBarOfSession && !runningSession) || (applySessionOffset && tickBuilt && Time[0] >= openingRangeStartTimeLocal && priorTickTime < openingRangeStartTimeLocal)
				|| (applySessionOffset && !tickBuilt && Time[0] > openingRangeStartTimeLocal && Time[1] <= openingRangeStartTimeLocal))
			{
				sessionCount = sessionCount + 1;
				numberOfSessions = Math.Min(3, Math.Max(sessionCount, numberOfSessions));
				runningSession = false;
				if (( activeSession == anaSessionCountOR42.Second && sessionCount == 2 || activeSession == anaSessionCountOR42.Third && sessionCount == 3) 
					&& (lastBarTimeStamp1.Date.DayOfWeek != DayOfWeek.Monday || countDown < 0))
				{
					if(applySessionOffset)
						selectedStartTime = new TimeSpan(openingRangeStartTime.Hour, openingRangeStartTime.Minute,0);
					else
						selectedStartTime = new TimeSpan(sessionStartTime.Hour, sessionStartTime.Minute,0);
				}
				applySessionOffset = false;
				if (sessionCount == 2)
				{
					if (activeSession == anaSessionCountOR42.First && shiftPeriod < 1)
					{	
						sessionOpen			= false;
						if(!extendOpeningRange)
							initOpeningRange = false;
					}
					else if ((activeSession == anaSessionCountOR42.First && shiftPeriod == 1) || activeSession == anaSessionCountOR42.Second)
					{
						nightSessionOpen	= false;
						europeanSessionOpen = false;
						asianSessionOpen	= false;
						currentOpen			= Open[0];
						openingRangeHigh	= High[0];
						openingRangeLow		= Low[0];
						sessionOpen			= true;
						initOpeningRange	= true;
						if(shiftPeriod < 0)
							runningSession	= true;
					}
					else
					{
						nightSessionOpen	= true;
						initNightSession	= true;
						nightSessionHigh	= Math.Max(nightSessionHigh, High[0]);
						nightSessionLow		= Math.Min(nightSessionLow, Low[0]);
						if (asian && !asianSessionOpen && ((tickBuilt && Time[0] >= asianStartTimeLocal && Time[0] < asianEndTimeLocal) || (!tickBuilt && Time[0] > asianStartTimeLocal && Time[0]<= asianEndTimeLocal)))
						{
							asianSessionHigh 	= High[0];
							asianSessionLow 	= Low[0];
							asianSessionOpen	= true;
							initAsian			= true;
						}	
						else if (asianSessionOpen && Time[0]<= asianEndTimeLocal)
						{
							asianSessionHigh	= Math.Max(asianSessionHigh, High[0]);
							asianSessionLow		= Math.Min(asianSessionLow, Low[0]);
						}
						else
							asianSessionOpen = false;
						if (european && !europeanSessionOpen && ((tickBuilt && Time[0] >= europeanStartTimeLocal && Time[0] < europeanEndTimeLocal) || (!tickBuilt && Time[0] > europeanStartTimeLocal && Time[0]<= europeanEndTimeLocal)))
						{
							europeanSessionHigh = High[0];
							europeanSessionLow 	= Low[0];
							europeanSessionOpen	= true;
							initEuropean		= true;
						}	
						else if (europeanSessionOpen && Time[0]<= europeanEndTimeLocal)
						{
							europeanSessionHigh	= Math.Max(europeanSessionHigh, High[0]);
							europeanSessionLow	= Math.Min(europeanSessionLow, Low[0]);
						}
						else
							europeanSessionOpen = false;
						sessionOpen			 = false;
						if(!extendOpeningRange)
						{
							initOpeningRange = false;
							initNightSession = false;	
						}
					}
					if (activeSession == anaSessionCountOR42.Third && shiftPeriod != 0)
					{
						applySessionOffset = true;
						openingRangeStartTimeLocal = sessionEndTimeLocal.Add(openingPeriodOffset);
						openingRangeStartTime = (TimeZoneInfo.ConvertTime(sessionEndTimeLocal, TimeZoneInfo.Local, Bars.Session.TimeZoneInfo)).Add(openingPeriodOffset);
					}
				}
				else if (sessionCount == 3)
				{	
					nightSessionOpen 	= false;
					asianSessionOpen	= false;
					europeanSessionOpen = false;
					if (activeSession != anaSessionCountOR42.Third)
					{
						sessionOpen	= false;
						if(!extendOpeningRange)
						{
							initOpeningRange = false;
							initNightSession = false;
						}
					}
					else 
					{
						currentOpen			= Open[0];
						openingRangeHigh	= High[0];
						openingRangeLow		= Low[0];
						sessionOpen			= true;
						initOpeningRange	= true;
						if(shiftPeriod < 0)
							runningSession  = true;
					}
				}
				else
				{
					sessionOpen	= false;
					if(!extendOpeningRange)
					{
						initOpeningRange = false;
						initNightSession = false;
					}
				}
			}
			else if (nightSessionOpen)
			{
				nightSessionHigh	= Math.Max(nightSessionHigh, High[0]);
				nightSessionLow		= Math.Min(nightSessionLow, Low[0]);
				if (asian && !asianSessionOpen && ((tickBuilt && Time[0] >= asianStartTimeLocal && Time[0] < asianEndTimeLocal) || (!tickBuilt && Time[0] > asianStartTimeLocal && Time[0]<= asianEndTimeLocal)))
				{
					asianSessionHigh 	= High[0];
					asianSessionLow 	= Low[0];
					asianSessionOpen	= true;
					initAsian			= true;
				}	
				else if (asianSessionOpen && Time[0]<= asianEndTimeLocal)
				{
					asianSessionHigh	= Math.Max(asianSessionHigh, High[0]);
					asianSessionLow		= Math.Min(asianSessionLow, Low[0]);
				}
				else
					asianSessionOpen = false;
						if (european && !europeanSessionOpen && ((tickBuilt && Time[0] >= europeanStartTimeLocal && Time[0] < europeanEndTimeLocal) || (!tickBuilt && Time[0] > europeanStartTimeLocal && Time[0]<= europeanEndTimeLocal)))
				{
					europeanSessionHigh = High[0];
					europeanSessionLow 	= Low[0];
					europeanSessionOpen	= true;
					initEuropean 		= true;
				}	
				else if (europeanSessionOpen && Time[0]<= europeanEndTimeLocal)
				{
					europeanSessionHigh	= Math.Max(europeanSessionHigh, High[0]);
					europeanSessionLow	= Math.Min(europeanSessionLow, Low[0]);
				}
				else
					europeanSessionOpen = false;
			}
			else if (((shiftPeriod == 0) && sessionOpen && ((tickBuilt && Time[0] < sessionStartTimeLocal.Add(openingPeriod)) || (!tickBuilt && Time[0] <= sessionStartTimeLocal.Add(openingPeriod))))
				|| ((shiftPeriod != 0) && sessionOpen && ((tickBuilt && Time[0] < openingRangeStartTimeLocal.Add(openingPeriod)) || (!tickBuilt && Time[0] <= openingRangeStartTimeLocal.Add(openingPeriod)))))
			{
				openingRangeHigh	= Math.Max(openingRangeHigh, High[0]);
				openingRangeLow		= Math.Min(openingRangeLow, Low[0]);
			}
			else
				runningSession = false;
			priorTickTime = Time[0];
			initPreSession = false;
			if (plotOHL && initNightSession && asian && initAsian)
			{
				preSessionHigh = asianSessionHigh;
				preSessionLow = asianSessionLow;
				initPreSession = true;
			}
			else if (plotOHL && initNightSession && european && initEuropean)
			{
				preSessionHigh = europeanSessionHigh;
				preSessionLow = europeanSessionLow;
				initPreSession = true;
			}
			else if (plotOHL && initNightSession && !asian && !european)
			{
				preSessionHigh = nightSessionHigh;
				preSessionLow = nightSessionLow;
				initPreSession = true;
			}
			if(isCurrency)
			{
				preSessionHigh = Math.Round(preSessionHigh/displaySize)*displaySize;
				preSessionLow = Math.Round(preSessionLow/displaySize)*displaySize;
				openingRangeHigh = Math.Round(openingRangeHigh/displaySize)*displaySize;
				openingRangeLow = Math.Round(openingRangeLow/displaySize)*displaySize;
			}	
			if(plotPreSession && initPreSession)
			{
				PreSessionHigh.Set(preSessionHigh);
				PreSessionLow.Set(preSessionLow);
			}
			else
			{
				PreSessionHigh.Reset();
				PreSessionLow.Reset();
			}
				
			if (plotOHL && initOpeningRange)
			{
				if(plotCurrentOpen)
					CurrentOpen.Set(currentOpen);
				else
					CurrentOpen.Reset();
				OpeningRangeHigh.Set(openingRangeHigh);
				OpeningRangeLow.Set(openingRangeLow);
				openingRangeMidline	= Math.Round((openingRangeHigh + openingRangeLow)/(2*displaySize))*displaySize;
				OpeningRangeMidline.Set(openingRangeMidline);
			}
			else
			{
				CurrentOpen.Reset();
				OpeningRangeHigh.Reset();
   			  	OpeningRangeLow.Reset();
				OpeningRangeMidline.Reset();
			}
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Session # for which the opening range is calculated")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Select RTH session")]
		public anaSessionCountOR42 SelectedSession
		{
			get { return selectedSession; }
			set { selectedSession = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Session for which the pre-session high and low are calculated")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Pre-session")]
		public anaPreSessionOR42 SelectedPreSession
		{
			get { return selectedPreSession; }
			set { selectedPreSession = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Option to extend the opening range until the end of the trading day")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Extend OR plots until close")]
		public bool ExtendOpeningRange
		{
			get { return extendOpeningRange; }
			set { extendOpeningRange = value; }
		}
		
		///<summary
		///</summary>
		[Browsable(false)]
		[XmlIgnore]
		public TimeSpan OpeningPeriod
		{
			get { return openingPeriod;}
			set { openingPeriod = value;}
		}	
	
		///<summary
		///</summary>
		[Description("Enter opening period in hours and minutes")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Opening period (h:min)")]
		public string S_OpeningPeriod	
		{
			get 
			{ 
				return string.Format("{0:D2}:{1:D2}", openingPeriod.Hours, openingPeriod.Minutes);
			}
			set 
			{ 
				string[]values =((string)value).Split(':');
				openingPeriod = new TimeSpan(Convert.ToInt16(values[0]),Convert.ToInt16(values[1]),0);
			}
		}
		
		///<summary
		///</summary>
		[Browsable(false)]
		[XmlIgnore]
		public TimeSpan OpeningPeriodOffset
		{
			get { return openingPeriodOffset;}
			set { openingPeriodOffset = value;}
		}	
	
		///<summary
		///</summary>
		[Description("Enter offset for start of opening period relative to session start")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Offset (+ h:min)")]
		public string S_OpeningPeriodOffset	
		{
			get 
			{ 	
				if(TimeSpan.Compare(openingPeriodOffset, new TimeSpan(0,0,0)) > -1)
					return "+ " + string.Format("{0:D2}:{1:D2}", Math.Abs(openingPeriodOffset.Hours), Math.Abs(openingPeriodOffset.Minutes));
				else 
					return "- " + string.Format("{0:D2}:{1:D2}", Math.Abs(openingPeriodOffset.Hours), Math.Abs(openingPeriodOffset.Minutes));
			}
			set 
			{ 
				char[] delimiters = new char[] {' ',':'};
				string[]values =((string)value).Split(delimiters, StringSplitOptions.None);
				if(values[0] == "+")
					openingPeriodOffset = new TimeSpan(Convert.ToInt16(values[1]),Convert.ToInt16(values[2]),0);
				else if (values[0] == "-")
					openingPeriodOffset = new TimeSpan(-Convert.ToInt16(values[1]),-Convert.ToInt16(values[2]),0);
				else
					openingPeriodOffset = new TimeSpan(0,0,0);
			}
		}

		///<summary
		///</summary>
		[Description("Time zone for Asian session")]
		[GridCategory("User defined values")]
		[Gui.Design.DisplayNameAttribute("Time zone (Asia)")]
		[TypeConverter(typeof(TZListConverter))]
		public string TSTZone
		{
			get
			{
				return tstZone.StandardName;
			}
			set
			{
				tstZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault<TimeZoneInfo>(x=>((x.DisplayName==value)||(x.StandardName==value)));
			}
		}		
		
		///<summary
		///</summary>
		[Browsable(false)]
		[XmlIgnore]
		public TimeSpan AsianStartTime
		{
			get { return asianStartTime;}
			set { asianStartTime = value;}
		}	
	
		///<summary
		///</summary>
		[Description("Session start time for Asian session")]
		[GridCategory("User defined values")]
		[Gui.Design.DisplayNameAttribute("Asian session begin")]
		public string S_AsianStartTime
		{
			get 
			{ 
				return string.Format("{0:D2}:{1:D2}", asianStartTime.Hours, asianStartTime.Minutes);
			}
			set 
			{ 
				string[]values =((string)value).Split(':');
				asianStartTime = new TimeSpan(Convert.ToInt16(values[0]),Convert.ToInt16(values[1]),0);
			}
		}
		
		///<summary
		///</summary>
		[Browsable(false)]
		[XmlIgnore]
		public TimeSpan AsianEndTime
		{
			get { return asianEndTime;}
			set { asianEndTime = value;}
		}	
	
		///<summary
		///</summary>
		[Description("Session end time for asian session")]
		[GridCategory("User defined values")]
		[Gui.Design.DisplayNameAttribute("Asian session end")]
		public string S_AsianEndTime
		{
			get 
			{ 
				return string.Format("{0:D2}:{1:D2}", asianEndTime.Hours, asianEndTime.Minutes);
			}
			set 
			{ 
				string[]values =((string)value).Split(':');
				asianEndTime = new TimeSpan(Convert.ToInt16(values[0]),Convert.ToInt16(values[1]),0);
			}
		}		

		///<summary
		///</summary>
		[Description("Time zone for European session")]
		[GridCategory("User defined values")]
		[Gui.Design.DisplayNameAttribute("Time zone (Europe)")]
		[TypeConverter(typeof(TZListConverter))]
		public string CETZone
		{
			get
			{
				return cetZone.StandardName;
			}
			set
			{
				cetZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault<TimeZoneInfo>(x=>((x.DisplayName==value)||(x.StandardName==value)));
			}
		}		
		
		///<summary
		///</summary>
		[Browsable(false)]
		[XmlIgnore]
		public TimeSpan EuropeanStartTime
		{
			get { return europeanStartTime;}
			set { europeanStartTime = value;}
		}	
	
		///<summary
		///</summary>
		[Description("Session start time for European session")]
		[GridCategory("User defined values")]
		[Gui.Design.DisplayNameAttribute("European session begin")]
		public string S_EuropeanStartTime
		{
			get 
			{ 
				return string.Format("{0:D2}:{1:D2}", europeanStartTime.Hours, europeanStartTime.Minutes);
			}
			set 
			{ 
				string[]values =((string)value).Split(':');
				europeanStartTime = new TimeSpan(Convert.ToInt16(values[0]),Convert.ToInt16(values[1]),0);
			}
		}
		
		///<summary
		///</summary>
		[Browsable(false)]
		[XmlIgnore]
		public TimeSpan EuropeanEndTime
		{
			get { return europeanEndTime;}
			set { europeanEndTime = value;}
		}	
	
		///<summary
		///</summary>
		[Description("Session end time for European session")]
		[GridCategory("User defined values")]
		[Gui.Design.DisplayNameAttribute("Europe session end")]
		public string S_EuropeanEndTime
		{
			get 
			{ 
				return string.Format("{0:D2}:{1:D2}", europeanEndTime.Hours, europeanEndTime.Minutes);
			}
			set 
			{ 
				string[]values =((string)value).Split(':');
				europeanEndTime = new TimeSpan(Convert.ToInt16(values[0]),Convert.ToInt16(values[1]),0);
			}
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries OpeningRangeHigh
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries OpeningRangeLow
		{
			get { return Values[1]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries OpeningRangeMidline
		{
			get { return Values[2]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CurrentOpen
		{
			get { return Values[3]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries PreSessionHigh
		{
			get { return Values[4]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries PreSessionLow
		{
			get { return Values[5]; }
		}
			
		/// <summary>
		/// </summary>
		[Description("Option to show current open")]
		[Category("Display Options")]
		[Gui.Design.DisplayNameAttribute("Show current open")]
		public bool ShowCurrentOpen 
		{
			get { return showCurrentOpen; }
			set { showCurrentOpen = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Option to show the range of the pre-session")]
		[Category("Display Options")]
		[Gui.Design.DisplayNameAttribute("Show pre-session range")]
		public bool ShowPreSession 
		{
			get { return showPreSession; }
			set { showPreSession = value; }
		}
		
		[Description("Option to show plots for prior days")]
		[Category("Display Options")]
		[Gui.Design.DisplayNameAttribute("Show prior days")]
		public bool ShowPriorPeriods 
		{
			get { return showPriorPeriods; }
			set { showPriorPeriods = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Option where to plot labels")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Label position")]
		public anaPlotAlignOR42 PlotLabels
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
		[Description("Opacity of opening range ")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Opacity opening range")]
		public int Opacity
		{
			get { return opacity; }
			set { opacity = Math.Min(10, Math.Max(0, value)); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Select color for opening range")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Opening range")]
		public Color OpeningRangeColor
		{
			get { return openingRangeColor; }
			set { openingRangeColor = value; }
		}

		// Serialize Color object
		[Browsable(false)]
		public string OpeningRangeColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(openingRangeColor); }
			set { openingRangeColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
 
		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Select color for midline of opening range")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Opening range midline")]
		public Color OpeningMidColor
		{
			get { return openingMidColor; }
			set { openingMidColor = value; }
		}

		// Serialize Color object
		[Browsable(false)]
		public string OpeningMidColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(openingMidColor); }
			set { openingMidColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
 
		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Select fill color for opening range")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Opening range fill")]
		public Color OpeningRangeFillColor
		{
			get { return openingRangeFillColor; }
			set { openingRangeFillColor = value; }
		}

		// Serialize Color object
		[Browsable(false)]
		public string OpeningRangeFillColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(openingRangeFillColor); }
			set { openingRangeFillColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
 
		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Select color for regular open")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Regular open")]
		public Color RegOpenColor
		{
			get { return regOpenColor; }
			set { regOpenColor = value; }
		}

		// Serialize Color object
		[Browsable(false)]
		public string RegOpenColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(regOpenColor); }
			set { regOpenColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
 
		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for pre-session high and low")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Pre-session ")]
		public Color PreSessionColor
		{
			get { return preSessionColor; }
			set { preSessionColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string PreSessionColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(preSessionColor); }
			set { preSessionColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Select color for developing opening range")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Developing range")]
		public Color DevelopingColor
		{
			get { return developingColor; }
			set { developingColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DevelopingColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(developingColor); }
			set { developingColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Line width for opening range.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width opening range")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for opening range.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style opening range")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		} 
		
		/// <summary>
		/// </summary>
		[Description("Line width for midrange.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width midrange")]
		public int Plot2Width
		{
			get { return plot2Width; }
			set { plot2Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for midrange.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style midrange")]
		public DashStyle Dash2Style
		{
			get { return dash2Style; }
			set { dash2Style = value; }
		} 		

		/// <summary>
		/// </summary>
		[Description("Line width for RTH open.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width for RTH open")]
		public int Plot3Width
		{
			get { return plot3Width; }
			set { plot3Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for RTH open.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style RTH open")]
		public DashStyle Dash3Style
		{
			get { return dash3Style; }
			set { dash3Style = value; }
		} 
		
		/// <summary>
		/// </summary>
		[Description("Line Width for pre-session range.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width pre-session range")]
		public int Plot4Width
		{
			get { return plot4Width; }
			set { plot4Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for pre-session range.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style pre-session range")]
		public DashStyle Dash4Style
		{
			get { return dash4Style; }
			set { dash4Style = value; }
		} 		
		#endregion

		
		#region Miscellaneous

		public class TZListConverter : TypeConverter
		{
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				return true;
			}
			
			public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				return new StandardValuesCollection((TimeZoneInfo.GetSystemTimeZones()).Select(x=>x.DisplayName).ToList());
			}
		}		
		
		public override string FormatPriceMarker(double price)
		{
			double truncatedPart = Math.Truncate(price);
			int fractionalPart = Convert.ToInt32(320 * Math.Abs(price - truncatedPart) - 0.0001); 
			string fraction = "";
			string priceMarker = "";
			if (TickSize == 0.03125) 
			{
				fractionalPart = fractionalPart / 10;
				fraction = fractionalPart.ToString();
				if (fractionalPart < 10)
					fraction = "'0" + fraction;
				else 
					fraction = "'" + fraction;
				priceMarker = truncatedPart.ToString() + fraction;
			}
			else if (TickSize == 0.015625 || TickSize == 0.0078125)
			{
				fraction = fractionalPart.ToString();
				if (fractionalPart < 10)
					fraction = "'00" + fraction;
				if (fractionalPart < 100)
					fraction = "'0" + fraction;
				else	
					fraction = "'" + fraction;
				priceMarker = truncatedPart.ToString() + fraction;
			}
			else
				priceMarker = price.ToString(Gui.Globals.GetTickFormatString(TickSize));
			return priceMarker;
		}		
		
		private DateTime GetLastBarSessionDate(DateTime time, Data.Bars bars, int barsAgo)
		{
			if (time > cacheSessionEndTmp) 
			{
				sessionDateTmp = Bars.GetTradingDayFromLocal(time);
				if(cacheSessionDate != sessionDateTmp) 
				{
					cacheSessionDate = sessionDateTmp;
					if (newSessionBarIdxArr1.Count == 0 
						|| (newSessionBarIdxArr1.Count > 0 && CurrentBar > (int) newSessionBarIdxArr1[newSessionBarIdxArr1.Count - 1]))
							newSessionBarIdxArr1.Add(CurrentBar);
				}
				Bars.Session.GetNextBeginEnd(bars, barsAgo, out cacheSessionBeginTmp, out cacheSessionEndTmp); 
				if(tickBuilt)
					cacheSessionEndTmp = cacheSessionEndTmp.AddSeconds(-1);
			}
			return sessionDateTmp;
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
			errorBrush.Dispose();
			openingRangeFillBrush.Dispose();
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
			
			// plot error if data not complete
			DateTime lastBarSessionBegin = Cbi.Globals.MinDate;
			DateTime lastBarSessionEnd = Cbi.Globals.MinDate;
			Bars.Session.GetNextBeginEnd(Bars.Get(this.LastBarIndexPainted).Time, out lastBarSessionBegin, out lastBarSessionEnd);
			SizeF errorSize = graphics.MeasureString(errorData2, errorFont);
			errorTextHeight	= errorSize.Height + 5;
			if (countDown == 1 || plotStart >= lastBarSessionEnd)
				graphics.DrawString(errorData2, errorFont, errorBrush, bounds.X + bounds.Width/2, bounds.Y + bounds.Height/2, stringFormatCenter);			
			
			int lastBarIndex			= this.LastBarIndexPainted;
			int firstBarIndex			= Math.Max(BarsRequired, this.FirstBarIndexPainted - 1);
			DateTime prePeriodStart		= Cbi.Globals.MinDate;
			DateTime prePeriodEnd 		= Cbi.Globals.MinDate;
			DateTime refPeriodStart		= Cbi.Globals.MinDate;
			DateTime refPeriodEnd 		= Cbi.Globals.MinDate;
			
			bool firstLoop 			= true;
			do
			{	
				DateTime lastBarTime		= Bars.Get(lastBarIndex).Time;
				DateTime plotSessionDate 	= Bars.GetTradingDayFromLocal(lastBarTime);
				while (lastBarIndex > firstBarIndex && (!Values[0].IsValidPlot(lastBarIndex) && !Values[4].IsValidPlot(lastBarIndex)))
				{
					lastBarIndex = lastBarIndex - 1;
					firstLoop = false;
				}
				if (!Values[0].IsValidPlot(lastBarIndex) && !Values[4].IsValidPlot(lastBarIndex))
					return;
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
				string[] labels			= new String [Values.Length];
				Color[]	plotColors		= new Color [Values.Length];
				
				for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
				{
					if(Values[seriesCount].IsValidPlot(lastBarIndex))
						yArr[seriesCount] = ChartControl.GetYByValue(this, Values[seriesCount].Get(lastBarIndex));
					else
						yArr[seriesCount] = 0;
					labels[seriesCount] = Plots[seriesCount].Name;
					plotColors[seriesCount] = Plots[seriesCount].Pen.Color;
				}
				
				if(plotCurrentOpen)
				{
					for (int i = 0; i < 3; i++)
						if(yArr[i] == yArr[3])
						{
							labels[i] = Plots[3].Name + "/" + Plots[i].Name;
							plotColors[3] = Color.Transparent;
						}	
				}	
				if(plotPreSession)
				{
					for (int i = 0; i < 3; i++)
						if(yArr[i] == yArr[4])
						{
							labels[4] = Plots[4].Name + "/" + Plots[i].Name;
							plotColors[i] = Color.Transparent;
						}	
					for (int i = 0; i < 3; i++)
						if(yArr[i] == yArr[5])
						{
							labels[5] = Plots[5].Name + "/" + Plots[i].Name;
							plotColors[i] = Color.Transparent;
						}	
				}	
				
				if(asian)
				{
					prePeriodStart = TimeZoneInfo.ConvertTime(plotSessionDate + EuropeanStartTime, cetZone, TimeZoneInfo.Local);
					prePeriodEnd = TimeZoneInfo.ConvertTime(plotSessionDate + EuropeanEndTime, cetZone, TimeZoneInfo.Local);
				}
				else if (european)
				{
					prePeriodStart = TimeZoneInfo.ConvertTime(plotSessionDate + EuropeanStartTime, cetZone, TimeZoneInfo.Local);
					prePeriodEnd = TimeZoneInfo.ConvertTime(plotSessionDate + EuropeanEndTime, cetZone, TimeZoneInfo.Local);
				}
				else 
				{
					prePeriodStart = TimeZoneInfo.ConvertTime(plotSessionDate + tradingDayStartTime, Bars.Session.TimeZoneInfo, TimeZoneInfo.Local);
					prePeriodEnd = TimeZoneInfo.ConvertTime(plotSessionDate + selectedStartTime, Bars.Session.TimeZoneInfo, TimeZoneInfo.Local);
				}
				if(lastBarTime <= prePeriodStart)
				{
					if (prePeriodEnd > prePeriodStart)
						prePeriodEnd = prePeriodEnd.AddDays(-1);
					prePeriodStart= prePeriodStart.AddDays(-1);
				}
				refPeriodStart = TimeZoneInfo.ConvertTime(plotSessionDate + selectedStartTime, Bars.Session.TimeZoneInfo, TimeZoneInfo.Local);
				refPeriodEnd = refPeriodStart.Add(openingPeriod);
				if(lastBarTime <= refPeriodStart)
				{
					if (refPeriodEnd > refPeriodStart)
						refPeriodEnd = refPeriodEnd.AddDays(-1);
					refPeriodStart= refPeriodStart.AddDays(-1);
				}
				
				if(opacity > 0)
				{
					int lastX = -1;
					for (int idx = lastBarPlotIndex; idx >= firstBarPlotIndex; idx--)
					{	
						if (idx - Displacement >= Bars.Count) 
								continue;
						else if (idx - Displacement < 0)
							break;
						if (idx < CurrentBar && !Values[0].IsValidPlot(idx))
							break;
						lastX = Math.Max(ChartControl.GetXByBarIdx(BarsArray[0], idx-1) + sessionGap, lastXtoFill);
					}
					graphics.FillRectangle(openingRangeFillBrush, lastX, yArr[0], firstXtoFill - lastX, yArr[1] - yArr[0]);
				}
				
				int maxCount = Values.Length;
				for (int seriesCount = 0; seriesCount < maxCount ; seriesCount++)
				{
					DataSeries		series			= (DataSeries) Values[seriesCount];
					if (!series.IsValidPlot(lastBarIndex))
						continue;
					SmoothingMode 	oldSmoothingMode 	= graphics.SmoothingMode;
					Gui.Chart.Plot	plot				= Plots[seriesCount];
					SolidBrush		brush				= brushes[seriesCount];
					Color			cacheColor			= plot.Pen.Color;
					int 			x 					= 0;
					int 			firstX				= -1;
					int 			lastX				= -1;
					using (GraphicsPath	path = new GraphicsPath()) 
					{
						plot.Pen.Color = plotColors[seriesCount]; 							
						if (brush.Color != plotColors[seriesCount])	
							brush = new SolidBrush(plotColors[seriesCount]);
						if(seriesCount < 3) 
						{
							if (lastBarTime <= refPeriodEnd)
							{
								plot.Pen.Color = developingColor;
								brush.Color = developingColor;
							}
						}
						else if (seriesCount > 3)
						{
							if (lastBarTime <= prePeriodEnd)
							{
								plot.Pen.Color = developingColor;
								brush.Color = developingColor;
							}
						}	
						for (int idx = lastBarPlotIndex; idx >= firstBarPlotIndex; idx--)
						{
							if (idx - Displacement >= Bars.Count) 
								continue;
							else if (idx - Displacement < 0)
								break;
							if (idx < CurrentBar && !Values[seriesCount].IsValidPlot(idx))
								break;
							if(idx == lastBarPlotIndex)
							{
								firstX	= Math.Max(ChartControl.GetXByBarIdx(BarsArray[0], idx), lastXtoFill);
								lastX = firstX;
							}	
							x	= Math.Max(ChartControl.GetXByBarIdx(BarsArray[0], idx-1) + sessionGap, lastXtoFill);
							if (lastX >= 0)
								path.AddLine(lastX - plot.Pen.Width / 2, yArr[seriesCount], x - plot.Pen.Width / 2, yArr[seriesCount]);
							lastX	= x;
						}
						graphics.SmoothingMode = SmoothingMode.AntiAlias;
						graphics.DrawPath(plot.Pen, path);
						plot.Pen.Color = cacheColor;
						graphics.SmoothingMode = oldSmoothingMode;
						if (PlotLabels == anaPlotAlignOR42.Right)
						graphics.DrawString(labels[seriesCount], labelFont, brush, firstX + labelOffset, yArr[seriesCount] - labelFont.GetHeight() / 2, stringFormatNear);
						if (PlotLabels == anaPlotAlignOR42.Left)
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
        private anaOpeningRangeV42[] cacheanaOpeningRangeV42 = null;

        private static anaOpeningRangeV42 checkanaOpeningRangeV42 = new anaOpeningRangeV42();

        /// <summary>
        /// This indicator displays opening range and pre-session range for the selected session.
        /// </summary>
        /// <returns></returns>
        public anaOpeningRangeV42 anaOpeningRangeV42(string cETZone, bool extendOpeningRange, string s_AsianEndTime, string s_AsianStartTime, string s_EuropeanEndTime, string s_EuropeanStartTime, string s_OpeningPeriod, string s_OpeningPeriodOffset, anaPreSessionOR42 selectedPreSession, anaSessionCountOR42 selectedSession, string tSTZone)
        {
            return anaOpeningRangeV42(Input, cETZone, extendOpeningRange, s_AsianEndTime, s_AsianStartTime, s_EuropeanEndTime, s_EuropeanStartTime, s_OpeningPeriod, s_OpeningPeriodOffset, selectedPreSession, selectedSession, tSTZone);
        }

        /// <summary>
        /// This indicator displays opening range and pre-session range for the selected session.
        /// </summary>
        /// <returns></returns>
        public anaOpeningRangeV42 anaOpeningRangeV42(Data.IDataSeries input, string cETZone, bool extendOpeningRange, string s_AsianEndTime, string s_AsianStartTime, string s_EuropeanEndTime, string s_EuropeanStartTime, string s_OpeningPeriod, string s_OpeningPeriodOffset, anaPreSessionOR42 selectedPreSession, anaSessionCountOR42 selectedSession, string tSTZone)
        {
            if (cacheanaOpeningRangeV42 != null)
                for (int idx = 0; idx < cacheanaOpeningRangeV42.Length; idx++)
                    if (cacheanaOpeningRangeV42[idx].CETZone == cETZone && cacheanaOpeningRangeV42[idx].ExtendOpeningRange == extendOpeningRange && cacheanaOpeningRangeV42[idx].S_AsianEndTime == s_AsianEndTime && cacheanaOpeningRangeV42[idx].S_AsianStartTime == s_AsianStartTime && cacheanaOpeningRangeV42[idx].S_EuropeanEndTime == s_EuropeanEndTime && cacheanaOpeningRangeV42[idx].S_EuropeanStartTime == s_EuropeanStartTime && cacheanaOpeningRangeV42[idx].S_OpeningPeriod == s_OpeningPeriod && cacheanaOpeningRangeV42[idx].S_OpeningPeriodOffset == s_OpeningPeriodOffset && cacheanaOpeningRangeV42[idx].SelectedPreSession == selectedPreSession && cacheanaOpeningRangeV42[idx].SelectedSession == selectedSession && cacheanaOpeningRangeV42[idx].TSTZone == tSTZone && cacheanaOpeningRangeV42[idx].EqualsInput(input))
                        return cacheanaOpeningRangeV42[idx];

            lock (checkanaOpeningRangeV42)
            {
                checkanaOpeningRangeV42.CETZone = cETZone;
                cETZone = checkanaOpeningRangeV42.CETZone;
                checkanaOpeningRangeV42.ExtendOpeningRange = extendOpeningRange;
                extendOpeningRange = checkanaOpeningRangeV42.ExtendOpeningRange;
                checkanaOpeningRangeV42.S_AsianEndTime = s_AsianEndTime;
                s_AsianEndTime = checkanaOpeningRangeV42.S_AsianEndTime;
                checkanaOpeningRangeV42.S_AsianStartTime = s_AsianStartTime;
                s_AsianStartTime = checkanaOpeningRangeV42.S_AsianStartTime;
                checkanaOpeningRangeV42.S_EuropeanEndTime = s_EuropeanEndTime;
                s_EuropeanEndTime = checkanaOpeningRangeV42.S_EuropeanEndTime;
                checkanaOpeningRangeV42.S_EuropeanStartTime = s_EuropeanStartTime;
                s_EuropeanStartTime = checkanaOpeningRangeV42.S_EuropeanStartTime;
                checkanaOpeningRangeV42.S_OpeningPeriod = s_OpeningPeriod;
                s_OpeningPeriod = checkanaOpeningRangeV42.S_OpeningPeriod;
                checkanaOpeningRangeV42.S_OpeningPeriodOffset = s_OpeningPeriodOffset;
                s_OpeningPeriodOffset = checkanaOpeningRangeV42.S_OpeningPeriodOffset;
                checkanaOpeningRangeV42.SelectedPreSession = selectedPreSession;
                selectedPreSession = checkanaOpeningRangeV42.SelectedPreSession;
                checkanaOpeningRangeV42.SelectedSession = selectedSession;
                selectedSession = checkanaOpeningRangeV42.SelectedSession;
                checkanaOpeningRangeV42.TSTZone = tSTZone;
                tSTZone = checkanaOpeningRangeV42.TSTZone;

                if (cacheanaOpeningRangeV42 != null)
                    for (int idx = 0; idx < cacheanaOpeningRangeV42.Length; idx++)
                        if (cacheanaOpeningRangeV42[idx].CETZone == cETZone && cacheanaOpeningRangeV42[idx].ExtendOpeningRange == extendOpeningRange && cacheanaOpeningRangeV42[idx].S_AsianEndTime == s_AsianEndTime && cacheanaOpeningRangeV42[idx].S_AsianStartTime == s_AsianStartTime && cacheanaOpeningRangeV42[idx].S_EuropeanEndTime == s_EuropeanEndTime && cacheanaOpeningRangeV42[idx].S_EuropeanStartTime == s_EuropeanStartTime && cacheanaOpeningRangeV42[idx].S_OpeningPeriod == s_OpeningPeriod && cacheanaOpeningRangeV42[idx].S_OpeningPeriodOffset == s_OpeningPeriodOffset && cacheanaOpeningRangeV42[idx].SelectedPreSession == selectedPreSession && cacheanaOpeningRangeV42[idx].SelectedSession == selectedSession && cacheanaOpeningRangeV42[idx].TSTZone == tSTZone && cacheanaOpeningRangeV42[idx].EqualsInput(input))
                            return cacheanaOpeningRangeV42[idx];

                anaOpeningRangeV42 indicator = new anaOpeningRangeV42();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CETZone = cETZone;
                indicator.ExtendOpeningRange = extendOpeningRange;
                indicator.S_AsianEndTime = s_AsianEndTime;
                indicator.S_AsianStartTime = s_AsianStartTime;
                indicator.S_EuropeanEndTime = s_EuropeanEndTime;
                indicator.S_EuropeanStartTime = s_EuropeanStartTime;
                indicator.S_OpeningPeriod = s_OpeningPeriod;
                indicator.S_OpeningPeriodOffset = s_OpeningPeriodOffset;
                indicator.SelectedPreSession = selectedPreSession;
                indicator.SelectedSession = selectedSession;
                indicator.TSTZone = tSTZone;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaOpeningRangeV42[] tmp = new anaOpeningRangeV42[cacheanaOpeningRangeV42 == null ? 1 : cacheanaOpeningRangeV42.Length + 1];
                if (cacheanaOpeningRangeV42 != null)
                    cacheanaOpeningRangeV42.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaOpeningRangeV42 = tmp;
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
        /// This indicator displays opening range and pre-session range for the selected session.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaOpeningRangeV42 anaOpeningRangeV42(string cETZone, bool extendOpeningRange, string s_AsianEndTime, string s_AsianStartTime, string s_EuropeanEndTime, string s_EuropeanStartTime, string s_OpeningPeriod, string s_OpeningPeriodOffset, anaPreSessionOR42 selectedPreSession, anaSessionCountOR42 selectedSession, string tSTZone)
        {
            return _indicator.anaOpeningRangeV42(Input, cETZone, extendOpeningRange, s_AsianEndTime, s_AsianStartTime, s_EuropeanEndTime, s_EuropeanStartTime, s_OpeningPeriod, s_OpeningPeriodOffset, selectedPreSession, selectedSession, tSTZone);
        }

        /// <summary>
        /// This indicator displays opening range and pre-session range for the selected session.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaOpeningRangeV42 anaOpeningRangeV42(Data.IDataSeries input, string cETZone, bool extendOpeningRange, string s_AsianEndTime, string s_AsianStartTime, string s_EuropeanEndTime, string s_EuropeanStartTime, string s_OpeningPeriod, string s_OpeningPeriodOffset, anaPreSessionOR42 selectedPreSession, anaSessionCountOR42 selectedSession, string tSTZone)
        {
            return _indicator.anaOpeningRangeV42(input, cETZone, extendOpeningRange, s_AsianEndTime, s_AsianStartTime, s_EuropeanEndTime, s_EuropeanStartTime, s_OpeningPeriod, s_OpeningPeriodOffset, selectedPreSession, selectedSession, tSTZone);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// This indicator displays opening range and pre-session range for the selected session.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaOpeningRangeV42 anaOpeningRangeV42(string cETZone, bool extendOpeningRange, string s_AsianEndTime, string s_AsianStartTime, string s_EuropeanEndTime, string s_EuropeanStartTime, string s_OpeningPeriod, string s_OpeningPeriodOffset, anaPreSessionOR42 selectedPreSession, anaSessionCountOR42 selectedSession, string tSTZone)
        {
            return _indicator.anaOpeningRangeV42(Input, cETZone, extendOpeningRange, s_AsianEndTime, s_AsianStartTime, s_EuropeanEndTime, s_EuropeanStartTime, s_OpeningPeriod, s_OpeningPeriodOffset, selectedPreSession, selectedSession, tSTZone);
        }

        /// <summary>
        /// This indicator displays opening range and pre-session range for the selected session.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaOpeningRangeV42 anaOpeningRangeV42(Data.IDataSeries input, string cETZone, bool extendOpeningRange, string s_AsianEndTime, string s_AsianStartTime, string s_EuropeanEndTime, string s_EuropeanStartTime, string s_OpeningPeriod, string s_OpeningPeriodOffset, anaPreSessionOR42 selectedPreSession, anaSessionCountOR42 selectedSession, string tSTZone)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaOpeningRangeV42(input, cETZone, extendOpeningRange, s_AsianEndTime, s_AsianStartTime, s_EuropeanEndTime, s_EuropeanStartTime, s_OpeningPeriod, s_OpeningPeriodOffset, selectedPreSession, selectedSession, tSTZone);
        }
    }
}
#endregion
