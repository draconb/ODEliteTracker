using ODEliteTracker.Models.Settings;
using ODMVVM.ViewModels;
using System.Windows;

namespace ODEliteTracker.ViewModels.ModelViews.Compass
{
	public sealed class CompassSettingsViewModel : ODObservableObject
	{
		private bool inShip;

		private bool hideInSrv;
		public bool HideInSrv
		{
			get => hideInSrv;
			set
			{
				hideInSrv = value;
				OnPropertyChanged(nameof(HideInSrv));
            }
		}

		private bool hideOnFoot;
		public bool HideOnFoot
		{
			get => hideOnFoot;
			set
			{
				hideOnFoot = value;
				OnPropertyChanged(nameof(HideOnFoot));
            }
		}

		private bool hideWhenNoLatLon;
		public bool HideWhenNoLatLon
		{
			get => hideWhenNoLatLon;
			set
			{
				hideWhenNoLatLon = value;
				OnPropertyChanged(nameof(HideWhenNoLatLon));
				SetCompassVis(false, false, false);
            }
		}

		private bool hideTargetInfo;
		public bool HideTargetInfo
		{
			get => hideTargetInfo;
			set
			{
				hideTargetInfo = value;
				OnPropertyChanged(nameof(HideTargetInfo));
			}
		}

		private double speedInShip;
		public double SpeedInShip
		{
			get => speedInShip;
			set
			{
				speedInShip = value;
				OnPropertyChanged(nameof(SpeedInShip));
                OnPropertyChanged(nameof(CurrentSpeed));
            }
		}

		private double speedOnFoot;
		public double SpeedOnFoot
		{
			get => speedOnFoot;
			set
			{
				speedOnFoot = value;
				OnPropertyChanged(nameof(SpeedOnFoot));
                OnPropertyChanged(nameof(CurrentSpeed));
            }
		}

		public double CurrentSpeed => inShip ? speedInShip : speedOnFoot;

        private Visibility compassVis = Visibility.Hidden;
        public Visibility CompassVis
        {
            get => compassVis;
            set
            {
                compassVis = value;
                OnPropertyChanged(nameof(CompassVis));
            }
        }

        internal void SetCompassSpeed(bool inShip)
		{
			this.inShip = inShip;
            OnPropertyChanged(nameof(CurrentSpeed));
        }

		public void LoadSettings(CompassSettings settings)
		{
			HideInSrv = settings.Bools.HasFlag(CompassBools.HideWhenInSRV);
			HideOnFoot = settings.Bools.HasFlag(CompassBools.HideWhenOnFoot);
			HideWhenNoLatLon = settings.Bools.HasFlag(CompassBools.HideWhenNoLongLat);
            HideTargetInfo = settings.Bools.HasFlag(CompassBools.HideTargetWhenNotActive);
			SpeedInShip = settings.SpeedInShip;
			SpeedOnFoot = settings.SpeedOnFoot;
		}

		public CompassSettings GetSettings()
		{
			var bools = CompassBools.None;

			if (HideInSrv)
				bools |= CompassBools.HideWhenInSRV;

			if (HideOnFoot)
				bools |= CompassBools.HideWhenOnFoot;

			if (hideWhenNoLatLon)
				bools |= CompassBools.HideWhenNoLongLat;

			return new CompassSettings()
			{
				Bools = bools,
				SpeedInShip = SpeedInShip,
				SpeedOnFoot = SpeedOnFoot,
			};
		}

        internal void SetCompassVis(bool hasLatLong, bool onFoot, bool inSrv)
        {
			if (!hasLatLong && HideWhenNoLatLon
				|| onFoot && HideOnFoot
				|| inSrv && HideInSrv)
			{
				CompassVis = Visibility.Hidden;
				return;
			}

            CompassVis = Visibility.Visible;
        }
    }
}
