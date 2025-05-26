using ODEliteTracker.Models.Galaxy;

namespace ODEliteTracker.ViewModels.ModelViews.Galaxy
{
    public sealed class SystemPositionVM(Position pos)
    {
        public double X { get; } = pos.X;
        public string XString => X.ToString("N3");
        public double Y { get; } = pos.Y;
        public string YString => Y.ToString("N3");
        public double Z { get;  } = pos.Z;
        public string ZString => Z.ToString("N3");
    }
}
