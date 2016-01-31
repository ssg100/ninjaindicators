using System;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;



////THANKS TO MR JOE FOR HAVING REVERSE ENGINEERED NINJA SPEC
/// see http://www.bigmiketrading.com/ninjatrader-programming/7396-ntd-file-specification.html

namespace Gom
{

	partial class GomNinjaTickFileManager : IDataManager, IDisposable
	{
		delegate int GetBigEndianDelegate();
		
		private GetBigEndianDelegate[] GetBigEndian;
        public string Name { get {return "NinjaTickFile"; }}
        public bool IsWritable { get { return true; } }
        public bool IsMillisecCompliant { get { return false; } }

		private string _InstrName;
		public double _TickSize;

		protected DateTime curReadDate = Gom.Utils.nullDT;
		DateTime curTime;
		double multiplier;
		double curPrice;
		ulong firstVolume;
	
		bool newFile=true;

		protected BinaryReader br;
		protected BinaryWriter bw;
		
//		protected FileStream fs;
		protected MemoryStream ms;
		
		private bool _writeOK = true;
		
		long curDateTicks=0L;
		
		bool isRealNinja=true;
		
		private string _FolderRN;
		private string _FolderNotRN;
		
		Dictionary<string,bool> _RNFiles;
		Dictionary<string,bool> _NotRNFiles;	
		
		int _maxfilesize=1;
		byte[] buffer=new byte[1];
		
		long _maxtime;
		
		private void initWrite()
		{
			freewriter();

			string FileName = GetFileName(new DateTime(curDateTicks),false);

			if (!File.Exists(FileName))
			{
				FileStream fs = File.Create(FileName);
				fs.Close();
			}
			
			try
			{
				bw = new BinaryWriter(File.Open(FileName, FileMode.Append, FileAccess.Write, FileShare.Read));
				_writeOK = true;
			}
			catch (IOException)
			{
				_writeOK = false;
			}
			
		}
		
		private bool initread(string FileName)
		{
			bool found;
			int size;
			freereader();

			try
			{
				using( FileStream fs= new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite,  0x10000,FileOptions.SequentialScan))
				{
					size=(int)fs.Length;					
					if (size > _maxfilesize)
						{
							_maxfilesize=size;
							buffer=new byte[size];
						}
				 	fs.Read(buffer, 0, size);
				}	
				ms=new  MemoryStream(buffer,0,size,false);
				br=new BinaryReader (ms);

				newFile=true;
				found = true;
			}
			catch (IOException)
			{
				found = false;
			}
			return (found);
		}

		
		private bool FindFileNameAndOpen(Dictionary<string,bool> filedict,ref DateTime date)
		{
			bool found = false;
			bool fileexists=false;

			while (date <= DateTime.Now)
			{
				string FileName=GetFileName(date);
				if (filedict.ContainsKey(FileName))
				{
					FileInfo f = new FileInfo(FileName);
					if (f.Length > 0)
					{
						curReadDate = date;
						found = initread(FileName);
						break;
					}
				}
				date = date.AddHours(1);
			}
			return found;			
		}
		

private bool FindFileNameAndOpen( DateTime date)
		{
			bool found;
			Gom.MarketDataType gomData=new Gom.MarketDataType();

			if (isRealNinja)
			{
				found=FindFileNameAndOpen(_RNFiles,ref date);
				
				if (found)
				{
					_maxtime=date.AddHours(1).Ticks;
					File.Delete(GetFileName(date.AddHours(-1),false));
					return true;
				}
				else
				{
					_maxtime=DateTime.Now.Ticks;
					isRealNinja=false;
					long ticktime=0;
					date=new DateTime(curTime.Year,curTime.Month,curTime.Day,curTime.Hour,0,0);
					found=FindFileNameAndOpen(_NotRNFiles,ref date);	
					if (found)
					do
					{
						GetNextTick(ref gomData);
						ticktime = gomData.Time.Ticks;
					}
					while ((ticktime != 0L) && (ticktime <= curTime.Ticks));
					
					return 		(ticktime>0L);
					
				}
			}
			else 
			{
				return FindFileNameAndOpen(_NotRNFiles,ref date);	
				
			}
			
		}
		
		
		
	/*	private bool FindFileNameAndOpen(DateTime date)
		{
			
		
			bool found = false;
			bool fileexists=false;

			while (date <= DateTime.Now)
			{
				string FileName;
				
				FileName= GetFileName(date,true);
				
				if (!_RNFiles.ContainsKey(FileName))
				{
					isRealNinja=false;
					FileName=GetFileName(date,false);
					fileexists=_NotRNFiles.ContainsKey(FileName);
				}
				else
				{	
					isRealNinja=true;
					fileexists=true;
					_maxtime=date.AddHours(1).Ticks;
					File.Delete(GetFileName(date.AddHours(-1),false));
				}
				
				if (fileexists)
				{
					FileInfo f = new FileInfo(FileName);
					if (f.Length > 0)
					{
						curReadDate = date;
						found = initread(FileName);
						break;
					}
				}
				date = date.AddHours(1);
			}
			return found;
		}
*/
		
