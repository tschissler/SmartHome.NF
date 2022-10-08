using SharedContracts.DataPoints;
using Syncfusion.Blazor.Charts;

namespace Smarthome.Web.Data
{
    public class HistoryChartDataSeries
    {
        public string Name { get; set; }
        public DecimalDataPoint DataPoint { get; set; }
        public ChartSeriesType Type { get; set; }
        public double Opacity { get; set; }
        public string Color { get; set; }
    }
}
