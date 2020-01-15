using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SharpNetCheck
{
    class NetCheck
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Author: Uknow");
            System.Console.WriteLine("Github: https://github.com/uknowsec/SharpNetCheck");
            System.Console.WriteLine("");
            if (args.Length < 3)
            {
                System.Console.WriteLine("Usage: SharpNetCheck -dns -host ceye.io");
                System.Console.WriteLine("       SharpNetCheck -http -host/ip ceye.io");
                System.Console.WriteLine("       SharpNetCheck -all -host ceye.io");
            }
            if (args.Length >= 3 && (args[0] == "-dns"))
            {
                Console.WriteLine();
                dns(args[2]);
                Console.WriteLine("Net Ckeck by DNS , Please check the DNSlog");
                Console.WriteLine();
            }
            if (args.Length >= 3 && (args[0] == "-http"))
            {
                Console.WriteLine();
                http(args[2]);
                Console.WriteLine("Net Ckeck by HTTP , Please check the DNSlog");
                Console.WriteLine();
            }
            if (args.Length >= 3 && (args[0] == "-all"))
            {
                Console.WriteLine();
                http(args[2]);
                dns(args[2]);
                Console.WriteLine("Net Ckeck by DNS and HTTP , Please check the DNSlog");
                Console.WriteLine();
            }
        }

        public static void dns(string host)
        {
            string machineName = Environment.MachineName;
            string ip = "";
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            foreach (IPAddress ipa in ipadrlist)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = ip + ipa.ToString() + "-";
                }
            }
            string dnsstring = ip + machineName;
            Cmd c = new Cmd();
            string all = c.RunCmd("nslookup " + dnsstring + "." + host);

        }

        public static void http(string url)
        {
            string machineName = Environment.MachineName;
            int i =1 ;
            NameValueCollection postValues = new NameValueCollection();
            postValues.Add("host", machineName);
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            foreach (IPAddress ipa in ipadrlist)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                {
                    //组装数据  
                    postValues.Add("ip"+ i.ToString(), ipa.ToString());
                    i++;
                }
            }
            Post(url, postValues);
        }

        public static void Post(string url, NameValueCollection postValues)
        {
            try
            {
                //定义webClient对象
                WebClient webClient = new WebClient();

                url = "http://" + url + "/";

                //向服务器发送POST数据
                byte[] responseArray = webClient.UploadValues(url, postValues);
                string data = Encoding.ASCII.GetString(responseArray);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Cmd 的摘要说明。
        /// </summary>
        public class Cmd
        {
            private Process proc = null;
            /// <summary>
            /// 构造方法
            /// </summary>
            public Cmd()
            {
                proc = new Process();
            }
            /// <summary>
            /// 执行CMD语句
            /// </summary>
            /// <param name="cmd">要执行的CMD命令</param>
            public string RunCmd(string cmd)
            {
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();
                proc.StandardInput.WriteLine(cmd + "&exit");
                string outStr = proc.StandardOutput.ReadToEnd();
                proc.Close();
                return outStr;
            }
        }
     }
}
