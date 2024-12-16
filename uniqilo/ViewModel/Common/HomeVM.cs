using uniqilo.ViewModel.Category;
using uniqilo.ViewModel.Product;
using uniqilo.ViewModel.Slider;

namespace uniqilo.ViewModel.Common
{
    public class HomeVM
    {
        public IEnumerable<SliderItemVM> Sliders { get; set; }
        public IEnumerable<ProductItemVM> Products { get; set; }
        public IEnumerable<CategoryItemVM> Categories { get; set; }
    }
}
