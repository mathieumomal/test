
namespace Etsi.Ultimate.Business.ItuRecommendation
{
    /// <summary>
    /// Record that will be exported to Excel by ItuPreliminaryExporter.
    /// </summary>
    public class ItuPreliminaryRecord
    {
        /// <summary>
        /// Gets or sets the specification identifier.
        /// </summary>
        public int SpecificationId { get; set; }

        /// <summary>
        /// Column: Type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Column: Specification
        /// </summary>
        public string SpecificationNumber { get; set; }
        
        /// <summary>
        /// Column: Title
        /// </summary>
        public string Title { get; set; }
    }

    public class ItuPreliminaryColumn
    {
        /// <summary>
        /// Column index
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Column name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Column width
        /// </summary>
        public int Width { get; set; }
    }

    public static class ItuPreliminaryRecordConfiguration
    {
        #region constants
        public const int RowHeader = 1;
        public const int ColumnStart = 1;
        public const int ColumnEnd = 3;

        //Style
        public const int FontSize = 8;
        public const string Font = "Arial";
        public const int Scale = 100;
        public const int DefaultColWidth = 10;
        public const int DefaultColHeight = 12;
        #endregion

        public static ItuPreliminaryColumn Type { get { return new ItuPreliminaryColumn{ Index = 1, Name = "Type", Width = 10}; }}
        public static ItuPreliminaryColumn SpecNumber { get { return new ItuPreliminaryColumn { Index = 2, Name = "Spec #", Width = 10 }; } }
        public static ItuPreliminaryColumn Title { get { return new ItuPreliminaryColumn { Index = 3, Name = "Title", Width = 30 }; } }
    }
}
