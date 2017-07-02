using System.Text;
namespace NppPluginNET.Core.Formatter
{
    public interface IFormatter
    {
        StringBuilder FormattedBuffer { get; }
        bool IsLoaded { get; }
        StringBuilder TextBuffer { get; set; }
        void FormatText();
    }
}
