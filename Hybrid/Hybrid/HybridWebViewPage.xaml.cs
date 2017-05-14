using Xamarin.Forms;

namespace Hybrid
{
	public partial class HybridWebViewPage : ContentPage
	{
		public HybridWebViewPage ()
		{
			InitializeComponent ();

			hybridWebView.RegisterAction (data => DisplayAlert ("Alert", "Hello " + data, "OK"));
		}
	}
}
