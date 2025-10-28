using System.Collections.ObjectModel;
using System.Data;
using MemoryPack;
using MudBlazor;
using static LilytechLab.GuitarStringTensionCalculator.Data.GuitarSetting.StringSetting;

namespace LilytechLab.GuitarStringTensionCalculator.Data;

[MemoryPackable]
public partial class GuitarSetting {

	#region constants/readonly
	private static readonly float[] neckLengthArray = { 24.0f, 24.75f, 25.0f, 25.4f, 25.5f, 26.0f, 26.25f, 26.5f, 27.0f, 27.5f, 28.0f, 28.5f, 28.75f, 30.0f };

	private static readonly Dictionary<TypeOfStringSetForSix, TypeOfPlainStringGauge[]> dicOfStringSetForSix
		= new() {
			{ TypeOfStringSetForSix.DAddarioErnieBall_009,
				[TypeOfPlainStringGauge.P009]}

		};
	#endregion

	#region fields
	private string guitarName;

	private bool isMultiScale = false;

	private float neckLength = 25.5f;

	private float minNeckLength = 25.5f;

	private float maxNeckLength = 26.5f;

	private int stringCount = 6;

	private TypeOfStandardTuning standardTuningType = TypeOfStandardTuning.E;

	private TypeOfStringSetForSix stringSetForSix = TypeOfStringSetForSix.DAddarioErnieBall_009;
	#endregion

	#region constructors
	public GuitarSetting(int guitarNumber) : this($"Guitar #{guitarNumber}") { }

	[MemoryPackConstructor]
	public GuitarSetting(string guitarName) {
		for (var i = 0; i < 6; i++) {
			var stringSetting = new StringSetting(i + 1,this.NeckLength);
			this.StringSettings.Add( stringSetting);
			this.SetGauge(i + 1);
		}

		this.guitarName = guitarName;
		this.UpdateChart();

		StringSetting.TensionChanged += (_, _) => this.UpdateChart();
	}
	#endregion

	#region events
	public static event EventHandler? ChartValueChanged;
	#endregion

	#region enums
	public enum TypeOfKey {
		F,
		E,
		Dsharp,
		D,
		Csharp,
		C,
		B,
		Asharp,
		A,
		Gsharp,
		G,
		Fsharp
	}

	public enum TypeOfNeck {
		Normal,
		Multi
	}

	public enum TypeOfStringSetForSix {
		DunlopRevWillys_007,
		ErnieBallZippySlinky_007,
		DunlopRevWillys_008,
		DAddarioEXL130_008,
		ErnieBallExtraSlinky_008,
		DunlopExtraLight_008,
		FenderYngwieMalmsteen_008,
		DAddarioEXL130Plus_0085,
		DAddarioErnieBall_009,
		DAddarioErnieBallHybrid_009,
		DAddarioEXL120BTBalanced_009,
		DAddarioEXL120Pllus_0095,
		DAddarioErnieBall_010,
		DAddarioErnieBallBalanced_010,
		DAddarioEXL110BTBalanced_010,
		GHSDavidGlimour_010,
		DunlopHeavyCoreHeavy_010,
		DunlopZakkWylde_010,
		DAddarioEXL110Plus_0105,
		DAddarioEXL115_011,
		DAddarioEXL11115BTBalanced_011,
		ErnieBallPowerSlinky_011,
		DunlopHeavyCoreHeavier_011,
		DAddarioEXL117_011,
		DAddarioEXL145_012,
		DAddarioEXL148_012,
		DAddarioEJ21_012,
		ErnieBallNotEvenSlinky_012,
		DAddarioEXL158_013,
		DaddarioEJ22_013,
		ErnieBallBaritoneSlinky_013,
		DAddarioEXL157
	}

	public enum TypeOfStringSetForSeven {
		DAddarioErnieBall
	}

	public enum TypeOfStringSetForEight {
		DAddarioErnieBall
	}

	public enum TypeOfTuning {
		Standard,
		Drop,
		Open
	}

	public enum TypeOfStandardTuning {
		F = 1,
		E = 0,
		Eflat = -1,
		D = -2,
		Csharp = -3,
		C = -4,
		B = -5,
		Bflat = -6,
		A = -7,
		Aflat = -8,
		G = -9
	}
	#endregion

