using PDTUtils.MVVM.Models;

namespace PDTUtils.MVVM.ViewModels
{
    class HopperViewModel
    {
        public HopperModel LeftHopper { get; set; } // £1 Hopper
        public HopperModel RightHopper { get; set; } // 10p Hopper
        
        public HopperViewModel()
        {
            LeftHopper = new HopperModel();
            RightHopper = new HopperModel();
        }
    }
}
