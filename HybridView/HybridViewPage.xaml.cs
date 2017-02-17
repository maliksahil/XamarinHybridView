using Xamarin.Forms;

namespace HybridView
{
	public partial class HybridViewPage : ContentPage
	{
		public HybridViewPage()
		{
			InitializeComponent();
			hybridWebView.RegisterAction(data => DisplayAlert("Alert", Device.OS.ToString(), "OK"));
		}
	}
}
