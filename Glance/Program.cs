using Spectre.Console;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Glance
{
    internal class Program
    {
        static void Main(string[] args)
        {
            InitWindow();

            BatteryInfo batteryInfo = new();
            batteryInfo.Update();

            var layout = new Layout("Root")
                .SplitColumns(
                    new Layout("Left"),
                    new Layout("Right")
                .SplitRows(
                    new Layout("Top"),
                    new Layout("Bottom")));

            layout["Top"].Update(CreateBatteryPanel(batteryInfo)).MinimumSize(5).Ratio(1);
            layout["Right"].Ratio(1);

            while (true)
            {
                AnsiConsole.Write(layout);

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Spacebar)
                {
                    Console.Clear();
                }
            }
        }
        static void InitWindow()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Glance";

            const int MF_BYCOMMAND = 0x00000000;
            const int SC_MINIMIZE = 0xF020;
            const int SC_MAXIMIZE = 0xF030;
            const int SC_SIZE = 0xF000;

            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);        // Disable resizing
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);    // Disable minimizing
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);    // Disable maximizing
        }
        static Spectre.Console.Panel CreateBatteryPanel(BatteryInfo batteryInfo)
        {
            Spectre.Console.Color batteryColor = Spectre.Console.Color.Blue;
            if (batteryInfo.Percentage < 20)
            {
                batteryColor = Spectre.Console.Color.Red;
            }
            else if (20 <= batteryInfo.Percentage && batteryInfo.Percentage < 50)
            {
                batteryColor = Spectre.Console.Color.Orange1;
            }

            BreakdownChart batteryChart = new()
            {
                Width = 25,
            };
            batteryChart.HideTags();
            batteryChart.AddItem("Battery", batteryInfo.Percentage, batteryColor);
            batteryChart.AddItem("Else", 100 - batteryInfo.Percentage, Spectre.Console.Color.Grey37);

            Grid batteryGrid = new();

            batteryGrid.AddColumn();
            batteryGrid.AddColumn();

            if (batteryInfo.Charging)
            {
                batteryGrid.AddRow(batteryChart, new Markup($"[{batteryColor.ToMarkup()}]{batteryInfo.Percentage}%[/] :battery:"));
            }
            else
            {
                batteryGrid.AddRow(batteryChart, new Markup($"[{batteryColor.ToMarkup()}]{batteryInfo.Percentage}%[/]"));
            }

            Spectre.Console.Panel batteryPanel = new(Align.Center(batteryGrid, VerticalAlignment.Top))
            {
                Header = new PanelHeader("// Battery Status //").Centered(),
                Padding = new Spectre.Console.Padding(2, 1, 2, 1),
                Border = BoxBorder.Heavy,
            };
            return batteryPanel;
        }
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
    }
}