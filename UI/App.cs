namespace UI
{
    public class App
    {
        private readonly ConsoleUI _ui;
        public App(ConsoleUI ui) => _ui = ui;
        public void Run() => _ui.ShowMainMenu();
    }
}
