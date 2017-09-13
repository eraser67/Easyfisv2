﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Globalization;

namespace easyfis.ApiControllers
{
    public class ApiHourlyTopSellingReportController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.easyfisdbDataContext db = new Data.easyfisdbDataContext();

        // =================
        // Time Stamp Format
        // =================
        public String TimeStampFormat(String salesInvoiceItemTimeStamp)
        {
            CultureInfo cultureESUS = CultureInfo.CreateSpecificCulture("en-US");
            if (!salesInvoiceItemTimeStamp.Equals(""))
            {
                DateTime dateToDisplay = new DateTime(0001, 1, 1, Convert.ToInt32(salesInvoiceItemTimeStamp), 0, 0);
                return dateToDisplay.ToString("t", cultureESUS);
            }
            else
            {
                return "";
            }
        }

        // ====================================
        // Hourly Top Selling Items Report List
        // ====================================
        [Authorize, HttpGet, Route("api/hourlyTopItemsSellingReport/list/{startDate}/{endDate}/{companyId}/{branchId}")]
        public List<Models.TrnSalesInvoiceItem> ListHourlyTopItemsSellingReport(String startDate, String endDate, String companyId, String branchId)
        {
            var salesInvoiceItems = from d in db.TrnSalesInvoiceItems
                                    where d.TrnSalesInvoice.BranchId == Convert.ToInt32(branchId)
                                    && d.TrnSalesInvoice.MstBranch.CompanyId == Convert.ToInt32(companyId)
                                    && d.TrnSalesInvoice.SIDate >= Convert.ToDateTime(startDate)
                                    && d.TrnSalesInvoice.SIDate <= Convert.ToDateTime(endDate)
                                    && d.TrnSalesInvoice.IsLocked == true
                                    group d by new
                                    {
                                        Branch = d.TrnSalesInvoice.MstBranch.Branch,
                                        ItemId = d.ItemId,
                                        Item = d.MstArticle.Article,
                                        BasePrice = d.MstArticle.Price,
                                        BaseUnit = d.MstArticle.MstUnit.Unit,
                                        SalesItemTimeStamp = d.SalesItemTimeStamp.Hour,
                                    } into g
                                    select new Models.TrnSalesInvoiceItem
                                    {
                                        Branch = g.Key.Branch,
                                        ItemId = g.Key.ItemId,
                                        Item = g.Key.Item,
                                        BaseUnit = g.Key.BaseUnit,
                                        BaseQuantity = g.Sum(d => d.BaseQuantity),
                                        BasePrice = g.Key.BasePrice,
                                        Amount = g.Sum(d => d.BaseQuantity) * g.Key.BasePrice,
                                        SalesItemTimeStamp = TimeStampFormat(g.Key.SalesItemTimeStamp.ToString())
                                    };

            return salesInvoiceItems.OrderByDescending(d => d.BaseQuantity).ToList();
        }
    }
}
