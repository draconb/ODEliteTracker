using EliteJournalReader.Events;

namespace ODEliteTracker.Models.Colonisation
{
    public sealed class ConstructionResource
    {
        public ConstructionResource(ColonisationResource resource)
        {
            FDEVName = resource.Name;
            if(string.IsNullOrEmpty(resource.Name_Localised) == false)
            {
                LocalName = resource.Name_Localised;
            }
            RequiredAmount = resource.RequiredAmount;
            ProvidedAmount = resource.ProvidedAmount;
            Payment = resource.Payment;
        }

        public string FDEVName { get; set; }

        private string? localName;
        public string? LocalName
        {
            get
            {
                if (string.IsNullOrEmpty(localName))
                    return FDEVName;
                return localName;
            }
            set
            {
                localName = value;
            }
        }
        public int RequiredAmount { get; set; }
        public int ProvidedAmount { get; set; }
        public int Payment { get; set; }

        internal bool Update(ColonisationResource resource)
        {
            if (string.IsNullOrEmpty(resource.Name_Localised) == false)
            {
                LocalName = resource.Name_Localised;
            }

            bool updated = RequiredAmount != resource.RequiredAmount || ProvidedAmount != resource.ProvidedAmount;

            if (updated == false)
                return false;

            RequiredAmount = resource.RequiredAmount;
            ProvidedAmount = resource.ProvidedAmount;
            Payment = resource.Payment;

            return updated;
        }
    }
}
