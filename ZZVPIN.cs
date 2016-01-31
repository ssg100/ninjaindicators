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
    public class ZZVPIN : Indicator
    {
		//public static extern void Analyze();
        #region Variables
        // Wizard generated variables
            private int volBucketSize = 30000; // Default setting for VolBucketSize (V variable)
            private int sampleSize = 49; // Default setting for SampleSize
			private DataSeries	deltaPrice;
			private DataSeries	vpinDataseries;
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
		
			private double VolBuy = 0;
			private double VolSell = 0;	//Vs
			private double cummDeltaVsVB = 0;  // |Vs - VB|
			private double VPINvalue = 0;			// VPIN final value
			private Queue deltaVol;
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
			deltaPrice	= new DataSeries(this, MaximumBarsLookBack.Infinite); 
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
			double vpintemp=0;
			
			if(useRealtimeVol)
			{
				if(Historical)
					return;
			}
			

            if (CurrentBar < 150) 
			{
				return;
			}
			
			deltaPrice[0] = Close[0] - Close[1];
			
			if( useRealtimeVol )
				accumVol = accumVol + lastTradeVol;
			else {
				accumVol = accumVol + Volume[1];

				timeBarCounter = timeBarCounter + 1;
				Print("timeBarCounter  = " + timeBarCounter);
				Print("numTimeBarLastBucket  = " + numTimeBarLastBucket);
				Print("deltaPrice[1]  = " + deltaPrice[0]);
				Print("deltaPrice[1]  = " + deltaPrice[1]);
				Print("deltaPrice[1]  = " + deltaPrice[2]);
				Print("Volume[1]  = " + Volume[1]);
				Print("StdDev  = " + StdDev(deltaPrice,100)[0]);
				Print("SMA  = " + SMA(deltaPrice,100)[0]);
				
				stddevtemp = StdDev(deltaPrice,100)[1];
				normdisttemp = NORMDIST( (deltaPrice[1]/stddevtemp), /*SMA(deltaPrice,100)[1]*/  0, StdDev(deltaPrice, 100)[1], true);
				if(double.IsNaN(normdisttemp)) normdisttemp =0.5;
				//if( stddevtemp == 0 ) stddevtemp = 0.01;
				Print("normdist = " + normdisttemp );

				VolBuy = VolBuy + (Volume[1] * normdisttemp  );
				VolSell = VolSell + ( Volume[1] - (Volume[1] * normdisttemp) );
				Print("VolBuy = " + VolBuy);
				Print("VolSell = " + VolSell);
			}
			Print("accumVol = " + accumVol);
				

			// New bar is formed.  Calculate deltaP/sigma.
			// 	  calculate the stddev based on num of time bars included in a vol bucket -- in slow days, can be more time bars, and need prev vol bucket # of time bars
			if( useRealtimeVol )	
			{
				if( currentBarSaved != CurrentBar )
				{
					Print("deltaPrice 0 = " + deltaPrice[0]);
					Print("deltaPrice 1 = " + deltaPrice[1]);
					timeBarCounter = timeBarCounter + 1;		// count time bar inside vol bucket
				
					//VolBuy = Volume[1] * NORMDIST( (deltaPrice[1]/StdDev(deltaPrice,timeBarCounter)[0]), SMA(deltaPrice,timeBarCounter)[0], StdDev(deltaPrice, timeBarCounter)[0], true);
				
					
					currentBarSaved = CurrentBar;
				}
			}
			
			// Find the mean of delta price of the gathered data of 1/50 * daily vol. 
			// Keep track of volume. Mark the start with the 1-min price bar open


			// V bucket is accomplished, VPIN then calculated (every completed bucket)
			if( accumVol > volBucketSize )
			{
				n = n + 1;  // count bucket sampling
				bucketFull = true;
				numTimeBarLastBucket = timeBarCounter;
				if(numTimeBarLastBucket < 2) numTimeBarLastBucket = 5;
				timeBarCounter = 0;
				accumVol = 0; //reset accummulated vol counter
				
				// VPIN formula sigma over 50 buckets... Delta |Vs - Vb|  /  nV
				// so accummulate delta |Vs -Vb| every completed bucket
				//cummDeltaVsVB = cummDeltaVsVB + Math.Abs(VolSell - VolBuy);
				
				if( n <= sampleSize ) 
				{	// filll up queue until 50
					Print( " filling up queue , n = " + n);
					Print( "delta abs Vs-Vb = " +  (double) Math.Abs(VolSell - VolBuy));
					deltaVol.Enqueue( (double) Math.Abs(VolSell - VolBuy) );
					
					foreach (double value in deltaVol)
					{
						 
	   					 Print( " Value in each deltaVol = " + value);

					}
				}
				else
				{
					Print( " Queue is full count = " + deltaVol.Count);
					initialBucketFull = true;			
				}
				
				if( initialBucketFull )
				{
					deltaVol.Dequeue(  );
					deltaVol.Enqueue( Math.Abs(VolSell - VolBuy) );		
				
					//deltaVol.CopyTo( deltaVolArray, 0 );
					
					// Loop through queue.
					foreach (double value in deltaVol)
					{
						 sumDeltaVol = value + sumDeltaVol;
	   					 Print( " Value in each deltaVol = " + value);

					}
					Print("sumDeltaVol " + sumDeltaVol);


					// Calculate now, VPIN = sigma cummulative deltaVsVb / nV
					VPINvalue = sumDeltaVol / (sampleSize * volBucketSize);
					Print("VPINvalue " + VPINvalue);
			
					vpinDataseries.Set(VPINvalue);	
				}
				
				VolBuy = 0;
				VolSell = 0;
			}
			else
				vpinDataseries.Set(VPINvalue);
			


			Print("vpinDataseries [0] " + vpinDataseries[0]);
			Print("vpinDataseries [1] " + vpinDataseries[1]);
			Print("vpinDataseries [2] " + vpinDataseries[2]);
			vpintemp=NORMDIST( (vpinDataseries[1]/StdDev(vpinDataseries,50)[1]), /*SMA(vpinDataseries,50)[1]*/0, StdDev(vpinDataseries, 50)[1], true);

			
			// PLOT 
           // CDFofVPIN.Set(vpintemp)  ;
			//if( double.IsNaN(VPINvalue) ) VPINvalue = 0.25;
			VPIN.Set(VPINvalue);
			
			
			
        }
		
		protected override void OnMarketData(MarketDataEventArgs e)
		{
   			// Print some data to the Output window
    		if (e.MarketDataType == MarketDataType.Last){
         		//Print("Last = " + e.Price + " " + e.Volume);
				lastTradeVol = e.Volume;
			}

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
        private ZZVPIN[] cacheZZVPIN = null;

        private static ZZVPIN checkZZVPIN = new ZZVPIN();

        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        public ZZVPIN ZZVPIN(int sampleSize, int volBucketSize)
        {
            return ZZVPIN(Input, sampleSize, volBucketSize);
        }

        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        public ZZVPIN ZZVPIN(Data.IDataSeries input, int sampleSize, int volBucketSize)
        {
            if (cacheZZVPIN != null)
                for (int idx = 0; idx < cacheZZVPIN.Length; idx++)
                    if (cacheZZVPIN[idx].SampleSize == sampleSize && cacheZZVPIN[idx].VolBucketSize == volBucketSize && cacheZZVPIN[idx].EqualsInput(input))
                        return cacheZZVPIN[idx];

            lock (checkZZVPIN)
            {
                checkZZVPIN.SampleSize = sampleSize;
                sampleSize = checkZZVPIN.SampleSize;
                checkZZVPIN.VolBucketSize = volBucketSize;
                volBucketSize = checkZZVPIN.VolBucketSize;

                if (cacheZZVPIN != null)
                    for (int idx = 0; idx < cacheZZVPIN.Length; idx++)
                        if (cacheZZVPIN[idx].SampleSize == sampleSize && cacheZZVPIN[idx].VolBucketSize == volBucketSize && cacheZZVPIN[idx].EqualsInput(input))
                            return cacheZZVPIN[idx];

                ZZVPIN indicator = new ZZVPIN();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.SampleSize = sampleSize;
                indicator.VolBucketSize = volBucketSize;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZVPIN[] tmp = new ZZVPIN[cacheZZVPIN == null ? 1 : cacheZZVPIN.Length + 1];
                if (cacheZZVPIN != null)
                    cacheZZVPIN.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZVPIN = tmp;
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
        public Indicator.ZZVPIN ZZVPIN(int sampleSize, int volBucketSize)
        {
            return _indicator.ZZVPIN(Input, sampleSize, volBucketSize);
        }

        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZVPIN ZZVPIN(Data.IDataSeries input, int sampleSize, int volBucketSize)
        {
            return _indicator.ZZVPIN(input, sampleSize, volBucketSize);
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
        public Indicator.ZZVPIN ZZVPIN(int sampleSize, int volBucketSize)
        {
            return _indicator.ZZVPIN(Input, sampleSize, volBucketSize);
        }

        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZVPIN ZZVPIN(Data.IDataSeries input, int sampleSize, int volBucketSize)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZVPIN(input, sampleSize, volBucketSize);
        }
    }
}
#endregion
