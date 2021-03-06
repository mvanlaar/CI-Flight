using HtmlAgilityPack;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using CsvHelper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CI_FLights2014
{
    public class Program
    {

        [Serializable]
        public class CIFLight
        {
            // Auto-implemented properties. 

            public string FromIATA;
            public string FromICAO;
            public string ToIATA;
            public string ToICAO;
            public Boolean FromUseICAO;
            public Boolean ToUseICAO;
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

        public class IATAAirport
        {
            public string stop_iata;
            public string stop_icao;
            public string stop_name;
            public string stop_city;
            public string stop_country;
            public string stop_lat;
            public string stop_lon;
            public string stop_timezone;
        }

        public class AirportDef 
         { 
             // Auto-implemented properties.  
             public string Name { get; set; } 
             public string IATA { get; set; }
             public string ICAO { get; set; }
             public Boolean UseICAO { get; set; }
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
            new AirlinesDef { IATA = "AR", Name="AEROLINEAS ARGENTINAS", DisplayName="Aerol�neas Argentinas",WebsiteUrl="http://www.aerolineas.com.ar/" }, 
            new AirlinesDef { IATA = "AM", Name="AEROMEXICO SUCURSAL COLOMBIA", DisplayName="Aerom�xico",WebsiteUrl="http://www.aeromexico.com/" }, 
            new AirlinesDef { IATA = "P5", Name="AEROREPUBLICA", DisplayName="Copa Airlines",WebsiteUrl="http://www.copa.com" }, 
            new AirlinesDef { IATA = "AC", Name="AIR CANADA", DisplayName="AirCanada",WebsiteUrl="http://www.aircanada.com" }, 
            new AirlinesDef { IATA = "AF", Name="AIR FRANCE", DisplayName="Air France",WebsiteUrl="http://www.airfrance.com" }, 
            new AirlinesDef { IATA = "4C", Name="AIRES", DisplayName="LATAM Colombia",WebsiteUrl="http://www.latam.com/" }, 
            new AirlinesDef { IATA = "AA", Name="AMERICAN", DisplayName="American Airlines",WebsiteUrl="http://www.aa.com" }, 
            new AirlinesDef { IATA = "AV", Name="AVIANCA", DisplayName="Avianca",WebsiteUrl="http://www.avianca.com" }, 
            new AirlinesDef { IATA = "V0", Name="CONVIASA", DisplayName="Conviasa",WebsiteUrl="http://www.conviasa.aero/" }, 
            new AirlinesDef { IATA = "CM", Name="COPA", DisplayName="Copa Airlines",WebsiteUrl="http://www.copaair.com/" }, 
            new AirlinesDef { IATA = "CU", Name="CUBANA", DisplayName="Cubana de Aviaci�n",WebsiteUrl="http://www.cubana.cu/home/?lang=en" }, 
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
            new AirlinesDef { IATA = "JJ", Name="TAM", DisplayName="TAM Linhas A�reas",WebsiteUrl="http://www.latam.com/"}
        };


        static List<AirportDef> _Airports = new List<AirportDef> 
        { 
            new AirportDef { Name = "ALDANA", IATA="IPI", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "ARARACUARA", IATA="AQA", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "ARAUCA - MUNICIPIO", IATA="AUC", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "ARMENIA", IATA="AXM", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "ARUBA", IATA="AUA", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "ATLANTA", IATA="ATL", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "BAHIA SOLANO", IATA="BSC", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "BARCELONA", IATA="BCN", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "BARRANCABERMEJA", IATA="EJA", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "BARRANQUILLA", IATA="BAQ", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "BOGOTA", IATA="BOG", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "BUCARAMANGA", IATA="BGA", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "BUENOS AIRES", IATA="EZE", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "CALI", IATA="CLO", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "CANCUN", IATA="CUN", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "CARACAS", IATA="CCS", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "CAREPA", IATA="APO", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "CARTAGENA", IATA="CTG", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "COROZAL", IATA="CZU", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "CUCUTA", IATA="CUC", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "CURACAO", IATA="CUR", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "EL YOPAL", IATA="EYP", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "ESMERALDAS", IATA="ESM", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "FLORENCIA", IATA="FLA", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "FORT LAUDERDALE", IATA="FLL", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "GUAPI", IATA="GPI", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "GUATEMALA", IATA="GUA" , ICAO="", UseICAO=false}, 
            new AirportDef { Name = "GUAYAQUIL", IATA="GYE", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "HABANA", IATA="HAV", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "HOUSTON", IATA="HOU", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "IBAGUE", IATA="IBE", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "LETICIA", IATA="LET", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "LIMA", IATA="LIM", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "MADRID", IATA="MAD", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "MAICAO", IATA="MCJ" , ICAO="", UseICAO=false}, 
            new AirportDef { Name = "MANIZALES", IATA="MZL", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "MEDELLIN", IATA="EOH", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "MEXICO", IATA="MEX", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "MIAMI", IATA="MIA", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "MITU", IATA="MVP", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "MONTERIA", IATA="MTR", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "NEIVA", IATA="NVA", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "NEW YORK", IATA="JFK", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "NUQUI", IATA="NQU", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "ORLANDO", IATA="ORL", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "PANAMA", IATA="PNM", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "PARIS", IATA="CDG", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "PASTO", IATA="PSO", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "PEREIRA", IATA="PEI", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "POPAYAN", IATA="PPN", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "PROVIDENCIA", IATA="PVA", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "PUERTO ASIS", IATA="PUU", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "PUERTO CARRENO", IATA="PCR", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "PUERTO INIRIDA", IATA="PDA", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "PUERTO LEGUIZAMO", IATA="LQM", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "PUNTA CANA", IATA="PUJ", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "QUIBDO", IATA="UIB", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "QUITO", IATA="UIO", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "REMEDIOS", IATA="OTU", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "RIOHACHA", IATA="RCH", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "RIONEGRO - ANTIOQUIA", IATA="MDE", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SAN ANDRES - ISLA", IATA="ADZ", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SAN JOSE", IATA="SJE", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SAN SALVADOR", IATA="SAL", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SAN VICENTE DEL CAGUAN", IATA="SVI", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SANTA MARTA", IATA="SMR", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SANTIAGO", IATA="SLC", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SANTO DOMINGO", IATA="SDQ", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SAO PAULO", IATA="GRU", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SARAVENA", IATA="RVE", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "TAME", IATA="TME", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "TARAPACA", IATA="TCD", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "TORONTO", IATA="YYZ", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "TUMACO", IATA="TCO", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "URIBIA", IATA="000", ICAO="SKPB", UseICAO=true }, // No IATA CODE?!
            new AirportDef { Name = "VALENCIA", IATA="VLN", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "VALLEDUPAR", IATA="VUP", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "VILLAVICENCIO", IATA="VVC", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "WASHINGTON", IATA="IAD", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "FRANKFURT", IATA="FRA", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "TOLU", IATA="TLU", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "RIO DE JANEIRO", IATA="GIG", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "LA PAZ", IATA="LPB", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SAN VICENTE DEL CAGU", IATA="SVI", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "VILLA GARZON", IATA="VGZ", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "LA PEDRERA", IATA="LPD", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "ACANDI", IATA="ACD", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SURAMERICA", IATA="UIO", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "LONDRES", IATA="LHR", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "LISBOA", IATA="LIS", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "PITALITO", IATA="PTX", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "PAITILLA MARCO", IATA="PAC", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "BALBOA", IATA="BLB", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "FORTALEZA", IATA="FOR", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "MONTELIBANO", IATA="MTB", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "BUENAVENTURA", IATA="BUN", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "CAUCASIA", IATA="CAQ", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "EL BAGRE", IATA="EBG", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "CONDOTO", IATA="COG", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "BRASILIA", IATA="BSB", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SAN JOSE DEL GUAVIAR", IATA="SJE", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SAN PEDRO SULA", IATA="SAP", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "LA CHORRERA", IATA="LCR", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "CAPURGANA", IATA="CPB", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "SAN JUAN", IATA="SJU", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "TACHINA", IATA="ESM", ICAO="", UseICAO=false }, 
            new AirportDef { Name = "DALAS", IATA="DFW", ICAO="", UseICAO=false },
            new AirportDef { Name = "LA MACARENA", IATA = "LMC", ICAO="", UseICAO=false},
            new AirportDef {Name = "LOS ANGELES", IATA = "LAX", ICAO="", UseICAO=false},
            new AirportDef {Name = "BRIDGENTOWN", IATA = "BGI", ICAO="", UseICAO=false},
            new AirportDef {Name = "CUZCO", IATA = "CUZ", ICAO="", UseICAO=false},
            new AirportDef {Name = "TIBU", IATA = "TIB", ICAO="", UseICAO=false}            
         };
        
        public static readonly List<string> _AirlinesWanted = new List<string>() { "DA", "EF", "9R" };

        private static void Main(string[] args)
        {
            // Read The Airports Json
            Console.WriteLine("Reading IATA Airports....");
            string IATAAirportsFile = AppDomain.CurrentDomain.BaseDirectory + "IATAAirports.json";
            JArray o1 = JArray.Parse(File.ReadAllText(IATAAirportsFile));
            IList<IATAAirport> TempIATAAirports = o1.ToObject<IList<IATAAirport>>();
            var IATAAirports = TempIATAAirports as List<IATAAirport>;

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

                        // IATA CODES for Airport en Airlines
                        var item = _Airports.Find(q => q.Name == Route_SRC_FullName);
                        string TEMP_FromIATA = item.IATA;
                        string TEMP_FromICAO = item.ICAO;
                        Boolean TEMP_FromUseICAO = item.UseICAO;
                        var item2 = _Airports.Find(q => q.Name == Route_DST_FullName);
                        string TEMP_ToIATA = item2.IATA;
                        string TEMP_ToICAO = item.ICAO;
                        Boolean TEMP_ToUseICAO = item.UseICAO;
                        var item3 = _Airlines.Find(q => q.Name == Airline_Name);
                        string TEMP_Airline = item3.IATA;
                                                
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
                            if (_AirlinesWanted.Contains(TEMP_Airline, StringComparer.OrdinalIgnoreCase))
                            {                                
                                // Not Avianca (AV, 2K, O6, LR), we got a better source for Avianca.
                                // Not CopaAirlines (CM, P5), we got a better source for Copa Airlines.
                                CIFLights.Add(new CIFLight
                                {
                                    FromIATA = TEMP_FromIATA,
                                    FromICAO = TEMP_FromICAO,
                                    FromUseICAO = TEMP_FromUseICAO,
                                    ToIATA = TEMP_ToIATA,
                                    ToICAO = TEMP_ToICAO,
                                    ToUseICAO = TEMP_ToUseICAO,
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
                }
                Console.WriteLine("Creating GTFS Directory...");
                string gtfsDir = AppDomain.CurrentDomain.BaseDirectory + "\\gtfs";
                System.IO.Directory.CreateDirectory(gtfsDir);
                Console.WriteLine("Creating GTFS Files...");

                // Check for Seperate GTFS Files
                if (Convert.ToBoolean(ConfigurationManager.AppSettings.Get("SeperateGTFS")))
                { 
                    // create list of agency's
                    var agency = CIFLights.Select(m => new { m.FlightAirline }).Distinct().ToList();
                    // select routes for agency
                    for (int a = 0; a < agency.Count; a++)
                    {
                        // select routes for agency
                        var agencyroute = CIFLights.Find(q => q.FlightAirline == agency[a].FlightAirline);
                        // generate directory
                        string gtfsagencyDir = AppDomain.CurrentDomain.BaseDirectory + "\\gtfs\\" + agency[a].FlightAirline;
                        System.IO.Directory.CreateDirectory(gtfsagencyDir);
                        string gtfsagencyagency = gtfsagencyDir + "\\agency.txt";
                        string Agency_Name = null;
                        string Agency_Url = null;
                        var item4 = _Airlines.Find(q => q.IATA == agency[a].FlightAirline);
                        Agency_Name = item4.DisplayName;
                        Agency_Url = item4.WebsiteUrl;

                        using (var gtfsagency = new StreamWriter(gtfsagencyagency))
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
                            csv.WriteField(agencyroute.FlightAirline);                               
                            csv.WriteField(Agency_Name);
                            csv.WriteField(Agency_Url);
                            csv.WriteField("America/Bogota");
                            csv.WriteField("ES");
                            csv.WriteField("");
                            csv.WriteField("");
                            csv.WriteField("");
                            csv.NextRecord();                            
                        }
                        //GTFS.GenerateAgency(Name: Agency_Name, Code: agencyroute.FlightAirline, Url: Agency_Url, Timezone: "America/Bogota", Language: "ES", Pathtofile: gtfsagencyagency);
                        //GenerateRoutes
                        string gtfsagencyroutes = gtfsagencyDir + "\\routes.txt";
                        string gtfsagencystops = gtfsagencyDir + "\\stops.txt";
                        Console.WriteLine("Creating GTFS File routes.txt ...");

                        // Only Select Routes from Airline                        

                        using (var gtfsroutes = new StreamWriter(gtfsagencyroutes))
                        {
                            // Route record
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
                            IEnumerable<CIFLight> agencyroutes =
                            from student in CIFLights
                            where student.FlightAirline == agency[a].FlightAirline
                            select student;
                            var routes = agencyroutes.Select(m => new { m.FromIATA, m.ToIATA, m.FlightAirline }).Distinct().ToList();

                            for (int i = 0; i < routes.Count; i++) // Loop through List with for)
                            {
                                csvroutes.WriteField(routes[i].FromIATA + routes[i].ToIATA);
                                csvroutes.WriteField(routes[i].FlightAirline);
                                csvroutes.WriteField(routes[i].FromIATA + routes[i].ToIATA + routes[i].FlightAirline);
                                csvroutes.WriteField(routes[i].FromIATA + " - " + routes[i].ToIATA + " - " + routes[i].FlightAirline);
                                csvroutes.WriteField(""); // routes[i].FlightAircraft + ";" + CIFLights[i].FlightAirline + ";" + CIFLights[i].FlightOperator + ";" + CIFLights[i].FlightCodeShare
                                csvroutes.WriteField(1102);
                                csvroutes.WriteField("");
                                csvroutes.WriteField("");
                                csvroutes.WriteField("");
                                csvroutes.NextRecord();
                            }

                            // stops.txt

                            List<string> agencyairportsiata =
                             agencyroutes.SelectMany(m => new string[] { m.FromIATA, m.ToIATA })
                                     .Distinct()
                                     .ToList();

                            using (var gtfsstops = new StreamWriter(gtfsagencystops))
                            {
                                // Route record
                                var csvstops = new CsvWriter(gtfsstops);
                                csvstops.Configuration.Delimiter = ",";
                                csvstops.Configuration.Encoding = Encoding.UTF8;
                                csvstops.Configuration.TrimFields = true;
                                // header                                 
                                csvstops.WriteField("stop_id");
                                csvstops.WriteField("stop_name");
                                csvstops.WriteField("stop_desc");
                                csvstops.WriteField("stop_lat");
                                csvstops.WriteField("stop_lon");
                                csvstops.WriteField("zone_id");
                                csvstops.WriteField("stop_url");
                                csvstops.NextRecord();

                                for (int i = 0; i < agencyairportsiata.Count; i++) // Loop through List with for)
                                {
                                    //int result1 = IATAAirports.FindIndex(T => T.stop_id == 9458)
                                    var airportinfo = IATAAirports.Find(q => q.stop_iata == agencyairportsiata[i]);
                                    csvstops.WriteField(airportinfo.stop_iata);
                                    csvstops.WriteField(airportinfo.stop_name);
                                    csvstops.WriteField(airportinfo.stop_city + " - " + airportinfo.stop_country);
                                    csvstops.WriteField(airportinfo.stop_lat);
                                    csvstops.WriteField(airportinfo.stop_lon);
                                    csvstops.WriteField("");
                                    csvstops.WriteField("");
                                    csvstops.NextRecord();
                                }
                            }
                        }   
                        string gtfsagencycalendar = gtfsagencyDir + "\\calendar.txt";
                        string gtfsagencytrips = gtfsagencyDir + "\\trips.txt";
                        string gtfsagencystoptimes = gtfsagencyDir + "\\stop_times.txt";

                        Console.WriteLine("Creating GTFS File trips.txt and stop_times.txt...");
                        using (var gtfscalendar = new StreamWriter(gtfsagencycalendar))
                        {
                            using (var gtfstrips = new StreamWriter(gtfsagencytrips))
                            {
                                using (var gtfsstoptimes = new StreamWriter(gtfsagencystoptimes))
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

                                    IEnumerable<CIFLight> agencyroutes =
                                    from student in CIFLights
                                    where student.FlightAirline == agency[a].FlightAirline
                                    select student;

                                    foreach (CIFLight flight in agencyroutes) // Loop through List with for)
                                    {
                                        // Calender

                                        csvcalendar.WriteField(flight.FromIATA + flight.ToIATA + flight.FlightAirline + flight.FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", flight.FromDate) + String.Format("{0:yyyyMMdd}", flight.ToDate) + Convert.ToInt32(flight.FlightMonday) + Convert.ToInt32(flight.FlightTuesday) + Convert.ToInt32(flight.FlightWednesday) + Convert.ToInt32(flight.FlightThursday) + Convert.ToInt32(flight.FlightFriday) + Convert.ToInt32(flight.FlightSaterday) + Convert.ToInt32(flight.FlightSunday));
                                        csvcalendar.WriteField(Convert.ToInt32(flight.FlightMonday));
                                        csvcalendar.WriteField(Convert.ToInt32(flight.FlightTuesday));
                                        csvcalendar.WriteField(Convert.ToInt32(flight.FlightWednesday));
                                        csvcalendar.WriteField(Convert.ToInt32(flight.FlightThursday));
                                        csvcalendar.WriteField(Convert.ToInt32(flight.FlightFriday));
                                        csvcalendar.WriteField(Convert.ToInt32(flight.FlightSaterday));
                                        csvcalendar.WriteField(Convert.ToInt32(flight.FlightSunday));
                                        csvcalendar.WriteField(String.Format("{0:yyyyMMdd}", flight.FromDate));
                                        csvcalendar.WriteField(String.Format("{0:yyyyMMdd}", flight.ToDate));
                                        csvcalendar.NextRecord();

                                        // Trips

                                        //var item4 = _Airlines.Find(q => q.Name == flight.FlightAirline);
                                        //string TEMP_IATA = item4.IATA;
                                        var FromAirportInfo = IATAAirports.Find(q => q.stop_iata == flight.FromIATA);
                                        var ToAirportInfo = IATAAirports.Find(q => q.stop_iata == flight.ToIATA);

                                        csvtrips.WriteField(flight.FromIATA + flight.ToIATA);
                                        csvtrips.WriteField(flight.FromIATA + flight.ToIATA + flight.FlightAirline + flight.FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", flight.FromDate) + String.Format("{0:yyyyMMdd}", flight.ToDate) + Convert.ToInt32(flight.FlightMonday) + Convert.ToInt32(flight.FlightTuesday) + Convert.ToInt32(flight.FlightWednesday) + Convert.ToInt32(flight.FlightThursday) + Convert.ToInt32(flight.FlightFriday) + Convert.ToInt32(flight.FlightSaterday) + Convert.ToInt32(flight.FlightSunday));
                                        csvtrips.WriteField(flight.FromIATA + flight.ToIATA + flight.FlightAirline + flight.FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", flight.FromDate) + String.Format("{0:yyyyMMdd}", flight.ToDate) + Convert.ToInt32(flight.FlightMonday) + Convert.ToInt32(flight.FlightTuesday) + Convert.ToInt32(flight.FlightWednesday) + Convert.ToInt32(flight.FlightThursday) + Convert.ToInt32(flight.FlightFriday) + Convert.ToInt32(flight.FlightSaterday) + Convert.ToInt32(flight.FlightSunday));
                                        csvtrips.WriteField(ToAirportInfo.stop_name);
                                        csvtrips.WriteField(flight.FlightNumber + " " + ToAirportInfo.stop_name);
                                        csvtrips.WriteField("");
                                        csvtrips.WriteField("");
                                        csvtrips.WriteField("");
                                        csvtrips.WriteField("1");
                                        csvtrips.WriteField("");
                                        csvtrips.NextRecord();

                                        // Depart Record
                                        csvstoptimes.WriteField(flight.FromIATA + flight.ToIATA + flight.FlightAirline + flight.FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", flight.FromDate) + String.Format("{0:yyyyMMdd}", flight.ToDate) + Convert.ToInt32(flight.FlightMonday) + Convert.ToInt32(flight.FlightTuesday) + Convert.ToInt32(flight.FlightWednesday) + Convert.ToInt32(flight.FlightThursday) + Convert.ToInt32(flight.FlightFriday) + Convert.ToInt32(flight.FlightSaterday) + Convert.ToInt32(flight.FlightSunday));
                                        csvstoptimes.WriteField(String.Format("{0:HH:mm:ss}", flight.DepartTime));
                                        csvstoptimes.WriteField(String.Format("{0:HH:mm:ss}", flight.DepartTime));
                                        csvstoptimes.WriteField(flight.FromIATA);
                                        csvstoptimes.WriteField("0");
                                        csvstoptimes.WriteField("");
                                        csvstoptimes.WriteField("0");
                                        csvstoptimes.WriteField("0");
                                        csvstoptimes.WriteField("");
                                        csvstoptimes.WriteField("");
                                        csvstoptimes.NextRecord();
                                        // Arrival Record
                                        if (flight.DepartTime.TimeOfDay < System.TimeSpan.Parse("23:59:59") && flight.ArrivalTime.TimeOfDay > System.TimeSpan.Parse("00:00:00"))
                                        //if (!flight.FlightNextDayArrival)
                                        {
                                            csvstoptimes.WriteField(flight.FromIATA + flight.ToIATA + flight.FlightAirline + flight.FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", flight.FromDate) + String.Format("{0:yyyyMMdd}", flight.ToDate) + Convert.ToInt32(flight.FlightMonday) + Convert.ToInt32(flight.FlightTuesday) + Convert.ToInt32(flight.FlightWednesday) + Convert.ToInt32(flight.FlightThursday) + Convert.ToInt32(flight.FlightFriday) + Convert.ToInt32(flight.FlightSaterday) + Convert.ToInt32(flight.FlightSunday));
                                            csvstoptimes.WriteField(String.Format("{0:HH:mm:ss}", flight.ArrivalTime));
                                            csvstoptimes.WriteField(String.Format("{0:HH:mm:ss}", flight.ArrivalTime));
                                            csvstoptimes.WriteField(flight.ToIATA);
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
                                            int hour = flight.ArrivalTime.Hour;
                                            hour = hour + 24;
                                            int minute = flight.ArrivalTime.Minute;
                                            string strminute = minute.ToString();
                                            if (strminute.Length == 1) { strminute = "0" + strminute; }
                                            csvstoptimes.WriteField(flight.FromIATA + flight.ToIATA + flight.FlightAirline + flight.FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", flight.FromDate) + String.Format("{0:yyyyMMdd}", flight.ToDate) + Convert.ToInt32(flight.FlightMonday) + Convert.ToInt32(flight.FlightTuesday) + Convert.ToInt32(flight.FlightWednesday) + Convert.ToInt32(flight.FlightThursday) + Convert.ToInt32(flight.FlightFriday) + Convert.ToInt32(flight.FlightSaterday) + Convert.ToInt32(flight.FlightSunday));
                                            csvstoptimes.WriteField(hour + ":" + strminute + ":00");
                                            csvstoptimes.WriteField(hour + ":" + strminute + ":00");
                                            csvstoptimes.WriteField(flight.ToIATA);
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
                        // Create Zip File
                        string startPath = gtfsagencyDir;
                        string zipPath = gtfsDir + "\\" + agency[a].FlightAirline + ".zip";
                        if (File.Exists(zipPath)) { File.Delete(zipPath); }
                        ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, false);
                    }                    
                }
                else 
                {
                    string gtfsDirfull = AppDomain.CurrentDomain.BaseDirectory + "\\gtfs\\full";
                    System.IO.Directory.CreateDirectory(gtfsDirfull);
                    Console.WriteLine("Creating GTFS File agency.txt...");
                    using (var gtfsagency = new StreamWriter(@"gtfs\\full\\agency.txt"))
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

                    Console.WriteLine("Creating GTFS File routes.txt ...");

                    using (var gtfsroutes = new StreamWriter(@"gtfs\\full\\routes.txt"))
                    {
                        // Route record
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

                        var routes = CIFLights.Select(m => new { m.FromIATA, m.ToIATA, m.FlightAirline }).Distinct().ToList();

                        for (int i = 0; i < routes.Count; i++) // Loop through List with for)
                        {
                            //var item4 = _Airlines.Find(q => q.Name == routes[i].FlightAirline);
                            //string TEMP_Name = item4.DisplayName;
                            //string TEMP_Url = item4.WebsiteUrl;
                            //string TEMP_IATA = item4.IATA;

                            var FromAirportInfo = IATAAirports.Find(q => q.stop_iata == routes[i].FromIATA);
                            var ToAirportInfo = IATAAirports.Find(q => q.stop_iata == routes[i].ToIATA);


                            csvroutes.WriteField(routes[i].FromIATA + routes[i].ToIATA + routes[i].FlightAirline);
                            csvroutes.WriteField(routes[i].FlightAirline);
                            csvroutes.WriteField("");
                            csvroutes.WriteField(FromAirportInfo.stop_city + " - " + ToAirportInfo.stop_city);
                            csvroutes.WriteField(""); // routes[i].FlightAircraft + ";" + CIFLights[i].FlightAirline + ";" + CIFLights[i].FlightOperator + ";" + CIFLights[i].FlightCodeShare
                            csvroutes.WriteField(1102);
                            csvroutes.WriteField("");
                            csvroutes.WriteField("");
                            csvroutes.WriteField("");
                            csvroutes.NextRecord();
                        }
                    }

                    // stops.txt

                    List<string> agencyairportsiata =
                     CIFLights.SelectMany(m => new string[] { m.FromIATA, m.ToIATA })
                             .Distinct()
                             .ToList();

                    using (var gtfsstops = new StreamWriter(@"gtfs\\full\\stops.txt"))
                    {
                        // Route record
                        var csvstops = new CsvWriter(gtfsstops);
                        csvstops.Configuration.Delimiter = ",";
                        csvstops.Configuration.Encoding = Encoding.UTF8;
                        csvstops.Configuration.TrimFields = true;
                        // header                                 
                        csvstops.WriteField("stop_id");
                        csvstops.WriteField("stop_name");
                        csvstops.WriteField("stop_desc");
                        csvstops.WriteField("stop_lat");
                        csvstops.WriteField("stop_lon");
                        csvstops.WriteField("zone_id");
                        csvstops.WriteField("stop_url");
                        csvstops.NextRecord();

                        for (int i = 0; i < agencyairportsiata.Count; i++) // Loop through List with for)
                        {


                            //int result1 = IATAAirports.FindIndex(T => T.stop_id == 9458)
                            var airportinfo = IATAAirports.Find(q => q.stop_iata == agencyairportsiata[i]);
                            csvstops.WriteField(airportinfo.stop_iata);
                            csvstops.WriteField(airportinfo.stop_name);
                            csvstops.WriteField(airportinfo.stop_city + " - " + airportinfo.stop_country);
                            csvstops.WriteField(airportinfo.stop_lat);
                            csvstops.WriteField(airportinfo.stop_lon);
                            csvstops.WriteField("");
                            csvstops.WriteField("");
                            csvstops.NextRecord();
                        }
                    }                

                    Console.WriteLine("Creating GTFS File trips.txt and stop_times.txt...");
                    using (var gtfscalendar = new StreamWriter(@"gtfs\\full\\calendar.txt"))
                    {                    
                        using (var gtfstrips = new StreamWriter(@"gtfs\\full\\trips.txt"))
                        {
                            using (var gtfsstoptimes = new StreamWriter(@"gtfs\\full\\stop_times.txt"))
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

                                    //var item4 = _Airlines.Find(q => q.Name == CIFLights[i].FlightAirline);
                                    //string TEMP_IATA = item4.IATA;
                                    var FromAirportInfo = IATAAirports.Find(q => q.stop_iata == CIFLights[i].FromIATA);
                                    var ToAirportInfo = IATAAirports.Find(q => q.stop_iata == CIFLights[i].ToIATA);

                                    csvtrips.WriteField(CIFLights[i].FromIATA + CIFLights[i].ToIATA + CIFLights[i].FlightAirline);
                                    csvtrips.WriteField(CIFLights[i].FromIATA + CIFLights[i].ToIATA + CIFLights[i].FlightAirline + CIFLights[i].FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", CIFLights[i].FromDate) + String.Format("{0:yyyyMMdd}", CIFLights[i].ToDate) + Convert.ToInt32(CIFLights[i].FlightMonday) + Convert.ToInt32(CIFLights[i].FlightTuesday) + Convert.ToInt32(CIFLights[i].FlightWednesday) + Convert.ToInt32(CIFLights[i].FlightThursday) + Convert.ToInt32(CIFLights[i].FlightFriday) + Convert.ToInt32(CIFLights[i].FlightSaterday) + Convert.ToInt32(CIFLights[i].FlightSunday));
                                    csvtrips.WriteField(CIFLights[i].FromIATA + CIFLights[i].ToIATA + CIFLights[i].FlightAirline + CIFLights[i].FlightNumber.Replace(" ", "") + String.Format("{0:yyyyMMdd}", CIFLights[i].FromDate) + String.Format("{0:yyyyMMdd}", CIFLights[i].ToDate) + Convert.ToInt32(CIFLights[i].FlightMonday) + Convert.ToInt32(CIFLights[i].FlightTuesday) + Convert.ToInt32(CIFLights[i].FlightWednesday) + Convert.ToInt32(CIFLights[i].FlightThursday) + Convert.ToInt32(CIFLights[i].FlightFriday) + Convert.ToInt32(CIFLights[i].FlightSaterday) + Convert.ToInt32(CIFLights[i].FlightSunday));
                                    csvtrips.WriteField(ToAirportInfo.stop_city);
                                    csvtrips.WriteField(CIFLights[i].FlightNumber);
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
                                    csvstoptimes.WriteField(ToAirportInfo.stop_city);
                                    csvstoptimes.WriteField("0");
                                    csvstoptimes.WriteField("0");
                                    csvstoptimes.WriteField("");
                                    csvstoptimes.WriteField("");
                                    csvstoptimes.NextRecord();
                                    // Arrival Record
                                    if(CIFLights[i].DepartTime.TimeOfDay < System.TimeSpan.Parse("23:59:59") && CIFLights[i].ArrivalTime.TimeOfDay > System.TimeSpan.Parse("00:00:00"))
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
                    // Create Zip File
                    string startPath = gtfsDir + "\\full";
                    string zipPath = gtfsDir + "\\AeroCivil.zip";
                    if (File.Exists(zipPath)) { File.Delete(zipPath); }
                    ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, false);
                }
            }
        }        
    }
}

