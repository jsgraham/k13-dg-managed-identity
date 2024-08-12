using CMS.Helpers;

namespace DancingGoat.Models.InlineEditors
{
    /// <summary>
    /// View model for Text editor.
    /// </summary>
    public sealed class TextEditorViewModel : InlineEditorViewModel
    {
        /// <summary>
        /// Editor text.
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        /// Placeholder text.
        /// </summary>
        public string PlaceholderText { get; set; } = ResHelper.GetString("DancingGoatMVC.InlineEditors.TextEditor.Placeholder");
    }
}