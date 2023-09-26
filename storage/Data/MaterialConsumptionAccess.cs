using System.Collections.Generic;
using System.Data;
using storage.Models;
namespace storage.Data;

public class MaterialConsumptionAccess : DataAccess
{
    public void CreateMaterialConsumption(MaterialConsumption materialConsumption)
        {
            DataRow? newRow = Ds.Tables["MaterialConsumption"]?.NewRow();
            if (newRow != null)
            {
                newRow["Id"] = materialConsumption.Id;
                newRow["Count"] = materialConsumption.Count;
                newRow["InvoiceId"] = materialConsumption.InvoiceId;
                newRow["MaterialId"] = materialConsumption.MaterialId;
                Ds.Tables["MaterialConsumption"]?.Rows.Add(newRow);
            }
            SaveDataToXml();
        }

        public List<MaterialConsumption> GetAllMaterialConsumptions()
        {
            var materialConsumptions = new List<MaterialConsumption>();

            var dataRowCollection = Ds.Tables["MaterialConsumption"]?.Rows;
            if (dataRowCollection == null)
                return materialConsumptions;
            foreach (DataRow row in dataRowCollection)
            {
                materialConsumptions.Add(new MaterialConsumption
                {
                    Id = (long)row["Id"],
                    Count = (long)row["Count"],
                    InvoiceId = (long)row["InvoiceId"],
                    MaterialId = (long)row["MaterialId"]
                });
            }

            return materialConsumptions;
        }

        public void UpdateMaterialConsumption(MaterialConsumption updatedMaterialConsumption)
        {
            var rows = Ds.Tables["MaterialConsumption"]?.Select($"Id = {updatedMaterialConsumption.Id}");
            if (rows is { Length: <= 0 })
                return;
            var row = rows?[0];
            if (row != null)
            {
                row["Count"] = updatedMaterialConsumption.Count;
                row["InvoiceId"] = updatedMaterialConsumption.InvoiceId;
                row["MaterialId"] = updatedMaterialConsumption.MaterialId;
            }
            SaveDataToXml();
        }

        public void DeleteMaterialConsumption(long materialConsumptionId)
        {
            var rows = Ds.Tables["MaterialConsumption"]?.Select($"Id = {materialConsumptionId}");
            if (rows != null)
                foreach (var row in rows)
                {
                    Ds.Tables["MaterialConsumption"]?.Rows.Remove(row);
                }
            SaveDataToXml();
        }
}
