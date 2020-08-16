using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HueController
{
    class ControlItem
    {

        public bool? Powered { get; set; } = null;

        public Color CSharpColor { 
            set 
            {
                Color = new RGBColor(value.R.ToString("X2") + value.G.ToString("X2") + value.B.ToString("X2"));
            } 
        }

        public RGBColor? Color { get; set; } = null;

        public Alert? Alert { get; set; } = null;

        public Effect? Effect { get; set; } = null;

    }
}
