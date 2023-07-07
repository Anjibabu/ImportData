using OfficeOpenXml;
using System.Data;
using System.Reflection;
//install EPPlus from nuget
namespace ImportBulkDataFromExcel
{
    public class ExcelData
    {
        public string? ApplicationCode { get; set; }
        public string? TpDealerId { get; set; }
        public string? TpDealerName { get; set; }
        public string? TpDealerAddress { get; set; }
        public string? TpDealerState { get; set; }
        public string? TpDealerZip { get; set; }
        public string? TpDealerPhone { get; set; }
        public string? TpDealerEmail { get; set; }
        public string? ShfDealerId { get; set; }
        public string? ShfDealerName { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedOn { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started..");
            string outputFilePath = @"D:\\sqlscript.txt";
            var data = ReadExcelToList(@"D:\\test.xlsx");

            string sqlScript = GenerateSqlScript("testdata", ConvertListToDataTable(data));
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                string[] lines = sqlScript.Split('\n');
                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }
            }
            Console.WriteLine(sqlScript);
            Console.WriteLine("Done..");
            //Console.ReadLine();
        }
        public static List<ExcelData> ReadExcelToList(string filePath)
        {
            List<ExcelData> dataList = new List<ExcelData>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming you want to read the first worksheet

                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                for (int row = 2; row <= rowCount; row++) // Start from row 2 to skip headers
                {
                    ExcelData data = new ExcelData();

                    for (int col = 1; col <= colCount; col++)
                    {
                        string value = worksheet.Cells[row, col].Value?.ToString();

                        // Assign the value to the corresponding property based on the column index
                        switch (col)
                        {
                            case 1:
                                data.ApplicationCode = value;
                                break;
                            case 2:
                                data.TpDealerId = value;
                                break;
                            case 3:
                                data.TpDealerName = value;
                                break;
                            case 4:
                                data.TpDealerAddress = value;
                                break;
                            case 5:
                                data.TpDealerState = value;
                                break;
                            case 6:
                                data.TpDealerZip = value;
                                break;
                            case 7:
                                data.TpDealerPhone = value;
                                break;
                            case 8:
                                data.TpDealerEmail = value;
                                break;
                            case 9:
                                data.ShfDealerId = value;
                                break;
                            case 10:
                                data.ShfDealerName = value;
                                break;
                            case 11:
                                data.CreatedBy = value;
                                break;
                            case 12:
                                data.CreatedOn = value;
                                break;
                            case 13:
                                data.UpdatedBy = value;
                                break;
                            case 14:
                                data.UpdatedOn = value;
                                break;

                        }

                    }

                    dataList.Add(data);
                }
            }

            return dataList;
        }
        private static string GenerateSqlScript(string tableName, DataTable dt)
        {
            string sqlScript = "";
            string createTableVariable = $"DECLARE @{tableName} TABLE (\r\n    ApplicationCode VARCHAR(200),\r\n    TpDealerId VARCHAR(200),\r\n    TpDealerName VARCHAR(200),\r\n    TpDealerAddress VARCHAR(200),\r\n    TpDealerState VARCHAR(200),\r\n    TpDealerZip VARCHAR(200),\r\n    TpDealerPhone VARCHAR(200),\r\n    TpDealerEmail VARCHAR(200),\r\n    ShfDealerId VARCHAR(200),\r\n    ShfDealerName VARCHAR(200),\r\n    CreatedBy VARCHAR(200),\r\n    CreatedOn VARCHAR(200),\r\n    UpdatedBy VARCHAR(200),\r\n    UpdatedOn VARCHAR(200));";
            sqlScript += createTableVariable;
            sqlScript += "\n\n";
            foreach (DataRow row in dt.Rows)
            {
                string values = string.Join(", ", row.ItemArray.Select(item => $"'{item}'"));
                string insertStatement = $"INSERT INTO @{tableName} VALUES ({values});\n";
                sqlScript += insertStatement;
            }
            sqlScript += $"\n\nSELECT * FROM  @{tableName}\n";
            return sqlScript;
        }


        public static DataTable ConvertListToDataTable<T>(List<T> list)
        {
            DataTable dataTable = new DataTable();

            // Get the properties of the type T
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Create columns in the DataTable based on the properties
            foreach (PropertyInfo property in properties)
            {
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            // Add data to the DataTable
            foreach (T item in list)
            {
                DataRow dataRow = dataTable.NewRow();

                foreach (PropertyInfo property in properties)
                {
                    dataRow[property.Name] = property.GetValue(item) ?? DBNull.Value;
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }


    }
}
