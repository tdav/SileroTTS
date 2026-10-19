namespace TtsTray;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var mutex = new Mutex(true, @"Local\TtsTray.SingleInstance", out bool createdNew);
        if (!createdNew)
        {
            // Программа уже запущена — просим первую копию показать окно и выходим.
            try
            {
                using var showEvent = EventWaitHandle.OpenExisting(@"Local\TtsTray.Show");
                showEvent.Set();
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                // Первая копия ещё не успела создать событие — просто выходим.
            }

            return;
        }

        ApplicationConfiguration.Initialize();

        var form = new MainForm(AppSettings.Load());

        // Создаём окно, не показывая его: handle нужен для NotifyIcon, BeginInvoke и слушателя буфера обмена.
        _ = form.Handle;

        using var showRequest = new EventWaitHandle(false, EventResetMode.AutoReset, @"Local\TtsTray.Show");
        var registration = ThreadPool.RegisterWaitForSingleObject(
            showRequest, (_, _) => form.BeginInvoke(form.ShowFromTray), null, Timeout.Infinite, executeOnlyOnce: false);

        // Цикл сообщений без главной формы: закрытие окна не завершает программу, выход — через меню трея.
        Application.Run();

        registration.Unregister(null);
        GC.KeepAlive(mutex);
    }
}
