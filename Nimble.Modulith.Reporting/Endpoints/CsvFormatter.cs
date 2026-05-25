using System.Text;

namespace Nimble.Modulith.Reporting.Endpoints;

public static class CsvFormatter
{
    public static string ToCsv<T>(IEnumerable<T> data)
    {
        if (data == null || !data.Any()) return string.Empty;

        var sb = new StringBuilder();
        var properties = typeof(T).GetProperties();

        sb.AppendLine(string.Join(",", properties.Select(p => EscapeField(p.Name))));

        foreach (var item in data)
        {
            var row = properties.Select(p => {
                var value = p.GetValue(item, null);
                return EscapeField(value?.ToString() ?? string.Empty);
            });
            sb.AppendLine(string.Join(",", row));
        }

        return sb.ToString();
    }

    private static string EscapeField(string field)
    {
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
}