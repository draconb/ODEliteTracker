using ODEliteTracker.Stores;
using ODEliteTracker.ViewModels.PopOuts;
using ODEliteTracker.Views;
using System.Windows;

namespace ODEliteTracker.Services
{
    public sealed class PopOutService(Func<Type, PopOutViewModel> popOutFactory, SettingsStore setting)
    {
        private readonly Func<Type, PopOutViewModel> popOutFactory = popOutFactory;
        private readonly SettingsStore setting = setting;
        private readonly List<PopOutViewModel> activeViews = [];

        public event EventHandler? PopOutsUpdated;
        public IEnumerable<PopOutViewModel> ActiveViews => activeViews.OrderBy(x => x.Name).ThenBy(x => x.Count);

        public void OpenPopOut(Type type, int commanderID, int count = 0)
        {
            if (type.BaseType != typeof(PopOutViewModel))
            {
                return;
            }

            var newView = popOutFactory.Invoke(type);

            if (newView == null)
                return;

            OpenPopOutActual(newView, commanderID, count);
        }

        public void OpenPopOut<TPopOutViewModel>(int commanderID) where TPopOutViewModel : PopOutViewModel
        {
            var newView = popOutFactory.Invoke(typeof(TPopOutViewModel));

            if (newView == null)
                return;

            OpenPopOutActual(newView, commanderID);
        }

        private void OpenPopOutActual(PopOutViewModel newView, int commanderID, int count = 0)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var popOutParams = setting.GetParams(newView, count, commanderID);
                newView.ApplyParams(popOutParams);
                if (newView.Position.IsZero)
                    newView.OnResetPosition(null);
                newView.WindowClosed += OnPopOutWindowClosed;
                activeViews.Add(newView);

                var view = new PopOutWindow(newView);

                view.Show();

                PopOutsUpdated?.Invoke(this, EventArgs.Empty);
            });
        }

        private void OnPopOutWindowClosed(object? sender, PopOutViewModel e)
        {
            setting.SaveParams(e, false, setting.SelectedCommanderID);
            activeViews.Remove(e);
            PopOutsUpdated?.Invoke(this, EventArgs.Empty);
        }
        
        public void OpenSavedViews(int commanderID)
        {
            if (commanderID == 0)
                return;

            var views = setting.GetCommanderPopOutParams(commanderID).Where(x => x.Active);

            if(views == null || !views.Any()) 
                return;

            foreach (var view in views)
            {
                if (view.Type == null)
                    continue;

                OpenPopOut(view.Type, commanderID, view.Count);
            }
        }

        public void CloseViews(int commanderID)
        {
            if (activeViews.Count == 0 || commanderID == 0)
                return;

            foreach (var view in activeViews)
            {
                setting.SaveParams(view, true, commanderID);
                view.CloseWindow();                
            }
            activeViews.Clear();
        }
    }
}
