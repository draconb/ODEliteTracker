using ODEliteTracker.Stores;
using ODMVVM.Commands;
using ODMVVM.Services.MessageBox;
using ODMVVM.ViewModels;
using System.Windows.Input;

namespace ODEliteTracker.ViewModels.ModelViews.Shared
{
    public enum BountySorting
    {
        Name,
        Value
    }

    public sealed class BountyManagerVM : ODObservableObject, IDisposable
    {
        public BountyManagerVM(SharedDataStore sharedData) 
        {
            this.sharedData = sharedData;
            this.sharedData.BountiesUpdated += OnBountiesUpdated;
            this.sharedData.StoreLive += OnStoreLive;

            if(this.sharedData.IsLive)
            {
                OnStoreLive(null, true);
            }

            SetTopMostFaction = new ODRelayCommand<string>(OnSetTopMostFaction);
            ClearTopMostFaction = new ODRelayCommand((_) => OnSetTopMostFaction(null));
            AddIgnoredBounties = new ODRelayCommand<string>(OnAddIgnoredBounty);
            RemoveIgnoredBounties = new ODRelayCommand<string>(OnRemoveIgnoredBounty);
        }

        private readonly SharedDataStore sharedData;

        public EventHandler<string>? TopFactionSet;

        public ICommand SetTopMostFaction { get; }
        public ICommand ClearTopMostFaction { get; }
        public ICommand AddIgnoredBounties { get; }
        public ICommand RemoveIgnoredBounties { get; }

        private IEnumerable<BountyVM> _bounties = [];

        private BountySorting sorting = BountySorting.Name; 
        public BountySorting Sorting
        {
            get => sorting;
            set
            {
                sorting = value;
                OnPropertyChanged(nameof(Sorting));
                OnPropertyChanged(nameof(Bounties));
            }
        }

        private string topFaction = string.Empty;
        public string TopFaction
        {
            get => topFaction;
            set
            {
                if (string.Equals(value, topFaction, StringComparison.OrdinalIgnoreCase))
                    return;

                topFaction = value;
                TopFactionSet?.Invoke(this, TopFaction);
                OnPropertyChanged(nameof(TopFaction));
                OnPropertyChanged(nameof(Bounties));
            }
        }

        public IEnumerable<BountyVM> Bounties
        {
            get
            {
                return sorting switch
                {
                    BountySorting.Value => _bounties.OrderByDescending(x => string.Equals(x.Name, TopFaction)).ThenBy(x => x.Value),
                    _ => _bounties.OrderByDescending(x => string.Equals(x.Name, TopFaction)).ThenBy(x => x.Name),
                };
            }
        }

        public string TotalBVCount => _bounties.Any() ? $"{_bounties.Sum(x => x.CountInt):N0}" : "0";
        public string TotalBVValue => _bounties.Any() ? $"{_bounties.Sum(x => x.ValueLong):N0}" : "0";
        public void UpdateBounties()
        {
            _bounties = sharedData.GetBounties().Select(x => new BountyVM(x));
            OnPropertyChanged(nameof(Bounties));
            OnPropertyChanged(nameof(TotalBVCount));
            OnPropertyChanged(nameof(TotalBVValue));
        }

        private void OnBountiesUpdated(object? sender, EventArgs e)
        {
            UpdateBounties();
        }

        private void OnStoreLive(object? sender, bool e)
        {
            if (e == false)
                return;

            UpdateBounties();
        }

        private void OnSetTopMostFaction(string? obj)
        {
            TopFaction = string.IsNullOrEmpty(obj) ? string.Empty : obj;
        }

        private void OnAddIgnoredBounty(string obj)
        {
            var messageBox = ODDialogService.ShowWithOwner(null, "Add To Ignored Bounties?", $"Ignore bounties from {obj} before now?", System.Windows.MessageBoxButton.YesNo);
            if (messageBox == System.Windows.MessageBoxResult.Yes)
            {
                sharedData.AddIgnoredBountyFaction(obj);
            }

        }

        private void OnRemoveIgnoredBounty(string obj)
        {
            sharedData.RemoveIgnoreBountyFaction(obj);
        }

        public void Dispose()
        {
            this.sharedData.BountiesUpdated -= OnBountiesUpdated;
            this.sharedData.StoreLive -= OnStoreLive;
        }
    }
}
