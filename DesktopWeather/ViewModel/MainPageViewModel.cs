using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopWeather.Model;
using DesktopWeather.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;

namespace DesktopWeather.ViewModel
{
    partial class MainPageViewModel : ObservableObject
    {
        static Timer timer = new(1000);
        Requests requests = new();
        Window? mainWindow;
        Config? config;

        string location = "";

        bool initSuccess = false;
        bool requestSuccess = false;

        string lastUpdateTime = "";
        string lastRequestTime = "";

        [ObservableProperty]
        ObservableCollection<DailyForecastInfo> dailyForecastInfos = new()
        {
            new(){ MinTemperature="—℃", MaxTemperature="—℃", Date="—", WeatherIcon="../Icon/CLOUDY.png" },
            new(){ MinTemperature="—℃", MaxTemperature="—℃", Date="—", WeatherIcon="../Icon/CLOUDY.png" },
            new(){ MinTemperature="—℃", MaxTemperature="—℃", Date="—", WeatherIcon="../Icon/CLOUDY.png" },
            new(){ MinTemperature="—℃", MaxTemperature="—℃", Date="—", WeatherIcon="../Icon/CLOUDY.png" },
        };

        [ObservableProperty]
        RealTimeInfo realTimeWeatherInfo = new()
        {
            Temperature = "—℃",
            WindDescription = "—",
            Humidity = "—%",
            UpdateTime = "Update Time: —",
            WeatherIcon = "../Icon/CLOUDY.png"
        };

        [ObservableProperty]
        ObservableCollection<HourlyForecastInfo> hourlyForecastInfos = new()
        {
            new(){Temperature="—℃", WeatherIcon="../Icon/CLOUDY.png", Time="—"},
            new(){Temperature="—℃", WeatherIcon="../Icon/CLOUDY.png", Time="—"},
            new(){Temperature="—℃", WeatherIcon="../Icon/CLOUDY.png", Time="—"},
            new(){Temperature="—℃", WeatherIcon="../Icon/CLOUDY.png", Time="—"},
            new(){Temperature="—℃", WeatherIcon="../Icon/CLOUDY.png", Time="—"},
            new(){Temperature="—℃", WeatherIcon="../Icon/CLOUDY.png", Time="—"},
            new(){Temperature="—℃", WeatherIcon="../Icon/CLOUDY.png", Time="—"},
            new(){Temperature="—℃", WeatherIcon="../Icon/CLOUDY.png", Time="—"},
        };

        [ObservableProperty]
        Visibility locationErrorShow = Visibility.Visible;

        [RelayCommand]
        void Loaded(RoutedEventArgs e)
        {
            mainWindow = (Window)e.Source;
            SetWindowToDesktop.setWindowToWorkerW1(mainWindow);

            LoadConfig();
            SetWindowToDesktop.MoveWindow(mainWindow!, config!.X, config!.Y);
            Task.Run(() =>
            {
                Update();
                TimerStart();
            });
        }

        [RelayCommand]
        void OpenConfig()
        {
            Process.Start(config!.EditorPath!, Path.Combine(Environment.CurrentDirectory, "Config", "config.json"));
        }

        [RelayCommand]
        void ReloadConfig(bool isCompletely)
        {
            LoadConfig();
            SetWindowToDesktop.MoveWindow(mainWindow!, config!.X, config!.Y);
            if (isCompletely)
            {
                Task.Run(() =>
                {
                    Update();
                });
            }
        }

        [RelayCommand]
        void Close()
        {
            mainWindow?.Close();
        }

