using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public static class CreateStock
    {
        static CreateStock()
        {

        }

        public static bool Create(string stock, string sheet, string sheetsample)
        {
            if (!System.IO.File.Exists(stock))
                return false;

            Dictionary<string, double> allmeters = new Dictionary<string, double>();

            if(System.IO.File.Exists(sheet))
            {
                ExcelEdit ee0 = new ExcelEdit();
                ee0.Open(sheet);
                Microsoft.Office.Interop.Excel.Worksheet ws = ee0.GetSheet("Sheet1");
                int index = 2;
                while (true)
                {
                    object id = ee0.GetCellValue(ws, index, 2);
                    if (id == null)
                        break;
                    string sid = id as string;
                    if (sid == "")
                        break;
                    object am = ee0.GetCellValue(ws, index, 5);
                    if (am == null)
                        break;
                    double dam = 0.0;
                    if (!double.TryParse(am.ToString(), out dam))
                        break;
                    if (allmeters.ContainsKey(sid))
                        allmeters[sid] += dam;
                    else
                        allmeters[sid] = dam;
                    index++;
                }
                ee0.Close();
            }

            if(System.IO.File.Exists(sheetsample))
            {
                ExcelEdit ee1 = new ExcelEdit();
                ee1.Open(sheetsample);
                Microsoft.Office.Interop.Excel.Worksheet ws1 = ee1.GetSheet("Sheet1");
                int index = 2;
                while (true)
                {
                    object id = ee1.GetCellValue(ws1, index, 1);
                    if (id == null)
                        break;
                    string sid = id as string;
                    if (sid == "")
                        break;
                    object am = ee1.GetCellValue(ws1, index, 4);
                    if (am == null)
                        break;
                    double dam = 0.0;
                    if (!double.TryParse(am.ToString(), out dam))
                        break;
                    if (allmeters.ContainsKey(sid))
                        allmeters[sid] += dam;
                    else
                        allmeters[sid] = dam;
                    index++;
                }
                ee1.Close();
            }

            {
                ExcelEdit ed2 = new ExcelEdit();
                ed2.Open(stock);
                Microsoft.Office.Interop.Excel.Worksheet ws2 = ed2.GetSheet("总统计");    
                int index = 2;
                while (true)
                {
                    object id = ed2.GetCellValue(ws2, index, 1);
                    if (id == null)
                        break;
                    string sid = id as string;
                    if (sid == "")
                        break;
                    object am = ed2.GetCellValue(ws2, index, 2);
                    if (am == null)
                        break;
                    double dam = 0.0;
                    if (!double.TryParse(am.ToString(), out dam))
                        break;
                    object jjm = ed2.GetCellValue(ws2, index, 3);
                    if (jjm == null)
                        break;
                    double djjm = 0.0;
                    if (!double.TryParse(jjm.ToString(), out djjm))
                        break;
                    if (allmeters.ContainsKey(sid))
                    {
                        double m = dam - allmeters[sid];
                        ed2.SetCellValue(ws2, index, 2, m);
                        dam = m;
                    }

                    if (dam < djjm)
                        ed2.SetCellColor(ws2, index, 2, 3);
                    else
                        ed2.SetCellColor(ws2, index, 2, 2);
                    index++;
                }
                ed2.SaveAs(ed2.mFilename);
                ed2.Close();
            }
            return true;
        }
    }
}
