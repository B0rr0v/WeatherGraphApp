using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls;

namespace WeatherGraphApp
{
    public partial class MainWindow : Window
    {
        private readonly Random _random = new Random();
        private readonly double maxTemperature = 40;
        private double[] _temperatures;
        private int _currentIndex = 0;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            GenerateTemperatures();
            StartGraphUpdate();
            StartClockUpdate();
        }

        private void StartGraphUpdate()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (sender, args) => UpdateGraph();
            _timer.Start();
        }
        private void StartClockUpdate()
        {
            DispatcherTimer clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            clockTimer.Tick += (sender, args) =>
            {
                CurrentTimeTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
            };
            clockTimer.Start();
        }
        
        private void GenerateTemperatures()
        {
            _temperatures = RandomTempForSochi.GenerateTemperatures();
        }

        private void UpdateGraph()
        {
            // обновление графика и перевод его в цикл

            if (_currentIndex >= _temperatures.Length)
            {
                WeatherCanvas.Children.Clear();
                _currentIndex = 0;
                GenerateTemperatures();
            }

            // получение текущей темп погоды
            double currentTemperature = _temperatures[_currentIndex];
            double yPosition = (WeatherCanvas.Height - (currentTemperature / maxTemperature) * WeatherCanvas.Height) + 40;
            double xPosition = _currentIndex * 40;

            // точка на канвасе
            Ellipse point = new Ellipse
            {
                Fill = Brushes.Blue,
                Width = 5,
                Height = 5
            };
            Canvas.SetLeft(point, xPosition);
            Canvas.SetTop(point, yPosition);
            WeatherCanvas.Children.Add(point);

            // текст темп для канваса
            TextBlock temperatureLabel = new TextBlock
            {
                Text = $"{currentTemperature:F1}°C",
                FontSize = 12,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(temperatureLabel, xPosition - 10);
            Canvas.SetTop(temperatureLabel, yPosition - 20);
            WeatherCanvas.Children.Add(temperatureLabel);
            CurrentWeatherText.Text = "Текущая температура в сочи: " + Math.Round(currentTemperature, 1).ToString() + "°C";
            

            // соедининей точек через линию (если не первая точка)
            if (_currentIndex > 0)
            {
                double previousTemperature = _temperatures[_currentIndex - 1];
                double previousY = (WeatherCanvas.Height - (previousTemperature / maxTemperature) * WeatherCanvas.Height) + 40;
                double previousX = (_currentIndex - 1) * 40;

                Line line = new Line
                {
                    X1 = previousX,
                    Y1 = previousY,
                    X2 = xPosition,
                    Y2 = yPosition,
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2
                };
                WeatherCanvas.Children.Add(line);
            }
            _currentIndex++;
        }

        private void CleanupOldElements()
        {
            double canvasWidth = WeatherCanvas.ActualWidth;

            for (int i = WeatherCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (WeatherCanvas.Children[i] is FrameworkElement element)
                {
                    double leftPosition = Canvas.GetLeft(element);

                    if (leftPosition < -50) // нет смысла менять. Работает и пусть. Тут если точка вышла за пределы, то удалять её
                    {
                        WeatherCanvas.Children.RemoveAt(i);
                    }
                }
            }
        }
    }

    static class RandomTempForSochi
    {
        static double _maxTemp = 15;
        static double _minTemp = 15;
        static Random rnd =new Random();
        static double[] _temperatures = new double[20];

        public static double[] GenerateTemperatures()
        {
            // Генерация массива температур

            double min = 12.0; // мин знач для темп
            double max = 15.0; // макс знач для темп

            _temperatures = new double[20];
            for (int i = 0; i < _temperatures.Length; i++)
            {
                _temperatures[i] = min + rnd.NextDouble() * (max - min);
            }

            return _temperatures;
        }
    }

   
}
