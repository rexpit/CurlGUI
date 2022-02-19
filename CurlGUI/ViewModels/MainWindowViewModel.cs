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
            this.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:97.0) Gecko/20100101 Firefox/97.0";
            this.Option_R = true;
            this.Option_e = false;
            this.Option_A = false;
            this.TextStdout = string.Empty;
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
        /// 謎
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 謎
        /// </summary>
        /// <param name="propertyName"></param>
        private void RaisePropertyChanged([CallerMemberName]string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
