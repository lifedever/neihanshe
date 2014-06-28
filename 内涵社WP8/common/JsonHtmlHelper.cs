using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using Wincn;

namespace neihanshe.common
{
    public class JsonHtmlHelper : HtmlHelper
    {
        private readonly double _width;

        public JsonHtmlHelper(ObservableCollection<ArticleItem> observable)
        {
            ObservableCollection = observable;
            _width = Application.Current.Host.Content.ActualWidth;
        }

        public ObservableCollection<ArticleItem> ObservableCollection { get; set; }

        protected override void HtmlHandler(string html)
        {
            try
            {
                if (html != null && ObservableCollection != null)
                {
                    html = html.Remove(1, html.IndexOf(",", StringComparison.Ordinal));
                    var items = JsonConvert.DeserializeObject<List<ArticleItem>>(html);
                    if (items != null && items.Count > 0)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            ObservableCollection.Clear();
                            ;
                            foreach (ArticleItem item in items)
                            {
                                if (item.Id != null)
                                {
                                    item.PicW = (_width - 21).ToString();
                                    ObservableCollection.Add(item);
                                }
                            }
                            if (SystemTray.ProgressIndicator != null)
                            {
                                SystemTray.ProgressIndicator.IsVisible = false;
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var toast = new ToastPrompt {Title = "错误:", Message = "无数据返回,请稍后重试！"};
                    toast.Show();
                });
                MainPage.page--;
            }
        }
    }
}