using Spectre.Console;

namespace Winreels.Interface;

/// <summary>
/// This class is used to create TUIs that can have multiple scenes.
/// The user can toggle between scenes by pressing Tab.
/// </summary>
public class MainInterface
{
    public int currentScene = 0;
    private readonly HashSet<string> scenes = [];
    public Action<string>? OnSceneSwitched;

    public static readonly Action<ConsoleKey, char>? OnInput;
    private static Thread? inputThread;
    private static Thread? resizeThread;
    private static int lastWidth;
    private static int lastHeight;

    // Adds a scene to this MainInterface.
    public MainInterface AddScene(string name)
    {
        scenes.Add(name);
        return this;
    }

    // Removes a scene from this MainInterface.
    public MainInterface RemoveScene(string name)
    {
        scenes.Remove(name);
        return this;
    }

    // Initializes and renders the first scene, call this after registering scenes and events.
    public void Initialize()
    {
        if (inputThread == null)
        {
            inputThread = new Thread(() =>
            {
                IAnsiConsoleInput input = AnsiConsole.Console.Input;

                while (true)
                {
                    if (input.IsKeyAvailable())
                    {
                        ConsoleKeyInfo? i = input.ReadKey(true);
                        if (i == null)
                            continue;

                        ConsoleKeyInfo info = i.GetValueOrDefault();
                        ConsoleKey key = info.Key;
                        char ch = info.KeyChar;
                        HandleInput(key, ch);
                        OnInput?.Invoke(key, ch);
                    }
                }
            });
            inputThread.Start();
        }
        if (resizeThread == null)
        {
            resizeThread = new Thread(() =>
            {
                lastWidth  = Console.BufferWidth;
                lastHeight = Console.BufferHeight;

                while (true)
                {
                    int currWidth = Console.BufferWidth;
                    int currHeight = Console.BufferHeight;

                    if (currWidth != lastWidth || currHeight != lastHeight)
                    {
                        lastWidth = currWidth;
                        lastHeight = currHeight;
                        DrawScene(currentScene);
                    }

                    Thread.Sleep(50);
                }
            });
            resizeThread.Start();
        }
        DrawScene(0);
    }

    // Renders a certain scene with the provided id.
    public void DrawScene(int id)
    {
        string name = scenes.ElementAt(id);

        AnsiConsole.Cursor.Hide();
        AnsiConsole.Clear();

        Table menu = new Table()
            .Border(TableBorder.Rounded)
            .Centered();

        foreach (string scene in scenes)
        {
            string color = scene == name ? Color.Red.ToString() : Color.Orange1.ToString();
            menu.AddColumn(new TableColumn($"[{color}][bold]{scene}[/][/]"));
        }

        AnsiConsole.Write(menu);
        OnSceneSwitched?.Invoke(name);
    }

    // A function for handling input in the terminal.
    private void HandleInput(ConsoleKey key, char ch)
    {
        switch (key)
        {
            case ConsoleKey.Tab:
                currentScene++;
                if (currentScene >= scenes.Count)
                    currentScene = 0;
                DrawScene(currentScene);
                break;
        }
    }
}