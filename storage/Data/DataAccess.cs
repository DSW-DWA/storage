using System.Data;

namespace storage.Data;

public class DataAccess
{
    const string DataSchemaPath = @"./Data/schema.xml";
    const string DataPath = @"./Data/data.xml";

    readonly DataSet _dataSet;

    public DataAccess()
    {
        _dataSet = new DataSet();
        _dataSet.ReadXmlSchema(DataSchemaPath);
        _dataSet.ReadXml(DataPath);
    }

    public DataSet GetDataSet()
    {
        return _dataSet;
    }

    public static void SaveDataToXml(DataSet dataSet)
    {
        dataSet.WriteXml(DataPath);
    }
}
