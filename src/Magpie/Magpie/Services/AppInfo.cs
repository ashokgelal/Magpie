namespace Magpie.Services
{
    public class AppInfo
    {
        public string AppIconPath { get; set; }

        public void SetAppIcon(string imageNamespace, string imagePath)
        {
            AppIconPath = string.Format("pack://application:,,,/{0};component/{1}", imageNamespace, imagePath);
        }
    }
}