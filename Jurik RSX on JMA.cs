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
		[Description("RSX on JMA on price")]
		public class Jurik_RSX_on_JMA : Indicator
		#endregion
    {
		#region Variables	// default values
			private double j_len = 7;
			private double j_phase = 0; 
			private double r_len = 30; 	
			private double phaseshift = 0.9; 
			private double botLine = -75;	 
			private double topLine = 75;	 
			// ------------------------------------
			private double phasedValue = 0;
			private double RSXvalue = 0;
			private DataSeries PriceSeries;
			#endregion

		#region Input Parameters
			[Description("bot flat line, any value between -100 and +100")]
			[GridCategory("Parameters")]
			public double __BotLine
			{
				get { return botLine; }
				set { botLine = Math.Min(Math.Max(-100, value), 100); }
			}
	
			[Description("top flat line, any value between -100 and +100")]
			[GridCategory("Parameters")]
			public double __TopLine
			{
				get { return topLine; }
				set { topLine = Math.Min(Math.Max(-100, value), 100); }
			}
			
			[Description("JMA length, any value >= 1")]
			[GridCategory("Parameters")]
			public double _JMA_len
			{
				get { return j_len; }
				set { j_len = Math.Max(1, value); }
			}
			
			[Description("JMA phase, any value between -100 and +100")]
			[GridCategory("Parameters")]
			public double _JMA_phase
			{
				get { return j_phase; }
				set { j_phase = Math.Max(-100, Math.Min(100,value)); }
			}
			
			[Description("RSX length, any value >= 2")]
			[GridCategory("Parameters")]
			public double _RSX_len
			{
				get { return r_len; }
				set { r_len = Math.Max(2, value); }
			}
			
			[Description("color phase shifter, any value between 0 and 1")]
			[GridCategory("Parameters")]
			public double PhaseShift
			{
				get { return phaseshift; }
				set { phaseshift =  Math.Min(2, Math.Max(0, value)); }
			}
			#endregion

        protected override void Initialize()
        {
			#region Chart Features

				Add(new Plot(Color.DodgerBlue, PlotStyle.Line, "RSX"));
				Add(new Plot(Color.Gray, PlotStyle.Hash, "Top Line"));
				Add(new Plot(Color.Gray, PlotStyle.Hash, "Bot Line"));
				Add(new Plot(Color.MediumTurquoise, PlotStyle.Bar, "RSX+"));
				Add(new Plot(Color.MediumVioletRed, PlotStyle.Bar, "RSX-"));
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range min"));  // invisible            
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range max"));  // invisible
				Add(new Plot(Color.White, PlotStyle.Line, "phased blend"));  // invisible  
			
				Plots[0].Pen.Width = 2;
				Plots[1].Pen.DashStyle = DashStyle.Dash;
				Plots[2].Pen.DashStyle = DashStyle.Dash;
				Plots[3].Pen.Width = 2;
				Plots[4].Pen.Width = 2;
			
				CalculateOnBarClose	= false;
				Overlay				= false;
				PriceTypeSupported	= false;
				#endregion
						
			#region Series Initialization
				PriceSeries = new DataSeries(this);
				#endregion
        }

        protected override void OnBarUpdate()
        {
			#region Indicator formula
				PriceSeries.Set( ( High[0] + Low[0] + Close[0] )/3 );
				__TopLineSeries.Set( __TopLine );
				__BotLineSeries.Set( __BotLine );
			
				RSXvalue = JurikRSX(JurikJMA(PriceSeries, _JMA_phase, _JMA_len), _RSX_len)[0] ;
				RSXvalue = ( CurrentBar < 31 ? 0 : 2*RSXvalue-100 ) ;
				RSXJMA.Set( RSXvalue ) ;
			
				if (CurrentBar > 0)
					phasedValue = RSXJMA[0] - PhaseShift * RSXJMA[1] ;
		
				RSXRSXpos.Set( phasedValue > 0 ? RSXJMA[0] : 0 ); 
				RSXRSXneg.Set( phasedValue < 0 ? RSXJMA[0] : 0 ); 
				Phased_blend.Set( phasedValue );
				#endregion
			
			#region Panel Stabilizer
				panel_range_min.Set(-1);
				panel_range_max.Set(101);
				#endregion
        }

        #region Output Values	
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries RSXJMA
			{
				get { return Values[0]; }
			}
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries __TopLineSeries
			{
				get { return Values[1]; }
			}
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries __BotLineSeries
			{
				get { return Values[2]; }
			}
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries RSXRSXpos
			{
				get { return Values[3]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries RSXRSXneg
			{
				get { return Values[4]; }
			}

			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_min
			{
				get { return Values[5]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_max
			{
				get { return Values[6]; }
			}
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries Phased_blend
			{
				get { return Values[7]; }
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
        private Jurik_RSX_on_JMA[] cacheJurik_RSX_on_JMA = null;

        private static Jurik_RSX_on_JMA checkJurik_RSX_on_JMA = new Jurik_RSX_on_JMA();

        /// <summary>
        /// RSX on JMA on price
        /// </summary>
        /// <returns></returns>
        public Jurik_RSX_on_JMA Jurik_RSX_on_JMA(double __BotLine, double __TopLine, double _JMA_len, double _JMA_phase, double _RSX_len, double phaseShift)
        {
            return Jurik_RSX_on_JMA(Input, __BotLine, __TopLine, _JMA_len, _JMA_phase, _RSX_len, phaseShift);
        }

        /// <summary>
        /// RSX on JMA on price
        /// </summary>
        /// <returns></returns>
        public Jurik_RSX_on_JMA Jurik_RSX_on_JMA(Data.IDataSeries input, double __BotLine, double __TopLine, double _JMA_len, double _JMA_phase, double _RSX_len, double phaseShift)
        {
            if (cacheJurik_RSX_on_JMA != null)
                for (int idx = 0; idx < cacheJurik_RSX_on_JMA.Length; idx++)
                    if (Math.Abs(cacheJurik_RSX_on_JMA[idx].__BotLine - __BotLine) <= double.Epsilon && Math.Abs(cacheJurik_RSX_on_JMA[idx].__TopLine - __TopLine) <= double.Epsilon && Math.Abs(cacheJurik_RSX_on_JMA[idx]._JMA_len - _JMA_len) <= double.Epsilon && Math.Abs(cacheJurik_RSX_on_JMA[idx]._JMA_phase - _JMA_phase) <= double.Epsilon && Math.Abs(cacheJurik_RSX_on_JMA[idx]._RSX_len - _RSX_len) <= double.Epsilon && Math.Abs(cacheJurik_RSX_on_JMA[idx].PhaseShift - phaseShift) <= double.Epsilon && cacheJurik_RSX_on_JMA[idx].EqualsInput(input))
                        return cacheJurik_RSX_on_JMA[idx];

            lock (checkJurik_RSX_on_JMA)
            {
                checkJurik_RSX_on_JMA.__BotLine = __BotLine;
                __BotLine = checkJurik_RSX_on_JMA.__BotLine;
                checkJurik_RSX_on_JMA.__TopLine = __TopLine;
                __TopLine = checkJurik_RSX_on_JMA.__TopLine;
                checkJurik_RSX_on_JMA._JMA_len = _JMA_len;
                _JMA_len = checkJurik_RSX_on_JMA._JMA_len;
                checkJurik_RSX_on_JMA._JMA_phase = _JMA_phase;
                _JMA_phase = checkJurik_RSX_on_JMA._JMA_phase;
                checkJurik_RSX_on_JMA._RSX_len = _RSX_len;
                _RSX_len = checkJurik_RSX_on_JMA._RSX_len;
                checkJurik_RSX_on_JMA.PhaseShift = phaseShift;
                phaseShift = checkJurik_RSX_on_JMA.PhaseShift;

                if (cacheJurik_RSX_on_JMA != null)
                    for (int idx = 0; idx < cacheJurik_RSX_on_JMA.Length; idx++)
                        if (Math.Abs(cacheJurik_RSX_on_JMA[idx].__BotLine - __BotLine) <= double.Epsilon && Math.Abs(cacheJurik_RSX_on_JMA[idx].__TopLine - __TopLine) <= double.Epsilon && Math.Abs(cacheJurik_RSX_on_JMA[idx]._JMA_len - _JMA_len) <= double.Epsilon && Math.Abs(cacheJurik_RSX_on_JMA[idx]._JMA_phase - _JMA_phase) <= double.Epsilon && Math.Abs(cacheJurik_RSX_on_JMA[idx]._RSX_len - _RSX_len) <= double.Epsilon && Math.Abs(cacheJurik_RSX_on_JMA[idx].PhaseShift - phaseShift) <= double.Epsilon && cacheJurik_RSX_on_JMA[idx].EqualsInput(input))
                            return cacheJurik_RSX_on_JMA[idx];

                Jurik_RSX_on_JMA indicator = new Jurik_RSX_on_JMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.__BotLine = __BotLine;
                indicator.__TopLine = __TopLine;
                indicator._JMA_len = _JMA_len;
                indicator._JMA_phase = _JMA_phase;
                indicator._RSX_len = _RSX_len;
                indicator.PhaseShift = phaseShift;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_RSX_on_JMA[] tmp = new Jurik_RSX_on_JMA[cacheJurik_RSX_on_JMA == null ? 1 : cacheJurik_RSX_on_JMA.Length + 1];
                if (cacheJurik_RSX_on_JMA != null)
                    cacheJurik_RSX_on_JMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_RSX_on_JMA = tmp;
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
        /// RSX on JMA on price
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_RSX_on_JMA Jurik_RSX_on_JMA(double __BotLine, double __TopLine, double _JMA_len, double _JMA_phase, double _RSX_len, double phaseShift)
        {
            return _indicator.Jurik_RSX_on_JMA(Input, __BotLine, __TopLine, _JMA_len, _JMA_phase, _RSX_len, phaseShift);
        }

        /// <summary>
        /// RSX on JMA on price
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_RSX_on_JMA Jurik_RSX_on_JMA(Data.IDataSeries input, double __BotLine, double __TopLine, double _JMA_len, double _JMA_phase, double _RSX_len, double phaseShift)
        {
            return _indicator.Jurik_RSX_on_JMA(input, __BotLine, __TopLine, _JMA_len, _JMA_phase, _RSX_len, phaseShift);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// RSX on JMA on price
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_RSX_on_JMA Jurik_RSX_on_JMA(double __BotLine, double __TopLine, double _JMA_len, double _JMA_phase, double _RSX_len, double phaseShift)
        {
            return _indicator.Jurik_RSX_on_JMA(Input, __BotLine, __TopLine, _JMA_len, _JMA_phase, _RSX_len, phaseShift);
        }

        /// <summary>
        /// RSX on JMA on price
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_RSX_on_JMA Jurik_RSX_on_JMA(Data.IDataSeries input, double __BotLine, double __TopLine, double _JMA_len, double _JMA_phase, double _RSX_len, double phaseShift)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_RSX_on_JMA(input, __BotLine, __TopLine, _JMA_len, _JMA_phase, _RSX_len, phaseShift);
        }
    }
}
#endregion