		private bool ManageFileChange()
		{
			bool found = false;

			freereader();
			
			/*if 	(isRealNinja)
				found = FindFileNameAndOpen(curReadDate,true);
			else
			*/
				found = FindFileNameAndOpen(curReadDate.AddHours(1));

			return found;
		}


		private string GetFileName(DateTime date)
		{
			return GetFileName(date, isRealNinja);
		}
		
		private string GetFileName(DateTime date,bool isRN)
		{
			string folder;
			
			if (!isRN)
			{	
				folder=_FolderNotRN+_InstrName+".";
			}
			else
				folder = _FolderRN+_InstrName+@"\";
			
			return(folder+date.AddHours(1).ToString("yyyyMMddHH")+"00.Last.ntd");
		
		}

		private void ReadFirstLine()
		{	
			uint  reccount;
			double price1;
			double price2;
			double price3;
			
			multiplier=-br.ReadDouble();
			reccount=br.ReadUInt32();
			reccount=br.ReadUInt32();
			curPrice=br.ReadDouble();
			curPrice=br.ReadDouble();
			curPrice=br.ReadDouble();
			curPrice=br.ReadDouble();;
			curTime=new DateTime(br.ReadInt64());
			firstVolume=br.ReadUInt64();
			
		}
		
		public bool RecordTick(DateTime date, double bid, double ask, double price, int volume)
		{
			
			long newDateTicks = new DateTime(date.Year,date.Month,date.Day,date.Hour,0,0).Ticks;

			if (((_writeOK) && (newDateTicks > curDateTicks)) || (curDateTicks == 0))
			{
				curDateTicks = newDateTicks;
				initWrite();
			}

			if (_writeOK)
			{
				bw.Write(date.Ticks);
				bw.Write(price);
				bw.Write(volume);
				bw.Flush();
			}
			
			return _writeOK;
		}

		
		private int GetBigEndian0()
		{
			return 0;	
		}	
		
		
		private int GetBigEndian1()
		{
			
			return br.ReadByte();
				
		}
		
		private int GetBigEndian2()
		{
			int retval = br.ReadByte();

			retval = retval << 8 ;
			retval += br.ReadByte();
			
			return retval;
				
		}
		
		private int GetBigEndian3()
		{
			int retval = br.ReadByte();

			retval = retval << 8 ;
			retval += br.ReadByte();
			
			retval = retval << 8 ;
			retval += br.ReadByte();
			
			return retval;
				
		}
		
		private int GetBigEndian4()
		{
			int retval= br.ReadByte();

			retval = retval << 8 ;
			retval += br.ReadByte();
			
			retval = retval << 8 ;
			retval += br.ReadByte();

			retval = retval << 8 ;
			retval += br.ReadByte();
			
			return retval;
				
		}
		
		//public delegate void GetNextTickDelegate(ref MarketDataType gomData);
		
