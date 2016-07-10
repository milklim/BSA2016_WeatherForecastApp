using System.Collections.Generic;
using System.Web.Mvc;
using weatherForecastApp.Models;
using weatherForecastApp.Services;

namespace weatherForecastApp.Controllers
{
    public class HomeController : Controller
    {
        private IWeatherService wService;
        public HomeController(IWeatherService wServiceParam)
        {
            wService = wServiceParam;
        }


        public ActionResult Index(string cityName, int days)
        {
            List<string> cities = new List<string> { "Днепропетровск", "Киев", "Львов", "Одесса", "Харьков" };
            ViewBag.cityList = cities; ;
              
            switch (days)
            {
                case 1:
                    RedirectToAction("CurrentWeather", cityName);
                    break;
                case 3:
                    RedirectToAction("Forecast3Days", cityName);
                    break;
                case 7:
                    RedirectToAction("ForecastForWeek", cityName);
                    break;
            }


            return View(cities);
        }

        public ActionResult CurrentWeather(string cityName)
        {
            CurrentWeather wView = wService.GetForecast(cityName, TypeOfForecast.CurrentWeather) as CurrentWeather;
            return View(wView);
        }

    
        public ActionResult Forecast3Days(string cityName)
        {
            WeatherForecast3DaysDetailed wView = wService.GetForecast(cityName, TypeOfForecast.For3Days) as WeatherForecast3DaysDetailed;
            return View(wView);
        }


        public ActionResult Forecast7Days(string cityName)
        {
            WeatherForecast7Days wView = wService.GetForecast(cityName, TypeOfForecast.For7Days) as WeatherForecast7Days;
            return View(wView);
        }

        [HttpPost]
        public ActionResult PostRequestHandler(string cityName)
        {
            return RedirectToAction("CurrentWeather", cityName);
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