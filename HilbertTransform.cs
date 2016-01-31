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
    [Description("Enter the description of your new custom indicator here")]
    public class HilbertTransform : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int wMAPeriods = 10; // Default setting for WMAPeriods
        // User defined variables (add any user defined variables below)		
		private DataSeries Smooth;
		private DataSeries Detrender;
		private DataSeries I1;
		private DataSeries Q1;		
		private DataSeries jI;
		private DataSeries jQ;		
		private DataSeries I2;
		private DataSeries Q2;
		private DataSeries Re;
		private DataSeries Im;				
		private DataSeries Period;		
		private DataSeries SmoothPeriod;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "InPhase"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "Quadrature"));
			//Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "Period"));
            Add(new Line(Color.FromKnownColor(KnownColor.WindowText), 0, "ZeroLine"));
            CalculateOnBarClose	= true;
            Overlay				= false;
            PriceTypeSupported	= false;
			
			Smooth = new DataSeries(this);
			Detrender = new DataSeries(this);
			I1 = new DataSeries(this);
			Q1 = new DataSeries(this);
			jI = new DataSeries(this);
			jQ = new DataSeries(this);
			I2 = new DataSeries(this);
			Q2 = new DataSeries(this);
			Re = new DataSeries(this);
			Im = new DataSeries(this);			
			Period = new DataSeries(this);
			SmoothPeriod = new DataSeries(this);
			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if( this.CurrentBar<50)
				return;
						
			Smooth.Set((4*Median[0] + 3*Median[1] + 2*Median[2] + Median[3])/10);
			Detrender.Set((0.0962*Smooth[0]+0.5769*Smooth[2]-0.5769*Smooth[4]-0.0962*Smooth[6])*(0.075*Period[1]+.54));
			
			
			//InPhase and Quadrature components			
			Q1.Set((0.0962*Detrender[0]+0.5769*Detrender[2]-0.5769*Detrender[4]-0.0962*Detrender[6])*(0.075*Period[1]+0.54));			
			I1.Set(Detrender[3]);
			
			//Advance the phase of I1 and Q1 by 90 degrees
			jI.Set((0.0962*I1[0]+0.5769*I1[2]-0.5769*I1[4]-0.0962*I1[6])*(0.075*Period[1]+.54));
			jQ.Set((0.0962*Q1[0]+0.5769*Q1[2]-0.5769*Q1[4]-0.0962*Q1[6])*(0.075*Period[1]+.54));
			
			//Phasor Addition
			I2.Set(I1[0]-jQ[0]);
			Q2.Set(Q1[0]+jI[0]);
			
			//Smooth the I and Q components before applying the discriminator
			I2.Set(0.2*I2[0]+0.8*I2[1]);
			Q2.Set(0.2*Q2[0]+0.8*Q2[1]);
			
			//Homodyne Discriminator
			Re.Set(I2[0]*I2[1] + Q2[0]*Q2[1]);
			Im.Set(I2[0]*Q2[1] - Q2[0]*I2[1]);
			Re.Set(0.2*Re[0] + 0.8*Re[1]);
			Im.Set(0.2*Im[0] + 0.8*Im[1]);
			
			double rad2Deg = 180.0 / (4.0 * Math.Atan (1));
			
			if(Im[0]!=0 && Re[0]!=0)
				Period.Set(360/(Math.Atan(Im[0]/Re[0])*rad2Deg ));
			
			if(Period[0]>(1.5*Period[1]))
				Period.Set(1.5*Period[1]);
			
			if(Period[0]<(0.67*Period[1]))
				Period.Set(0.67*Period[1]);
			
			if(Period[0]<6)
				Period.Set(6);
			
			if(Period[0]>50)
				Period.Set(50);
			
			Period.Set(0.2*Period[0] + 0.8*Period[1]);
			SmoothPeriod.Set(0.33*Period[0] + 0.67*SmoothPeriod[1]);
			
			
			InPhase.Set(I1[0]);
			Quadrature.Set(Q1[0]);			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries InPhase
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Quadrature
        {
            get { return Values[1]; }
        }
		
		[Browsable(false)]
		public DataSeries CycleSmoothPeriod{
			get{ return SmoothPeriod; }
		}
		
		[Browsable(false)]
		public DataSeries CyclePeriod{
			get{ return Period; }
		}
		
        [Description("")]
        [Category("Parameters")]
        public int WMAPeriods
        {
            get { return wMAPeriods; }
            set { wMAPeriods = Math.Max(3, value); }
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
        private HilbertTransform[] cacheHilbertTransform = null;

        private static HilbertTransform checkHilbertTransform = new HilbertTransform();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public HilbertTransform HilbertTransform(int wMAPeriods)
        {
            return HilbertTransform(Input, wMAPeriods);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public HilbertTransform HilbertTransform(Data.IDataSeries input, int wMAPeriods)
        {
            if (cacheHilbertTransform != null)
                for (int idx = 0; idx < cacheHilbertTransform.Length; idx++)
                    if (cacheHilbertTransform[idx].WMAPeriods == wMAPeriods && cacheHilbertTransform[idx].EqualsInput(input))
                        return cacheHilbertTransform[idx];

            lock (checkHilbertTransform)
            {
                checkHilbertTransform.WMAPeriods = wMAPeriods;
                wMAPeriods = checkHilbertTransform.WMAPeriods;

                if (cacheHilbertTransform != null)
                    for (int idx = 0; idx < cacheHilbertTransform.Length; idx++)
                        if (cacheHilbertTransform[idx].WMAPeriods == wMAPeriods && cacheHilbertTransform[idx].EqualsInput(input))
                            return cacheHilbertTransform[idx];

                HilbertTransform indicator = new HilbertTransform();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.WMAPeriods = wMAPeriods;
                Indicators.Add(indicator);
                indicator.SetUp();

                HilbertTransform[] tmp = new HilbertTransform[cacheHilbertTransform == null ? 1 : cacheHilbertTransform.Length + 1];
                if (cacheHilbertTransform != null)
                    cacheHilbertTransform.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheHilbertTransform = tmp;
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
        public Indicator.HilbertTransform HilbertTransform(int wMAPeriods)
        {
            return _indicator.HilbertTransform(Input, wMAPeriods);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.HilbertTransform HilbertTransform(Data.IDataSeries input, int wMAPeriods)
        {
            return _indicator.HilbertTransform(input, wMAPeriods);
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
        public Indicator.HilbertTransform HilbertTransform(int wMAPeriods)
        {
            return _indicator.HilbertTransform(Input, wMAPeriods);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.HilbertTransform HilbertTransform(Data.IDataSeries input, int wMAPeriods)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.HilbertTransform(input, wMAPeriods);
        }
    }
}
#endregion
