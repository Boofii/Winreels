using Spectre.Console;

namespace Winreels.Interface;

/// <summary>
/// This class is used to create TUIs that can have multiple scenes.
/// The user can toggle between scenes by pressing Tab.
/// The user can toggle between input fields by pressing Shift+Any.
/// </summary>
public class MainInterface
{
    public const int MAX_LENGTH = 32;
    public const float FIELD_START_X = 0.32F;
    public const float FIELD_START_Y = 0.28F;
    public const float FIELD_SKIP = 0.15F;

    public Color mBorderColor = Color.Black;
    public Color mIdleColor = Color.DarkRed;
    public Color mHighlightColor = Color.Red;
    public Color fIdleColor = Color.Green;
    public Color fHighlightColor = Color.LightGreen;
    public int currentScene = 0;
    public int currentField = 0;

    public readonly HashSet<string> scenes = [];
    public readonly Dictionary<int, List<Panel>> fields = []; 
    public readonly Dictionary<int, List<string>> texts = [];

    public Action<int>? OnSceneDraw;
    public Action<int, int, string>? OnFieldEnter;
    public readonly Action<ConsoleKey, char, ConsoleModifiers>? OnInput;

    private Thread? inputThread;
    private Thread? resizeThread;
    private int lastWidth;
    private int lastHeight;
    private bool inputRunning = true;
    private bool resizeRunning = true;

    // Adds a scene to this MainInterface.
    public MainInterface AddScene(string name)
    {
        scenes.Add(name);
        return this;
    }

    // Adds input fields to this MainInterface.
    public MainInterface AddFields(int scene, string[] names)
    {
        fields.TryAdd(scene, []);
        texts.TryAdd(scene, []);
        foreach (string name in names) {
            fields[scene].Add(new Panel("").Padding(16, 0).Header(name).RoundedBorder().BorderColor(Color.Green));
            texts[scene].Add("");
        }
        return this;
    }

    // Removes a scene from this MainInterface.
    public MainInterface RemoveScene(string name)
    {
        scenes.Remove(name);
        return this;
    }

    // Removes input fields from this MainInterface.
    public MainInterface RemoveFields(int id, int[] indexes)
    {
        if (fields.TryGetValue(id, out List<Panel>? list1) && texts.TryGetValue(id, out List<string>? list2)) {
            foreach (int i in indexes) {
                list1.Remove(list1.ElementAt(i));
                list2.Remove(list2.ElementAt(i));
            }
        }
        return this;
    }

    // Initializes and renders the first scene, call this after registering scenes and events.
    public virtual void Initialize()
    {
        if (inputThread == null)
        {
            inputThread = new Thread(() =>
            {
                IAnsiConsoleInput input = AnsiConsole.Console.Input;

                while (inputRunning)
                {
                    if (input.IsKeyAvailable())
                    {
                        ConsoleKeyInfo? i = input.ReadKey(true);
                        if (i == null)
                            continue;

                        ConsoleKeyInfo info = i.GetValueOrDefault();
                        ConsoleKey key = info.Key;
                        char ch = info.KeyChar;
                        ConsoleModifiers modifiers = info.Modifiers;
                        HandleInput(key, ch, modifiers);
                        OnInput?.Invoke(key, ch, modifiers);
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

                while (resizeRunning)
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
            .BorderColor(mBorderColor)
            .Centered();

        foreach (string scene in scenes)
        {
            string color = scene == name ? mHighlightColor.ToString() : mIdleColor.ToString();
            menu.AddColumn(new TableColumn($"[{color}][bold]{scene}[/][/]"));
        }

        AnsiConsole.Write(menu);
        if (fields.TryGetValue(id, out List<Panel>? list))
        {
            Table input = new Table()
                .NoBorder()
                .Centered();

            input.AddColumn(new TableColumn(""));

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Height = 3;
                Color color = i == currentField ? fHighlightColor : fIdleColor;
                input.AddRow(list[i].BorderColor(color));
            }

            AnsiConsole.Write(input);

            int consoleWidth = Console.WindowWidth;
            int consoleHeight = Console.WindowHeight;

            for (int i = 0; i < list.Count; i++)
            {
                int x = (int)(consoleWidth * FIELD_START_X);
                int y = (int)(consoleHeight * FIELD_START_Y) + (int)(consoleHeight * FIELD_SKIP * i);
                if (i >= 2)
                    y -= 1 * (i / 2);

                string text = texts[currentScene][i];

                int lx = Console.CursorLeft;
                int ly = Console.CursorTop + 1;

                AnsiConsole.Cursor.SetPosition(x, y);
                AnsiConsole.Write(new Markup(text));
                AnsiConsole.Cursor.SetPosition(lx, ly);
            }
        }

        OnSceneDraw?.Invoke(id);
    }

    // A function for handling input in the terminal.
    private void HandleInput(ConsoleKey key, char ch, ConsoleModifiers modifiers)
    {
        switch (key)
        {
            case ConsoleKey.Tab:
                currentScene++;
                currentField = 0;
                if (currentScene >= scenes.Count)
                    currentScene = 0;
                DrawScene(currentScene);
                break;
            case ConsoleKey.Enter:
                OnFieldEnter?.Invoke(currentScene, currentField, texts[currentScene][currentField]);
                break;
            default:
                if (fields.TryGetValue(currentScene, out List<Panel>? list))
                {
                    if ((modifiers & ConsoleModifiers.Shift) != 0)
                    {
                        currentField++;
                        if (currentField >= list.Count) {
                            currentField = 0;
                        }
                    }
                    else
                    {
                        string text = texts[currentScene][currentField];
                        if (key == ConsoleKey.Backspace && text.Length > 0)
                            text = text[..^1];
                        else if (!char.IsControl(ch) && text.Length < MAX_LENGTH)
                            text += ch;
                        texts[currentScene][currentField] = text;
                    }
                    DrawScene(currentScene);
                }
                break;
        }
    }
    
    public void Close()
    {
        inputRunning = false;
        resizeRunning = false;
        AnsiConsole.Clear();
    }
}