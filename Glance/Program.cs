using Spectre.Console;
using System.Text;

namespace Glance
{
    internal class Program
    {
        static void Main(string[] args)
        {
            InitWindow();

            BatteryInfo batteryInfo = new();
            batteryInfo.Update();

            MachineInfo machineInfo = new();
            machineInfo.Update();

            CPUInfo cpuInfo = new();
            cpuInfo.Update();

            GPUInfo gpuInfo = new();
            gpuInfo.Update();

            var layout = new Layout("Root")
                .SplitColumns(
                    new Layout("Left")
                        .SplitRows(
                            new Layout(CreateCPUPanel(cpuInfo)),
                            new Layout(CreateGPUPanel(gpuInfo))),
                    new Layout("Right")
                        .SplitRows(
                            new Layout(CreateBatteryPanel(batteryInfo)).Size(5),
                            new Layout(CreateMachinePanel(machineInfo)).Size(7),
                            new Layout("Bottom").Ratio(2)));
            layout["Left"].Ratio(2);

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
            Console.CursorVisible = false;
            Console.Title = "Glance";
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
                batteryGrid.AddRow(batteryChart, new Markup($"[{batteryColor.ToMarkup()}]{batteryInfo.Percentage}%[/] [orange1]:high_voltage:[/]"));
            }
            else
            {
                batteryGrid.AddRow(batteryChart, new Markup($"[{batteryColor.ToMarkup()}]{batteryInfo.Percentage}%[/] :battery:"));
            }

            Spectre.Console.Panel batteryPanel = new(Align.Center(batteryGrid, VerticalAlignment.Top))
            {
                Header = new PanelHeader("// Battery Status //").Centered(),
                Padding = new Spectre.Console.Padding(2, 1, 2, 1),
                Border = BoxBorder.Heavy,
            };
            return batteryPanel;
        }
        static Spectre.Console.Panel CreateMachinePanel(MachineInfo machineInfo)
        {
            Grid machineGrid = new();
            machineGrid.AddColumn();

            machineGrid.AddRow(new Markup($"[dim]USER[/] [bold]{machineInfo.UserName}[/]"));
            machineGrid.AddRow(new Markup($"[dim]CODE[/] [bold]{machineInfo.DesktopName}[/]"));
            machineGrid.AddRow(new Markup($"[dim]  OS[/] [bold]{machineInfo.OSVersion}[/]"));

            Spectre.Console.Panel machinePanel = new(Align.Center(machineGrid, VerticalAlignment.Top))
            {
                Header = new PanelHeader("// Machine Info //").Centered(),
                Padding = new Spectre.Console.Padding(2, 1, 2, 1),
                Border = BoxBorder.Heavy,
            };
            return machinePanel;
        }
        static Spectre.Console.Panel CreateCPUPanel(CPUInfo cpuInfo)
        {
            Grid cpuGrid = new();
            cpuGrid.AddColumn();

            cpuGrid.AddRow(new Markup($"[dim]NAME[/] [bold]{cpuInfo.Name}[/]"));

            Spectre.Console.Panel gpuPanel = new(Align.Center(cpuGrid, VerticalAlignment.Top))
            {
                Header = new PanelHeader("// CPU Info //").Centered(),
                Padding = new Spectre.Console.Padding(2, 1, 2, 1),
                Border = BoxBorder.Heavy,
                Expand = true,
            };
            return gpuPanel;
        }
        static Spectre.Console.Panel CreateGPUPanel(GPUInfo gpuInfo)
        {
            Grid gpuGrid = new();
            gpuGrid.AddColumn();

            gpuGrid.AddRow(new Markup($"[dim]  NAME[/] [bold]{gpuInfo.Name}[/]"));
            gpuGrid.AddRow(new Markup($"[dim]DRIVER[/] [bold]{gpuInfo.DriverVersion}[/]"));

            Spectre.Console.Panel gpuPanel = new(Align.Center(gpuGrid, VerticalAlignment.Top))
            {
                Header = new PanelHeader("// GPU Info //").Centered(),
                Padding = new Spectre.Console.Padding(2, 1, 2, 1),
                Border = BoxBorder.Heavy,
                Expand = true,
            };
            return gpuPanel;
        }
    }
}