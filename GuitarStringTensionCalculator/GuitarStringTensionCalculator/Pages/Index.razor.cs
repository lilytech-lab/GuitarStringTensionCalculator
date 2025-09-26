using LilytechLab.GuitarStringTensionCalculator.Data;
using MudBlazor;

namespace LilytechLab.GuitarStringTensionCalculator.Pages;

public partial class Index {

	#region field members
	private string message = string.Empty;

	private GuitarSetting guitarSetting = new(1);

	private List<ChartSeries> serieses = [];

	private string[] xAxisLabels = Enumerable.Range(1, 8).Select(x => $"{x.ToString()}弦").ToArray();

	private ChartOptions chartOptions = new ChartOptions() {
		ShowToolTips = true,
		YAxisRequireZeroPoint = true,
		YAxisTicks = 1,
	};

	#endregion

	protected override void OnInitialized() {
		this.serieses.Add(this.guitarSetting.ChartSeries);

		GuitarSetting.ChartValueChanged += (_, _) => this.StateHasChanged();
	}

}

