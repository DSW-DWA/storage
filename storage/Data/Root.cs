using System.Collections.Generic;

namespace storage.Data;

public class Root
{
    public List<Category> category { get; set; }
    public List<Material> material { get; set; }
    public List<Invoice> invoice { get; set; }
    public List<MaterialConsumption> material_consumption { get; set; }
    public List<MaterialReceipt> material_receipt { get; set; }
}