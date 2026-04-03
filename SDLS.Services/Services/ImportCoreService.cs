using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SDLS.Services.Interfaces;
using System.Text;

namespace SDLS.Services.Services
{
    public class ImportCoreService : IImportCoreService
    {
        public async Task<byte[]> BuildTemplateAsync(
            IReadOnlyList<string> headers,
            IReadOnlyList<string> sample,
            string format = "xlsx",
            string sheetName = "Template")
        {
            if (headers == null || headers.Count == 0)
                throw new ArgumentException("Headers không h?p l?.");

            if (sample == null || sample.Count != headers.Count)
                throw new ArgumentException("Sample ph?i có s? c?t b?ng headers.");

            if (string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase))
            {
                var sb = new StringBuilder();
                sb.AppendLine(string.Join(',', headers));
                sb.AppendLine(string.Join(',', sample.Select(EscapeCsv)));
                return Encoding.UTF8.GetBytes(sb.ToString());
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(string.IsNullOrWhiteSpace(sheetName) ? "Template" : sheetName);

            for (var i = 0; i < headers.Count; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
                ws.Cell(1, i + 1).Style.Font.Bold = true;
                ws.Cell(2, i + 1).Value = sample[i];
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public async Task<List<Dictionary<string, string>>> ReadRowsAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không h?p l?.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".csv" && ext != ".xlsx")
                throw new ArgumentException("Ch? h? tr? file CSV ho?c XLSX.");

            using var stream = file.OpenReadStream();
            return ext == ".csv"
                ? ReadCsvRows(stream)
                : ReadXlsxRows(stream);
        }

        private static string EscapeCsv(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";

            return value;
        }

        private static List<Dictionary<string, string>> ReadCsvRows(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var lines = new List<string>();
            while (!reader.EndOfStream)
                lines.Add(reader.ReadLine() ?? string.Empty);

            if (lines.Count < 2)
                return new List<Dictionary<string, string>>();

            var headers = ParseCsvLine(lines[0]);
            var rows = new List<Dictionary<string, string>>();

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var values = ParseCsvLine(line);
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < headers.Count; i++)
                    dict[headers[i]] = i < values.Count ? values[i] : string.Empty;

                rows.Add(dict);
            }

            return rows;
        }

        private static List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            result.Add(sb.ToString());
            return result;
        }

        private static List<Dictionary<string, string>> ReadXlsxRows(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            var ws = workbook.Worksheets.First();
            var range = ws.RangeUsed();
            if (range == null)
                return new List<Dictionary<string, string>>();

            var headers = range.Row(1).Cells().Select(c => c.GetString()).ToList();
            var rows = new List<Dictionary<string, string>>();

            foreach (var row in range.RowsUsed().Skip(1))
            {
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < headers.Count; i++)
                    dict[headers[i]] = row.Cell(i + 1).GetString();

                if (dict.Values.All(v => string.IsNullOrWhiteSpace(v)))
                    continue;

                rows.Add(dict);
            }

            return rows;
        }
    }
}
