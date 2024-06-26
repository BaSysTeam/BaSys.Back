﻿using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.DAL;
using BaSys.SuperAdmin.DAL.Models;
using BaSys.SuperAdmin.DTO;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Services;

public class AppRecordsService : IAppRecordsService
{
    private readonly SuperAdminDbContext _context;

    public AppRecordsService(SuperAdminDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all AppRecords
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<AppRecordDto>> GetAppRecords()
    {
        return (await _context.AppRecords
                .AsNoTracking()
                .ToListAsync())
            .Select(x => new AppRecordDto(x));
    }

    /// <summary>
    /// Return individual record by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<AppRecordDto?> GetAppRecord(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException();

        var record = await _context.AppRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.ToUpper().Equals(id.ToUpper()));

        if (record == null)
            throw new ArgumentException($"Record with id: {id} not found");
        
        return new AppRecordDto(record);
    }

    /// <summary>
    /// Add
    /// </summary>
    /// <param name="appRecord"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<AppRecordDto> AddAppRecord(AppRecordDto appRecord)
    {
        if (string.IsNullOrEmpty(appRecord?.Id))
            throw new ArgumentException($"Id cannot be empty");
        
        if (await _context.AppRecords.AsNoTracking().AnyAsync(x => x.Id.ToUpper() == appRecord.Id.ToUpper()))
            throw new ArgumentException($"Element with Id == '{appRecord.Id}' already exists");

        _context.AppRecords.Add(appRecord.ToModel());
        await _context.SaveChangesAsync();

        return appRecord;
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<int> DeleteAppRecord(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException();

        var appRecord = await _context.AppRecords.FirstOrDefaultAsync(x => x.Id.ToUpper() == id.ToUpper());
        if (appRecord == null)
            throw new ArgumentException($"Element with Id == '{id}' not found");

        _context.AppRecords.Remove(appRecord);
        var deletedCount = await _context.SaveChangesAsync();

        return deletedCount;
    }

    /// <summary>
    /// Edit
    /// </summary>
    /// <param name="appRecord"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<AppRecordDto> UpdateAppRecord(AppRecordDto appRecord)
    {
        if (appRecord == null ||
            string.IsNullOrEmpty(appRecord.Id) ||
            string.IsNullOrEmpty(appRecord.Title))
            throw new ArgumentException();

        var dbAppRecord = await _context.AppRecords.FirstOrDefaultAsync(x => x.Id.ToUpper() == appRecord.Id.ToUpper());
        if (dbAppRecord == null)
            throw new ArgumentException($"Element with Id == '{appRecord.Id}' not found");

        dbAppRecord.Fill(appRecord.ToModel());

        await _context.SaveChangesAsync();

        return appRecord;
    }
}