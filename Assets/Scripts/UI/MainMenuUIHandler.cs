using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUIHandler : MonoBehaviour
{
    public const string PLAYER_NICKNAME = "PlayerNickname";
    public TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(PLAYER_NICKNAME))
            inputField.text = PlayerPrefs.GetString(PLAYER_NICKNAME);
        else
            inputField.text = "Player" + Random.Range(100, 999);
    }

    public void OnJoinGameClicked()
    {
        PlayerPrefs.SetString(PLAYER_NICKNAME, inputField.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
