using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using Coding4Fun.Toolkit.Controls;
using MicroMsg.sdk;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using neihanshe.common;

namespace neihanshe
{
    public partial class ItemPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public const string APPID = "wxd930ea5d5a258f4f";//sdk_demo的appid
        public static ArticleItem StaticArticleItem;
        private ArticleItem _articleItem;

        public ItemPage()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        public ArticleItem ArticleItem
        {
            get { return _articleItem; }
            set
            {
                _articleItem = value;
                NotifyPropertyChanged("ArticleItem");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ArticleItem = StaticArticleItem;
            string html = string.Format("<html><body><img width=\"100%\" src=\"{0}\"></body></html>", ArticleItem.PicUrl);

            ImageBrowser.NavigateToString(html);
            //ImageBrowser.Navigate(new Uri(ArticleItem.PicUrl, UriKind.Absolute));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region 微信接口
        private void ShareWeiXin_btnClick(object sender, EventArgs e)
        {

            string AppID = APPID;
            int scene = SendMessageToWX.Req.WXSceneChooseByUser;//发送到朋友圈
            WXBaseMessage message = null;
            WXImageMessage msg = new WXImageMessage();
            msg.Title = "来自内涵社的图片";
            msg.Description = ArticleItem.Title;
            msg.ThumbData = readRes(100, 200, 85);
            //图片的byte[]数据
            msg.ImageData = readRes();

            //图片的网络链接，ImageUrl和ImageData二者取其一，不要同时都填值

            message = msg;

            SendMessageToWX.Req req = new SendMessageToWX.Req(message, scene);
            IWXAPI api = WXAPIFactory.CreateWXAPI(AppID);

            api.SendReq(req);
        }

        #endregion

        #region Utils

        //工具函数，读取res资源成byte数组
        private byte[] readRes()
        {
            BitmapImage imageSource = ImgZoom.Source as BitmapImage;

            using (MemoryStream ms = new MemoryStream())
            {
                WriteableBitmap btmMap = new WriteableBitmap(imageSource);
                // write an image into the stream
                Extensions.SaveJpeg(btmMap, ms, imageSource.PixelWidth, imageSource.PixelHeight, 0, 100);

                return ms.ToArray();
            }
        }
        private byte[] readRes(int w, int h, int q)
        {
            BitmapImage imageSource = ImgZoom.Source as BitmapImage;

            using (MemoryStream ms = new MemoryStream())
            {

                WriteableBitmap btmMap = new WriteableBitmap(imageSource);
                // write an image into the stream
                Extensions.SaveJpeg(btmMap, ms, w, h, 0, q);

                return ms.ToArray();
            }
        }
        #endregion

        private void Save_btnClick(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                MediaLibrary library = new MediaLibrary();
                WriteableBitmap bitmap = new WriteableBitmap(ImgZoom.Source as BitmapImage);
                Extensions.SaveJpeg(bitmap, ms, bitmap.PixelWidth, bitmap.PixelHeight, 0, 100);
                ms.Seek(0, SeekOrigin.Begin);
                ms.Seek(0, SeekOrigin.Current);
                library.SavePicture(Guid.NewGuid().ToString(), ms);
                ms.Close();
                ToastPrompt prompt = new ToastPrompt();
                prompt.Title = "恭喜！";
                prompt.Message = "图片保存成功！";
                prompt.Show();
            }
            catch
            {
                ToastPrompt prompt = new ToastPrompt();
                prompt.Title = "错误！";
                prompt.Message = "图片保存失败！";
                prompt.Show();
            }
            finally
            {
                ms.Close();
            }
        }

        private void WaitShareWeiXin_btnClick(object sender, EventArgs e)
        {
            MessageBox.Show("微信分享功能正在等待微信官方认证，认证通过后会马上加入进来，小伙伴们耐心等待呦！", "温馨提示：", MessageBoxButton.OK);
        }

    }
}