using System;
using System.Web.Mvc;
using weatherForecastApp.Models;
using weatherForecastApp.Services;

namespace weatherForecastApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(int cityId, int days)
        {
            WeatherView weatherView = new WeatherView();
            // Ловим исключения - если не получилось достучаться до сервиса погоды, то переход на страницу ошибки
            try
            {
                // Если (cityId == 0) - показываем погоду для города по-умолчанию, в данном случае - для Киева
                if (cityId == 0)
                {
                    return View(WeatherService.GetCurrentWeatherByCityId(703448));
                }

                // Выбираем на какое кол-во дней показать прогноз
                switch (days)
                {
                    case 1:
                        weatherView = WeatherService.GetCurrentWeatherByCityId(cityId);
                        break;
                    case 3:
                        weatherView = WeatherService.GetWeatherFor3DaysByCityId(cityId);
                        break;
                    case 7:
                        weatherView = WeatherService.GetWeatherFor7DaysByCityId(cityId);
                        break;
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error"); ;
            }

            return View(weatherView);
        }


        // POST: Home/Index
        [HttpPost]
        public ActionResult Index(string cityName)
        {
            // Если из поля ввода пришла пустая строка, то возвращаем ту же самую страницу
            if (cityName=="")
            {
                return RedirectToAction("Index");
            }

            WeatherView weatherView = new WeatherView();
            // Ловим исключения - если не получилось достучаться до сервиса погоды, то переход на страницу ошибки
            try
            {
                int cityId = (int)WeatherService.GetCityIdByName(cityName);
                // (cityId == 0) означает что сервис погоды не смог разобрать название города. Переходим на страницу ошибки
                if (cityId == 0)
                {
                    return RedirectToAction("Error");
                }
                weatherView = WeatherService.GetCurrentWeatherByCityId(cityId);
            }
            catch (Exception)
            {
                return RedirectToAction("Error");
            }

            return View(weatherView);
        }

        public ActionResult Error(string mess)
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}