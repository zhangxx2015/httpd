using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public static class Dbg
{
    private static readonly string DbgHost = ConfigurationManager.AppSettings["DbgHost"];
    private static readonly int DbgPort = int.Parse(ConfigurationManager.AppSettings["DbgPort"]);

    public static void Log(string format,params object[] objs){
        using(var client = new UdpClient()){
            var bytes = Encoding.Default.GetBytes(string.Format(format,objs));
            var timeout = Environment.TickCount + 5000;
            while (bytes.Length > client.Send(bytes, bytes.Length, DbgHost, DbgPort) || Environment.TickCount > timeout) {
                Thread.Sleep(1);
            }
        }
    }
}
    public class ServiceProgram : ServiceBase {
        public static string InstallServiceName = "zxHttpd";
        private static bool Debug=false;

        public static void ServiceMain(string[] args) {
            //#if DEBUG
            //            args = new[] {"/d"};
            //#endif
            if (args.Length > 0) {
                foreach (var key in args) {
                    switch (key.ToUpper()) {
                        case "/H":
                            Console.WriteLine(@"
/H Show Help
/I Install Service
/U Uninstall Service
/S Start Service
/T Stop Service
/D Debug Service
");
                            return;
                            break;
                        case "/I":
                            InstallService();
                            return;
                            break;
                        case "/U":
                            UninstallService();
                            return;
                            break;
                        case "/S":
                            try {
                                //foreach (var p in Process.GetProcesses(InstallServiceName).Where(p => p.Id != Process.GetCurrentProcess().Id)) p.Kill();
                                using (var process = Process.Start(new ProcessStartInfo() { FileName = "net", Arguments = string.Format(" start {0}", InstallServiceName), CreateNoWindow = true, UseShellExecute = false, RedirectStandardError = true })) {
                                    if (process == null)
                                        return;
                                    process.WaitForExit();
                                    var err = process.StandardError.ReadToEnd();
                                    if (!string.IsNullOrEmpty(err)) {
                                        Console.WriteLine(err);
                                        return;
                                    }
                                    foreach (var sc in ServiceController.GetServices().Where(sc => sc.ServiceName == InstallServiceName)) {
                                        Console.WriteLine("Start Service is {0}", sc.CanStop ? "succeed" : "failed");
                                        break;
                                    }
                                }
                            } catch (Exception) {
                            }
                            return;
                            break;
                        case "/T":
                            using (var process = Process.Start(new ProcessStartInfo() { FileName = "net", Arguments = string.Format(" stop {0}", InstallServiceName), CreateNoWindow = true, UseShellExecute = false, RedirectStandardError = true })) {
                                if (process == null)
                                    return;
                                process.WaitForExit();
                                var err = process.StandardError.ReadToEnd();
                                if (!string.IsNullOrEmpty(err)) {
                                    Console.WriteLine(err);
                                    return;
                                }
                                foreach (var sc in ServiceController.GetServices().Where(sc => sc.ServiceName == InstallServiceName)) {
                                    Console.WriteLine("Stop Service is {0}", !sc.CanStop ? "succeed" : "failed");
                                    break;
                                }
                            }
                            return;
                            break;
                        case "/D":
                            Debug = true;
                            var service = new ServiceProgram();
                            service.OnStart(null);
                            Console.WriteLine("Service Started...");
                            Console.WriteLine("<press any key to exit...>");
                            Console.Read();
                            return;
                            break;
                        default:
                            return;
                            break;
                    }
                }
            }
            ServiceBase.Run(new ServiceProgram());
        }


        readonly ServiceEntry inst = (from type in Assembly.GetExecutingAssembly().GetTypes() where null != type.GetInterface("ServiceEntry") select (ServiceEntry)Activator.CreateInstance(type)).FirstOrDefault();

        //public void Start(string[] args) {
        //    OnStart(args);
        //}

        public static bool running;
        protected override void OnStart(string[] args)
        {


            try {
                //start any threads or http listeners etc
                //var process = Process.GetProcessesByName(InstallServiceName).FirstOrDefault();
                //if (null != process && Process.GetCurrentProcess().Id != process.Id) process.Kill();
                running = true;
                inst.Control(running);
                new Thread(delegate() {



                    inst.Execute(Debug);
                    //new ServiceEntry().Execute(Debug);




                    //var client = new UdpClient();
                    while (running) {
                        Thread.Sleep(1);
                        //    Thread.Sleep(1000);
                        //    client.Send(Encoding.UTF8.GetBytes("hello"), 5, "192.168.1.186", 31120);
                    }


                }) { IsBackground = true }.Start();
            } catch (Exception ex) {
                EventLog.WriteEntry(ex.Message + ex.StackTrace);
            }



        }

        /// <SUMMARY>
        /// Stop this service.
        /// </SUMMARY>
        protected override void OnStop() {
            //stop any threads here and wait for them to be stopped.
            inst.Control(false);
            running = false;
            
        }

        protected override void Dispose(bool disposing) {
            //clean your resources if you have to
            base.Dispose(disposing);
        }





        private static bool IsServiceInstalled() {
            return ServiceController.GetServices().Any(s => s.ServiceName == InstallServiceName);
        }

        private static void InstallService() {
            if (IsServiceInstalled()) {
                UninstallService();
            }
            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
        }

        private static void UninstallService() {
            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
        }
    }
