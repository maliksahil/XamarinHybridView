using System;
using Xamarin.Forms;

namespace CustomRenderer
{
    public class HybridWebView : View
    {
        Action<FunctionParameters> action;

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: "Uri",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public void RegisterAction(Action<FunctionParameters> callback)
        {
            action = callback;
        }

        public void Cleanup()
        {
            action = null;
        }

        public string InvokeAction(string data)
        {
            if (action == null || data == null)
            {
                return "";
            }
            var parameters = new FunctionParameters() { Input = data };
            action.Invoke(parameters);
            return parameters.Output;
        }
    }

    public class FunctionParameters
    {
        public string Input { get; set; }
        public string Output { get; set; }
    }
}
