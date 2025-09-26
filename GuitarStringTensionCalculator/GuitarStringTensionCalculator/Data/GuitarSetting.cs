using System.Data;
using MudBlazor;

namespace LilytechLab.GuitarStringTensionCalculator.Data;

public class GuitarSetting {

	#region constants/readonly
	private static readonly float[] neckLengthArray = { 24.0f, 24.75f, 25.0f, 25.4f, 25.5f, 26.0f, 26.25f, 26.5f, 27.0f, 27.5f, 28.0f, 28.5f, 28.75f, 30.0f };

	//private static readonly Dictionary<TypeOfStringSetForSix, (TypeOfPlainOrWound, TypeOfPlainStringGauge)> = new();
	#endregion

	#region fields
	private string guitarName;

	private TypeOfNeck neckType = TypeOfNeck.Normal;

	private float neckLength = 25.5f;

	private float minNeckLength = 25.5f;

	private float maxNeckLength = 30.0f;

	private int stringCount = 6;

	private TypeOfStandardTuning standardTuningType = TypeOfStandardTuning.E;

	private TypeOfStringSetForSix stringSetForSix = TypeOfStringSetForSix.DAddarioErnieBall_009;
	#endregion

	#region constructors
	public GuitarSetting(int guitarNumber) {
		for (var i = 0; i < 6; i++) {
			var stringSetting = new StringSetting(i + 1,this.NeckLength);
			this.StringSettings.Add( stringSetting);
		}

		this.guitarName = $"Guitar #{guitarNumber}";
		this.SetGauge();
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

	public static float[] NeckLengthArray => neckLengthArray;

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

	public enum TypeOfDropTuning {
		D = 0,
		Csharp = -1,
		C = -2,
		B = -3
	}

	public enum TypeOfOpenTuning {

	}
	#endregion

	#region properties
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
		get => this.neckType;
		set {
			if (this.neckType == value) return;

			this.neckType = value;
			this.SetStringLength();
		}
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
					var stringSetting = new StringSetting(7, this.NeckLength);
					this.StringSettings.Add(stringSetting);
				}
				if (this.stringCount == 8) {
					var stringSetting = new StringSetting(8, this.NeckLength);
					this.StringSettings.Add(stringSetting);
				}

				this.SetGauge();
			} else {
				this.StringSettings.RemoveRange(this.stringCount, oldCount - this.stringCount);
				this.UpdateChart();
			}

			if (this.NeckType == TypeOfNeck.Multi) {
				this.SetStringLength();
			}
			
		}
	}

	public TypeOfStringSetForSix StringSetForSix {
		get => this.stringSetForSix;
		set {
			this.stringSetForSix = value;
			this.SetGauge();
		}
	}

	public TypeOfTuning TuningType { get; set; } = TypeOfTuning.Standard;

	public TypeOfStandardTuning StandardTuningType {
		get => this.standardTuningType;
		set {
			if (this.standardTuningType == value) return;

			this.standardTuningType = value;
			this.StringSettings.ForEach(x => x.Offset = (int)(this.standardTuningType));
		}
	}

	public List<StringSetting> StringSettings { get; private set; } = new(8);

	public ChartSeries ChartSeries { get; } = new() {
		ShowDataMarkers = true
	};
	#endregion

	#region private methods

	private void SetStringLength() {
		for (var i = 0; i < this.StringSettings.Count; i++) {
			if (this.NeckType == TypeOfNeck.Normal) {
				this.StringSettings[i].Length = this.NeckLength;
			} else {
				var diff = (this.MaxNeckLength - this.MinNeckLength) / (this.StringCount - 1);
				this.StringSettings[i].Length = this.MinNeckLength + (diff * i);
			}
		}

		this.UpdateChart();
	}

	private void SetGauge() {
		this.StringSettings[0].PlainStringGauge = StringSetting.TypeOfPlainStringGauge.P010;
		this.StringSettings[1].PlainStringGauge = StringSetting.TypeOfPlainStringGauge.P013;
		this.StringSettings[2].PlainStringGauge = StringSetting.TypeOfPlainStringGauge.P017;
		this.StringSettings[3].WoundStringGauge = StringSetting.TypeOfWoundStringGauge.W026;
		this.StringSettings[4].WoundStringGauge = StringSetting.TypeOfWoundStringGauge.W036;
		this.StringSettings[5].WoundStringGauge = StringSetting.TypeOfWoundStringGauge.W046;

		if (this.StringSettings.Count > 6)
			this.StringSettings[6].WoundStringGauge = StringSetting.TypeOfWoundStringGauge.W054;

		if (this.StringSettings.Count == 8)
			this.StringSettings[7].WoundStringGauge = StringSetting.TypeOfWoundStringGauge.W064;

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
	public class StringSetting {

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
			P007 = 1085,
			P008 = 1418,
			P0085 = 1601,
			P009 = 1794,
			P0095 = 1999,
			P010 = 2215,
			P0105 = 2442,
			P011 = 2680,
			P0115 = 2930,
			P012 = 3190,
			P013 = 3744,
			P0135 = 4037,
			P014 = 4342,
			P015 = 4984,
			P016 = 5671,
			P017 = 6402,
			P018 = 7177,
			P019 = 7997,
			P020 = 8861,
			P022 = 10722,
			P024 = 12760,
			P026 = 14975
		}

		public enum TypeOfWoundStringGauge {
			W017 = 5524,
			W018 = 6215,
			W019 = 6947,
			W020 = 7495,
			W021 = 8293,
			W022 = 9184,
			W024 = 10857,
			//W025 = ,
			W026 = 12671,
			W028 = 14666,
			W030 = 17236,
			W032 = 19347,
			W034 = 21590,
			W036 = 23964,
			//W037 = ,
			W038 = 26471,
			W039 = 27932,
			//W040,
			W042 = 32279,
			W044 = 35182,
			W046 = 38216,
			W048 = 41382,
			W049 = 43014,
			//W050 = ,
			W052 = 48109,
			W054 = 53838,
			W056 = 57598,
			W059 = 64191,
			W060 = 66542,
			W062 = 70697,
			W064 = 74984,
			//W065 = ,
			W066 = 79889,
			W068 = 84614,
			W070 = 89304,
			W072 = 94129,
			W074 = 98869,
			W080 = 115011,
			//W084 = 
		}
		#endregion

		#region properties
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

		public double Tension {
			get => this.tension;
			private set {
				if (this.tension == value) return;

				this.tension = value;
				TensionChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public double Tension_lb { get; private set; }

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

			this.Tension_lb = (uw * Math.Pow(2 * this.Length * this.Freqency, 2) / 386.4);
			this.Tension = this.Tension_lb / 2.2046;
		}
		#endregion

	}
	#endregion

}

