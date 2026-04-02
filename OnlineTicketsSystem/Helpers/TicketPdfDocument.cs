//using NuGet.Packaging;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;


//namespace OnlineTicketsSystem.Helpers
//{
//    public class TicketPdfDocument
//    {

//        public static byte[] Generate(
//           string title,
//           string category,
//           string city,
//           string venue,
//           DateTime eventDate,
//           int quantity,
//           decimal unitPrice,
//           decimal totalPrice,
//           DateTime? paidAt,
//           string verificationCode)
//        {
//            //QuestPDF.Settings.License = LicenseType.Community;
//            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
//            var document = Document.Create(container =>
//            {
//                container.Page(page =>
//                {
//                    page.Margin(30);

//                    page.Header()
//                        .Text("Билет за събитие")
//                        .FontSize(22)
//                        .Bold()
//                        .AlignCenter();

//                    page.Content().Column(col =>
//                    {
//                        col.Spacing(10);

//                        col.Item().Text($"Събитие: {title}").FontSize(16).Bold();
//                        col.Item().Text($"Категория: {category}");
//                        col.Item().Text($"Град: {city}");
//                        col.Item().Text($"Място: {venue}");
//                        col.Item().Text($"Дата: {eventDate:dd.MM.yyyy HH:mm}");
//                        col.Item().Text($"Количество: {quantity}");
//                        col.Item().Text($"Единична цена: {unitPrice:F2} евро.");
//                        col.Item().Text($"Обща сума: {totalPrice:F2} евро.");
//                        col.Item().Text($"Дата на плащане: {(paidAt.HasValue ? paidAt.Value.ToString("dd.MM.yyyy HH:mm") : "Няма")}");
//                        col.Item().Text($"Код за проверка: {verificationCode}")
//                            .Bold()
//                            .FontSize(14);

//                        col.Item().PaddingTop(20).Text("Моля, представете този билет при вход.").Italic();
//                    });

//                    page.Footer()
//                        .AlignCenter()
//                        .Text(txt =>
//                        {
//                            txt.Span("Online Tickets System");
//                        });
//                });
//            });

//            return document.GeneratePdf();
//        }
//    }
//}
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
            QuestPDF.Settings.License = LicenseType.Community;

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

                        col.Item().Row(row =>
                        {
                            // Лявата част – текст
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text($"Събитие: {title}").FontSize(16).Bold();
                                left.Item().Text($"Категория: {category}");
                                left.Item().Text($"Град: {city}");
                                left.Item().Text($"Място: {venue}");
                                left.Item().Text($"Дата: {eventDate:dd.MM.yyyy HH:mm}");
                                left.Item().Text($"Количество: {quantity}");
                                left.Item().Text($"Единична цена: {unitPrice:F2} евро.");
                                left.Item().Text($"Обща сума: {totalPrice:F2} евро.");
                                left.Item().Text($"Дата на плащане: {(paidAt.HasValue ? paidAt.Value.ToString("dd.MM.yyyy HH:mm") : "Няма")}");
                                left.Item().Text($"Код за проверка: {verificationCode}")
                                    .Bold()
                                    .FontSize(14);

                                left.Item().PaddingTop(20).Text("Моля, представете този билет при вход.").Italic();
                            });

                            // Дясната част – малък QR код
                            row.ConstantItem(90).AlignRight().AlignTop().Image("wwwroot/images/qr-heart.png");
                                
                        });
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("Online Tickets System");
                });
            });

            return document.GeneratePdf();
        }
    }
}

