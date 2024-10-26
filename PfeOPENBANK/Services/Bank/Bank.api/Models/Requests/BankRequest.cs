using Bank.api.Models.Responses;
using System.ComponentModel.DataAnnotations;

namespace Bank.api.Models.Requests
{
    public class BankRequest
    {
        public string Id { get; set; }
        public string Short_name { get; set; }
        public string Full_name { get; set; }
        [DataType(DataType.Url)]
        public string Logo { get; set; }
        [DataType(DataType.Url)]
        public string Website { get; set; }
        public BankRouting Bank_routing { get; set; }

        public static List<BankRequest> GetTunisianBanks()
        {
            return new List<BankRequest>
        {
            new BankRequest
            {
                Id = "1",
                Short_name = "BT",
                Full_name = "Banque de Tunisie",
                Logo = "https://www.bt.com.tn/images/logos/logo_bt_horizontal.png",
                Website = "https://www.bt.com.tn/",
                Bank_routing = new BankRouting{
                Scheme = "BT",
                Address = "Avenue Habib Bourguiba, Tunis, Tunisia" }
            },
            new BankRequest
            {
                Id = "2",
                Short_name = "STB",
                Full_name = "Société Tunisienne de Banque",
                Logo = "https://www.stb.com.tn/sites/default/files/styles/logo_stb/public/logo_stb.png",
                Website = "https://www.stb.com.tn/",
                Bank_routing = new BankRouting{
                Scheme = "STB",
                Address = "..." }
            },
            new BankRequest
            {
                Id = "3",
                Short_name = "BIAT",
                Full_name = "Banque Internationale Arabe de Tunisie",
                Logo = "https://www.biat.com.tn/images/logo.png",
                Website = "https://www.biat.com.tn/",
                Bank_routing = new BankRouting{
                Scheme = "BIAT",
                Address = "..." }
            },
            
        };
        }
    }
}
