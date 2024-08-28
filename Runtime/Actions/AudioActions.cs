/*
MIT License

Copyright (c) 2024 Tolin Simpson

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OGK
{
    [SRName("Audio/Play Sound")]
    public class PlaySoundAction : ActionModule
    {
        [Message("Use in a update loop. Next action will play once done.")]
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip clip;
        [SerializeField, Range(0, 1)] private float volume = 1, pitch = 1;

        private float playTime = 0;
        private bool playing = false;

        public override ActionEvent Invoke()
        { 
            if (source != null && clip != null) 
            { 
                if(playing == false)
                {
                    source.pitch = pitch; 
                    source.PlayOneShot(clip, volume);
                    playTime = 0;
                    playing = true;
                }
                else
                {
                    if(playTime < clip.length)
                    {
                        playTime += Time.deltaTime;
                    }
                    else
                    {
                        playing = false;
                        return ActionEvent.Release;
                    }
                }
                return ActionEvent.Hold;
            } 
            else return ActionEvent.Error;  
        }
    }

    [SRName("Audio/Play Sound Instant")]
    public class PlaySoundInstantAction : ActionModule
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip clip;
        [SerializeField, Range(0, 1)] private float volume = 1, pitch = 1;
        public override ActionEvent Invoke() { if (source != null && clip != null) { source.pitch = pitch; source.PlayOneShot(clip, volume); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Audio/Play Random Sound Instant")]
    public class PlayRandomSoundInstantAction : ActionModule
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
        [SerializeField, Range(0, 1)] private float volume = 1;
        [SerializeField, Range(-1, 1)] private float minPitch = -1, maxPitch = 1;
        public override ActionEvent Invoke()
        {
            if (source != null && clips.Count > 0)
            {
                source.pitch = Random.Range(minPitch, maxPitch);
                source.PlayOneShot(clips[Random.Range(0, clips.Count)], volume);
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Audio/Play Random Sound")]
    public class PlayRandomSoundAction : ActionModule
    {
        [Message("Use in a update loop. Next action will play once done.")]
        [SerializeField] private AudioSource source;
        [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
        [SerializeField, Range(0, 1)] private float volume = 1;
        [SerializeField, Range(-1, 1)] private float minPitch = -1, maxPitch = 1;

        private AudioClip clip;
        private float playTime = 0;
        private bool playing = false;

        public override ActionEvent Invoke()
        {
            if (source != null)
            {
                if (playing == false)
                {
                    clip = clips[Random.Range(0, clips.Count)];
                    if(clip != null)
                    {
                        source.pitch = Random.Range(minPitch, maxPitch);
                        source.PlayOneShot(clip, volume);
                        playTime = 0;
                        playing = true;
                    }
                    else
                    {
                        return ActionEvent.Error;
                    }
                }
                else
                {
                    if (playTime < clip.length)
                    {
                        playTime += Time.deltaTime;
                    }
                    else
                    {
                        playing = false;
                        return ActionEvent.Release;
                    }
                }
                return ActionEvent.Hold;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Audio/Play Sound List")]
    public class PlaySoundListAction : ActionModule
    {
        [Message("Use in a update loop. Next action will play once done.")]
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip[] clips;

        private AudioClip clip;
        private float playTime = 0;
        private bool playing = false;
        private int clipIndex = 0;

        public override ActionEvent Invoke()
        {
            if (source != null)
            {
                clip = clips[clipIndex % clips.Length];
                if(clip != null)
                {
                    if (playing == false)
                    {
                        source.PlayOneShot(clip);
                        playTime = 0;
                        playing = true;
                    }
                    else
                    {
                        if (playTime < clip.length)
                        {
                            playTime += Time.deltaTime;
                        }
                        else
                        {
                            playing = false;
                            return ActionEvent.Release;
                        }
                    }
                    return ActionEvent.Hold;
                }
                else return ActionEvent.Hold;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Audio/Play Sound At Position")]
    public class PlaySoundAtPositiontAction : ActionModule
    {
        [Message("Use in a update loop. Next action will play once done.")]
        [SerializeField] private AudioClip clip;
        [SerializeField] private Vector3 position;
        [SerializeField, Range(0,1)] private float volume = 1;

        private float playTime = 0;
        private bool playing = false;

        public override ActionEvent Invoke()
        {
            if (clip != null)
            {
                if (playing == false)
                {
                    AudioSource.PlayClipAtPoint(clip, position, volume);
                    playTime = 0;
                    playing = true;
                }
                else
                {
                    if (playTime < clip.length)
                    {
                        playTime += Time.deltaTime;
                    }
                    else
                    {
                        playing = false;
                        return ActionEvent.Release;
                    }
                }
                return ActionEvent.Hold;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Audio/Toggle Mute By Tag")]
    public class MuteByTagAction : ActionModule
    {
        [SerializeField] private string tag;
        [SerializeField] private bool state;

        public override ActionEvent Invoke()
        {
           GameObject.FindGameObjectWithTag(tag).GetComponent<AudioSource>().mute = state;
           return ActionEvent.Continue;
        }
    }
}