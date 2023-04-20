namespace Galaga.Systems;

public class GameStatsSystem
{
    private int _hitBullets;
    private int _missedBullets;
    
    public float ShotPercentage => (float)_hitBullets / (_hitBullets + _missedBullets);

    public void MissedBullet() => _missedBullets++;

    public void HitBullet() => _hitBullets++;
}