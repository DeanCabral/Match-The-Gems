using UnityEngine;
using System.Collections;

public class Gem
{
    // Variables for position of gem on board
    public int x;
    public int y;
    // Variable for the type of gem
    public int Type;

    // Default public contructor
    public Gem()
    {
        x = 0;
        y = 0;
        Type = 0;
    }

    // Editable public constructor
    public Gem(int xpos, int ypos, int type)
    {
        x = xpos;
        y = ypos;
        Type = type;
    }
}
