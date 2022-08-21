using System;
using System.Collections.Generic;
using System.Text;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Network;
using Titanium.Web.Proxy.Models;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Linq;

namespace MistTrainGirlsThief
{
    public class ProxyService
    {
        //パディントン駅が1854年にできたらしいので1854
        private readonly int Port = 11854;

        private MTGThief mtgThief;

        #region Singleton

        public static ProxyService Instance { get; } = new();

        private ProxyService()
        {
            //Rxでやろうとしたときの名残
            //SessionCompleted = Observable.FromEvent<SessionEventArgs>(
            //    h => proxyServer.BeforeResponse += async (s, e) => await Task.Run(() => h(e)),
            //    h => proxyServer.BeforeResponse -= async (s, e) => await Task.Run(() => h(e))
            //    );

            proxyServer.CertificateManager.RootCertificateIssuerName = "Mist Train Girls Thief";
            proxyServer.CertificateManager.RootCertificateName = "Mist Train Girls Thief";

            //外部に公開するならこっち
            //var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Loopback, Port, true);
            //今回はミストレと同じPCで動かすのでこっち
            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Loopback, Port, true);

            proxyServer.AddEndPoint(explicitEndPoint);

            mtgThief = new MTGThief(this);

            //Debug
            //proxyServer.BeforeRequest += async (s, e) => Logger.Instance.AddLog($"received: {e.HttpClient.Request.RequestUri}");

            //イベント登録
            //ラムダ式で書いちゃったけど別関数にしたほうがいいと思う
            proxyServer.BeforeResponse += async (s, e) =>
            {
                try
                {
                    if (e.HttpClient.Request.RequestUri.Host == "assets.mist-train-girls.com")
                    {
                        mtgThief.AssetReceived(e.HttpClient.Request.RequestUri.PathAndQuery, await e.GetResponseBody());
                        //mtgThief.AssetReceived(e.HttpClient.Request.RequestUri.PathAndQuery, await e.GetResponseBody().Timeout(TimeSpan.FromSeconds(1)));
                    }
                    else if (e.HttpClient.Request.RequestUri.AbsoluteUri.StartsWith("https://mist-train-girls.azurefd.net/api/"))
                    {
                        mtgThief.ApiReceived(e.HttpClient.Request.RequestUri.PathAndQuery, await e.GetResponseBody());
                        //mtgThief.ApiReceived(e.HttpClient.Request.RequestUri.PathAndQuery, await e.GetResponseBody().Timeout(TimeSpan.FromSeconds(1)));
                    }
                    else if (e.HttpClient.Request.RequestUri.AbsoluteUri.StartsWith("https://app-misttrain-prod-001.azurewebsites.net/api/"))
                    {
                        mtgThief.ApiReceived(e.HttpClient.Request.RequestUri.PathAndQuery, await e.GetResponseBody());
                    }
                }
                catch (Exception) { }
            };
        }

        #endregion

        /// <summary>
        /// プロキシ設定情報テキストの通知
        /// </summary>
        public IObservable<string> ProxyInfoUpdated => ProxyInfoUpdatedSubject.AsObservable();

        private Subject<string> ProxyInfoUpdatedSubject = new Subject<string>();

        private ProxyServer proxyServer = new ProxyServer();

        //Rxでやろうとしたときの名残
        //メモリリークなのかなんなのか、しばらくすると通信が止まってしまうので諦めた
        //public IObservable<SessionEventArgs> SessionCompleted;
        //HttpClient.Request
        // RequestUri: System.Uri
        //  AbsolutePath: /webhp
        //  AbsoluteUri: https://www.google.co.jp/webhp?hl=ja
        //  Host: www.google.co.jp
        //  PathAndQuery: /webhp?hl=ja
        //  ToString: https://www.google.co.jp/webhp?hl=ja
        // Host: www.google.co.jp
        //

        /// <summary>
        /// プロキシを起動
        /// どうせ時間かかるだろうと思ってAsync
        /// </summary>
        /// <returns></returns>
        public Task StartProxyAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!proxyServer.ProxyRunning)
                    {
                        proxyServer.Start();
                        var endPoint = proxyServer.ProxyEndPoints.FirstOrDefault();
                        if (endPoint != null)
                        {
                            var proxyInfo = $"{endPoint.IpAddress}:{endPoint.Port}";
                            ProxyInfoUpdatedSubject.OnNext(proxyInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = $"{ex.GetType()}: {ex.Message}";
                    Logger.Instance.AddLog(errorMessage);
                }
            });
        }

        /// <summary>
        /// プロキシを停止
        /// あんまり時間かからないだろうけどStartに合わせてAsync
        /// </summary>
        /// <returns></returns>
        public Task StopProxyAsync()
        {
            return Task.Run(() =>
            {
                if (proxyServer.ProxyRunning)
                {
                    proxyServer.Stop();
                }
            });
        }
    }
}
