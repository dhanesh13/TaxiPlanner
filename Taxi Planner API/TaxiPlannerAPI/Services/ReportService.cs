using Antlr.Runtime.Tree;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TaxiPlannerAPI.Data;

namespace TaxiPlannerAPI.Services
{
    public class ReportService
    {
        public  void GenerateReport1(List<Transport> transports)
        {
            string path = getPath(1);

            int row = 3;
            string sheetName = DateTime.Today.ToString("dd-MMMM-yyyy");
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.AddWorksheet(sheetName);
                var ws = wb.Worksheet(sheetName);

                ws.Cell("A1").Value = "Date";
                ws.Cell("A1").Style.Font.Bold = true;
                ws.Cell("B1").Value = DateTime.Today;
                ws.Cell("B1").Style.NumberFormat.Format = "dd MMMM yyyy";

                ws.Cell("A2").Value = "Time";
                ws.Cell("B2").Value = "Van";
                ws.Cell("C2").Value = "Region";
                ws.Cell("D2").Value = "Subregions";
                ws.Cell("E2").Value = "Passengers Count";

                ws.Cells("A2:E2").Style.Font.Bold = true;
                ws.Cells("A2:E2").Style.Fill.BackgroundColor = XLColor.DarkOrange;
                ws.Column("D").Width = 50;
                ws.Column("C").Width = 20;
                ws.Column("E").Width = 20;

                int tc = 1;
                DateTime ts = DateTime.Today;

                foreach (Transport transport in transports)
                {
                    ws.Cell("B" + row).Value = tc++;
                    ws.Cell("C" + row).Value = transport.region;

                    string sr = transport.subregions.Aggregate((i, j) => i + ", " + j);
                    ws.Cell("D" + row).Value = sr;
                    ws.Cell("E" + row).Value = transport.passengers_count;

                    if (ts != transport.timestamp)
                    {
                        ws.Cell("A" + row).Value = transport.timestamp;
                        ws.Cells("A" + row).Style.Font.Bold = true;
                        ws.Cell("A" + row).Style.NumberFormat.Format = "HH:mm";
                        //row++;
                        ts = transport.timestamp;
                    }
                    row++;
                }

                wb.SaveAs(path);
            }
        }

        public  void GenerateReport2(List<Transport> transports)
        {
            string path = getPath(2);

            int row = 2;
            string sheetName = DateTime.Today.ToString("dd-MMMM-yyyy");

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.AddWorksheet(sheetName);
                var ws = wb.Worksheet(sheetName);

                ws.Cell("A1").Value = "Time";
                ws.Cell("B1").Value = "Region";
                ws.Cell("C1").Value = "Passenger";
                ws.Cell("D1").Value = "Address";

                ws.Column("B").Width = 20;
                ws.Column("C").Width = 35;
                ws.Column("D").Width = 35;

                ws.Cells("A1:D1").Style.Font.Bold = true;
                ws.Cells("A1:D1").Style.Fill.BackgroundColor = XLColor.DarkOrange;

                foreach (Transport transport in transports)
                {

                    ws.Cell("A" + row).Value = transport.timestamp;
                    ws.Cells("A" + row).Style.Font.Bold = true;
                    ws.Cell("A" + row).Style.NumberFormat.Format = "HH:mm";
                    ws.Cell("B" + row).Value = transport.region;
                    ws.Cells("B" + row).Style.Font.Bold = true;


                    foreach (EmployeeDTO passenger in transport.passengers)
                    {
                        ws.Cell("C" + row).Value = passenger.user_name;
                        ws.Cell("D" + row++).Value = passenger.sub_regions;
                    }

                    row++;
                }

                wb.SaveAs(path);
            }
        }

        public static string getFileName(int reportNum)
        {
            string reportname = "";
            switch (reportNum)
            {
                case 1:
                    reportname = "sdworx Regions Report";
                    break;

                case 2:
                    reportname = "sdworx Passengers Report";
                    break;

                default:
                    break;
            }
            string date = DateTime.Today.ToString("dd-MMM-yyyy");
            reportname += " " + date + ".xlsx";
            return reportname;
        }

        public static string getPath(int reportNum)
        {
            // TODO: use get filename
            string reportname = "";
            switch (reportNum)
            {
                case 1:
                    reportname = "sdworx Regions Report";
                    break;

                case 2:
                    reportname = "sdworx Passengers Report";
                    break;

                default:
                    break;
            }
            string date = DateTime.Today.ToString("dd-MMM-yyyy");
            reportname += " " + date +".xlsx";

            //if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/Reports")))
            if (!Directory.Exists(("C:\\Users\\Ramen Pajani\\Desktop\\taxiplannerapi\\TaxiPlannerAPI\\Reports")))
            {
                Directory.CreateDirectory(("C:\\Users\\Ramen Pajani\\Desktop\\taxiplannerapi\\TaxiPlannerAPI\\Reports"));
            }
            string p = Path.Combine(("C:\\Users\\Ramen Pajani\\Desktop\\taxiplannerapi\\TaxiPlannerAPI\\Reports"), reportname);
            return p;
        }

    }
}