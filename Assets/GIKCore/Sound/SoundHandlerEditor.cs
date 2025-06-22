using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GIKCore.Bundle;

namespace GIKCore.Sound
{
    public class SoundHandlerEditor : MonoBehaviour
    {
        // Fields
        [SerializeField] private AudioSource m_AudioSource;
        [SerializeField] private GameObject m_SFXPool;

        // Values
        private const float TIME_CEAR_POOL = 5f;
        private const int MAX_SFX = 100;
        private const string PREFIX = "sfx_";

        private string currentScene = "", lastTheme = "";
        private float time = 0f;

        // Methods
        public void DoDestroy() { Destroy(gameObject); }

        public AudioSource audioSource { get { return m_AudioSource; } }
        public float volume
        {
            set
            {
                if (value > 1.0f) value = 1.0f;
                if (value < 0.0f) value = 0.0f;
                m_AudioSource.volume = value;
            }
        }

        public void MuteMusic() { m_AudioSource.mute = !m_AudioSource.mute; }
        public void PlayMusic(string theme, string key,bool checkExists = false)
        {
            if (string.IsNullOrEmpty(theme)) return;
            if (checkExists && lastTheme.Equals(theme)) return;
            lastTheme = theme;
            m_AudioSource.clip = BundleHandler.LoadSound(theme,key);
            m_AudioSource.Play();
        }

        public void PlaySFX(string clipname,string key, bool loop)
        {
            if (string.IsNullOrEmpty(clipname)) return;
            if (SoundHandler.main.isSFXOn)
            {
                Transform[] data = FindSound(clipname);
                Transform panel = data[0];
                Transform deActive = data[1];

                if (deActive == null)
                {
                    int countSFX = panel.childCount;
                    if (countSFX < MAX_SFX)
                    {
                        GameObject objToSpawn = Instantiate(m_SFXPool);
                        objToSpawn.name = PREFIX + clipname ;
                        objToSpawn.transform.parent = panel.transform;
                        AudioClip clip = BundleHandler.LoadSound(clipname,key);
                        SFXPool sfx = objToSpawn.GetComponent<SFXPool>();
                        sfx.audioSource.clip = clip;
                        if (PlayerPrefs.HasKey("SFXVolume"))
                            sfx.audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                        
                        sfx.audioSource.Play();
                        sfx.audioSource.loop = loop;
                    }
                }
                else
                {
                    deActive.gameObject.SetActive(true);
                    SFXPool sfx = deActive.gameObject.GetComponent<SFXPool>();
                    if (PlayerPrefs.HasKey("SFXVolume"))
                        sfx.audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                    sfx.audioSource.Play();
                    sfx.audioSource.loop = loop;
                }
            }
        }
        public void PlaySFX(AudioClip clip, bool loop)
        {
            if (clip == null) return;
            if (SoundHandler.main.isSFXOn)
            {
                Transform[] data = FindSound(clip.name);
                Transform panel = data[0];
                Transform deActive = data[1];

                if (deActive == null)
                {
                    int countSFX = panel.childCount;

                    if (countSFX < MAX_SFX)
                    {
                        GameObject objToSpawn = Instantiate(m_SFXPool) as GameObject;
                        objToSpawn.name = PREFIX + clip.name;
                        objToSpawn.transform.parent = panel.transform;

                        SFXPool sfx = objToSpawn.GetComponent<SFXPool>();
                        sfx.audioSource.clip = clip;
                        if (PlayerPrefs.HasKey("SFXVolume"))
                            sfx.audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                        sfx.audioSource.Play();
                        sfx.audioSource.loop = loop;
                    }
                }
                else
                {
                    deActive.gameObject.SetActive(true);
                    SFXPool sfx = deActive.gameObject.GetComponent<SFXPool>();
                    if (PlayerPrefs.HasKey("SFXVolume"))
                        sfx.audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
                    sfx.audioSource.Play();
                    sfx.audioSource.loop = loop;
                }
            }
        }
        public void StopSFX(string clipname)
        {
            Transform panel = transform.Find(PREFIX + clipname);
            if (panel == null) return;
            foreach (Transform child in panel)
                child.GetComponent<AudioSource>().Stop();
        }
        public void StopAllSFX()
        {
            foreach (Transform child in transform)
            {
                for (int i = 0; i < child.childCount; i++)
                {
                    child.GetChild(i).GetComponent<AudioSource>().Stop();
                }
            }
        }

        private Transform[] FindSound(string name)
        {
            Transform panelT = this.transform.Find(PREFIX + name);
            Transform panel = null;

            if (panelT == null)
            {
                GameObject go = new GameObject(PREFIX + name);
                go.transform.parent = this.transform;
                panel = go.transform;
            }
            else panel = panelT;

            Transform deActive = null;
            foreach (Transform child in panel.transform)
            {
                if (!child.gameObject.activeSelf)
                {
                    deActive = child;
                    break;
                }
            }

            return new Transform[] { panel, deActive };
        }
        private void ClearSound(bool clearAll = false)
        {
            foreach (Transform child in transform)
            {
                for (int i = 0; i < child.childCount; i++)
                {
                    Transform tmp = child.GetChild(i);
                    if (clearAll) Destroy(tmp.gameObject);
                    else if (!tmp.gameObject.activeSelf)
                        Destroy(tmp.gameObject);
                }
            }
        }

        // System
        void Awake()
        {
            if (SoundHandler.main.editorSound == null)
            {
                SoundHandler.main.editorSound = this;
                DontDestroyOnLoad(transform.root.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Use this for initialization
        //void Start() { }

        // Update is called once per frame
        void Update()
        {
            string activeScene = SceneManager.GetActiveScene().name;
            if (!activeScene.Equals(currentScene))
            {
                currentScene = activeScene;
                ClearSound(true);
            }

            time += Time.deltaTime;
            if (time >= TIME_CEAR_POOL)
            {
                time = 0;
                ClearSound();
            }
        }
    }
}