using System;
using System.Data;
using System.IO;
using System.Linq;

namespace Interoperability
{
    class Program
    {
        static void Main(string[] args)
        {
            var dt = new System.Data.DataTable();
            dt.Columns.AddRange(new int[5] { 1, 2, 3, 4, 5 }.Select(x => new DataColumn("column" + x.ToString())).ToArray());
            for (int i = 0; i < 10; i++)
            {
                var r = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    r[j] = "row" + i.ToString() + j.ToString();
                }
                dt.Rows.Add(r);
            }
            Console.WriteLine("Hello World!");




            var excelApp = OfficeOpenXML.GetInstance();
            using (var stream = excelApp.GetExcelStream(dt, false))
            {

                using (FileStream fs = new FileStream(@"C:\Users\Public\myexcel.xlsx", FileMode.Create))
                {
                    stream.CopyTo(fs);
                    fs.Flush();
                }
            }
        }
    }
}
