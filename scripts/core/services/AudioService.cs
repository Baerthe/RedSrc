namespace Core;
using Godot;
using Core.Interface;
public sealed class AudioService : IAudioService
{
    private readonly AudioStreamPlayer _audioPlayer;
    private float _volume = 1.0f;
    public AudioService()
    {
        _audioPlayer = new AudioStreamPlayer();
        _audioPlayer.VolumeDb = LinearToDb(_volume);
        _audioPlayer.Autoplay = false;
        _audioPlayer.Bus = "Master"; // Ensure this bus exists in your project settings
        GD.Print("AudioService created");
    }
    public void PlaySound(string soundName)
    {
        var soundPath = $"res://assets/.../{soundName}.wav"; // Assuming sounds are stored in this path
        var audioStream = GD.Load<AudioStream>(soundPath);
        if (audioStream == null)
        {
            GD.PrintErr($"Sound '{soundName}' not found at path: {soundPath}");
            return;
        }
        _audioPlayer.Stream = audioStream;
        _audioPlayer.VolumeDb = LinearToDb(_volume);
        _audioPlayer.Play();
        GD.Print($"Playing sound: {soundName}");
    }
    public void StopSound(string soundName)
    {
        if (_audioPlayer.Playing && _audioPlayer.Stream != null && _audioPlayer.Stream.ResourcePath.Contains(soundName))
        {
            _audioPlayer.Stop();
            GD.Print($"Stopped sound: {soundName}");
        }
    }
    public void SetVolume(float volume)
    {
        _volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        _audioPlayer.VolumeDb = LinearToDb(_volume);
        GD.Print($"Audio volume set to: {_volume}");
    }
    public float GetVolume()
    {
        return _volume;
    }
    private float LinearToDb(float linear)
    {
        return linear <= 0 ? -80 : 20 * Mathf.Log(linear);
    }
}