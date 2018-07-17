using System;
namespace TempCertificados.Process
{
    public class SheetReader
    {
        public string Data { get; set; }
        public int TotalRows { get; set; }
        public int TotalColumns { get; set; }
        public string[] Rows { get; set; }
        public string[,] Cells { get; set; }
        public string[] Tags { get; set; }

        public SheetReader(string data)
        {
            Data = data;
            GetRows();
            GetTags();
            GetCells();
        }

        private void GetRows()
        {
            //somente interessa as linhas que não estão em branco
            Rows = Array.FindAll(Data.Split('\n'), x => x.Trim() != string.Empty);
            TotalRows = Rows.Length;
        }

        private void GetCells()
        {
            var cells = new string [TotalRows,TotalColumns];
            int i = 0;
            foreach(var row in Rows)
            {

                //dada que a primeira linha sempre representa a coluna
                // de nuyméro fixo, então só importa os 5 primeiros 
                // items de cada linha, ignorando qualquer espaço que tenha depois
                var splited = row.Split('\t', TotalColumns);
                splited[TotalColumns - 1] = splited[TotalColumns - 1].TrimEnd();
                var j = 0;
                foreach(var cell in splited)
                {
                    cells[i, j] = cell;
                    j++;
                }
                i++;
            }
            Cells = cells;
        }

        private void GetTags()
        {
            var tags = Rows[0].Split('\t');

            //a primeira linha da planilha determina o número de
            //colunas
            Tags = Array.FindAll(tags, x => x != string.Empty);
            TotalColumns = Tags.Length;
        }

        public string[] GetTagColumn(string tag)
        {
            int pos = Array.FindIndex(Tags, x => x == tag);

            if (pos != -1)
            {
                var column = new string[TotalRows-1];
                for (int i = 1; i < TotalRows; i++)
                {
                    column[i-1] = Cells[i,pos];

                }

                return column;
            }

            return null;
        }

    }
}
