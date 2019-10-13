using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace zxLibHttp
{
    public static class zxHttpClient
    {
        public static byte[] HttpGetBytes(string url, int timeout = 5000) {
            try {
                //const int HttpWaitTimeout = 5000;
                using (var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.None }) {
                    using (var http = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 0, 0, timeout) }) {
                        //_logger.Info("WakeUp to invoke!");
                        //////using (var content = new FormUrlEncodedContent(
                        //////    /*
                        //////    JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        //////    System.Configuration.ConfigurationManager.AppSettings["PostArgs"]

                        //////    )
                        //////    */
                        //////        args
                        //////        )) {
                        using (var postAsync = http.GetAsync(url)) {
                            postAsync.Wait();
                            using (var response = postAsync.Result) {
                                response.EnsureSuccessStatusCode();
                                using (var async = response.Content.ReadAsByteArrayAsync()) {
                                    async.Wait();
                                    return async.Result;
                                }//using(var readAsStringAsync
                            }//using (var response
                        }//using(var postAsync

                        //////}//using(var content

                    }//using (var http
                }//using (var handler
            } catch (Exception ex) {
            }
            return null;
        }
        public static string HttpGet(string url,int timeout = 5000) {
            try {
                //const int HttpWaitTimeout = 5000;
                using (var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.None }) {
                    using (var http = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 0, 0, timeout) }) {
                        //_logger.Info("WakeUp to invoke!");
                        //////using (var content = new FormUrlEncodedContent(
                        //////    /*
                        //////    JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        //////    System.Configuration.ConfigurationManager.AppSettings["PostArgs"]

                        //////    )
                        //////    */
                        //////        args
                        //////        )) {
                        using (var postAsync = http.GetAsync(url)) {
                            postAsync.Wait();
                            using (var response = postAsync.Result) {
                                response.EnsureSuccessStatusCode();
                                using (var readAsStringAsync = response.Content.ReadAsStringAsync()) {
                                    readAsStringAsync.Wait();
                                    var json = readAsStringAsync.Result;
                                    //var ret = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                                    //if (ret["Status"].ToString() == "OK" && ret["ResType"].ToString() == "Ok") {
                                    //    _logger.Info("succeed of invoke");
                                    //    continue;
                                    //}
                                    //_logger.Error("failed of invoke.");
                                    return json;
                                }//using(var readAsStringAsync
                            }//using (var response
                        }//using(var postAsync

                        //////}//using(var content

                    }//using (var http
                }//using (var handler
            } catch (Exception ex) {
            }
            return null;
        }
        public static byte[] HttpPostBytes(string url, Dictionary<string, string> args = null, int timeout = 5000) {
            //const int HttpWaitTimeout = 5000;
            using (var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.None }) {
                using (var http = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 0, 0, timeout) }) {
                    //_logger.Info("WakeUp to invoke!");
                    using (var content = new FormUrlEncodedContent(
                        /*
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        System.Configuration.ConfigurationManager.AppSettings["PostArgs"]
                                    
                        )
                        */
                        args
                        )) {
                            using (var postAsync = http.PostAsync(url, content)) {
                                postAsync.Wait();
                                using (var response = postAsync.Result) {
                                    response.EnsureSuccessStatusCode();
                                    using (var async = response.Content.ReadAsByteArrayAsync()) {
                                        async.Wait();
                                        return async.Result;
                                        //var ret = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                                        //if (ret["Status"].ToString() == "OK" && ret["ResType"].ToString() == "Ok") {
                                        //    _logger.Info("succeed of invoke");
                                        //    continue;
                                        //}
                                        //_logger.Error("failed of invoke.");
                                        //return json;
                                    }//using(var readAsStringAsync
                                }//using (var response
                            }//using(var postAsync
                        }//using(var content
                }//using (var http
            }//using (var handler
        }
        public static string HttpPost(string url, Dictionary<string, string> args = null,int timeout=5000) {
            //const int HttpWaitTimeout = 5000;
            using (var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.None }) {
                using (var http = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 0, 0, timeout) }) {
                    //_logger.Info("WakeUp to invoke!");
                    using (var content = new FormUrlEncodedContent(
                        /*
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        System.Configuration.ConfigurationManager.AppSettings["PostArgs"]
                                    
                        )
                        */
                        args
                        )) {
                            using (var postAsync = http.PostAsync(url, content)) {
                                postAsync.Wait();
                                using (var response = postAsync.Result) {
                                    response.EnsureSuccessStatusCode();
                                    using (var readAsStringAsync = response.Content.ReadAsStringAsync()) {
                                        readAsStringAsync.Wait();
                                        var json = readAsStringAsync.Result;
                                        //var ret = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                                        //if (ret["Status"].ToString() == "OK" && ret["ResType"].ToString() == "Ok") {
                                        //    _logger.Info("succeed of invoke");
                                        //    continue;
                                        //}
                                        //_logger.Error("failed of invoke.");
                                        return json;
                                    }//using(var readAsStringAsync
                                }//using (var response
                            }//using(var postAsync
                        }//using(var content
                }//using (var http
            }//using (var handler
        }


    }
}