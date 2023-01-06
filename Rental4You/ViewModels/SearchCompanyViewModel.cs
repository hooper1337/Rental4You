using Rental4You.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.ViewModels
{
    public class SearchCompanyViewModel
    {
        public List<Company> companyList { get; set; }
        public int numberOfResults { get; set; }
        [Display(Name = "Company Search", Prompt = "Introduce the text for the search...")]
        public string textToSearch { get; set; }
        [Display(Name = "Company available")]
        public Boolean available { get; set; }
    }
}
