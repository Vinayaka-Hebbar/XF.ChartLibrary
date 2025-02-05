﻿namespace XF.ChartLibrary.Data
{
#if __IOS__ || __TVOS__
    using NSUIImage = UIKit.UIImage;
#endif
    public partial class EntryBase
    {
        public EntryBase(float y, NSUIImage icon) : this(y)
        {
            Icon = icon;
        }

        public EntryBase(float y, NSUIImage icon, object data) : this(y)
        {
            Icon = icon;
            Data = data;
        }

        public NSUIImage Icon { get; set; }
    }
}
