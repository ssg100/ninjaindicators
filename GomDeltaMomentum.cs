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
	public class GomDeltaMomentum : GomDeltaIndicator
	{
		#region Variables
		// Wizard generated variables
		// User defined variables (add any user defined variables below)

		int curdelta;
		int prevdelta;
		int curbardelta;

		#endregion

		protected override void GomInitialize()
		{
			Add(new Plot(new Pen(Color.Green, 3), PlotStyle.Bar, "UpMomo"));
			Add(new Plot(new Pen(Color.Red, 3), PlotStyle.Bar, "DownMomo"));
			Add(new Plot(Color.Transparent, PlotStyle.Line, "DeltaMomo"));

			CalculateOnBarClose = false;
			Overlay = false;
			PriceTypeSupported = false;
		}

		void PlotChart()
		{
			if (curdelta > 0)
			{
				UpMomo.Set(curdelta);
				DownMomo.Set(0);
			}

			if (curdelta < 0)
			{
				DownMomo.Set(curdelta);
				UpMomo.Set(0);
			}

			DeltaMomo.Set(curdelta);
		}

		protected override void GomOnBarUpdate()
		{
			if (FirstTickOfBar)
			{
				prevdelta = curdelta;
				curbardelta = 0;

				if (curdelta > 0)
					UpMomo.Set(curdelta);

				if (curdelta < 0)
					DownMomo.Set(curdelta);
			}

		}


		protected override void GomOnMarketData(Gom.MarketDataType e)
		{

			curbardelta += CalcDelta(e);

			if (curbardelta > 0)
				curdelta = Math.Max(prevdelta, 0) + curbardelta;
			if (curbardelta < 0)
				curdelta = Math.Min(prevdelta, 0) + curbardelta;

		}

		protected override void GomOnBarUpdateDone()
		{
			PlotChart();
		}


		#region Properties
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries UpMomo
		{
			get { return Values[0]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries DownMomo
		{
			get { return Values[1]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries DeltaMomo
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
        private GomDeltaMomentum[] cacheGomDeltaMomentum = null;

        private static GomDeltaMomentum checkGomDeltaMomentum = new GomDeltaMomentum();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public GomDeltaMomentum GomDeltaMomentum()
        {
            return GomDeltaMomentum(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public GomDeltaMomentum GomDeltaMomentum(Data.IDataSeries input)
        {
            if (cacheGomDeltaMomentum != null)
                for (int idx = 0; idx < cacheGomDeltaMomentum.Length; idx++)
                    if (cacheGomDeltaMomentum[idx].EqualsInput(input))
                        return cacheGomDeltaMomentum[idx];

            lock (checkGomDeltaMomentum)
            {
                if (cacheGomDeltaMomentum != null)
                    for (int idx = 0; idx < cacheGomDeltaMomentum.Length; idx++)
                        if (cacheGomDeltaMomentum[idx].EqualsInput(input))
                            return cacheGomDeltaMomentum[idx];

                GomDeltaMomentum indicator = new GomDeltaMomentum();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                GomDeltaMomentum[] tmp = new GomDeltaMomentum[cacheGomDeltaMomentum == null ? 1 : cacheGomDeltaMomentum.Length + 1];
                if (cacheGomDeltaMomentum != null)
                    cacheGomDeltaMomentum.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGomDeltaMomentum = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomDeltaMomentum GomDeltaMomentum()
        {
            return _indicator.GomDeltaMomentum(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.GomDeltaMomentum GomDeltaMomentum(Data.IDataSeries input)
        {
            return _indicator.GomDeltaMomentum(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomDeltaMomentum GomDeltaMomentum()
        {
            return _indicator.GomDeltaMomentum(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.GomDeltaMomentum GomDeltaMomentum(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GomDeltaMomentum(input);
        }
    }
}
#endregion
