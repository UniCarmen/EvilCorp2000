using BusinessLayer.Models;

namespace EvilCorp2000.Pages.Shared.ViewModels
{
    public class FilterPartialViewModel
    {
        public string? Search { get; set; }
        public string? SortOrderString { get; set; }
        public string? FilterCategoryString { get; set; }
        public Guid? FilterCategory { get; set; }
        public List<CategoryDTO>? Categories { get; set; }        
    }
}
