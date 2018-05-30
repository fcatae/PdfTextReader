using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PdfTextReader.Parser
{
    class Jornal
    {
        const string GENERIC_PDF_PAGE = "http://www.imprensanacional.gov.br";
        const string DOCUMENTO_RECENTE_25_OUT_2017 = "2017_10_25";

        public string PubName { get; set; }
        public string PubDate { get; set; }
        public string DocumentUrl { get; set; }
        
        readonly Regex _jornalRegex = new Regex(@"(DO[123](_EXTRA)?)_(\d\d\d\d_\d\d_\d\d)");
        readonly Regex _jornalAnvisaRegex = new Regex(@"(\d\d\d\d_\d\d_\d\d)_P_ANVISA");

        public Jornal(string pdf)
        {
            string file = pdf.ToUpper();

            InitDOU(file);
            InitDO1Anvisa(file);
        }

        void InitDOU(string file)
        {
            var match = _jornalRegex.Match(file);

            if (!match.Success)
                return;

            string tipo = match.Groups[1].Value;
            string data = match.Groups[3].Value;

            PubName = tipo;
            PubDate = data;
        }

        void InitDO1Anvisa(string file)
        {
            var match = _jornalAnvisaRegex.Match(file);

            if (!match.Success)
                return;

            string tipo = "DO1_ANVISA";
            string data = match.Groups[1].Value;

            PubName = tipo;
            PubDate = data;
        }

        Dictionary<string, int> _entriesAntigos = new Dictionary<string, int>
        {
            { "DO1", 1},
            { "DO2", 2},
            { "DO3", 3},
            { "DO1_ANVISA", 1010 },
            { "DO1_EXTRA", 1000},
            { "DO2_EXTRA", 2000},
            { "DO3_EXTRA", 3000}
        };

        // A partir de 25/10/2017, as novas numeracoes para os DOU
        Dictionary<string, int> _entriesRecentes = new Dictionary<string, int>
        {
            { "DO1", 515},
            { "DO2", 529},
            { "DO3", 530},
            { "DO1_ANVISA", 531 },
            { "DO1_EXTRA", 521},
            { "DO2_EXTRA", 525},
            { "DO3_EXTRA", 526}
        };

        int GetJornalId()
        {
            string tipo = this.PubName;
            string data = this.PubDate;

            int recente = String.Compare(data, DOCUMENTO_RECENTE_25_OUT_2017);

            var entrada = (recente >= 0) ? _entriesRecentes : _entriesAntigos;

            return entrada[tipo];
        }
        
        string GetJornalDateDMY()
        {
            var comps = PubDate.Split('_');
            return $"{comps[2]}/{comps[1]}/{comps[0]}";
        }

        public string GetDocumentPageUrl(string pagina)
        {
            if (String.IsNullOrEmpty(PubName) || String.IsNullOrEmpty(PubDate))
                return null;

            int jornalId = GetJornalId();
            string date = GetJornalDateDMY();

            string baseUrl = "http://pesquisa.in.gov.br/imprensa/jsp/visualiza/index.jsp";
            string query = $"?jornal={jornalId}&data={date}&pagina={pagina}";

            return baseUrl + query;
        }

        //public string GetEditionNumber()
        //{
        //    return null;
        //}
    }
}
