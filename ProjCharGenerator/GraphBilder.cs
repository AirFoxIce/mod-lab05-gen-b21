using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class GraphBuilder
{
    public static void Build(
        Dictionary<string, double> expected,
        Dictionary<string, double> actual,
        string projectDirectory,
        string filename,
        string title,
        string xlabel)
    {
        if (actual.Count == 0)
        {
            Console.WriteLine("Нет данных для построения графика");
            return;
        }

        var plot = new Plot();

        var labels = new List<string>(expected.Keys);
        labels.AddRange(actual.Keys);
        labels = new HashSet<string>(labels).ToList();
        labels.Sort();

        int count = Math.Min(labels.Count, 100);
        double[] ysExpected = new double[count];
        double[] ysActual = new double[count];

        for (int i = 0; i < count; i++)
        {
            if (expected.TryGetValue(labels[i], out double val1))
                ysExpected[i] = val1;
            if (actual.TryGetValue(labels[i], out double val2))
                ysActual[i] = val2;
        }

        int k = 1;
        for (int i = 0; i < count; i++)
        {
            double[] positions = { k + 0.5, k + 1.3 };
            double[] values = { ysActual[i], ysExpected[i] };

            var bars = plot.Add.Bars(positions, values);
            bars.Bars[0].FillColor = Colors.Green;
            bars.Bars[1].FillColor = Colors.Red;
            k += 4;
        }

        var tickGen = new NumericManual();
        int j = 0;
        for (int x = 1; x <= 4 * count; x += 4)
        {
            tickGen.AddMajor(x + 1.0, labels[j]);
            j++;
        }
        plot.Axes.Bottom.TickGenerator = tickGen;
        plot.Axes.Bottom.TickLabelStyle.Rotation = -45;
        plot.Axes.Bottom.TickLabelStyle.FontSize = 10;
        plot.Axes.Bottom.TickLabelStyle.Alignment = Alignment.MiddleRight;

        plot.YLabel("Частота");
        plot.XLabel(xlabel);
        plot.Title(title);
        plot.ShowLegend(new[]
        {
            new LegendItem { LabelText = "Ожидаемые", LineColor = Colors.Red, LineWidth = 10, MarkerShape = MarkerShape.None },
            new LegendItem { LabelText = "Фактические", LineColor = Colors.Green, LineWidth = 10, MarkerShape = MarkerShape.None }
        });

        string resultsDir = Path.Combine(Directory.GetParent(projectDirectory).FullName, "Results");
        Directory.CreateDirectory(resultsDir);
        string path = Path.Combine(resultsDir, filename + ".png");

        plot.SavePng(path, 2000, 500);
        Console.WriteLine("График построен: " + path);
    }
}
