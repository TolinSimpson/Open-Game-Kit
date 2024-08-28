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
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Open Game Kit/Utility/Morse Code Generator")]
    public class MorseCodeGenerator : MonoBehaviour
    {
        public string text;

        [Space]
        public AudioSource audioSource;
        public AudioClip dotSound;
        public AudioClip dashSound;

        [Min(0)]
        public float dotDuration = 0.2f;
        [Min(0)]
        public float dashDuration = 0.6f;
        [Min(0)]
        public float letterGapDuration = 0.4f;
        [Min(0)]
        public float wordGapDuration = 1.0f;

        [Range(20, 20000)]
        public float dotFrequency = 1000f;
        [Range(20, 20000)]
        public float dashFrequency = 1000f;

        private void Awake()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            if (dotSound == null)
            {
                dotSound = GenerateTone("Dot", dotFrequency, dotDuration);
            }

            if (dashSound == null)
            {
                dashSound = GenerateTone("Dash", dashFrequency, dashDuration);
            }


            GenerateMorseCodeAudio(text);
        }

        private Dictionary<char, string> morseCodeDictionary = new Dictionary<char, string>()
    {
        {'A', ".-"}, {'B', "-..."}, {'C', "-.-."}, {'D', "-.."}, {'E', "."}, {'F', "..-."},
        {'G', "--."}, {'H', "...."}, {'I', ".."}, {'J', ".---"}, {'K', "-.-"}, {'L', ".-.."},
        {'M', "--"}, {'N', "-."}, {'O', "---"}, {'P', ".--."}, {'Q', "--.-"}, {'R', ".-."},
        {'S', "..."}, {'T', "-"}, {'U', "..-"}, {'V', "...-"}, {'W', ".--"}, {'X', "-..-"},
        {'Y', "-.--"}, {'Z', "--.."}, {'0', "-----"}, {'1', ".----"}, {'2', "..---"},
        {'3', "...--"}, {'4', "....-"}, {'5', "....."}, {'6', "-...."}, {'7', "--..."},
        {'8', "---.."}, {'9', "----."}
    };


        private AudioClip GenerateTone(string clipName, float frequency, float duration)
        {
            int sampleRate = AudioSettings.outputSampleRate;
            int numSamples = Mathf.RoundToInt(duration * sampleRate);

            float[] samples = new float[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * i / sampleRate);
            }

            AudioClip clip = AudioClip.Create(clipName, numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);

            return clip;
        }

        public void GenerateMorseCodeAudio(string text)
        {
            StartCoroutine(PlayMorseCodeAudio(text));
        }

        private IEnumerator PlayMorseCodeAudio(string text)
        {
            foreach (char c in text.ToUpper())
            {
                if (morseCodeDictionary.ContainsKey(c))
                {
                    string morseCode = morseCodeDictionary[c];
                    foreach (char symbol in morseCode)
                    {
                        if (symbol == '.')
                        {
                            audioSource.PlayOneShot(dotSound);
                            yield return new WaitForSeconds(dotDuration);
                        }
                        else if (symbol == '-')
                        {
                            audioSource.PlayOneShot(dashSound);
                            yield return new WaitForSeconds(dashDuration);
                        }
                    }
                    yield return new WaitForSeconds(letterGapDuration);
                }
                else if (c == ' ')
                {
                    yield return new WaitForSeconds(wordGapDuration);
                }
            }
        }
    }
}