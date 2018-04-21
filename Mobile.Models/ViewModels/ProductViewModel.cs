﻿using System.Collections.Generic;

namespace Mobile.Models.ViewModels
{
    public class ProductViewModel : SearchProductViewModel
    {
        public string DiscountAccompanying { get; set; }
    }

    public class ProductDetailViewModel : SearchProductViewModel
    {
        public string Code { get; set; }
        public string Detail { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> MoreImages { get; set; }
        public bool IncludeVAT { get; set; }
        public int Quantity { get; set; }
        public string CategoryName { get; set; }
        public bool Status { get; set; }
    }
}
