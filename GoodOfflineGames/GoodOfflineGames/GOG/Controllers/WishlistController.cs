﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Models;
using GOG.SharedModels;

namespace GOG.Controllers
{
    public class WishlistController: ProductsResultController
    {
        public WishlistController(ProductsResult productsResult,
            IStringDataRequestController stringDataRequestController,
            ISerializationController serializationController) :
            base(productsResult, stringDataRequestController, serializationController)
        {
        }

        public async Task UpdateWishlisted(IConsoleController consoleController)
        {
            consoleController.Write("Updating wishlisted products...");

            var wishlistGogData = await stringDataRequestController.RequestData<ProductsResult>(Urls.Wishlist);

            wishlistGogData.Products.ForEach(wp =>
            {
                var wishlistedProduct = productsResult.Products.Find(p => p.Id == wp.Id);
                wishlistedProduct.Wishlisted = true;

            });

            consoleController.WriteLine("DONE.");
        }
    }
}
