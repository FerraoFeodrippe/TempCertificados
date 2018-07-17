using System;
using iTextSharp.text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace TempCertificados.Process
{
    public class Core
    {
        private readonly IConfiguration _configuration;
        private readonly string html;
        private readonly Image img;
        private readonly SheetReader sheetReader;
        private readonly SendEmail se;
        private readonly GeneratorPDF generatorPDF;


        public Core(IConfiguration configuration, string sheet,
                    string img64, string html)
        {
            if (sheet == null)
            {
                throw new Exception("Planilha não preenchida!");
            }

            _configuration = configuration;
            sheetReader = new SheetReader(sheet);

            //remove o desnecessario para transformar em bytes 
            //-> "data:image/jpeg;base64," (23 caracteres)
            img = Image.GetInstance(Convert.FromBase64String(img64.Remove(0, 23)));
            img.SetAbsolutePosition(0, 0);
            img.ScaleAbsolute(PageSize.A4.Height, PageSize.A4.Width);
            img.Alignment = Image.UNDERLYING;

            this.html = html;

            se = new SendEmail(_configuration);

            generatorPDF = new GeneratorPDF(sheetReader, img, html);
        }

        public async Task Start()
        {
            var nomes = sheetReader.GetTagColumn("Nome");
            if (nomes == null)
            {
                throw new Exception("Não foi possível identificar o cabeçalho de Nome." +
                                    "\nÉ necessário o nome da pessoa indicado pela coluna");
            }
            var emails = sheetReader.GetTagColumn("Email");
            if (emails == null)
            {
                throw new Exception("Não foi possível identificar o cabeçalho de Email.");
            }

            var pdfList = generatorPDF.CreatePDFs();

            if (pdfList != null && pdfList.Count > 0)
            {
                var total = pdfList.Count;
                for (int i = 0; i < total; i++)
                {
                    await se.To(emails[i], nomes[i], pdfList[i]);
                }
                
            }
  
        }
    }
}
