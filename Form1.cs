using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reactive.Linq;

namespace MistTrainGirlsThief
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ProxyService.Instance.ProxyInfoUpdated
                .ObserveOn(System.Threading.SynchronizationContext.Current) //UIスレッドで購読
                .Subscribe(info =>
                {
                    infoBox.Text = $"Proxy: {info}";
                });

            Logger.Instance.LogAdded
                .ObserveOn(System.Threading.SynchronizationContext.Current) //UIスレッドで購読
                .Subscribe(logText =>
                {
                    logBox.AppendText(logText + "\r\n");
                });
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //フォームが表示されたら
            //ログ表示するところにフォーカスして
            ActiveControl = logBox;
            //プロキシ起動
            ProxyService.Instance.StartProxyAsync();
        }
    }
}
