using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Linq;
using System.Linq;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using System.IO;

namespace MistTrainGirlsThief
{
    /// <summary>
    /// ミストレのリソースとAPIの保存を扱うクラス
    /// Rxでやろうとしてたからクラスを分けたけど、RxじゃなくしたのでProxyServiceに一本化すればよかったかも
    /// </summary>
    public class MTGThief
    {
        //Rxでやろうとしたときの名残
        //private ProxyService proxy;

        public MTGThief(ProxyService proxyService)
        {
            //proxy = proxyService;

            //Rxでやろうとしたときの名残
            //proxy.SessionCompleted
            //    .Where(session => session.HttpClient.Request.RequestUri.Host == "assets.mist-train-girls.com")
            //    //.Do(session => Logger.Instance.AddLog($"received: {session.HttpClient.Request.RequestUri}"))
            //    .Subscribe(async session =>
            //    {
            //        try
            //        {
            //            AssetReceived(session.HttpClient.Request.RequestUri.PathAndQuery, await session.GetResponseBody().Timeout(TimeSpan.FromSeconds(1)));
            //            //AssetReceived(session.HttpClient.Request.RequestUri.PathAndQuery, session.HttpClient.Response.Body);
            //        }
            //        catch (Exception)
            //        {
            //            Logger.Instance.AddLog("error!");
            //        }

            //    });

            //proxy.SessionCompleted
            //    //.Where(session => session.HttpClient.Request.RequestUri.Host == "mist-train-girls.azurefd.net")
            //    .Where(session => session.HttpClient.Request.RequestUri.AbsoluteUri.StartsWith("https://mist-train-girls.azurefd.net/api/"))
            //    //.Do(session => Logger.Instance.AddLog($"received: {session.HttpClient.Request.RequestUri}"))
            //    .Subscribe(async session =>
            //    {
            //        try
            //        {
            //            ApiReceived(session.HttpClient.Request.RequestUri.PathAndQuery, await session.GetResponseBody().Timeout(TimeSpan.FromSeconds(1)));
            //            //ApiReceived(session.HttpClient.Request.RequestUri.PathAndQuery, session.HttpClient.Response.Body);
            //        }
            //        catch (Exception)
            //        {
            //            Logger.Instance.AddLog("error!");
            //        }
            //    });
        }

        public void AssetReceived(string pathAndQuery, byte[] responseData)
        {
            try
            {
                if (responseData.Length == 0)
                {
                    return;
                }

                var path = pathAndQuery;
                if (path.Contains("?"))
                {
                    path = path.Substring(0, path.IndexOf("?"));
                }

                var filePath = "MistTrainGirls" + path.Replace("/", "\\");

                if (SaveFile(filePath, responseData))
                {
                    Logger.Instance.AddLog($"リソース保存: {path}");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.AddLog($"エラー: {ex.GetType()} {ex.Message}\r\n{ex.StackTrace}");
            }
        }

        public void ApiReceived(string pathAndQuery, byte[] responseData)
        {
            try
            {
                var apiPath = pathAndQuery.Substring(4);

                //たまにクエリ付きのがあるので取り除いておく
                if (apiPath.Contains("?"))
                {
                    apiPath = apiPath.Substring(0, apiPath.IndexOf("?"));
                }

                //リクエストも保存する前提で「pathBefore S/Q pathAfter」としたけど(Q: Req、S: Res)、めんどくさかったのでリクエスト保存はやめた
                //なのでbeforeとafterに分ける意味がなくなってしまった

                var pathBefore = "MistTrainGirls\\api\\" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var pathAfter = apiPath.Replace("/", "@") + ".json";

                if (responseData.Length != 0)
                {
                    var responsePath = pathBefore + "S" + pathAfter;
                    if (SaveFile(responsePath, responseData))
                    {
                        Logger.Instance.AddLog($"API保存: {apiPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.AddLog($"エラー: {ex.GetType()} {ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// ファイルに保存　すでにファイルがあれば上書きはしない
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SaveFile(string filePath, byte[] data)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }
                if (!File.Exists(filePath))
                {
                    File.WriteAllBytes(filePath, data);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.AddLog($"エラー@ファイル保存: {ex.GetType()} {ex.Message}");
            }
            return false;
        }
    }
}
