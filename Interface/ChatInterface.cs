
namespace Winreels.Interface;

public class ChatInterface : Scene
{
    public override string Name => "Chat";

    public string allText = "";

    public override void Initialize()
    {
        base.Initialize();
        fields.Add(new Field("Frame", false, 64, 18));
        fields.Add(new Field("Input", true));
        fields[1].OnEnter += Submit;
    }

    public override void Render()
    {
        base.Render();

        for (int i = 0; i < fields.Count; i++)
        {
            fields[i].Render(i == currentField);
        }
    }

    public override void OnInput(ConsoleKeyInfo info)
    {
        base.OnInput(info);

        if (info.Key == ConsoleKey.DownArrow)
        {
            
        }
    }

    public void Submit()
    {
        if (ClientInterface.Instance == null)
            return;

        string input = $"[blue]{ClientInterface.Instance.username}: [/]{fields[1].text}\n";
        allText += input;
    }
}