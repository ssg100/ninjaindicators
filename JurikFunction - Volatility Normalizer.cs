// ======================================================
// NinjaTrader module by Jurik Research Software
// © 2010 Jurik Research   ;   www.jurikres.com
// ======================================================
//
// FUNCTION -- Data Series Volatility Normalizer
//
// ******* THIS IS A FUNCTION, NOT AN INDICATOR *******
//
// ======================================================

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

namespace NinjaTrader.Indicator
{
	#region Header
		[Description("NOT AN INDICATOR -- DO NOT USE TO PLOT")]
		public class JurikFunction_volat_norm : Indicator
		#endregion
    {
        #region Variables	// default values
			private int startBarNumber = 50;  
		    // ----------------------------------
			private double summ = 0;
			private double normalizer = 0;
			private DataSeries AbsDiff;
			#endregion
		
		#region Input Parameters
			[Description("Begins normalizing at this bar number. Recommend using 50.")]
			[GridCategory("Parameters")]
			public int StartBarNumber
			{
				get { return startBarNumber; }
				set { startBarNumber = Math.Max(1, value); }
			}
			#endregion
		
        protected override void Initialize()
        {	
			#region Series Initialization
				AbsDiff 		= new DataSeries(this);
				#endregion
        }

        protected override void OnBarUpdate()
        {
			#region Function Formula
			
				if (CurrentBar <= StartBarNumber)
				{
					Value.Set(0);
					AbsDiff.Set(CurrentBar == 0 ? 0 : Math.Abs(Input[0]-Input[1]));
				}
				
				if (CurrentBar == StartBarNumber)
				{
					summ = 0;
					for (int idx = StartBarNumber; idx >=0; idx--)
						summ += AbsDiff[idx];
					normalizer = summ / (1+StartBarNumber);
				}
			
				if (CurrentBar >= StartBarNumber)
					Value.Set(Input[0] / normalizer ) ;

				#endregion
        }
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private JurikFunction_volat_norm[] cacheJurikFunction_volat_norm = null;

        private static JurikFunction_volat_norm checkJurikFunction_volat_norm = new JurikFunction_volat_norm();

        /// <summary>
        /// NOT AN INDICATOR -- DO NOT USE TO PLOT
        /// </summary>
        /// <returns></returns>
        public JurikFunction_volat_norm JurikFunction_volat_norm(int startBarNumber)
        {
            return JurikFunction_volat_norm(Input, startBarNumber);
        }

        /// <summary>
        /// NOT AN INDICATOR -- DO NOT USE TO PLOT
        /// </summary>
        /// <returns></returns>
        public JurikFunction_volat_norm JurikFunction_volat_norm(Data.IDataSeries input, int startBarNumber)
        {
            if (cacheJurikFunction_volat_norm != null)
                for (int idx = 0; idx < cacheJurikFunction_volat_norm.Length; idx++)
                    if (cacheJurikFunction_volat_norm[idx].StartBarNumber == startBarNumber && cacheJurikFunction_volat_norm[idx].EqualsInput(input))
                        return cacheJurikFunction_volat_norm[idx];

            lock (checkJurikFunction_volat_norm)
            {
                checkJurikFunction_volat_norm.StartBarNumber = startBarNumber;
                startBarNumber = checkJurikFunction_volat_norm.StartBarNumber;

                if (cacheJurikFunction_volat_norm != null)
                    for (int idx = 0; idx < cacheJurikFunction_volat_norm.Length; idx++)
                        if (cacheJurikFunction_volat_norm[idx].StartBarNumber == startBarNumber && cacheJurikFunction_volat_norm[idx].EqualsInput(input))
                            return cacheJurikFunction_volat_norm[idx];

                JurikFunction_volat_norm indicator = new JurikFunction_volat_norm();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.StartBarNumber = startBarNumber;
                Indicators.Add(indicator);
                indicator.SetUp();

                JurikFunction_volat_norm[] tmp = new JurikFunction_volat_norm[cacheJurikFunction_volat_norm == null ? 1 : cacheJurikFunction_volat_norm.Length + 1];
                if (cacheJurikFunction_volat_norm != null)
                    cacheJurikFunction_volat_norm.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurikFunction_volat_norm = tmp;
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
        /// NOT AN INDICATOR -- DO NOT USE TO PLOT
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JurikFunction_volat_norm JurikFunction_volat_norm(int startBarNumber)
        {
            return _indicator.JurikFunction_volat_norm(Input, startBarNumber);
        }

        /// <summary>
        /// NOT AN INDICATOR -- DO NOT USE TO PLOT
        /// </summary>
        /// <returns></returns>
        public Indicator.JurikFunction_volat_norm JurikFunction_volat_norm(Data.IDataSeries input, int startBarNumber)
        {
            return _indicator.JurikFunction_volat_norm(input, startBarNumber);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// NOT AN INDICATOR -- DO NOT USE TO PLOT
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JurikFunction_volat_norm JurikFunction_volat_norm(int startBarNumber)
        {
            return _indicator.JurikFunction_volat_norm(Input, startBarNumber);
        }

        /// <summary>
        /// NOT AN INDICATOR -- DO NOT USE TO PLOT
        /// </summary>
        /// <returns></returns>
        public Indicator.JurikFunction_volat_norm JurikFunction_volat_norm(Data.IDataSeries input, int startBarNumber)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.JurikFunction_volat_norm(input, startBarNumber);
        }
    }
}
#endregion
