using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF_Demo.Models;
using System.Net;
using System.Text.Json;

namespace QuestPDF_Demo.Services
{
    public static class QuestPdfService
    {
        public static async Task<PdfReportFileInfo> GenerateSample1Pdf()
        {
            // this is just a proof of concept, please feel free to refactor / make the code asynchronous if db/http calls are involved

            try
            {
                // read investment advice data from json file & deserialize into a C# object
                string customerInvestmentData = File.ReadAllText(@"./Assets/customer-investment-data.json");
                var investmentTradeInfo = JsonSerializer.Deserialize<InvestmentTradeInfo>(customerInvestmentData);
                var imageStream = await GetCompanyLogoUrlImage2(investmentTradeInfo.CompanyLogoUrl);

                Document document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial", "Calibri", "Tahoma"));
                        page.Header().Width(1, Unit.Inch).Image(imageStream);

                        page.Content().Column(x =>
                        {
                            x.Item().PaddingVertical((float)0.5, Unit.Centimetre).Text(investmentTradeInfo.DealInfo.TransactionDate);
                            x.Item().Text(investmentTradeInfo.Customer.FullName).FontSize(15).Bold();
                            x.Item().PaddingBottom(1, Unit.Centimetre).Text(investmentTradeInfo.Customer.Address).FontSize(13);

                            x.Item().PaddingBottom((float)0.3, Unit.Centimetre).Text("Dear Sir/Ma,");

                            x.Item().AlignCenter()
                                .Text($"INVESTMENT ADVICE FOR {investmentTradeInfo.DealInfo.Type} - {investmentTradeInfo.DealInfo.Issuer} - REF NO: {investmentTradeInfo.DealInfo.ReferenceNumber}".ToUpper())
                                .FontSize(13)
                                .SemiBold()
                                .Underline();

                            x.Item().PaddingTop((float)0.5, Unit.Centimetre).Text("Please refer to the details of the investment below: ");

                            x.Item().PaddingTop((float)0.5, Unit.Centimetre).Row(row =>
                            {
                                row.RelativeItem().Text("Issuer: ").SemiBold();
                                row.RelativeItem().AlignRight().Text($"{investmentTradeInfo.DealInfo.Issuer}".ToUpper());
                            });

                            x.Item().PaddingTop((float)0.3, Unit.Centimetre).Row(row =>
                            {
                                row.RelativeItem().Text("Investment Type: ").SemiBold();
                                row.RelativeItem().AlignRight().Text($"{investmentTradeInfo.DealInfo.Type}".ToUpper());
                            });

                            x.Item().PaddingTop((float)0.3, Unit.Centimetre).Row(row =>
                            {
                                row.RelativeItem().Text("Currency: ").SemiBold();
                                row.RelativeItem().AlignRight().Text($"{investmentTradeInfo.DealInfo.Currency}".ToUpper());
                            });

                            x.Item().PaddingTop((float)0.3, Unit.Centimetre).Row(row =>
                            {
                                row.RelativeItem().Text("Start Date: ").SemiBold();
                                row.RelativeItem().AlignRight().Text(investmentTradeInfo.DealInfo.StartDate);
                            });

                            x.Item().PaddingTop((float)0.3, Unit.Centimetre).Row(row =>
                            {
                                row.RelativeItem().Text("End Date: ").SemiBold();
                                row.RelativeItem().AlignRight().Text(investmentTradeInfo.DealInfo.EndDate);
                            });

                            x.Item().PaddingTop((float)0.3, Unit.Centimetre).Row(row =>
                            {
                                row.RelativeItem().Text("Duration (days): ").SemiBold();
                                row.RelativeItem().AlignRight().Text(investmentTradeInfo.DealInfo.Duration);
                            });

                            x.Item().PaddingTop((float)0.3, Unit.Centimetre).Row(row =>
                            {
                                row.RelativeItem().Text("Net Rate (% per annum): ").SemiBold();
                                row.RelativeItem().AlignRight().Text(investmentTradeInfo.DealInfo.NetRate);
                            });

                            x.Item().PaddingTop((float)0.3, Unit.Centimetre).Row(row =>
                            {
                                row.RelativeItem().Text("Principal: ").SemiBold();
                                row.RelativeItem().AlignRight().Text($"${investmentTradeInfo.DealInfo.Principal:N2}");
                            });

                            x.Item().PaddingTop((float)0.3, Unit.Centimetre).Row(row =>
                            {
                                row.RelativeItem().Text("Expected Interest: ").SemiBold();
                                row.RelativeItem().AlignRight().Text($"${investmentTradeInfo.DealInfo.ExpectedInterest:N2}");
                            });

                            x.Item().PaddingTop((float)0.3, Unit.Centimetre).Row(row =>
                            {
                                row.RelativeItem().Text("Net Maturity Value: ").SemiBold();
                                row.RelativeItem().AlignRight().Text($"${investmentTradeInfo.DealInfo.NetMaturityValue:N2}");
                            });


                            x.Item().PaddingTop((float)0.8, Unit.Centimetre).Text("Terms & conditions: ").SemiBold();

                            x.Item().PaddingTop((float)0.3, Unit.Centimetre).Row(row =>
                            {
                                row.Spacing(5);
                                row.AutoItem().PaddingLeft(10).Text("•");
                                row.RelativeItem().Text("This investment is offered under the Jane Doe Investment Securities Management Service.");
                            });

                            x.Item().PaddingTop((float)0.5, Unit.Centimetre).Text("We thank you for your valued patronage and continued interest in Jane Doe Investment Securities Limited.");


                            x.Item().PaddingTop(1, Unit.Centimetre).Text("Warm regards,");
                            x.Item().PaddingTop((float)0.3, Unit.Centimetre).Row(row =>
                            {
                                row.Spacing(5);
                                row.AutoItem().Text("For: ").NormalWeight();
                                row.RelativeItem().Text("Jane Doe Investment Securities Limited");
                            });

                        });



                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("THIS IS A SYSTEM GENERATED MAIL, PLEASE DO NOT REPLY TO THIS EMAIL.").FontSize(9);
                            });
                    });
                });
                byte[] pdfBytes = document.GeneratePdf();

                return new PdfReportFileInfo() { 
                    ByteArray = pdfBytes, 
                    FileName = $"Investment_Advice_{investmentTradeInfo.Customer.FullName}_{investmentTradeInfo.DealInfo.ReferenceNumber}.pdf",
                    MimeType = "application/pdf"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private static async Task<Image> GetCompanyLogoUrlImage2(string imagePath)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://asset.brandfetch.io/");
            client.DefaultRequestHeaders.Accept.Clear();
            var imageStream = await client.GetStreamAsync(imagePath);
            return Image.FromStream(imageStream);
        }
    }
}
