#region Using declarations
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using System.Runtime.InteropServices; // DLL support
using System.Collections.Generic;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{

    /// <summary>
    /// VPIN indicator
    /// </summary>
    [Description("VPIN indicator")]
    public class ZZVPINwithMinuteBar : Indicator
    {
		//public static extern void Analyze();
        #region Variables
        // Wizard generated variables
            private int volBucketSize = 31999; // Default setting for VolBucketSize (V variable)
            private int sampleSize = 30; // Default setting for SampleSize
			private int nWindowsize = 50;
			private int cdfsamplesize = 50;
			private DataSeries	deltaPrice;
			private DataSeries	vpinDataseries;
			private DataSeries	deltaPdivSigmaP;
		
			private double avgVpin = 0;
			//private DataSeries  stdDev;
			private bool useRealtimeVol = false;
			private long lastTradeVol = 0;
			private double accumVol = 0;
			private long n = 0;   // n = number of samples
			private int currentBarSaved = 0;
			private int timeBarCounter = 5;  //counter for num of time bars inside prev vol bucket
			private int numTimeBarLastBucket = 6;	// use this to 
			/// </summary>
			private bool bucketFull = false;	// flag if bucket is full
			private double VolCounter = 0;
			private double VolBuy = 0;
			private double VolSell = 0;	//Vs
			private double cummDeltaVsVB = 0;  // |Vs - VB|
			private double VPINvalue = 0;			// VPIN final value
			private Queue deltaVol;
			private Queue queueAvgVpin;
			private double vpintemp=0;
			//private Array deltaVolArray;
			private bool initialBucketFull = false; // one time flag indicating first bucket full for the Queue
		
        // User defined variables (add any user defined variables below)
        #endregion
		//public static extern void Mean();
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			//Array deltaVolArray=Array.CreateInstance( typeof(Double), sampleSize+1 );

			deltaVol = new Queue();
			queueAvgVpin = new Queue();
			deltaPrice	= new DataSeries(this, MaximumBarsLookBack.Infinite); 
			deltaPdivSigmaP = new DataSeries(this, MaximumBarsLookBack.Infinite); 
			vpinDataseries	= new DataSeries(this, MaximumBarsLookBack.Infinite);
			//stdDev		= new DataSeries(this, MaximumBarsLookBack.Infinite);
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "VPIN"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "CDFofVPIN"));
            Overlay				= true;
			if(useRealtimeVol)
				CalculateOnBarClose = false;
			else
				CalculateOnBarClose = true;
			//stats = new Simulator(this);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double squared = 0;
			double mean = 0;
			double sumDeltaVol = 0;
			double stddevtemp=0;
			double normdisttemp=0;
			double sumOfDerivation = 0;  
			double stddevVpin=0;
			double sumOfDerivationAverage=0;
			
			//Print("Currentbar   = " + CurrentBar);
			
			if(useRealtimeVol)
			{
				if(Historical)
					return;
			}
			

            if (CurrentBar < 205) 
			{
				return;
			}
			
			
			// CALCULATE EACH BAR 
			// then, if over V > 30000 , then basket full, CALCULATE VPIN
			
			deltaPrice[0] = Close[0] - Close[1];
			
			stddevtemp = StdDev(deltaPrice, cdfsamplesize)[0];
			deltaPdivSigmaP[0] = deltaPrice[0]/stddevtemp;
				
			normdisttemp = NORMDIST( deltaPdivSigmaP[0], SMA(deltaPdivSigmaP, cdfsamplesize)[0], StdDev(deltaPdivSigmaP, cdfsamplesize)[0], true);
			//normdisttemp = NORMDIST( deltaPdivSigmaP[0], 0, 1, true);
			//Print("normdist = " + normdisttemp );

			VolBuy = VolBuy + (Volume[0] * normdisttemp  );
			VolSell = VolSell + ( Volume[0] * (1 - normdisttemp ) );
			//Print("VolBuy = " + VolBuy);
			//Print("VolSell = " + VolSell);
				
			// keep track of the volume bucket
			// if over V, then calculate vpin
				
			VolCounter = VolCounter + Volume[0];

			vpinDataseries[0] = VPINvalue;
			
			// V bucket is accomplished, VPIN then calculated (every completed bucket)
			if( VolCounter > volBucketSize ) {
				//Print("===== new bucket, vol = " + VolCounter);
				
				// VPIN formula sigma over 50 buckets... Delta |Vs - Vb|  /  nV
				// so accummulate delta |Vs -Vb| every completed bucket
				//cummDeltaVsVB = cummDeltaVsVB + Math.Abs(VolSell - VolBuy);
				
				deltaVol.Enqueue( (double) Math.Abs(VolSell - VolBuy) );
				
				if( deltaVol.Count >=sampleSize ) {
					
					// calc vpin
					foreach (double value in deltaVol)
					{
						 cummDeltaVsVB = cummDeltaVsVB + value;
						
	   					 //Print( " Value in each deltaVol = " + value);
					}
					
					// Calculate now, VPIN = sigma cummulative deltaVsVb / nV
					VPINvalue = cummDeltaVsVB / (sampleSize * VolCounter);
					//Print("VPINvalue " + VPINvalue);
					
					queueAvgVpin.Enqueue( (double) VPINvalue );
					if( queueAvgVpin.Count >= sampleSize ){
						foreach (double value in queueAvgVpin)
						{
							avgVpin = avgVpin + value;
							sumOfDerivation += (value) * (value); 
							
						}
						sumOfDerivationAverage = sumOfDerivation / queueAvgVpin.Count;
						
						avgVpin = avgVpin/queueAvgVpin.Count;
						stddevVpin = Math.Sqrt(sumOfDerivationAverage - (avgVpin*avgVpin));
						
						queueAvgVpin.Dequeue();
					}
						
					// clear vars
					cummDeltaVsVB = 0;
					deltaVol.Dequeue(  );
					
					
				}
				
				vpinDataseries[0] = VPINvalue;	
				VolCounter = 0;
				VolBuy = 0;
				VolSell = 0;
				//Print("avgVpin " + avgVpin);
				//Print("stddevVpin " + stddevVpin);
				vpintemp=NORMDIST( (vpinDataseries[0]), avgVpin, stddevVpin, true);
				
				avgVpin=0;
				
				//if( (Time[0].Month == 8) && Time[0].Year == 2011 ){
				//	Print(Time[0].Month + "/" + Time[0].Day + " " + Time[0].TimeOfDay + "	" + Close[0] + "	" + vpinDataseries[0] + "	" + vpintemp);
				//}
			}
			
			

			
			

			//vpintemp=NORMDIST( (vpinDataseries[0]), 0, 1, true);
