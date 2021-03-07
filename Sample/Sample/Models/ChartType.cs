using System;
using Xamarin.Forms;

namespace Sample.Models
{
    public class ChartType
    {
        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Instead of using <see cref="System.Activator"/> may be creating with new is faster
        /// </summary>
        public Func<Page> Activator { get; set; }
    }
}