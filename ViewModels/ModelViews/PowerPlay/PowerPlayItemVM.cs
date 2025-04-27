namespace ODEliteTracker.ViewModels.ModelViews.PowerPlay
{
    public class PowerPlayItemVM(string name, int count)
    {
        public string Name { get; private set; } = name;
        public int Count { get; private set; } = count;
    }
}
