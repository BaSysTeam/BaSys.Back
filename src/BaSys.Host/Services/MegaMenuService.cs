using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;

namespace BaSys.Host.Services
{
    public class MegaMenuService : IMegaMenuService
    {
        public ResultWrapper<List<MegaMenuItemDto>> GetItems()
        {
            var result = new ResultWrapper<List<MegaMenuItemDto>>();
            var items = new List<MegaMenuItemDto>();

            var furniture = new MegaMenuItemDto
            {
                Key = Guid.NewGuid(),
                Label = "Furniture",
                Visible = true,
                Items = new List<List<ColumnItemDto>>
                {
                    new List<ColumnItemDto>
                    {
                        new ColumnItemDto
                        {
                            Key = Guid.NewGuid(),
                            Label = "Living Room",
                            Visible = true,
                            Items = new List<ColumnItemPointDto>
                            {
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "Accessories", Visible = true },
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "Armchair", Visible = true },
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "Coffee Table", Visible = true },
                            }
                        }
                    },
                    new List<ColumnItemDto>
                    {
                        new ColumnItemDto
                        {
                            Key = Guid.NewGuid(),
                            Label = "Kitchen",
                            Visible = true,
                            Items = new List<ColumnItemPointDto>
                            {
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "Bar stool", Visible = true },
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "Chair", Visible = true },
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "Table", Visible = true },
                            }
                        },
                        new ColumnItemDto
                        {
                            Key = Guid.NewGuid(),
                            Label = "Bathroom",
                            Visible = true,
                            Items = new List<ColumnItemPointDto>
                            {
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "Accessories", Visible = true },
                            }
                        }
                    }
                }
            };
            var electronics = new MegaMenuItemDto
            {
                Key = Guid.NewGuid(),
                Label = "Electronics",
                Visible = true,
                Items = new List<List<ColumnItemDto>>
                {
                    new List<ColumnItemDto>
                    {
                        new ColumnItemDto
                        {
                            Key = Guid.NewGuid(),
                            Label = "Computer",
                            Visible = true,
                            Items = new List<ColumnItemPointDto>
                            {
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "Monitor", Visible = true },
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "Mouse", Visible = true },
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "Keyboard", Visible = true },
                            }
                        }
                    },
                    new List<ColumnItemDto>
                    {
                        new ColumnItemDto
                        {
                            Key = Guid.NewGuid(),
                            Label = "Home Theather",
                            Visible = true,
                            Items = new List<ColumnItemPointDto>
                            {
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "Projector", Visible = true },
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "Speakers", Visible = true },
                                new ColumnItemPointDto { Key = Guid.NewGuid(), Label = "TVs", Visible = true },
                            }
                        }
                    }
                }
            };
            var newItem = new MegaMenuItemDto
            {
                Key = Guid.NewGuid(),
                Label = "New item",
                Visible = true,
            };

            items.Add(furniture);
            items.Add(electronics);
            items.Add(newItem);

            result.Success(items);
            return result;
        }
    }
}
