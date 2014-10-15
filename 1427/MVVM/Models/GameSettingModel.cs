using System.Collections.Generic;

namespace PDTUtils.MVVM.Models
{
    class GameSettingModel
    {
        public bool Active { get; set; }
        public bool Promo { get; set; }
        public uint ModelNumber { get; set; }
        public decimal StakeOne { get; set; }
        public decimal StakeTwo { get; set; }
        public decimal StakeThree { get; set; }
        public decimal StakeFour { get; set; }
        public decimal StakeFive { get; set; }
        public decimal StakeSix { get; set; }
        public decimal StakeSeven { get; set; }
        public decimal StakeEight { get; set; }
        public decimal StakeNine { get; set; }
        public decimal StakeTen { get; set; }
        public string Title { get; set; }
        public string ModelDirectory { get; set; }
        public string Exe { get; set; }
        public string HashKey { get; set; }
        
        public GameSettingModel()
        {
            Active = false;
            Promo = false;
            ModelNumber = 0;
            StakeOne = 0;
            StakeTwo = 0;
            StakeThree = 0;
            StakeFour = 0;
            StakeFive = 0;
            StakeSix = 0;
            StakeSeven = 0;
            StakeEight = 0;
            StakeNine = 0;
            StakeTen = 0;
            Title = "";
            ModelDirectory = "";
            Exe = "";
            HashKey = "";
        }
    }
}
