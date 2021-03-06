﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace easyfis.Models
{
    public class MstArticleComponent
    {

        [Key]

        public Int32 Id { get; set; }
        public Int32 ArticleId { get; set; }
        public String Article { get; set; }
        public String ManualArticleCode { get; set; }
        public Int32 ComponentArticleId { get; set; }
        public String ComponentArticle { get; set; }
        public Int32? ComponentArticleInventoryId { get; set; }
        public Decimal Quantity { get; set; }
        public Int32 UnitId { get; set; }
        public String Unit { get; set; }
        public Decimal Cost { get; set; }
        public Decimal Price { get; set; }
        public Decimal Amount { get; set; }
        public String Particulars { get; set; }
        public String ComponentArticleCode { get; internal set; }
    }
}