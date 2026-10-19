using System.Text.Json;

namespace TtsTray;

/// <summary>Настройки программы; хранятся в %APPDATA%\TtsTray\settings.json.</summary>
public sealed class AppSettings
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TtsTray", "settings.json");

    /// <summary>Id токена голоса SAPI; null — голос по умолчанию.</summary>
    public string? VoiceId { get; set; }

    /// <summary>Скорость SAPI: -10..10.</summary>
    public int Rate { get; set; } = 0;

    /// <summary>Громкость SAPI: 0..100.</summary>
    public int Volume { get; set; } = 100;

    /// <summary>Текст из буфера читается, если слов больше этого числа.</summary>
    public int WordThreshold { get; set; } = 100;

    public bool AutoReadClipboard { get; set; } = true;

    public bool StartWithWindows { get; set; } = false;

    public static AppSettings Load()
    {
        AppSettings settings;
        try
        {
            settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath)) ?? new AppSettings();
        }
        catch (Exception e) when (e is IOException or JsonException or UnauthorizedAccessException)
        {
            // Нет файла или он испорчен — работаем с настройками по умолчанию.
            settings = new AppSettings();
        }

        if (settings.WordThreshold < 1)
        {
            settings.WordThreshold = 100;
        }

        settings.Rate = Math.Clamp(settings.Rate, -10, 10);
        settings.Volume = Math.Clamp(settings.Volume, 0, 100);
        return settings;
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }
}
