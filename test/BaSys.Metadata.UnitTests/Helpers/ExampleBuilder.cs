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
            settings.Header.Columns.Add(new MetaObjectTableColumn()
            {
                Name = "id",
                DataTypeUid = DataTypeDefaults.Int.Uid,
                PrimaryKey = true,
            });
            settings.Header.Columns.Add(new MetaObjectTableColumn()
            {
                Name = "total",
                DataTypeUid = DataTypeDefaults.Decimal.Uid,
                Formula = "$t.products.sum(\"amount\")"

            });

            var tableProducts = BuildProductTable();

            settings.DetailTables.Add(tableProducts);

            return settings;
        }

        public static MetaObjectStorableSettings BuildDiscountInHeaderExample()
        {
            var settings = new MetaObjectStorableSettings();
            settings.Header.Columns.Add(new MetaObjectTableColumn()
            {
                Name = "discount",
                DataTypeUid = DataTypeDefaults.Decimal.Uid,

            });

            var tableProducts = BuildProductTable();
            var colulmnDiscountAmount = new MetaObjectTableColumn()
            {
                Name = "discount_amount",
                DataTypeUid = DataTypeDefaults.Decimal.Uid,
                Formula = "$r.amount * $h.discount"
            };
            var columnAmountTotal = new MetaObjectTableColumn()
            {
                Name = "amount_total",
                DataTypeUid = DataTypeDefaults.Decimal.Uid,
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
            var columnQuantity = new MetaObjectTableColumn()
            {
                Title = "Quantity",
                Name = "quantity",
                DataTypeUid = DataTypeDefaults.Decimal.Uid,
            };
            var columnPrice = new MetaObjectTableColumn()
            {
                Title = "Price",
                Name = "price",
                DataTypeUid = DataTypeDefaults.Decimal.Uid,
            };
            var columnAmount = new MetaObjectTableColumn()
            {
                Title = "Amount",
                Name = "amount",
                DataTypeUid = DataTypeDefaults.Decimal.Uid,
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
