using System;

namespace Galaga.Systems;

public class GameStatsSystem
{
    public int HitBullets;
    public int MissedBullets;
    private static GameStatsSystem _gameStatsSystem;
    
    public static GameStatsSystem GetSystem()
    {
        return _gameStatsSystem ??= new GameStatsSystem();
    }

    public void FinishGame()
    {
        HitBullets = 0;
        MissedBullets = 0;
    }

    public float ShotPercentage
    {
        get
        {
            if (MissedBullets + MissedBullets == 0) return 0;
            return (float)HitBullets / (HitBullets + MissedBullets);  
        }
    } 

    public void MissedBullet() => MissedBullets++;

    public void HitBullet() => HitBullets++;
}