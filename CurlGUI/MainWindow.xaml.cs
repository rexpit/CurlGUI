using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurlGUI
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// URL 入力テキストボックス Enter キー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextUrl_KeyDown(object sender, KeyEventArgs e)
        {
            // Enter キー押下
            if (e.Key == Key.Enter)
            {
                MakeAndExcecuteCurlCommand();
            }
        }

        /// <summary>
        /// 「保存」ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            MakeAndExcecuteCurlCommand();
        }

        /// <summary>
        /// 「保存」ボタン押下時処理
        /// </summary>
        private void MakeAndExcecuteCurlCommand()
        {
            // エラーチェック
            if (this.HasError())
            {
                return;
            }

            // パラメータ設定
            string url = this.TextUrl.Text;
            bool optionR = this.ChkOptionR.IsChecked.HasValue ? this.ChkOptionR.IsChecked.Value : false;
            bool optionE = this.ChkOptionE.IsChecked.HasValue ? this.ChkOptionE.IsChecked.Value : false;
            string referer = this.TextReferer.Text;
            bool optionA = this.ChkOptionA.IsChecked.HasValue ? this.ChkOptionA.IsChecked.Value : false;
            string userAgent = this.TextUserAgent.Text;

            // 保存先取得
            string savePath = this.GetSaveFilePath(url);
            if (string.IsNullOrEmpty(savePath))
            {
                return;
            }

            // コマンド生成
            var sbCmd = new StringBuilder();
            sbCmd.AppendFormat("{0}{1}", Environment.GetEnvironmentVariable("SystemRoot"), @"\system32\curl.exe");
            sbCmd.AppendFormat(@" ""{0}""", url);
            if (optionR)
            {
                sbCmd.Append(" -R");
            }
            if (optionE)
            {
                sbCmd.AppendFormat(@" -e ""{0}""", referer);
            }
            if (optionA)
            {
                sbCmd.AppendFormat(@" -A ""{0}""", userAgent);
            }
            sbCmd.AppendFormat(@" -o ""{1}""", url, savePath);

            this.TextStdout.Text += sbCmd.ToString() + "\n";

            // コマンド実行
            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = Environment.GetEnvironmentVariable("ComSpec"),
                Arguments = string.Format("/c {0}", sbCmd.ToString()),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }))
            {
                string stdout = process.StandardOutput.ReadToEnd();
                string stderr = process.StandardError.ReadToEnd();
                process.WaitForExit();
                this.TextStdout.Text += stdout;
                this.TextStdout.Text += stderr;
            }
        }

        /// <summary>
        /// エラーチェックを行います。
        /// </summary>
        /// <returns></returns>
        bool HasError()
        {
            if (!IsUrl(this.TextUrl.Text))
            {
                MessageBox.Show("URL を入力してください。", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return true;
            }
            if (this.ChkOptionE.IsChecked.HasValue && this.ChkOptionE.IsChecked.Value && !IsUrl(this.TextReferer.Text))
            {
                MessageBox.Show("Referer の URL を入力してください。", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return true;
            }
            return false;
        }

        /// <summary>
        /// ファイル保存ダイアログからファイル保存先を指定します。
        /// </summary>
        /// <returns>ファイルパス (失敗時は空文字)</returns>
        string GetSaveFilePath(string url = "")
        {
            string initialFileName = null;
            if (!string.IsNullOrEmpty(url))
            {
                Uri u = new Uri(url);
                if (u.Segments != null && u.Segments.Any())
                {
                    initialFileName = u.Segments.Last();
                }
            }
            var sfd = new SaveFileDialog
            {
                Filter = "すべてのファイル|*.*|画像ファイル|*.png;*.jpg;*.gif|JPEG|*.jpg|PNG|*.png|GIF|*.gif",
                FileName = initialFileName
            };
            bool? result = sfd.ShowDialog(this);
            return (result.HasValue && result.Value)
                ? sfd.FileName
                : string.Empty;
        }

        /// <summary>
        /// 指定された文字列が URL かどうか判定します。
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns>true: URL, false: URLでない</returns>
        public static bool IsUrl(string str)
        {
            return string.IsNullOrEmpty(str)
                ? false
                : Regex.IsMatch(str, @"(https?|ftp)(:\/\/[-_.!~*\'()a-zA-Z0-9;\/?:\@&=+\$,%#]+)");
        }
    }
}
