namespace OCRApp.Wasm
{
    public sealed class Program
    {
        private static App? _app;

        public static int Main(string[] args)
        {
            Microsoft.UI.Xaml.Application.Start(_ => _app = new AppHead());

            return 0;
        }
    }
}
