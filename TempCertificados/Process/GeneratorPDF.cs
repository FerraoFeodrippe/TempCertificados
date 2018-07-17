using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.html.simpleparser;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TempCertificados.Process{
    class GeneratorPDF
    {

        private SheetReader sheetReader;
        private string img64;
        private string html;
        private Image img;
        private SendEmail se;

        public GeneratorPDF(string sheet, string img64, string html, SendEmail se)
        {
            if (sheet== null)
            {
                throw new Exception("Planilha não preenchida!");
            }
            sheetReader = new SheetReader(sheet);
            this.img64 = img64;
            this.html = html;
            this.se = se;
            //remove o desnecessario para transformar em bytes 
            //-> "data:image/jpeg;base64," (23 caracteres)
            img = Image.GetInstance(Convert.FromBase64String(img64.Remove(0, 23)));
            img.SetAbsolutePosition(0, 0);
            img.ScaleAbsolute(PageSize.A4.Height, PageSize.A4.Width);
            img.Alignment = Image.UNDERLYING;
        }

        private string ReplaceTags(int row)
        {
            var result = html;

            Regex rCheck = new Regex(@"\{\{(\w+)\}\}");
            var tagsInside = Array.FindAll(sheetReader.Tags, x => html.Contains("{{" + x + "}}"));
            var matches = rCheck.Matches(html);

            foreach(Match m in matches)
            {
                var value = m.Groups[1].Value;
                var exists = Array.Exists(sheetReader.Tags, x => x == value);
                if (!exists) 
                {
                    throw new Exception(
                        string.Format("Não foi possível identificar o cabeçalho com a tag {{{{{0}}}}} posta no texto",
                                      value));
                }

            }

            foreach (var tag in tagsInside)
            {
                result = result.Replace("{{" + tag + "}}", sheetReader.GetTagColumn(tag)[row]);
            }
            
            return result;
        }

        public async Task CreateAndSendPDFs()
        {
            var nPersons = sheetReader.TotalRows - 1;
            var tags = sheetReader.Tags;
            var firstDatas = sheetReader.GetTagColumn(tags[0]);
            var emails = sheetReader.GetTagColumn("Email");
            if (emails == null)
            {
                throw new Exception("Não foi possível identificar o cabeçalho de Email.");
            }
            for (int i = 0; i < nPersons; i++)
            {
                var firstData = firstDatas[i];
                await CreateAndSendPDF(firstData, emails[i], ReplaceTags(i));

            }
        }

        private async Task CreateAndSendPDF(string name,string email, string htmlReplaced)
        {

            var document = new Document(PageSize.A4.Rotate(),0,0,0,0);
            MemoryStream memStream = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, memStream);
            var htmlReader = new StringReader(htmlReplaced);
            var htmlWorker = new HTMLWorker(document);
            document.Open();

            document.Add(img);

            htmlWorker.StartDocument();

            htmlWorker.Parse(htmlReader);

            htmlWorker.EndDocument();
            htmlWorker.Close();

            document.Close();
            await se.To(email, name, memStream.ToArray());
        }
    }    
}
