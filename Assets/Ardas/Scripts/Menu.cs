using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Menu : MonoBehaviour
{
	public TextMeshProUGUI musicText;
	public TextMeshProUGUI sfxText;

	public AudioMixer musicMixer;
	public AudioMixer sfxMixer;

	private bool isMusicOn = true;
	private bool isSFXOn = true;

	public void StartGame() {
		SceneManager.LoadScene("SampleScene");
		AudioManager.instance.StartGameplayMusic();
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void ToggleMusic()
	{
		isMusicOn = !isMusicOn;
		musicMixer.SetFloat("Volume", isMusicOn ? 0f : -80f);
		musicText.text = isMusicOn ? "MUSIC\nON" : "MUSIC\nOFF";
	}

	public void ToggleSFX()
	{
		isSFXOn = !isSFXOn;
		sfxMixer.SetFloat("Volume", isSFXOn ? 0f : -80f);
		sfxText.text = isSFXOn ? "SFX\nON" : "SFX\nOFF";
	}

	public void AddPrefix()
	{
		
	}

	public void RemovePrefix()
	{
		
	}
}
