using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurlGUI.ViewModels
{
    /// <summary>
    /// MainWindow の ViewModel
    /// </summary>
    public class MainWindowViewModel
    {
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
    }
}
