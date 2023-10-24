using System.Data;
using System.Data.SQLite;

namespace storage.Data;

public class DataAccess
{
    const string DataSchemaPath = @"./Data/schema.xml";
    const string DataPath = @"./Data/data.xml";

    readonly DataSet _dataSet;

    public DataAccess(DataSet ds)
    {
        _dataSet = ds;
        /*_dataSet.ReadXmlSchema(DataSchemaPath);
        _dataSet.ReadXml(DataPath);*/
    }

    public DataSet GetDataSet()
    {
        return _dataSet;
    }

    public static void SaveDataToXml(DataSet dataSet)
    {
        //dataSet.WriteXml(DataPath);

        var ds = new DataSet();

        var connectionString = "Data Source=storage.db;Foreign Keys=True;Mode=ReadWrite";
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter())
            using (SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(adapter))
            {

                adapter.SelectCommand = new SQLiteCommand("SELECT * FROM Category", connection);
                adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                adapter.InsertCommand = commandBuilder.GetInsertCommand();
                adapter.DeleteCommand = commandBuilder.GetDeleteCommand();
                adapter.Update(dataSet, "Category");
            }

            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter())
            using (SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(adapter))
            {
                adapter.SelectCommand = new SQLiteCommand("SELECT * FROM Material", connection);
                adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                adapter.InsertCommand = commandBuilder.GetInsertCommand();
                adapter.DeleteCommand = commandBuilder.GetDeleteCommand();
                adapter.Update(dataSet, "Material");
            }

            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter())
            using (SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(adapter))
            {

                adapter.SelectCommand = new SQLiteCommand("SELECT * FROM Invoice", connection);
                adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                adapter.InsertCommand = commandBuilder.GetInsertCommand();
                adapter.DeleteCommand = commandBuilder.GetDeleteCommand();
                adapter.Update(dataSet, "Invoice");
            }

            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter())
            using (SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(adapter))
            {
                adapter.SelectCommand = new SQLiteCommand("SELECT * FROM MaterialConsumption", connection);
                adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                adapter.InsertCommand = commandBuilder.GetInsertCommand();
                adapter.DeleteCommand = commandBuilder.GetDeleteCommand();
                adapter.Update(dataSet, "MaterialConsumption");
            }

            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter())
            using (SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(adapter))
            {
                adapter.SelectCommand = new SQLiteCommand("SELECT * FROM MaterialReceipt", connection);
                adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                adapter.InsertCommand = commandBuilder.GetInsertCommand();
                adapter.DeleteCommand = commandBuilder.GetDeleteCommand();
                adapter.Update(dataSet, "MaterialReceipt");
            }
        }
    }
}
