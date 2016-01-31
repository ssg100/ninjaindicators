//### 
//### Colored time zones
//###
//### User		Date 		Description
//### ------	-------- 	-------------
//### Gaston	Dec 2010 	Added code to handle multi-day timezones
//### Gaston	Dec 2010 	Added code to display current time zone name as chart is scrolled
//###
#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Color the chart background for up to three custom time frames.
    /// </summary>
    [Description("Color the chart background for up to three custom time frames.")]
    public class TimeZoneColors : Indicator
    {
        #region Variables
        
				//### Tokyo time Zone
            private int zn1HrSt 		= 19;
			private int zn1MinSt 		= 00;
        	private int zn1HrEn 		= 04;
			private int zn1MinEn 		= 00;
			private Color region1Color 	= Color.Pink;
			private String region1Name 	= "Tokyo";
		
				//### London time Zone
			private int zn2HrSt 		= 03;
			private int zn2MinSt 		= 00;
        	private int zn2HrEn 		= 12;
			private int zn2MinEn 		= 00;
			private Color region2Color 	= Color.Beige;
			private String region2Name 	= "London";
		
				//### New York time Zone
			private int zn3HrSt 		= 08;
			private int zn3MinSt 		= 00;
        	private int zn3HrEn 		= 16;
			private int zn3MinEn 		= 00;
			private Color region3Color 	= Color.LightGreen;
			private String region3Name 	= "New York";
		
			int CZn1St=0, CZn2St=0, CZn3St=0;
			int CZn1En=0, CZn2En=0, CZn3En=0;
		
			private bool colorAll 		= false;
		
			private bool alertBool		= true;
		
			private System.Drawing.Font largeFont 	= new Font("Times", 20, System.Drawing.FontStyle.Bold);
			string currentZone="";
			int lastScreenBar=0;
		
        #endregion

        protected override void Initialize()
        {
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
			
			CZn1St		= ((Zn1HrSt * 10000) + (Zn1MinSt * 100));
			CZn1En	 	= ((Zn1HrEn * 10000) + (Zn1MinEn * 100));
			
			CZn2St		= ((Zn2HrSt * 10000) + (Zn2MinSt * 100));
			CZn2En	 	= ((Zn2HrEn * 10000) + (Zn2MinEn * 100));
			
			CZn3St		= ((Zn3HrSt * 10000) + (Zn3MinSt * 100));
			CZn3En	 	= ((Zn3HrEn * 10000) + (Zn3MinEn * 100));				
        }

		
		string GetTimeZone( int x ) {
			int zone=0;
			string zones="";
			if ( x < 0 ) x = 0;	
			
				//### Check for Zone 1
			zone=0;
			if ( CZn1En < CZn1St ) { //### Does the time zone end in the next day?
				if ( ToTime(Time[x])  >  CZn1St && ToTime(Time[x]) > CZn1En ) zone = 1;
				if ( ToTime(Time[x])  <  CZn1St && ToTime(Time[x]) < CZn1En ) zone = 1;
			}
			else if ( ToTime(Time[x]) >= CZn1St && ToTime(Time[x]) < CZn1En ) zone = 1;
				//## Overlapped zones?
			if ( zone > 0 ) zones = ( zones.CompareTo("") == 0 ) ? "Z"+zone+zones : "+"+zone+zones;
			
				
				//### Check for Zone 2
			zone=0;
			if ( CZn2En < CZn2St ) {  //### Does the time zone end in the next day?
				if ( ToTime(Time[x])  >  CZn2St && ToTime(Time[x]) > CZn2En ) zone = 2;
				if ( ToTime(Time[x])  <  CZn2St && ToTime(Time[x]) < CZn2En ) zone = 2;
			}
			else if ( ToTime(Time[x]) >= CZn2St && ToTime(Time[x]) < CZn2En ) zone = 2;
				//## Overlapped zones?
			if ( zone > 0 ) zones = ( zones.CompareTo("") == 0 ) ? "Z"+zone+zones : "+"+zone+zones;
			
				//### Check for Zone 3
			zone=0;
			if ( CZn3En < CZn3St ) { //### Does the time zone end in the next day?
				if ( ToTime(Time[x])  >  CZn3St && ToTime(Time[x]) > CZn3En ) zone = 3;
				if ( ToTime(Time[x])  <  CZn3St && ToTime(Time[x]) < CZn3En ) zone = 3;
			}
			else if ( ToTime(Time[x]) >= CZn3St && ToTime(Time[x]) < CZn3En ) zone = 3;
				//## Overlapped zones?
			if ( zone > 0 ) zones = ( zones.CompareTo("") == 0 ) ? "Z"+zone+zones : "+"+zone+zones;
			
			return zones;
		}
		
        protected override void OnBarUpdate()
        {
			Color zondeColor=Color.Empty;
			int a=0, r=0, g=0, b=0;

				string zone = GetTimeZone(0);
				if ( zone.CompareTo("") == 0  ) {r = ChartControl.BackColor.R; g = ChartControl.BackColor.G; b = ChartControl.BackColor.B;}
				else {
					if ( zone.Contains("Z3") ) {r = Region3Color.R; g = Region3Color.G; b = Region3Color.B;}
					if ( zone.Contains("Z2") ) {r = Region2Color.R; g = Region2Color.G; b = Region2Color.B;}
					if ( zone.Contains("Z1") ) {r = Region1Color.R; g = Region1Color.G; b = Region1Color.B;}
					
						//### Blend colors of overlapped timezones
					if ( zone.Contains("+1") ) {
						r = (int)(Math.Abs((r-Region1Color.R)*.35) + Math.Min(r,Region1Color.R));
						g = (int)(Math.Abs((g-Region1Color.G)*.35) + Math.Min(g,Region1Color.G));
						b = (int)(Math.Abs((b-Region1Color.B)*.35) + Math.Min(b,Region1Color.B));
					}
					if ( zone.Contains("+2") ) {
						r = (int)(Math.Abs((r-Region2Color.R)*.35) + Math.Min(r,Region2Color.R));
						g = (int)(Math.Abs((g-Region2Color.G)*.35) + Math.Min(g,Region2Color.G));
						b = (int)(Math.Abs((b-Region2Color.B)*.35) + Math.Min(b,Region2Color.B));
					}
					if ( zone.Contains("+3") ) {
						r = (int)(Math.Abs((r-Region3Color.R)*.35) + Math.Min(r,Region3Color.R));
						g = (int)(Math.Abs((g-Region3Color.G)*.35) + Math.Min(g,Region3Color.G));
						b = (int)(Math.Abs((b-Region3Color.B)*.35) + Math.Min(b,Region3Color.B));
					}
				}
				
				
			if (r > 255 || r < 0 ) r = 255; if (g > 255 || g < 0 ) g = 255; if ( b > 255 || b < 0 ) b = 255;	//### Limit check
			BackColor = Color.FromArgb(255,r,g,b);
		
			if (ColorAll == true) 
				BackColorAll = Color.FromArgb(255,r,g,b);
			else 
				BackColor = Color.FromArgb(255,r,g,b);
			
			if (AlertBool == true)
				if (ToTime(Time[0]) == CZn1St)
					Alert("r1st",NinjaTrader.Cbi.Priority.Medium, "Beginning of Time Region #1","Alert2.wav",0,Region1Color,Color.Black);
			
				else if (ToTime(Time[0]) == CZn1En)
					Alert("r1en",NinjaTrader.Cbi.Priority.Medium, "End of Time Region #1","Alert2.wav",0,Region1Color,Color.Black);
		
				else if (ToTime(Time[0]) == CZn2St)
					Alert("r2st",NinjaTrader.Cbi.Priority.Medium, "Beginning of Time Region #2","Alert2.wav",0,Region2Color,Color.Black);
			
				else if (ToTime(Time[0]) == CZn2En)
					Alert("r2en",NinjaTrader.Cbi.Priority.Medium, "End of Time Region #2","Alert2.wav",0,Region2Color,Color.Black);
			
				else if (ToTime(Time[0]) == CZn3St)
					Alert("r3st",NinjaTrader.Cbi.Priority.Medium, "Beginning of Time Region #3","Alert2.wav",0,Region3Color,Color.Black);
			
				else if (ToTime(Time[0]) == CZn3En)
					Alert("r3en",NinjaTrader.Cbi.Priority.Medium, "End of Time Region #3","Alert2.wav",0,Region3Color,Color.Black);			
        }

		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max) {
			base.Plot(graphics, bounds, min, max);
			string zone="";
			if ( ChartControl.LastBarPainted != lastScreenBar ) {
				int x = Bars.Count - ChartControl.LastBarPainted -1;
				if ( GetTimeZone(x).Contains("1") ) zone += region1Name +"\n";
				if ( GetTimeZone(x).Contains("2") ) zone += region2Name +"\n";
				if ( GetTimeZone(x).Contains("3") ) zone += region3Name +"\n";
				if ( zone.CompareTo(currentZone) != 0 ) currentZone = zone;
			}
			DrawTextFixed("Zone", currentZone, TextPosition.TopRight , Color.Black, largeFont, Color.Empty, Color.Red, 0);
			lastScreenBar = ChartControl.LastBarPainted;
		}	
				
        #region Properties

        [Description("Hour for 1st time region to begin, 24 hour clock.")]
        [Category("1st Time Region")]
		[Gui.Design.DisplayNameAttribute("Begin Hour")]
        public int Zn1HrSt
        {
            get { return zn1HrSt; }
            set { zn1HrSt = Math.Max(0, value); }
        }
		
		
		[Description("Minute for 1st time region to begin")]
        [Category("1st Time Region")]
		[Gui.Design.DisplayNameAttribute("Begin Minute")]
        public int Zn1MinSt
        {
            get { return zn1MinSt; }
            set { zn1MinSt = Math.Max(0, value); }
        }
		
		
		[Description("Hour for 1st time region to end, 24 hour clock.")]
        [Category("1st Time Region")]
		[Gui.Design.DisplayNameAttribute("End Hour")]
        public int Zn1HrEn
        {
            get { return zn1HrEn; }
            set { zn1HrEn = Math.Max(0, value); }
        }
		
		
		[Description("Minute for 1st time region to end")]
        [Category("1st Time Region")]
		[Gui.Design.DisplayNameAttribute("End Minute")]
        public int Zn1MinEn
        {
            get { return zn1MinEn; }
            set { zn1MinEn = Math.Max(0, value); }
        }
		
		[Description("Hour for 2nd time region to begin, 24 hour clock.")]
        [Category("2nd Time Region")]
		[Gui.Design.DisplayNameAttribute("Begin Hour")]
        public int Zn2HrSt
        {
            get { return zn2HrSt; }
            set { zn2HrSt = Math.Max(0, value); }
        }
		
		
		[Description("Minute for 2nd time region to begin")]
        [Category("2nd Time Region")]
		[Gui.Design.DisplayNameAttribute("Begin Minute")]
        public int Zn2MinSt
        {
            get { return zn2MinSt; }
            set { zn2MinSt = Math.Max(0, value); }
        }
		
		
		[Description("Hour for 2nd time region to end, 24 hour clock.")]
        [Category("2nd Time Region")]
		[Gui.Design.DisplayNameAttribute("End Hour")]
        public int Zn2HrEn
        {
            get { return zn2HrEn; }
            set { zn2HrEn = Math.Max(0, value); }
        }
		
		
		[Description("Minute for 2nd time region to end")]
        [Category("2nd Time Region")]
		[Gui.Design.DisplayNameAttribute("End Minute")]
        public int Zn2MinEn
        {
            get { return zn2MinEn; }
            set { zn2MinEn = Math.Max(0, value); }
        }
	
		[Description("Hour for 3rd time region to begin, 24 hour clock.")]
        [Category("3rd Time Region")]
		[Gui.Design.DisplayNameAttribute("Begin Hour")]
        public int Zn3HrSt
        {
            get { return zn3HrSt; }
            set { zn3HrSt = Math.Max(0, value); }
        }
		
		
		[Description("Minute for 3rd time region to begin")]
        [Category("3rd Time Region")]
		[Gui.Design.DisplayNameAttribute("Begin Minute")]
        public int Zn3MinSt
        {
            get { return zn3MinSt; }
            set { zn3MinSt = Math.Max(0, value); }
        }
		
		
		[Description("Hour for 3rd time region to end, 24 hour clock.")]
        [Category("3rd Time Region")]
		[Gui.Design.DisplayNameAttribute("End Hour")]
        public int Zn3HrEn
        {
            get { return zn3HrEn; }
            set { zn3HrEn = Math.Max(0, value); }
        }
		
		
		[Description("Minute for 3rd time region to end")]
        [Category("3rd Time Region")]
		[Gui.Design.DisplayNameAttribute("End Minute")]
        public int Zn3MinEn
        {
            get { return zn3MinEn; }
            set { zn3MinEn = Math.Max(0, value); }
        }

		[Description("Colors across all panels or just main price display panel")]
        [Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Color all?")]
        public bool ColorAll
        {
            get { return colorAll; }
            set { colorAll = value; }
        }
		
		
		[XmlIgnore()]
        [Description("Color for 1st region")]
        [Category("1st Time Region")]
		[Gui.Design.DisplayNameAttribute("Region Color")]
        public Color Region1Color
        {
            get { return region1Color; }
            set { region1Color = value; }
        }
		
		[Browsable(false)]
		public string Region1ColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(region1Color); }
			set { region1Color = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		
		[XmlIgnore()]
        [Description("Color for 2nd region")]
        [Category("2nd Time Region")]
		[Gui.Design.DisplayNameAttribute("Region Color")]
        public Color Region2Color
        {
            get { return region2Color; }
            set { region2Color = value; }
        }
		
		[Browsable(false)]
		public string Region2ColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(region2Color); }
			set { region2Color = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		
		[XmlIgnore()]
        [Description("Color for 3rd region")]
        [Category("3rd Time Region")]
		[Gui.Design.DisplayNameAttribute("Region Color")]
        public Color Region3Color
        {
            get { return region3Color; }
            set { region3Color = value; }
        }
		
		[Browsable(false)]
		public string Region3ColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(region3Color); }
			set { region3Color = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		
		[Description("Sound and display an alert with Alerts window open ")]
        [Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Alert on Begin/End?")]
        public bool AlertBool
        {
            get { return alertBool; }
            set { alertBool = value; }
        }
		
		[Description("Name of 1st time region")]
        [Category("1st Time Region")]
		[Gui.Design.DisplayNameAttribute("Region Name")]
        public string Region1Name
        {
            get { return region1Name; }
            set { region1Name = value; }
        }
			
		[Description("Name of 2nd time region")]
        [Category("2nd Time Region")]
		[Gui.Design.DisplayNameAttribute("Region Name")]
        public string Region2Name
        {
            get { return region2Name; }
            set { region2Name = value; }
        }
		
		[Description("Name of 3rd time region")]
        [Category("3rd Time Region")]
		[Gui.Design.DisplayNameAttribute("Region Name")]
        public string Region3Name
        {
            get { return region3Name; }
            set { region3Name = value; }
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
        private TimeZoneColors[] cacheTimeZoneColors = null;

        private static TimeZoneColors checkTimeZoneColors = new TimeZoneColors();

        /// <summary>
        /// Color the chart background for up to three custom time frames.
        /// </summary>
        /// <returns></returns>
        public TimeZoneColors TimeZoneColors(bool alertBool, bool colorAll)
        {
            return TimeZoneColors(Input, alertBool, colorAll);
        }

        /// <summary>
        /// Color the chart background for up to three custom time frames.
        /// </summary>
        /// <returns></returns>
        public TimeZoneColors TimeZoneColors(Data.IDataSeries input, bool alertBool, bool colorAll)
        {
            if (cacheTimeZoneColors != null)
                for (int idx = 0; idx < cacheTimeZoneColors.Length; idx++)
                    if (cacheTimeZoneColors[idx].AlertBool == alertBool && cacheTimeZoneColors[idx].ColorAll == colorAll && cacheTimeZoneColors[idx].EqualsInput(input))
                        return cacheTimeZoneColors[idx];

            lock (checkTimeZoneColors)
            {
                checkTimeZoneColors.AlertBool = alertBool;
                alertBool = checkTimeZoneColors.AlertBool;
                checkTimeZoneColors.ColorAll = colorAll;
                colorAll = checkTimeZoneColors.ColorAll;

                if (cacheTimeZoneColors != null)
                    for (int idx = 0; idx < cacheTimeZoneColors.Length; idx++)
                        if (cacheTimeZoneColors[idx].AlertBool == alertBool && cacheTimeZoneColors[idx].ColorAll == colorAll && cacheTimeZoneColors[idx].EqualsInput(input))
                            return cacheTimeZoneColors[idx];

                TimeZoneColors indicator = new TimeZoneColors();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AlertBool = alertBool;
                indicator.ColorAll = colorAll;
                Indicators.Add(indicator);
                indicator.SetUp();

                TimeZoneColors[] tmp = new TimeZoneColors[cacheTimeZoneColors == null ? 1 : cacheTimeZoneColors.Length + 1];
                if (cacheTimeZoneColors != null)
                    cacheTimeZoneColors.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTimeZoneColors = tmp;
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
        /// Color the chart background for up to three custom time frames.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TimeZoneColors TimeZoneColors(bool alertBool, bool colorAll)
        {
            return _indicator.TimeZoneColors(Input, alertBool, colorAll);
        }

        /// <summary>
        /// Color the chart background for up to three custom time frames.
        /// </summary>
        /// <returns></returns>
        public Indicator.TimeZoneColors TimeZoneColors(Data.IDataSeries input, bool alertBool, bool colorAll)
        {
            return _indicator.TimeZoneColors(input, alertBool, colorAll);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Color the chart background for up to three custom time frames.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TimeZoneColors TimeZoneColors(bool alertBool, bool colorAll)
        {
            return _indicator.TimeZoneColors(Input, alertBool, colorAll);
        }

        /// <summary>
        /// Color the chart background for up to three custom time frames.
        /// </summary>
        /// <returns></returns>
        public Indicator.TimeZoneColors TimeZoneColors(Data.IDataSeries input, bool alertBool, bool colorAll)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TimeZoneColors(input, alertBool, colorAll);
        }
    }
}
#endregion
