using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.UnitTests.Helpers
{
    public static class ExampleBuilder
    {
        public static MetaObjectStorableSettings BuildProductOperation()
        {
            var settings = new MetaObjectStorableSettings();
            settings.Header.Columns.Add(new MetaObjectTableColumn(DataTypeDefaults.Int.Uid, true)
            {
                Name = "id"
            });
            settings.Header.Columns.Add(new MetaObjectTableColumn(DataTypeDefaults.Decimal.Uid)
            {
                Name = "total",
                Formula = "$t.products.sum(\"amount\")"

            });

            var tableProducts = BuildProductTable();

            settings.DetailTables.Add(tableProducts);

            return settings;
        }

        public static MetaObjectStorableSettings BuildDiscountInHeaderExample()
        {
            var settings = new MetaObjectStorableSettings();
            settings.Header.Columns.Add(new MetaObjectTableColumn(DataTypeDefaults.Decimal.Uid)
            {
                Name = "discount",
            });

            var tableProducts = BuildProductTable();
            var colulmnDiscountAmount = new MetaObjectTableColumn(DataTypeDefaults.Decimal.Uid)
            {
                Name = "discount_amount",
                Formula = "$r.amount * $h.discount"
            };
            var columnAmountTotal = new MetaObjectTableColumn(DataTypeDefaults.Decimal.Uid)
            {
                Name = "amount_total",
                Formula = "$r.amount - $r.discount_amount"
            };

            tableProducts.Columns.Add(colulmnDiscountAmount);
            tableProducts.Columns.Add(columnAmountTotal);


            settings.DetailTables.Add(tableProducts);

            return settings;
        }

        public static MetaObjectTable BuildProductTable()
        {
            var tableProducts = new MetaObjectTable();
            tableProducts.Title = "Products";
            tableProducts.Name = "products";

            var columnProduct = new MetaObjectTableColumn()
            {
                Name = "product"
            };
            var columnQuantity = new MetaObjectTableColumn(DataTypeDefaults.Decimal.Uid)
            {
                Title = "Quantity",
                Name = "quantity",
            };
            var columnPrice = new MetaObjectTableColumn(DataTypeDefaults.Decimal.Uid)
            {
                Title = "Price",
                Name = "price",
            };
            var columnAmount = new MetaObjectTableColumn(DataTypeDefaults.Decimal.Uid)
            {
                Title = "Amount",
                Name = "amount",
                Formula = "$r.price * $r.quantity"
            };
            tableProducts.Columns.Add(columnProduct);
            tableProducts.Columns.Add(columnQuantity);
            tableProducts.Columns.Add(columnPrice);
            tableProducts.Columns.Add(columnAmount);

            return tableProducts;
        }
    }
}
