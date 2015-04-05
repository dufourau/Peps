using ServiceStack.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

namespace Peps
{
    public class MarketData
    {
        //All asset prices
        Dictionary<String, SortedList<DateTime, double>> marketDataDictionary;
        //All the dates
        public SortedSet<DateTime> dates;
        // List of symbols including currencies symbols
        private List<string> symbolList;
        // List of currency symbols (exchange rates)
        private List<string> currencySymbols;
        //All the rates
        public double[][] rates;

        private DateTime firstDBAvailableDate;
        private DateTime lastDBAvailableDate;

        public DateTime minimumDateToRetrieve = new DateTime(2005, 6, 30);

        public static string urlPrototype = @"http://ichart.finance.yahoo.com/table.csv?s={0}&a={1}&b={2}&c={3}&d={4}&e={5}&f={6}&g={7}&ignore=.csv";
        public static string currencyURL = @"https://www.quandl.com/api/v1/datasets/CURRFX/{0}.csv?&trim_start={1}&trim_end={2}";
        public static string dateFormat = @"yyyy-MM-dd";

        public MarketData()
        {
            marketDataDictionary = new Dictionary<String, SortedList<DateTime, double>>();
            dates = new SortedSet<DateTime>();
            // Initialize symbol list
            symbolList = new List<string>();
            currencySymbols = new List<string>();
            foreach (PropertyInfo property
                in typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.Name.Length == 12)
                {
                    symbolList.Add(Properties.Resources.ResourceManager.GetString(property.Name).Split(';')[1]);
                }
                
            }

            foreach (PropertyInfo property
                in typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {

                if (property.Name.Substring(0, 2).Equals("Fx"))
                {
                    symbolList.Add(property.Name.Substring(2));
                    currencySymbols.Add(property.Name.Substring(2));
                }
            }
            // Initialize dictionary
            foreach (string symbol in symbolList)
            {
                marketDataDictionary[symbol] = new SortedList<DateTime, double>();
            }
            // Initialize rates
            this.rates = Utils.parseFileToMatrix(Properties.Resources.rate, null);
        }

        public ArrayList getAllPricesAtDate(DateTime date)
        {
            ArrayList prices = new ArrayList();
            foreach (string symbol in symbolList)
            {
                prices.Add(marketDataDictionary[symbol][date]);
            }
            return prices;
        }

        public void loadAllStockPrices(){

            string stringFirstDateInDatabase, stringLastDateInDatabase;

            DateTime today = DateTime.Today;

            using (var redis = new RedisClient(Properties.Settings.Default.RedisDatabaseURL))
            {
                stringFirstDateInDatabase = redis.GetValue("firstDate");
                stringLastDateInDatabase = redis.GetValue("lastDate");
            }

            if (stringFirstDateInDatabase != null) firstDBAvailableDate = DateTime.ParseExact(stringFirstDateInDatabase, dateFormat, null);
            if (stringLastDateInDatabase != null) lastDBAvailableDate = DateTime.ParseExact(stringLastDateInDatabase, dateFormat, null);

            // Market data is already in our database
            if (stringFirstDateInDatabase != null && stringLastDateInDatabase != null)
            {
                if (lastDBAvailableDate.CompareTo(today) == -1)
                {
                    loadMarketDataFromWeb(lastDBAvailableDate, today);
                    lastDBAvailableDate = today;
                    dumpToRedis(lastDBAvailableDate, today);
                }

                foreach (string symbol in symbolList)
                {
                    using (var redis = new RedisClient(Properties.Settings.Default.RedisDatabaseURL))
                    {
                        Dictionary<string, string> dataForSymbol = redis.GetAllEntriesFromHash(symbol);
                        SortedList<DateTime, double> parsedData = new SortedList<DateTime,double>();
                        foreach (KeyValuePair<string, string> pair in dataForSymbol){
                            DateTime date = DateTime.ParseExact(pair.Key, dateFormat, null);
                            parsedData.Add(date, Double.Parse(pair.Value.Replace(',','.'), CultureInfo.InvariantCulture));
                            dates.Add(date);
                        }
                        marketDataDictionary[symbol] = parsedData;
                    }
                }
            }
            else
            {
                loadMarketDataFromWeb(minimumDateToRetrieve, today);
                firstDBAvailableDate = minimumDateToRetrieve;
                lastDBAvailableDate = today;
                dumpToRedis(minimumDateToRetrieve, today);
            }
        }
               
        private void loadMarketDataFromWeb(DateTime startDate, DateTime endDate)
        {
            SortedList<DateTime, double> symbolData;
            foreach (string symbol in symbolList)
            {
                symbolData = marketDataDictionary[symbol];
                loadSymbolFromWeb(symbol, startDate, endDate);
            }  
        }  

