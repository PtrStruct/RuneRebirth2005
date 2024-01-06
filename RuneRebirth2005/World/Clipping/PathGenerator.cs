using RuneRebirth2005.Entities;

namespace RuneRebirth2005.World.Clipping;

public static class PathGenerator
{
    private static Character _character;
    private static Character _following;

    public static Location GetCombatPath(Character character, Character following)
    {
        _character = character;
        _following = following;
        
        if (character == null || following == null)
        {
            return null;
        }

        int characterX = character.Location.X;
        int characterY = character.Location.Y;
        int characterZ = character.Location.Z;
        int followX = following.Location.X;
        int followY = following.Location.Y;

        double lowDist = 9999.0D;
        int lowX = 0;
        int lowY = 0;

        int x3 = followX;
        int y3 = followY - 1;

        int loop = following.Size;

        if (PathFinder.IsAccessible(followX, followY, characterZ, x3, y3) && IsInteractionPathClear(x3, y3, characterZ, followX, followY))
        {
            lowDist = GetManhattanDistance(x3, y3, characterX, characterY);
            lowX = x3;
            lowY = y3;
        }


        for (int k = 0; k < 4; k++)
        {
            for (int i = 0; i < loop - (k == 0 ? 1 : 0); i++)
            {
                if (k == 0)
                {
                    x3++;
                }
                else if (k == 1)
                {
                    if (i == 0)
                    {
                        x3++;
                    }

                    y3++;
                }
                else if (k == 2)
                {
                    if (i == 0)
                    {
                        y3++;
                    }

                    x3--;
                }
                else if (k == 3)
                {
                    if (i == 0)
                    {
                        x3--;
                    }

                    y3--;
                }

                double d;
                if ((d = GetManhattanDistance(x3, y3, characterX, characterY)) < lowDist)
                {
                    if (PathFinder.IsAccessible(followX, followY, characterZ, x3, y3) &&
                        IsInteractionPathClear(x3, y3, characterZ, followX, followY))
                    {
                        lowDist = d;
                        lowX = x3;
                        lowY = y3;
                    }
                }
            }
        }

        return new Location(lowX, lowY);
    }
    
    public static bool IsInteractionPathClear(int x0, int y0, int z, int followX, int followY)
    {
        var deltaX = followX - x0;
        var deltaY = followY - y0;

        double error = 0;
        var deltaError = Math.Abs(
            deltaY / (deltaX == 0 ? deltaY : (double)deltaX));

        var x = x0;
        var y = y0;

        var pX = x;
        var pY = y;

        var incrX = x0 < followX;
        var incrY = y0 < followY;

        if (!PathFinder.IsAccessible(x0, y0, z, followX, followY)) 
        {
            return false;
        }
        
        while (true)
        {
            if (x != followX) x += incrX ? 1 : -1;

            if (y != followY)
            {
                error += deltaError;

                if (error >= 0.5)
                {
                    y += incrY ? 1 : -1;
                    error -= 1;
                }
            }

            if (!Region.canProjectileMove(pX, pY, followX, followY, z, 1, 1)) return false;

            if (incrX && incrY
                      && x >= followX && y >= followY)
                break;
            if (!incrX && !incrY
                       && x <= followX && y <= followY)
                break;
            if (!incrX && incrY
                       && x <= followX && y >= followY)
                break;
            if (incrX && !incrY
                      && x >= followX && y <= followY)
                break;

            pX = x;
            pY = y;
        }

        return true;
    }
    
    static int GetManhattanDistance(int x, int y, int x2, int y2) {
        return Math.Abs(x - x2) + Math.Abs(y - y2);
    }
}