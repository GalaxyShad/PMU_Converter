using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PMU_Converter
{
    internal class MainModel
    {
        /*private ValuteData valutesData = null;
        public ValuteData ValutesData
        {
            get { return valutesData; }
        }*/

        private const string apiUrl = "https://www.cbr-xml-daily.ru";

        public MainModel()
        {
            FlurlHttp.ConfigureClient(apiUrl, cli =>
                cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());

        }

        public async Task<ValuteData> UpdateDataFromServer(DateTime curDate)
        {
            ValuteData valutes = null;

            DateTime now = DateTime.Now;
            for (DateTime date = curDate; date <= now; date = date.AddDays(1))
            {
                try
                {
                    valutes = await $"{apiUrl}/archive/{date:yyyy/MM/dd}/daily_json.js".
                        GetJsonAsync<ValuteData>();
                }
                catch (FlurlHttpException ex)
                {
                    if (ex.StatusCode != 404)
                    {
                        throw new Exception($"Ошибка получения данных с сервера. Код ошибки: {ex.StatusCode}");
                    }
                }

                if (valutes != null)
                    break;
            }

            if (valutes != null)
                valutes.Valute.Add("RUB", new Valute
                {
                    CharCode = "RUB",
                    ID = "0",
                    Name = "Российские рубли",
                    Nominal = 1,
                    Value = 1.0,
                });

            return valutes;

        }

        private class UntrustedCertClientFactory : DefaultHttpClientFactory
        {
            public override HttpMessageHandler CreateMessageHandler()
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
            }
        }

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
