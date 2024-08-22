using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF_Demo.Models;
using System.Net;
using System.Reflection;
using System.Text.Json;
using static QuestPDF.Helpers.Colors;

namespace QuestPDF_Demo.Services
{
    public static class QuestPdfService
    {
        public static Image _medicalLogo1 { get; } = Image.FromFile("Assets/medical-logo.PNG");
        public static Image _medicalLogo2 { get; } = Image.FromFile("Assets/medical-logo2.PNG");

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

        public static byte[] GeneratePharmacyReportBytes()
        {
            byte[] reportBytes;

            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0.8f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Tahoma", "Arial", ""));

                    page.Content().Border(1);
                    page.Content().Element(ComposePharmacyReportContent);
                    page.Footer().Element(ComposeFooterContent);
                });
            });

            reportBytes = document.GeneratePdf();

            return reportBytes;
        }

        static void ComposeFooterContent(IContainer container)
        {
            container.Column(footer =>
            {
                footer.Item().Row(row =>
                {
                    row.AutoItem().Text($"{DateTime.Today.ToString("ddd MMM dd, yyyy")}").FontSize(8);
                    row.RelativeItem().AlignCenter().Text($"ANY ALTERATION ON THIS INCOME REPORT SHEET RENDERS IT INVALID").FontSize(8);
                    row.AutoItem().AlignRight().Text(text =>
                    {
                        text.Span("Page ").FontSize(8);
                        text.CurrentPageNumber().FontSize(8);
                        text.Span(" of ").FontSize(8);
                        text.TotalPages().FontSize(8);
                    });
                });
            });
        }

        static IContainer DefaultCellStyle(IContainer container, string backgroundColor = "")
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .Background(!string.IsNullOrEmpty(backgroundColor) ? backgroundColor : Colors.White)
                .PaddingVertical(7)
                .PaddingHorizontal(3);
        }

        static IContainer EvaluateOutstandingBalanceBackgroundColor(IContainer container, double amountDue, double amountPaid)
        {
            if (amountDue - amountPaid > 0)
            {
                // this applies a grey background to the outstanding balance cell 
                return container
                .PaddingVertical(7)
                .PaddingHorizontal(3)
                .Background(Colors.Grey.Lighten2);
            }

            return DefaultCellStyle(container);
        }

        static void ComposePharmacyReportContent(IContainer container)
        {

            var transactions = GetPharmacyReport();
            int serialNumber = 0;

            

            container.Column(mainContentColumn =>
            {
                mainContentColumn.Item().Row(row =>
                {
                    row.AutoItem().Column(column =>
                    {
                        column.Item().Width(1, Unit.Inch).Image(_medicalLogo1);
                    });

                    row.RelativeItem().AlignCenter().Column(column =>
                    {
                        column
                            .Item().Text("MEDIPLUS DIAGNOSTIC CENTRE")
                            .FontSize(20).SemiBold();

                        column
                            .Item().AlignCenter().PaddingBottom(0.5f, Unit.Centimetre).Text("Lagos, Nigeria.")
                            .FontSize(13).SemiBold();

                        column
                            .Item().AlignCenter().Text("PHARMACY INCOME REPORT").Underline()
                            .FontSize(16);
                    });

                    row.AutoItem().AlignRight().Column(column =>
                    {
                        column.Item().Width(1, Unit.Inch).Image(_medicalLogo2);
                    });
                });


               mainContentColumn.Item().PaddingTop(0.8f, Unit.Centimetre).Row(row =>
               {
                   row.RelativeItem().Shrink().Border(1).Table(table =>
                   {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(90);
                            columns.ConstantColumn(90);
                            columns.ConstantColumn(100);
                            columns.ConstantColumn(200);
                        });

                        // please be sure to call the 'header' handler!
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).AlignCenter().Text("S/N").FontSize(9).SemiBold();
                            header.Cell().Element(CellStyle).Text("Customer ID").FontSize(9).SemiBold();
                            header.Cell().Element(CellStyle).Text("Customer Name").FontSize(9).SemiBold();
                            header.Cell().Element(CellStyle).Text("Transaction Date").FontSize(9).SemiBold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Amount Due (₦)").FontSize(9).SemiBold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Amount Paid (₦)").FontSize(9).SemiBold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Outstanding Bal.(₦)").FontSize(9).SemiBold();
                            header.Cell().Element(CellStyle).Text("Remarks").FontSize(9).SemiBold();

                            // you can extend existing styles by creating additional methods
                            IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.Grey.Lighten3);
                        });

                        var summaryEntry = transactions.LastOrDefault();
                        foreach (var transaction in transactions)
                        {
                            serialNumber += 1;

                            // conditionally renders the row (table content summary)
                            if (transaction.Equals(summaryEntry))
                            {
                                table.Cell().Text("");
                                table.Cell().Text("");
                                table.Cell().Text("");
                                table.Cell().Text("");

                                table.Cell().Element(CellStyle).AlignRight().Text($"{transaction.AmountDue:N2}").FontSize(10).SemiBold();
                                table.Cell().Element(CellStyle).AlignRight().Text($"{transaction.AmountPaid:N2}").FontSize(10).SemiBold();
                                table.Cell().Element(CellStyle).AlignRight()
                                .Text($"{summaryEntry.OutstandingAmount:N2}").FontSize(10).SemiBold();

                                continue;
                            }

                            table.Cell().Element(CellStyle).AlignCenter().Text($"{serialNumber}").FontSize(9);

                            table.Cell().Element(CellStyle).Text($"{transaction.CustomerId}").FontSize(9);
                            table.Cell().Element(CellStyle).Text($"{transaction.CustomerName}").FontSize(9);
                            table.Cell().Element(CellStyle).Text($"{transaction.TransactonDate.ToString("yyyy-MMM-dd")}").FontSize(9);
                            table.Cell().Element(CellStyle).AlignRight().Text($"{transaction.AmountDue:N2}").FontSize(9);
                            table.Cell().Element(CellStyle).AlignRight().Text($"{transaction.AmountPaid:N2}").FontSize(9);
                            table.Cell().Element(OustandingBalanceCellStyle).AlignRight().Text($"{transaction.AmountDue - transaction.AmountPaid:N2}").FontSize(9);
                            table.Cell().Element(CellStyle).Text($"").FontSize(9);

                            IContainer CellStyle(IContainer container) => DefaultCellStyle(container).ShowOnce();
                            IContainer OustandingBalanceCellStyle(IContainer container) => EvaluateOutstandingBalanceBackgroundColor(container, transaction.AmountDue, transaction.AmountPaid).ShowOnce();
                        }

                   });
               });

               mainContentColumn.Item().PaddingTop((float)4.0, Unit.Centimetre).Row(row => {
                   row.RelativeItem().Column(column =>
                   {
                      column.Item().Text("_______________________________");
                      column.Item().PaddingLeft(1.2f, Unit.Centimetre).PaddingTop(0.4f, Unit.Centimetre).Text("Doctor signature & date");
                   });
                   
                   row.RelativeItem().AlignRight().Column(column =>
                   {
                      column.Item().Text("_______________________________");
                      column.Item().PaddingLeft(1.2f, Unit.Centimetre).PaddingTop(0.4f, Unit.Centimetre).Text("Accountant signature & date");
                   });
               });
            });
        }
        
        private static List<PharmacyReportInfo> GetPharmacyReport()
        {
            var pharmacyReport = new List<PharmacyReportInfo>
            {
                new PharmacyReportInfo() { AmountDue = 65900.00, AmountPaid = 45350.00, CustomerId = "CUST/90/2891", CustomerName = "Ibrahim Hassan", Id = 1, TransactonDate = new DateTime(2024, 3, 19) },
                new PharmacyReportInfo() { AmountDue=4500, AmountPaid = 4500, CustomerId="CUST/23/8811", CustomerName = "Ayodeji Peter", Id=2, TransactonDate=new DateTime(2024, 4, 22) },
                new PharmacyReportInfo() { AmountDue=8900.34, AmountPaid = 8000, CustomerId="CUST/11/7399", CustomerName = "James Okuzor", Id=3, TransactonDate=new DateTime(2024, 6, 5) },
                new PharmacyReportInfo() { AmountDue=67500, AmountPaid = 67500, CustomerId="CUST/77/2833", CustomerName = "Olufemi Oyedepo", Id=4, TransactonDate=new DateTime(2024, 1, 5) },
                new PharmacyReportInfo() { AmountDue=20000, AmountPaid = 19800, CustomerId="CUST/14/8900", CustomerName = "Tessy Okotie", Id=5, TransactonDate=new DateTime(2024, 2, 10) },
                new PharmacyReportInfo() { AmountDue=1250, AmountPaid = 1250, CustomerId="CUST/92/2011", CustomerName = "Adewale Suleiman", Id=6, TransactonDate=new DateTime(2024, 2, 3) },
                new PharmacyReportInfo() { AmountDue=81320, AmountPaid = 81320, CustomerId="CUST/34/6500", CustomerName = "John Barnes", Id=7, TransactonDate=new DateTime(2023, 7, 7) },
                new PharmacyReportInfo() { AmountDue=48750, AmountPaid = 48750, CustomerId="CUST/11/4390", CustomerName = "Jabilla Muhammed", Id=8, TransactonDate=new DateTime(2024, 4, 2) },
                new PharmacyReportInfo() { AmountDue=32900, AmountPaid = 30000, CustomerId="CUST/34/1872", CustomerName = "Kolawole Emmanuel", Id=9, TransactonDate=new DateTime(2024, 2, 18) },
                new PharmacyReportInfo() { AmountDue=43500, AmountPaid = 23850, CustomerId="CUST/14/3390", CustomerName = "James Toney", Id=10, TransactonDate=new DateTime(2024, 3, 3) },
                new PharmacyReportInfo() { AmountDue=77000, AmountPaid = 77000, CustomerId="CUST/10/1019", CustomerName = "Gordon Davis", Id=11, TransactonDate=new DateTime(2024, 5, 5) },
                new PharmacyReportInfo() { AmountDue=2300, AmountPaid = 2300, CustomerId="CUST/28/2011", CustomerName = "Esteban Juan Rodriguez", Id=12, TransactonDate=new DateTime(2024, 8, 18) },
                new PharmacyReportInfo() { AmountDue=6750, AmountPaid = 4400, CustomerId="CUST/27/2899", CustomerName = "Jacques Du Plessis", Id=13, TransactonDate=new DateTime(2024, 1, 19) },
                new PharmacyReportInfo() { AmountDue=3200, AmountPaid = 950, CustomerId="CUST/29/9902", CustomerName = "Donald Mafa", Id=14, TransactonDate=new DateTime(2024, 8, 11) },
                new PharmacyReportInfo() { AmountDue=5000, AmountPaid = 5000, CustomerId="CUST/11/3381", CustomerName = "Angelo Gabriel", Id=15, TransactonDate=new DateTime(2024, 7, 23 )},
                new PharmacyReportInfo() { AmountDue=7800, AmountPaid = 7800, CustomerId="CUST/15/2401", CustomerName = "Naseem Muhammad", Id=16, TransactonDate=new DateTime(2024, 9, 30) },
                new PharmacyReportInfo() { AmountDue=65900, AmountPaid = 65700, CustomerId="CUST/15/6654", CustomerName = "Yu Xiao Ping", Id=17, TransactonDate=new DateTime(2024, 4, 4) },
                new PharmacyReportInfo() { AmountDue=25600, AmountPaid = 24000, CustomerId="CUST/13/8899", CustomerName = "Jane Ashcroft-Peters", Id=18, TransactonDate=new DateTime(2024, 8, 9) },
                new PharmacyReportInfo() { AmountDue=4000, AmountPaid = 4000, CustomerId="CUST/55/9109", CustomerName = "Ashley Nelly", Id=19, TransactonDate=new DateTime(2024, 7, 21) },
                new PharmacyReportInfo() { AmountDue=21000, AmountPaid = 5000, CustomerId="CUST/21/4300", CustomerName = "Simone Clemente", Id=20, TransactonDate=new DateTime(2024, 3, 28) },
            };


            var summary = new PharmacyReportInfo()
            {
                AmountDue = pharmacyReport.Sum(s => s.AmountDue),
                AmountPaid = pharmacyReport.Sum(s => s.AmountPaid),
                OutstandingAmount = pharmacyReport.Sum(s => s.AmountDue) - pharmacyReport.Sum(s => s.AmountPaid),
            };

            pharmacyReport.Add(summary);

            return pharmacyReport;
        }
    }
}
