namespace gRPCSample.Core.Helpers
{
    public static class MyConsole
    {
        /// <summary>
        /// With color
        /// </summary>
        /// <param name="color"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteLine(ConsoleColor color, string format)
        {
            // Remember the current foreground color
            ConsoleColor currentColor = Console.ForegroundColor;

            // Set the new foreground color
            Console.ForegroundColor = color;

            // Write the formatted string to the console
            Console.WriteLine(format);

            // Reset the foreground color to the original
            Console.ResetColor();
        }

        public static void WriteLine(string format)
        {
            // Write the formatted string to the console
            Console.WriteLine(format);
        }
    }
}
