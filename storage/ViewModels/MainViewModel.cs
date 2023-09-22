using System.Data;
using ReactiveUI;
using System.IO;
using Newtonsoft.Json;

namespace storage.ViewModels;

public class MainViewModel : ReactiveObject
{
    public DataSet Ds;

    public MainViewModel()
    {
        InitializeDataSet();
    }

    private void InitializeDataSet()
    {
        var json = File.ReadAllText(@"./Data/data.json");
        
        Ds = JsonConvert.DeserializeObject<DataSet>(json);

        Ds.Relations.Add(Ds.Tables["category"].Columns["id"], Ds.Tables["material"].Columns["category_id"]);
        Ds.Relations.Add(Ds.Tables["material"].Columns["id"], Ds.Tables["material_consumption"].Columns["material_id"]);
        Ds.Relations.Add(Ds.Tables["material"].Columns["id"], Ds.Tables["material_receipt"].Columns["material_id"]);
        Ds.Relations.Add(Ds.Tables["invoice"].Columns["id"], Ds.Tables["material_consumption"].Columns["invoice_id"]);
        Ds.Relations.Add(Ds.Tables["invoice"].Columns["id"], Ds.Tables["material_receipt"].Columns["invoice_id"]);
    }
}
