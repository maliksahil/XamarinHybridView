using System;
using Xamarin.Forms;

namespace CustomRenderer
{
    public partial class HybridWebViewPage : ContentPage
    {
        public HybridWebViewPage()
        {
            InitializeComponent();

            hybridWebView.RegisterAction(functionParameters =>
            {
                string[] inputs = functionParameters.Input.Split(',');
                switch (inputs[0])
                {
                    case "findPrimesNative":
                        functionParameters.Output = calcPrimes(10000000).ToString();
                        break;
                    case "IsPrime":
                        functionParameters.Output = IsPrime(Convert.ToInt32(inputs[1])).ToString();
                        break;
                    default:
                        break;
                }
            });

        }
        private int calcPrimes(int max)
        {
            int numPrimes = 0;
            DateTime timeNow = DateTime.Now;
            for (int i = 2; i <= max; i++)
            {
                if (IsPrime(i))
                {
                    numPrimes++;
                }
            }
            DateTime timeEnd = DateTime.Now;
            return (int)(timeEnd - timeNow).TotalMilliseconds;
        }

        private bool IsPrime(int number)
        {
            if (number < 2) return false;
            var q = Math.Floor(Math.Sqrt(number));
            for (var i = 2; i <= q; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
