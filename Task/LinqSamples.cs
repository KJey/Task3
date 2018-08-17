// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
	[Title("LINQ Module")]
	[Prefix("Linq")]
	public class LinqSamples : SampleHarness
	{

		private DataSource dataSource = new DataSource();

		[Category("Restriction Operators")]
		[Title("Where - Task 1")]
		[Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
		public void Linq1()
		{
			int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

			var lowNums =
				from num in numbers
				where num < 5
				select num;

			Console.WriteLine("Numbers < 5:");
			foreach (var x in lowNums)
			{
				Console.WriteLine(x);
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 2")]
		[Description("This sample return return all presented in market products")]

        public void Linq2()
        {
            var products =
                from p in dataSource.Products
                where p.UnitsInStock > 0
                select p;

            foreach (var p in products)
            {
                ObjectDumper.Write(p);
            }
        }


        [Category("Tasks")]
        [Title("First task")]
        [Description("List of all customers, whose sum of all orders more than a certain value of X")]
        public void Linq001()
        {

            decimal x = 5000;

            var customers = dataSource.Customers
                .Where(c => c.Orders.Sum(o => o.Total) > x)
                .Select(c => new
                {
                    CustomerName = c.CompanyName,
                    Sum = c.Orders.Sum(o => o.Total)
                });

            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }
        }


        [Category("Tasks")]
        [Title("Second task")]
        [Description("List of suppliers in the same Country and City with Customer")]
        public void Linq002()
        {

            var customersandsuppliers = dataSource.Customers
                                        .Select(c => new
                                        {
                                            CustomerName = c.CompanyName,
                                            CustomerID = c.CustomerID,
                                            Suppliers = dataSource.Suppliers.Where(x=> x.Country==c.Country && x.City==c.City)
                    
                                        });




            foreach (var c in customersandsuppliers)
            {
                    ObjectDumper.Write(c.CustomerName + "(" + c.CustomerID + ") List of suppliers: " + string.Join(",",( c.Suppliers.Select(x => x.SupplierName))));
            }
        }


        [Category("Tasks")]
        [Title("Third task")]
        [Description("List of all customers, who have orders more than a certain value of X")]
        public void Linq003()
        {

            decimal x = 10000;

            var customers = dataSource.Customers
                .Where(c => c.Orders.Any(o=>o.Total>x))
                .Select(c => new
                {
                    CustomerName = c.CompanyName
                });

            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }
        }

    }
}
