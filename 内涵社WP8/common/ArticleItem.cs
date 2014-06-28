using System;
using Newtonsoft.Json;

namespace neihanshe.common
{
    public class ArticleItem
    {
        private string _userInfo;

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("user_info")]
        public string UserInfo
        {
            get { return _userInfo; }
            set
            {
                if (value.Contains("</a>"))
                {
                    int a = value.IndexOf("@", StringComparison.Ordinal);
                    int b = value.LastIndexOf("</a>", StringComparison.Ordinal);
                    value = value.Substring(a + 1, b - a - 1);
                }
                _userInfo = value;
            }
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("pic_h")]
        public string PicH { get; set; }

        [JsonProperty("pic_url")]
        public string PicUrl { get; set; }

        [JsonProperty("video")]
        public string Video { get; set; }

        [JsonProperty("up")]
        public string Up { get; set; }

        [JsonProperty("dn")]
        public string Dn { get; set; }

        [JsonProperty("cmt")]
        public string Cmt { get; set; }

        [JsonProperty("q_num")]
        public string QNum { get; set; }

        [JsonProperty("t_num")]
        public string TNum { get; set; }

        [JsonProperty("s_num")]
        public string SNum { get; set; }

        [JsonProperty("r_num")]
        public string RNum { get; set; }

        public string PicW { get; set; }

        public override string ToString()
        {
            return
                string.Format(
                    "Cmt: {0}, Dn: {1}, Id: {2}, PicH: {3}, PicUrl: {4}, PicW: {5}, QNum: {6}, RNum: {7}, SNum: {8}, Title: {9}, TNum: {10}, Up: {11}, Uid: {12}, UserInfo: {13}, Video: {14}",
                    Cmt, Dn, Id, PicH, PicUrl, PicW, QNum, RNum, SNum, Title, TNum, Up, Uid, UserInfo, Video);
        }
    }
}