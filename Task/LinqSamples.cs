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
using System.Text.RegularExpressions;
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

        #region Done
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
        //Для каждого клиента составьте список поставщиков, находящихся в той же стране и том же городе. 
        //Сделайте задания с использованием группировки и без.
        public void Linq002()
        {
            var customersandsuppliers = dataSource.Customers
                                        .Select(c => new
                                        {
                                            CustomerName = c.CompanyName,
                                            c.CustomerID,
                                            Suppliers = dataSource.Suppliers.Where(x=> x.Country==c.Country && x.City==c.City)
                                        });
            ObjectDumper.Write("Without groupping:");
            foreach (var c in customersandsuppliers)
            {
                if (c.Suppliers.FirstOrDefault() != null)
                {
                    ObjectDumper.Write(c.CustomerName + "(" + c.CustomerID + ") List of suppliers: " + string.Join(",", (c.Suppliers.Select(x => x.SupplierName))));
                }
            }

            var customersandsuppliersgroups = dataSource.Customers
                                                        .GroupJoin(dataSource.Suppliers,
                                                        c => new { c.Country, c.City},
                                                        s => new { s.Country, s.City},
                                                        (c, s) => new { Customers = c,Suppliers = s});
            ObjectDumper.Write("--------------------------------------------------------");
            ObjectDumper.Write("With groupping:");
            foreach (var c in customersandsuppliersgroups)
            {
                if (c.Suppliers.FirstOrDefault() != null)
                {
                    ObjectDumper.Write(" Customer " + c.Customers.CompanyName+" ("+c.Customers.Country+"," + c.Customers.City + ")");
                    foreach (var s in c.Suppliers)
                    {
                        ObjectDumper.Write("  Supplier " + s.SupplierName);
                    }
                }
            }

        }


        [Category("Tasks")]
        [Title("Third task")]
        [Description("List of all customers, who have orders more than a certain value of X")]
        public void Linq003()
        {

            decimal x = 10000;

            var customers = dataSource.Customers
                .Where(c => c.Orders.Any(o => o.Total > x))
                .Select(c => new
                {
                    CustomerName = c.CompanyName
                });

            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }
        }


        [Category("Tasks")]
        [Title("Fourth task")]
        [Description("List of all customers with date of first order")]
        public void Linq004()
        {
            var customers = dataSource.Customers.Where(c => c.Orders.Any())
                .Select(c => new
                {
                    CustomerName = c.CompanyName,
                    DateOfEntry = c.Orders.Min(x => x.OrderDate)

                });

            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }
        }


        [Category("Tasks")]
        [Title("Fifth task")]
        [Description("List of all customers with date of first order, ordered by date and customer's name")]
        public void Linq005()
        {
            var customers = dataSource.Customers
                .Where(c => c.Orders.Any())
                .Select(c => new
                {
                    CustomerName = c.CompanyName,
                    DateOfEntry = c.Orders.Min(x => x.OrderDate),
                    OrderedSum = c.Orders.Sum(x=> x.Total)
                })
                .OrderBy(c => c.DateOfEntry.Year)
                .ThenBy(c => c.DateOfEntry.Month)
                .ThenByDescending(c => c.OrderedSum)
                .ThenBy(c => c.CustomerName)
                ;

            foreach (var c in customers)
            {
                ObjectDumper.Write("Year:" + c.DateOfEntry.Year + " Month: " + c.DateOfEntry.Month + " Sum: " + c.OrderedSum + " Name: " + c.CustomerName);
            }
        }


        [Category("Tasks")]
        [Title("Sixth task")]
        [Description("List of customers who has non-number post code or empty region field or phone number doesn't have operator's code")]
        //Укажите всех клиентов, у которых указан нецифровой почтовый код или не заполнен регион или в телефоне 
        //    не указан код оператора(для простоты считаем, что это равнозначно «нет круглых скобочек в начале»).
        public void Linq006()
        {

            Regex postalcodecheck = new Regex("[^0-9]");
            var customers = from c in dataSource.Customers
                            where string.IsNullOrWhiteSpace(c.Region) || string.IsNullOrEmpty(c.Region)
                            || c.PostalCode == null || c.PostalCode.Any(x => x < '0' || x > '9')
                            || c.Phone.First() != '('
                            select c;

            foreach (var c in customers)
            {
                ObjectDumper.Write("Region \"" + c.Region + "\" PostalCode \"" + c.PostalCode + "\" Phone \"" + c.Phone + "\"");
            }
        }


        [Category("Tasks")]
        [Title("Seventh task")]
        [Description("List of products groupped by category, availability on stock, and price")]
        //Сгруппируйте все продукты по категориям, внутри – по наличию на складе, внутри последней группы отсортируйте по стоимости
        public void Linq007()
        {

            var products =
                from p in dataSource.Products
                group p by p.Category into prod
                select new
                {
                    Category = prod.Key,
                    Count = prod.Count(),
                    Products = from p in prod
                               group p by p.UnitsInStock into uis

                               select new
                               {
                                   Quantity = uis.Key,
                                   Product = from p in prod orderby p.UnitPrice select p,
                               }
                };

            foreach (var p in products)
            {
                ObjectDumper.Write(p.Category + " : " + p.Count);
                foreach (var p1 in p.Products)
                {
                    ObjectDumper.Write(" Quantity in stock = " + p1.Quantity);
                    foreach (var p2 in p1.Product)
                        ObjectDumper.Write("  Name: " + p2.ProductName + " Price = " + p2.UnitPrice);

                }
            }
        }


        [Category("Tasks")]
        [Title("Eighth task")]
        [Description("List of products separated in 3 groups by price")]
        //Сгруппируйте товары по группам «дешевые», «средняя цена», «дорогие». Границы каждой группы задайте сами
        public void Linq008()
        {
            decimal cheapprice = 10;
            decimal expenciveprice = 20;

            var products =
                from p in dataSource.Products
                group p by p.UnitPrice < cheapprice ? "Cheap" : p.UnitPrice < expenciveprice ? "Middle" : "Expencive" into prod
                select new
                {
                    Category = prod.Key,
                    Count = prod.Count(),
                    Products = from p in prod select p
                };

            foreach (var p in products)
            {
                ObjectDumper.Write(p.Category + " : " + p.Count + " products");
                foreach (var p1 in p.Products)
                {
                    ObjectDumper.Write("  Product \"" + p1.ProductName + "\" with price = " + p1.UnitPrice);
                }
            }
        }
        #endregion

        [Category("Tasks")]
        [Title("Nineth task")]
        [Description("List of cities with profitability")]
        //Рассчитайте среднюю прибыльность каждого города(среднюю сумму заказа по всем клиентам из данного города) 
        //и среднюю интенсивность(среднее количество заказов, приходящееся на клиента из каждого города)
        public void Linq009()
        {
            var customers = dataSource.Customers

                .GroupBy(c => c.City)
                .Select(c => new
                {
                    City = c.Key,
                    Avg_Orders_quantity = c.Average(x => x.Orders.Length),
                    Avg_Orders_Sum = c.Average(x => x.Orders.Sum(o => o.Total))
                }).OrderBy(x => x.City);





            foreach (var c in customers)
            {
                ObjectDumper.Write("City: " + c.City + ", Quantity of orders=" + c.Avg_Orders_quantity + ", Average cost="+ c.Avg_Orders_Sum);
            }
        }


        [Category("Tasks")]
        [Title("Tenth task")]
        [Description("Average annual statistic of client's activities")]
        //Сделайте среднегодовую статистику активности клиентов 
        //по месяцам (без учета года), 
        //статистику по годам, 
        //по годам и месяцам (т.е. когда один месяц в разные годы имеет своё значение).
        public void Linq010()
        {
            var statistics = dataSource.Customers
                .Select(s => new
                {
                    Name = s.CompanyName,
                    Avg_Month = s.Orders.GroupBy(x=> x.OrderDate.Month).Select(x=> new { Month = x.Key,Count = x.Count()}),
                    Avg_Year = s.Orders.GroupBy(x => x.OrderDate.Year).Select(x => new { Year = x.Key, Count = x.Count() }),
                    Avg_Month_and_Year = s.Orders.GroupBy(x => new { x.OrderDate.Year, x.OrderDate.Month }).Select(x => new { Year = x.Key.Year, Month = x.Key.Month, Count = x.Count() }),
                }).OrderBy(s => s.Name);





            foreach (var stat in statistics)
            {
                ObjectDumper.Write("Company name " + stat.Name + "-------------------------------------------");

                ObjectDumper.Write(" Monthly statistics: ");
                foreach (var s1 in stat.Avg_Month)
                {
                    ObjectDumper.Write("  Month " + s1.Month + " = " + s1.Count);

                }

                ObjectDumper.Write(" Yearly statistics: ");
                foreach (var s2 in stat.Avg_Year)
                {
                    ObjectDumper.Write("  Year " + s2.Year + " = " + s2.Count);

                }

                ObjectDumper.Write(" Monthly statistics: ");
                foreach (var s3 in stat.Avg_Month_and_Year)
                {
                    ObjectDumper.Write("  Year " + s3.Year + " Month " + s3.Month + " = " + s3.Count);

                }
            }
        }
    }
}
