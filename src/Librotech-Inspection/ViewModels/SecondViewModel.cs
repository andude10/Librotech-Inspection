using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using OxyPlot;
using OxyPlot.Series;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels
{
    public class SecondViewModel : ReactiveObject, IRoutableViewModel
    {
        private PlotModel _myModel;
        public PlotModel MyModel
        {
            get => _myModel;
            set => this.RaiseAndSetIfChanged(ref this._myModel, value);
        }

        public string UrlPathSegment => "Second";
        public IScreen HostScreen { get; }
        public SecondViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;

            // Tests
            var testValues = new DataPoint[100];
            
            var rnd = new Random();
            var lineSeries = new LineSeries();
            
            for (var i = 0; i < testValues.Length; i++)
            {
                lineSeries.Points.Add(new DataPoint(rnd.Next(-5, 5), rnd.Next(-30,30)));
            }

            var model = new PlotModel();
            model.Series.Add(lineSeries);
            MyModel = model;
            
            
            Back = HostScreen.Router.NavigateBack;
        }

        public ReactiveCommand<Unit, IRoutableViewModel> Back { get; }
    }
}
