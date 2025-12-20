using Spectre.Console;

namespace Winreels.Interface;

/// <summary>
/// This class is used to create a screen based TUI.
/// The screens can be switched by pressing Tab.
/// Each scene handles input and resize automatically.
/// </summary>
public static class SceneManager
{
    public static readonly Color IDLE_COLOR = Color.FromHex("#701818ff");
    public static readonly Color ACTIVE_COLOR = Color.FromHex("#ba2c2cff");

    public static readonly List<Scene> Scenes = [];
    public static int CurrentScene { get; private set; }

    private static int lastWidth = 0;
    private static int lastHeight = 0;

    // Adds a scene to the SceneManager.
    public static void AddScene(Scene scene)
    {
        if (Scenes.Contains(scene))
            return;
        Scenes.Add(scene);
    }

    // Removes a scene from the SceneManager.
    public static void RemoveScene(Scene scene)
    {
        if (!Scenes.Contains(scene))
            return;
        Scenes.Remove(scene);
    }

    // Initializes input and resize, and all the currently registered scenes.
    public static void Initialize()
    {
        Thread inputThread = new Thread(() =>
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo? value = Console.ReadKey(true);
                    if (value.HasValue)
                    {
                        ConsoleKeyInfo info = value.GetValueOrDefault();
                        HandleInput(info);
                        RenderScene();
                    }
                }
            }
        });
        Thread resizeThread = new Thread(() =>
        {
            lastWidth = Console.WindowWidth;
            lastHeight = Console.WindowHeight;
            while (true)
            {
                int currentWidth = Console.WindowWidth;
                int currentHeight = Console.WindowHeight;
                if (currentWidth != lastWidth || currentHeight != lastHeight)
                {
                    lastWidth = currentWidth;
                    lastHeight = currentHeight;
                    RenderScene();
                }
                Thread.Sleep(50);
            }
        });
        InitializeScenes();
        RenderScene();
        inputThread.Start();
        resizeThread.Start();
    }

    // If tab is pressed, switch to the next scene, otherwise handle input for the scene itself.
    private static void HandleInput(ConsoleKeyInfo info)
    {
        switch (info.Key)
        {
            case ConsoleKey.Tab:
                CurrentScene++;
                if (CurrentScene >= Scenes.Count)
                    CurrentScene = 0;
                Scenes[CurrentScene].currentField = 0;
                RenderScene();
                break;
            default:
                InputScene(info);
                break;
        }
    }

    // Renders a scene bar at the top of the terminal.
    private static void RenderSceneBar()
    {
        Table table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Black)
            .Centered();

        foreach (Scene scene in Scenes)
        {
            bool active = scene == Scenes[CurrentScene];
            Color color = active ? ACTIVE_COLOR : IDLE_COLOR;

            table.AddColumn(new TableColumn(
                new Markup($"[{color.ToMarkup()}]{scene.Name}[/]"))
            );
        }

        AnsiConsole.Write(table);
    }

    // Initializes all scenes.
    private static void InitializeScenes()
    {
        AnsiConsole.Cursor.Hide();

        foreach (Scene scene in Scenes) {
            scene.Initialize();
        }
    }

    // Renders the current scene.
    private static void RenderScene()
    {
        AnsiConsole.Clear();
        RenderSceneBar();
        
        Scene scene = Scenes[CurrentScene];
        scene.Render();
    }

    // Handles input for the current scene.
    private static void InputScene(ConsoleKeyInfo info)
    {
        Scene scene = Scenes[CurrentScene];
        scene.OnInput(info);
    }
}

/// <summary>
/// This class provides an easy way for creating scenes.
/// A scene can have custom rendering, input and fields.
/// Fields can be switched by pressing shift+any_key.
/// </summary>
public abstract class Scene()
{
    public abstract string Name { get;}

    public List<Field> fields = [];
    public int currentField = 0;

    public virtual void Initialize() {}

    public virtual void Render() {}

    public virtual void OnInput(ConsoleKeyInfo info)
    {
        switch (info.Key)
        {
            case ConsoleKey.Enter:
                fields[currentField].OnEnter?.Invoke();
                break;
            case ConsoleKey.Backspace:
                Field f1 = fields[currentField];
                if (f1.text.Length > 0)
                    f1.text = f1.text[..^1];
                break;
            default:
                if ((info.Modifiers & ConsoleModifiers.Shift) != 0)
                {
                    currentField++;
                    if (currentField >= fields.Count)
                        currentField = 0;
                    break;
                }
                Field f2 = fields[currentField];
                if (f2.writeable && !char.IsControl(info.KeyChar))
                    if (f2.text.Length < Field.MAX_LENGTH)
                        f2.text += info.KeyChar;
                break;
        }
    }
}

/// <summary>
/// This class contains data for a field instance.
/// </summary>
public class Field(string name, bool writeable)
{
    public static readonly Color IDLE_COLOR = Color.FromHex("#b99210ff");
    public static readonly Color ACTIVE_COLOR = Color.FromHex("#fed43f");
    public const int MAX_LENGTH = 32;

    public readonly string name = name;
    public readonly bool writeable = writeable;
    public string text = "";
    public Action? OnEnter;

    // An example rendering method.
    public void Render(bool isActive)
    {
        Panel p = new Panel(text)
            .Header(name)
            .BorderColor(isActive ? ACTIVE_COLOR : IDLE_COLOR)
            .RoundedBorder();
        p.Width = 36;
        p.Height = 3;

        Table t1 = new Table()
            .AddColumn(new TableColumn(p))
            .NoBorder()
            .Centered();
        
        AnsiConsole.Write(t1);
    }
}