        void GetWeatherData()
        {
            try
            {
                string url;
                HttpContent response;

                // 地理位置位置校验
                if (config!.LocationCorrection)
                {
                    string ip = GetIP.getIP(requests);

                    url = $"https://restapi.amap.com/v3/ip?key={config!.AmapToken}&ip={ip}";
                    response = requests.get(url);
                    JObject ipLocationInfo = JObject.Parse(response.ReadAsStringAsync().Result);
                    string ipCity = ipLocationInfo["city"].Value<string>();

                    url = $"https://restapi.amap.com/v3/geocode/regeo?key={config!.AmapToken}&location={config!.ConfigLocation}";
                    response = requests.get(url);
                    string configCity = JObject.Parse(response.ReadAsStringAsync().Result)["regeocode"]["addressComponent"]["city"].Value<string>();

                    if (ipCity != configCity)
                    {
                        LocationErrorShow = Visibility.Visible;
                        location = DegreeConvert.calcCenter(ipLocationInfo["rectangle"].Value<string>());
                    }
                    else
                    {
                        LocationErrorShow = Visibility.Hidden;
                        location = config.ConfigLocation!;
                    }
                }
                else
                {
                    location = config.ConfigLocation!;
                    LocationErrorShow = Visibility.Hidden;
                }

                // 获取天气
                url = $"https://api.caiyunapp.com/v2.6/{config!.WeatherToken}/{location}/weather?dailysteps=4&hourlysteps=8";
                response = requests.get(url);

                requestSuccess = true;

                JToken result = JObject.Parse(response.ReadAsStringAsync().Result)["result"];
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // 天级别预报
                    JToken dailyTemperature = result["daily"]["temperature"];
                    JToken dailySkycon = result["daily"]["skycon"];

                    DailyForecastInfos.Clear();
                    for (int i = 0; i < 4; i++)
                    {
                        DailyForecastInfos.Add(new()
                        {
                            MaxTemperature = $"{dailyTemperature[i]["max"].Value<int>()}℃",
                            MinTemperature = $"{dailyTemperature[i]["min"].Value<int>()}℃",
                            WeatherIcon = $"../Icon/{dailySkycon[i]["value"].Value<string>()}.png",
                            Date = DateTime.Parse(dailySkycon[i]["date"].Value<string>()).ToString("yyyy-MM-dd"),
                        });
                    }

                    // 实况
                    RealTimeWeatherInfo = new()
                    {
                        WeatherIcon = $"../Icon/{result["realtime"]["skycon"].Value<string>()}.png",
                        Temperature = $"{result["realtime"]["temperature"].Value<int>()}℃",
                        WindDescription = $"{WindConvert.SpeedToDesc(result["realtime"]["wind"]["speed"].Value<int>())} {WindConvert.CornerToDirection(result["realtime"]["wind"]["direction"].Value<float>())}",
                        Humidity = $"{Convert.ToInt32(result["realtime"]["humidity"].Value<float>() * 100)}%",
                        UpdateTime = $"Update Time: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")}"
                    };

                    // 小时级预报
                    JToken hourlyTemperature = result["hourly"]["temperature"];
                    JToken hourlySkycon = result["hourly"]["skycon"];

                    HourlyForecastInfos.Clear();
                    for (int i = 0; i < 8; i++)
                    {
                        HourlyForecastInfos.Add(new()
                        {
                            Temperature = $"{hourlyTemperature[i]["value"].Value<int>()}℃",
                            WeatherIcon = $"../Icon/{hourlySkycon[i]["value"].Value<string>()}.png",
                            Time = DateTime.Parse(hourlySkycon[i]["datetime"].Value<string>()).ToString("HH:mm"),
                        });
                    }
                });
            }
            catch (AggregateException)
            {
                requestSuccess = false;
            }
        }

        void Update()
        {
            DateTime now = DateTime.Now;
            lastRequestTime = now.ToString("yyyy-MM-dd HH:mm");
            GetWeatherData();
            if (requestSuccess)
            {
                lastUpdateTime = now.ToString("yyyy-MM-dd HH:mm");
                if (!initSuccess)
                {
                    initSuccess = true;
                }
            }
        }

        void LoadConfig()
        {
            FileStream fileStream = new FileStream(Path.Combine(Environment.CurrentDirectory, "Config", "config.json"), FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);
            string json = streamReader.ReadToEnd();

            config = JsonConvert.DeserializeObject<Config>(json);
            config.ConfigLocation = DegreeConvert.convert(config.Location!);
        }

        void TimerStart()
        {
            timer.Elapsed += new ElapsedEventHandler(TimerExecute);
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
        }

        void TimerExecute(object? source, ElapsedEventArgs e)
        {
            timer.Stop();
            
            DateTime now = DateTime.Now;
            if ((!initSuccess) ||
                ((now - DateTime.Parse(lastUpdateTime)).Minutes >= 10) ||
                (!requestSuccess && (now - DateTime.Parse(lastRequestTime)).Minutes >= 1) ||
                (now.ToString("yyyy-MM-dd HH:mm") != lastUpdateTime && now.ToString("mm")[1] == '0'))
            {
                Task.Run(() =>
                {
                    Update();
                });
                
            }
            
            timer.Start();
        }
    }
}
