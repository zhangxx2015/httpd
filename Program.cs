using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using zxLibHttp;

namespace httpd
{
    
    public class Entry : ServiceEntry {
        private static readonly int port = int.Parse(ConfigurationManager.AppSettings["port"]);
        private static readonly string staticdir = ConfigurationManager.AppSettings["staticdir"];
        private static readonly string UriPrefix = string.Format("http://*:{0}/", port);

        private bool _running;
        public void Control(bool running) {
            _running = running;
        }

        public void Execute(bool debug) {
            Console.WriteLine("tiny http server, create by zhangxx(20437023)");
            new Thread(() =>{
                Thread.Sleep(2000);
                var testUrl = string.Format("{0}echo/", UriPrefix).Replace("*","localhost");
                var respMessage = zxHttpClient.HttpGet(testUrl);
                Console.WriteLine("self test response:{0}", respMessage);
                Console.WriteLine("start service is succeed. listen port:[{0}]", port);
            }).Start();
            zxHttpServer.Running(UriPrefix,
                delegate(string route, string body, Dictionary<string, object> vars, HttpListenerRequest req, HttpListenerResponse resp) {
                    if (route.StartsWith("/echo/")){
                        var message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        return zxHttpServer.ToResp(Encoding.UTF8.GetBytes(message), mime: "application/json");
                    }
                    if (route.StartsWith(string.Format("/{0}/", staticdir))) {
                        var sfile = route.Replace('/', '\\');
                        var pidx = sfile.IndexOf('?');
                        if (-1 != pidx)
                            sfile = sfile.Substring(0, pidx);
                        var fullPath = string.Join("\\", Environment.CurrentDirectory, sfile).Replace("\\\\", "\\");
                        if (File.Exists(fullPath))
                            return zxHttpServer.ToResp(File.ReadAllBytes(fullPath), mime: MimeMapping.GetMimeMapping(fullPath));
                    }
                    return zxHttpServer.ToResp("404", 404);
                });
            Dbg.Log("start http service is succeed.[{0}]", UriPrefix);
            while (_running) {
                Thread.Sleep(1);
            }
        }
    }

    class Program{
        static void Main(string[] args){
            ServiceProgram.ServiceMain(args.Length > 0 ? args : new[] { "/H" });
            //ServiceProgram.ServiceMain(new[] {"/D"});
        }
    }
}
