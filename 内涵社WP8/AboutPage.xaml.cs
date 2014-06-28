using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace neihanshe
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void FeedbackButton_OnClick(object sender, RoutedEventArgs e)
        {
            var email = new EmailComposeTask();
            email.To = "gefangshuai@live.com";
            email.Subject = "关于“内涵社”的反馈信息";
            email.Show();
        }

        private void RateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }
    }
}