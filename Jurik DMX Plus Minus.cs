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
		[Description("custom DMX +/-")]
		public class Jurik_DMX_plusminus : Indicator
		#endregion
    {
		#region Variables	// default values
			private int dmx_len = 30; 
			private double threshold = 50;
			// -------------------------------
			private double DMXplus  = 0;
			private double DMXminus = 0;
			#endregion
		
		#region Input Parameters
		
			[Description("flat line, any value between 0 and 100")]
			[GridCategory("Parameters")]
			public double _Threshold
			{
				get { return threshold; }
				set { threshold = Math.Min(Math.Max(0, value), 100); }
			}
			
			[Description("DMX length, any integer > 2")]
			[GridCategory("Parameters")]
			public int DMX_len
			{
				get { return dmx_len; }
				set { dmx_len = Math.Max(3, value); }
			}
			#endregion

        protected override void Initialize()
        {
			#region Chart Features
				Add(new Plot(Color.Cyan, PlotStyle.Line, "DMX+"));
				Add(new Plot(Color.Red, PlotStyle.Line, "DMX-"));
				Add(new Plot(Color.Gray, PlotStyle.Hash, "Threshold Line"));
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range max"));  // invisible
			
				Plots[2].Pen.DashStyle = DashStyle.Dash;
				
				Add(new Line(Color.Gray, 0, "Zero Line"));
				Lines[0].Pen.Width = 2;
	
				CalculateOnBarClose	= false;
				Overlay				= false;
				PriceTypeSupported	= false;
				#endregion
			
			#region Series Initialization
				// none
				#endregion
		}

        protected override void OnBarUpdate()
        {
			#region Indicator Formula
				_ThresholdLine.Set( _Threshold );
				
				DMXplus  = JurikDMX(DMX_len).DMXPlus[0];
				DMXminus = JurikDMX(DMX_len).DMXMinus[0];
			
				DMX_plus.Set( CurrentBar >= 50 ? DMXplus : 0);
				DMX_minus.Set( CurrentBar >= 50 ? DMXminus : 0);
				#endregion
			
			#region Panel Stabilizer
				panel_range_max.Set(70);
				#endregion
        }

        #region Output Values
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries DMX_plus
			{
				get { return Values[0]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries DMX_minus
			{
				get { return Values[1]; }
			}
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries _ThresholdLine
			{
				get { return Values[2]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_max
			{
				get { return Values[3]; }
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
        private Jurik_DMX_plusminus[] cacheJurik_DMX_plusminus = null;

        private static Jurik_DMX_plusminus checkJurik_DMX_plusminus = new Jurik_DMX_plusminus();

        /// <summary>
        /// custom DMX +/-
        /// </summary>
        /// <returns></returns>
        public Jurik_DMX_plusminus Jurik_DMX_plusminus(double _Threshold, int dMX_len)
        {
            return Jurik_DMX_plusminus(Input, _Threshold, dMX_len);
        }

        /// <summary>
        /// custom DMX +/-
        /// </summary>
        /// <returns></returns>
        public Jurik_DMX_plusminus Jurik_DMX_plusminus(Data.IDataSeries input, double _Threshold, int dMX_len)
        {
            if (cacheJurik_DMX_plusminus != null)
                for (int idx = 0; idx < cacheJurik_DMX_plusminus.Length; idx++)
                    if (Math.Abs(cacheJurik_DMX_plusminus[idx]._Threshold - _Threshold) <= double.Epsilon && cacheJurik_DMX_plusminus[idx].DMX_len == dMX_len && cacheJurik_DMX_plusminus[idx].EqualsInput(input))
                        return cacheJurik_DMX_plusminus[idx];

            lock (checkJurik_DMX_plusminus)
            {
                checkJurik_DMX_plusminus._Threshold = _Threshold;
                _Threshold = checkJurik_DMX_plusminus._Threshold;
                checkJurik_DMX_plusminus.DMX_len = dMX_len;
                dMX_len = checkJurik_DMX_plusminus.DMX_len;

                if (cacheJurik_DMX_plusminus != null)
                    for (int idx = 0; idx < cacheJurik_DMX_plusminus.Length; idx++)
                        if (Math.Abs(cacheJurik_DMX_plusminus[idx]._Threshold - _Threshold) <= double.Epsilon && cacheJurik_DMX_plusminus[idx].DMX_len == dMX_len && cacheJurik_DMX_plusminus[idx].EqualsInput(input))
                            return cacheJurik_DMX_plusminus[idx];

                Jurik_DMX_plusminus indicator = new Jurik_DMX_plusminus();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator._Threshold = _Threshold;
                indicator.DMX_len = dMX_len;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_DMX_plusminus[] tmp = new Jurik_DMX_plusminus[cacheJurik_DMX_plusminus == null ? 1 : cacheJurik_DMX_plusminus.Length + 1];
                if (cacheJurik_DMX_plusminus != null)
                    cacheJurik_DMX_plusminus.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_DMX_plusminus = tmp;
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
        /// custom DMX +/-
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_DMX_plusminus Jurik_DMX_plusminus(double _Threshold, int dMX_len)
        {
            return _indicator.Jurik_DMX_plusminus(Input, _Threshold, dMX_len);
        }

        /// <summary>
        /// custom DMX +/-
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_DMX_plusminus Jurik_DMX_plusminus(Data.IDataSeries input, double _Threshold, int dMX_len)
        {
            return _indicator.Jurik_DMX_plusminus(input, _Threshold, dMX_len);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// custom DMX +/-
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_DMX_plusminus Jurik_DMX_plusminus(double _Threshold, int dMX_len)
        {
            return _indicator.Jurik_DMX_plusminus(Input, _Threshold, dMX_len);
        }

        /// <summary>
        /// custom DMX +/-
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_DMX_plusminus Jurik_DMX_plusminus(Data.IDataSeries input, double _Threshold, int dMX_len)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_DMX_plusminus(input, _Threshold, dMX_len);
        }
    }
}
#endregion
