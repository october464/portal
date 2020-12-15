using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Models.Charts
{
    public class ChartModel
    {
      
        public decimal Values { get; set; }
        public string Labels { get; set; }


    }

    public class CategoryChartModel
    {
        public decimal CategoryTransaction { get; set; }
        public string CategoryName { get; set; }
    }
}
