using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace weatherForecastApp.Models
{
    public class WeatherView
    {
        public string CityName { get; set; }
        public int CityId { get; set; }
        public Coord Coordinates { get; set; }
        public List<WeatherParameters> Forecast { get; set; }
        public WeatherView(string name, int id, double lonCoord, double latCoord)
        {
            CityName = name;
            CityId = id;
            Coordinates = new Coord() { lat = latCoord, lon = lonCoord };
            Forecast = new List<WeatherParameters>();
        }

        public WeatherView()
        {  
        }

    }
    public class WeatherParameters
    {
        public string DateForForecast { get; set; }
        public double Temperature { get; set; }
        public double TemperatureMin { get; set; }
        public double TemperatureMax { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public double WindSpeed { get; set; }
        public string WindDirection { get; set; }
        public int Clouds { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public WeatherParameters(string date, double temp, double tempMin, double tempMax,
                    double humid, double pressure, double speed, string direct, int clouds, string descr, string icon)
        {
            DateForForecast = date;
            Temperature = Math.Round(temp, 1);
            TemperatureMin = tempMin;
            TemperatureMin = tempMin;
            Humidity = humid;
            Pressure = pressure;
            WindSpeed = Math.Round(speed);
            WindDirection = direct;
            Clouds = clouds;
            Description = descr;
            Icon = String.Format("{0}{1}{2}", "http://openweathermap.org/img/w/", icon, ".png");
        }
    }
}