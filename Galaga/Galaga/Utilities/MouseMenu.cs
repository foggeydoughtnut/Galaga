using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga.Utilities;

public static class MouseMenu
{
    public static int UpdateIndexBasedOnMouse(MouseState currentMouseState, GameWindow window, Dictionary<int, Tuple<int, int>> optionPositions, int currentIndex)
    {
        var positionRatio = (float)currentMouseState.Y / window.ClientBounds.Height;
        var offsetPosition = (int)(positionRatio * 1080); 
        foreach (var (index, position) in optionPositions)
        {
            if (offsetPosition <= position.Item1 || offsetPosition >= position.Item2)
                continue;
            return index;
        }

        return currentIndex;
    }
    
    public static Dictionary<int, Tuple<int, int>> DefineOptionPositions(IReadOnlyDictionary<string, SpriteFont> fonts, int numOptions, RenderTarget2D renderTarget)
    {
        var optionPositions = new Dictionary<int, Tuple<int, int>>();
        var middle = numOptions / 2;
        for (var i = 0; i < numOptions; i++)
        {
            var (_, y) = fonts[ "default"].MeasureString("word");
            var diff = i - middle;
            var top = Convert.ToInt32(renderTarget.Height / 2) + diff * Constants.MENU_BUFFER;
            optionPositions[i] = new Tuple<int, int>(top, (int)y + top);
        }

        return optionPositions;
    } 
}