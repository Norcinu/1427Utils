using System.Collections.Generic;
using System.Windows.Input;

namespace PDTUtils.MVVM.Models
{
    class GameSettingModel
    {
        public bool? Active
        {
            get { return this._active; }
            set
            {
                if (value == null)
                    this._active = false;
                else
                    this._active = value;
            }
        }
        
        public bool Promo { get; set; }
        public uint ModelNumber { get; set; }
        public string StakeOne { get; set; }
        public string StakeTwo { get; set; }
        public string StakeThree { get; set; }
        public string StakeFour { get; set; }
        public string StakeFive { get; set; }
        public string StakeSix { get; set; }
        public string StakeSeven { get; set; }
        public string StakeEight { get; set; }
        public string StakeNine { get; set; }
        public string StakeTen { get; set; }
       /* public decimal StakeOne { get; set; }
        public decimal StakeTwo { get; set; }
        public decimal StakeThree { get; set; }
        public decimal StakeFour { get; set; }
        public decimal StakeFive { get; set; }
        public decimal StakeSix { get; set; }
        public decimal StakeSeven { get; set; }
        public decimal StakeEight { get; set; }
        public decimal StakeNine { get; set; }
        public decimal StakeTen { get; set; }*/
        public string Title { get; set; }
        public string ModelDirectory { get; set; }
        public string Exe { get; set; }
        public string HashKey { get; set; }

        private bool? _active;

        public GameSettingModel()
        {
            Active = false;
            Promo = false;
            ModelNumber = 0;
            StakeOne = "";
            StakeTwo = "";
            StakeThree = "";
            StakeFour = "";
            StakeFive = "";
            StakeSix = "";
            StakeSeven = "";
            StakeEight = "";
            StakeNine = "";
            StakeTen = "";
            Title = "";
            ModelDirectory = "";
            Exe = "";
            HashKey = "";
        }
        
        public ICommand ToggleActive { get { return new DelegateCommand(o => DoToggleActive()); } }
        public void DoToggleActive()
        {
            this.Active = !!this.Active;
            System.Diagnostics.Debug.WriteLine(this.Active, "this.Active = {0}");
        }
        
        public ICommand ToggleStake { get { return new DelegateCommand(o => DoToggleStake()); } }
        void DoToggleStake()
        {
            if (this.StakeOne == "")
                this.StakeOne = "10";
            else
                this.StakeOne = "0";

            System.Diagnostics.Debug.WriteLine(this.StakeOne, "this.StakeOne = {0}");
        }
    }
}
