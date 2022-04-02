using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.ByName;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Picklist;

public class PicklistService: IPicklistService
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public event Action? OnChange;
    public List<KeyValueDto> DataSource { get; private set; } = new();

    public PicklistService(IApplicationDbContext context, IMapper mapper)
    {
        
        _context = context;
        _mapper = mapper;
    }
    public async Task Initialize()
    {
        if (DataSource.Count > 0) return;
        await _semaphore.WaitAsync();
        try
        {

            DataSource = await _context.KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
          .ProjectTo<KeyValueDto>(_mapper.ConfigurationProvider)
          .ToListAsync();
            OnChange?.Invoke();
        }
        finally
        {
            _semaphore.Release();
        }
       
    }
    public async Task Refresh()
    {
        await _semaphore.WaitAsync();
        try
        {
            DataSource = await _context.KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
          .ProjectTo<KeyValueDto>(_mapper.ConfigurationProvider)
          .ToListAsync();
            OnChange?.Invoke();
        }
        finally
        {
            _semaphore.Release();
        }
       
    }
}
