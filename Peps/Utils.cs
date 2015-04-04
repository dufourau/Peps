﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Peps
{
    public static class Utils
    {
        public static double[][] parseFileToMatrix(String file, List<String> dates)
        {
            String[] lines = file.Split('\n').Where(x => x != "" && x != null).ToArray();
            double[][] parsed = new double[lines.Length][];
            String[] symbols = lines[0].Trim().Split(' ');
            for (int i = 1; i < lines.Length; i++)
            {
                String[] items = lines[i].Trim().Split(' ');
                parsed[i - 1] = new double[items.Length - 1];
                if(dates !=null) dates.Add(items[0]);
                for (int j = 1; j < items.Length; j++)
                {
                    parsed[i - 1][j - 1] = double.Parse(items[j], System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            return parsed;
        }

        public static DateTime createDateTime(String year, String month, String day)
        {
            int intYear = Int32.Parse(year, CultureInfo.InvariantCulture);
            int intMonth = Int32.Parse(month, CultureInfo.InvariantCulture);
            int intDay = Int32.Parse(day, CultureInfo.InvariantCulture);
            return new DateTime(intYear, intMonth, intDay);

        }

        public static double[][] parseFileToMatrix(String file)
        {
            String[] lines = file.Split('\n').Where(x => x != "" && x != null).ToArray();
            double[][] parsed = new double[lines.Length][];
            String[] symbols = lines[0].Trim().Split(' ');
            for (int i = 1; i < lines.Length; i++)
            {
                String[] items = lines[i].Trim().Split(' ');
                parsed[i - 1] = new double[items.Length];
                for (int j = 0; j < items.Length; j++)
                {
                    parsed[i - 1][j] = double.Parse(items[j], System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            return parsed;
        }

        public static double[] parseFileToArray(String file)
        {
            String[] lines = file.Split('\n').Where(x => x != "" && x != null).ToArray();
            double[] parsed = new double[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                parsed[i] = double.Parse(lines[i], System.Globalization.CultureInfo.InvariantCulture);
            }
            return parsed;
        }

        public static int[] convertToInt(String[] stringArray)
        {
            int[] intArray = new int[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++)
            {
                intArray[i] = int.Parse(stringArray[i], System.Globalization.CultureInfo.InvariantCulture);
            }
            return intArray;
        }

        public static List<string> parseDates(String file)
        {
            List<String> dates = new List<String>();
            String[] lines = file.Split('\n').Where(x => x != "" && x != null).ToArray();
            for (int i = 1; i < lines.Length; i++)
            {
                String[] items = lines[i].Trim().Split(' ');
                dates.Add(items[0]);
            }
            return dates;
        }

        public static double[] Convert2dArrayto1d(double[,] previousStocksPrices)
        {

            int k = 0;
            double[] oneDpreviousStockPrices = new double[previousStocksPrices.GetLength(0) * previousStocksPrices.GetLength(1)];

            for (int i = 0; i < previousStocksPrices.GetLength(0); i++)
            {
                for (int j = 0; j < previousStocksPrices.GetLength(1); j++)
                {
                    oneDpreviousStockPrices[k++] = previousStocksPrices[i, j];
                }
            }
            return oneDpreviousStockPrices;

        }
    }
}