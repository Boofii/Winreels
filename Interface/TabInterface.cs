using System.Data;

namespace Winreels.Interface;

public class TabInterface
{
    public const int MAX_TABS = 10;
    public const int MAIN_WIDTH = 60;
    public const int MAIN_HEIGHT = 12;

    private readonly string[] tabs = new string[MAX_TABS];
    private int tabsCount = 0;
    private ConsoleColor color = ConsoleColor.Red;

    public TabInterface WithColor(ConsoleColor color)
    {
        this.color = color;
        return this;
    }

    public TabInterface AddTab(string tab)
    {
        if (tabsCount >= MAX_TABS)
        {
            tabs[tabsCount] = tab;
            return this;
        }
        tabsCount++;
        return this;
    }

    public void Render()
    {
        Console.Clear();
    }
}