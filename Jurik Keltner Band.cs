// ==============================================
// NinjaTrader module by Jurik Research Software
// © 2010 Jurik Research   ;   www.jurikres.com
// ==============================================

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

namespace NinjaTrader.Indicator
{
	#region Header
		[Description("Keltner Band using JMA")]
		public class Jurik_JMA_Keltner : Indicator
		#endregion
    {
        #region Variables	// default values
            private int bandLen = 15; 		
            private double bandWidth = 1.0; 		
            private int bandDelay = 0; 			
			// -------------------------------
            private double DWMA = 0; 		
			private double bandSize = 0;
			private double TR = 0;
			private DataSeries trueRange;
			private DataSeries PriceSeries;
        	#endregion
		
		#region Input Parameters
			[Description("Band delay, any integer >= 0")]
			[GridCategory("Parameters")]
			public int Band_delay
			{
				get { return bandDelay; }
				set { bandDelay = Math.Max(0, value); }
			}
			
			[Description("Band smoothness, any integer > 0")]
			[GridCategory("Parameters")]
			public int Band_len
			{
				get { return bandLen; }
				set { bandLen = Math.Max(1, value) ; }
			}
	
			[Description("Band width scale, any value >= 0")]
			[GridCategory("Parameters")]
			public double Band_width
			{
				get { return bandWidth; }
				set { bandWidth = Math.Max(0, value); }
			}
			#endregion
		
        protected override void Initialize()
        {
			#region Chart Features
            	Add(new Plot(Color.Red, PlotStyle.Line, "JMA"));
            	Add(new Plot(Color.SteelBlue, PlotStyle.Line, "UpperBand"));
            	Add(new Plot(Color.SteelBlue, PlotStyle.Line, "LowerBand"));
			
				Plots[0].Pen.Width = 2;			
				Plots[1].Pen.DashStyle = DashStyle.Dot;
				Plots[2].Pen.DashStyle = DashStyle.Dot;
			
            	CalculateOnBarClose	= false;
            	Overlay				= true;
            	PriceTypeSupported	= false;
				#endregion
						
			#region Series Initialization
				PriceSeries = new DataSeries(this);
				trueRange = new DataSeries(this);			
				#endregion
        }

        protected override void OnBarUpdate()
        {
			#region Indicator formula
				PriceSeries.Set( ( High[0] + Low[0] + Close[0] )/3 );
				JMA.Set(JurikJMA( PriceSeries, 7, -25 )[0]);
				TR = High[0] - Low[0] ;
				if (CurrentBar > 0) TR = Math.Max( TR, Math.Max(High[0]-Close[1], Close[1]-Low[0]));
				trueRange.Set( TR ) ;
				bandSize = Band_width * SMA(trueRange,200)[Math.Min(CurrentBar,Band_delay)];
				DWMA = WMA(WMA(PriceSeries,Band_len),Band_len)[Math.Min(CurrentBar,Band_delay)];
				UpperBand.Set(DWMA + bandSize);
				LowerBand.Set(DWMA - bandSize);				
				#endregion
        }

        #region Output Values
			[Browsable(false)]	// do not remove
			[XmlIgnore()]		// do not remove
			public DataSeries JMA
			{
				get { return Values[0]; }
			}
	
			[Browsable(false)]	// do not remove
			[XmlIgnore()]		// do not remove
			public DataSeries UpperBand
			{
				get { return Values[1]; }
			}
	
			[Browsable(false)]	// do not remove
			[XmlIgnore()]		// do not remove
			public DataSeries LowerBand
			{
				get { return Values[2]; }
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
        private Jurik_JMA_Keltner[] cacheJurik_JMA_Keltner = null;

        private static Jurik_JMA_Keltner checkJurik_JMA_Keltner = new Jurik_JMA_Keltner();

        /// <summary>
        /// Keltner Band using JMA
        /// </summary>
        /// <returns></returns>
        public Jurik_JMA_Keltner Jurik_JMA_Keltner(int band_delay, int band_len, double band_width)
        {
            return Jurik_JMA_Keltner(Input, band_delay, band_len, band_width);
        }

        /// <summary>
        /// Keltner Band using JMA
        /// </summary>
        /// <returns></returns>
        public Jurik_JMA_Keltner Jurik_JMA_Keltner(Data.IDataSeries input, int band_delay, int band_len, double band_width)
        {
            if (cacheJurik_JMA_Keltner != null)
                for (int idx = 0; idx < cacheJurik_JMA_Keltner.Length; idx++)
                    if (cacheJurik_JMA_Keltner[idx].Band_delay == band_delay && cacheJurik_JMA_Keltner[idx].Band_len == band_len && Math.Abs(cacheJurik_JMA_Keltner[idx].Band_width - band_width) <= double.Epsilon && cacheJurik_JMA_Keltner[idx].EqualsInput(input))
                        return cacheJurik_JMA_Keltner[idx];

            lock (checkJurik_JMA_Keltner)
            {
                checkJurik_JMA_Keltner.Band_delay = band_delay;
                band_delay = checkJurik_JMA_Keltner.Band_delay;
                checkJurik_JMA_Keltner.Band_len = band_len;
                band_len = checkJurik_JMA_Keltner.Band_len;
                checkJurik_JMA_Keltner.Band_width = band_width;
                band_width = checkJurik_JMA_Keltner.Band_width;

                if (cacheJurik_JMA_Keltner != null)
                    for (int idx = 0; idx < cacheJurik_JMA_Keltner.Length; idx++)
                        if (cacheJurik_JMA_Keltner[idx].Band_delay == band_delay && cacheJurik_JMA_Keltner[idx].Band_len == band_len && Math.Abs(cacheJurik_JMA_Keltner[idx].Band_width - band_width) <= double.Epsilon && cacheJurik_JMA_Keltner[idx].EqualsInput(input))
                            return cacheJurik_JMA_Keltner[idx];

                Jurik_JMA_Keltner indicator = new Jurik_JMA_Keltner();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Band_delay = band_delay;
                indicator.Band_len = band_len;
                indicator.Band_width = band_width;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_JMA_Keltner[] tmp = new Jurik_JMA_Keltner[cacheJurik_JMA_Keltner == null ? 1 : cacheJurik_JMA_Keltner.Length + 1];
                if (cacheJurik_JMA_Keltner != null)
                    cacheJurik_JMA_Keltner.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_JMA_Keltner = tmp;
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
        /// Keltner Band using JMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_JMA_Keltner Jurik_JMA_Keltner(int band_delay, int band_len, double band_width)
        {
            return _indicator.Jurik_JMA_Keltner(Input, band_delay, band_len, band_width);
        }

        /// <summary>
        /// Keltner Band using JMA
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_JMA_Keltner Jurik_JMA_Keltner(Data.IDataSeries input, int band_delay, int band_len, double band_width)
        {
            return _indicator.Jurik_JMA_Keltner(input, band_delay, band_len, band_width);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Keltner Band using JMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_JMA_Keltner Jurik_JMA_Keltner(int band_delay, int band_len, double band_width)
        {
            return _indicator.Jurik_JMA_Keltner(Input, band_delay, band_len, band_width);
        }

        /// <summary>
        /// Keltner Band using JMA
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_JMA_Keltner Jurik_JMA_Keltner(Data.IDataSeries input, int band_delay, int band_len, double band_width)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_JMA_Keltner(input, band_delay, band_len, band_width);
        }
    }
}
#endregion
