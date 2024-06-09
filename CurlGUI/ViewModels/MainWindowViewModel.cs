using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CurlGUI.ViewModels
{
    /// <summary>
    /// MainWindow の ViewModel
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.Url = @"https://";
            this.Referer = @"https://www.pixiv.net";
            this.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:126.0) Gecko/20100101 Firefox/126.0";
            this.Option_R = true;
            this.Option_L = true;
            this.Option_k = false;
            this.Option_e = false;
            this.Option_A = false;
            this.TextStdout = string.Empty;
            this.AreButtonsEnabled = true;
        }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Referer
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        /// UserAgent
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// -R オプションが有効か？
        /// </summary>
        public bool Option_R { get; set; }

        /// <summary>
        /// -L オプションが有効か？
        /// </summary>
        public bool Option_L { get; set; }

        /// <summary>
        /// -k オプションが有効か？
        /// </summary>
        public bool Option_k { get; set; }

        /// <summary>
        /// -e オプションが有効か？
        /// </summary>
        public bool Option_e { get; set; }

        /// <summary>
        /// -A オプションが有効か？
        /// </summary>
        public bool Option_A { get; set; }

        /// <summary>
        /// 標準出力
        /// </summary>
        public string TextStdout
        {
            get { return this._textStdout; }
            set
            {
                this._textStdout = value;
                // 変更を View に反映
                RaisePropertyChanged();
            }
        }
        private string _textStdout;

        /// <summary>
        /// ボタンを有効化するか？
        /// </summary>
        public bool AreButtonsEnabled {
            get { return this._areButtonsEnabled; }
            set
            {
                this._areButtonsEnabled = value;
                // 変更を View に反映
                RaisePropertyChanged();
            }
        }
        private bool _areButtonsEnabled;

        #region プロパティ変更通知イベント
        /// <summary>
        /// プロパティ変更通知イベント
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティ変更通知イベントを発生させます。
        /// 変更を View に反映します。
        /// </summary>
        /// <param name="propertyName"></param>
        private void RaisePropertyChanged([CallerMemberName]string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