		public  void GetNextTick(ref MarketDataType gomData)
		{
			if (isRealNinja)
			{
			if (newFile)
			{
				ReadFirstLine();
				gomData.Price=curPrice;
				gomData.TickType=TickTypeEnum.Unknown;
				gomData.Volume=(int)firstVolume;
				gomData.Time=curTime;
				gomData.IsNewTimeStamp=Gom.MarketDataType.TimeStampStatus.Different;
				newFile=false;
			}
			else
			{	
				byte statbyte;
				int i;

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
				

				//time

				int nbbytestimes=statbyte & 3 ; //0000011
				
				if (nbbytestimes != 0)
				{	
					long nbsec = GetBigEndian[nbbytestimes]();

					
					curTime = curTime.AddTicks(nbsec*10000000);
					
					if (curTime.Ticks>=_maxtime)
					{
					gomData.Time = Gom.Utils.nullDT;
					if (ManageFileChange())
						GetNextTick(ref  gomData);						
					return;
					}
					
					gomData.Time=curTime;
					gomData.IsNewTimeStamp=Gom.MarketDataType.TimeStampStatus.Different;
					
				}
				else
				{
					gomData.IsNewTimeStamp=Gom.MarketDataType.TimeStampStatus.Same;
				}
				
				//price
				int statusprice=(statbyte & 12)>>2;//0001100	
				int nbbytesprice=0;
				switch(statusprice)
				{
					case 1: //001
						nbbytesprice=1;
						break;
					case 2: //010
						nbbytesprice=2;
						break;
					case 3: //011
						nbbytesprice=4 ;
						break;
	
				}
				
				int deltaprice=GetBigEndian[nbbytesprice]();
	
				switch(nbbytesprice)
				{
					case 1:
						deltaprice -= 0x80;
						break;
					case 2:
						deltaprice -= 0x4000;
						break;
					case 4:
						deltaprice -= 0x40000000;
						break;
				}
				
				curPrice += multiplier*deltaprice;
				gomData.Price=curPrice;
				

				//volume
				int nbbytesvolume=0;
				int statusvol=(statbyte & 112)>>4;
				
				switch (statusvol)    //01110000
				{
					case 1: //001
						nbbytesvolume=1;
						break;
					case 6: //110
						nbbytesvolume=2;
						break;
					case 7: //111
						nbbytesvolume=4;
						break;
					case 2: //010
						nbbytesvolume=8;
						break;
					case 3: //011
						nbbytesvolume=1;
						break;
					case 4: //100
						nbbytesvolume=1;
						break;
					case 5: //101
						nbbytesvolume=1;
						break;
				}	
				
				int volume=GetBigEndian[nbbytesvolume]();
					
				if (statusvol==3)
					volume *= 100;
				else if (statusvol==4)
					volume *= 500;
				else if (statusvol==5)
					volume *= 1000;
				
				gomData.Volume=volume;
				gomData.TickType=TickTypeEnum.Unknown;
				
			}
			}
			else
			{				
				try
				{
					gomData.Time = new DateTime(br.ReadInt64());
				}
				catch (EndOfStreamException)
				{
					gomData.Time = Gom.Utils.nullDT;
					if (ManageFileChange())
						GetNextTick(ref  gomData);
					return;
				}
				
				gomData.Price=br.ReadDouble();
				gomData.Volume=br.ReadInt32();
				gomData.TickType=TickTypeEnum.Unknown;
			}
				
		}
		

		
		private void Init()
		{
			GetBigEndian=new GetBigEndianDelegate[5]{GetBigEndian0,GetBigEndian1,GetBigEndian2,GetBigEndian3,GetBigEndian4};
			
			_FolderRN=Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+@"\NinjaTrader 7\db\tick\";
			
			_FolderNotRN = Environment.GetEnvironmentVariable("GOMFOLDER");

			if (String.IsNullOrEmpty(_FolderNotRN))
				_FolderNotRN = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				
			_FolderNotRN=_FolderNotRN+@"\";
			
			

			DirectoryInfo di = new DirectoryInfo(_FolderRN);
			
			string mastername=_InstrName.Split(' ')[0];
			var directories=di.GetDirectories(mastername+"*");
			
			List<string> filelist=new List<string>();
			directories.ToList().ForEach(dir=>filelist.AddRange(dir.GetFiles("*.ntd", SearchOption.AllDirectories).Select(x=>x.FullName)));
			_RNFiles=filelist.ToDictionary(x=>x,x=>true);
			

			di = new DirectoryInfo(_FolderNotRN);
		 	var files= di.GetFiles("*.ntd",SearchOption.TopDirectoryOnly);
			_NotRNFiles=files.ToDictionary(x=>x.FullName,x=>true);
		
		
		}
		
		private bool  _inited=false;
		
		public void SetCursorTime(DateTime time0, ref MarketDataType gomData)
		{
			bool found = false;
			gomData.Time=Gom.Utils.nullDT;
			long time0tick = time0.Ticks;
			long ticktime;
			
			if (!_inited)
			{
				Init();
				_inited=true;
			}
			
			found = FindFileNameAndOpen(new DateTime(time0.Year,time0.Month,time0.Day,time0.Hour,0,0));

			if (found)
				do
				{
					GetNextTick(ref gomData);
					ticktime = gomData.Time.Ticks;
				}
				while ((ticktime != 0L) && (ticktime < time0tick));
				
		
		}


		//IDisposable
		#region IDisposable
		private void freereader()
		{
			if (br != null)
			{
				br.Close();
				br = null;
			}
			
			if (ms != null)
				{
					ms.Close();
					ms.Dispose();
				}	
		}
		
		private void freewriter()
		{
			if (bw != null)
			{
				bw.Close();
				bw = null;
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


		public GomNinjaTickFileManager(bool isInstr, string name, double tickSize, bool writeData, Gom.FileModeType fileMode)
		{
			if (isInstr)
			{	_InstrName = name;
				_TickSize=tickSize;
			}

		
		}

		public GomNinjaTickFileManager()
		{

		}

	}



}

