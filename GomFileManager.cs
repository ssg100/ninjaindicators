
#define FORCEFLUSH
//#undef FORCEFLUSH
using System;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using System.Text;
using System.Diagnostics;
using System.Reflection;


namespace Gom
{

	abstract partial class FileManager : IDataManager, IDisposable
	{
	#region FileManager

        #region Backward reader
        
        
        //class to read file backwards. found it on
        //http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/9acdde1a-03cd-4018-9f87-6e201d8f5d09
		//we need it to find last written date without parsing all the file		

		public class GomBackwardReader : IDisposable
		{
			private FileStream fs = null;

			public GomBackwardReader(string path)
			{
				fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				fs.Seek(0, SeekOrigin.End);
			}

			public string Readline()
			{
				byte[] line;
				byte[] text = new byte[1];
				long position = 0;
				int count;

				fs.Seek(0, SeekOrigin.Current);
				position = fs.Position;

				//do we have trailing \r\n?
				if (fs.Length > 1)
				{
					byte[] vagnretur = new byte[2];
					fs.Seek(-2, SeekOrigin.Current);
					fs.Read(vagnretur, 0, 2);
					if (ASCIIEncoding.ASCII.GetString(vagnretur).Equals("\r\n"))
					{
						//move it back
						fs.Seek(-2, SeekOrigin.Current);
						position = fs.Position;
					}
				}

				while (fs.Position > 0)
				{
					text.Initialize();

					//read one char
					fs.Read(text, 0, 1);

					string asciiText = ASCIIEncoding.ASCII.GetString(text);

					//moveback to the charachter before
					fs.Seek(-2, SeekOrigin.Current);

					if (asciiText.Equals("\n"))
					{
						fs.Read(text, 0, 1);
						asciiText = ASCIIEncoding.ASCII.GetString(text);

						if (asciiText.Equals("\r"))
						{
							fs.Seek(1, SeekOrigin.Current);
							break;
						}
					}
				}

				count = int.Parse((position - fs.Position).ToString());
				line = new byte[count];
				fs.Read(line, 0, count);
				fs.Seek(-count, SeekOrigin.Current);

				return ASCIIEncoding.ASCII.GetString(line);

			}

			public bool SOF { get { return fs.Position == 0; } }


			protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (fs != null)
					{
						fs.Close();
						fs.Dispose();
						fs = null;
					}
				}
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}
		#endregion


        abstract public string Name { get; }
        virtual public bool IsWritable { get { return false; } }
        virtual public bool IsMillisecCompliant { get { return false; } }

		public const string dateformat = "yyMMddHHmmss";
        public const string dateformatmillisec = "yyMMddHHmmssfff";
        public string[] dateformats = { dateformat, dateformatmillisec };

		protected CultureInfo curCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();

		private string _InstrName;
		private bool _writeData;
		private string _converterFileName;
		private Gom.FileModeType _fileMode;
		private bool _writeOK = true;
        protected  bool _useMillisec;

		protected long curDateGMTTicks;
		protected long newDateGMTTicks;
		protected DateTime curReadDate = Gom.Utils.nullDT;

		protected double _tickSize;
		protected bool _isBinary;
		private long _lastTimeInFile;

		protected StreamReader sr;
		protected StreamWriter sw;

		protected BinaryReader br;
		protected BinaryWriter bw;

		protected string _filename="";
		
		// FILE MANAGEMENT
		#region File Management
		
		private void initWrite()
		{
			freewriter();

			_filename = GetFileName(new DateTime(curDateGMTTicks),false);

			if (!File.Exists(_filename))
			{
				FileStream fs = File.Create(_filename);
				fs.Close();
			}

			if (_writeData && IsWritable)
			{
				try
				{
					if (_isBinary)
						bw = new BinaryWriter(File.Open(_filename, FileMode.Append, FileAccess.Write, FileShare.Read));
					else
						sw = new StreamWriter(File.Open(_filename, FileMode.Append, FileAccess.Write, FileShare.Read));

					_writeOK = true;
				}
				catch (IOException)
				{
					_writeOK = false;
				}
			}

			if (_writeOK)
				_lastTimeInFile = GetMaxTimeInFile().Ticks;
			else
				_lastTimeInFile = DateTime.MaxValue.Ticks;
		}


