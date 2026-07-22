namespace FreelanceHub.Web.ViewModels
{
    /// <summary>
    /// ViewModel for the reusable _SelectItemsModal partial.
    /// Pass this to render a modal for selecting categories, tags, or skills.
    /// </summary>
    public class SelectItemsModalViewModel
    {
        /// <summary>
        /// Unique ID for the modal (e.g., "categoryModal", "tagModal", "skillModal")
        /// </summary>
        public string ModalId { get; set; } = string.Empty;

        /// <summary>
        /// Title displayed in the modal header (e.g., "Select Categories")
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Type of items being selected: "category", "tag", or "skill"
        /// Used to identify which items are selected and for CSS classes
        /// </summary>
        public string ItemType { get; set; } = string.Empty;

        /// <summary>
        /// Collection of items to display in the modal.
        /// Each item must have Id and Name properties.
        /// </summary>
        public IEnumerable<SelectableItem> Items { get; set; } = new List<SelectableItem>();
    }

    /// <summary>
    /// Represents a single selectable item (category, tag, or skill).
    /// </summary>
    public class SelectableItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
