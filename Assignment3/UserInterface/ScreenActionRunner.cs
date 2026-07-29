namespace GourmetSpot.UserInterface
{
    internal static class ScreenActionRunner
    {
        public static bool TryRun(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Input/output error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
            return false;
        }
    }
}
