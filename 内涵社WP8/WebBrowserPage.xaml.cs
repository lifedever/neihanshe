using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;

namespace neihanshe
{
    public partial class WebBrowserPage : PhoneApplicationPage
    {
        public WebBrowserPage()
        {
            InitializeComponent();
        }

        private void WebBrowserPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            IndexNav();
            HotNav();
            NewNav();
        }

        private void NewNav()
        {
            WebProgressBar.Visibility = Visibility.Visible;
            NewBrowser.Navigate(new Uri("http://m.neihanshe.cn/new", UriKind.Absolute));
        }

        private void HotNav()
        {
            WebProgressBar.Visibility = Visibility.Visible;
            HotBrowser.Navigate(new Uri("http://m.neihanshe.cn/hot", UriKind.Absolute));
        }

        private void IndexNav()
        {
            WebProgressBar.Visibility = Visibility.Visible;
            IndexBrowser.Navigate(new Uri("http://m.neihanshe.cn/", UriKind.Absolute));
        }

        private void Browser_OnNavigating(object sender, NavigatingEventArgs e)
        {
            WebProgressBar.Visibility = Visibility.Visible;
        }

        private void Browser_OnNavigated(object sender, NavigationEventArgs e)
        {
            WebProgressBar.Visibility = Visibility.Collapsed;
        }

        private void Browser_OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            var prompt = new ToastPrompt {Title = "错误：", Message = "网络连接失败，请稍重试！"};
            prompt.Show();
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (GetCurrentWebBrowser().CanGoBack)
            {
                e.Cancel = true;
                GetCurrentWebBrowser().GoBack();
            }
        }

        private WebBrowser GetCurrentWebBrowser()
        {
            WebBrowser browser = IndexBrowser;
            int index = MyPivot.SelectedIndex;
            switch (index)
            {
                case 0:
                    browser = IndexBrowser;
                    break;
                case 1:
                    browser = HotBrowser;
                    break;
                case 2:
                    browser = NewBrowser;
                    break;
            }
            return browser;
        }


        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            int index = MyPivot.SelectedIndex;
            switch (index)
            {
                case 0:
                    IndexNav();
                    break;
                case 1:
                    HotNav();
                    break;
                case 2:
                    NewNav();
                    break;
            }
        }

        private void ForwardApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            GetCurrentWebBrowser().GoForward();
        }

        private void ApplicationBarMenuItem_OnClick(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BackApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            Boolean calcleNotify = false;
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("notifyBack"))
            {
                calcleNotify = (Boolean) settings["notifyBack"];
            }
            else
            {
                settings.Add("notifyBack", false);
            }
            if (!calcleNotify)
            {
                var checkBox = new CheckBox {Content = "下次不再提示"};
                var custom = new CustomMessageBox
                {
                    Height = 280,
                    Content = checkBox,
                    Message = "如果您的手机有实体返回键，您可以直接操作实体键。",
                    Title = "温馨提示：",
                    IsLeftButtonEnabled = true,
                    LeftButtonContent = "确定"
                };
                checkBox.Click += (o, args) => { settings["notifyBack"] = checkBox.IsChecked; };
                custom.Show();
            }
        }
    }
}