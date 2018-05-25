using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PdfTextReader.Parser
{
    class Jornal
    {
        const string DOCUMENTO_RECENTE_25_OUT_2017 = "2017_10_25";

        int _jornalTypeId;

        public string JornalTypeId => _jornalTypeId.ToString();

        public Jornal(string pdf)
        {
            _jornalTypeId = GetJornalIdentificador(pdf);
        }

        Dictionary<string, int> _entriesAntigos = new Dictionary<string, int>
        {
            { "DO1", 1},
            { "DO2", 2},
            { "DO3", 3},
            // ANVISA "_p_anvisa" 1010
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
            // ANVISA "_p_anvisa" 531?
            { "DO1_EXTRA", 521},
            { "DO2_EXTRA", 525},
            { "DO3_EXTRA", 526}
        };

        readonly Regex _jornalRegex = new Regex(@"(DO[123](_EXTRA)?)_(\d\d\d\d_\d\d_\d\d)");

        int GetJornalIdentificador(string filename)
        {
            string file = filename.ToUpper();

            var match =_jornalRegex.Match(file);

            if (!match.Success)
                return -1;

            string tipo = match.Groups[1].Value;
            string data = match.Groups[3].Value;

            int recente = String.Compare(data, DOCUMENTO_RECENTE_25_OUT_2017);

            var entrada = (recente >= 0) ? _entriesRecentes : _entriesAntigos;

            return entrada[tipo];
        }


    }
}
