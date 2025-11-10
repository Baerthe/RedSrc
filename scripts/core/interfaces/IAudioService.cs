namespace Core.Interface;
public interface IAudioService
{
    void PlaySound(string soundName);
    void StopSound(string soundName);
    void SetVolume(float volume);
    float GetVolume();
}