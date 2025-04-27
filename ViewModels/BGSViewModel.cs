using ODEliteTracker.Stores;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels
{
    public sealed class BGSViewModel : ODViewModel
    {
        public BGSViewModel(BGSDataStore dataStore) 
        {
            this.dataStore = dataStore;

            this.dataStore.OnStoreLive += OnStoreLive;
        }

        private readonly BGSDataStore dataStore;
        public override bool IsLive => dataStore.IsLive;

        private void OnStoreLive(object? sender, bool e)
        {
            if (e)
            {
                OnModelLive(true);
            }
        }
    }
}