        public void loadSymbolFromWeb(string symbol, DateTime startDate, DateTime endDate)
        {
            try
            {
                string url, urlFormated;
                if (currencySymbols.Contains(symbol))
                {
                    url = currencyURL;
                    urlFormated = string.Format(url, symbol, startDate.ToString(dateFormat), endDate.ToString(dateFormat));
                } else {
                    url = urlPrototype;
                    urlFormated = string.Format(url, symbol, startDate.Month, startDate.Day, startDate.Year,
                        endDate.Month, endDate.Day, endDate.Year, "d");
                }
                var webClient = new WebClient();
                String csvData = webClient.DownloadString(urlFormated);
                string[] rows = csvData.Replace("\r", "").Split('\n');
                for (int i = rows.Length - 2; i >= 1; i--)
                {   
                    string[] splittedRow = rows[i].Split(',')[0].Split('-');
                    int year = Int32.Parse(splittedRow[0], CultureInfo.InvariantCulture);
                    int month = Int32.Parse(splittedRow[1], CultureInfo.InvariantCulture);
                    int day = Int32.Parse(splittedRow[2], CultureInfo.InvariantCulture);
                    DateTime date = new DateTime(year, month, day);
                    this.dates.Add(date);
                    int index = 6;
                    if (currencySymbols.Contains(symbol)) index = 1;
                    marketDataDictionary[symbol][date] = Double.Parse(rows[i].Split(',')[index], CultureInfo.InvariantCulture);
                    }                        
                }
            catch
            {
                Console.WriteLine("No files for the stock " + symbol + " for the specified date range founded on the server.");
            }
        }

        public List<double> getPricesRange(string symbol, DateTime startDate, DateTime endDate)
        {
            ArrayList prices = new ArrayList();
            SortedList<DateTime, double> pricesDictionary = marketDataDictionary[symbol];
            int startIndex = pricesDictionary.IndexOfKey(startDate);
            int endIndex = pricesDictionary.IndexOfKey(endDate);
            return pricesDictionary.Values.ToList().GetRange(startIndex, endIndex - startIndex);
        }

        public double getStockPriceFromWeb(string symbol, DateTime date)
        {
            try
            {
                string url = string.Format(urlPrototype, symbol, date.Month, date.Day, date.Year, date.Month, date.Day + 1, date.Year, "d");
                var webClient = new WebClient();
                String csvData = webClient.DownloadString(url);
                string[] rows = csvData.Replace("\r", "").Split('\n');
                return Double.Parse(rows[1].Split(',')[6], CultureInfo.InvariantCulture);
            }
            catch
            {
                Console.WriteLine("No files for the stock " + symbol + " for the specified date range founded on the server.");
                return -1;
            }
        }

        public double getPrice(string symbol, DateTime date)
        {
            return marketDataDictionary[symbol][date];
        }

        public double getLastCurrencyPriceFromWeb(string symbol, DateTime startDate, DateTime endDate)
        {
            try
            {
                string url = string.Format(currencyURL, symbol, startDate.ToString(dateFormat), startDate.AddDays(1).ToString(dateFormat));
                var webClient = new WebClient();
                string csvData = webClient.DownloadString(url);
                string[] rows = csvData.Replace("\r", "").Split('\n');
                return Double.Parse(rows[1].Split(',')[1], CultureInfo.InvariantCulture);
            }
            catch
            {
                Console.WriteLine("No files for the currency " + symbol + " for the specified date range founded on the server.");
                return -1;
            }
        }

        public void dumpToRedis(DateTime startDate, DateTime endDate)
        {
            using(var redis = new RedisClient(Properties.Settings.Default.RedisDatabaseURL))
            {
                SortedList<DateTime, double> symbolData;
                foreach (string symbol in symbolList)
                {
                    symbolData = marketDataDictionary[symbol];
                    foreach (KeyValuePair<DateTime, double> pair in symbolData)
                    {
                        if (pair.Key.CompareTo(startDate) == 1 && pair.Key.CompareTo(endDate) <= 0)
                        {
                            redis.SetEntryInHash(symbol, pair.Key.ToString(dateFormat), pair.Value.ToString());
                        }
                    }
                }
                redis.SetEntry("firstDate", dates.Min.ToString(dateFormat));
                redis.SetEntry("lastDate", dates.Max.ToString(dateFormat));
            }
        }

        public DateTime getNextDate(DateTime date){
            List<DateTime> datesList = dates.ToList();
            int index = datesList.IndexOf(date);
            if (index == -1 || index >= datesList.Count) return date;
            return datesList[index + 1];
        }
    }
}