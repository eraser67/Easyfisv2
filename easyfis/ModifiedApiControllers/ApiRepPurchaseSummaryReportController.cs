﻿using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Http;

namespace easyfis.ApiControllers
{
    public class ApiRepPurchaseSummaryReportController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.easyfisdbDataContext db = new Data.easyfisdbDataContext();

        // ===============================
        // Get Purchase Order Total Amount
        // ===============================
        public Decimal GetPurchaseOrderTotalAmount(Int32 POId)
        {
            var purchaseOrderItems = from d in db.TrnPurchaseOrderItems
                                     where d.POId == POId
                                     select d;

            if (purchaseOrderItems.Any())
            {
                return purchaseOrderItems.Sum(d => d.Amount);
            }
            else
            {
                return 0;
            }
        }

        // ===================
        // Purchase Order List
        // ===================
        [Authorize]
        [HttpGet]
        [Route("api/purchaseSummaryReport/list/{startDate}/{endDate}/{companyId}/{branchId}")]
        public List<Models.TrnPurchaseOrder> listPurchaseSummaryReport(String startDate, String endDate, String companyId, String branchId)
        {
            var purchaseOrders = from d in db.TrnPurchaseOrders
                                 where d.PODate >= Convert.ToDateTime(startDate)
                                 && d.PODate <= Convert.ToDateTime(endDate)
                                 && d.MstBranch.CompanyId == Convert.ToInt32(companyId)
                                 && d.BranchId == Convert.ToInt32(branchId)
                                 && d.IsLocked == true
                                 select new Models.TrnPurchaseOrder
                                 {
                                     Id = d.Id,
                                     Branch = d.MstBranch.Branch,
                                     PONumber = d.PONumber,
                                     PODate = d.PODate.ToShortDateString(),
                                     Supplier = d.MstArticle.Article,
                                     Remarks = d.Remarks,
                                     IsClose = d.IsClose,
                                     Amount = GetPurchaseOrderTotalAmount(d.Id)
                                 };

            return purchaseOrders.ToList();
        }
    }
}
