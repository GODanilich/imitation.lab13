using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
namespace lab_2__exchange_rates_
{

    public partial class Form1 : Form
    {
        Random rnd = new Random();
        double BoxMuller(double mean, double variance)
        {
            double z = Math.Cos(2 * Math.PI * rnd.NextDouble()) * Math.Sqrt(-2 * Math.Log(rnd.NextDouble()));
            return mean + z * Math.Sqrt(variance);
        }

        private ChartArea chartArea;
        private Timer timer = new Timer();
        private int currentIndex = 0;
        private double rate_USD = 92.05;
        private double rate_EURO = 99.49;
        private int range;
        private double drift = 0.01; // Средний темп роста (дрейф)
        private double volatility = 0.05; // Волатильность
        List<double> stat_EURO = new List<double>();
        List<double> stat_USD = new List<double>();

        public Form1()
        {
            InitializeComponent();
            timer.Tick += Timer_Tick;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {


            int.TryParse(forecastRange.Text, out int deltaRange);
            range += deltaRange;
            
            double dt = 0.1;

            for (int i = 0; i < deltaRange; i++)
            {
                stat_USD.Add(rate_USD);

                double Z_USD = BoxMuller(0, 1);
                rate_USD *= Math.Exp((drift - 0.5 * volatility * volatility) * dt + volatility * Math.Sqrt(dt) * Z_USD);

                stat_EURO.Add(rate_EURO);

                double Z_EURO = BoxMuller(0, 1);
                rate_EURO *= Math.Exp((drift - 0.5 * volatility * volatility) * dt + volatility * Math.Sqrt(dt) * Z_EURO);
            }

            // Настройка графика
            chartArea = chart1.ChartAreas[0];

            // Установка минимального значения по оси X
            chartArea.AxisX.Minimum = 0;
            // Установка максимального значения по оси X
            chartArea.AxisX.Maximum = range;
            // Установка размера шага по оси X
            chartArea.AxisX.Interval = (range < 50) ? 1.0 : range / 50.0;

            // Установка минимального значения по оси Y
            chartArea.AxisY.Minimum = Math.Min(stat_EURO.Min(), stat_USD.Min());
            // Установка максимального значения по оси Y
            chartArea.AxisY.Maximum = Math.Max(stat_EURO.Max(), stat_USD.Max());
            // Установка размера шага по оси Y
            chartArea.AxisY.Interval = chartArea.AxisY.Maximum / 100;

            // Инициализация таймера
            timer.Interval = 5000 / deltaRange;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Обработчик события таймера

            // Проверка на выход за пределы списка
            if (currentIndex < range)
            {
                // Ваш код обработки текущей точки, например, вывод в консоль
                chart1.Series[0].Points.AddXY(currentIndex, stat_USD[currentIndex]);
                chart1.Series[1].Points.AddXY(currentIndex, stat_EURO[currentIndex]);

                // Увеличение индекса для следующей итерации
                currentIndex++;
            }
            else
            {
                // Если все точки обработаны, остановите таймер
                timer.Stop();
            }
        }
    }
}
