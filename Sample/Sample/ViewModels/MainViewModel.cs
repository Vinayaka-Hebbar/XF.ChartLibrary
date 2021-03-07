using Sample.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sample.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public IList<ChartTypes> Charts { get; }

        public object SelectedItem { get; set; }

        public MainViewModel()
        {
            Charts = new ObservableCollection<ChartTypes>
            {
                new ChartTypes("Line Chart")
                {
                    new ChartType()
                    {
                        Activator = () => new Pages.LineChartSample(),
                        Name = "Basic",
                        Description = "Simple line chart"
                    }
                },
                new ChartTypes("Bar Chart")
                {
                    new ChartType()
                    {
                        Activator = () => new Pages.BarChartSample(),
                        Name = "Basic",
                        Description = "Simple line chart"
                    }
                },
                new ChartTypes("Pie Chart")
                {
                    new ChartType()
                    {
                        Activator = () => new Pages.PieChartSample(),
                        Name = "Basic",
                        Description = "Simple pie chart"
                    }
                }
            };
        }
    }
}
