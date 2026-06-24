using System;
using System.Diagnostics;
using Gtk;
using System.Text.Json;
using System.IO;

public class BuildInfo
{
    public string name { get; set; }
    public string platform_build { get; set; }
    public string version { get; set; }
    public string programm_lang { get; set; }
    public string build_by { get; set; }
    public string welcome { get; set; }
}

public class RootConfig
{
    public string programm_lib { get; set; }
    public string method { get; set; }
    public BuildInfo build { get; set; }
}

class PowerShell
{
    public static string pwsh()
    {
        string json = File.ReadAllText("libpwsh.json");
        var libpwsh = JsonSerializer.Deserialize<RootConfig>(json);

        return $"{libpwsh.build.welcome}\n" +
               $"made by {libpwsh.build.build_by}\n" +
               $"written on {libpwsh.build.programm_lang}\n" +
               $"your platform: {libpwsh.build.platform_build}\n";
    }
}

class Program
{
    static void Main()
    {
        Application.Init();

        var win = new Window("PowerLinux - pwsh GUI");
        win.SetDefaultSize(600, 400);
        win.DeleteEvent += (o, e) => Application.Quit();

        var mainBox = new Box(Orientation.Vertical, 10);

        var css = new CssProvider();
        css.LoadFromData(@"
            box {
                background-color: #1a237e;
            }
        ");
        StyleContext.AddProviderForScreen(win.Screen, css, 800);
        
        var outputView = new TextView();
        outputView.Editable = false;
        outputView.WrapMode = WrapMode.Word;
        var buffer = outputView.Buffer;

        string output = PowerShell.pwsh();
        buffer.InsertAtCursor(output);
        buffer.InsertAtCursor("\nType 'exit' to close.\n");

        var scroll = new ScrolledWindow();
        scroll.Add(outputView);
        scroll.SetSizeRequest(600, 300);

        var hbox = new Box(Orientation.Horizontal, 5);
        var entry = new Entry();
        entry.PlaceholderText = "Type your command..";

        var btnSend = new Button("Send");

        btnSend.Clicked += (s, e) =>
        {
            string cmd = entry.Text;
            if (!string.IsNullOrEmpty(cmd))
            {
                buffer.InsertAtCursor($"PS> {cmd}\n");

                if (cmd.ToLower() == "exit")
                {
                    Application.Quit();
                    return;
                }

                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "pwsh",
                            Arguments = $"-c \"{cmd}\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    string outputCmd = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(outputCmd))
                        buffer.InsertAtCursor(outputCmd);
                    if (!string.IsNullOrEmpty(error))
                        buffer.InsertAtCursor($"Error: {error}\n");

                    buffer.InsertAtCursor("\n");
                }
                catch (Exception ex)
                {
                    buffer.InsertAtCursor($"Error: {ex.Message}\n\n");
                }

                entry.Text = "";
            }
        };

        entry.Activated += (s, e) => btnSend.Click();

        hbox.PackStart(entry, true, true, 0);
        hbox.PackStart(btnSend, false, false, 0);

        mainBox.PackStart(scroll, true, true, 0);
        mainBox.PackStart(hbox, false, false, 0);

        win.Add(mainBox);
        win.ShowAll();

        Application.Run();
    }
}
