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
		[Description("customizable DMX")]
		public class Jurik_DMX_custom : Indicator
		#endregion
    {
        #region Variables	// default values
            private int dmx_len = 50 ; 
			private double botLine = -50; 
			private double topLine = 50;
			// ---------------------------
			private double DMX_value = 0;
        	#endregion
		
		#region Input Parameters
			[Description("bot flat line, any value between -100 and +100")]
        	[Category("Parameters")]
        	public double _BotLine
        	{
        	    get { return botLine; }
        	    set { botLine = Math.Min(Math.Max(-100, value), 100); }
        	}
				
			[Description("top flat line, any value between -100 and +100")]
        	[Category("Parameters")]
        	public double _TopLine
        	{
        	    get { return topLine; }
        	    set { topLine = Math.Min(Math.Max(-100, value), 100); }
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
				Add(new Plot(Color.Orange, PlotStyle.Line, "DMX"));
				Add(new Plot(Color.Black, PlotStyle.Hash, "Top Line"));
				Add(new Plot(Color.Black, PlotStyle.Hash, "Bot Line"));
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range min"));            
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range max"));
			
				Plots[0].Pen.Width = 2;
				Plots[1].Pen.DashStyle = DashStyle.Dash;
				Plots[2].Pen.DashStyle = DashStyle.Dash;
				
				Add(new Line(Color.Gray, 0, "Zero Line"));
				Lines[0].Pen.Width = 1;
	
				CalculateOnBarClose	= false;
				Overlay				= false;
				PriceTypeSupported	= false;
				#endregion
		}

        protected override void OnBarUpdate()
        {
			#region Indicator formula
				_TopLineSeries.Set(_TopLine);
				_BotLineSeries.Set(_BotLine);
			
				DMX_value = JurikDMX(DMX_len).DMXValue[0];
				DMX_series.Set( CurrentBar >= 50 ? DMX_value : 0);
				#endregion
			
			#region Panel Stabilizer
				panel_range_min.Set(-101);
				panel_range_max.Set(101);
				#endregion
        }

        #region Output Values
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries DMX_series
			{
				get { return Values[0]; }
			}
						
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries _TopLineSeries
			{
				get { return Values[1]; }
			}
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries _BotLineSeries
			{
				get { return Values[2]; }
			}
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_min
			{
				get { return Values[3]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_max
			{
				get { return Values[4]; }
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
        private Jurik_DMX_custom[] cacheJurik_DMX_custom = null;

        private static Jurik_DMX_custom checkJurik_DMX_custom = new Jurik_DMX_custom();

        /// <summary>
        /// customizable DMX
        /// </summary>
        /// <returns></returns>
        public Jurik_DMX_custom Jurik_DMX_custom(double _BotLine, double _TopLine, int dMX_len)
        {
            return Jurik_DMX_custom(Input, _BotLine, _TopLine, dMX_len);
        }

        /// <summary>
        /// customizable DMX
        /// </summary>
        /// <returns></returns>
        public Jurik_DMX_custom Jurik_DMX_custom(Data.IDataSeries input, double _BotLine, double _TopLine, int dMX_len)
        {
            if (cacheJurik_DMX_custom != null)
                for (int idx = 0; idx < cacheJurik_DMX_custom.Length; idx++)
                    if (Math.Abs(cacheJurik_DMX_custom[idx]._BotLine - _BotLine) <= double.Epsilon && Math.Abs(cacheJurik_DMX_custom[idx]._TopLine - _TopLine) <= double.Epsilon && cacheJurik_DMX_custom[idx].DMX_len == dMX_len && cacheJurik_DMX_custom[idx].EqualsInput(input))
                        return cacheJurik_DMX_custom[idx];

            lock (checkJurik_DMX_custom)
            {
                checkJurik_DMX_custom._BotLine = _BotLine;
                _BotLine = checkJurik_DMX_custom._BotLine;
                checkJurik_DMX_custom._TopLine = _TopLine;
                _TopLine = checkJurik_DMX_custom._TopLine;
                checkJurik_DMX_custom.DMX_len = dMX_len;
                dMX_len = checkJurik_DMX_custom.DMX_len;

                if (cacheJurik_DMX_custom != null)
                    for (int idx = 0; idx < cacheJurik_DMX_custom.Length; idx++)
                        if (Math.Abs(cacheJurik_DMX_custom[idx]._BotLine - _BotLine) <= double.Epsilon && Math.Abs(cacheJurik_DMX_custom[idx]._TopLine - _TopLine) <= double.Epsilon && cacheJurik_DMX_custom[idx].DMX_len == dMX_len && cacheJurik_DMX_custom[idx].EqualsInput(input))
                            return cacheJurik_DMX_custom[idx];

                Jurik_DMX_custom indicator = new Jurik_DMX_custom();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator._BotLine = _BotLine;
                indicator._TopLine = _TopLine;
                indicator.DMX_len = dMX_len;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_DMX_custom[] tmp = new Jurik_DMX_custom[cacheJurik_DMX_custom == null ? 1 : cacheJurik_DMX_custom.Length + 1];
                if (cacheJurik_DMX_custom != null)
                    cacheJurik_DMX_custom.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_DMX_custom = tmp;
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
        /// customizable DMX
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_DMX_custom Jurik_DMX_custom(double _BotLine, double _TopLine, int dMX_len)
        {
            return _indicator.Jurik_DMX_custom(Input, _BotLine, _TopLine, dMX_len);
        }

        /// <summary>
        /// customizable DMX
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_DMX_custom Jurik_DMX_custom(Data.IDataSeries input, double _BotLine, double _TopLine, int dMX_len)
        {
            return _indicator.Jurik_DMX_custom(input, _BotLine, _TopLine, dMX_len);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// customizable DMX
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_DMX_custom Jurik_DMX_custom(double _BotLine, double _TopLine, int dMX_len)
        {
            return _indicator.Jurik_DMX_custom(Input, _BotLine, _TopLine, dMX_len);
        }

        /// <summary>
        /// customizable DMX
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_DMX_custom Jurik_DMX_custom(Data.IDataSeries input, double _BotLine, double _TopLine, int dMX_len)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_DMX_custom(input, _BotLine, _TopLine, dMX_len);
        }
    }
}
#endregion
