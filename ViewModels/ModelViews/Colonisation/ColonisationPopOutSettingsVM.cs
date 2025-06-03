using ODEliteTracker.Models.Settings;
using ODMVVM.Commands;
using ODMVVM.Extensions;
using ODMVVM.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ODEliteTracker.ViewModels.ModelViews.Colonisation
{
    public class ColonisationPopOutSettingsVM : ODObservableObject
    {
		public ColonisationPopOutSettingsVM() 
		{
			SetHeadersVisCommand = new ODRelayCommand(OnSetColumnHeaderVis);
			SetColumnVisCommand = new ODRelayCommand<ColonisationColumns>(OnSetColumnVis);
		}

		public EventHandler? ColumnVisibilityChanged;

        private Visibility nameVis;
		public Visibility NameVis
		{
			get => nameVis;
			set
			{
				nameVis = value;
				OnPropertyChanged(nameof(NameVis));
			}
		}

		private Visibility categoryVis;
		public Visibility CategoryVis
		{
			get => categoryVis;
			set
			{
				categoryVis = value;
				OnPropertyChanged(nameof(CategoryVis));
			}
		}

		private Visibility marketStockVis;
		public Visibility MarketStockVis
		{
			get => marketStockVis;
			set
			{
				marketStockVis = value;
				OnPropertyChanged(nameof(MarketStockVis));
			}
		}

		private Visibility carrierStockVis;
		public Visibility CarrierStockVis
		{
			get => carrierStockVis;
			set
			{
				carrierStockVis = value;
				OnPropertyChanged(nameof(CarrierStockVis));
			}
		}

        private Visibility carrierDiffVis;
        public Visibility CarrierDiffVis
        {
            get => carrierDiffVis;
            set
            {
                carrierDiffVis = value;
                OnPropertyChanged(nameof(CarrierDiffVis));
            }

        }
        private Visibility remainingVis;
		public Visibility RemainingVis
		{
			get => remainingVis;
			set
			{
				remainingVis = value;
				OnPropertyChanged(nameof(RemainingVis));
			}
		}

		private DataGridHeadersVisibility headersVis;
		public DataGridHeadersVisibility HeadersVis
		{
			get => headersVis;
			set
			{
                headersVis = value;
				OnPropertyChanged(nameof(HeadersVis));
				OnPropertyChanged(nameof(HeaderVisBool));
			}
		}

		public bool HeaderVisBool => HeadersVis == DataGridHeadersVisibility.Column;
        public ICommand SetHeadersVisCommand { get; }
        public ICommand SetColumnVisCommand { get; }

        private void OnSetColumnVis(ColonisationColumns columns)
        {
            switch (columns)
            {
                case ColonisationColumns.Name:
					NameVis = nameVis == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                case ColonisationColumns.Category:
					CategoryVis = categoryVis == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                case ColonisationColumns.MarketStock:
					MarketStockVis = marketStockVis == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                case ColonisationColumns.CarrierStock:
					CarrierStockVis = carrierStockVis == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
				case ColonisationColumns.CarrierDiff:
					CarrierDiffVis = carrierDiffVis == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
					break;
                case ColonisationColumns.Remaining:
					RemainingVis = remainingVis == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
            }

			ColumnVisibilityChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnSetColumnHeaderVis(object? obj)
        {
			HeadersVis = headersVis == DataGridHeadersVisibility.Column ? DataGridHeadersVisibility.None : DataGridHeadersVisibility.Column;
        }

        internal void LoadSettings(ColonisationPopOutSettings settings)
		{
			NameVis = settings.Columns.HasFlag(ColonisationColumns.Name).ToVis();
            CategoryVis = settings.Columns.HasFlag(ColonisationColumns.Category).ToVis();
            MarketStockVis = settings.Columns.HasFlag(ColonisationColumns.MarketStock).ToVis();
            CarrierStockVis = settings.Columns.HasFlag(ColonisationColumns.CarrierStock).ToVis();
            RemainingVis = settings.Columns.HasFlag(ColonisationColumns.Remaining).ToVis();

			HeadersVis = settings.ShowColumnHeaders ? DataGridHeadersVisibility.Column : DataGridHeadersVisibility.None;

            ColumnVisibilityChanged?.Invoke(this, EventArgs.Empty);
        }

        internal ColonisationPopOutSettings GetSettings()
		{
			var columns = ColonisationColumns.None;

			if (NameVis == Visibility.Visible)
				columns |= ColonisationColumns.Name;
			if (CategoryVis == Visibility.Visible)
				columns |= ColonisationColumns.Category;
			if (MarketStockVis == Visibility.Visible)
				columns |= ColonisationColumns.MarketStock;
			if (CarrierStockVis == Visibility.Visible)
				columns |= ColonisationColumns.CarrierStock;
			if (RemainingVis == Visibility.Visible)
				columns |= ColonisationColumns.Remaining;

			return new ColonisationPopOutSettings()
			{
				Columns = columns,
				ShowColumnHeaders = HeadersVis == DataGridHeadersVisibility.Column
			};
        }
    }
}
