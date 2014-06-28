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
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

       
        # region 双指缩放

        // these two fields fully define the zoom state:


        private const double MaxImageZoom = 20;
        private Point _imagePosition = new Point(0, 0);
        private Point _oldFinger1;
        private Point _oldFinger2;
        private double _oldScaleFactor;
        private double _totalImageScale = 1d;

        #region Event handlers

        /// <summary>
        ///     Initializes the zooming operation
        /// </summary>
        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            _oldFinger1 = e.GetPosition(ImgZoom, 0);
            _oldFinger2 = e.GetPosition(ImgZoom, 1);
            _oldScaleFactor = 1;
        }

        /// <summary>
        ///     Computes the scaling and translation to correctly zoom around your fingers.
        /// </summary>
        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            double scaleFactor = e.DistanceRatio/_oldScaleFactor;
            if (!IsScaleValid(scaleFactor))
                return;

            Point currentFinger1 = e.GetPosition(ImgZoom, 0);
            Point currentFinger2 = e.GetPosition(ImgZoom, 1);

            Point translationDelta = GetTranslationDelta(
                currentFinger1,
                currentFinger2,
                _oldFinger1,
                _oldFinger2,
                _imagePosition,
                scaleFactor);

            _oldFinger1 = currentFinger1;
            _oldFinger2 = currentFinger2;
            _oldScaleFactor = e.DistanceRatio;

            UpdateImageScale(scaleFactor);
            UpdateImagePosition(translationDelta);
        }

        /// <summary>
        ///     Moves the image around following your finger.
        /// </summary>
        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            var translationDelta = new Point(e.HorizontalChange, e.VerticalChange);

            if (IsDragValid(1, translationDelta))
                UpdateImagePosition(translationDelta);
        }

        /// <summary>
        ///     Resets the image scaling and position
        /// </summary>
        private void OnDoubleTap(object sender, GestureEventArgs e)
        {
            ResetImagePosition();
        }

        #endregion

        #region Utils

        /// <summary>
        ///     Computes the translation needed to keep the image centered between your fingers.
        /// </summary>
        private Point GetTranslationDelta(
            Point currentFinger1, Point currentFinger2,
            Point oldFinger1, Point oldFinger2,
            Point currentPosition, double scaleFactor)
        {
            var newPos1 = new Point(
                currentFinger1.X + (currentPosition.X - oldFinger1.X)*scaleFactor,
                currentFinger1.Y + (currentPosition.Y - oldFinger1.Y)*scaleFactor);

            var newPos2 = new Point(
                currentFinger2.X + (currentPosition.X - oldFinger2.X)*scaleFactor,
                currentFinger2.Y + (currentPosition.Y - oldFinger2.Y)*scaleFactor);

            var newPos = new Point(
                (newPos1.X + newPos2.X)/2,
                (newPos1.Y + newPos2.Y)/2);

            return new Point(
                newPos.X - currentPosition.X,
                newPos.Y - currentPosition.Y);
        }

        /// <summary>
        ///     Updates the scaling factor by multiplying the delta.
        /// </summary>
        private void UpdateImageScale(double scaleFactor)
        {
            _totalImageScale *= scaleFactor;
            ApplyScale();
        }

        /// <summary>
        ///     Applies the computed scale to the image control.
        /// </summary>
        private void ApplyScale()
        {
            ((CompositeTransform) ImgZoom.RenderTransform).ScaleX = _totalImageScale;
            ((CompositeTransform) ImgZoom.RenderTransform).ScaleY = _totalImageScale;
        }

        /// <summary>
        ///     Updates the image position by applying the delta.
        ///     Checks that the image does not leave empty space around its edges.
        /// </summary>
        private void UpdateImagePosition(Point delta)
        {
            var newPosition = new Point(_imagePosition.X + delta.X, _imagePosition.Y + delta.Y);

            if (newPosition.X > 0) newPosition.X = 0;
            if (newPosition.Y > 0) newPosition.Y = 0;

            if ((ImgZoom.ActualWidth*_totalImageScale) + newPosition.X < ImgZoom.ActualWidth)
                newPosition.X = ImgZoom.ActualWidth - (ImgZoom.ActualWidth*_totalImageScale);

            if ((ImgZoom.ActualHeight*_totalImageScale) + newPosition.Y < ImgZoom.ActualHeight)
                newPosition.Y = ImgZoom.ActualHeight - (ImgZoom.ActualHeight*_totalImageScale);

            _imagePosition = newPosition;

            ApplyPosition();
        }

        /// <summary>
        ///     Applies the computed position to the image control.
        /// </summary>
        private void ApplyPosition()
        {
            ((CompositeTransform) ImgZoom.RenderTransform).TranslateX = _imagePosition.X;
            ((CompositeTransform) ImgZoom.RenderTransform).TranslateY = _imagePosition.Y;
        }

        /// <summary>
        ///     Resets the zoom to its original scale and position
        /// </summary>
        private void ResetImagePosition()
        {
            _totalImageScale = 1;
            _imagePosition = new Point(0, 0);
            ApplyScale();
            ApplyPosition();
        }

        /// <summary>
        ///     Checks that dragging by the given amount won't result in empty space around the image
        /// </summary>
        private bool IsDragValid(double scaleDelta, Point translateDelta)
        {
            if (_imagePosition.X + translateDelta.X > 0 || _imagePosition.Y + translateDelta.Y > 0)
                return false;

            if ((ImgZoom.ActualWidth*_totalImageScale*scaleDelta) + (_imagePosition.X + translateDelta.X) <
                ImgZoom.ActualWidth)
                return false;

            if ((ImgZoom.ActualHeight*_totalImageScale*scaleDelta) + (_imagePosition.Y + translateDelta.Y) <
                ImgZoom.ActualHeight)
                return false;

            return true;
        }

        /// <summary>
        ///     Tells if the scaling is inside the desired range
        /// </summary>
        private bool IsScaleValid(double scaleDelta)
        {
            return (_totalImageScale*scaleDelta >= 1) && (_totalImageScale*scaleDelta <= MaxImageZoom);
        }

        #endregion

       

        #endregion
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
        //工具函数，读取res资源成byte数组
        private byte[] readRes()
        {
            BitmapImage imageSource = ImgZoom.Source as BitmapImage;

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    WriteableBitmap btmMap = new WriteableBitmap(imageSource);
                    // write an image into the stream
                    Extensions.SaveJpeg(btmMap, ms, imageSource.PixelWidth, imageSource.PixelHeight, 0, 100);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return ms.ToArray();
            }
        }
        private byte[] readRes(int w, int h, int q)
        {
            BitmapImage imageSource = ImgZoom.Source as BitmapImage;

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    WriteableBitmap btmMap = new WriteableBitmap(imageSource);
                    // write an image into the stream
                    Extensions.SaveJpeg(btmMap, ms, w, h, 0, q);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return ms.ToArray();
            }
        }
        private void Save_btnClick(object sender, EventArgs e)
        {
            var libary = new MediaLibrary();
            libary.SavePicture(Guid.NewGuid() + ".png", readRes());
            ToastPrompt toast = new ToastPrompt();
            toast.Title = "恭喜：";
            toast.Message = "图像保存成功！";
            toast.Show();
        }
    }
}