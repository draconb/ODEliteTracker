using EliteJournalReader.Events;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.PowerPlay
{
    public sealed class PowerPlayConflictVM(PowerConflict data) :ODObservableObject
    {
        public string Power => data.Power;
        public string Progress
        {
            get
            {
                if(data.ConflictProgress < 0)
                    return string.Empty;
                return $"{data.ConflictProgress * 100:N2} %";
            }
        }

        public void Update()
        {
            OnPropertyChanged(nameof(Power));
            OnPropertyChanged(nameof(Progress));
        }
    }
}
