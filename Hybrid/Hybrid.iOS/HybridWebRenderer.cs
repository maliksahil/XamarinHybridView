using System.IO;
using CustomRenderer;
using Hybrid.iOS;
using Foundation;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace Hybrid.iOS
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>, IWKScriptMessageHandler
    {
        const string JavaScriptFunction = @"
            isXamarin = true;
            isCordova = false;
            window.history.replaceState = function (data, title, url) {
                console.log(data);
                console.log(title);
                console.log(url);
            };
            window.history.pushState = function (data, title, url) {
                console.log(data);
                console.log(title);
                console.log(url);
            };
            window.id = 1;
            window.handlers = {};
            window.Resolver = function () {
                this.resolve = function () { },
                this.reject = function () { },
                this.resolveMessage = function (data) {
                    this.resolve(data);
                }
            };

            function invokeXamarinAction(data) {
                return new Promise((resolve, reject) => {
                    var handle = 'm' + this.id++;
                    window.handlers[handle] = new Resolver();
                    window.handlers[handle].resolve = resolve;
                    window.handlers[handle].reject = reject;
                    window.webkit.messageHandlers.invokeAction.postMessage({ data: data, id: handle });
                });
            }
    ";
        WKUserContentController userController;

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                userController = new WKUserContentController();
                var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
                userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, "invokeAction");

                var config = new WKWebViewConfiguration { UserContentController = userController };
                var webView = new WKWebView(Frame, config);
                SetNativeControl(webView);
            }
            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler("invokeAction");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null)
            {
                string fileName = Path.Combine(NSBundle.MainBundle.BundlePath, string.Format("Content/{0}", Element.Uri));
                Control.LoadRequest(new NSUrlRequest(new NSUrl(fileName, false)));
            }
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            var data = message.Body.ValueForKey(new NSString("data")).ToString();
            var handler = message.Body.ValueForKey(new NSString("id")).ToString();
            Element.InvokeAction(message.Body.ToString());
            var returnValue = Element.InvokeAction(data);
            if (returnValue == null) returnValue = "";
            returnValue = returnValue.Replace("\r", "").Replace("\n", "");
            Control.EvaluateJavaScript("window.handlers['" + handler + "'].resolveMessage('" + returnValue + "');", new WKJavascriptEvaluationResult((o, e) => { }));
        }
    }
}