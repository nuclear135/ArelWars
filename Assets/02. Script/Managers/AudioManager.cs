using UnityEngine;

// 단순한 오디오 매니저
// - bgm: 루프, sfx: 1회 재생
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioSource sfx;

    public void PlayBGM(AudioClip clip, float vol = 0.6f)
    {
        if (bgm == null)
        {
            return;
        }

        bgm.clip = clip;
        bgm.volume = vol;
        bgm.loop = true;
        bgm.Play();
    }

    public void StopBGM()
    {
        if (bgm != null)
        {
            bgm.Stop();
        }
    }

    public void PlaySFX(AudioClip clip, float vol = 1f)
    {
        if (sfx != null && clip != null)
        {
            sfx.PlayOneShot(clip, vol);
        }
    }
}
