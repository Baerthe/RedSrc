namespace Interface;
public interface IAudioService : IService
{
    void PlaySound(string soundName);
    void StopSound(string soundName);
    void SetVolume(float volume);
    float GetVolume();
}