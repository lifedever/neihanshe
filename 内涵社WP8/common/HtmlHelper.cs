using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using Coding4Fun.Toolkit.Controls;

namespace Wincn
{
    public abstract class HtmlHelper
    {
        private Dispatcher simpleDispatcher; // 刷新UI用


        public static string HtmlString { get; set; }

        public void HttpGet(string url)
        {
            simpleDispatcher = Deployment.Current.Dispatcher;
            //创建WebRequest类
            HttpWebRequest request = WebRequest.CreateHttp(new Uri(url));
            //设置请求方式GET POST
            request.Method = "GET";
            //返回应答请求异步操作的状态                
            request.BeginGetResponse(ResponseCallback, request);
        }

        private void ResponseCallback(IAsyncResult result)
        {
            try
            {
                //获取异步操作返回的的信息
                var request = (HttpWebRequest) result.AsyncState;
                //结束对 Internet 资源的异步请求
                var response = (HttpWebResponse) request.EndGetResponse(result);
                //解析应答头
                //parseRecvHeader(response.Headers);
                //获取请求体信息长度
                long contentLength = response.ContentLength;

                //获取应答码
                var statusCode = (int) response.StatusCode;
                string statusText = response.StatusDescription;

                //应答头信息验证
                using (Stream stream = response.GetResponseStream())
                {
                    //获取请求信息
                    var read = new StreamReader(stream);
                    HtmlString = read.ReadToEnd();
                    HtmlHandler(HtmlString);
                }
            }
            catch (WebException e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var prompt = new ToastPrompt();
                    prompt.Title = "错误：";
                    prompt.Message = "网络连接失败！";
                    prompt.Show();
                });
            }
        }

        /// <summary>
        ///     获取html后处理(需要实现)
        /// </summary>
        /// <param name="html"></param>
        protected abstract void HtmlHandler(string html);
    }
}