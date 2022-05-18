using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace PMU_Converter
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private MainModel model = new MainModel();

        public MainViewModel()
        {
            //CurrentDate = DateTime.Now;
            //UpdateValutesData();
            LoadState();
        }

        private async void UpdateValutesData()
        {

            ValuteData valuteData = null;
            try
            {
                valuteData = await model.UpdateDataFromServer(CurrentDate);
            }
            catch (Exception ex)
            {
                TextCourse = ex.Message;
            }

            if (valuteData == null)
            {
                TextCourse = "На данную дату курсов не найдено.";
                return;
            }
            else
            {
                TextCourse = $"Курс на " +
                    $"{valuteData.Date:dd}." +
                    $"{valuteData.Date:MM}." +
                    $"{valuteData.Date:yyyy}.";
            }

            ValutesList.Clear();
            foreach (var valute in valuteData.Valute)
            {
                ValutesList.Add(new ValutesListElement()
                {
                    CharCode = valute.Key,
                    Name = valute.Value.Name,
                    Value = valute.Value.Value,
                });
            }

            SaveState();
        }

        private void UpdateCalculation()
        {
            if (BaseValute == null || NewValute == null)
                return;

            NewValue = ((baseValue * BaseValute.Value) / newValute.Value).ToString();
            SaveState();
        }

        private void LoadState()
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); 
            var path = Path.Combine(documentsPath, "Data.json");

            if (!File.Exists(path))
            {
                CurrentDate = DateTime.Now;
                UpdateValutesData();
                return;
            }

            StreamReader streamReader = new StreamReader(path);
            string strData = streamReader.ReadToEnd();
            streamReader.Close();

            var data = JsonConvert.DeserializeObject<StateData>(strData);
            ValutesList = data.ValutesList;
            BaseValue = data.BaseValue;
            NewValue = data.NewValue;
            BaseValute = data.BaseValute;
            NewValute = data.NewValute;
            TextCourse = data.TextCourse;
        }

        private void SaveState()
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, "Data.json");

            string strData = JsonConvert.SerializeObject(this, Formatting.Indented);

            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(strData);
            streamWriter.Close();
        }

        public class StateData
        {
            public ValutesListElement BaseValute { get; set; }
            public ValutesListElement NewValute { get; set; }
            public string BaseValue { get; set; }
            public string NewValue { get; set; }
            public ObservableCollection<ValutesListElement> ValutesList { get; set; }
            public string TextCourse { get; set; }
        }

        public class ValutesListElement
        {
            public string CharCode { get; set; }

            public string Name { get; set; }

            public double Value { get; set; }

            public override string ToString()
            {
                return $"{Name} ({CharCode})";
            }
        }


        private string textCourse;
        public string TextCourse
        {
            get { return textCourse; }
            set
            {
                if (textCourse == value) return;
                textCourse = value;
                OnPropertyChanged(nameof(TextCourse));
            }
        }

        private DateTime curDate = DateTime.Now;
        public DateTime CurrentDate
        {
            get { return curDate; }
            set
            {
                if (curDate == value) return;
                curDate = value;
                OnPropertyChanged(nameof(CurrentDate));

                UpdateValutesData();
            }
        }

        private ValutesListElement baseValute;
        public ValutesListElement BaseValute
        {
            get 
            {
                return baseValute;
            }
            set
            {
                if (baseValute == value) return;
                baseValute = value;
                OnPropertyChanged(nameof(BaseValute));
                UpdateCalculation();
            }
        }

        private ValutesListElement newValute;
        public ValutesListElement NewValute
        {
            get { return newValute; }
            set
            {
                if (newValute == value) return;
                newValute = value;
                OnPropertyChanged(nameof(NewValute));
                UpdateCalculation();
            }
        }

        private double baseValue;
        public string BaseValue
        {
            get { return baseValue.ToString(); }
            set
            {
                if (!Double.TryParse(value, out _)) return;

                if (baseValue == Double.Parse(value)) return;
                baseValue = Double.Parse(value);
                OnPropertyChanged(nameof(BaseValue));
                UpdateCalculation();
            }
        }

        private double newValue;
        public string NewValue
        {
            get { return newValue.ToString("0.00"); }
            set
            {
                if (!Double.TryParse(value, out _)) return;

                if (newValue == Double.Parse(value)) return;
                newValue = Double.Parse(value);
                OnPropertyChanged(nameof(NewValue));
            }
        }

        private ObservableCollection<ValutesListElement> valutesList =
            new ObservableCollection<ValutesListElement>();
        public ObservableCollection<ValutesListElement> ValutesList
        {
            get { return valutesList; }
            set
            {
                if (valutesList == value) return;
                valutesList = value;
                OnPropertyChanged(nameof(ValutesList));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
