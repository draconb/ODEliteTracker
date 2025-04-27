using EliteJournalReader;
using ODEliteTracker.Models.PowerPlay;
using ODMVVM.Helpers;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.PowerPlay
{
    public sealed class PowerPlayCycleDataVM : ODObservableObject
    {
        private readonly PowerplayCycleData data;

        public PowerPlayCycleDataVM(PowerplayCycleData data) 
        {
            this.data = data;

            foreach (var item in data.GoodsCollected)
            {
                GoodsCollected.Add(new(item.Key, item.Value));
            }

            foreach (var item in data.GoodsDelivered)
            {
                GoodsDelivered.Add(new(item.Key, item.Value));
            }

            if (data.PowerConflict != null && data.PowerConflict.Count > 0)
            {
                var ordered = data.PowerConflict.OrderByDescending(x => x.ConflictProgress).ToList();

                var winning = ordered.First();
                ordered.Remove(winning);

                PowerPlayConflictData = new PowerPlayConflictDataVM()
                {
                    WinningPower = new(winning)
                };

                if(ordered.Count == 0)
                {
                    PowerPlayConflictData.LosingPowers = [new(new("No Opposing Power", -1))];
                    PowerPlayConflictData.ConflictState = "Expansion";
                    return;
                }

                PowerPlayConflictData.LosingPowers = [.. ordered.Select(x => new PowerPlayConflictVM(x))];
                PowerPlayConflictData.ConflictState = "Contested";
            }
        }

        public string MeritsEarned => $"{data.MeritsEarned:N0}";
        public string ControllingPower => data.ControllingPower ?? "";
        public PowerplayState PowerState => data.PowerState;
        public string PowerStateString => data.PowerState.GetEnumDescription();
        public string PowerplayStateControlProgress => $"{data.PowerplayStateControlProgress * 100:N0}";
        public string PowerplayStateReinforcement => $"{data.PowerplayStateReinforcement * 100:N0}";
        public string PowerplayStateUndermining => $"{data.PowerplayStateUndermining * 100:N0}";
        public List<PowerPlayItemVM> GoodsCollected { get; } = [];
        public List<PowerPlayItemVM> GoodsDelivered { get; } = [];
        public List<string>? Powers => data.Powers;
        public PowerPlayConflictDataVM? PowerPlayConflictData { get; set; }
    }
}
