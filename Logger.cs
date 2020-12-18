using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace MistTrainGirlsThief
{
    /// <summary>
    /// ロガー（通知するだけ）　DIめんどくさかったのでシングルトンでいいやとなった
    /// </summary>
    public class Logger
    {
        #region Singleton

        public static Logger Instance { get; } = new();

        private Logger() { }

        #endregion

        /// <summary>
        /// ログが追加されたら通知
        /// </summary>
        public IObservable<string> LogAdded => LogAddedSubject.AsObservable();

        private Subject<string> LogAddedSubject = new();

        /// <summary>
        /// ログを追加
        /// </summary>
        /// <param name="log"></param>
        public void AddLog(string log)
        {
            LogAddedSubject.OnNext(log);
        }
    }
}
