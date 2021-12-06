using BnSBuddy2.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.NCServer
{
    public static class ServerValues
    {
        private static string CNAME_ResolverAsync(string addr)
        {
            try
            {
                var lookup = new DnsClient.LookupClientOptions
                {
                    UseCache = false
                };
                var querier = new DnsClient.LookupClient(lookup);
                DnsClient.IDnsQueryResponse result = querier.Query(addr, DnsClient.QueryType.CNAME);
                return result.Answers.CnameRecords().FirstOrDefault().CanonicalName.ToString().TrimEnd('.');
            }
            catch
            {
                return addr;
            }
        }

        private static string DNS_Resolver(string url)
        {
            try
            {
                string url2 = CNAME_ResolverAsync(url);
                IPAddress address = ((IEnumerable<IPAddress>)Dns.GetHostAddresses(url2)).FirstOrDefault<IPAddress>();
                if (address == null)
                {
                    return url;
                }
                else
                {
                    IPEndPoint ipEndPoint = new IPEndPoint(address, 27500);
                    return ipEndPoint.Address.ToString();
                }
            }
            catch
            {
                Form1.CurrentForm.AddLauncherLog("[Error] Unknown dns error occured!");
                return url;
            }
        }

        public static List<string> GetServerValues(string Server, string Service)
        {
            List<string> returnedValues = new List<string>();
            if (Service == "Version")
            {
                if (Server == "Korean")
                {
                    returnedValues.Add("up4svr.ncupdate.com"); // was up4svr.plaync.co.kr
                    if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                        returnedValues.Add("BNS_LIVE"); /* launcher version check: ncLauncherS */
                    else if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Test")
                        returnedValues.Add("BNS_TEST");
                }
                else if (Server == "NA/EU")
                {
                    returnedValues.Add("updater.nclauncher.ncsoft.com");
                    returnedValues.Add("BnS_UE4");
                }
                else if (Server == "Taiwan")
                {
                    returnedValues.Add("up4svr.plaync.com.tw");
                    returnedValues.Add("TWBNSUE4"); /* launcher version check: ncLauncherS */
                }
                else if (Server == "Japanese")
                {
                    returnedValues.Add("BnSUpdate.ncsoft.jp");
                    returnedValues.Add("BNS_JPN_UE4");
                }
                else
                {
                    returnedValues.Add("updater.nclauncher.ncsoft.com");
                    returnedValues.Add("BnS_UE4");
                }
                return returnedValues;
            }
            else if (Service == "Login")
            {
                if (Server == "Korean")
                {
                    returnedValues.Add("up4svr.ncupdate.com");
                    if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                        returnedValues.Add("NCLauncherW"); /* launcher version check: ncLauncherS */
                    else
                        returnedValues.Add("NCLauncherW");
                }
                else if (Server == "NA/EU")
                {
                    returnedValues.Add("updater.nclauncher.ncsoft.com");
                    returnedValues.Add("BnS_UE4");
                }
                else if (Server == "Taiwan")
                {
                    returnedValues.Add("up4svr.plaync.com.tw");
                    returnedValues.Add("ncLauncherS"); /* launcher version check: ncLauncherS */
                }
                else if (Server == "Japanese")
                {
                    returnedValues.Add("BnSUpdate.ncsoft.jp");
                    returnedValues.Add("ncLauncher");
                }
                else
                {
                    returnedValues.Add("updater.nclauncher.ncsoft.com");
                    returnedValues.Add("ncLauncherS");
                }
                return returnedValues;
            }
            else 
                return returnedValues;
        }
    }
}
