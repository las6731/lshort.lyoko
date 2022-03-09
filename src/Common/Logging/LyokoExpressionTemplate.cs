using Serilog.Expressions;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace LShort.Lyoko.Common.Logging;

/// <summary>
/// A basic extension of the <see cref="ExpressionTemplate"/> from <see cref="Serilog.Expressions"/> that provide my
/// preferred defaults for console logging.
/// </summary>
public class LyokoExpressionTemplate : ExpressionTemplate
{
    private const string DefaultTemplate =
        "[{@t:HH:mm:ss} {@l:u3}]{#if SourceContext is not null} {SourceContext}{#if Rest(true)[?] is not null} - {Rest(true)}{#end}:{#end} {@m}\n{@x}";

    private static readonly TemplateTheme DefaultTheme = TemplateTheme.Code;

    /// <inheritdoc />
    public LyokoExpressionTemplate(string? template = null, IFormatProvider? formatProvider = null, NameResolver? nameResolver = null, TemplateTheme? theme = null, bool applyThemeWhenOutputIsRedirected = false)
        : base(template ?? DefaultTemplate, formatProvider, nameResolver, theme ?? DefaultTheme, applyThemeWhenOutputIsRedirected)
    {
    }
}