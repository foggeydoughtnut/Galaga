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
    
    public float ShotPercentage => (float)HitBullets / (HitBullets + MissedBullets);

    public void MissedBullet() => MissedBullets++;

    public void HitBullet() => HitBullets++;
}