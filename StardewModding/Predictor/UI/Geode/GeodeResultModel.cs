namespace Dannnno.StardewMods.Predictor.UI
{
    internal struct GeodeResultModel
    {
        public int ParentGeodeId { get; set; }

        public int TreasureId { get; set; }

        public string DisplayName { get; set; }

        public string HoverText { get; set; }

        public bool AlreadyDonatedToMuseum { get; set; }

        public bool HistoricalResult { get; set; }

        public int Index { get; set; }

        public bool Current { get; internal set; }
    }
}
