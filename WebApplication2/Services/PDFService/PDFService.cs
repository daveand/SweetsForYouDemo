using DinkToPdf;
using DinkToPdf.Contracts;
using RazorLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services.PDFService
{
    public class PDFService : IPDFService
    {
        private readonly IRazorLightEngine _razorEngine;
        private readonly IConverter _pdfConverter;

        public PDFService(IRazorLightEngine razorEngine, IConverter pdfConverter)
        {
            _razorEngine = razorEngine;
            _pdfConverter = pdfConverter;
        }
        public async Task<byte[]> Create(OrdersPdfModel orders)
        {
            var model = orders;
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\templates\\PDFTemplate.cshtml");
            //var templatePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\templates\\PDFTemplate.cshtml");
            string template = await _razorEngine.CompileRenderAsync(templatePath, model);

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings() { Top = 10, Bottom = 10, Left = 25, Right = 20 },
                DocumentTitle = "Plocklista",

            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = template,
                WebSettings = { DefaultEncoding = "utf-8", MinimumFontSize = 20 },
                HeaderSettings = { FontName = "Segoe UI", FontSize = 12, Line = true, Center = "Plocklista - " + orders.Customer.Company + " - " + orders.Customer.Department },
                FooterSettings = { FontName = "Segoe UI", FontSize = 12, Line = true, Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            byte[] file = _pdfConverter.Convert(pdf);
            return file;
        }
    }
}
