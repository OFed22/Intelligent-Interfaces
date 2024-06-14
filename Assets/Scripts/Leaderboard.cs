using System.Collections.Generic;

[System.Serializable]
public class LeaderboardEntry
{
    public string nickname;
    public int score;

    public LeaderboardEntry(string nickname, int score)
    {
        this.nickname = nickname;
        this.score = score;
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}
