using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Peps
{
    public class MarketData
    {
        public Hashtable table;
        public Hashtable currSymbol;
        public List<String> Symbols;
        public List<String> CurrSymbols;
        public int sizeList;
        public MarketData(){
            table = new Hashtable();
            currSymbol = new Hashtable(); 
            Symbols= new List<String>();
            CurrSymbols = new List<String>();
            sizeList = 2371;
        }

        public void storeData(){
            String[][] temp= new String[table.Keys.Count+1][];
            Symbols.AddRange(CurrSymbols);
            
            int i = 1;
            foreach (String sym in Symbols)
            {
                if(i==1){
                    temp[0]= new String[((List <Price>) table[sym]).Count+1];
                }
                
                temp[i]= new String[((List <Price>) table[sym]).Count+1];
                temp[i][0] = (String) sym;
                int j = 1;
                foreach (Price price in ((List<Price>)table[sym]))
                {
                    if(i==1){
                         temp[0][j]= price.d.ToString()+"-"+price.m.ToString()+"-"+price.y.ToString();
                    }
                    temp[i][j] = ((price.High + price.Low) / 2).ToString();
                    j++;
                }
                i++;
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Dufourau\Desktop\market.txt"))
            
            for (int z = 0; z < temp[0].Length; z++ )
            {
                String tempStr = "";
                for (int k = 0; k < temp.Length; k++)
                {
                    if(z<temp[k].Length){
                        tempStr += temp[k][z] + " ";
                    }   
                }
                    file.WriteLine(tempStr);
            }
        }
        public void fixeDataSize()
        {
            foreach (String sym in Symbols)
            {
                int diff= ((List <Price>) table[sym]).Count -sizeList;
                if(diff<0){
                    List<Price> temp = ((List<Price>)table[sym]).GetRange(((List<Price>)table[sym]).Count+diff, -diff);
                    ((List<Price>)table[sym]).AddRange(temp); 
                }
            }
        }

        /*
         * Use it to retrieve stock price on yahoofinance
         * a et d: start and end month minus 1
         * b et e: start and end day 
         * c et f: start and end year
         */
        public void retrieveDataFromWeb(String a, String b, String c, String d, String e, String f){
            //Jusque norvatis
            //Euro
            Symbols.Add("IBDRY");
            //Euro
            Symbols.Add("ENLAY");
            //Livre
            currSymbol.Add("HSEA","GBPEUR");
            Symbols.Add("HSEA");
            //Livre
            currSymbol.Add("DEO","GBPEUR");
            Symbols.Add("DEO");
            //Livre
            currSymbol.Add("TSCDF","GBPEUR");
            Symbols.Add("TSCDF");
            //Dollars
            currSymbol.Add("WFC","USDEUR");
            Symbols.Add("WFC");
            //Dollars
            currSymbol.Add("PEP", "USDEUR");
            Symbols.Add("PEP");
            //CHF
            currSymbol.Add("NVS", "CHFEUR");
            Symbols.Add("NVS");      
            //Add the currency
          
            CurrSymbols.Add("GBPEUR");           
            CurrSymbols.Add("USDEUR");
            CurrSymbols.Add("CHFEUR");
            
            //Retrieve stock prices
            foreach(String sym in Symbols){
                String csvData;
                using (WebClient web = new WebClient())
                {
                    //30 novembre 2005
                    //30 fevrier 2015.
                    //TO DO 
                    //automatiser la recherche url en fction de la date et de l'action
                    String url = "http://ichart.finance.yahoo.com/table.csv?s=" + sym + "&d=" + d + "&e=" + e + "&f=" + f + "&g=d&a=" + a + "&b=" + b + "&c=" + c + "&ignore=.csv";

                    csvData = web.DownloadString(url);
                }
                YahooFinance.HistoricalParse(csvData, table, sym,false);
            }
            //Retrieve
            foreach (String sym in CurrSymbols)
            {
                String csvData;
                using (WebClient web = new WebClient())
                {
                    String url = "https://www.quandl.com/api/v1/datasets/CURRFX/" + sym + ".csv";

                    csvData = web.DownloadString(url);
                }
                YahooFinance.HistoricalParse(csvData, table, sym,true);
            }
        }
    }
}