using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper;
using Budgeter.Models;
using CsvHelper.Configuration;

namespace Budgeter.Helpers
{
    public sealed class MyClassMap : CsvClassMap<Transaction>
    {
        public MyClassMap()
        {
            Map(m => m.Date).Name("Date");
            Map(m => m.AccountId).Name("Account");
            Map(m => m.CategoryId).Name("Category");
            Map(m => m.Amount).Name("Amount");
            Map(m => m.Description).Name("Description");
            Map(m => m.IsReconciled).Name("Processed");
            Map(m => m.IsExpense).Name("Expense");
        }
    }
}