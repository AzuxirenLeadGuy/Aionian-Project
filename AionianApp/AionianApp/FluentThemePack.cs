using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

using System.Linq;

namespace AionianApp;

public readonly record struct FluentThemePack
{
    public FluentTheme Theme { init; get; }
    public ThemeVariant Variant { init; get; }
    public void ApplyTheme(Application app)
    {
        FluentTheme theme = Theme;
        app.Styles.RemoveAll(app.Styles.Where(x => x is FluentTheme));	// Remove previously present FluentTheme
        app.Styles.Add(theme);
        app.RequestedThemeVariant = this.Variant;
    }
    public static FluentThemePack DefaultTheme => new()
    {
        Theme = new() { DensityStyle = DensityStyle.Compact },
        Variant = ThemeVariant.Default
    };
    public static FluentThemePack Theme_01_Light => new()
    {
        Theme = new FluentTheme()
        {
            Palettes =
            {
                [ThemeVariant.Light] = new ColorPaletteResources()
                {
                    Accent=Color.FromRgb( 0x89, 0x61, 0xcc),
                    AltHigh=Color.FromRgb(0xff, 0xff, 0xff),
                    AltLow=Color.FromRgb(0xff, 0xff, 0xff),
                    AltMedium=Color.FromRgb(0xff, 0xff, 0xff),
                    AltMediumHigh=Color.FromRgb(0xff, 0xff, 0xff),
                    AltMediumLow=Color.FromRgb(0xff, 0xff, 0xff),
                    BaseHigh=Color.FromRgb(0,0,0),
                    BaseLow=Color.FromRgb(0xee, 0xce, 0xff),
                    BaseMedium=Color.FromRgb(0xa9,0x87,0xbc),
                    BaseMediumHigh=Color.FromRgb(0x7b,0x58,0x90),
                    BaseMediumLow=Color.FromRgb(0x92,0x70,0xa6),
                    ChromeAltLow=Color.FromRgb(0x7b,0x58,0x90),
                    ChromeBlackHigh=Color.FromRgb(0,0,0),
                    ChromeBlackLow=Color.FromRgb(0xee,0xce,0xff),
                    ChromeBlackMedium=Color.FromRgb(0x7b,0x58,0x90),
                    ChromeBlackMediumLow=Color.FromRgb(0xa9,0x87,0xbc),
                    ChromeDisabledHigh=Color.FromRgb(0xee,0xce,0xff),
                    ChromeDisabledLow=Color.FromRgb(0xa9,0x87,0xbc),
                    ChromeGray=Color.FromRgb(0x92,0x70,0xa6),
                    ChromeHigh=Color.FromRgb(0xee,0xce,0xff),
                    ChromeLow=Color.FromRgb(0xfe,0xea,0xff),
                    ChromeMedium=Color.FromRgb(0xfb,0xe4,0xff),
                    ChromeMediumLow=Color.FromRgb(0xfe,0xea,0xff),
                    ChromeWhite=Color.FromRgb(0xff, 0xff, 0xff),
                    ListLow=Color.FromRgb(0xfb,0xe4,0xff),
                    ListMedium=Color.FromRgb(0xee,0xce,0xff),
                    RegionColor=Color.FromRgb(0xfe,0xf6,0xff),
                },
            },
            DensityStyle = DensityStyle.Compact
        },
        Variant = ThemeVariant.Light
    };
    public static FluentThemePack Theme_01_Dark => new()
    {
        Theme = new FluentTheme()
        {
            Palettes =
            {
                [ThemeVariant.Dark] = new ColorPaletteResources()
                {
                    Accent=Color.FromRgb( 0x5d, 0x4e, 0xa4),
                    AltHigh=Color.FromRgb(0,0,0),
                    AltLow=Color.FromRgb(0,0,0),
                    AltMedium=Color.FromRgb(0,0,0),
                    AltMediumHigh=Color.FromRgb(0,0,0),
                    AltMediumLow=Color.FromRgb(0,0,0),
                    BaseHigh=Color.FromRgb(0xff, 0xff, 0xff),
                    BaseLow=Color.FromRgb( 0x4d, 0x43, 0x53),
                    BaseMedium=Color.FromRgb( 0xab, 0xa0, 0xb0),
                    BaseMediumHigh=Color.FromRgb( 0xc2, 0xb7, 0xc7),
                    BaseMediumLow=Color.FromRgb( 0x7c, 0x71, 0x81),
                    ChromeAltLow=Color.FromRgb( 0xc2, 0xb7, 0xc7),
                    ChromeBlackHigh=Color.FromRgb(0,0,0),
                    ChromeBlackLow=Color.FromRgb( 0xc2, 0xb7, 0xc7),
                    ChromeBlackMedium=Color.FromRgb(0,0,0),
                    ChromeBlackMediumLow=Color.FromRgb(0,0,0),
                    ChromeDisabledHigh=Color.FromRgb( 0x4d, 0x43, 0x53),
                    ChromeDisabledLow=Color.FromRgb( 0xab, 0xa0, 0xb0),
                    ChromeGray=Color.FromRgb( 0x93, 0x89, 0x99),
                    ChromeHigh=Color.FromRgb( 0x93, 0x89, 0x99),
                    ChromeLow=Color.FromRgb( 0x27, 0x16, 0x34),
                    ChromeMedium=Color.FromRgb( 0x31, 0x21, 0x3c),
                    ChromeMediumLow=Color.FromRgb( 0x44, 0x38, 0x4b),
                    ChromeWhite=Color.FromRgb(0xff, 0xff, 0xff),
                    ListLow=Color.FromRgb( 0x31, 0x21, 0x3c),
                    ListMedium=Color.FromRgb( 0x4d, 0x43, 0x53),
                    RegionColor=Color.FromRgb( 0x26, 0x27, 0x38),
                },
            },
            DensityStyle = DensityStyle.Compact
        },
        Variant = ThemeVariant.Dark
    };
}