		private bool initread()
		{
			bool found=false;

			freereader();

			if (!String.IsNullOrEmpty(_filename))
			{
				try
				{
					if (_isBinary)
						br = new BinaryReader(File.Open(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
					else
						sr = new StreamReader(File.Open(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

					found = true;
				}
				catch (IOException)
				{
					found = false;
				}
			}
			
			return (found);
		}

		private bool FindFileNameAndOpen(DateTime date)
		{
			bool found = false;

			if (_fileMode == Gom.FileModeType.SingleFile)
			{
				 _filename = GetFileName(Gom.Utils.nullDT);
				found = initread();
			}
			else
			{
				while (date <= DateTime.Now.ToUniversalTime().Date)
				{
					_filename = GetFileName(date);				
					
					if (File.Exists(_filename))
					{
						FileInfo f = new FileInfo(_filename);
						if (f.Length > 0)
						{
							curReadDate = date;
							found = initread();
							break;
						}
					}
					date = date.AddDays(1);
				}
			}
			
			return found;
		}

		protected  bool ManageFileChange()
		{
			bool found = false;

			freereader();

			if (_fileMode == Gom.FileModeType.OnePerDay)
				found = FindFileNameAndOpen(curReadDate.AddDays(1));

			return found;
		}
	
		private string GetFileName(DateTime date)
		{
			string FileName=GetFileName(date,false);	
		
			if (!File.Exists(FileName))
				FileName = GetFileName(date,true);	
		
			return FileName;
		}
	
		
		private string GetFileName(DateTime date, bool addinstrfolder)
		{
			if (_converterFileName == null)
			{
				//file name is instrument name
				
				string nomfic=_InstrName;
				
				char[] invalidFileChars = Path.GetInvalidFileNameChars();

				// remove invalid chars (*,/,\ etx)
				foreach (char invalidFChar in invalidFileChars)
					nomfic = nomfic.Replace(invalidFChar.ToString(), "");

				if (_fileMode == Gom.FileModeType.OnePerDay)
					nomfic += "." + date.ToString("yyyyMMdd");

				string extension;
				if (_isBinary)
					extension = ".dat";
				else
					extension = ".txt";

				string folder = Environment.GetEnvironmentVariable("GOMFOLDER");

				if (folder == null)
					folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				if (addinstrfolder)
					folder+="\\"+_InstrName;
				
				return (folder + "\\" + nomfic + "." + Name + extension);
			}
			else
				return _converterFileName;
		}

		#endregion


		//Helpers
		#region Helpers

		public bool RecordTick(DateTime date, TickTypeEnum tickType, double price, int volume)
		{
			DateTime newDateTimeGMT = date.ToUniversalTime();
			newDateGMTTicks = newDateTimeGMT.Date.Ticks;

			if (((_fileMode == Gom.FileModeType.OnePerDay) && (_writeOK) && (newDateGMTTicks > curDateGMTTicks)) || (curDateGMTTicks == 0))
			{
				curDateGMTTicks = newDateGMTTicks;
				initWrite();
			}


			if ((_writeOK)&&(newDateTimeGMT.Ticks > _lastTimeInFile))
				RecordTickGMT(newDateTimeGMT, tickType, price, volume);

			return _writeOK;
		}


		protected void SwapCulture()
		{
			if (curCulture.NumberFormat.NumberDecimalSeparator == ".")
				curCulture.NumberFormat.NumberDecimalSeparator = ",";
			else
				curCulture.NumberFormat.NumberDecimalSeparator = ".";
		}


		#endregion

		// IDataManager
		#region IDataManager
		
		public bool RecordTick(DateTime date, double bid, double ask, double price, int volume)
		{
			return RecordTick(date, Gom.Utils.GetIntTickType(bid, ask, price), price, volume);
		}

		public abstract void GetNextTick(ref MarketDataType gomData);

		public void SetCursorTime(DateTime time0, ref MarketDataType gomData)
		{
			bool found = false;
			gomData.Time=Gom.Utils.nullDT;
			long time0tick = time0.Ticks;
			long ticktime;
			
			found = FindFileNameAndOpen(time0.ToUniversalTime().Date);

			if (found)
				do
				{
					GetNextTick(ref gomData);
					ticktime = gomData.Time.Ticks;
				}
				while ((ticktime != 0L) && (ticktime < time0tick));
		}
		
		#endregion


		//IDisposable
		#region IDisposable
		private void freereader()
		{
			if (br != null)
			{
				br.Close();
				br = null;
			}

			if (sr != null)
			{
				sr.Close();
				sr.Dispose();
				sr = null;
			}
		}

		private void freewriter()
		{
			if (bw != null)
			{
				bw.Flush();
				bw.Close();
				bw = null;
			}

			if (sw != null)
			{
				sw.Flush();
				sw.Close();
				sw.Dispose();
				sw = null;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{

				freereader();
				freewriter();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion


		// Virtual Methods

		virtual public void RecordTickGMT(DateTime date, TickTypeEnum tickType, double price, int volume)
		{ }

		virtual protected DateTime GetMaxTimeInFile()
		{
			return Gom.Utils.nullDT;
		}

		//Contructors
		#region constructors
		public FileManager()
		{
			curCulture.NumberFormat.NumberGroupSeparator = "";
		}

		public FileManager(bool isInstr, string name, double tickSize, bool writeData, Gom.FileModeType fileMode)
			: this()
		{
			if (isInstr)
				_InstrName = name;
			else
				_converterFileName = name;

			_tickSize = tickSize;
			_writeData = writeData;
            _fileMode = fileMode;
		}

		public FileManager(string filename)
			: this()
		{
			_converterFileName = filename;
			_writeData = false;
			_fileMode = Gom.FileModeType.SingleFile;
		}

		#endregion
	}

	#endregion


	#region Flat
	class FileManagerFlat : FileManager //YYMMAAHHMMSS \t TickType \t Price \t Volume
	{
        public override string Name { get { return "Flat"; } }
        public override bool IsWritable { get { return true; } }
        public override bool IsMillisecCompliant { get { return true; } }

		public FileManagerFlat() : base() { }

		public FileManagerFlat(bool isInstr, string name, double tickSize, bool writedata, Gom.FileModeType fileMode)
			: base(isInstr, name, tickSize, writedata, fileMode)
        { 
            _useMillisec = true;
        }

		public override void RecordTickGMT(DateTime time, TickTypeEnum tickType, double price, int volume)
		{
			sw.WriteLine(time.ToString((_useMillisec)?dateformatmillisec:dateformat) + "\t" + (int)tickType + "\t" + price.ToString("G10", CultureInfo.InvariantCulture) + "\t" + volume);

#if FORCEFLUSH
			sw.Flush();
#endif
		}

		private string GetNextLinePivotFormatted()
		{
			string retString = null;

			if (!sr.EndOfStream)
			{
				retString = sr.ReadLine();
			}
			return retString;
		}

		public override void GetNextTick(ref MarketDataType gomData)
		{
			string retString = GetNextLinePivotFormatted();

			if (retString != null)
			{
				string[] split = retString.Split('\t');

                gomData.Time = DateTime.ParseExact(split[0], dateformats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToLocalTime();
				gomData.TickType = (TickTypeEnum)Enum.Parse(typeof(TickTypeEnum), split[1]);
				try
				{
					gomData.Price = Double.Parse(split[2], curCulture);
				}
				catch (FormatException)
				{
					SwapCulture();
					gomData.Price = Double.Parse(split[2], curCulture);
				}
				gomData.Volume = Int32.Parse(split[3]);
			}
			else
			{
				gomData.Time = Gom.Utils.nullDT;
				if (ManageFileChange())
					GetNextTick(ref  gomData);
			}
		}


		protected override DateTime GetMaxTimeInFile()
		{
			DateTime retTime = new DateTime(0L);
			string stringRead, lastTimeInFileString = "";

			using (GomBackwardReader br = new GomBackwardReader(_filename))
			{
				if (!br.SOF)
				{
					stringRead = br.Readline();

					if ((stringRead != null) && (stringRead.Length > 12))
					{
						lastTimeInFileString = stringRead.Substring(0, 12); // get the date						
                        retTime = DateTime.ParseExact(lastTimeInFileString, dateformats, CultureInfo.CurrentCulture, DateTimeStyles.None);//.ToLocalTime();
					}
				}
			}
			return retTime;
		}
	}
	#endregion

	#region Short
	// DYYMMAAHHMM00 \t Price  on every minute
	// SS TickType \t Delta per reference price of the minute in ticks \t \t Volume  on every tick.
	// SS is omitted is same as previous line

	class FileManagerShort : FileManager
	{
        public override string Name { get { return "Short"; } }
        public override bool IsWritable{ get { return true; } }

		private int lastMinute = -1;
		private int lastSecond;
		private double pivotPrice;

		private DateTime lastReadMinute = Gom.Utils.nullDT;
		private DateTime lastReadSecond;
		private double lastReadPivot;

		public FileManagerShort() : base() { }

		public FileManagerShort(bool isInstr, string name, double tickSize, bool writedata, Gom.FileModeType fileMode)
			: base(isInstr, name, tickSize, writedata, fileMode) { }


		public override void RecordTickGMT(DateTime time, TickTypeEnum tickType, double price, int volume)
		{
			bool newMinuteHappened = false;

			int newMinute = Int32.Parse(time.ToString("yyMMddHHmm"));

			if (newMinute != lastMinute)
			{
				sw.WriteLine(newMinute.ToString("D10") + "\t" + price.ToString("G10", CultureInfo.InvariantCulture));
				lastMinute = newMinute;
				pivotPrice = price;
				newMinuteHappened = true;
			}

			if ((time.Second != lastSecond) || newMinuteHappened)
			{
				sw.WriteLine(time.Second.ToString("D2") + "\t" + (int)tickType + "\t" + Convert.ToInt32((price - pivotPrice) / _tickSize) + "\t" + volume);
				lastSecond = time.Second;
			}
			else
				sw.WriteLine((int)tickType + "\t" + Convert.ToInt32((price - pivotPrice) / _tickSize) + "\t" + volume);

#if FORCEFLUSH
			sw.Flush();
#endif

		}

		public override void GetNextTick(ref MarketDataType gomData)
		{

			string readString = null;

			string[] split;

			gomData.Time = Gom.Utils.nullDT;

			if (!sr.EndOfStream)
			{
				readString = sr.ReadLine();
				if (readString != null)
				{
					split = readString.Split('\t');

					if (split.Length == 2)
					{
						lastReadMinute = DateTime.ParseExact(split[0] + "00", dateformat, CultureInfo.InvariantCulture).ToLocalTime();
						try
						{
							lastReadPivot = Double.Parse(split[1], curCulture);
						}
						catch (FormatException)
						{
							SwapCulture();
							lastReadPivot = Double.Parse(split[1], curCulture);
						}

						GetNextTick(ref  gomData);
						return;
					}

					else if (split.Length == 4)
					{
						gomData.Price = Int32.Parse(split[2]) * _tickSize + lastReadPivot;
						gomData.TickType = (TickTypeEnum)Enum.Parse(typeof(TickTypeEnum), split[1]);
						gomData.Volume = Int32.Parse(split[3]);

						lastReadSecond = lastReadMinute.AddSeconds(Int32.Parse(split[0])); ;
						gomData.Time = lastReadSecond;
					}
					else if (split.Length == 3)
					{
						gomData.Price = Int32.Parse(split[1]) * _tickSize + lastReadPivot;
						gomData.TickType = (TickTypeEnum)Enum.Parse(typeof(TickTypeEnum), split[0]);
						gomData.Volume = Int32.Parse(split[2]);
						gomData.Time = lastReadSecond;
					}
				}
			}
			else
			{
				if (ManageFileChange())
					GetNextTick(ref  gomData);
			}
		}


		protected override DateTime GetMaxTimeInFile()
		{
			DateTime retTime = new DateTime(0L);
			string stringRead, lastTimeInFileString = "";
			string readSeconds;
			string[] split;

			using (GomBackwardReader br = new GomBackwardReader(_filename))
			{
				if (!br.SOF)
				{
					do
					{
						stringRead = br.Readline();
						split = stringRead.Split('\t');
					}
					while ((!br.SOF) && (split.Length != 4));

					if (!br.SOF)
					{
						readSeconds = split[0];

						do
						{
							stringRead = br.Readline();
							split = stringRead.Split('\t');
						}
						while ((!br.SOF) && (split.Length != 2));

						if (!br.SOF)
						{
							split = stringRead.Split('\t');
							lastTimeInFileString = split[0] + readSeconds;
							retTime = DateTime.ParseExact(lastTimeInFileString, dateformat, CultureInfo.CurrentCulture);//.ToLocalTime();
						}
					}
				}
			}

			return retTime;
		}
	}
	#endregion

	#region Binary
	class FileManagerBinary : FileManager
	{
        public override string Name { get {return "Binary";}}
        public override bool IsWritable { get { return true; } }

		private int lastMinute = -1;
		private int lastSecond;
		private double pivotPrice;

		private DateTime lastReadMinute = Gom.Utils.nullDT;
		private DateTime lastReadSecond;
		private double lastReadPivot;




		public FileManagerBinary()
			: base()
		{
			_isBinary = true;
		}

		public FileManagerBinary(bool isInstr, string name, double tickSize, bool writedata, Gom.FileModeType fileMode)
			: base(isInstr, name, tickSize, writedata, fileMode)
		{
			_isBinary = true;
		}

		public void writedata(int second, TickTypeEnum tickType, double price, int volume, bool withsecond)
		{

			Byte statbyte;
			Byte sec;
			int diff;

			if (withsecond)
				statbyte = 3 << 6;
			else
				statbyte = 2 << 6;

			statbyte += checked((Byte)((int)tickType << 3));

			diff = Convert.ToInt32(((price - pivotPrice) / _tickSize));

			if (diff >= -8 && diff <= +7 && volume <= 15)
				statbyte += 7;
			else
			{
				if ((diff > SByte.MaxValue) || (diff < SByte.MinValue))
					statbyte += 1 << 2;

				if (volume > UInt16.MaxValue)
					statbyte += 2;
				else if (volume > Byte.MaxValue)
					statbyte += 1;
			}

			bw.Write(statbyte);

			if (withsecond)
			{
				sec = checked((Byte)second);
				bw.Write(sec);
			}

			// bw.Write((byte)0);
			if (diff >= -8 && diff <= +7 && volume <= 15)
			{
				SByte res = checked((SByte)((SByte)(diff << 4) + volume));
				bw.Write(res);
			}
			else
			{
				if ((diff > SByte.MaxValue) || (diff < SByte.MinValue))
				{
					Int16 res = checked((Int16)diff);
					bw.Write(res);
				}
				else
				{
					SByte res = checked((SByte)diff);
					bw.Write(res);
				}

				if (volume > UInt16.MaxValue)
				{
					Int32 res = checked((Int32)volume);
					bw.Write(res);
				}
				else if (volume > Byte.MaxValue)
				{
					UInt16 res = checked((UInt16)volume);
					bw.Write(res);
				}
				else
				{
					Byte res = checked((Byte)volume);
					bw.Write(res);
				}
			}
		}


		public override void RecordTickGMT(DateTime time, TickTypeEnum tickType, double price, int volume)
		{

			bool newMinuteHappened = false;

			//int newMinute = Int32.Parse(time.ToString("yyMMddHHmm"));
			int newMinute = (time.Year - 2000) * 100000000 + time.Month * 1000000 + time.Day * 10000 + time.Hour * 100 + time.Minute;

			if (newMinute != lastMinute)
			{
				Byte n1 = checked((Byte)1 << 6);
				n1 += checked((Byte)(newMinute % 61));
				bw.Write(n1);

				UInt32 n2 = checked((UInt32)newMinute);
				bw.Write(n2);

				UInt32 n3 = checked(Convert.ToUInt32(price / _tickSize));
				bw.Write(n3);

				lastMinute = newMinute;
				pivotPrice = price;
				newMinuteHappened = true;
			}

			if ((time.Second != lastSecond) || newMinuteHappened)
			{
				writedata(time.Second, tickType, price, volume, true);
				lastSecond = time.Second;
			}
			else
				writedata(time.Second, tickType, price, volume, false);

#if FORCEFLUSH
			bw.Flush();
#endif
		}


		public override void GetNextTick(ref MarketDataType gomData)
		{
			byte statbyte;
			gomData.IsNewTimeStamp=Gom.MarketDataType.TimeStampStatus.Different;

			try
			{
				statbyte = br.ReadByte();
			}
			catch (EndOfStreamException)
			{
				gomData.Time = Gom.Utils.nullDT;
				if (ManageFileChange())
					GetNextTick(ref  gomData);
				return;
			}

			if (statbyte >> 6 == 1)
			{
				lastReadMinute = DateTime.ParseExact(br.ReadUInt32().ToString("D10") + "00", dateformat, CultureInfo.InvariantCulture).ToLocalTime();
				lastReadPivot = br.ReadUInt32() * _tickSize;
				GetNextTick(ref  gomData);
				return;
			}

			else
			{
				if (statbyte >> 6 == 3)
					lastReadSecond = lastReadMinute.AddSeconds(br.ReadByte());
				else
					gomData.IsNewTimeStamp=Gom.MarketDataType.TimeStampStatus.Same;
				
				
				gomData.TickType = (TickTypeEnum)((statbyte & 56 /*00111000*/) >> 3);
				gomData.Time = lastReadSecond;

				if ((statbyte & 7 /*00000111*/ ) == 7)
				{
					SByte toto = br.ReadSByte();
					gomData.Volume = toto & 15 /*00001111*/;
					gomData.Price = lastReadPivot + ((SByte)(toto & 240 /*11110000*/ ) >> 4) * _tickSize;
				}
				else
				{
					if ((statbyte & 4 /*00000100*/) > 0)
						gomData.Price = lastReadPivot + br.ReadInt16() * _tickSize;
					else
						gomData.Price = lastReadPivot + br.ReadSByte() * _tickSize;

					if ((statbyte & 3 /*00000011*/) == 0)
						gomData.Volume = br.ReadByte();
					else if ((statbyte & 3 /*00000011*/) == 1)
						gomData.Volume = br.ReadUInt16();
					else if ((statbyte & 3 /*00000011*/) == 2)
						gomData.Volume = br.ReadInt32();
				}
			}
		}

		protected override DateTime GetMaxTimeInFile()
		{
			DateTime retTime = Gom.Utils.nullDT;

			string readTimeInFile;

			using (FileStream fs = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{

				fs.Seek(0, SeekOrigin.End);

				if (fs.Position == 0)
					goto end;


				byte[] data = new byte[5];

				bool found = false;
				do
				{

					fs.Seek(-6, SeekOrigin.Current);
					fs.Read(data, 0, 5);

					if (((data[0] & 192) == 64) && ((data[0] & 63) == BitConverter.ToUInt32(data, 1) % 61))

						try
						{
							DateTime.ParseExact(BitConverter.ToUInt32(data, 1).ToString("D10") + "00", dateformat, CultureInfo.CurrentCulture);
							found = true;
						}
						catch (FormatException)
						{
						}
				}
				while ((fs.Position >= 6) && (!found));

				if (!found)
					goto end;

				readTimeInFile = BitConverter.ToUInt32(data, 1).ToString("D10");

				int enumsize = Enum.GetValues(typeof(TickTypeEnum)).Length;

				found = false;
				data = new byte[2];
				fs.Seek(0, SeekOrigin.End);
				do
				{

					fs.Seek(-3, SeekOrigin.Current);
					fs.Read(data, 0, 2);
					// readByte = checked((byte)fs.ReadByte());

					if (((data[0] >> 6) == 3) && (((data[0] & 56) >> 3) <= enumsize) && (data[1] < 60))
						found = true;

					// fs.Seek(-1, SeekOrigin.Current);
				} while ((fs.Position >= 3) && (!found));

				if (!found)
					goto end;

				readTimeInFile += data[1].ToString("D2");
				retTime = DateTime.ParseExact(readTimeInFile, dateformat, CultureInfo.CurrentCulture);//.ToLocalTime();

			}

		end:
			return retTime;
		}
	}
	#endregion

	#region Millisec
	class FileManagerMillisec : FileManager
	{
        public override string Name { get { return "Millisec"; } }
        public override bool IsWritable { get { return true; } }
        public override bool IsMillisecCompliant { get { return true; } }

		private int lastMinute = -1;
		private int lastTimeStamp;
		private double pivotPrice;

		private DateTime lastReadMinute = Gom.Utils.nullDT;
		private DateTime lastReadSecond;
		private double lastReadPivot;

		public FileManagerMillisec()
			: base()
		{
			_isBinary = true;
		}

		public FileManagerMillisec(bool isInstr, string name, double tickSize, bool writedata, Gom.FileModeType fileMode)
			: base(isInstr, name, tickSize, writedata, fileMode)
		{
			_isBinary = true;
		}

		public void writedata(int timestamp, TickTypeEnum tickType, double price, int volume, bool withsecond)
		{

			Byte statbyte;
			ushort sec;
			int diff;

			if (withsecond)
				statbyte = 3 << 6;
			else
				statbyte = 2 << 6;

			statbyte += checked((Byte)((int)tickType << 3));

			diff = Convert.ToInt32(((price - pivotPrice) / _tickSize));

			if (diff >= -8 && diff <= +7 && volume <= 15)
				statbyte += 7;
			else
			{
				if ((diff > SByte.MaxValue) || (diff < SByte.MinValue))
					statbyte += 1 << 2;

				if (volume > UInt16.MaxValue)
					statbyte += 2;
				else if (volume > Byte.MaxValue)
					statbyte += 1;
			}

			bw.Write(statbyte);

			if (withsecond)
			{
				sec = checked((UInt16)(timestamp));
				bw.Write(sec);
			}

			// bw.Write((byte)0);
			if (diff >= -8 && diff <= +7 && volume <= 15)
			{
				SByte res = checked((SByte)((SByte)(diff << 4) + volume));
				bw.Write(res);
			}
			else
			{
				if ((diff > SByte.MaxValue) || (diff < SByte.MinValue))
				{
					Int16 res = checked((Int16)diff);
					bw.Write(res);
				}
				else
				{
					SByte res = checked((SByte)diff);
					bw.Write(res);
				}

				if (volume > UInt16.MaxValue)
				{
					Int32 res = checked((Int32)volume);
					bw.Write(res);
				}
				else if (volume > Byte.MaxValue)
				{
					UInt16 res = checked((UInt16)volume);
					bw.Write(res);
				}
				else
				{
					Byte res = checked((Byte)volume);
					bw.Write(res);
				}
			}
			pivotPrice = price;

		}


		public override void RecordTickGMT(DateTime time, TickTypeEnum tickType, double price, int volume)
		{
			bool newMinuteHappened = false;

			int newMinute = Int32.Parse(time.ToString("yyMMddHHmm"));

			if (newMinute != lastMinute)
			{
				Byte n1 = checked((Byte)1 << 6);
				n1 += checked((Byte)(newMinute % 61));
				bw.Write(n1);

				UInt32 n2 = checked((UInt32)newMinute);
				bw.Write(n2);

				UInt32 n3 = checked(Convert.ToUInt32(price / _tickSize));
				bw.Write(n3);

				lastMinute = newMinute;
				pivotPrice = price;
				newMinuteHappened = true;
			}

			int nextTimeStamp = time.Second * 1000 + time.Millisecond;

			if ((nextTimeStamp != lastTimeStamp) || newMinuteHappened)
			{
				writedata(nextTimeStamp, tickType, price, volume, true);
				lastTimeStamp = nextTimeStamp;
			}
			else
				writedata(nextTimeStamp, tickType, price, volume, false);

#if FORCEFLUSH
			bw.Flush();
#endif

		}


		public override void GetNextTick(ref MarketDataType gomData)
		{
			bool EOS = false;

			byte statbyte;

			try
			{
				statbyte = br.ReadByte();
			}
			catch (EndOfStreamException)
			{
				gomData.Time = Gom.Utils.nullDT;
				EOS = true;
				goto end;
			}

			if (statbyte >> 6 == 1)
			{
				lastReadMinute = DateTime.ParseExact(br.ReadUInt32().ToString("D10") + "00", dateformat, CultureInfo.InvariantCulture).ToLocalTime();
				lastReadPivot = br.ReadUInt32() * _tickSize;
				GetNextTick(ref  gomData);
				return;
			}

			else
			{
				if (statbyte >> 6 == 3)
					lastReadSecond = lastReadMinute.AddMilliseconds(br.ReadUInt16());

				gomData.TickType = (TickTypeEnum)((statbyte & 56 /*00111000*/) >> 3);
				gomData.Time = lastReadSecond;

				if ((statbyte & 7 /*00000111*/ ) == 7)
				{
					SByte toto = br.ReadSByte();
					gomData.Volume = toto & 15 /*00001111*/;
					gomData.Price = lastReadPivot + ((SByte)(toto & 240 /*11110000*/ ) >> 4) * _tickSize;
				}
				else
				{
					if ((statbyte & 4 /*00000100*/) > 0)
						gomData.Price = lastReadPivot + br.ReadInt16() * _tickSize;
					else
						gomData.Price = lastReadPivot + br.ReadSByte() * _tickSize;

					if ((statbyte & 3 /*00000011*/) == 0)
						gomData.Volume = br.ReadByte();
					else if ((statbyte & 3 /*00000011*/) == 1)
						gomData.Volume = br.ReadUInt16();
					else if ((statbyte & 3 /*00000011*/) == 2)
						gomData.Volume = br.ReadInt32();

				}

				lastReadPivot = gomData.Price;
			}

		end:
			if (EOS)
			{
				if (ManageFileChange())
					GetNextTick(ref  gomData);
			}
		}


		protected override DateTime GetMaxTimeInFile()
		{
			return Gom.Utils.nullDT;
		}
	}
	#endregion

	#region Ninja
	class FileManagerNinja : FileManager
	{
        public override string Name { get { return "Ninja"; } }

		public FileManagerNinja() : base() { }

		public FileManagerNinja(string name) : base(name) { }

		public override void GetNextTick(ref MarketDataType gomData)
		{
			string lineread;
			string[] split;

			gomData.Time = Gom.Utils.nullDT;

			if (!sr.EndOfStream)
			{
				lineread = sr.ReadLine();
				if (lineread != null)
				{
					split = lineread.Split(';');

					gomData.Time = DateTime.ParseExact(split[0], "yyyyMMdd HHmmss", CultureInfo.InvariantCulture);

					try
					{
						gomData.Price = Double.Parse(split[1], curCulture);
					}
					catch (FormatException)
					{
						SwapCulture();
						gomData.Price = Double.Parse(split[1], curCulture);
					}

					gomData.Volume = Int32.Parse(split[2]);
					gomData.TickType = TickTypeEnum.Unknown;
				}
			}

		}
	}
	#endregion

	#region IRT
	class FileManagerIRT : FileManager 
	{
        public override string Name { get { return "IRT"; } }

		private CultureInfo _culture;

		private bool firstline = true;

		public FileManagerIRT() : base() { }

		public FileManagerIRT(string name, CultureInfo fileCulture)
			: base(name)
		{
			_culture = fileCulture;
		}

		public override void GetNextTick(ref MarketDataType gomData)
		{

			string lineread;
			string[] split;

			gomData.Time = Gom.Utils.nullDT;

			if (!sr.EndOfStream)
			{
				lineread = sr.ReadLine();
				if ((lineread != null) && (!firstline))
				{
					split = lineread.Split('\t');

					gomData.Time = DateTime.Parse(split[1], _culture);
					gomData.Price = Double.Parse(split[2], _culture);

					gomData.Volume = Int32.Parse(split[3]);
					gomData.TickType = Gom.Utils.GetIntTickType(Double.Parse(split[4], _culture), Double.Parse(split[5], _culture), gomData.Price);

				}
				if (firstline)
				{
					firstline = false;
					GetNextTick(ref gomData);
				}
			}

		}
	}
	#endregion

	#region CollectorIQ
	class FileManagerCollectorIQ : FileManager
	{
        public override string Name { get { return "CollectorIQ"; } }

		private CultureInfo _culture;

		public FileManagerCollectorIQ() : base() { }

		public FileManagerCollectorIQ(string name, CultureInfo fileCulture)
			: base(name)
		{
			_culture = fileCulture;
		}

		public override void GetNextTick(ref MarketDataType gomData)
		{
			string lineread;
			string[] split;

			gomData.Time = Gom.Utils.nullDT;

			if (!sr.EndOfStream)
			{
				lineread = sr.ReadLine();
				if (lineread != null)
				{
					split = lineread.Split('\t');

					gomData.Time = DateTime.Parse(split[0], _culture).Add(TimeSpan.Parse(split[1]));
					gomData.Price = Double.Parse(split[2], _culture);

					gomData.Volume = Int32.Parse(split[3]);
					gomData.TickType = Gom.Utils.GetIntTickType(Double.Parse(split[5], _culture), Double.Parse(split[6], _culture), gomData.Price);
				}
			}
		}
	}
	#endregion

	#region Collector
	class FileManagerCollector : FileManager
	{
        public override string Name { get { return "Collector"; } }

		private CultureInfo _culture;
		private double bid, ask;

		public FileManagerCollector() : base() { }

		public FileManagerCollector(string name, CultureInfo fileCulture)
			: base(name)
		{
			_culture = fileCulture;
		}

		public override void GetNextTick(ref MarketDataType gomData)
		{
			string lineread;
			string[] split = { "" };

			gomData.Time = Gom.Utils.nullDT;

			if (!sr.EndOfStream)
				do
				{
					lineread = sr.ReadLine();
					if ((lineread != null))
					{
						split = lineread.Split(';');
						if (split.Length > 1)
						{

							if (!String.IsNullOrEmpty(split[1]))
							{
								gomData.Time = DateTime.Parse(split[0], _culture);
								gomData.Price = Double.Parse(split[1], _culture);
								gomData.Volume = Int32.Parse(split[2]);
								gomData.TickType = Gom.Utils.GetIntTickType(bid, ask, gomData.Price);
							}
							else
							{
								if (!String.IsNullOrEmpty(split[3]) && (split[3] != "0"))
									bid = Double.Parse(split[3], _culture);
								if (!String.IsNullOrEmpty(split[4]) && (split[4] != "0"))
									ask = Double.Parse(split[4], _culture);

								//GetNextLineDataFormatted(ref  gomData);
							}
						}
					}

				} while (String.IsNullOrEmpty(split[1]) && (lineread != null));
		}
	}
	#endregion

	
	public enum FileModeType { SingleFile, OnePerDay };
}

