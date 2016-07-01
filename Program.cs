using HtmlAgilityPack;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using CsvHelper;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CI_FLights2014
{
    public class Program
    {

        [Serializable]
        public class CIFLight
        {
            // Auto-implemented properties. 

            public string FromIATA;
            public string ToIATA;
            public DateTime FromDate;
            public DateTime ToDate;
            public Boolean FlightMonday;
            public Boolean FlightTuesday;
            public Boolean FlightWednesday;
            public Boolean FlightThursday;
            public Boolean FlightFriday;
            public Boolean FlightSaterday;
            public Boolean FlightSunday;
            public DateTime DepartTime;
            public DateTime ArrivalTime;
            public String FlightNumber;
            public String FlightAirline;
            public String FlightOperator;
            public String FlightAircraft;
        }

        public class AirportDef 
         { 
             // Auto-implemented properties.  
             public string Name { get; set; } 
             public string IATA { get; set; } 
         }

        public class AirlinesDef
        {
            // Auto-implemented properties.  
            public string Name { get; set; }
            public string IATA { get; set; }
            public string DisplayName { get; set; }
            public string WebsiteUrl { get; set; }
        }
        static List<AirlinesDef> _Airlines = new List<AirlinesDef> 
        {
            new AirlinesDef { IATA = "DA", Name="AEROLINEA DE ANTIOQUIA S.A.", DisplayName="ADA",WebsiteUrl="https://www.ada-aero.com/" }, 
            new AirlinesDef { IATA = "EF", Name="EASYFLY S.A", DisplayName="Easyfly",WebsiteUrl="http://www.easyfly.com.co" }, 
            new AirlinesDef { IATA = "2K", Name="AEROGAL", DisplayName="Avianca Ecuador",WebsiteUrl="http://www.avianca.com" }, 
            new AirlinesDef { IATA = "9H", Name="DUTCH ANTILLES EXPRESS SUCURSAL COLOMBIA", DisplayName="Dutch Antilles Express",WebsiteUrl="https://nl.wikipedia.org/wiki/Dutch_Antilles_Express" }, 
            new AirlinesDef { IATA = "AR", Name="AEROLINEAS ARGENTINAS", DisplayName="Aerolíneas Argentinas",WebsiteUrl="http://www.aerolineas.com.ar/" }, 
            new AirlinesDef { IATA = "AM", Name="AEROMEXICO SUCURSAL COLOMBIA", DisplayName="Aeroméxico",WebsiteUrl="http://www.aeromexico.com/" }, 
            new AirlinesDef { IATA = "P5", Name="AEROREPUBLICA", DisplayName="Copa Airlines",WebsiteUrl="http://www.copa.com" }, 
            new AirlinesDef { IATA = "AC", Name="AIR CANADA", DisplayName="AirCanada",WebsiteUrl="http://www.aircanada.com" }, 
            new AirlinesDef { IATA = "AF", Name="AIR FRANCE", DisplayName="Air France",WebsiteUrl="http://www.airfrance.com" }, 
            new AirlinesDef { IATA = "4C", Name="AIRES", DisplayName="LATAM Colombia",WebsiteUrl="http://www.latam.com/" }, 
            new AirlinesDef { IATA = "AA", Name="AMERICAN", DisplayName="American Airlines",WebsiteUrl="http://www.aa.com" }, 
            new AirlinesDef { IATA = "AV", Name="AVIANCA", DisplayName="Avianca",WebsiteUrl="http://www.avianca.com" }, 
            new AirlinesDef { IATA = "V0", Name="CONVIASA", DisplayName="Conviasa",WebsiteUrl="http://www.conviasa.aero/" }, 
            new AirlinesDef { IATA = "CM", Name="COPA", DisplayName="Copa Airlines",WebsiteUrl="http://www.copaair.com/" }, 
            new AirlinesDef { IATA = "CU", Name="CUBANA", DisplayName="Cubana de Aviación",WebsiteUrl="http://www.cubana.cu/home/?lang=en" }, 
            new AirlinesDef { IATA = "DL", Name="DELTA", DisplayName="Delta",WebsiteUrl="http://www.delta.com" }, 
            new AirlinesDef { IATA = "4O", Name="INTERJET", DisplayName="Interjet",WebsiteUrl="http://www.interjet.com/" }, 
            new AirlinesDef { IATA = "5Z", Name="FAST COLOMBIA SAS", DisplayName="ViVaColombia",WebsiteUrl="http://www.vivacolombia.co/" }, 
            new AirlinesDef { IATA = "IB", Name="IBERIA", DisplayName="Iberia",WebsiteUrl="http://www.iberia.com" }, 
            new AirlinesDef { IATA = "B6", Name="JETBLUE AIRWAYS CORPORATION", DisplayName="Jetblue",WebsiteUrl="http://www.jetblue.com" }, 
            new AirlinesDef { IATA = "LR", Name="LACSA", DisplayName="Avianca Costa Rica",WebsiteUrl="http://www.avianca.com" }, 
            new AirlinesDef { IATA = "LA", Name="LAN AIRLINES S.A.", DisplayName="LAN Airlines",WebsiteUrl="http://www.lan.com/" }, 
            new AirlinesDef { IATA = "LP", Name="LAN PERU", DisplayName="LAN Airlines",WebsiteUrl="http://www.lan.com/" }, 
            new AirlinesDef { IATA = "LH", Name="LUFTHANSA", DisplayName="Lufthansa",WebsiteUrl="http://www.lufthansa.com" }, 
            new AirlinesDef { IATA = "9R", Name="SERVICIO AEREO A TERRITORIOS NACIONALES SATENA", DisplayName="Satena",WebsiteUrl="http://www.satena.com/" }, 
            new AirlinesDef { IATA = "NK", Name="SPIRIT AIRLINES", DisplayName="Spirit",WebsiteUrl="http://www.spirit.com" }, 
            new AirlinesDef { IATA = "TA", Name="TACA INTERNATIONAL", DisplayName="TACA Airlines",WebsiteUrl="http://www.taca.com/" }, 
            new AirlinesDef { IATA = "EQ", Name="TAME", DisplayName="TAME",WebsiteUrl="http://www.tame.com.ec/" }, 
            new AirlinesDef { IATA = "3P", Name="TIARA", DisplayName="Tiara Air Aruba",WebsiteUrl="http://www.tiara-air.com/" }, 
            new AirlinesDef { IATA = "T0", Name="TRANS AMERICAN AIR LINES S.A. SUCURSAL COL.", DisplayName="Trans American Airlines",WebsiteUrl="http://www.avianca.com/" }, 
            new AirlinesDef { IATA = "UA", Name="UNITED AIR LINES INC", DisplayName="United",WebsiteUrl="http://www.united.com" }, 
            new AirlinesDef { IATA = "4C", Name="LATAM AIRLINES GROUP S.A SUCURSAL COLOMBIA", DisplayName="LATAM",WebsiteUrl="http://www.latam.com/" }, 
            new AirlinesDef { IATA = "TP", Name="TAP PORTUGAL SUCURSAL COLOMBIA", DisplayName="TAP",WebsiteUrl="http://www.flytap.com" }, 
            new AirlinesDef { IATA = "7P", Name="AIR PANAMA", DisplayName="Air Panama",WebsiteUrl="http://www.airpanama.com/" }, 
            new AirlinesDef { IATA = "O6", Name="OCEANAIR", DisplayName="Avianca Brazil",WebsiteUrl="http://www.avianca.com" },
            new AirlinesDef { IATA = "8I", Name="INSELAIR ARUBA", DisplayName="Insel Air Aruba",WebsiteUrl="http://www.fly-inselair.com/"},
            new AirlinesDef { IATA = "7I", Name="INSEL AIR", DisplayName="Insel Air",WebsiteUrl="http://www.fly-inselair.com/"},
            new AirlinesDef { IATA = "TK", Name="TURK HAVA YOLLARI (TURKISH AIRKINES CO.)", DisplayName="Turkish Airlines",WebsiteUrl="http://www.turkishairlines.com"},
            new AirlinesDef { IATA = "UX", Name="AIR EUROPA", DisplayName="Air Europe",WebsiteUrl="http://www.aireurope.com"},
            new AirlinesDef { IATA = "9V", Name="AVIOR AIRLINES,C.A.", DisplayName="Avior Airlines",WebsiteUrl="http://www.avior.com.ve/"},
            new AirlinesDef { IATA = "KL", Name="KLM", DisplayName="KLM",WebsiteUrl="http://www.klm.nl"},
            new AirlinesDef { IATA = "JJ", Name="TAM", DisplayName="TAM Linhas Aéreas",WebsiteUrl="http://www.latam.com/"}
        };


        static List<AirportDef> _Airports = new List<AirportDef> 
        { 
            new AirportDef { Name = "ALDANA", IATA="IPI" }, 
            new AirportDef { Name = "ARARACUARA", IATA="AQA" }, 
            new AirportDef { Name = "ARAUCA - MUNICIPIO", IATA="AUC" }, 
            new AirportDef { Name = "ARMENIA", IATA="AXM" }, 
            new AirportDef { Name = "ARUBA", IATA="AUA" }, 
            new AirportDef { Name = "ATLANTA", IATA="ATL" }, 
            new AirportDef { Name = "BAHIA SOLANO", IATA="BSC" }, 
            new AirportDef { Name = "BARCELONA", IATA="BCN" }, 
            new AirportDef { Name = "BARRANCABERMEJA", IATA="EJA" }, 
            new AirportDef { Name = "BARRANQUILLA", IATA="BAQ" }, 
            new AirportDef { Name = "BOGOTA", IATA="BOG" }, 
            new AirportDef { Name = "BUCARAMANGA", IATA="BGA" }, 
            new AirportDef { Name = "BUENOS AIRES", IATA="EZE" }, 
            new AirportDef { Name = "CALI", IATA="CLO" }, 
            new AirportDef { Name = "CANCUN", IATA="CUN" }, 
            new AirportDef { Name = "CARACAS", IATA="CCS" }, 
            new AirportDef { Name = "CAREPA", IATA="APO" }, 
            new AirportDef { Name = "CARTAGENA", IATA="CTG" }, 
            new AirportDef { Name = "COROZAL", IATA="CZU" }, 
            new AirportDef { Name = "CUCUTA", IATA="CUC" }, 
            new AirportDef { Name = "CURACAO", IATA="CUR" }, 
            new AirportDef { Name = "EL YOPAL", IATA="EYP" }, 
            new AirportDef { Name = "ESMERALDAS", IATA="ESM" }, 
            new AirportDef { Name = "FLORENCIA", IATA="FLA" }, 
            new AirportDef { Name = "FORT LAUDERDALE", IATA="FLL" }, 
            new AirportDef { Name = "GUAPI", IATA="GPI" }, 
            new AirportDef { Name = "GUATEMALA", IATA="GUA" }, 
            new AirportDef { Name = "GUAYAQUIL", IATA="GYE" }, 
            new AirportDef { Name = "HABANA", IATA="HAV" }, 
            new AirportDef { Name = "HOUSTON", IATA="HOU" }, 
            new AirportDef { Name = "IBAGUE", IATA="IBE" }, 
            new AirportDef { Name = "LETICIA", IATA="LET" }, 
            new AirportDef { Name = "LIMA", IATA="LIM" }, 
            new AirportDef { Name = "MADRID", IATA="MAD" }, 
            new AirportDef { Name = "MAICAO", IATA="MCJ" }, 
            new AirportDef { Name = "MANIZALES", IATA="MZL" }, 
            new AirportDef { Name = "MEDELLIN", IATA="EOH" }, 
            new AirportDef { Name = "MEXICO", IATA="MEX" }, 
            new AirportDef { Name = "MIAMI", IATA="MIA" }, 
            new AirportDef { Name = "MITU", IATA="MVP" }, 
            new AirportDef { Name = "MONTERIA", IATA="MTR" }, 
            new AirportDef { Name = "NEIVA", IATA="NVA" }, 
            new AirportDef { Name = "NEW YORK", IATA="JFK" }, 
            new AirportDef { Name = "NUQUI", IATA="NQU" }, 
            new AirportDef { Name = "ORLANDO", IATA="ORL" }, 
            new AirportDef { Name = "PANAMA", IATA="PNM" }, 
            new AirportDef { Name = "PARIS", IATA="CDG" }, 
            new AirportDef { Name = "PASTO", IATA="PSO" }, 
            new AirportDef { Name = "PEREIRA", IATA="PEI" }, 
            new AirportDef { Name = "POPAYAN", IATA="PPN" }, 
            new AirportDef { Name = "PROVIDENCIA", IATA="PVA" }, 
            new AirportDef { Name = "PUERTO ASIS", IATA="PUU" }, 
            new AirportDef { Name = "PUERTO CARRENO", IATA="PCR" }, 
            new AirportDef { Name = "PUERTO INIRIDA", IATA="PDA" }, 
            new AirportDef { Name = "PUERTO LEGUIZAMO", IATA="LQM" }, 
            new AirportDef { Name = "PUNTA CANA", IATA="PUJ" }, 
            new AirportDef { Name = "QUIBDO", IATA="UIB" }, 
            new AirportDef { Name = "QUITO", IATA="UIO" }, 
            new AirportDef { Name = "REMEDIOS", IATA="OTU" }, 
            new AirportDef { Name = "RIOHACHA", IATA="RCH" }, 
            new AirportDef { Name = "RIONEGRO - ANTIOQUIA", IATA="MDE" }, 
            new AirportDef { Name = "SAN ANDRES - ISLA", IATA="ADZ" }, 
            new AirportDef { Name = "SAN JOSE", IATA="SJE" }, 
            new AirportDef { Name = "SAN SALVADOR", IATA="SAL" }, 
            new AirportDef { Name = "SAN VICENTE DEL CAGUAN", IATA="SVI" }, 
            new AirportDef { Name = "SANTA MARTA", IATA="SMR" }, 
            new AirportDef { Name = "SANTIAGO", IATA="SLC" }, 
            new AirportDef { Name = "SANTO DOMINGO", IATA="SDQ" }, 
            new AirportDef { Name = "SAO PAULO", IATA="GRU" }, 
            new AirportDef { Name = "SARAVENA", IATA="RVE" }, 
            new AirportDef { Name = "TAME", IATA="TME" }, 
            new AirportDef { Name = "TARAPACA", IATA="TCD" }, 
            new AirportDef { Name = "TORONTO", IATA="YYZ" }, 
            new AirportDef { Name = "TUMACO", IATA="TCO" }, 
            new AirportDef { Name = "URIBIA", IATA="000" }, // No IATA CODE?!
            new AirportDef { Name = "VALENCIA", IATA="VLN" }, 
            new AirportDef { Name = "VALLEDUPAR", IATA="VUP" }, 
            new AirportDef { Name = "VILLAVICENCIO", IATA="VVC" }, 
            new AirportDef { Name = "WASHINGTON", IATA="IAD" }, 
            new AirportDef { Name = "FRANKFURT", IATA="FRA" }, 
            new AirportDef { Name = "TOLU", IATA="TLU" }, 
            new AirportDef { Name = "RIO DE JANEIRO", IATA="GIG" }, 
            new AirportDef { Name = "LA PAZ", IATA="LPB" }, 
            new AirportDef { Name = "SAN VICENTE DEL CAGU", IATA="SVI" }, 
            new AirportDef { Name = "VILLA GARZON", IATA="VGZ" }, 
            new AirportDef { Name = "LA PEDRERA", IATA="LPD" }, 
            new AirportDef { Name = "ACANDI", IATA="ACD" }, 
            new AirportDef { Name = "SURAMERICA", IATA="UIO" }, 
            new AirportDef { Name = "LONDRES", IATA="LHR" }, 
            new AirportDef { Name = "LISBOA", IATA="LIS" }, 
            new AirportDef { Name = "PITALITO", IATA="PTX" }, 
            new AirportDef { Name = "PAITILLA MARCO", IATA="PAC" }, 
            new AirportDef { Name = "BALBOA", IATA="BLB" }, 
            new AirportDef { Name = "FORTALEZA", IATA="FOR" }, 
            new AirportDef { Name = "MONTELIBANO", IATA="MTB" }, 
            new AirportDef { Name = "BUENAVENTURA", IATA="BUN" }, 
            new AirportDef { Name = "CAUCASIA", IATA="CAQ" }, 
            new AirportDef { Name = "EL BAGRE", IATA="EBG" }, 
            new AirportDef { Name = "CONDOTO", IATA="COG" }, 
            new AirportDef { Name = "BRASILIA", IATA="BSB" }, 
            new AirportDef { Name = "SAN JOSE DEL GUAVIAR", IATA="SJE" }, 
            new AirportDef { Name = "SAN PEDRO SULA", IATA="SAP" }, 
            new AirportDef { Name = "LA CHORRERA", IATA="LCR" }, 
            new AirportDef { Name = "CAPURGANA", IATA="CPB" }, 
            new AirportDef { Name = "SAN JUAN", IATA="SJU" }, 
            new AirportDef { Name = "TACHINA", IATA="ESM" }, 
            new AirportDef { Name = "DALAS", IATA="DFW" },
            new AirportDef { Name = "LA MACARENA", IATA = "LMC"},
            new AirportDef {Name = "LOS ANGELES", IATA = "LAX"},
            new AirportDef {Name = "BRIDGENTOWN", IATA = "BGI"},
            new AirportDef {Name = "CUZCO", IATA = "CUZ"},
            new AirportDef {Name = "TIBU", IATA = "TIB"}            
         }; 



        private static void Main(string[] args)
        {
            List<CIFLight> CIFLights = new List<CIFLight> { };
            Uri address = new Uri(ConfigurationManager.AppSettings.Get("Download-Url"));
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.Method = "GET";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                Console.WriteLine("Bezig met downloaden van het bestand op de lijst met luchthavens op te halen...");
                Console.WriteLine("Content length is {0}", response.ContentLength);
                Console.WriteLine("Last Modified is {0}", response.LastModified);
                DateTime arg_C1_0 = DateTime.MinValue;
                Convert.ToDateTime(response.LastModified);
                StreamReader reader = new StreamReader(response.GetResponseStream());
                HtmlDocument AP = new HtmlDocument();
                AP.LoadHtml(reader.ReadToEnd());
                Console.WriteLine("Download is compleet...");
                if (Convert.ToBoolean(ConfigurationManager.AppSettings.Get("Save-Download")))
                {
                    AP.Save("Luchthavens.html");
                }
                HtmlNodeCollection Airports = AP.DocumentNode.SelectNodes("//select[@id='ctl00_m_g_c81c37f3_6665_404d_8946_2d58b132affa_ctl00_ListOrigen'] //option");
                for (int Aint = 0; Aint < Airports.Count; Aint++)
                {
                    string Airport = Airports[Aint].Attributes["value"].Value.ToString();
                    Console.WriteLine("Parsing airport: {0}", Airport);
                    BrowserSession b = new BrowserSession();
                    b.Get("http://www.aerocivil.gov.co/SitePages/consultas/FormItinerarios.aspx");
                    b.FormElements["ctl00$m$g_c81c37f3_6665_404d_8946_2d58b132affa$ctl00$ListHoraDesde"] = "0";
                    b.FormElements["ctl00$m$g_c81c37f3_6665_404d_8946_2d58b132affa$ctl00$ListHoraHasta"] = "86400";
                    b.FormElements["ctl00$m$g_c81c37f3_6665_404d_8946_2d58b132affa$ctl00$ListOrigen"] = Airport;
                    b.FormElements["ctl00$m$g_c81c37f3_6665_404d_8946_2d58b132affa$ctl00$ListDestino"] = Airport;
                    b.FormElements["ctl00$m$g_c81c37f3_6665_404d_8946_2d58b132affa$ctl00$ListAerolinea"] = "%";
                    string AirportResponse = b.Post("http://www.aerocivil.gov.co/SitePages/consultas/FormItinerarios.aspx");
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(AirportResponse);
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings.Get("Save-Download")))
                    {
                        document.Save(Airport + ".html");
                    }
                    HtmlNodeCollection rows = document.DocumentNode.SelectNodes("//table[@id='ctl00_m_g_c81c37f3_6665_404d_8946_2d58b132affa_ctl00_ResultadoItinerario'] //tr");
                    new CultureInfo("en-US", true);
                    for (int i = 1; i < rows.Count; i++)
                    {
                        string Airline_Name = null;
                        DateTime Route_Start = DateTime.MinValue;
                        DateTime Route_Einde = DateTime.MinValue;
                        Route_Start = DateTime.Now;
                        string Airline_FlightNumber = null;
                        string Route_SRC_FullName = null;
                        string Route_DST_FullName = null;
                        string Route_Depart_Time = null;
                        string Route_Arrival_Time = null;
                        string Route_Equipment = null;
                        string Route_Seat_Number = null;
                        string Route_Day_Monday = null;
                        string Route_Day_Thuesday = null;
                        string Route_Day_Wednesday = null;
                        string Route_Day_Thursday = null;
                        string Route_Day_Friday = null;
                        string Route_Day_Saterday = null;
                        string Route_Day_Sunday = null;
                        bool Route_Day_Monday_Bit = false;
                        bool Route_Day_Thuesday_Bit = false;
                        bool Route_Day_Wednesday_Bit = false;
                        bool Route_Day_Thursday_Bit = false;
                        bool Route_Day_Friday_Bit = false;
                        bool Route_Day_Saterday_Bit = false;
                        bool Route_Day_Sunday_Bit = false;
                        Convert.ToInt32(rows[i].ChildNodes.Count.ToString());
                        if (Convert.ToInt32(rows[i].ChildNodes.Count.ToString()) == 18)
                        {
                            Airline_Name = Regex.Replace(rows[i].ChildNodes[1].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline).Trim();
                            Regex.Replace(rows[i].ChildNodes[1].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline).Trim();
                            Airline_FlightNumber = Regex.Replace(rows[i].ChildNodes[2].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Regex.Replace(rows[i].ChildNodes[2].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_SRC_FullName = Regex.Replace(rows[i].ChildNodes[3].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Regex.Replace(rows[i].ChildNodes[3].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_DST_FullName = Regex.Replace(rows[i].ChildNodes[4].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Regex.Replace(rows[i].ChildNodes[4].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_Depart_Time = Regex.Replace(rows[i].ChildNodes[5].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Regex.Replace(rows[i].ChildNodes[5].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_Arrival_Time = Regex.Replace(rows[i].ChildNodes[6].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Regex.Replace(rows[i].ChildNodes[6].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_Equipment = Regex.Replace(rows[i].ChildNodes[7].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Regex.Replace(rows[i].ChildNodes[7].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_Seat_Number = Regex.Replace(rows[i].ChildNodes[8].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Regex.Replace(rows[i].ChildNodes[8].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_Day_Monday = Regex.Replace(rows[i].ChildNodes[9].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_Day_Thuesday = Regex.Replace(rows[i].ChildNodes[10].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_Day_Wednesday = Regex.Replace(rows[i].ChildNodes[11].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_Day_Thursday = Regex.Replace(rows[i].ChildNodes[12].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_Day_Friday = Regex.Replace(rows[i].ChildNodes[13].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_Day_Saterday = Regex.Replace(rows[i].ChildNodes[14].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            Route_Day_Sunday = Regex.Replace(rows[i].ChildNodes[15].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
                            string Route_Einde_Temp = Regex.Replace(rows[i].ChildNodes[16].InnerText, "<!--.*?-->", string.Empty, RegexOptions.Singleline).Trim();
                            Route_Einde = Convert.ToDateTime(Route_Einde_Temp);
                        }
                        if (!Route_Day_Monday.Equals("0"))
                        {
                            Route_Day_Monday_Bit = true;
                        }
                        else
                        {
                            Route_Day_Monday_Bit = false;
                        }
                        if (!Route_Day_Thuesday.Equals("0"))
                        {
                            Route_Day_Thuesday_Bit = true;
                        }
                        else
                        {
                            Route_Day_Thuesday_Bit = false;
                        }
                        if (!Route_Day_Wednesday.Equals("0"))
                        {
                            Route_Day_Wednesday_Bit = true;
                        }
                        else
                        {
                            Route_Day_Wednesday_Bit = false;
                        }
                        if (!Route_Day_Thursday.Equals("0"))
                        {
                            Route_Day_Thursday_Bit = true;
                        }
                        else
                        {
                            Route_Day_Thursday_Bit = false;
                        }
                        if (!Route_Day_Friday.Equals("0"))
                        {
                            Route_Day_Friday_Bit = true;
                        }
                        else
                        {
                            Route_Day_Friday_Bit = false;
                        }
                        if (!Route_Day_Saterday.Equals("0"))
                        {
                            Route_Day_Saterday_Bit = true;
                        }
                        else
                        {
                            Route_Day_Saterday_Bit = false;
                        }
                        if (!Route_Day_Sunday.Equals("0"))
                        {
                            Route_Day_Sunday_Bit = true;
                        }
                        else
                        {
                            Route_Day_Sunday_Bit = false;
                        } 

                        // IATA CODES VOOR Airport en Airlines
                        var item = _Airports.Find(q => q.Name == Route_SRC_FullName);
                        string TEMP_FromIATA = item.IATA;
                        var item2 = _Airports.Find(q => q.Name == Route_DST_FullName);
                        string TEMP_ToIATA = item2.IATA;
                        var item3 = _Airlines.Find(q => q.Name == Airline_Name);
                        string TEMP_Airline = item3.IATA;


                        //Console.WriteLine("{0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} - {8} - {9} - {10} - {11} - {12} - {13} - {14} - {15}", new object[]
                        //{
                        //    Airline_Name,
                        //    Route_Einde,
                        //    Airline_FlightNumber,
                        //    Route_SRC_FullName,
                        //    Route_DST_FullName,
                        //    Route_Depart_Time,
                        //    Route_Arrival_Time,
                        //    Route_Equipment,
                        //    Route_Seat_Number,
                        //    Route_Day_Monday_Bit,
                        //    Route_Day_Thuesday_Bit,
                        //    Route_Day_Wednesday_Bit,
                        //    Route_Day_Thursday_Bit,
                        //    Route_Day_Friday_Bit,
                        //    Route_Day_Saterday_Bit,
                        //    Route_Day_Sunday_Bit
                        //});

                        //using (StreamWriter sw = File.AppendText("raw-output.txt"))
                        //{
                        //    sw.WriteLine("{0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} - {8} - {9} - {10} - {11} - {12} - {13} - {14} - {15}", new object[]
                        //    {
                        //        Airline_Name,
                        //        Route_Einde,
                        //        Airline_FlightNumber,
                        //        Route_SRC_FullName,
                        //        Route_DST_FullName,
                        //        Route_Depart_Time,
                        //        Route_Arrival_Time,
                        //        Route_Equipment,
                        //        Route_Seat_Number,
                        //        Route_Day_Monday_Bit,
                        //        Route_Day_Thuesday_Bit,
                        //        Route_Day_Wednesday_Bit,
                        //        Route_Day_Thursday_Bit,
                        //        Route_Day_Friday_Bit,
                        //        Route_Day_Saterday_Bit,
                        //        Route_Day_Sunday_Bit
                        //    });
                        //}

                        bool alreadyExists = CIFLights.Exists(x => x.FromIATA == TEMP_FromIATA 
                                && x.ToIATA == TEMP_ToIATA
                                && x.ToDate == Route_Einde                                
                                && x.FlightNumber == Airline_FlightNumber 
                                && x.FlightAirline == TEMP_Airline
                                && x.FlightMonday == Route_Day_Monday_Bit
                                && x.FlightTuesday == Route_Day_Thuesday_Bit
                                && x.FlightWednesday == Route_Day_Wednesday_Bit
                                && x.FlightThursday == Route_Day_Thursday_Bit
                                && x.FlightFriday == Route_Day_Friday_Bit
                                && x.FlightSaterday == Route_Day_Saterday_Bit
                                && x.FlightSunday == Route_Day_Sunday_Bit);

                        if (alreadyExists)
                        {
                            Console.WriteLine("Flight Already found...");
                        }
                        else
                        {
                            CIFLights.Add(new CIFLight
                            {
                                FromIATA = TEMP_FromIATA,
                                ToIATA = TEMP_ToIATA,
                                FromDate = Route_Start,
                                ToDate = Route_Einde,
                                ArrivalTime = DateTime.Parse(Route_Arrival_Time),
                                DepartTime = DateTime.Parse(Route_Depart_Time),
                                FlightAircraft = Route_Equipment,
                                FlightAirline = TEMP_Airline,
                                FlightMonday = Route_Day_Monday_Bit,
                                FlightTuesday = Route_Day_Thuesday_Bit,
                                FlightWednesday = Route_Day_Wednesday_Bit,
                                FlightThursday = Route_Day_Thursday_Bit,
                                FlightFriday = Route_Day_Friday_Bit,
                                FlightSaterday = Route_Day_Saterday_Bit,
                                FlightSunday = Route_Day_Sunday_Bit,
                                FlightNumber = Airline_FlightNumber,
                                FlightOperator = Airline_Name
                            });
                        }
                    }
                }
                Console.WriteLine("Creating GTFS Files...");
                string gtfsDir = AppDomain.CurrentDomain.BaseDirectory + "\\gtfs";
                System.IO.Directory.CreateDirectory(gtfsDir);


                Console.WriteLine("Creating GTFS File agency.txt...");
                using (var gtfsagency = new StreamWriter(@"gtfs\\agency.txt"))
                {
                    var csv = new CsvWriter(gtfsagency);
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.Encoding = Encoding.UTF8;
                    csv.Configuration.TrimFields = true;
                    // header 
                    csv.WriteField("agency_id");
                    csv.WriteField("agency_name");
                    csv.WriteField("agency_url");
                    csv.WriteField("agency_timezone");
                    csv.WriteField("agency_lang");
                    csv.WriteField("agency_phone");
                    csv.WriteField("agency_fare_url");
                    csv.WriteField("agency_email");
                    csv.NextRecord();

                    var airlines = CIFLights.Select(m => new {m.FlightAirline}).Distinct().ToList();

                    for (int i = 0; i < airlines.Count; i++) // Loop through List with for)
                    {
                        csv.WriteField(airlines[i].FlightAirline);
                        var item4 = _Airlines.Find(q => q.IATA == airlines[i].FlightAirline);
                        string TEMP_Name = item4.DisplayName;
                        string TEMP_Url = item4.WebsiteUrl;
                        csv.WriteField(TEMP_Name);
                        csv.WriteField(TEMP_Url);
                        csv.WriteField("America/Bogota");
                        csv.WriteField("ES");
                        csv.WriteField("");
                        csv.WriteField("");
                        csv.WriteField("");
                        csv.NextRecord();
                    }                    
                }

                Console.WriteLine("Creating GTFS File trips.txt and stop_times.txt...");
                using (var gtfscalendar = new StreamWriter(@"gtfs\\calendar.txt"))
                {
                    using (var gtfsroutes = new StreamWriter(@"gtfs\\routes.txt"))
                    {
                        using (var gtfstrips = new StreamWriter(@"gtfs\\trips.txt"))
                        {
                            using (var gtfsstoptimes = new StreamWriter(@"gtfs\\stop_times.txt"))
                            {
                                // Headers 
                                var csvstoptimes = new CsvWriter(gtfsstoptimes);
                                csvstoptimes.Configuration.Delimiter = ",";
                                csvstoptimes.Configuration.Encoding = Encoding.UTF8;
                                csvstoptimes.Configuration.TrimFields = true;
                                // header 
                                csvstoptimes.WriteField("trip_id");
                                csvstoptimes.WriteField("arrival_time");
                                csvstoptimes.WriteField("departure_time");
                                csvstoptimes.WriteField("stop_id");
                                csvstoptimes.WriteField("stop_sequence");
                                csvstoptimes.WriteField("stop_headsign");
                                csvstoptimes.WriteField("pickup_type");
                                csvstoptimes.WriteField("drop_off_type");
                                csvstoptimes.WriteField("shape_dist_traveled");
                                csvstoptimes.WriteField("timepoint");
                                csvstoptimes.NextRecord();

                                var csvtrips = new CsvWriter(gtfstrips);
                                csvtrips.Configuration.Delimiter = ",";
                                csvtrips.Configuration.Encoding = Encoding.UTF8;
                                csvtrips.Configuration.TrimFields = true;
                                // header 
                                csvtrips.WriteField("route_id");
                                csvtrips.WriteField("service_id");
                                csvtrips.WriteField("trip_id");
                                csvtrips.WriteField("trip_headsign");
                                csvtrips.WriteField("trip_short_name");
                                csvtrips.WriteField("direction_id");
                                csvtrips.WriteField("block_id");
                                csvtrips.WriteField("shape_id");
                                csvtrips.WriteField("wheelchair_accessible");
                                csvtrips.WriteField("bikes_allowed ");
                                csvtrips.NextRecord();

                                var csvroutes = new CsvWriter(gtfsroutes);
                                csvroutes.Configuration.Delimiter = ",";
                                csvroutes.Configuration.Encoding = Encoding.UTF8;
                                csvroutes.Configuration.TrimFields = true;
                                // header 
                                csvroutes.WriteField("route_id");
                                csvroutes.WriteField("agency_id");
                                csvroutes.WriteField("route_short_name");
                                csvroutes.WriteField("route_long_name");
                                csvroutes.WriteField("route_desc");
                                csvroutes.WriteField("route_type");
                                csvroutes.WriteField("route_url");
                                csvroutes.WriteField("route_color");
                                csvroutes.WriteField("route_text_color");
                                csvroutes.NextRecord();

                                var csvcalendar = new CsvWriter(gtfscalendar);
                                csvcalendar.Configuration.Delimiter = ",";
                                csvcalendar.Configuration.Encoding = Encoding.UTF8;
                                csvcalendar.Configuration.TrimFields = true;
                                // header 
                                csvcalendar.WriteField("service_id");
                                csvcalendar.WriteField("monday");
                                csvcalendar.WriteField("tuesday");
                                csvcalendar.WriteField("wednesday");
                                csvcalendar.WriteField("thursday");
                                csvcalendar.WriteField("friday");
                                csvcalendar.WriteField("saturday");
                                csvcalendar.WriteField("sunday");
                                csvcalendar.WriteField("start_date");
                                csvcalendar.WriteField("end_date");
                                csvcalendar.NextRecord();

                                //1101 International Air Service
                                //1102 Domestic Air Service
                                //1103 Intercontinental Air Service
                                //1104 Domestic Scheduled Air Service
                                
                                for (int i = 0; i < CIFLights.Count; i++) // Loop through List with for)
                                {
                                    // Route record
                                    csvroutes.WriteField(CIFLights[i].FromIATA + CIFLights[i].ToIATA + CIFLights[i].FlightAirline + CIFLights[i].FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", CIFLights[i].FromDate) + String.Format("{0:yyyyMMdd}", CIFLights[i].ToDate) + Convert.ToInt32(CIFLights[i].FlightMonday) + Convert.ToInt32(CIFLights[i].FlightTuesday) + Convert.ToInt32(CIFLights[i].FlightWednesday) + Convert.ToInt32(CIFLights[i].FlightThursday) + Convert.ToInt32(CIFLights[i].FlightFriday) + Convert.ToInt32(CIFLights[i].FlightSaterday) + Convert.ToInt32(CIFLights[i].FlightSunday));
                                    csvroutes.WriteField(CIFLights[i].FlightAirline);
                                    csvroutes.WriteField(CIFLights[i].FromIATA + CIFLights[i].ToIATA);
                                    csvroutes.WriteField("");
                                    csvroutes.WriteField(CIFLights[i].FlightAircraft + ";" + CIFLights[i].FlightAirline + ";" + CIFLights[i].FlightOperator);
                                    csvroutes.WriteField(1102);
                                    csvroutes.WriteField("");
                                    csvroutes.WriteField("");
                                    csvroutes.WriteField("");
                                    csvroutes.NextRecord();

                                    // Calender

                                    csvcalendar.WriteField(CIFLights[i].FromIATA + CIFLights[i].ToIATA + CIFLights[i].FlightAirline + CIFLights[i].FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", CIFLights[i].FromDate) + String.Format("{0:yyyyMMdd}", CIFLights[i].ToDate) + Convert.ToInt32(CIFLights[i].FlightMonday) + Convert.ToInt32(CIFLights[i].FlightTuesday) + Convert.ToInt32(CIFLights[i].FlightWednesday) + Convert.ToInt32(CIFLights[i].FlightThursday) + Convert.ToInt32(CIFLights[i].FlightFriday) + Convert.ToInt32(CIFLights[i].FlightSaterday) + Convert.ToInt32(CIFLights[i].FlightSunday));
                                    csvcalendar.WriteField(Convert.ToInt32(CIFLights[i].FlightMonday));
                                    csvcalendar.WriteField(Convert.ToInt32(CIFLights[i].FlightTuesday));
                                    csvcalendar.WriteField(Convert.ToInt32(CIFLights[i].FlightWednesday));
                                    csvcalendar.WriteField(Convert.ToInt32(CIFLights[i].FlightThursday));
                                    csvcalendar.WriteField(Convert.ToInt32(CIFLights[i].FlightFriday));
                                    csvcalendar.WriteField(Convert.ToInt32(CIFLights[i].FlightSaterday));
                                    csvcalendar.WriteField(Convert.ToInt32(CIFLights[i].FlightSunday));
                                    csvcalendar.WriteField(String.Format("{0:yyyyMMdd}", CIFLights[i].FromDate));
                                    csvcalendar.WriteField(String.Format("{0:yyyyMMdd}", CIFLights[i].ToDate));
                                    csvcalendar.NextRecord();

                                    // Trips
                                    csvtrips.WriteField(CIFLights[i].FromIATA + CIFLights[i].ToIATA + CIFLights[i].FlightAirline + CIFLights[i].FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", CIFLights[i].FromDate) + String.Format("{0:yyyyMMdd}", CIFLights[i].ToDate) + Convert.ToInt32(CIFLights[i].FlightMonday) + Convert.ToInt32(CIFLights[i].FlightTuesday) + Convert.ToInt32(CIFLights[i].FlightWednesday) + Convert.ToInt32(CIFLights[i].FlightThursday) + Convert.ToInt32(CIFLights[i].FlightFriday) + Convert.ToInt32(CIFLights[i].FlightSaterday) + Convert.ToInt32(CIFLights[i].FlightSunday));
                                    csvtrips.WriteField(CIFLights[i].FromIATA + CIFLights[i].ToIATA + CIFLights[i].FlightAirline + CIFLights[i].FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", CIFLights[i].FromDate) + String.Format("{0:yyyyMMdd}", CIFLights[i].ToDate) + Convert.ToInt32(CIFLights[i].FlightMonday) + Convert.ToInt32(CIFLights[i].FlightTuesday) + Convert.ToInt32(CIFLights[i].FlightWednesday) + Convert.ToInt32(CIFLights[i].FlightThursday) + Convert.ToInt32(CIFLights[i].FlightFriday) + Convert.ToInt32(CIFLights[i].FlightSaterday) + Convert.ToInt32(CIFLights[i].FlightSunday));
                                    csvtrips.WriteField(CIFLights[i].FromIATA + CIFLights[i].ToIATA + CIFLights[i].FlightAirline + CIFLights[i].FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", CIFLights[i].FromDate) + String.Format("{0:yyyyMMdd}", CIFLights[i].ToDate) + Convert.ToInt32(CIFLights[i].FlightMonday) + Convert.ToInt32(CIFLights[i].FlightTuesday) + Convert.ToInt32(CIFLights[i].FlightWednesday) + Convert.ToInt32(CIFLights[i].FlightThursday) + Convert.ToInt32(CIFLights[i].FlightFriday) + Convert.ToInt32(CIFLights[i].FlightSaterday) + Convert.ToInt32(CIFLights[i].FlightSunday));
                                    csvtrips.WriteField(CIFLights[i].ToIATA);
                                    csvtrips.WriteField("");
                                    csvtrips.WriteField("");
                                    csvtrips.WriteField("");
                                    csvtrips.WriteField("");
                                    csvtrips.WriteField("1");
                                    csvtrips.WriteField("");
                                    csvtrips.NextRecord();

                                    // Depart Record
                                    csvstoptimes.WriteField(CIFLights[i].FromIATA + CIFLights[i].ToIATA + CIFLights[i].FlightAirline + CIFLights[i].FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", CIFLights[i].FromDate) + String.Format("{0:yyyyMMdd}", CIFLights[i].ToDate) + Convert.ToInt32(CIFLights[i].FlightMonday) + Convert.ToInt32(CIFLights[i].FlightTuesday) + Convert.ToInt32(CIFLights[i].FlightWednesday) + Convert.ToInt32(CIFLights[i].FlightThursday) + Convert.ToInt32(CIFLights[i].FlightFriday) + Convert.ToInt32(CIFLights[i].FlightSaterday) + Convert.ToInt32(CIFLights[i].FlightSunday));
                                    csvstoptimes.WriteField(String.Format("{0:HH:mm:ss}", CIFLights[i].DepartTime));
                                    csvstoptimes.WriteField(String.Format("{0:HH:mm:ss}", CIFLights[i].DepartTime));
                                    csvstoptimes.WriteField(CIFLights[i].FromIATA);
                                    csvstoptimes.WriteField("0");
                                    csvstoptimes.WriteField("");
                                    csvstoptimes.WriteField("0");
                                    csvstoptimes.WriteField("0");
                                    csvstoptimes.WriteField("");
                                    csvstoptimes.WriteField("");
                                    csvstoptimes.NextRecord();
                                    // Arrival Record
                                    if(CIFLights[i].DepartTime.TimeOfDay < System.TimeSpan.Parse("00:09:00") && CIFLights[i].ArrivalTime.TimeOfDay > System.TimeSpan.Parse("00:09:00"))
                                    //if (!CIFLights[i].FlightNextDayArrival)
                                    {
                                        csvstoptimes.WriteField(CIFLights[i].FromIATA + CIFLights[i].ToIATA + CIFLights[i].FlightAirline + CIFLights[i].FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", CIFLights[i].FromDate) + String.Format("{0:yyyyMMdd}", CIFLights[i].ToDate) + Convert.ToInt32(CIFLights[i].FlightMonday) + Convert.ToInt32(CIFLights[i].FlightTuesday) + Convert.ToInt32(CIFLights[i].FlightWednesday) + Convert.ToInt32(CIFLights[i].FlightThursday) + Convert.ToInt32(CIFLights[i].FlightFriday) + Convert.ToInt32(CIFLights[i].FlightSaterday) + Convert.ToInt32(CIFLights[i].FlightSunday));
                                        csvstoptimes.WriteField(String.Format("{0:HH:mm:ss}", CIFLights[i].ArrivalTime));
                                        csvstoptimes.WriteField(String.Format("{0:HH:mm:ss}", CIFLights[i].ArrivalTime));
                                        csvstoptimes.WriteField(CIFLights[i].ToIATA);
                                        csvstoptimes.WriteField("2");
                                        csvstoptimes.WriteField("");
                                        csvstoptimes.WriteField("0");
                                        csvstoptimes.WriteField("0");
                                        csvstoptimes.WriteField("");
                                        csvstoptimes.WriteField("");
                                        csvstoptimes.NextRecord();
                                    }
                                    else
                                    {
                                        //add 24 hour for the gtfs time
                                        int hour = CIFLights[i].ArrivalTime.Hour;
                                        hour = hour + 24;
                                        int minute = CIFLights[i].ArrivalTime.Minute;
                                        string strminute = minute.ToString();
                                        if (strminute.Length == 1) { strminute = "0" + strminute; }



                                        csvstoptimes.WriteField(CIFLights[i].FromIATA + CIFLights[i].ToIATA + CIFLights[i].FlightAirline + CIFLights[i].FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", CIFLights[i].FromDate) + String.Format("{0:yyyyMMdd}", CIFLights[i].ToDate) + Convert.ToInt32(CIFLights[i].FlightMonday) + Convert.ToInt32(CIFLights[i].FlightTuesday) + Convert.ToInt32(CIFLights[i].FlightWednesday) + Convert.ToInt32(CIFLights[i].FlightThursday) + Convert.ToInt32(CIFLights[i].FlightFriday) + Convert.ToInt32(CIFLights[i].FlightSaterday) + Convert.ToInt32(CIFLights[i].FlightSunday));
                                        csvstoptimes.WriteField(hour + ":" + strminute + ":00");
                                        csvstoptimes.WriteField(hour + ":" + strminute + ":00");
                                        csvstoptimes.WriteField(CIFLights[i].ToIATA);
                                        csvstoptimes.WriteField("2");
                                        csvstoptimes.WriteField("");
                                        csvstoptimes.WriteField("0");
                                        csvstoptimes.WriteField("0");
                                        csvstoptimes.WriteField("");
                                        csvstoptimes.WriteField("");
                                        csvstoptimes.NextRecord();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

