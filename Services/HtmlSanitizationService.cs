using Ganss.Xss;

namespace OnlineBloggingPlatform.Services
{
    public interface IHtmlSanitizationService
    {
        string Sanitize(string html);
        string SanitizeForDisplay(string html);
    }

    public class HtmlSanitizationService : IHtmlSanitizationService
    {
        private readonly HtmlSanitizer _sanitizer;
        private readonly HtmlSanitizer _displaySanitizer;

        public HtmlSanitizationService()
        {
            // Configure sanitizer for content editing (more permissive)
            _sanitizer = new HtmlSanitizer();
            
            // Allow common HTML tags for rich text content
            _sanitizer.AllowedTags.Clear();
            _sanitizer.AllowedTags.UnionWith(new[] {
                "p", "br", "strong", "em", "u", "h1", "h2", "h3", "h4", "h5", "h6",
                "ul", "ol", "li", "a", "img", "blockquote", "div", "span",
                "table", "thead", "tbody", "tr", "td", "th", "pre", "code"
            });

            // Allow safe attributes
            _sanitizer.AllowedAttributes.Clear();
            _sanitizer.AllowedAttributes.UnionWith(new[] {
                "href", "src", "alt", "title", "class", "style"
            });

            // Configure safer CSS properties
            _sanitizer.AllowedCssProperties.Clear();
            _sanitizer.AllowedCssProperties.UnionWith(new[] {
                "color", "background-color", "font-size", "font-weight", "text-align",
                "margin", "padding", "border", "width", "height"
            });

            // Remove dangerous protocols
            _sanitizer.AllowedSchemes.Clear();
            _sanitizer.AllowedSchemes.UnionWith(new[] { "http", "https", "mailto" });

            // Configure display sanitizer (more restrictive for user-generated content like comments)
            _displaySanitizer = new HtmlSanitizer();
            _displaySanitizer.AllowedTags.Clear();
            _displaySanitizer.AllowedTags.UnionWith(new[] {
                "p", "br", "strong", "em", "u", "a"
            });

            _displaySanitizer.AllowedAttributes.Clear();
            _displaySanitizer.AllowedAttributes.Add("href");

            _displaySanitizer.AllowedSchemes.Clear();
            _displaySanitizer.AllowedSchemes.UnionWith(new[] { "http", "https", "mailto" });
        }

        public string Sanitize(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            return _sanitizer.Sanitize(html);
        }

        public string SanitizeForDisplay(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            return _displaySanitizer.Sanitize(html);
        }
    }
}