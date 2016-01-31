// ==============================================
// NinjaTrader module by Jurik Research Software
// © 2010 Jurik Research   ;   www.jurikres.com
// ==============================================

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

namespace NinjaTrader.Indicator
{
	#region Header
		[Description("MACD using JMA & DWMA")]
		public class Jurik_JMA_DWMA_MACD : Indicator
		#endregion
    {
        #region Variables	// default values
			private int dwma_len = 35; 		 
			private double jma_len = 20; 	
			private double jma_phase = 100; 	 
			private int trail_len = 10; 	
			// --------------------------
			private double MacdValue = 0;
			private double MacdTrail = 0;
			private DataSeries PriceSeries;
			private DataSeries MacdSeries;
        	#endregion
		
		#region Input Parameters
			[Description("DWMA length, any integer > 2")]
			[GridCategory("Parameters")]
			public int Dwma_len
			{
				get { return dwma_len; }
				set { dwma_len = Math.Max(3, value); }
			}
	
			[Description("JMA length, any value >= 1")]
			[GridCategory("Parameters")]
			public double Jma_len
			{
				get { return jma_len; }
				set { jma_len = Math.Max(1, value); }
			}
	
			[Description("JMA phase, any value between -100 and +100")]
			[GridCategory("Parameters")]
			public double Jma_phase
			{
				get { return jma_phase; }
				set { jma_phase = Math.Max(-100, Math.Min(100,value)); }
			}
			[Description("Trailing SMA length, any integer > 1")]
			[GridCategory("Parameters")]
			public int Trail_len
			{
				get { return trail_len; }
				set { trail_len = Math.Max(2, value); }
			}
        	#endregion
		
        protected override void Initialize()
        {
			#region Chart Features
				Add(new Plot(Color.LimeGreen, PlotStyle.Line, "MACD"));
				Add(new Plot(Color.Violet, PlotStyle.Line, "trail"));
			
				Plots[0].Pen.Width = 2;
				Plots[1].Pen.Width = 2;
				Plots[1].Pen.DashStyle = DashStyle.Dot ;
			
				Add(new Line(Color.Gray, 0, "Zero Line"));
				Lines[0].Pen.Width = 2;
			
				CalculateOnBarClose	= false;
				Overlay				= false;
				PriceTypeSupported	= false;
				#endregion
						
			#region Series Initialization
				PriceSeries = new DataSeries(this);
				MacdSeries 	= new DataSeries(this);
				#endregion
        }

        protected override void OnBarUpdate()
        {
			#region Indicator Formula
				PriceSeries.Set( ( High[0] + Low[0] + 2*Close[0] )/4 );
				MacdValue = JurikJMA(PriceSeries, Jma_phase, Jma_len)[0] - WMA(WMA(PriceSeries, Dwma_len), Dwma_len)[0];
				MacdSeries.Set( MacdValue );
				MacdTrail = SMA(MacdSeries, Trail_len)[0];
				Jurik_MACD.Set( CurrentBar >= 50 ? MacdValue : 0 );
				Jurik_MACD_trail.Set( CurrentBar >= 50 ? MacdTrail : 0 ); 
				#endregion
        }

        #region Output Values
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries Jurik_MACD
			{
				get { return Values[0]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries Jurik_MACD_trail
			{
				get { return Values[1]; }
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
        private Jurik_JMA_DWMA_MACD[] cacheJurik_JMA_DWMA_MACD = null;

        private static Jurik_JMA_DWMA_MACD checkJurik_JMA_DWMA_MACD = new Jurik_JMA_DWMA_MACD();

        /// <summary>
        /// MACD using JMA & DWMA
        /// </summary>
        /// <returns></returns>
        public Jurik_JMA_DWMA_MACD Jurik_JMA_DWMA_MACD(int dwma_len, double jma_len, double jma_phase, int trail_len)
        {
            return Jurik_JMA_DWMA_MACD(Input, dwma_len, jma_len, jma_phase, trail_len);
        }

        /// <summary>
        /// MACD using JMA & DWMA
        /// </summary>
        /// <returns></returns>
        public Jurik_JMA_DWMA_MACD Jurik_JMA_DWMA_MACD(Data.IDataSeries input, int dwma_len, double jma_len, double jma_phase, int trail_len)
        {
            if (cacheJurik_JMA_DWMA_MACD != null)
                for (int idx = 0; idx < cacheJurik_JMA_DWMA_MACD.Length; idx++)
                    if (cacheJurik_JMA_DWMA_MACD[idx].Dwma_len == dwma_len && Math.Abs(cacheJurik_JMA_DWMA_MACD[idx].Jma_len - jma_len) <= double.Epsilon && Math.Abs(cacheJurik_JMA_DWMA_MACD[idx].Jma_phase - jma_phase) <= double.Epsilon && cacheJurik_JMA_DWMA_MACD[idx].Trail_len == trail_len && cacheJurik_JMA_DWMA_MACD[idx].EqualsInput(input))
                        return cacheJurik_JMA_DWMA_MACD[idx];

            lock (checkJurik_JMA_DWMA_MACD)
            {
                checkJurik_JMA_DWMA_MACD.Dwma_len = dwma_len;
                dwma_len = checkJurik_JMA_DWMA_MACD.Dwma_len;
                checkJurik_JMA_DWMA_MACD.Jma_len = jma_len;
                jma_len = checkJurik_JMA_DWMA_MACD.Jma_len;
                checkJurik_JMA_DWMA_MACD.Jma_phase = jma_phase;
                jma_phase = checkJurik_JMA_DWMA_MACD.Jma_phase;
                checkJurik_JMA_DWMA_MACD.Trail_len = trail_len;
                trail_len = checkJurik_JMA_DWMA_MACD.Trail_len;

                if (cacheJurik_JMA_DWMA_MACD != null)
                    for (int idx = 0; idx < cacheJurik_JMA_DWMA_MACD.Length; idx++)
                        if (cacheJurik_JMA_DWMA_MACD[idx].Dwma_len == dwma_len && Math.Abs(cacheJurik_JMA_DWMA_MACD[idx].Jma_len - jma_len) <= double.Epsilon && Math.Abs(cacheJurik_JMA_DWMA_MACD[idx].Jma_phase - jma_phase) <= double.Epsilon && cacheJurik_JMA_DWMA_MACD[idx].Trail_len == trail_len && cacheJurik_JMA_DWMA_MACD[idx].EqualsInput(input))
                            return cacheJurik_JMA_DWMA_MACD[idx];

                Jurik_JMA_DWMA_MACD indicator = new Jurik_JMA_DWMA_MACD();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Dwma_len = dwma_len;
                indicator.Jma_len = jma_len;
                indicator.Jma_phase = jma_phase;
                indicator.Trail_len = trail_len;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_JMA_DWMA_MACD[] tmp = new Jurik_JMA_DWMA_MACD[cacheJurik_JMA_DWMA_MACD == null ? 1 : cacheJurik_JMA_DWMA_MACD.Length + 1];
                if (cacheJurik_JMA_DWMA_MACD != null)
                    cacheJurik_JMA_DWMA_MACD.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_JMA_DWMA_MACD = tmp;
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
        /// MACD using JMA & DWMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_JMA_DWMA_MACD Jurik_JMA_DWMA_MACD(int dwma_len, double jma_len, double jma_phase, int trail_len)
        {
            return _indicator.Jurik_JMA_DWMA_MACD(Input, dwma_len, jma_len, jma_phase, trail_len);
        }

        /// <summary>
        /// MACD using JMA & DWMA
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_JMA_DWMA_MACD Jurik_JMA_DWMA_MACD(Data.IDataSeries input, int dwma_len, double jma_len, double jma_phase, int trail_len)
        {
            return _indicator.Jurik_JMA_DWMA_MACD(input, dwma_len, jma_len, jma_phase, trail_len);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// MACD using JMA & DWMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_JMA_DWMA_MACD Jurik_JMA_DWMA_MACD(int dwma_len, double jma_len, double jma_phase, int trail_len)
        {
            return _indicator.Jurik_JMA_DWMA_MACD(Input, dwma_len, jma_len, jma_phase, trail_len);
        }

        /// <summary>
        /// MACD using JMA & DWMA
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_JMA_DWMA_MACD Jurik_JMA_DWMA_MACD(Data.IDataSeries input, int dwma_len, double jma_len, double jma_phase, int trail_len)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_JMA_DWMA_MACD(input, dwma_len, jma_len, jma_phase, trail_len);
        }
    }
}
#endregion
