using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Utils
{
    public static class CurrencyCodeMapper
    {
        private static readonly Dictionary<string, string> SymbolsByCode;

        public static string GetSymbol(string code) { return SymbolsByCode[code]; }

        static CurrencyCodeMapper()
        {
            SymbolsByCode = new Dictionary<string, string>();

            var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                          .Select(x => new RegionInfo(x.LCID));

            foreach (var region in regions)
                if (!SymbolsByCode.ContainsKey(region.ISOCurrencySymbol))
                    SymbolsByCode.Add(region.ISOCurrencySymbol, region.CurrencySymbol);
        }
        public static string getCurrencySymbol(string CurrencyCode)
        {
            string symbol = string.Empty;
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            IList Result = new ArrayList();
            foreach (CultureInfo ci in cultures)
            {
                RegionInfo ri = new RegionInfo(ci.LCID);
                if (ri.ISOCurrencySymbol == CurrencyCode)
                {
                    symbol = ri.CurrencySymbol;
                    return symbol;
                }
            }
            return symbol;

        }
    }
}
