using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InstagramDemo.Data
{
    public class MultiResImageChooser
    {
        public Uri BestResolutionImage
        {
            get
            {
                switch (ResolutionHelper.CurrentResolution)
                {
                    case Resolutions.HD:
                        return new Uri("/Assets/SplashScreen/1080x1920.png", UriKind.Relative);
                    case Resolutions.WXGA:
                        return new Uri("/Assets/SplashScreen/768x1280.png", UriKind.Relative);
                    case Resolutions.WVGA:
                        return new Uri("/Assets/SplashScreen/480x800.png", UriKind.Relative);
                    default:
                        return new Uri("/Assets/SplashScreen/720x1280.png", UriKind.Relative);
                }
            }
        }
       

    }
}
