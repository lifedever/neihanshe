﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using neihanshe.common;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace neihanshe
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        private static string type = TypeUtils.HOT;
        public static int page = 1;
        private static string postUrl = "http://neihanshe.cn/apps/get_json.php?class={0}&page={1}";

        public bool IsLoading = false;
        public ScrollViewer ListScrollViewer;
        private ObservableCollection<ArticleItem> _articleItems;
        private bool _slided;
        private bool isCmt;
        private bool isExit;
        private bool isFull;

        // 构造函数
        public MainPage()
        {
            try
            {
                ArticleItems = new ObservableCollection<ArticleItem>();
                InitializeComponent();
                MyLongListSelector.DataContext = this;
                MenuListBox.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public ObservableCollection<ArticleItem> ArticleItems
        {
            get { return _articleItems; }
            set
            {
                _articleItems = value;
                NotifyPropertyChanged("ArticleItems");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // 为 ViewModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void LoadArticles()
        {
            var pi = new ProgressIndicator();
            pi.Text = string.Format("加载第{0}页数据……", page);
            pi.IsIndeterminate = true;
            pi.IsVisible = true;
            SystemTray.SetProgressIndicator(this, pi);
            var helper = new JsonHtmlHelper(ArticleItems);
            helper.HttpGet(String.Format(postUrl, type, page));
            Debug.WriteLine(String.Format(postUrl, type, page));
        }

        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        private void SlideImage_OnTap(object sender, GestureEventArgs e)
        {
            try
            {
                if (_slided)
                {
                    UnSlide();
                }
                else
                {
                    Slide();
                }
            }
            catch (Exception exception)
            {
                var toast = new ToastPrompt();
                toast.Message = exception.Message;
                toast.Title = "错误";
                toast.Show();
            }
        }

        /// <summary>
        ///     滑动
        /// </summary>
        private void Slide()
        {
            MyLongListSelector.IsEnabled = false;
            LayoutBack.Visibility = Visibility.Visible;
            SlideStoryboardBegin.Begin();
            _slided = true;
        }

        /// <summary>
        ///     恢复滑动
        /// </summary>
        private void UnSlide()
        {
            MyLongListSelector.IsEnabled = true;
            SlideStoryboardEnd.Begin();
            _slided = false;
        }


        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (isCmt)
            {
                e.Cancel = true;
                UnSildeCmt();

                return;
            }

            if (_slided)
            {
                if (!isExit)
                {
                    isExit = true;
                    var toast = new ToastPrompt {Message = "再按一次退出程序"};
                    toast.Completed += (o, ex) => { isExit = false; };
                    toast.Show();
                    e.Cancel = true;
                }
                else
                {
                    NavigationService.RemoveBackEntry();
                }
            }
            else
            {
                Slide();
                e.Cancel = true;
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object item = MenuListBox.SelectedItem;
            if (item == null)
            {
                return;
            }
            int index = MenuListBox.SelectedIndex;
            switch (index)
            {
                case 0:
                    type = TypeUtils.HOT;
                    HotListBox.Visibility = Visibility.Visible;
                    CmtListBox.Visibility = Visibility.Collapsed;
                    NewListBox.Visibility = Visibility.Collapsed;
                    HotListBox.SelectedIndex = 0;
                    break;
                case 1:
                    type = TypeUtils.CMT_HOT;
                    HotListBox.Visibility = Visibility.Collapsed;
                    CmtListBox.Visibility = Visibility.Visible;
                    NewListBox.Visibility = Visibility.Collapsed;
                    CmtListBox.SelectedIndex = 0;

                    break;
                case 2:
                    type = TypeUtils.NEW;
                    HotListBox.Visibility = Visibility.Collapsed;
                    CmtListBox.Visibility = Visibility.Collapsed;
                    NewListBox.Visibility = Visibility.Visible;
                    NewListBox.SelectedIndex = 0;
                    break;
                default:
                    type = TypeUtils.HOT;
                    HotListBox.Visibility = Visibility.Visible;
                    CmtListBox.Visibility = Visibility.Collapsed;
                    NewListBox.Visibility = Visibility.Collapsed;
                    HotListBox.SelectedIndex = 0;
                    break;
            }
            for (int i = 0; i < 3; i++)
            {
                var listItem = (MenuListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem);

                if (listItem != null)
                {
                    if (i == index)
                    {
                        listItem.Background = new SolidColorBrush(Color.FromArgb(255, 30, 144, 255));
                    }
                    else
                    {
                        listItem.Background = null;
                    }
                }
            }
            UnSlide();
            page = 1;
            ArticleItems.Clear();
            LoadArticles();
        }

        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            ArticleItems.Clear();
            page = 1;
            LoadArticles();
        }

        private void HotListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object item = HotListBox.SelectedItem;
            if (item == null)
            {
                return;
            }
            int index = HotListBox.SelectedIndex;
            switch (index)
            {
                case 0:
                    type = TypeUtils.HOT;
                    break;
                case 1:
                    type = TypeUtils.WEEK;
                    break;
                case 2:
                    type = TypeUtils.MONTH;
                    break;
                default:
                    type = TypeUtils.HOT;
                    break;
            }
            for (int i = 0; i < 3; i++)
            {
                var listItem = (HotListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem);

                if (listItem != null)
                {
                    if (i == index)
                    {
                        listItem.Background = new SolidColorBrush(Color.FromArgb(255, 30, 144, 255));
                        listItem.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        listItem.Background = null;
                    }
                }
            }
            ArticleItems.Clear();
            page = 1;
            LoadArticles();
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            //MenuListBox.SelectedIndex = 0;
        }

        private void CmtListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object item = CmtListBox.SelectedItem;
            if (item == null)
            {
                return;
            }
            int index = CmtListBox.SelectedIndex;
            switch (index)
            {
                case 0:
                    type = TypeUtils.CMT_HOT;
                    break;
                case 1:
                    type = TypeUtils.CMT_WEEK;
                    break;
                case 2:
                    type = TypeUtils.CMT_MONTH;
                    break;
                default:
                    type = TypeUtils.CMT_HOT;
                    break;
            }
            for (int i = 0; i < 3; i++)
            {
                var listItem = (CmtListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem);

                if (listItem != null)
                {
                    if (i == index)
                    {
                        listItem.Background = new SolidColorBrush(Color.FromArgb(255, 30, 144, 255));
                        listItem.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        listItem.Background = null;
                    }
                }
            }
            ArticleItems.Clear();
            page = 1;
            LoadArticles();
        }

        private void NewListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object item = NewListBox.SelectedItem;
            if (item == null)
            {
                return;
            }
            int index = NewListBox.SelectedIndex;
            switch (index)
            {
                case 0:
                    type = TypeUtils.INDEX;
                    break;
                case 1:
                    type = TypeUtils.NEW;
                    break;

                default:
                    type = TypeUtils.INDEX;
                    break;
            }
            for (int i = 0; i < 3; i++)
            {
                var listItem = (NewListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem);

                if (listItem != null)
                {
                    if (i == index)
                    {
                        listItem.Background = new SolidColorBrush(Color.FromArgb(255, 30, 144, 255));
                        listItem.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        listItem.Background = null;
                    }
                }
            }
            ArticleItems.Clear();
            page = 1;
            LoadArticles();
        }

        private void BackIconButton_OnClick(object sender, EventArgs e)
        {
            page--;
            LoadArticles();
        }

        private void NextIconButton_OnClick(object sender, EventArgs e)
        {
            page++;
            LoadArticles();
            MyLongListSelector.SelectedIndex = 0;
        }

        private void FullSizeApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            var menuItem = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;

            if (isFull) //取消全屏
            {
                SystemTray.IsVisible = true;
                ApplicationBar.Mode = ApplicationBarMode.Default;
                MyLongListSelector.Margin = new Thickness(0, 0, 0, 70);
                menuItem.Text = "全屏模式";
            }
            else
            {
                SystemTray.IsVisible = false;
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
                MyLongListSelector.Margin = new Thickness(0, 0, 0, 20);

                menuItem.Text = "取消全屏";
            }
            isFull = !isFull;
        }

        /// <summary>
        ///     浏览器模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowserModeMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/WebBrowserPage.xaml", UriKind.Relative));
        }


        /// <summary>
        ///     点击查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UIElement_OnTap(object sender, GestureEventArgs e)
        {
            var item = MyLongListSelector.SelectedItem as ArticleItem;
            if (item == null)
            {
                return;
            }
            ItemPage.StaticArticleItem = item;
            NavigationService.Navigate(new Uri("/ItemPage.xaml", UriKind.Relative));
        }

        private void CMTUIElement_OnTap(object sender, GestureEventArgs e)
        {
            MyLongListSelector.IsEnabled = false;
            isCmt = true;
            var img = sender as Image;
            CmtBrowser.Navigate(new Uri(string.Format("http://m.neihanshe.cn/apps/comment.php?id={0}", img.Tag),
                UriKind.Absolute));
            CMTStoryboardBegin.Begin();
        }

        private void CloseCmtTap(object sender, GestureEventArgs e)
        {
            UnSildeCmt();
        }

        private void UnSildeCmt()
        {
            MyLongListSelector.IsEnabled = true;
            isCmt = false;
            CMTStoryboardEnd.Begin();
        }


     

        private void AboutBrowserModeMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}