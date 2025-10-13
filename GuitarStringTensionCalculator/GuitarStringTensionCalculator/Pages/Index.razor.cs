using LilytechLab.GuitarStringTensionCalculator.Data;
using MemoryPack;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace LilytechLab.GuitarStringTensionCalculator.Pages;

public partial class Index {

	#region constants/readonly
	private readonly List<ChartSeries> serieses = [];

	private readonly ChartOptions chartOptions = new() {
		ShowToolTips = true,
		YAxisRequireZeroPoint = true,
		YAxisTicks = 1,
	};
	#endregion

	#region field members
	private string message = string.Empty;

	private string[] xAxisLabels = null!;

	private List<GuitarSetting> guitarSettings = [];
	#endregion

	#region properties
	[Inject]
	public required IJSRuntime JSRuntime { private get; init; }
	#endregion

	#region public/protected methods
	protected override void OnInitialized() {
		this.xAxisLabels = Enumerable.Range(1, 8).Select(x => CreateStringNotation(x)).ToArray();

		this.AddGuitar();

		GuitarSetting.ChartValueChanged += (_, _) => this.StateHasChanged();
	}
	#endregion

	#region private methods
	#region event methods
	private async Task Download() {
		var bytes = MemoryPackSerializer.Serialize(this.guitarSettings);

		var base64String = Convert.ToBase64String(bytes);
		await JSRuntime.InvokeVoidAsync(
			"downloadFromBase64String",
			"StringTensionCalculatorSetting.llstc",
			base64String
		);
	}

	private async Task Upload(IBrowserFile file) {
		using var fs = file.OpenReadStream();
		using var ms = new MemoryStream();
		await fs.CopyToAsync(ms);
		var bytes = ms.ToArray();

		var setting = MemoryPackSerializer.Deserialize<List<GuitarSetting>>(bytes);
		if (setting != null) {
			this.message = "";
			this.guitarSettings = setting;

			this.serieses.Clear();
			this.serieses.AddRange(this.guitarSettings.Select(x => x.ChartSeries));

			this.StateHasChanged();
		} else {
			this.message = Loc["FileIsCorrupted"];
		}
	}

	private void AddGuitar() {
		var newGuitar = new GuitarSetting(this.guitarSettings.Count + 1);
		this.guitarSettings.Add(newGuitar);
		this.serieses.Add(newGuitar.ChartSeries);
	}

	private void RemoveGuitar(int index) {
		this.serieses.RemoveAt(index);
		this.guitarSettings.RemoveAt(index);
	}
	#endregion

	private string CreateStringNotation(int stringNumber) {
		var ret = string.Empty;

		if (Loc["Th"] != "th" || stringNumber >= 4) {
			ret = stringNumber + Loc["Th"];
		} else {
			ret = stringNumber +
				stringNumber switch {
					1 => "st",
					2 => "nd",
					3 => "rd",
					_ => Loc["Th"]
				};
		}

		return ret;
	}
	#endregion

}

