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
    [AddComponentMenu("Open Game Kit/Gameplay/Action Sequencer")]
    public class ActionSequencer : MonoBehaviour
    {
        [Tooltip("What update loop the action sequencer should run in. (Applies to all sequences).")]
        public UpdateModes updateMode = UpdateModes.Update;

        public List<ActionSequence> sequences = new List<ActionSequence>();

        [Min(0)]
        public int currentSequenceIndex = 0;

#if UNITY_EDITOR
        public int previousPage = 0;
        public int pagination = 0;
        public bool loadingSO = false;
#endif

        #region Initialzation & Updates:

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            if (AppManager.Singleton == null)
            {
                AppManager.Singleton = new GameObject("Application Manager").AddComponent<AppManager>();
                Debug.LogWarning("|Action Sequencer|: AppManager instance not found, creating one...", gameObject);
            }
        }

        private void OnEnable()
        {
            if (AppManager.Singleton != null)
            {
                AppManager.Singleton.Register(this);
            }
            else
            {
                AppManager.Singleton = new GameObject("Application Manager").AddComponent<AppManager>();
                Debug.LogWarning("|Action Sequencer|: AppManager instance not found, creating one...", gameObject);
                AppManager.Singleton.Register(this);
            }
           // onEnable.Invoke();
        }

        private void OnDisable()
        {
            if (AppManager.Singleton != null)
            {
                AppManager.Singleton.Unregister(this);
            }
          //  onDisable.Invoke();
        }

        #endregion

        /// <summary>
        /// Plays the current sequence.
        /// </summary>
        public void PlaySequence()
        {
            sequences[currentSequenceIndex].Play();
        }
        public void PlaySequence(int sequenceIndex)
        {
            currentSequenceIndex = Mathf.Clamp(sequenceIndex, 0, sequences.Count);
            PlaySequence();
        }

        public void EnableSequence(int sequenceIndex)
        {
            if (sequenceIndex < 0 || sequenceIndex > sequences.Count)
            {
                Debug.LogWarning("Tried to enable a sequence out of range.");
            }
            else
            {
                sequences[sequenceIndex].enabled = true;
            }
        }

        public ActionSequence GetCurrentSequence()
        {
            return sequences[currentSequenceIndex];
        }
    }

    [SRName("Action Sequence/Sequencer Event")]
    public class SequencerEventAction : ActionModule
    {
        public ActionSequencer sequencer;

        public SequencerEvents sequencerEvent = SequencerEvents.Repeat;
        public enum SequencerEvents { Repeat, PlayNext, PlayPrevious, PlayFirst, PlayLast, PlayRandom, DisableCurrentSequence, DisablePreviousSequence }

        public override ActionEvent Invoke() 
        { 
            switch(sequencerEvent)
            {
                case SequencerEvents.Repeat: sequencer.PlaySequence(); break;
                case SequencerEvents.PlayNext: sequencer.PlaySequence(sequencer.currentSequenceIndex++); break;
                case SequencerEvents.PlayPrevious: sequencer.PlaySequence(sequencer.currentSequenceIndex--); break;
                case SequencerEvents.PlayFirst: sequencer.PlaySequence(0); break;
                case SequencerEvents.PlayLast: sequencer.PlaySequence(sequencer.sequences.Count); break;
                case SequencerEvents.PlayRandom: sequencer.PlaySequence(Random.Range(0, sequencer.sequences.Count)); break;
                case SequencerEvents.DisableCurrentSequence: sequencer.sequences[sequencer.currentSequenceIndex].enabled = false; break;
                case SequencerEvents.DisablePreviousSequence: if(sequencer.currentSequenceIndex > 0) { sequencer.sequences[sequencer.currentSequenceIndex -1].enabled = false; } break;
            }
            return ActionEvent.Stop; 
        }
    }

}