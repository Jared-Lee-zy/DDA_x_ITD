///Made By Jared Lee Zhengyu
/// Date Of Creation  : 2025-12-05
/// Function of Script: Player data structure for Firebase integration.
public class player
{
    public string email;
    public float guitBesttime;
    public float pianoBesttime;
    public player() { }

    public player(string email, float guitBesttime, float pianoBesttime)
    {
        this.email = email;
        this.guitBesttime = guitBesttime;
        this.pianoBesttime = pianoBesttime;
    }

}