using System;

namespace storage.Models;

public class Invoice
{
    public long Id { get; init; }
    public DateTime CreatedAt { get; init; }

    public Invoice(long id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }
}