	#region properties
	[MemoryPackIgnore]
	public static float[] NeckLengthArray => neckLengthArray;

	private static LinkedList<TypeOfKey> KeyRing { get; set; } = new(Enum.GetValues<TypeOfKey>());

	public string GuitarName {
		get => this.guitarName;
		set {
			if (this.guitarName == value) return;

			this.guitarName = value;
			this.UpdateChart();
		}
	}

	public TypeOfNeck NeckType {
		get => this.isMultiScale ? TypeOfNeck.Multi : TypeOfNeck.Normal;
		set => this.isMultiScale = (value == TypeOfNeck.Multi);
	}

	public float NeckLength {
		get => this.neckLength;
		set {
			if (this.neckLength == value) return;

			this.neckLength = value;
			this.SetStringLength();
		}
	}

	public float MinNeckLength {
		get => this.minNeckLength;
		set {
			if (this.minNeckLength == value) return;

			this.minNeckLength = value;
			this.SetStringLength();
		}
	}

	public float MaxNeckLength {
		get => this.maxNeckLength;
		set {
			if (this.maxNeckLength == value) return;

			this.maxNeckLength = value;
			this.SetStringLength();
		}
	}

	public int StringCount {
		get => this.stringCount;
		set {
			if (this.stringCount == value) return;

			var oldCount = this.stringCount;
			this.stringCount = value;

			if (oldCount < this.stringCount) {
				if (oldCount == 6) {
					var stringNumber = 7;
					var stringSetting = new StringSetting(stringNumber, this.NeckLength);
					this.StringSettings.Add(stringSetting);
					this.SetGauge(stringNumber);
				}
				if (this.stringCount == 8) {
					var stringNumber = 8;
					var stringSetting = new StringSetting(stringNumber, this.NeckLength);
					this.StringSettings.Add(stringSetting);
					this.SetGauge(stringNumber);
				}
			} else {
				this.StringSettings.RemoveRange(this.stringCount, oldCount - this.stringCount);
				this.UpdateChart();
			}

			if (this.IsMultiScale) {
				this.SetStringLength();
			}
			
		}
	}

	public TypeOfStringSetForSix StringSetForSix {
		get => this.stringSetForSix;
		set {
			this.stringSetForSix = value;
			//this.SetGauge();
		}
	}

	public TypeOfStandardTuning StandardTuningType {
		get => this.standardTuningType;
		set {
			if (this.standardTuningType == value) return;

			this.standardTuningType = value;
			this.StringSettings.ForEach(x => x.Offset = (int)(this.standardTuningType));
		}
	}

	public List<StringSetting> StringSettings { get; private set; } = new(8);

	[SuppressDefaultInitialization]
	public bool IsMultiScale {
		get => this.isMultiScale;
		set {
			if (this.isMultiScale == value) return;

			this.isMultiScale = value;
			this.SetStringLength();
		}
	}

	[MemoryPackIgnore]
	public ChartSeries ChartSeries { get; } = new() {
		ShowDataMarkers = true
	};
	#endregion

	#region private methods

	private void SetStringLength() {
		for (var i = 0; i < this.StringSettings.Count; i++) {
			if (!this.IsMultiScale) {
				this.StringSettings[i].Length = this.NeckLength;
			} else {
				var diff = (this.MaxNeckLength - this.MinNeckLength) / (this.StringCount - 1);
				this.StringSettings[i].Length = this.MinNeckLength + (diff * i);
			}
		}

		this.UpdateChart();
	}

