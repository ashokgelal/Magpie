using System;

namespace MagpieUpdater.Services
{
    public class AppInfo
    {
        public string AppIconPath { get; set; }
        public string AppCastUrl { get; private set; }
        public string PublicSignatureFilename { get; set; }
        public int SubscribedChannel { get; set; }
        public bool DisableMagpieBranding { get; set; }
        public bool InteropWithWinForm { get; set; }

        public void SetAppIcon(string imageNamespace, string imagePath)
        {
            AppIconPath = string.Format("pack://application:,,,/{0};component/{1}", imageNamespace, imagePath);
        }

        public AppInfo(string appCastUrl, int subscribedChannel = 1)
        {
            if (string.IsNullOrWhiteSpace(appCastUrl))
            {
                throw new ArgumentNullException("appCastUrl");
            }
            AppCastUrl = appCastUrl;
            SubscribedChannel = subscribedChannel;
            PublicSignatureFilename = SignatureVerifier.DefaultDSAPubKeyFileName;
        }
    }
}