using System.Runtime.InteropServices;

namespace TtsTray;

/// <summary>Голос SAPI для списка: Id токена, отображаемое имя и группа ("Silero" | "Microsoft" | "Другие").</summary>
public sealed record VoiceItem(string Id, string Name, string Group);

/// <summary>
/// Обёртка над одним экземпляром COM SAPI.SpVoice на всё время работы программы.
/// Создавать и вызывать только из UI-потока (STA).
/// </summary>
/// <remarks>
/// System.Speech не используется: с голосами Silero он выдаёт тишину.
/// Окончание чтения определяется опросом WaitUntilDone(0): Status.RunningState ненадёжен.
/// </remarks>
public sealed class SapiSpeaker : IDisposable
{
    // Флаги SpeechVoiceSpeakFlags.
    private const int SvsfAsync = 1;
    private const int SvsfPurgeBeforeSpeak = 2;

    private readonly dynamic voice;
    private readonly Dictionary<string, object> tokens = new();
    private readonly string? defaultVoiceId;
    private readonly System.Windows.Forms.Timer timer;

    public SapiSpeaker()
    {
        this.voice = Activator.CreateInstance(Type.GetTypeFromProgID("SAPI.SpVoice", throwOnError: true)!)!;

        var voices = new List<VoiceItem>();
        string? anyRussianSileroId = null;
        foreach (dynamic token in this.voice.GetVoices())
        {
            string id = token.Id;
            string name = token.GetDescription();
            string group = id.Contains(@"TokenEnums\SileroTTS", StringComparison.OrdinalIgnoreCase) ? "Silero"
                : name.StartsWith("Microsoft ", StringComparison.Ordinal) ? "Microsoft"
                : "Другие";

            string language;
            try
            {
                language = token.GetAttribute("Language");
            }
            catch (COMException)
            {
                language = "";
            }

            // Голос по умолчанию — первый русский голос Silero. LCID 0x419 есть и у чувашского, удмуртского
            // и других голосов Silero, поэтому сначала ищем «(Russian)» в имени, а 0x419 — запасной вариант.
            if (group == "Silero" && language.Split(';').Contains("419", StringComparer.OrdinalIgnoreCase))
            {
                anyRussianSileroId ??= id;
                if (this.defaultVoiceId == null && name.EndsWith("(Russian)", StringComparison.Ordinal))
                {
                    this.defaultVoiceId = id;
                }
            }

            this.tokens[id] = token;
            voices.Add(new VoiceItem(id, name, group));
        }

        var groupOrder = new[] { "Silero", "Microsoft", "Другие" };
        this.Voices = voices.OrderBy(v => Array.IndexOf(groupOrder, v.Group)).ToList();
        this.defaultVoiceId ??= anyRussianSileroId ?? this.Voices.FirstOrDefault()?.Id;

        this.timer = new System.Windows.Forms.Timer { Interval = 200 };
        this.timer.Tick += (_, _) =>
        {
            if (!(this.IsBusy && !this.IsPaused) && !this.IsWarmingUp)
            {
                return;
            }

            if (!(bool)this.voice.WaitUntilDone(0))
            {
                return;
            }

            if (this.IsWarmingUp)
            {
                // Прогрев шёл в память — возвращаем вывод на динамики.
                this.voice.AudioOutputStream = null;
                this.IsWarmingUp = false;
            }

            this.IsBusy = false;
            this.StateChanged?.Invoke(this, EventArgs.Empty);
        };
        this.timer.Start();
    }

    public IReadOnlyList<VoiceItem> Voices { get; }

    public string? CurrentVoiceId { get; private set; }

    public int Rate
    {
        get => this.voice.Rate;
        set => this.voice.Rate = Math.Clamp(value, -10, 10);
    }

    public int Volume
    {
        get => this.voice.Volume;
        set => this.voice.Volume = Math.Clamp(value, 0, 100);
    }

    /// <summary>Идёт чтение пользователя (в том числе на паузе). Прогрев не считается.</summary>
    public bool IsBusy { get; private set; }

    public bool IsPaused { get; private set; }

    public bool IsWarmingUp { get; private set; }

    /// <summary>Поднимается в UI-потоке при смене IsBusy / IsPaused / IsWarmingUp.</summary>
    public event EventHandler? StateChanged;

    /// <summary>Выбирает голос по Id. false — Id не найден или null, выбран голос по умолчанию.</summary>
    public bool SelectVoice(string? id)
    {
        bool found = id != null && this.tokens.ContainsKey(id);
        string? targetId = found ? id : this.defaultVoiceId;
        if (targetId == null || targetId == this.CurrentVoiceId)
        {
            return found;
        }

        // Смена голоса посреди синтеза у SAPI не определена — сначала останавливаем.
        if (this.IsBusy || this.IsWarmingUp)
        {
            this.Stop();
        }

        this.voice.Voice = this.tokens[targetId];
        this.CurrentVoiceId = targetId;
        return found;
    }

    /// <summary>Прерывает текущее чтение (и прогрев), снимает паузу и читает текст на динамики.</summary>
    public void Speak(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || this.CurrentVoiceId == null)
        {
            return;
        }

        if (this.IsWarmingUp)
        {
            this.voice.Speak("", SvsfPurgeBeforeSpeak);
            this.voice.AudioOutputStream = null;
            this.IsWarmingUp = false;
        }

        this.voice.Speak(text, SvsfAsync | SvsfPurgeBeforeSpeak);

        // Ловушка SAPI: после очистки очереди голос остаётся на паузе, и новый текст молчит до Resume().
        if (this.IsPaused)
        {
            this.voice.Resume();
            this.IsPaused = false;
        }

        this.IsBusy = true;
        this.StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Pause()
    {
        // Пауза в SAPI — счётчик: второй Pause() потребовал бы второго Resume().
        if (!this.IsBusy || this.IsPaused)
        {
            return;
        }

        this.voice.Pause();
        this.IsPaused = true;
        this.StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Resume()
    {
        if (!this.IsPaused)
        {
            return;
        }

        this.voice.Resume();
        this.IsPaused = false;
        this.StateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Останавливает чтение и прогрев; голос не остаётся на паузе.</summary>
    public void Stop()
    {
        if (!this.IsBusy && !this.IsWarmingUp)
        {
            return;
        }

        this.voice.Speak("", SvsfPurgeBeforeSpeak);
        if (this.IsPaused)
        {
            this.voice.Resume();
        }

        if (this.IsWarmingUp)
        {
            this.voice.AudioOutputStream = null;
        }

        this.IsBusy = false;
        this.IsPaused = false;
        this.IsWarmingUp = false;
        this.StateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Загружает модель выбранного голоса фоновым синтезом короткой фразы в память,
    /// чтобы первое настоящее чтение не ждало ~3–9 секунд.
    /// </summary>
    public void WarmUp()
    {
        if (this.IsBusy || this.CurrentVoiceId == null)
        {
            return;
        }

        if (!this.IsWarmingUp)
        {
            this.voice.AudioOutputStream = Activator.CreateInstance(Type.GetTypeFromProgID("SAPI.SpMemoryStream", throwOnError: true)!);
        }

        this.voice.Speak("Прогрев.", SvsfAsync | SvsfPurgeBeforeSpeak);
        this.IsWarmingUp = true;
        this.StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        this.timer.Stop();
        this.timer.Dispose();
        this.Stop();
        Marshal.FinalReleaseComObject((object)this.voice);
    }
}