	private void SetGauge(int stringNumber) {
		var index = stringNumber - 1;
		switch (index) {
			case 0:
				this.StringSettings[index].PlainStringGauge = StringSetting.TypeOfPlainStringGauge.P010;
				break;
			case 1:
				this.StringSettings[index].PlainStringGauge = StringSetting.TypeOfPlainStringGauge.P013;
				break;
			case 2:
				this.StringSettings[index].PlainStringGauge = StringSetting.TypeOfPlainStringGauge.P017;
				break;
			case 3:
				this.StringSettings[index].WoundStringGauge = StringSetting.TypeOfWoundStringGauge.W026;
				break;
			case 4:
				this.StringSettings[index].WoundStringGauge = StringSetting.TypeOfWoundStringGauge.W036;
				break;
			case 5:
				this.StringSettings[index].WoundStringGauge = StringSetting.TypeOfWoundStringGauge.W046;
				break;
			case 6:
				this.StringSettings[index].WoundStringGauge = StringSetting.TypeOfWoundStringGauge.W059;
				break;
			case 7:
				this.StringSettings[index].WoundStringGauge = StringSetting.TypeOfWoundStringGauge.W074;
				break;
		}

		switch (this.StringSetForSix) {
			case TypeOfStringSetForSix.DAddarioErnieBall_009:
				break;
			default:
				break;
		};
	}

	private void UpdateChart() {
		this.ChartSeries.Name = this.GuitarName;
		this.ChartSeries.Data = this.StringSettings.Select(x => x.Tension).ToArray();

		ChartValueChanged?.Invoke(this, EventArgs.Empty);
	}
	#endregion

	#region inner classes
	[MemoryPackable]
	public partial class StringSetting {

		#region fields
		private float length;
		private TypeOfPlainStringGauge plainStringGauge = TypeOfPlainStringGauge.P010;
		private TypeOfWoundStringGauge woundStringGage = TypeOfWoundStringGauge.W026;
		private int offset = 0;
		private double tension;
		#endregion

		#region constructors
		public StringSetting(int stringNumber, float length) {
			this.StringNumber = stringNumber;

			var standardKeyNode = KeyRing.Find(this.StandardKey)!;
			this.KeyDic.Add(1, (standardKeyNode.Previous ?? KeyRing.Last!).Value);
			this.KeyDic.Add(0, standardKeyNode.Value);

			var currentNode = standardKeyNode;
			for (var i = -1; i >= -9; i--) {
				currentNode = currentNode.Next ?? KeyRing.First!;
				this.KeyDic.Add(i, currentNode.Value);
			}

			this.PlainOrWound = this.StringNumber < 4 ? TypeOfPlainOrWound.P : TypeOfPlainOrWound.W;

			this.Length = length;
		}
		#endregion

		#region events
		public static event EventHandler? TensionChanged;
		#endregion

		#region enums
		public enum TypeOfPlainOrWound {
			P,
			W
		}

		public enum TypeOfPlainStringGauge {
			P007 = 1086,
			P008 = 1418,
			P0085 = 1601,
			P009 = 1795,
			P0095 = 2000,
			P010 = 2216,
			P0105 = 2444,
			P011 = 2682,
			P0115 = 2931,
			P012 = 3192,
			P0125 = 3463,
			P013 = 3746,
			P0135 = 4039,
			P014 = 4344,
			P015 = 4987,
			P016 = 5674,
			P0165 = 6034,
			P017 = 6405,
			P018 = 7181,
			P019 = 8001,
			P020 = 8866,
			P021 = 9774,
			P022 = 10727,
			P024 = 12766,
			P026 = 14983
		}

		public enum TypeOfWoundStringGauge {
			W017 = 5460,
			W018 = 6193,
			W019 = 6974,
			W020 = 7400,
			W021 = 8245,
			W022 = 9139,
			W023 = 10082,
			W024 = 10827,
			W025 = 11850,
			W026 = 12658,
			W028 = 14540,
			W029 = 16019, // NYNW029
			W030 = 17034,
			W031 = 17934, // NYNW031
			W032 = 19021,
			W033 = 20264, // NYNW033
			W034 = 21130,
			W035 = 32932, // NYNW035
			W036 = 23359,
			W037 = 24819,
			W038 = 25710,
			W039 = 27233,
			W040 = 28806,
			W042 = 31405,
			W043 = 32932, // NYNW043
			W044 = 34126,
			W045 = 35023, // NYNW045
			W046 = 36969,
			W047 = 38587, // NYNW047
			W048 = 39932,
			W049 = 41459,
			W050 = 43017,
			W052 = 46223,
			W053 = 47973, // NYNW053
			W054 = 51617,
			W056 = 55129,
			W057 = 57005, // NYNW057
			W058 = 59226, // NYNW058
			W059 = 61816,
			W060 = 64143,
			W062 = 68032,
			W063 = 69218, // NYNW063
			W064 = 72042,
			W065 = 73940,
			W066 = 77097,
			W068 = 81724,
			W070 = 86151,
			W072 = 90700,
			W074 = 94948,
			W076 = 100039, // NYNW076
			W078 = 104923, // NYNW078
			W080 = 110556,
			W084 = 120301, // NYNW084T
			W090 = 137558
		}
		#endregion

