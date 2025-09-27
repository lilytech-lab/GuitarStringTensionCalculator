using LilytechLab.GuitarStringTensionCalculator.Data;
using MudBlazor;

namespace LilytechLab.GuitarStringTensionCalculator.Pages;

public partial class Index {

	#region constants/readonly
	private readonly List<GuitarSetting> guitarSettings = [];

	private readonly List<ChartSeries> serieses = [];

	private readonly string[] xAxisLabels = Enumerable.Range(1, 8).Select(x => $"{x}弦").ToArray();
	#endregion

	#region field members
	private string message = string.Empty;

	private ChartOptions chartOptions = new() {
		ShowToolTips = true,
		YAxisRequireZeroPoint = true,
		YAxisTicks = 1,
	};

	#endregion

	#region public/protected methods
	protected override void OnInitialized() {
		this.AddGuitar();

		GuitarSetting.ChartValueChanged += (_, _) => this.StateHasChanged();
	}
	#endregion

	#region private methods
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

}

