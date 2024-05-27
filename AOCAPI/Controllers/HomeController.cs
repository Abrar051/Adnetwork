using AOCAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Web;
using System;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace AOCAPI.Controllers
{
    public class HomeController : Controller
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task <IActionResult> FindSourceUrlCkey (string msisdn)
        {
            try
            {
                string connectionString = config.GetConnectionString("Basket");
                SqlConnection con = new SqlConnection(connectionString);
                string searchQuery = "select top(1) SOURCE_URL,ckey from Basket.dbo.tbl_all_log_Quiz_Bkash where MSISDN = '" + msisdn + "' and Theme_id like '%click%' order by TIME_STAMP desc";
                SqlCommand cmd = new SqlCommand(searchQuery, con);
                con.Open();
                var SOURCE_URL = cmd.ExecuteReader();
                if (SOURCE_URL.Read())
                {
                    // Create MsisdnCkey object from query result
                    MsisdnCkey msisdnCkey = new MsisdnCkey
                    {
                        SOURCE_URL = SOURCE_URL["SOURCE_URL"].ToString(),
                        ckey = SOURCE_URL["ckey"].ToString()
                    };
                    con.Close();
                    return Json(msisdnCkey);
                }
                if (SOURCE_URL == null)
                {
                    string searchQuery2 = "select top(1) SOURCE_URL,ckey from Basket.dbo.tbl_all_log_Quiz_Bkash where MSISDN = '" + msisdn + "' and Theme_id like '%signup_submit%' order by TIME_STAMP desc";
                    SqlCommand cmd2 = new SqlCommand(searchQuery, con);
                    SOURCE_URL = cmd.ExecuteReader();
                    if (SOURCE_URL.Read())
                    {
                        // Create MsisdnCkey object from query result
                        MsisdnCkey msisdnCkey = new MsisdnCkey
                        {
                            SOURCE_URL = SOURCE_URL["SOURCE_URL"].ToString(),
                            ckey = SOURCE_URL["ckey"].ToString()
                        };
                        con.Close();
                        return Json(msisdnCkey);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public MsisdnCkey FindSourceUrl (string msisdn)
        {
            try
            {
                string connectionString = config.GetConnectionString("Basket");
                SqlConnection con = new SqlConnection(connectionString);
                string searchQuery = "select top(1) SOURCE_URL,ckey from Basket.dbo.tbl_all_log_Quiz_Bkash where MSISDN = '" + msisdn + "' and Theme_id like '%click%' order by TIME_STAMP desc";
                SqlCommand cmd = new SqlCommand(searchQuery, con);
                con.Open();
                var SOURCE_URL = cmd.ExecuteReader();
                if (SOURCE_URL.Read())
                {
                    // Create MsisdnCkey object from query result
                    MsisdnCkey msisdnCkey = new MsisdnCkey
                    {
                        SOURCE_URL = SOURCE_URL["SOURCE_URL"].ToString(),
                        ckey = SOURCE_URL["ckey"].ToString()
                    };
                    con.Close();
                    return msisdnCkey;
                }
                if (SOURCE_URL == null)
                {
                    string searchQuery2 = "select top(1) SOURCE_URL,ckey from Basket.dbo.tbl_all_log_Quiz_Bkash where MSISDN = '" + msisdn +"' and Theme_id like '%signup_submit%' order by TIME_STAMP desc";
                    SqlCommand cmd2 = new SqlCommand(searchQuery, con);
                    SOURCE_URL = cmd.ExecuteReader();
                    if (SOURCE_URL.Read())
                    {
                        // Create MsisdnCkey object from query result
                        MsisdnCkey msisdnCkey = new MsisdnCkey
                        {
                            SOURCE_URL = SOURCE_URL["SOURCE_URL"].ToString(),
                            ckey = SOURCE_URL["ckey"].ToString()
                        };
                        con.Close();
                        return msisdnCkey;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }


        public int TrafficLogMSISDNCount(string MSISDN)
        {
            try
            {
                string connectionString = config.GetConnectionString("WapPortal_CMS");
                SqlConnection con = new SqlConnection(connectionString);
                string searchQuery = "select COUNT(*) from WapPortal_CMS.dbo.tbl_successful_adnetwork_traffic_log where MSISDN = '"+ MSISDN +"' and CAST (TIME_STAMP as date) = CAST (GETDATE() as date)";
                con.Open();
                SqlCommand cmd = new SqlCommand(searchQuery,con);
                var count = cmd.ExecuteScalar();
                con.Close();
                return (int)count ;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }


        public int TrafficLogCount(string ckey)
        {
            try
            {
                string connectionString = config.GetConnectionString("WapPortal_CMS");
                SqlConnection con = new SqlConnection(connectionString);
                string searchQuery = "select COUNT(*) from WapPortal_CMS.dbo.tbl_successful_adnetwork_traffic_log where CMPAIN_KEY = '"+ckey+"' and CAST (TIME_STAMP as date)=CAST (GETDATE() as date)";
                con.Open();
                SqlCommand cmd = new SqlCommand(searchQuery, con);
                var count = cmd.ExecuteScalar();
                con.Close();
                return (int)count;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }


        public string GetPage(string url)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
            string responseT = string.Empty;
            try
            {
                // Creates an HttpWebRequest for the specified URL. 
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                // Sends the HttpWebRequest and waits for a response.
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                UTF8Encoding encoding;
                encoding = new UTF8Encoding();
                StreamReader objStreamReader =
                new StreamReader(myHttpWebResponse.GetResponseStream(), encoding);
                string strResponse = "";

                strResponse = objStreamReader.ReadToEnd();

                if (url.Contains("http://n.gg.agency/ntf1/?token=ecbd48e5b20fcba0193558bdff4e9f32")
                    || url.Contains("http://m.mobplus.net/c/p/f30d2e9dda84451bb31c0e5e7fec2b97")
                    || url.Contains("http://162.243.217.139/dlv/track.php")
                    || url.Contains("https://adwynkmedia.o18.click/p")
                    || url.Contains("https://postback.level23.nl")
                    || url.Contains("https://smobipiumlink.com")
                    || url.Contains("http://offers.admoustache.affise.com")
                    || url.Contains("https://offers-advertizia.affise.com")
                    || url.Contains("https://funshop.o18.click")
                    || url.Contains("https://s2s.today")
                    || url.Contains("https://postback.mobidea.ai")
                    || url.Contains("http://www.pbterra.com")
                    || url.Contains("https://www.m-taikoo.com")
                    || url.Contains("http://postback.advertizer.com")
                    || url.Contains("https://offers-reclamedigital.affise.com")
                    || url.Contains("http://m.m2888.net")
                    || url.Contains("http://m.witskies.click")
                    )
                {
                    responseT = strResponse + "_" + myHttpWebResponse.StatusCode;
                }
                else
                {
                    responseT = myHttpWebResponse.ToString();
                }

                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                    Console.WriteLine("\r\nResponse Status Code is OK and StatusDescription is: {0}",
                                         myHttpWebResponse.StatusDescription);
                // Releases the resources of the response.
                myHttpWebResponse.Close();

            }
            catch (WebException e)
            {
                Console.WriteLine("\r\nWebException Raised. The following error occured : {0}", e.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nThe following Exception was raised : {0}", e.Message);
            }

            return responseT;
        }


        public bool UpdatePostBack (string url , string clickId , string ckey , string MSISDN , string response )
        {
            string TIME_STAMP = DateTime.Now.ToString();
            SqlConnection con = new SqlConnection(config.GetConnectionString("WapPortal_CMS"));
            con.Open();
            try
            {
                string insertQuery = "INSERT into WapPortal_CMS.dbo.tbl_successful_adnetwork_traffic_log values ('" + url + "','" + clickId + "','" + MSISDN + "','" + ckey + "',GETDATE(),'" + response + "')";
                SqlCommand cmd = new SqlCommand(insertQuery, con);
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                if (rowsAffected > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                con.Close();
                return false;
            }
            

        }


        public async Task<IActionResult> PostBackBkash (string msisdn)
        {
            try
            {
                MsisdnCkey msisdnCkey = FindSourceUrl(msisdn); // Get msisdn with ckey
                string sourceUrl = msisdnCkey.SOURCE_URL ;
                var sourceUrlMerge = sourceUrl;

                Uri myUri = new Uri(sourceUrlMerge);

                Dictionary<string, string> trafficNetworkDeterminer = new Dictionary<string, string>
                {
                    {"click_id", "http://n.gg.agency/ntf1/?token=ecbd48e5b20fcba0193558bdff4e9f32&profit=0.35&event=redeem&click_id=" + HttpUtility.ParseQueryString(myUri.Query).Get("click_id")  } ,   //Golden Goose

                    {"tid",     "https://shakingclickss.o18.click/p?m=12421&tid=" + HttpUtility.ParseQueryString(myUri.Query).Get("tid")},         //Rendimento

                    {"txid",    "http://m.m2888.net/c/p/7bea36fc812a401588eada6f249c5937?txid=" + HttpUtility.ParseQueryString(myUri.Query).Get("txid")},         //Grandtech

                    {"revlinkerid", "http://www.mypostback.com/ec789c4c-a4bf-4c94-a33b-bcd8a53c1c8d?revlinkerid=" + HttpUtility.ParseQueryString(myUri.Query).Get("revlinkerid") }, //revlinker id

                    {"levelid", "https://postback.level23.nl/?currency=USD&handler=10306&hash=bda25444728ae8b4099a2b05b5f3a00a&tracker=" + HttpUtility.ParseQueryString(myUri.Query).Get("levelid")} ,    //level23

                    {"mobipiumcid" , "https://smobipiumlink.com/conversion/index.php?jp=" + HttpUtility.ParseQueryString(myUri.Query).Get("mobipiumcid") + "&source=" + HttpUtility.ParseQueryString(myUri.Query).Get("mobipiumsid") + "&payout=" + HttpUtility.ParseQueryString(myUri.Query).Get("payout") },  //mobipium

                    {"trackgama" , "https://conv.trackgama.com/tracking/postback?click_id=" + HttpUtility.ParseQueryString(myUri.Query).Get("click_id") + "&conv_ip=" + HttpUtility.ParseQueryString(myUri.Query).Get("conv_ip") + "&event_type=" + HttpUtility.ParseQueryString(myUri.Query).Get("event_type") } , // adgama

                    {"clickid" , "https://adzcorner.o18.click/p?m=4198&tid=" + HttpUtility.ParseQueryString(myUri.Query).Get("clickid") } ,      //adzcorner

                    {"witskiesid" , "http://m.witskies.click/c/p/8791835c2d964f36b4a569edaaec75bf?txid=" + HttpUtility.ParseQueryString(myUri.Query).Get("witskiesid") + "&pubid=" + HttpUtility.ParseQueryString(myUri.Query).Get("pub_id") }  ,              //witskies

                    {"offerstarget" , "https://offerstarget.com/aff_lpb?transaction_id=" + HttpUtility.ParseQueryString(myUri.Query).Get("transaction_id") }  ,    //reclame

                    {"mobikokId" , "http://trace.sm4link.com/pb?a=1080&tid=" + HttpUtility.ParseQueryString(myUri.Query).Get("mobikokId") },   /// mobikok

                    {"olimobid" , "https://postback.76tfhu87.com/?sign=646ace654eef0f9aff61372dfeaa6f94&click=" + HttpUtility.ParseQueryString(myUri.Query).Get("olimobid") },   //olimob

                    {"collectclickid" , "http://162.243.217.139/dlv/track.php?ccuid=" + HttpUtility.ParseQueryString(myUri.Query).Get("collectclickid")} ,   //collectcent

                    {"adv_sub1" , "https://www.m-taikoo.com/p?m=" + HttpUtility.ParseQueryString(myUri.Query).Get("m") + "&tid=" + HttpUtility.ParseQueryString(myUri.Query).Get("tid") + "&adv_sub1=" + HttpUtility.ParseQueryString(myUri.Query).Get("adv_sub1") }      // advertizia

                };


                Dictionary <string , int> postbackRatio = new Dictionary<string, int>
                {
                    {"click_id" , 2 }, //Golden Goose
                    {"tid" , 2},    //Rendimento
                    {"txid" , 2 },     //Grandtech
                    {"revlinkerid" ,2 },  //revlinker id
                    {"levelid", 2 },    //level23
                    {"mobipiumcid", 2 }, //mobipium
                    {"trackgama", 2 }, // adgama
                    {"clickid", 2 }, //adzcorner
                    {"witskiesid" ,2}, //witskies
                    {"offerstarget",2 }, //reclame
                    {"mobikokId",2 }, /// mobikok
                    {"olimobid" ,2},    //olimob
                    {"collectclickid",2 }, //collectcent
                    {"adv_sub1",2 }, // advertizia

                };

                foreach (var trafficNetwork in trafficNetworkDeterminer)
                {
                    bool id = sourceUrlMerge.Contains(trafficNetwork.Key);
                    int postBackRatio = postbackRatio[trafficNetwork.Key];  // ratio of skip
                    if (id == true)
                    {
                        int trafficLogCount = TrafficLogCount(msisdnCkey.ckey);   // Get count for specific ckey
                        if (trafficLogCount % postbackRatio[trafficNetwork.Key] == 0)
                        {
                            bool update = UpdatePostBack("" , HttpUtility.ParseQueryString(myUri.Query).Get(trafficNetwork.Key), msisdnCkey.ckey, msisdn, "Postback skipped");
                            if (update == true)
                            {
                                return Ok("Postback Skipped");
                            }
                            else
                            {
                                return Ok("Update error");
                            }
                        }
                        else
                        {
                            string response = GetPage(trafficNetwork.Value);
                            bool update = UpdatePostBack(trafficNetwork.Value, HttpUtility.ParseQueryString(myUri.Query).Get(trafficNetwork.Key), msisdnCkey.ckey, msisdn, response);
                            if (update == true)
                            {
                                return Ok("Postback");
                            }
                            else
                            {
                                return Ok("Update error");
                            }
                        }
                        //return Json(trafficNetwork);
                        //return Json(postBackRatio);
                    }
                }
                return Json("");
            }
            catch (Exception ex)
            {
                return Json("");
            }
            //return Json(sourceUrl);
        }



    }
}