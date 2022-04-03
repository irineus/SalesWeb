using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalesWeb.Models.Entities;
using SalesWeb.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SalesWeb.Test.Models.Entities.Tests
{
    [TestClass]
    public class DepartmentTest
    {
        private Department? _departmentTest;
        private Seller? seller1;
        private Seller? seller2;
        private Seller? seller3;

        public TestContext? TestContext  { get; set; }

        #region Test Initialize and CleanUp

        [TestInitialize]
        public void TestInitialize()
        {
            _departmentTest = new Department() { Id = 1, Name = "Departamento Teste" };
            
            if ((TestContext.TestName == "AddSingleSeller") ||
                (TestContext.TestName == "AddMultipleSellers") ||
                (TestContext.TestName == "TotalSales_WithOneSeller") || 
                (TestContext.TestName == "TotalSales_WithMultipleSellers") ||
                (TestContext.TestName == "TotalBilledSales_WithOneSeller") ||
                (TestContext.TestName == "TotalBilledSales_WithMultipleSellers"))
            {
                seller1 = new Seller() { Id = 1, Name = "João Silva", BaseSalary = 2000.57, BirthDate = DateTime.Parse("09/01/1979"), Department = _departmentTest, DepartmentId = _departmentTest.Id, Email = "js@gmail.com" };
            }

            if ((TestContext.TestName == "AddMultipleSellers") || 
                (TestContext.TestName == "TotalSales_WithMultipleSellers") ||
                (TestContext.TestName == "TotalBilledSales_WithMultipleSellers"))
            {
                seller2 = new Seller() { Id = 2, Name = "Maria Caldeira", BaseSalary = 1500.00, BirthDate = DateTime.Parse("21/08/1995"), Department = _departmentTest, DepartmentId = _departmentTest.Id, Email = "maria.c@hotmail.com" };
                seller3 = new Seller() { Id = 3, Name = "Pedro Guimarães", BaseSalary = 900.12, BirthDate = DateTime.Parse("15/12/2001"), Department = _departmentTest, DepartmentId = _departmentTest.Id, Email = "paulogui@terra.com.br" };
            }

            if ((TestContext.TestName == "TotalSales_WithOneSeller") ||
                (TestContext.TestName == "TotalSales_WithMultipleSellers") ||
                (TestContext.TestName == "TotalBilledSales_WithOneSeller") ||
                (TestContext.TestName == "TotalBilledSales_WithMultipleSellers"))
            {
                _departmentTest.Sellers.Add(seller1);
                SalesRecord sr1 = new SalesRecord() { Id = 1, Amount = 120.62, Date = DateTime.Parse("10/03/2022 11:45:12"), Seller = seller1, SellerId = seller1.Id, Status = SaleStatus.PENDING};
                seller1.Sales.Add(sr1);
            }

            if ((TestContext.TestName == "TotalSales_WithMultipleSellers") ||
                (TestContext.TestName == "TotalBilledSales_WithMultipleSellers"))
            {
                _departmentTest.Sellers.Add(seller2);
                SalesRecord sr2 = new SalesRecord() { Id = 2, Amount = 29.95, Date = DateTime.Parse("28/02/2022 15:12:54"), Seller = seller2, SellerId = seller2.Id, Status = SaleStatus.BILLED };
                seller2.Sales.Add(sr2);
                _departmentTest.Sellers.Add(seller3);
                SalesRecord sr3 = new SalesRecord() { Id = 3, Amount = 89.00, Date = DateTime.Parse("31/12/2021 09:45:10"), Seller = seller3, SellerId = seller3.Id, Status = SaleStatus.CANCELED };
                seller3.Sales.Add(sr3);
            }


        }

        [TestCleanup]
        public void TestCleanUp()
        {
            _departmentTest = null;
            seller1 = null;
            seller2 = null;
            seller3 = null;
        }


        #endregion

        [TestMethod]
        public void AddSingleSeller()
        {
            _departmentTest.AddSeller(seller1);
            Assert.AreEqual(seller1, _departmentTest.Sellers.FirstOrDefault());
        }

        [TestMethod]
        public void AddMultipleSellers()
        {
            _departmentTest.AddSeller(seller1);
            _departmentTest.AddSeller(seller2);
            _departmentTest.AddSeller(seller3);
            
            Assert.AreEqual(3, _departmentTest.Sellers.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullSeller_ThrowsArgumentNullException()
        {
            _departmentTest.AddSeller(null);
        }

        [TestMethod]
        public void TotalSales_WithNoSeller()
        {
            DateTime initial = DateTime.MinValue;
            DateTime final = DateTime.MaxValue;
            Assert.AreEqual(0, _departmentTest.TotalSales(initial, final));
        }

        [TestMethod]
        public void TotalSales_WithOneSeller()
        {
            DateTime initial = DateTime.MinValue;
            DateTime final = DateTime.MaxValue;
            Assert.AreEqual(120.62, _departmentTest.TotalSales(initial, final));
        }

        [TestMethod]
        public void TotalSales_WithMultipleSellers()
        {
            DateTime initial = DateTime.MinValue;
            DateTime final = DateTime.MaxValue;
            Assert.AreEqual(239.57, _departmentTest.TotalSales(initial, final));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TotalSales_InitialDateAheadFinalDate_ThrowsArgumentOutOfRangeException()
        {
            DateTime initial = DateTime.MaxValue; 
            DateTime final = DateTime.MinValue;
            _departmentTest.TotalSales(initial, final);
        }

        [TestMethod]
        public void TotalBilledSales_WithNoSeller()
        {
            DateTime initial = DateTime.MinValue;
            DateTime final = DateTime.MaxValue;
            Assert.AreEqual(0, _departmentTest.TotalBilledSales(initial, final));
        }

        [TestMethod]
        public void TotalBilledSales_WithOneSeller()
        {
            DateTime initial = DateTime.MinValue;
            DateTime final = DateTime.MaxValue;
            Assert.AreEqual(0, _departmentTest.TotalBilledSales(initial, final));
        }

        [TestMethod]
        public void TotalBilledSales_WithMultipleSellers()
        {
            DateTime initial = DateTime.MinValue;
            DateTime final = DateTime.MaxValue;
            Assert.AreEqual(29.95, _departmentTest.TotalBilledSales(initial, final));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TotalBilledSales_InitialDateAheadFinalDate_ThrowsArgumentOutOfRangeException()
        {
            DateTime initial = DateTime.MaxValue;
            DateTime final = DateTime.MinValue;
            _departmentTest.TotalBilledSales(initial, final);
        }
    }
}
