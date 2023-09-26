using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using storage.Models;
using Xceed.Document.NET;

namespace storage.Data
{
    public class DataAccess
    {
        const string DataSchemaPath = @"./Data/schema.xml";
        const string DataPath = @"./Data/data.xml";

        internal DataSet Ds { get; set; }

        protected DataAccess()
        {
            Ds = new DataSet();
            Ds.ReadXmlSchema(DataSchemaPath);
            Ds.ReadXml(DataPath);
        }

        internal void SaveDataToXml()
        {
            Ds.WriteXml(DataPath);
        }
    }
}
