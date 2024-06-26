﻿using BaSys.SuperAdmin.DAL.Models;

namespace BaSys.SuperAdmin.DTO;

public class AppRecordDto
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Memo { get; set; }

    public AppRecordDto()
    {
    }

    public AppRecordDto(AppRecord model)
    {
        Id = model.Id;
        Title = model.Title;
        Memo = model.Memo;
    }

    public AppRecord ToModel()
    {
        return new AppRecord
        {
            Id = Id ?? string.Empty,
            Title = Title ?? string.Empty,
            Memo = Memo
        };
    }
}