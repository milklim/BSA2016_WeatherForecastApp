using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using weatherForecastApp.Models;

namespace weatherForecastApp.Services
{
    public static class WeatherService
    {
        static string appId = System.Web.Configuration.WebConfigurationManager.AppSettings["openWeatherAppId"];
 
        public static long GetCityIdByName(string name)
        {
            // Формируем строку запроса
            Uri uri = new Uri(String.Format("{0}{1}{2}{3}", "http://api.openweathermap.org/data/2.5/find?q=", Translit(name.ToLower()), "&type=like&units=metric&lang=ru&appid=", appId));
            string jsonStr;
            // При возникновении исключения, пробрасываем его в контроллер
            try
            {
                // Запрос на сервер
                jsonStr = GetJson(uri);
            }
            catch (Exception)
            {
                throw;
            }
            JsonTextReader reader = new JsonTextReader(new StringReader(jsonStr));
            // Ищем в ответе сервера id для города
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "id")
	            {
		            reader.Read();
                    return (long)reader.Value;
	            }
            }
            // Если id города не найден, возвращаем 0
            return 0;
        }
        public static WeatherView GetCurrentWeatherByCityId(int cityId)
        {
            // Формируем строку запроса
            Uri uri = new Uri(String.Format("{0}{1}{2}{3}", "http://api.openweathermap.org/data/2.5/weather?id=", cityId, "&units=metric&lang=ru&appid=", appId));
            string jsonStr;
            // При возникновении исключения, пробрасываем его в контроллер
            try
            {
                // Запрос на сервер
                jsonStr = GetJson(uri);
            }
            catch (Exception)
            {
                throw;
            }
           
            // Десериализация ответа
            CurrentWeather currWeather = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrentWeather>(jsonStr);
           
            // Формируем модель WeatherView, с которой работает представление (вьюха)
            WeatherView weatherView = new WeatherView(currWeather.name, currWeather.id, currWeather.coord.lon, currWeather.coord.lat);
            weatherView.Forecast.Add(new WeatherParameters(UnixTimeStampToDateTime(currWeather.dt).ToShortDateString(), currWeather.main.temp, currWeather.main.temp_min,
                                     currWeather.main.temp_max, currWeather.main.humidity, currWeather.main.pressure, currWeather.wind.speed, WindDegreesToCompas(currWeather.wind.deg),
                                     currWeather.clouds.all, currWeather.weather[0].description, currWeather.weather[0].icon));
            return weatherView;
        }

        public static WeatherView GetWeatherFor3DaysByCityId(int cityId)
        {
            // Формируем строку запроса
            Uri uri = new Uri(String.Format("{0}{1}{2}{3}", "http://api.openweathermap.org/data/2.5/forecast?id=", cityId, "&units=metric&lang=ru&appid=", appId));
           
            string jsonStr;
            // При возникновении исключения, пробрасываем его в контроллер
            try
            {
                // Запрос на сервер
                jsonStr = GetJson(uri);
            }
            catch (Exception)
            {
                throw;
            }

            // Десериализация ответа
            WeatherForecast3DaysDetailed forecastFor3Days = JsonConvert.DeserializeObject<WeatherForecast3DaysDetailed>(jsonStr);
           
            // Формируем модель WeatherView, с которой работает представление (вьюха)
            WeatherView weatherView = new WeatherView(forecastFor3Days.city.name, forecastFor3Days.city.id, forecastFor3Days.city.coord.lon, forecastFor3Days.city.coord.lat);
            foreach (var item in forecastFor3Days.Forecasts3Hours)
            {
                // Преобразуем POSIX время из отвера сервера в DateTime
                DateTime dt = UnixTimeStampToDateTime(item.dt);
                // Выбираем данные погоды на 3 дня (в ответе приходит прогноз на 5 дней)
                if (dt.Day - DateTime.Now.Day > 3) { return weatherView; }
                weatherView.Forecast.Add(new WeatherParameters(dt.ToString("dd.MM.yyyy  HH:mm"), item.main.temp, item.main.temp_min, item.main.temp_max,
                                        item.main.humidity, item.main.pressure, item.wind.speed, WindDegreesToCompas(item.wind.deg), item.clouds.all, item.weather[0].description, item.weather[0].icon));
            }
            return weatherView;
        }
        public static WeatherView GetWeatherFor7DaysByCityId(int cityId)
        {
            // Формируем строку запроса
            Uri uri = new Uri(String.Format("{0}{1}{2}{3}", "http://api.openweathermap.org/data/2.5/forecast/daily?id=", cityId, "&units=metric&lang=ru&appid=", appId));

            string jsonStr;
            // При возникновении исключения, пробрасываем его в контроллер
            try
            {
                // Запрос на сервер
                jsonStr = GetJson(uri);
            }
            catch (Exception)
            {
                throw;
            }

            // Десериализация ответа
            WeatherForecast7Days forecastFor7Days = JsonConvert.DeserializeObject<WeatherForecast7Days>(jsonStr);
           
            // Формируем модель WeatherView, с которой работает представление (вьюха)
            WeatherView weatherView = new WeatherView(forecastFor7Days.city.name, forecastFor7Days.city.id, forecastFor7Days.city.coord.lon, forecastFor7Days.city.coord.lat);
            foreach (var item in forecastFor7Days.Forecasts)
            {
                weatherView.Forecast.Add(new WeatherParameters(UnixTimeStampToDateTime(item.dt).ToShortDateString(), item.temp.day, item.temp.min, item.temp.max,
                                        item.humidity, item.pressure, item.speed, WindDegreesToCompas(item.deg), item.clouds, item.weather[0].description, item.weather[0].icon));
            }
            return weatherView;
        }


        // Ниже идут вспомогательные методы


        // метод преобразования даты из POSIX формата в DateTime
        private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        // Переводим направление ветра из градусов в чатабельный формат
        private static string WindDegreesToCompas(double deg)
        {
            string[] windDirections = { "северный", "северный, северо-восточный", "северо-восточный", "восточный, северо-восточный", "восточный", "восточный, юго-восточный", "юго-восточный", "южный, юго-восточный", "южный", "южный, юго-западный", "юго-западный", "западный, юго-западный", "западный", "западный, северо-западный", "северо-западный", "северный, северо-западный" };
            return windDirections[(int)Math.Round((deg - 11.25) / 22.5)];
        }

        // Транслитерируем название города, введеного пользователем
        private static string Translit(string s)
        {
            StringBuilder ret = new StringBuilder();
            string[] rus = { "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й",
          "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц",   
          "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я" };
            string[] eng = { "a", "b", "v", "h", "d", "e", "e", "zh", "z", "i", "y", 
          "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "kh", "ts",   
          "ch", "sh", "shch", null, "y", null, "e", "yu", "ya" };

            for (int j = 0; j < s.Length; j++)
                for (int i = 0; i < rus.Length; i++)
                {
                    if (s.Substring(j, 1) == rus[i]) { ret.Append(eng[i]); continue; }
                    return s;
                }
            return ret.ToString();
        }

        // отравляем запрос на сервис погоды, получаем отвем в  JSON
        private static string GetJson(Uri uri)
        {
            string jsonStr;
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                try
                {
                    jsonStr = client.DownloadString(uri);
                }
                catch (Exception)
                {                   
                    throw;
                }
            }
            return jsonStr;
        }
    }
}