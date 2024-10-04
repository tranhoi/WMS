namespace WebUIFinal.Core.Dto
{
    public class PrintLabelInfoDto
    {
        /// <summary>
        /// Base64 code.
        /// </summary>
        public string QrValue { get; set; }
        public List<LineInfo> Lines { get; set; } = new List<LineInfo>();
    }

    public class LineInfo
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsVisible { get; set; }
    }
}
