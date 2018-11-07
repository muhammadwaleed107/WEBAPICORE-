using System;
using System.Collections.Generic;
using System.Text;

namespace BolNMS.Mobile.Core.Models
{
    public class NewsFile
    {
        public int NewsFileId { get; set; }
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int NewsStatus { get; set; }
        public bool IsVerified { get; set; }
        public string Source { get; set; }
        public string CreationDate { get; set; }
        public string LanguageCode { get; set; }
        public string ImageThumbURL { get; set; }
        public string ImageURL { get; set; }
        public string CreatedBy { get; set; }
        public int CreatedById { get; set; }
        public string Text { get; set; }
        public string DescriptionText { get; set; }
        public List<string> URLs { get; set; }
        public string Slug { get; set; }
    }
}
