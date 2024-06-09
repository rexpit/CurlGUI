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
using System.Web;
using CurlGUI.Exceptions;
using CurlGUI.Models;
using CurlGUI.ViewModels;
using CurlGUI.Settings;

namespace CurlGUI
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Model
        /// </summary>
        private MainWindowModel Model { get; set; } = new MainWindowModel();

        /// <summary>
        /// 押されたボタンの種類
        /// </summary>
        private enum SelectedButton
        {
            Save,
            ShowHeaderInfo
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // 設定読み込み
            LoadSettings();
        }

        /// <summary>
        /// アプリ起動時設定を読み込む。
        /// </summary>
        private void LoadSettings()
        {
            // 読み込み機能は未実装
            var view = new MainWindowViewModel();
            this.DataContext = view;
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
                var view = this.DataContext as MainWindowViewModel;
                try
                {
                    // ボタンを無効化する。
                    view.AreButtonsEnabled = false;

                    // 各種ボタン押下時処理を実行する。
                    MakeAndExcecuteCurlCommand(SelectedButton.Save);

                }
                catch (ErrorMessageException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    // ボタンを有効化する。
                    view.AreButtonsEnabled = true;
                }
            }
        }

        /// <summary>
        /// 「保存」ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var view = this.DataContext as MainWindowViewModel;
            try
            {
                // ボタンを無効化する。
                view.AreButtonsEnabled = false;

                // 各種ボタン押下時処理を実行する。
                MakeAndExcecuteCurlCommand(SelectedButton.Save);
            }
            catch (ErrorMessageException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // ボタンを有効化する。
                view.AreButtonsEnabled = true;
            }
        }

        /// <summary>
        /// 「ヘッダー表示のみ」ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonShowHeader_Click(object sender, RoutedEventArgs e)
        {
            var view = this.DataContext as MainWindowViewModel;
            try
            {
                // ボタンを無効化する。
                view.AreButtonsEnabled = false;

                // 各種ボタン押下時処理を実行する。
                MakeAndExcecuteCurlCommand(SelectedButton.ShowHeaderInfo);
            }
            catch (ErrorMessageException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // ボタンを有効化する。
                view.AreButtonsEnabled = true;
            }
        }

        /// <summary>
        /// 各種ボタン押下時処理
        /// </summary>
        /// <param name="selectedButton">選択されたボタン</param>
        private void MakeAndExcecuteCurlCommand(SelectedButton selectedButton)
        {
            // エラーチェック
            if (this.HasError())
            {
                return;
            }

            // パラメータ設定
            var view = this.DataContext as MainWindowViewModel;
            string url = view.Url;
            bool option_R = view.Option_R;
            bool option_L = view.Option_L;
            bool option_k = view.Option_k;
            bool option_e = view.Option_e;
            string referer = view.Referer;
            bool option_A = view.Option_A;
            string userAgent = view.UserAgent;

            // 保存先取得
            string savePath = "";
            if (selectedButton == SelectedButton.Save)
            {
                savePath = this.GetSaveFilePath(url);
                if (string.IsNullOrEmpty(savePath))
                {
                    return;
                }
            }

            // コマンド生成
            var sbCmd = new StringBuilder();
            sbCmd.AppendFormat("{0}{1}", Environment.GetEnvironmentVariable("SystemRoot"), @"\system32\curl.exe");
            sbCmd.Append($@" ""{url}""");
            if (option_R)
            {
                sbCmd.Append(" -R");
            }
            if (option_L)
            {
                sbCmd.Append(" -L");
            }
            if (option_k)
            {
                sbCmd.Append(" -k");
            }
            if (option_e)
            {
                sbCmd.Append($@" -e ""{referer}""");
            }
            if (option_A)
            {
                sbCmd.Append($@" -A ""{userAgent}""");
            }
            // 選択されたボタンにより処理を分岐
            switch (selectedButton)
            {
                // 「保存」ボタン押下時は取得したファイル名を使用
                case SelectedButton.Save:
                    if (!string.IsNullOrEmpty(savePath))
                    {
                        sbCmd.Append($@" -o ""{savePath}""");
                    }
                    else
                    {
                        throw new ErrorMessageException("あり得ないパスです。ファイル名の指定がありません。");
                    }
                    break;

                // 「ヘッダー表示のみ」ボタン押下時
                case SelectedButton.ShowHeaderInfo:
                    sbCmd.Append(" -I");
                    break;

                default:
                    throw new ErrorMessageException("あり得ないパスです。switch (selectedButton) default case");
            }

            view.TextStdout += sbCmd.ToString() + "\n";

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
                view.TextStdout += stdout;
                view.TextStdout += stderr;
            }
        }

        /// <summary>
        /// エラーチェックを行います。
        /// </summary>
        /// <returns></returns>
        private bool HasError()
        {
            var view = this.DataContext as MainWindowViewModel;
            // URL チェック
            if (!IsUrl(view.Url))
            {
                throw new ErrorMessageException("URL を入力してください。");
            }
            // -e オプションが有効
            if (view.Option_e)
            {
                // URL チェック
                if (!IsUrl(view.Referer))
                {
                    throw new ErrorMessageException("Referer の URL を入力してください。");
                }
                // コマンドインジェクションチェック
                if (HasCommandInjection(view.Referer))
                {
                    throw new ErrorMessageException("Referer に使用不能な文字が含まれています。");
                }
            }
            // -A オプションが有効
            if (view.Option_A)
            {
                // コマンドインジェクションチェック
                if (HasCommandInjection(view.UserAgent))
                {
                    throw new ErrorMessageException("UserAgent に使用不能な文字が含まれています。");
                }
            }
            return false;
        }

        /// <summary>
        /// ファイル保存ダイアログからファイル保存先を指定します。
        /// </summary>
        /// <returns>ファイルパス (失敗時は空文字)</returns>
        private string GetSaveFilePath(string url = "")
        {
            string initialFileName = null;
            if (!string.IsNullOrEmpty(url))
            {
                Uri u = new Uri(url);
                if (u.Segments != null && u.Segments.Any())
                {
                    initialFileName = HttpUtility.UrlDecode(u.Segments.Last());
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

        /// <summary>
        /// 入力された文字列にコマンドインジェクションの可能性がないか判定します。
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns>true: 問題あり, false: 問題なし</returns>
        public static bool HasCommandInjection(string str)
        {
            return string.IsNullOrEmpty(str)
                ? false
                : Regex.IsMatch(str, @"[""\r\n]");
        }
    }
}
