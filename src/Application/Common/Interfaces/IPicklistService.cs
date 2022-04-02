using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;
public interface IPicklistService
{
    List<KeyValueDto> DataSource { get; } 
    event Action? OnChange;
    Task Initialize();
    Task Refresh();
}
