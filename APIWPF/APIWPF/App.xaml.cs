using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace APIWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly HttpClient httpClient = new HttpClient();

        public App() : base()
        {
            Exit += App_Exit;
        }


        private void App_Exit(object sender, ExitEventArgs e)
        {
            httpClient.Dispose();
        }
    }
}
