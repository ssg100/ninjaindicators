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
	/// <summary>
    /// Gom Delta Volume
    /// </summary>
    [Description("Gom Delta Volume")]
	public class GomDeltaVolume : GomDeltaIndicator
	{
		#region Variables

		int totalvolume, upvolume, downvolume, deltavolume;

		bool Showtotalvolume = true;
		bool Deltamode = true;

		bool Revneg = true;

		#endregion

		protected override void GomInitialize()
		{
			Add(new Plot(new Pen(Color.Green, 3), PlotStyle.Bar, "UpVolume"));
			Add(new Plot(new Pen(Color.Red, 3), PlotStyle.Bar, "DownVolume"));
			Add(new Plot(new Pen(Color.Blue, 3), PlotStyle.Bar, "TotalVolume"));

			Overlay = false;
			PriceTypeSupported = false;
		}


		void PlotChart()
		{
			if (Showtotalvolume)
				TotalVolume.Set(totalvolume);

			if (deltamode)
			{
				if (deltavolume > 0)
				{
					UpVolume.Set(deltavolume);
					DownVolume.Set(0);
				}
				else if (deltavolume < 0)
				{
					UpVolume.Set(0);
					DownVolume.Set((revneg) ? deltavolume : -deltavolume);
				}
				else
				{
					UpVolume.Set(0);
					DownVolume.Set(0);
				}
			}
			else
			{
				UpVolume.Set(upvolume);
				DownVolume.Set(downvolume);
			}


		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void GomOnBarUpdate()
		{
			if (FirstTickOfBar)
			{
				totalvolume = 0;
				upvolume = 0;
				downvolume = 0;
				deltavolume = 0;
			}
		}

		protected override void GomOnMarketData(Gom.MarketDataType e)
		{
			int delta = CalcDelta(e);

			totalvolume += e.Volume;

			if (delta > 0)
				upvolume += delta;
			if (delta < 0)
				downvolume += delta; 

			deltavolume += delta;
		}

		protected override void GomOnBarUpdateDone()
		{
			PlotChart();
		}


		#region Properties
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries TotalVolume
		{
			get { return Values[2]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries UpVolume
		{
			get { return Values[0]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries DownVolume
		{
			get { return Values[1]; }
		}


		[Description("Show Total Volume")]
		[Category("Settings")]
		[Gui.Design.DisplayNameAttribute("Show Total Volume")]
		public bool showtotalvolume
		{
			get { return Showtotalvolume; }
			set { Showtotalvolume = value; }
		}

		[Description("Delta Mode: show askvol-bidvol")]
		[Category("Settings")]
		[Gui.Design.DisplayNameAttribute("Delta Mode")]
		public bool deltamode
		{
			get { return Deltamode; }
			set { Deltamode = value; }
		}

		[Description("Reverse negative delta")]
		[Category("Settings")]
		[Gui.Design.DisplayNameAttribute("Reverse Negative Volume")]
		public bool revneg
		{
			get { return Revneg; }
			set { Revneg = value; }
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
        private GomDeltaVolume[] cacheGomDeltaVolume = null;

        private static GomDeltaVolume checkGomDeltaVolume = new GomDeltaVolume();

        /// <summary>
        /// Gom Delta Volume
        /// </summary>
        /// <returns></returns>
        public GomDeltaVolume GomDeltaVolume()
        {
            return GomDeltaVolume(Input);
        }

        /// <summary>
        /// Gom Delta Volume
        /// </summary>
        /// <returns></returns>
        public GomDeltaVolume GomDeltaVolume(Data.IDataSeries input)
        {
            if (cacheGomDeltaVolume != null)
                for (int idx = 0; idx < cacheGomDeltaVolume.Length; idx++)
                    if (cacheGomDeltaVolume[idx].EqualsInput(input))
                        return cacheGomDeltaVolume[idx];

            lock (checkGomDeltaVolume)
            {
                if (cacheGomDeltaVolume != null)
                    for (int idx = 0; idx < cacheGomDeltaVolume.Length; idx++)
                        if (cacheGomDeltaVolume[idx].EqualsInput(input))
                            return cacheGomDeltaVolume[idx];

                GomDeltaVolume indicator = new GomDeltaVolume();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                GomDeltaVolume[] tmp = new GomDeltaVolume[cacheGomDeltaVolume == null ? 1 : cacheGomDeltaVolume.Length + 1];
                if (cacheGomDeltaVolume != null)
                    cacheGomDeltaVolume.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGomDeltaVolume = tmp;
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
        /// Gom Delta Volume
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomDeltaVolume GomDeltaVolume()
        {
            return _indicator.GomDeltaVolume(Input);
        }

        /// <summary>
        /// Gom Delta Volume
        /// </summary>
        /// <returns></returns>
        public Indicator.GomDeltaVolume GomDeltaVolume(Data.IDataSeries input)
        {
            return _indicator.GomDeltaVolume(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Gom Delta Volume
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomDeltaVolume GomDeltaVolume()
        {
            return _indicator.GomDeltaVolume(Input);
        }

        /// <summary>
        /// Gom Delta Volume
        /// </summary>
        /// <returns></returns>
        public Indicator.GomDeltaVolume GomDeltaVolume(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GomDeltaVolume(input);
        }
    }
}
#endregion
