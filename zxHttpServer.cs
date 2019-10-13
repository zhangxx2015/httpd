using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace zxLibHttp
{
    using client = zxHttpClient;
    using server = zxHttpServer;

    public static class zxHttpServer {
        private static string Uncompress(string acceptEncoding, Stream inputStream) {
            return Uncompress(string.IsNullOrEmpty(acceptEncoding) ? null : acceptEncoding.Split(new[] { ',' }), inputStream);
        }

        private static string Uncompress(string[] acceptEncoding, Stream inputStream) {
            string data;
            if (acceptEncoding != null) {
                var contentEncoding = string.Join(",", acceptEncoding).ToUpper();
                if (contentEncoding.Contains("GZIP")) {
                    using (var stream = new GZipStream(inputStream, CompressionMode.Decompress)) {
                        const int size = 4096;
                        var buffer = new byte[size];
                        using (var memory = new MemoryStream()) {
                            int count;
                            do {
                                count = stream.Read(buffer, 0, size);
                                if (count > 0)
                                    memory.Write(buffer, 0, count);
                            } while (count > 0);
                            data = Encoding.UTF8.GetString(memory.ToArray());
                            memory.Close();
                        }
                        stream.Flush();
                        stream.Close();
                    }
                    return data;
                }
                if (contentEncoding.Contains("DEFLATE")) {
                    using (var stream = new DeflateStream(inputStream, CompressionMode.Decompress)) {
                        const int size = 4096;
                        var buffer = new byte[size];
                        using (var memory = new MemoryStream()) {
                            int count;
                            do {
                                count = stream.Read(buffer, 0, size);
                                if (count > 0)
                                    memory.Write(buffer, 0, count);
                            } while (count > 0);
                            data = Encoding.UTF8.GetString(memory.ToArray());
                            memory.Close();
                            memory.Dispose();
                        }
                        stream.Flush();
                        stream.Close();
                    }
                    return data;
                }
                throw new Exception("unknow compress format.");
            }
            using (var sr = new StreamReader(inputStream)) {
                data = sr.ReadToEnd();
                sr.Close();
            }
            return data;
        }

        private static string Uncompress(string acceptEncoding, byte[] bytes) {
            string data;
            using (var inputStream = new MemoryStream(bytes)) {
                if (!string.IsNullOrEmpty(acceptEncoding)) {
                    if (acceptEncoding.ToUpper().Contains("GZIP")) {
                        using (var stream = new GZipStream(inputStream, CompressionMode.Decompress)) {
                            const int size = 4096;
                            var buffer = new byte[size];
                            using (var memory = new MemoryStream()) {
                                int count;
                                do {
                                    count = stream.Read(buffer, 0, size);
                                    if (count > 0)
                                        memory.Write(buffer, 0, count);
                                } while (count > 0);
                                data = Encoding.UTF8.GetString(memory.ToArray());
                                memory.Close();
                            }
                            stream.Flush();
                            stream.Close();
                        }
                        return data;
                    }
                    if (acceptEncoding.ToUpper().Contains("DEFLATE")) {
                        using (var stream = new DeflateStream(inputStream, CompressionMode.Decompress)) {
                            const int size = 4096;
                            var buffer = new byte[size];
                            using (var memory = new MemoryStream()) {
                                int count;
                                do {
                                    count = stream.Read(buffer, 0, size);
                                    if (count > 0)
                                        memory.Write(buffer, 0, count);
                                } while (count > 0);
                                data = Encoding.UTF8.GetString(memory.ToArray());
                                memory.Close();
                                memory.Dispose();
                            }
                            stream.Flush();
                            stream.Close();
                        }
                        return data;
                    }
                    throw new Exception("unknow compress format.");
                }
                using (var sr = new StreamReader(inputStream)) {
                    data = sr.ReadToEnd();
                    sr.Close();
                }
            }
            return data;
        }




        private static void Compress(string acceptEncoding, List<byte> datas, Stream outputStream) {
            if (!string.IsNullOrEmpty(acceptEncoding)) {
                if (acceptEncoding.ToUpper().Contains("GZIP")) {
                    using (var writer = new GZipStream(outputStream, CompressionMode.Compress, true)) {
                        int index = 0;
                        while (datas.Count > 0) {
                            const int blockSize = 4096;
                            int length = datas.Count >= blockSize ? blockSize : datas.Count;
                            writer.Write(datas.GetRange(0, length).ToArray(), 0, length);
                            if (index++ % 2 == 0) {
                                writer.Flush();
                                Thread.Sleep(1);
                            }
                            datas.RemoveRange(0, length);
                        }
                        if (datas.Count > 0) {
                            writer.Write(datas.ToArray(), 0, datas.Count);
                            writer.Flush();
                        }
                        writer.Close();
                    }
                    return;
                }
                if (acceptEncoding.ToUpper().Contains("DEFLATE")) {
                    using (var writer = new DeflateStream(outputStream, CompressionMode.Compress, true)) {

                        int index = 0;
                        while (datas.Count > 0) {
                            const int blockSize = 4096;
                            int length = datas.Count >= blockSize ? blockSize : datas.Count;
                            writer.Write(datas.GetRange(0, length).ToArray(), 0, length);
                            if (index++ % 2 == 0) {
                                writer.Flush();
                                Thread.Sleep(1);
                            }
                            datas.RemoveRange(0, length);
                        }
                        if (datas.Count > 0) {
                            writer.Write(datas.ToArray(), 0, datas.Count);
                            writer.Flush();
                        }
                        writer.Close();
                    }
                    return;
                }
                throw new Exception("unknow compress format.");
            }
            using (var writer = new BinaryWriter(outputStream)) {
                writer.Write(datas.ToArray());
                writer.Flush();
                writer.Close();
            }
        }

        private static byte[] Compress(string acceptEncoding, byte[] inputData) {
            List<byte> inputBytes = new List<byte>(inputData);
            using (var outputStream = new MemoryStream()) {
                Compress(acceptEncoding, inputBytes, outputStream);
                inputBytes.Clear();
                inputBytes.AddRange(outputStream.ToArray());
            }
            return inputBytes.ToArray();
        }

        //private static readonly string LogFolder = ".";//ConfigurationManager.AppSettings["LogFolder"];
        //public static void Log(string msg) {
        //    try {
        //        //if (!Directory.Exists(LogFolder))
        //        //    Directory.CreateDirectory(LogFolder);
        //        var log = string.Format("{0}\t{1}", DateTime.Now.ToString("HH:mm:ss.fff"), msg);
        //        //File.AppendAllLines(string.Format(@"{0}\{1}.log", LogFolder, DateTime.Now.ToString("yyyy-MM-dd")), new[] { log });
        //        Console.WriteLine(log.PadRight(70, '.').Substring(0, 70));
        //    } catch (Exception) {
        //    }
        //}

        public static Tuple<byte[], int,string> ToResp(string text, int code = 200,string mime="text/html") {
            return new Tuple<byte[], int,string>( Encoding.Default.GetBytes(text),code,mime);
        }
        public static Tuple<byte[], int,string> ToResp(byte[] bytes, int code = 200, string mime = "application/octet-stream ") {
            return new Tuple<byte[], int,string>(bytes, code,mime);
        }
        public static bool IsRunning = false;
        public static string UriPrefix = "http://*:8080/";
        public static void Running(string uriPrefix, Func<string, string, Dictionary<string, object>,HttpListenerRequest,HttpListenerResponse, Tuple<byte[], int,string>> proc) {
            new Thread(delegate() {
                using (var listerner = new HttpListener()) {
                    IsRunning = true;
                    listerner.AuthenticationSchemes = AuthenticationSchemes.Anonymous;//指定身份验证 Anonymous匿名访问
                    listerner.Prefixes.Add(uriPrefix);
                    listerner.Start();
                    //TinyHttp.Log("WebServer Start Successed.......");
                    while (IsRunning) {
                        try {
                            var ctx = listerner.GetContext();
                            var req = ctx.Request;
                            var res = ctx.Response;



                            res.StatusCode = 200;//设置返回给客服端http状态代码
                            //string name = req.QueryString["name"];
                            //if (name != null)
                            //    Console.WriteLine(name);

                            var session = new Dictionary<string, object>();
                            //var qBody = req.HttpMethod.ToUpper().Equals("POST") ? Uncompress(req.Headers["Content-encoding"], req.InputStream) : new UriBuilder(req.Url).Query;

                            var qBody = new UriBuilder(req.Url).Query;
                            var queryItems = HttpUtility.ParseQueryString(qBody);
                            if (req.HttpMethod.ToUpper().Equals("POST")){
                                var pparam = HttpUtility.ParseQueryString(Uncompress(req.Headers["Content-encoding"], req.InputStream));
                                foreach (var key in pparam.Keys){
                                    queryItems.Add(key.ToString(),pparam.Get(key.ToString()));
                                }
                            } 


                            if (queryItems.Count > 0) {
                                foreach (var key in queryItems.Keys.Cast<object>().Where(key => null != key)) {
                                    if (session.ContainsKey(key.ToString()))
                                        throw new Exception("Duplicate parameter!");
                                    session.Add(key.ToString(), queryItems[key.ToString()]);
                                }
                            }
                            //var route = req.RawUrl.Replace(req.UserHostAddress, string.Empty);
                            //if (!string.IsNullOrEmpty(qBody))route = route.Replace(qBody, string.Empty);
                            var route = req.RawUrl;
                            var pidx = route.IndexOf("?", System.StringComparison.Ordinal);
                            if(-1!=pidx){
                                route = route.Substring(0, pidx);
                            }
                            route = HttpUtility.UrlDecode(route);

                            
                            var resp = proc(route, qBody, session, req, res);
                            //resp.Req = req.RawUrl;

                            //var html = JsonConvert.SerializeObject(resp, Formatting.Indented);
                            
                            res.StatusCode = resp.Item2;
                            res.ContentType = resp.Item3;
                            res.ContentEncoding = Encoding.UTF8;
                            //使用Writer输出http响应代码
                            using (var writer = new BinaryWriter(res.OutputStream)) {
                                //Console.WriteLine("hello");
                                //writer.WriteLine("<html><head><title>The WebServer Test</title></head><body>");
                                //writer.WriteLine("<div style=\"height:20px;color:blue;text-align:center;\"><p> hello {0}</p></div>", name);
                                //writer.WriteLine("<ul>");
                                //foreach (string header in req.Headers.Keys)
                                //    writer.WriteLine("<li><b>{0}:</b>{1}</li>", header, req.Headers[header]);
                                //writer.WriteLine("</ul>");
                                //writer.WriteLine("</body></html>");

                                //writer.Write(html);
                                
                                writer.Write(resp.Item1);
                                writer.Flush();

                                writer.Close();
                                res.Close();
                            }





                        } catch (Exception ex) {
                            //File.AppendAllLines(string.Format("ReportServer{0}.log",DateTime.Now.ToString("yyyy-MM-dd")),new []{ex.Message});
                            //Log(ex.Message);
                            System.Diagnostics.Trace.WriteLine(ex.Message);
                        }
                    }
                    listerner.Stop();
                }
            }) { IsBackground = true }.Start();
        }

    }
}
