namespace ODEliteTracker.ViewModels.ModelViews.BGS
{
    public sealed class FactionWarVM
    {
        public int LowSpaceCZ { get; set; }
        public int MediumSpaceCZ { get; set; }
        public int HighSpaceCZ { get; set; }
        public int LowGroundCZ { get; set; }
        public int MediumGroundCZ { get; set; }
        public int HighGroundCZ { get; set; }

        public int Total => LowSpaceCZ + MediumSpaceCZ + HighSpaceCZ + LowGroundCZ + MediumGroundCZ + HighGroundCZ;
        public override string ToString()
        {
            return $"{Total:N0}";
        }
    }
}
