using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Subs4.Common.Classes;
using Subs4.Common.Helpers;

namespace Subs4.ReportLib
{
    public static class ReportCreator
    {
        private static readonly Dictionary<string, string> Services =
            new Dictionary<string, string>
            {
                //{"03", "Содержание"},
                {"01", "Содержание"},
                {"10", "Отопление"},
                {"11", "Горячая вода"},
                {"12", "Холодная вода"},
                {"13", "Канализация"},
                {"22", "Газ"}
            };

        private static Font _font;
        private static Font Font
        {
            get
            {
                if (_font == null)
                {
                    var arialuniTff = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arialuni.ttf");
                    var baseFont = BaseFont.CreateFont(arialuniTff, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    _font = new Font(baseFont, 10, Font.NORMAL);
                }
                return _font;
            }
        }

        private static Font BoldFont
        {
            get
            {
                var font = new Font(Font);
                font.SetStyle(Font.BOLD);
                return font;
            }
        }
        
        private static void AddCell(
            this PdfPTable table, 
            string text, 
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
            int rowspan = 1,
            bool isBold = false)
        {
            var font = isBold ? BoldFont : Font;
            var cell = new PdfPCell(new Paragraph(text, font))
                       {
                           HorizontalAlignment = (int)horizontalAlignment,
                           VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                           Rowspan = rowspan
                       };
            table.AddCell(cell);
        }

        public static void CreateReport(IEnumerable<Person> persons, string filename)
        {
            var doc = new Document(PageSize.A4);
            doc.SetMargins(doc.LeftMargin, doc.RightMargin, 12f, 12f);

            PdfWriter.GetInstance(doc, File.Create(filename));

            doc.Open();

            var table = CreateTable(persons);

            doc.Add(table);

            doc.Close();
        }

        private static PdfPTable CreateTable(IEnumerable<Person> persons)
        {
            var columns = (
                from p in persons
                from b in p.Benefits
                select b.ServiceGroupCode
                ).Distinct()
                 .OrderBy(x => x)
                 .ToList();

            var table = new PdfPTable(columns.Count + 4);
            table.WidthPercentage = 100f;
            var colWidths = new[] {5f, 20f, 10f}.Concat(columns.Select(_ => 55f/columns.Count)).Append(10f).ToArray();
            table.SetWidths(colWidths);

            AddHeader(table, columns);

            AddBody(table, persons, columns);

            AddSummary(table, persons, columns);

            return table;
        }

        private static void AddHeader(PdfPTable table, IEnumerable<string> serviceGroupCodes)
        {
            table.AddCell("#", HorizontalAlignment.Center, isBold:true);
            table.AddCell("ФИО", HorizontalAlignment.Center, isBold: true);
            table.AddCell("Категория", HorizontalAlignment.Center, isBold: true);

            foreach (var serviceGroupCode in serviceGroupCodes)
            {
                table.AddCell(Services[serviceGroupCode], HorizontalAlignment.Center, isBold: true);
            }

            table.AddCell("Сумма", HorizontalAlignment.Center, isBold: true);
        }

        private static void AddBody(PdfPTable table, IEnumerable<Person> persons, IEnumerable<string> columns)
        {
            var c = 0;
            foreach (var person in persons)
            {
                c++;
                if (person.Categories.Count() == 1)
                {
                    table.AddCell(c.ToString(), HorizontalAlignment.Center);
                    table.AddCell(person.LastNameWithInitials, HorizontalAlignment.Left);
                    table.AddCell(person.Categories.First(x => x.IsMain).Code, HorizontalAlignment.Center);

                    foreach (var column in columns)
                    {
                        var benefit = person.Benefits.FirstOrDefault(x => x.ServiceGroupCode == column);
                        if (benefit != null)
                        {
                            table.AddCell(benefit.Value.ToString("0.00"), HorizontalAlignment.Right);
                        }
                        else
                        {
                            table.AddCell("", HorizontalAlignment.Center);
                        }
                    }

                    table.AddCell(person.Benefits.Select(x=>x.Value).Sum().ToString("0.00"), HorizontalAlignment.Right);
                }
                else if (person.Categories.Count() == 2)
                {
                    table.AddCell(c.ToString(), HorizontalAlignment.Center, 2);
                    table.AddCell(person.LastNameWithInitials, HorizontalAlignment.Left, 2);

                    foreach (var category in person.Categories)
                    {
                        table.AddCell(category.Code, HorizontalAlignment.Center);

                        foreach (var column in columns)
                        {
                            var benefit = person.Benefits.FirstOrDefault(x => x.ServiceGroupCode == column && x.CategoryCode == category.Code);
                            if (benefit != null)
                            {
                                table.AddCell(benefit.Value.ToString("0.00"), HorizontalAlignment.Right);
                            }
                            else
                            {
                                table.AddCell("", HorizontalAlignment.Center);
                            }
                        }

                        table.AddCell(person.Benefits.Where(x => x.CategoryCode == category.Code).Select(x => x.Value).Sum().ToString("0.00"), HorizontalAlignment.Right);
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        private static void AddSummary(PdfPTable table, IEnumerable<Person> persons, IEnumerable<string> columns)
        {
            table.AddCell("", HorizontalAlignment.Center, isBold: true);
            table.AddCell("Итого", HorizontalAlignment.Center, isBold: true);
            table.AddCell("", HorizontalAlignment.Center, isBold: true);

            var cols = from col in columns
                       let bs = from p in persons
                                from b in p.Benefits
                                where b.ServiceGroupCode == col
                                select b.Value
                       select bs.Sum();

            foreach (var col in cols)
            {
                table.AddCell(col.ToString("0.00"), HorizontalAlignment.Right, isBold: true);
            }

            var totalSum = persons.SelectMany(x => x.Benefits).Select(x => x.Value).Sum();

            table.AddCell(totalSum.ToString("0.00"), HorizontalAlignment.Right, isBold: true);
        }
    }
}
