using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.html.simpleparser;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TempCertificados.Process{
    class GeneratorPDF
    {

        private readonly SheetReader sheetReader;
        private readonly string html;
        private readonly Image img;

        public GeneratorPDF(SheetReader sheetReader, Image img, string html)
        {

            this.sheetReader = sheetReader;
            this.html = html;
            this.img = img;
        }

        private string ReplaceTags(int row)
        {
            var result = html;

            Regex rCheck = new Regex(@"\{\{(\w+)\}\}");
            var tagsInside = Array.FindAll(sheetReader.Tags,
                                           x => html.Contains("{{" + x + "}}"));
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
                result = result.Replace("{{" + tag + "}}",
                                        sheetReader.GetTagColumn(tag)[row]);
            }
            
            return result;
        }

        public List<byte []> CreatePDFs()
        {
            var nPersons = sheetReader.TotalRows - 1;
            var tags = sheetReader.Tags;

            var listaPDF = new List<byte[]>();
            for (int i = 0; i < nPersons; i++)
            {
                listaPDF.Add(CreatePDF(ReplaceTags(i)));
            }

            return listaPDF;
        }

        private byte[] CreatePDF(string htmlReplaced)
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
            return memStream.ToArray();
        }
    }    
}
