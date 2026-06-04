using UnityEngine;
using UnityEngine.UI;
public class SoundManager : MonoBehaviour
{
    public AudioSource[] EffectsSource;
    public AudioClip[] audioClips;
    public AudioSource MusicSource;
    public AudioClip musicClip;

    //public AudioSource[] ShapeSoundsSource;
    //public AudioClip[] ShapeSoundsClips;

    //public AudioSource[] AnimalSoundsSource;
    //public AudioClip[] AnimalSoundsClips;

    //public AudioSource[] FruitSoundsSource;
    //public AudioClip[] FruitSoundsClips;

    //public AudioSource[] VegetablesSoundsSource;
    //public AudioClip[] VegetablesSoundsClips;

    //public AudioSource[] BodyPartsSoundsSource;
    //public AudioClip[] BodyPartsSoundsClips;

    public static SoundManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void PlayEffect_Instance(int index)
    {
        EffectsSource[index].PlayOneShot(audioClips[index]);
    }
    //public void PlayShapeEffect_Instance(int index)
    //{
    //    ShapeSoundsSource[index].PlayOneShot(ShapeSoundsClips[index]);
    //}
    public void PlayEffect_Complete(int index)
    {
        if (!EffectsSource[index].isPlaying)
            EffectsSource[index].PlayOneShot(audioClips[index]);
    }
    //public void PlayShapeEffect_Complete(int index)
    //{
    //    if (!ShapeSoundsSource[index].isPlaying)
    //        ShapeSoundsSource[index].PlayOneShot(ShapeSoundsClips[index]);
    //}
    //public void PlayAnimalEffect_Instance(int index)
    //{
    //    AnimalSoundsSource[index].PlayOneShot(AnimalSoundsClips[index]);
    //}
    //public void PlayAnimalEffect_Complete(int index)
    //{
    //    if (!AnimalSoundsSource[index].isPlaying)
    //        AnimalSoundsSource[index].PlayOneShot(AnimalSoundsClips[index]);
    //}
    //public void PlayFruitEffect_Instance(int index)
    //{
    //    FruitSoundsSource[index].PlayOneShot(FruitSoundsClips[index]);
    //}
    //public void PlayVegetableEffect_Instance(int index)
    //{
    //    VegetablesSoundsSource[index].PlayOneShot(VegetablesSoundsClips[index]);
    //}
    //public void PlayBodyPartsEffect_Instance(int index)
    //{
    //    BodyPartsSoundsSource[index].PlayOneShot(BodyPartsSoundsClips[index]);
    //}
    public void PlayEffect_Loop(int index)
    {
        if (!EffectsSource[index].isPlaying)
        {
            EffectsSource[index].Play();
            EffectsSource[index].loop = true;
        }

    }
    public void StopEffect(int index)
    {
        EffectsSource[index].Stop();
    }
    public void StopAllSounds()
    {
        for (int i = 0; i < EffectsSource.Length; i++)
        {
            if (EffectsSource[i].isPlaying)
                EffectsSource[i].Stop();
        }
    }
    //public void StopAllShapeSounds()
    //{
    //    for (int i = 0; i < ShapeSoundsSource.Length; i++)
    //    {
    //        if (ShapeSoundsSource[i].isPlaying)
    //            ShapeSoundsSource[i].Stop();
    //    }
    //}
    //public void StopAllAnimalSounds()
    //{
    //    for (int i = 0; i < AnimalSoundsSource.Length; i++)
    //    {
    //        if (AnimalSoundsSource[i].isPlaying)
    //            AnimalSoundsSource[i].Stop();
    //    }
    //}
    //public void StopAllFruitSounds()
    //{
    //    for (int i = 0; i < FruitSoundsSource.Length; i++)
    //    {
    //        if (FruitSoundsSource[i].isPlaying)
    //            FruitSoundsSource[i].Stop();
    //    }
    //}
    //public void StopAllVegetableSounds()
    //{
    //    for (int i = 0; i < VegetablesSoundsSource.Length; i++)
    //    {
    //        if (VegetablesSoundsSource[i].isPlaying)
    //            VegetablesSoundsSource[i].Stop();
    //    }
    //}
    //public void StopAllBodyPartsSounds()
    //{
    //    for (int i = 0; i < BodyPartsSoundsSource.Length; i++)
    //    {
    //        if (BodyPartsSoundsSource[i].isPlaying)
    //            BodyPartsSoundsSource[i].Stop();
    //    }
    //}
    public void PlayMusic()
    {
        MusicSource.clip = musicClip;
        MusicSource.Play();
    }

    public void MusicSettingBtn_OnClick()
    {
        SoundManager.instance.PlayEffect_Instance(5);
        if (PlayerPrefs.GetInt("MusicOn") == 0)
        {
            PlayerPrefs.SetInt("MusicOn", 1);
            MusicSource.volume = 0f;
        }
        else
        {
            PlayerPrefs.SetInt("MusicOn", 0);
            MusicSource.volume = 0.2f;
        }
    }

    public void SoundSettingBtn_OnClick()
    {
        SoundManager.instance.PlayEffect_Instance(5);
        if (PlayerPrefs.GetInt("SoundOn") == 0)
        {
            PlayerPrefs.SetInt("SoundOn", 1);
            for (int i = 0; i < EffectsSource.Length; i++)
            {
                EffectsSource[i].volume = 0f;
            }
        }
        else
        {
            PlayerPrefs.SetInt("SoundOn", 0);
            for (int i = 0; i < EffectsSource.Length; i++)
            {
                EffectsSource[i].volume = 1f;
            }
        }
    }

    public void CheckOnStart()
    {
        if (PlayerPrefs.GetInt("MusicOn") == 1)
        {
            Setting.ins.musicBtnSprite[0].SetActive(true);
            Setting.ins.musicBtnSprite[1].SetActive(false);
            MusicSource.volume = 0f;
        }
        else if (PlayerPrefs.GetInt("MusicOn") == 0)
        {
            Setting.ins.musicBtnSprite[0].SetActive(false);
            Setting.ins.musicBtnSprite[1].SetActive(true);

            MusicSource.volume = 0.2f;
        }

        if (PlayerPrefs.GetInt("SoundOn") == 1)
        {
            Setting.ins.soundBtnSprite[0].SetActive(true);
            Setting.ins.soundBtnSprite[1].SetActive(false);
            for (int i = 0; i < EffectsSource.Length; i++)
            {
                EffectsSource[i].volume = 0f;
            }
        }
        else if (PlayerPrefs.GetInt("SoundOn") == 0)
        {
            Setting.ins.soundBtnSprite[0].SetActive(false);
            Setting.ins.soundBtnSprite[1].SetActive(true);
            for (int i = 0; i < EffectsSource.Length; i++)
            {
                EffectsSource[i].volume = 1f;
            }
        }

    }
}