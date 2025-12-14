///Made By Jared Lee Zhengyu
/// Date Of Creation  : 12/12/2025
/// Function of Script: Player data structure for Firebase integration.
public class player
{
    /// <summary>
    /// Player's email address.
    /// </summary>
    public string email;
    /// <summary>
    /// Player's best time for Guitar quiz.
    /// </summary>
    public float guitBesttime;
    /// <summary>
    /// Player's best time for Piano quiz.
    /// </summary>
    public float pianoBesttime;
    public player() { }
    /// <summary>
    /// Constructor to initialize player data.
    /// </summary>
    public player(string email, float guitBesttime, float pianoBesttime)
    {
        this.email = email;
        this.guitBesttime = guitBesttime;
        this.pianoBesttime = pianoBesttime;
    }

}