//Print("vpintemp " + vpintemp);
			//Print("vpinDataseries[0] " + vpinDataseries[0]);
			//Print("SMA(vpinDataseries,20)[0]  "+SMA(vpinDataseries,20)[0]);
			// PLOT 
			
			VPIN.Set(vpinDataseries[0]);
           CDFofVPIN.Set(vpintemp)  ;
			
			
			
        }
		


        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries VPIN
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries CDFofVPIN
        {
            get { return Values[1]; }
        }

        [Description("V")]
        [GridCategory("Parameters")]
        public int VolBucketSize
        {
            get { return volBucketSize; }
            set { volBucketSize = Math.Max(1000, value); }
        }

        [Description("n")]
        [GridCategory("Parameters")]
        public int SampleSize
        {
            get { return sampleSize; }
            set { sampleSize = Math.Max(1, value); }
        }
		
        [Description("cdfsamplesize")]
        [GridCategory("Parameters")]
        public int Cdfsamplesize
        {
            get { return cdfsamplesize; }
            set { cdfsamplesize = Math.Max(1, value); }
        }
		
        #endregion
    
	
 	#region Miscellaneous
/// <summary>The normdist.</summary>
        /// <param name="x">The x.</param>
        /// <param name="mean">The mean.</param>
        /// <param name="std">The std. </param>
        /// <param name="cumulative">The cumulative.</param>
        /// <returns>The normdist.</returns>
        private static double NORMDIST(double x, double mean, double std, bool cumulative)
        {
            if (cumulative)
            {
                return Phi(x, mean, std);
            }

            var tmp = 1/(Math.Sqrt(2*Math.PI)*std);
            return tmp*Math.Exp(-.5*Math.Pow((x - mean)/std, 2));
        }

        // from http://www.cs.princeton.edu/introcs/...Math.java.html
        // fractional error less than 1.2 * 10 ^ -7.

        // cumulative normal distribution
        /// <summary>The phi.</summary>
        /// <param name="z">The z.</param>
        /// <returns>The phi.</returns>
        private static double Phi(double z)
        {
            return 0.5*(1.0 + erf(z/Math.Sqrt(2.0)));
        }

        // cumulative normal distribution with mean mu and std deviation sigma
        /// <summary>The phi.</summary>
        /// <param name="z">The z.</param>
        /// <param name="mu">The mu.</param>
        /// <param name="sigma">The sigma.</param>
        /// <returns>The phi.</returns>
        private static double Phi(double z, double mu, double sigma)
        {
            return Phi((z - mu)/sigma);
        }

        /// <summary>The erf.</summary>
        /// <param name="z">The z.</param>
        /// <returns>The erf.</returns>
        private static double erf(double z)
        {
            var t = 1.0/(1.0 + 0.5*Math.Abs(z));

            // use Horner's method
            var ans = 1 -
                      t*
                      Math.Exp(
                          -z*z - 1.26551223 +
                          t*
                          (1.00002368 +
                           t*
                           (0.37409196 +
                            t*
                            (0.09678418 +
                             t*
                             (-0.18628806 +
                              t*
                              (0.27886807 +
                               t*(-1.13520398 + t*(1.48851587 + t*(-0.82215223 + t*0.17087277)))))))));
            if (z >= 0)
            {
                return ans;
            }

            return -ans;
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
        private ZZVPINwithMinuteBar[] cacheZZVPINwithMinuteBar = null;

        private static ZZVPINwithMinuteBar checkZZVPINwithMinuteBar = new ZZVPINwithMinuteBar();

        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        public ZZVPINwithMinuteBar ZZVPINwithMinuteBar(int cdfsamplesize, int sampleSize, int volBucketSize)
        {
            return ZZVPINwithMinuteBar(Input, cdfsamplesize, sampleSize, volBucketSize);
        }

        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        public ZZVPINwithMinuteBar ZZVPINwithMinuteBar(Data.IDataSeries input, int cdfsamplesize, int sampleSize, int volBucketSize)
        {
            if (cacheZZVPINwithMinuteBar != null)
                for (int idx = 0; idx < cacheZZVPINwithMinuteBar.Length; idx++)
                    if (cacheZZVPINwithMinuteBar[idx].Cdfsamplesize == cdfsamplesize && cacheZZVPINwithMinuteBar[idx].SampleSize == sampleSize && cacheZZVPINwithMinuteBar[idx].VolBucketSize == volBucketSize && cacheZZVPINwithMinuteBar[idx].EqualsInput(input))
                        return cacheZZVPINwithMinuteBar[idx];

            lock (checkZZVPINwithMinuteBar)
            {
                checkZZVPINwithMinuteBar.Cdfsamplesize = cdfsamplesize;
                cdfsamplesize = checkZZVPINwithMinuteBar.Cdfsamplesize;
                checkZZVPINwithMinuteBar.SampleSize = sampleSize;
                sampleSize = checkZZVPINwithMinuteBar.SampleSize;
                checkZZVPINwithMinuteBar.VolBucketSize = volBucketSize;
                volBucketSize = checkZZVPINwithMinuteBar.VolBucketSize;

                if (cacheZZVPINwithMinuteBar != null)
                    for (int idx = 0; idx < cacheZZVPINwithMinuteBar.Length; idx++)
                        if (cacheZZVPINwithMinuteBar[idx].Cdfsamplesize == cdfsamplesize && cacheZZVPINwithMinuteBar[idx].SampleSize == sampleSize && cacheZZVPINwithMinuteBar[idx].VolBucketSize == volBucketSize && cacheZZVPINwithMinuteBar[idx].EqualsInput(input))
                            return cacheZZVPINwithMinuteBar[idx];

                ZZVPINwithMinuteBar indicator = new ZZVPINwithMinuteBar();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Cdfsamplesize = cdfsamplesize;
                indicator.SampleSize = sampleSize;
                indicator.VolBucketSize = volBucketSize;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZVPINwithMinuteBar[] tmp = new ZZVPINwithMinuteBar[cacheZZVPINwithMinuteBar == null ? 1 : cacheZZVPINwithMinuteBar.Length + 1];
                if (cacheZZVPINwithMinuteBar != null)
                    cacheZZVPINwithMinuteBar.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZVPINwithMinuteBar = tmp;
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
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZVPINwithMinuteBar ZZVPINwithMinuteBar(int cdfsamplesize, int sampleSize, int volBucketSize)
        {
            return _indicator.ZZVPINwithMinuteBar(Input, cdfsamplesize, sampleSize, volBucketSize);
        }

        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZVPINwithMinuteBar ZZVPINwithMinuteBar(Data.IDataSeries input, int cdfsamplesize, int sampleSize, int volBucketSize)
        {
            return _indicator.ZZVPINwithMinuteBar(input, cdfsamplesize, sampleSize, volBucketSize);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZVPINwithMinuteBar ZZVPINwithMinuteBar(int cdfsamplesize, int sampleSize, int volBucketSize)
        {
            return _indicator.ZZVPINwithMinuteBar(Input, cdfsamplesize, sampleSize, volBucketSize);
        }

        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZVPINwithMinuteBar ZZVPINwithMinuteBar(Data.IDataSeries input, int cdfsamplesize, int sampleSize, int volBucketSize)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZVPINwithMinuteBar(input, cdfsamplesize, sampleSize, volBucketSize);
        }
    }
}
#endregion
