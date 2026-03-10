using NuGet.Packaging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace OnlineTicketsSystem.Helpers
{
    public class TicketPdfDocument
    {
       
        public static byte[] Generate(
           string title,
           string category,
           string city,
           string venue,
           DateTime eventDate,
           int quantity,
           decimal unitPrice,
           decimal totalPrice,
           DateTime? paidAt,
           string verificationCode)
        {
            //QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("Билет за събитие")
                        .FontSize(22)
                        .Bold()
                        .AlignCenter();

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text($"Събитие: {title}").FontSize(16).Bold();
                        col.Item().Text($"Категория: {category}");
                        col.Item().Text($"Град: {city}");
                        col.Item().Text($"Място: {venue}");
                        col.Item().Text($"Дата: {eventDate:dd.MM.yyyy HH:mm}");
                        col.Item().Text($"Количество: {quantity}");
                        col.Item().Text($"Единична цена: {unitPrice:F2} евро.");
                        col.Item().Text($"Обща сума: {totalPrice:F2} евро.");
                        col.Item().Text($"Дата на плащане: {(paidAt.HasValue ? paidAt.Value.ToString("dd.MM.yyyy HH:mm") : "Няма")}");
                        col.Item().Text($"Код за проверка: {verificationCode}")
                            .Bold()
                            .FontSize(14);

                        col.Item().PaddingTop(20).Text("Моля, представете този билет при вход.").Italic();
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(txt =>
                        {
                            txt.Span("Online Tickets System");
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}
