using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore.DB;
using GIKCore.Bundle;

namespace GIKCore.Sound
{
    public class SoundHandler
    {
        private static SoundHandler instance;
        public static SoundHandler main
        {
            get
            {
                if (instance == null) instance = new SoundHandler();
                return instance;
            }
        }

        public SoundHandler()
        {
            music = PlayerPrefs.GetInt("Music", 1);
            sfx = PlayerPrefs.GetInt("SFX", 1);
        }

        // Values
        public SoundHandlerEditor editorSound;
        public SFXPool editorSFX;

        private int music { get; set; }
        private int sfx { get; set; }

        // Methods
        public bool isMusicOn { get { return (music == 1 && !Config.sv1000); } }
        public bool isSFXOn { get { return (sfx == 1 && !Config.sv1000); } }

        public void OnOffMusic()
        {
            music = (music == 1) ? 0 : 1;
            PlayerPrefs.SetInt("Music", music);
            editorSound.MuteMusic();
        }
        public void OnOffSFX()
        {
            sfx = (sfx == 1) ? 0 : 1;
            PlayerPrefs.SetInt("SFX", sfx);
            if (!isSFXOn) editorSound.StopAllSFX();
        }

        public void PlayMusic(string theme, string key, bool checkExists = false)
        {
            editorSound.PlayMusic(theme,key, checkExists);
        }
        public void PlaySFX(AudioClip clip, bool loop = false)
        {
            if (isSFXOn) editorSound.PlaySFX(clip, loop);
        }
        public void PlaySFX(string clipname, string key, bool loop = false)
        {
            if (isSFXOn) editorSound.PlaySFX(clipname,key, loop);
        }
        public void StopSFX(string clipname) { editorSound.StopSFX(clipname); }

        public void Init(string theme = "")
        {
            editorSound.audioSource.clip = BundleHandler.LoadSound(theme,"sounds");
            editorSound.audioSource.mute = !isMusicOn;
            editorSound.audioSource.Play();
            Debug.Log(theme);
            if (PlayerPrefs.HasKey("MusicVolume"))
                ChangeMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1));
            else
                ChangeMusicVolume(1);

            if (PlayerPrefs.HasKey("SFXVolume"))
                ChangeSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1));
            else
                ChangeSFXVolume(1);
            if (PlayerPrefs.HasKey("isMute"))
            {
                if (PlayerPrefs.GetInt("isMute") == 1)
                {
                    Mute();
                }
            }
        }
        public static void DoDestroy()
        {
            instance = null;
            GameObject go = GameObject.Find("SoundHandlerPrefab");
            if (go != null)
                go.GetComponent<SoundHandlerEditor>().DoDestroy();
        }
        public void ChangeMusicVolume(float volume)
        {
            editorSound.volume = volume;
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
        public void ChangeSFXVolume(float volume)
        {
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
        public void Mute()
        {
            //editorSound.volume = 0;
            //editorSound.StopAllSFX();
            editorSound.volume = 0;
            PlayerPrefs.SetFloat("MusicVolume", 0);
            PlayerPrefs.SetFloat("SFXVolume", 0);
            if (!isSFXOn) editorSound.StopAllSFX();
        }
    }
}