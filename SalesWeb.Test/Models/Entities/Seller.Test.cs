using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalesWeb.Models.Entities;
using SalesWeb.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SalesWeb.Test.Models.Entities.Tests
{
    [TestClass]
    public class SellerTest
    {
        public TestContext? TestContext { get; set; }

        private Seller? _sellerTest;
        private SalesRecord _sr1;
        private SalesRecord _sr2;
        private SalesRecord _sr3;

        #region Test Initialize and CleanUp

        [TestInitialize]
        public void TestInitialize()
        {
            _sellerTest = new Seller() { Id = 1, Name = "João Silva", BaseSalary = 2000.57, BirthDate = DateTime.Parse("09/01/1979"), Department = new Department(), DepartmentId = 1, Email = "js@gmail.com" };
            if ((TestContext.TestName == "AddSingleSale") ||
                (TestContext.TestName == "AddMultipleSales") ||
                (TestContext.TestName == "RemoveSingleSale") ||
                (TestContext.TestName == "RemoveMultipleSales") ||
                (TestContext.TestName == "RemoveNonExistentSale_ThrowsKeyNotFoundException") ||
                (TestContext.TestName == "TotalSales_WithOneSale") ||
                (TestContext.TestName == "TotalSales_WithMultipleSales") ||
                (TestContext.TestName == "TotalBilledSales_WithOneSale") ||
                (TestContext.TestName == "TotalBilledSales_WithMultipleSale"))
            {
                _sr1 = new SalesRecord() { Id = 1, Amount = 149.19, Date = DateTime.Parse("09/03/2022 18:01:59"), Seller = _sellerTest, SellerId = _sellerTest.Id, Status = SaleStatus.PENDING };
            }

            if ((TestContext.TestName == "AddMultipleSales") ||
                (TestContext.TestName == "RemoveSingleSale") ||
                (TestContext.TestName == "RemoveMultipleSales") ||
                (TestContext.TestName == "RemoveNonExistentSale_ThrowsKeyNotFoundException") ||
                (TestContext.TestName == "TotalSales_WithMultipleSales") ||
                (TestContext.TestName == "TotalBilledSales_WithOneSale") ||
                (TestContext.TestName == "TotalBilledSales_WithMultipleSale"))
            {
                _sr2 = new SalesRecord() { Id = 2, Amount = 29.95, Date = DateTime.Parse("28/02/2022 15:12:54"), Seller = _sellerTest, SellerId = _sellerTest.Id, Status = SaleStatus.BILLED };
                _sr3 = new SalesRecord() { Id = 3, Amount = 89.00, Date = DateTime.Parse("31/12/2021 09:45:10"), Seller = _sellerTest, SellerId = _sellerTest.Id, Status = SaleStatus.CANCELED };

            }
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            _sellerTest = null;
            _sr1 = null;
            _sr2 = null;
            _sr3 = null;
        }

        #endregion

        [TestMethod]
        public void AddSingleSale()
        {
            _sellerTest.AddSales(_sr1);
            Assert.AreEqual(_sr1, _sellerTest.Sales.FirstOrDefault());
        }

        [TestMethod]
        public void AddMultipleSales()
        {
            _sellerTest.AddSales(_sr1);
            _sellerTest.AddSales(_sr2);
            _sellerTest.AddSales(_sr3);
            Assert.AreEqual(3, _sellerTest.Sales.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullSale_ThrowsArgumentNullException()
        {
            _sellerTest.AddSales(null);
        }

        [TestMethod]
        public void RemoveSingleSale()
        {
            _sellerTest.AddSales(_sr1);
            _sellerTest.RemoveSales(_sr1);
            Assert.AreEqual(0, _sellerTest.Sales.Count());
        }

        [TestMethod]
        public void RemoveMultipleSales()
        {
            _sellerTest.AddSales(_sr1);
            _sellerTest.AddSales(_sr2);
            _sellerTest.AddSales(_sr3);
            _sellerTest.RemoveSales(_sr1);
            _sellerTest.RemoveSales(_sr3);
            Assert.AreEqual(1, _sellerTest.Sales.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveNullSale_ThrowsArgumentNullException()
        {
            _sellerTest.RemoveSales(null);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void RemoveNonExistentSale_ThrowsKeyNotFoundException()
        {
            _sellerTest.AddSales(_sr1);
            _sellerTest.RemoveSales(_sr2);
        }

        [TestMethod]
        public void TotalSales_WithNoSale()
        {
            DateTime initial = DateTime.MinValue;
            DateTime final = DateTime.MaxValue;
            Assert.AreEqual(0, _sellerTest.TotalSales(initial, final));
        }

        [TestMethod]
        public void TotalSales_WithOneSale()
        {
            _sellerTest.AddSales(_sr1);
            DateTime initial = DateTime.MinValue;
            DateTime final = DateTime.MaxValue;
            Assert.AreEqual(149.19, _sellerTest.TotalSales(initial, final));
        }

        [TestMethod]
        public void TotalSales_WithMultipleSales()
        {
            _sellerTest.AddSales(_sr1);
            _sellerTest.AddSales(_sr2);
            _sellerTest.AddSales(_sr3);
            DateTime initial = DateTime.MinValue;
            DateTime final = DateTime.MaxValue;
            Assert.AreEqual(268.14, _sellerTest.TotalSales(initial, final));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TotalSales_InitialDateAheadFinalDate_ThrowsArgumentOutOfRangeException()
        {
            DateTime initial = DateTime.MaxValue;
            DateTime final = DateTime.MinValue;
            _sellerTest.TotalSales(initial, final);
        }

        [TestMethod]
        public void TotalBilledSales_WithNoSale()
        {
            DateTime initial = DateTime.MinValue;
            DateTime final = DateTime.MaxValue;
            Assert.AreEqual(0, _sellerTest.TotalBilledSales(initial, final));
        }

        [TestMethod]
        public void TotalBilledSales_WithOneSale()
        {
            _sellerTest.AddSales(_sr1);
            DateTime initial = DateTime.MinValue;
            DateTime final = DateTime.MaxValue;
            Assert.AreEqual(0, _sellerTest.TotalBilledSales(initial, final));
        }

        [TestMethod]
        public void TotalBilledSales_WithMultipleSale()
        {
            _sellerTest.AddSales(_sr1);
            _sellerTest.AddSales(_sr2);
            _sellerTest.AddSales(_sr3);
            DateTime initial = DateTime.MinValue;
            DateTime final = DateTime.MaxValue;
            Assert.AreEqual(29.95, _sellerTest.TotalBilledSales(initial, final));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TotalBilledSales_InitialDateAheadFinalDate_ThrowsArgumentOutOfRangeException()
        {
            DateTime initial = DateTime.MaxValue;
            DateTime final = DateTime.MinValue;
            _sellerTest.TotalBilledSales(initial, final);
        }
    }
}
