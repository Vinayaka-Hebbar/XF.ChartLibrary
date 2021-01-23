﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XF.ChartLibrary.Data
{
#if __IOS__ || __TVOS__
    using NSUIImage = UIKit.UIImage;
#endif
    public partial class Entry 
    {
        public Entry(double x, double y, NSUIImage icon) : base(y)
        {
            X = x;
            Icon = icon;
        }

        public Entry(double x, double y, NSUIImage icon, object data) : base(y)
        {
            X = x;
            Icon = icon;
            Data = data;
        }
    }
}
