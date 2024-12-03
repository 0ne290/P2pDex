using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Storage.Models;

[PrimaryKey(nameof(Guid))]
public abstract class ModelBase
{
    public string Guid { get; init; }
}