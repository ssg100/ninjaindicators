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
    /// Plot dot or line where the traderclutch swing is
    /// </summary>
    [Description("Plot dot or line where the traderclutch swing is")]
    public class ZZSwingIdentifierLow : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int swingSize = 4; 		// Default setting for SwingSize
			private DataSeries dsSwingPoint; 	// Define a DataSeries variable
			private bool searchingUp = true;			// search direction
		    private double markedBarLow = 0;	// keep current bar low
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Dot, "SwingPointHigh"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Dot, "SwingPointLow"));
            Overlay				= true;
			dsSwingPoint = new DataSeries(this);
			markedBarLow = 1200;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// algo:
			// mark current low bar
			// every new bar:
			// (if searching up)
			// if current low > marked low, then mark it
			// else   that means low of bar is lower
			//    if diff > 4 then current dsSwingPoint is found, set it as low[0] of marked bar
			//    searchingUp = false
			// 
			// else (seraching down)
			//    if current low < marked low, then mark it 
			//    else
			//       if diff> swingsize then current dsSwingpoint is found, set it as low[0] of marked bar
			//       searchingUp = true
			dsSwingPoint.Set( 0 );
			//Value.Set( 0 );
			
			if (searchingUp)
			{
				if(	Low[0] > markedBarLow )
					markedBarLow = Low[0];
				else
				{
					// current bar is lower
					if( (Math.Abs(markedBarLow - Low[0])) >= swingSize * TickSize )
					{
						dsSwingPoint.Set( markedBarLow );
						SwingPointHigh.Set( markedBarLow );
						searchingUp = false;
						markedBarLow = Low[0];
						
					}
				}
			}
			else
			{
				if(	Low[0] < markedBarLow )
					markedBarLow = Low[0];
				else
				{
					// current bar is lower
					if( (Math.Abs(markedBarLow - Low[0])) >= swingSize * TickSize )
					{
						dsSwingPoint.Set( markedBarLow );
						SwingPointLow.Set( markedBarLow );
						searchingUp = true;
						Value.Set( markedBarLow );
						markedBarLow = Low[0];
						
					}
				}				
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SwingPointHigh
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SwingPointLow
        {
            get { return Values[1]; }
        }

        [Description("Swing size in ticks")]
        [GridCategory("Parameters")]
        public int SwingSize
        {
            get { return swingSize; }
            set { swingSize = Math.Max(4, value); }
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
        private ZZSwingIdentifierLow[] cacheZZSwingIdentifierLow = null;

        private static ZZSwingIdentifierLow checkZZSwingIdentifierLow = new ZZSwingIdentifierLow();

        /// <summary>
        /// Plot dot or line where the traderclutch swing is
        /// </summary>
        /// <returns></returns>
        public ZZSwingIdentifierLow ZZSwingIdentifierLow(int swingSize)
        {
            return ZZSwingIdentifierLow(Input, swingSize);
        }

        /// <summary>
        /// Plot dot or line where the traderclutch swing is
        /// </summary>
        /// <returns></returns>
        public ZZSwingIdentifierLow ZZSwingIdentifierLow(Data.IDataSeries input, int swingSize)
        {
            if (cacheZZSwingIdentifierLow != null)
                for (int idx = 0; idx < cacheZZSwingIdentifierLow.Length; idx++)
                    if (cacheZZSwingIdentifierLow[idx].SwingSize == swingSize && cacheZZSwingIdentifierLow[idx].EqualsInput(input))
                        return cacheZZSwingIdentifierLow[idx];

            lock (checkZZSwingIdentifierLow)
            {
                checkZZSwingIdentifierLow.SwingSize = swingSize;
                swingSize = checkZZSwingIdentifierLow.SwingSize;

                if (cacheZZSwingIdentifierLow != null)
                    for (int idx = 0; idx < cacheZZSwingIdentifierLow.Length; idx++)
                        if (cacheZZSwingIdentifierLow[idx].SwingSize == swingSize && cacheZZSwingIdentifierLow[idx].EqualsInput(input))
                            return cacheZZSwingIdentifierLow[idx];

                ZZSwingIdentifierLow indicator = new ZZSwingIdentifierLow();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.SwingSize = swingSize;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZSwingIdentifierLow[] tmp = new ZZSwingIdentifierLow[cacheZZSwingIdentifierLow == null ? 1 : cacheZZSwingIdentifierLow.Length + 1];
                if (cacheZZSwingIdentifierLow != null)
                    cacheZZSwingIdentifierLow.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZSwingIdentifierLow = tmp;
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
        /// Plot dot or line where the traderclutch swing is
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZSwingIdentifierLow ZZSwingIdentifierLow(int swingSize)
        {
            return _indicator.ZZSwingIdentifierLow(Input, swingSize);
        }

        /// <summary>
        /// Plot dot or line where the traderclutch swing is
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZSwingIdentifierLow ZZSwingIdentifierLow(Data.IDataSeries input, int swingSize)
        {
            return _indicator.ZZSwingIdentifierLow(input, swingSize);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plot dot or line where the traderclutch swing is
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZSwingIdentifierLow ZZSwingIdentifierLow(int swingSize)
        {
            return _indicator.ZZSwingIdentifierLow(Input, swingSize);
        }

        /// <summary>
        /// Plot dot or line where the traderclutch swing is
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZSwingIdentifierLow ZZSwingIdentifierLow(Data.IDataSeries input, int swingSize)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZSwingIdentifierLow(input, swingSize);
        }
    }
}
#endregion
