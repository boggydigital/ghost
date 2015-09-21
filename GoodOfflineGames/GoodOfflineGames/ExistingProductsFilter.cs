﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Model;

namespace GOG
{
    public class ExistingProductsFilter : IFilterDelegate<Product, long>
    {
        public IEnumerable<Product> Filter(IEnumerable<Product> products, IList<long> existingProducts)
        {
            foreach (var product in products)
            {
                // filter products in current set = return products that are NOT present in current set
                if (!existingProducts.Contains(product.Id))
                {
                    yield return product;
                }
            }
        }
    }
}
