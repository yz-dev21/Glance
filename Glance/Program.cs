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

            GPUInfo gpuInfo = new();

            DiskInfo diskInfo = new();

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
                            new Layout(CreateDiskPanel(diskInfo)),
                            new Layout(CreateCommandsPanel()).Size(9)));
            layout["Left"].Ratio(2);
            while (true)
            {
                AnsiConsole.Write(layout);

                ConsoleKeyInfo key;
                do
                {
                    Console.CursorVisible = false;
                    key = Console.ReadKey();
                }
                while (key.Key != ConsoleKey.F5);

                // Console.Clear() somehow does not clear screen completely, so I found this solution.
                Console.Write("\f\u001bc\x1b[3J");
            }
        }
        static void InitWindow()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            Console.Title = "Glance";
        }
        static Spectre.Console.Panel CreateCommandsPanel()
        {
            Rows rows = new
            (
                new Markup("[bold]'C'[/]       [dim italic];Focus on CPU tab[/]"),
                new Markup("[bold]'G'[/]       [dim italic];Focus on GPU tab[/]"),
                new Markup("[bold]'←' / '→'[/] [dim italic];Next or previous object[/]"),
                new Markup("[bold]'F5'[/]      [dim italic];Refresh screen[/]"),
                new Markup("[bold]'ESC'[/]     [dim italic];Visit GitHub[/]")
            );
            Spectre.Console.Panel commandsPanel = new(Align.Left(rows, VerticalAlignment.Top))
            {
                Header = new PanelHeader("[invert] COMMANDS [/]").Centered(),
                Padding = new Spectre.Console.Padding(2, 1, 2, 1),
                Border = BoxBorder.Heavy,
                Expand = true
            };

            return commandsPanel;
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
                Header = new PanelHeader("[invert] BATTERY [/]").Centered(),
                Padding = new Spectre.Console.Padding(2, 1, 2, 1),
                Border = BoxBorder.Heavy,
            };
            return batteryPanel;
        }
        static Spectre.Console.Panel CreateMachinePanel(MachineInfo machineInfo)
        {
            Grid machineGrid = new();
            machineGrid.AddColumn();

            machineGrid.AddRow(new Markup($"[dim]USER[/] {machineInfo.UserName}"));
            machineGrid.AddRow(new Markup($"[dim]CODE[/] {machineInfo.DesktopName}"));
            machineGrid.AddRow(new Markup($"[dim]  OS[/] {machineInfo.OSVersion}"));

            Spectre.Console.Panel machinePanel = new(Align.Center(machineGrid, VerticalAlignment.Top))
            {
                Header = new PanelHeader("[invert] MACHINE [/]").Centered(),
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
                Header = new PanelHeader("[invert] CPU [/]").Centered(),
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
                Header = new PanelHeader("[invert] GPU [/]").Centered(),
                Padding = new Spectre.Console.Padding(2, 1, 2, 1),
                Border = BoxBorder.Heavy,
                Expand = true,
            };
            return gpuPanel;
        }
        static Spectre.Console.Panel CreateDiskPanel(DiskInfo diskInfo)
        {
            Grid diskGrid = new();
            diskGrid.AddColumn();
            diskGrid.AddColumn();
            diskGrid.AddColumn();

            for (int i = 0; i < diskInfo.Drives.Length; i++)
            {
                BreakdownChart chart = new()
                {
                    Width = 25,
                };
                chart.HideTags();
                chart.AddItem("Busy", diskInfo.Drives[i].AvailableFreeSpace, Spectre.Console.Color.Orange1);
                chart.AddItem("Free", diskInfo.Drives[i].TotalSize - diskInfo.Drives[i].AvailableFreeSpace, Spectre.Console.Color.Grey37);

                diskGrid.AddRow(new Markup($":floppy_disk: {diskInfo.Drives[i].Name}"), chart, new Markup($"[orange1]{DiskInfo.ToGB(diskInfo.Drives[i].TotalSize - diskInfo.Drives[i].AvailableFreeSpace)}GB[/] / {DiskInfo.ToGB(diskInfo.Drives[i].TotalSize)}GB"));
                if (i != diskInfo.Drives.Length - 1)
                    diskGrid.AddRow();
            }
            Spectre.Console.Panel diskPanel = new(Align.Center(diskGrid, VerticalAlignment.Top))
            {
                Header = new PanelHeader("[invert] DISKS [/]").Centered(),
                Padding = new Spectre.Console.Padding(2, 1, 2, 1),
                Border = BoxBorder.Heavy,
                Expand = true,
            };
            return diskPanel;
        }
    }
}