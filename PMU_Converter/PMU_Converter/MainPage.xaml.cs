using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using System.Net.Http;

namespace PMU_Converter
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new ViewModelMainPage();
        }
    }

    public class UntrustedCertClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
        }
    }

    public class ViewModelMainPage : INotifyPropertyChanged
    {
        const string apiUrl = "https://www.cbr-xml-daily.ru/archive/2022/01/10/daily_json.js";

        public ViewModelMainPage()
        {
            FlurlHttp.ConfigureClient("https://www.cbr-xml-daily.ru", cli =>
                cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());

            CurrentDate = DateTime.Now;
            TextCourse = "23413";

            GetDataFromServer();
            
        }

        async void GetDataFromServer()
        {
            ValuteData valutes = await apiUrl.
                    GetJsonAsync<ValuteData>();

            TextCourse = $"{valutes.Valute["AUD"].Name} {valutes.Valute["AUD"].Value}";
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

        private DateTime curDate;
        public DateTime CurrentDate
        {
            get { return curDate; }
            set
            {
                if (curDate == value) return;
                curDate = value;
                OnPropertyChanged(nameof(CurrentDate));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public class ValuteData
        {
            [JsonProperty("Date")]
            public DateTime Date { get; set; }

            [JsonProperty("PreviousDate")]
            public DateTime PreviousDate { get; set; }

            [JsonProperty("PreviousURL")]
            public string PreviousURL { get; set; }

            [JsonProperty("Timestamp")]
            public DateTime Timestamp { get; set; }

            [JsonProperty("Valute")]
            public Dictionary<string, Valute> Valute { get; set; }
        }

        public class Valute
        {
            [JsonProperty("ID")]
            public string ID { get; set; }

            [JsonProperty("NumCode")]
            public string NumCode { get; set; }

            [JsonProperty("CharCode")]
            public string CharCode { get; set; }

            [JsonProperty("Nominal")]
            public int Nominal { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Value")]
            public double Value { get; set; }

            [JsonProperty("Previous")]
            public double Previous { get; set; }
        }



    }
}

