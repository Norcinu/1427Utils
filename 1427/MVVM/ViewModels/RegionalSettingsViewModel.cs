using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class RegionalSettingsViewModel : ObservableObject
    {
        ObservableCollection<SpanishRegionalModel> _arcades = new ObservableCollection<SpanishRegionalModel>();
        ObservableCollection<SpanishRegionalModel> _street = new ObservableCollection<SpanishRegionalModel>();
        
        readonly string _espRegionIni = @"D:\1427\Config\EspRegional.ini";
  
        SpainRegionSelection _selected = new SpainRegionSelection();      
        
        readonly string[] _streetMarketRegions = new string[20]
        {
            "Andalucia", "Aragón", "Asturias", "Baleares", "País Vasco", "Cantabria", "Castilla-La Mancha",
            "Castilla León", "Catalonia", "Catalonia Light", "Extremadura", "Madrid", "Murcia", "Navarra",
            "La Rioja", "Valencia", "Valencia 500", "Valencia Light", "Canarias", "Galicia"
        };

        readonly string[] _arcadeRegions = new string[26]
        {
            "Andalucia", "Aragón", "Asturias", "Baleares", "Baleares (Special B)", "Basque (BS)", "Basque (Special BS)",
            "Cantabria", "Castilla-La Mancha", "Castilla-La Mancha (Special)", "Castilla León", "Catalonia", "Extremadura",
            "Madrid", "Madrid 2000", "Madrid 3000", "Murcia", "Murcia (arcade, reservate area)", "Navarra", "La Rioja",
            "Valencia 2000", "Valencia 3000", "Valencia", "Valencia 1000", "Canarias", "Galicia"
        };
        
        #region Properties
        public bool FirstScreen { get; set; }
        public bool SecondScreen { get; set; }

        public IEnumerable<SpanishRegionalModel> Arcades { get { return _arcades; } }
        public IEnumerable<SpanishRegionalModel> Street { get { return _street; } }
        public SpainRegionSelection Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
        #endregion
        
        #region Commands
        public ICommand SetActiveRegion { get { return new DelegateCommand(SetRegion); } }
        public ICommand Save { get { return new DelegateCommand(o => SaveChanges()); } }
        public ICommand Load { get { return new DelegateCommand(o => LoadSettings()); } }
        #endregion
        
        public RegionalSettingsViewModel()
        {
            System.Array.Sort(_streetMarketRegions);
            System.Array.Sort(_arcadeRegions);

            for (int i = 0; i < _streetMarketRegions.Length - 1; i++)
            {
                SpanishRegional sr = new SpanishRegional();
                BoLib.getRegionalValues(i, ref sr);
                _street.Add(new SpanishRegionalModel(_streetMarketRegions[i], sr));
            }

            int smLength = _streetMarketRegions.Length - 1;
            for (int i = 0; i < _arcadeRegions.Length - 1; i++)
            {
                SpanishRegional sr = new SpanishRegional();
                BoLib.getRegionalValues(smLength + i, ref sr);
                _arcades.Add(new SpanishRegionalModel(_arcadeRegions[i], sr));
            }
            
            this.LoadSettings();
            
            this.RaisePropertyChangedEvent("Arcades");
            this.RaisePropertyChangedEvent("Street");
            this.RaisePropertyChangedEvent("Selected");
        }
        
        public void SaveChanges()
        {
            NativeWinApi.WritePrivateProfileString("Current", "Region", Selected.Community, _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Current", "VenueType", Selected.VenueType, _espRegionIni);
        }
        
        public void LoadSettings()
        {
            string[] temp;
            var c = IniFileUtility.GetIniProfileSection(out temp, "Current", _espRegionIni);
            _selected.Community = temp[0].Substring(7);
            _selected.VenueType = temp[1].Substring(10);
        }
        
        public void SetRegion(object listview)
        {
            var lv = listview as ListView;
            if (lv.SelectedIndex == -1) 
                return;

            var name = lv.SelectedItem as SpanishRegionalModel;
            Selected.Community = name.Community;

            if (System.Convert.ToInt32(lv.Tag) == (int)ESiteType.StreetMarket)
                Selected.VenueType = "Street Market";
            else if (System.Convert.ToInt32(lv.Tag) == (int)ESiteType.Arcade)
                Selected.VenueType = "Arcade";

            this.RaisePropertyChangedEvent("Selected");
            this.SaveChanges();
        }
    }
}
