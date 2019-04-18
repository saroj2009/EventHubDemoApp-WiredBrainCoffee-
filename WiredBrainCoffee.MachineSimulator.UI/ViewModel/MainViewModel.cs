using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WiredBrainCoffee.EventHub.Sender;
using WiredBrainCoffee.EventHub.Sender.Model;

namespace WiredBrainCoffee.MachineSimulator.UI.ViewModel
{
    public class MainViewModel : BindableBase
    {
        private int _countCuppoccino;
        private int _counterEspresso;
        private string _city;
        private string _serialnumber;
        private int _boilerTemp;
        private int _beanLevel;
        private bool _isSendingPeriodically;
        private ICoffeeMachineDataSender _coffeeMachineDataSender;
        private DispatcherTimer _dispathcherTimer;
        public MainViewModel(ICoffeeMachineDataSender coffeeMachineDataSender)
        {
            _coffeeMachineDataSender = coffeeMachineDataSender;
            SerialNumber = Guid.NewGuid().ToString().Substring(0, 8);
            MakeCuppoccinoCommand = new DelegateCommand(MakeCuppoccino);
            MakeEspressoCommand = new DelegateCommand(MakeEspresso);
            Logs = new ObservableCollection<string>();
            _dispathcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _dispathcherTimer.Tick += _dispathcherTimer_Tick;
        }

        private async void _dispathcherTimer_Tick(object sender, EventArgs e)
        {
            var boilerTempData = CreateCoffeMachineData(nameof(BoilerTemp), BoilerTemp);
            var beanLevelData = CreateCoffeMachineData(nameof(BeanLevel), BeanLevel);

            await sendDataAsync(boilerTempData);
            await sendDataAsync(beanLevelData);
        }

        public ICommand MakeCuppoccinoCommand { get; }
        public ICommand MakeEspressoCommand { get; }
        public ObservableCollection<string> Logs { get;}
        public int CounterCuppoccino
        {
            get { return _countCuppoccino; }
            set
            {
                _countCuppoccino = value;
                RaisePropertyChanged();
            }
        }
        public int CounterEspresso
        {
            get { return _counterEspresso; }
            set
            {
                _counterEspresso = value;
                RaisePropertyChanged();
            }
        }
        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                RaisePropertyChanged();
            }
        }
        public string SerialNumber
        {
            get { return _serialnumber; }
            set
            {
                _serialnumber = value;
                RaisePropertyChanged();
            }
        }
        public int BoilerTemp
        {
            get { return _boilerTemp; }
            set
            {
                _boilerTemp = value;
                RaisePropertyChanged();
            }
        }
        public int BeanLevel
        {
            get { return _beanLevel; }
            set
            {
                _beanLevel = value;
                RaisePropertyChanged();
            }
        }
        public bool IsSendingPeriodically
        {
            get { return _isSendingPeriodically; }
            set
            {
                if (_isSendingPeriodically != value)
                {
                    _isSendingPeriodically = value;
                    if(_isSendingPeriodically)
                    {
                        _dispathcherTimer.Start();
                    }
                    else
                    {
                        _dispathcherTimer.Stop();
                    }
                    RaisePropertyChanged();
                }
            }
        }
        private async void MakeCuppoccino()
        {
            CounterCuppoccino++;
            CoffeMachineData CoffeMachieData = CreateCoffeMachineData(nameof(CounterCuppoccino), CounterCuppoccino);
            await sendDataAsync(CoffeMachieData);
        }
        private async void MakeEspresso()
        {
            CounterEspresso++;
            CoffeMachineData CoffeMachieData = CreateCoffeMachineData(nameof(CounterEspresso), CounterEspresso);
            await sendDataAsync(CoffeMachieData);
        }
        private CoffeMachineData CreateCoffeMachineData(string _SensorType, int _SensorValue)
        {
            return new CoffeMachineData
            {
                City = City,
                SerialNumber = SerialNumber,
                SensorType = _SensorType,
                SensorValue = _SensorValue,
                RecordingTime = DateTime.Now
            };
        }

        private async Task sendDataAsync(CoffeMachineData coffemachinedata)
        {
            try
            {
                await _coffeeMachineDataSender.SendDataAsync(coffemachinedata);
                WriteLog($"Sent Data: {coffemachinedata}");
            }
            catch (Exception ex)
            {
                WriteLog($"Exception: {ex.Message}");
            }
        }
        private void WriteLog(string logMessages)
        {
            Logs.Insert(0, logMessages);
        }

    }
}
