using System;
namespace weatherForecastApp.Services
{
    interface IRequestSender
    {
        string SendRequest(string request);
    }
}
