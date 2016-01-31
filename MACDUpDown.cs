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
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Colors the MACD plot when rising or falling")]
    public class MACDUpDown : Indicator
    {
        #region Variables
       
		private int 			fast 		= 12;
		private int				slow		= 26;
		private int				smooth		= 9;
		private Color			uptick 		= Color.Green;
		private Color			downtick	= Color.Red;
		
		private DataSeries		fastEma;
		private DataSeries		slowEma;
		
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Green, PlotStyle.Line, "Macd"));
			Add(new Plot(Color.DarkViolet, "Avg"));
			Add(new Plot(new Pen(Color.Navy, 2), PlotStyle.Bar, "Diff"));
			
			Add(new Line(Color.DarkGray, 0, "Zero line"));
			
			fastEma = new DataSeries(this);
			slowEma = new DataSeries(this);
			
			
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
           if (CurrentBar == 0)
			{
				fastEma.Set(Input[0]);
				slowEma.Set(Input[0]);
				Value.Set(0);
				Avg.Set(0);
				Diff.Set(0);
			}
			else
			{
				fastEma.Set((2.0 / (1 + Fast)) * Input[0] + (1 - (2.0 / (1 + Fast))) * fastEma[1]);
				slowEma.Set((2.0 / (1 + Slow)) * Input[0] + (1 - (2.0 / (1 + Slow))) * slowEma[1]);

				double macd		= fastEma[0] - slowEma[0];
				double macdAvg	= (2.0 / (1 + Smooth)) * macd + (1 - (2.0 / (1 + Smooth))) * Avg[1];
				
				Value.Set(macd);
				Avg.Set(macdAvg);
				Diff.Set(macd - macdAvg);
			}
			
			// Plots MACD color when rising or falling
        	if (Rising(MACD(fast, slow, smooth)))
			{
				PlotColors[0][0] = Uptick;
			}
			else if (Falling(MACD(fast, slow, smooth)))
			{	
				PlotColors[0][0] = Downtick;
			}
        }

        #region Properties
        
		
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Avg
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Default
		{
			get { return Values[0]; }
		}
		
        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Diff
		{
			get { return Values[2]; }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for fast EMA")]
		[GridCategory("Parameters")]
		public int Fast
		{
			get { return fast; }
			set { fast = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for slow EMA")]
		[GridCategory("Parameters")]
		public int Slow
		{
			get { return slow; }
			set { slow = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smooth; }
			set { smooth = Math.Max(1, value); }
		}
		
		[XmlIgnore()]
        [Description("Color for MACD when rising")]
        [Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Macd Up Color")]
        public Color Uptick
        {
            get { return uptick; }
            set { uptick = value; }
        }
		[Browsable(false)]
		public string UptickSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(Uptick); }
			set { Uptick = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
        [Description("Color for MACD when falling")]
        [Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Macd Down Color")]
        public Color Downtick
        {
            get { return downtick; }
            set { downtick = value; }
        }
		[Browsable(false)]
		public string DowntickSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(Downtick); }
			set { Downtick = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
        private MACDUpDown[] cacheMACDUpDown = null;

        private static MACDUpDown checkMACDUpDown = new MACDUpDown();

        /// <summary>
        /// Colors the MACD plot when rising or falling
        /// </summary>
        /// <returns></returns>
        public MACDUpDown MACDUpDown(int fast, int slow, int smooth)
        {
            return MACDUpDown(Input, fast, slow, smooth);
        }

        /// <summary>
        /// Colors the MACD plot when rising or falling
        /// </summary>
        /// <returns></returns>
        public MACDUpDown MACDUpDown(Data.IDataSeries input, int fast, int slow, int smooth)
        {
            if (cacheMACDUpDown != null)
                for (int idx = 0; idx < cacheMACDUpDown.Length; idx++)
                    if (cacheMACDUpDown[idx].Fast == fast && cacheMACDUpDown[idx].Slow == slow && cacheMACDUpDown[idx].Smooth == smooth && cacheMACDUpDown[idx].EqualsInput(input))
                        return cacheMACDUpDown[idx];

            lock (checkMACDUpDown)
            {
                checkMACDUpDown.Fast = fast;
                fast = checkMACDUpDown.Fast;
                checkMACDUpDown.Slow = slow;
                slow = checkMACDUpDown.Slow;
                checkMACDUpDown.Smooth = smooth;
                smooth = checkMACDUpDown.Smooth;

                if (cacheMACDUpDown != null)
                    for (int idx = 0; idx < cacheMACDUpDown.Length; idx++)
                        if (cacheMACDUpDown[idx].Fast == fast && cacheMACDUpDown[idx].Slow == slow && cacheMACDUpDown[idx].Smooth == smooth && cacheMACDUpDown[idx].EqualsInput(input))
                            return cacheMACDUpDown[idx];

                MACDUpDown indicator = new MACDUpDown();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Fast = fast;
                indicator.Slow = slow;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                MACDUpDown[] tmp = new MACDUpDown[cacheMACDUpDown == null ? 1 : cacheMACDUpDown.Length + 1];
                if (cacheMACDUpDown != null)
                    cacheMACDUpDown.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMACDUpDown = tmp;
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
        /// Colors the MACD plot when rising or falling
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MACDUpDown MACDUpDown(int fast, int slow, int smooth)
        {
            return _indicator.MACDUpDown(Input, fast, slow, smooth);
        }

        /// <summary>
        /// Colors the MACD plot when rising or falling
        /// </summary>
        /// <returns></returns>
        public Indicator.MACDUpDown MACDUpDown(Data.IDataSeries input, int fast, int slow, int smooth)
        {
            return _indicator.MACDUpDown(input, fast, slow, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Colors the MACD plot when rising or falling
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MACDUpDown MACDUpDown(int fast, int slow, int smooth)
        {
            return _indicator.MACDUpDown(Input, fast, slow, smooth);
        }

        /// <summary>
        /// Colors the MACD plot when rising or falling
        /// </summary>
        /// <returns></returns>
        public Indicator.MACDUpDown MACDUpDown(Data.IDataSeries input, int fast, int slow, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MACDUpDown(input, fast, slow, smooth);
        }
    }
}
#endregion