		#region properties
		[MemoryPackIgnore]
		public Dictionary<int, TypeOfKey> KeyDic { get; } = [];

		public int StringNumber { get; }

		public float Length {
			get => this.length;
			set {
				if (this.length == value) return;

				this.length = value;
				this.CalcTension();
			}
		}

		public TypeOfPlainOrWound PlainOrWound { get; set; }

		public TypeOfPlainStringGauge PlainStringGauge {
			get => this.plainStringGauge;
			set {
				this.plainStringGauge = value;
				this.CalcTension();
			}
		}

		public TypeOfWoundStringGauge WoundStringGauge {
			get => this.woundStringGage;
			set {
				this.woundStringGage = value;
				this.CalcTension();
			}
		}

		public int Offset { 
			get => this.offset;
			set {
				this.offset = value;
				this.CalcTension();
			}
		}

		[MemoryPackIgnore]
		public double Tension {
			get => this.tension;
			private set {
				if (this.tension == value) return;

				this.tension = value;
				TensionChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		[MemoryPackIgnore]
		public double Tension_lb { get; private set; }

		[MemoryPackIgnore]
		public double Freqency {
			get {
				var offsetFromA0OnStandardKey = this.StringNumber switch {
					1 => -5 + (12 * 4), // E4
					2 => 2 + (12 * 3), // B3
					3 => -2 + (12 * 3), // G3
					4 => -7 + (12 * 3), // D3
					5 => 0 + (12 * 2), // A2
					6 => -5 + (12 * 2), // E2
					7 => 2 + (12 * 1), // B1
					8 => -3 + (12 * 1), // F#1
					_ => throw new NotImplementedException($"StringNumber: {this.StringNumber} に対応する音高が設定されていません")
				};

				var freq = 27.5 * Math.Pow(2, 0 + ((offsetFromA0OnStandardKey + this.Offset) / 12.0));
				return freq;
			}
		}

		private TypeOfKey StandardKey => this.StringNumber switch {
			1 => TypeOfKey.E,
			2 => TypeOfKey.B,
			3 => TypeOfKey.G,
			4 => TypeOfKey.D,
			5 => TypeOfKey.A,
			6 => TypeOfKey.E,
			7 => TypeOfKey.B,
			8 => TypeOfKey.Fsharp,
			_ => throw new NotImplementedException($"StringNumber: {StringNumber} に対応するStandardKeyは設定されていません")
		};
		#endregion

		#region private methods
		private void CalcTension() {
			var uw = this.PlainOrWound switch {
				TypeOfPlainOrWound.P => (int)this.PlainStringGauge / Math.Pow(10, 8),
				TypeOfPlainOrWound.W => (int)this.WoundStringGauge / Math.Pow(10, 8),
				_ => throw new NotImplementedException($"PlainOrWound: {this.PlainOrWound} に対応するUWが設定されていません")
			};

			this.Tension_lb = (uw * Math.Pow(2 * this.Length * this.Freqency, 2) / 386.088);
			this.Tension = this.Tension_lb / 2.2046;
		}
		#endregion

	}
	#endregion

}

public static class Extension {

	public static string GetKeyName(this GuitarSetting.TypeOfStandardTuning @this) {
		return GetKeyName(@this.ToString());
	}

	public static string GetKeyName(this GuitarSetting.TypeOfKey @this) {
		return GetKeyName(@this.ToString());
	}

	private static string GetKeyName(string elementName) {
		return elementName.Replace("sharp", "#").Replace("flat", "♭");
	}

}

