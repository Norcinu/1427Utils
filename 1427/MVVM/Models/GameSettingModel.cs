
namespace PDTUtils.MVVM.Models
{
    class GameSettingModel
    {
        public bool Active { get; set; }
        public bool Promo { get; set; }
        public bool? StakeOne { get; set; }
        public bool? StakeTwo { get; set; } 
        public bool? StakeThree { get; set; }
        public bool? StakeFour { get; set; }
        public bool? StakeFive { get; set; }
        public bool? StakeSix { get; set; }
        public string Title { get; set; }
        
        public GameSettingModel()
        {
            Active = false;
            Promo = false;
            StakeOne = null;
            StakeTwo = null;
            StakeThree = null;
            StakeFour = null;
            StakeFive = null;
            StakeSix = null;
            Title = "Game Title";
        }
    }
}
