using System.Collections.Generic;
using storage.Models;

namespace storage.Data;

public class Root
{
    public List<Category> Category { get; set; }
    public List<Material> Material { get; set; }
    public List<Invoice> Invoice { get; set; }
    public List<MaterialConsumption> MaterialConsumption { get; set; }
    public List<MaterialReceipt> MaterialReceipt { get; set; }
}