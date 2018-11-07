using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BolNMS.Mobile.Core.DataTransfer
{
    public class NewsFileFilterDTO
    {
        public int PageOffset { get; set; }
        public int PageSize { get; set; }
        [DefaultValue(-1)]
        public int CategoryId { get; set; }
        public string SearchText { get; set; }
        public List<int> Filters { get; set; }
    }
}
