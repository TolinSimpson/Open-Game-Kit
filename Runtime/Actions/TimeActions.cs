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
    [SRName("Time/Yield Sequence")]
    public class YieldSequenceAction : ActionModule
    {
        [Message("The whole sequence permanently plays as a coroutine once run.")]
        [SerializeField, Min(0.001f)] private float time = 1;
        public override ActionEvent Invoke() { delay = time; return ActionEvent.Continue; }
    }

    [SRName("Time/Yield Sequence Range")]
    public class YieldSequenceRangeAction : ActionModule
    {
        [Message("The whole sequence permanently plays as a coroutine once run.")]
        [SerializeField, Min(0.001f)] private float minTime = 1;
        [SerializeField, Min(0.001f)] private float maxTime = 1;
        private float randomTime = -1;
        public override ActionEvent Invoke()
        {
            if(randomTime == -1)
            {
                randomTime = Random.Range(minTime, maxTime);
            }
            delay = randomTime;
            return ActionEvent.Continue;
        }
    }

    [SRName("Time/Pause Sequence")]
    public class PauseSequenceAction : ActionModule
    {
        [Message("Note: Only works if the sequence is played in an update loop.")]
        [Min(0.001f)]
        public float time = 1;
        private float currentTime = 0;
        public TimeScales timeScale = TimeScales.deltaTime;
        public override ActionEvent Invoke()
        {
            delay = time;
            if (currentTime < time)
            {
                switch(timeScale)
                {
                    case TimeScales.deltaTime: currentTime += Time.deltaTime; break;
                    case TimeScales.fixedDeltaTime: currentTime += Time.fixedDeltaTime; break;
                    case TimeScales.fixedUnscaledDeltaTime: currentTime += Time.fixedUnscaledDeltaTime; break;
                    case TimeScales.fixedUnscaledTime: currentTime += Time.fixedUnscaledTime; break;
                    case TimeScales.time: currentTime += Time.time; break;
                    case TimeScales.unscaledDeltaTime: currentTime += Time.unscaledDeltaTime; break;
                    case TimeScales.unscaledTime: currentTime += Time.unscaledTime; break;
                }

                return ActionEvent.Hold;
            }
            else
            {
                currentTime = 0;
                return ActionEvent.Release;
            }
        }
    }

    [SRName("Time/Pause Sequence For Delta Time")]
    public class PauseSequenceDeltaTimeAction : ActionModule
    {
        [Message("Note: Only works if the sequence is played in an update loop.")]
        [Min(0.001f)]
        public float time = 1;
        private float currentTime = 0;
        public override ActionEvent Invoke()
        {
            delay = time;
            if (currentTime < time)
            {
                currentTime += Time.deltaTime;
                return ActionEvent.Hold;
            }
            else
            {
                currentTime = 0;
                return ActionEvent.Release;
            }
        }
    }

    [SRName("Time/Pause Sequence Range DT")]
    public class PauseSequenceRangeDTAction : ActionModule
    {
        [Message("Note: Only works if the sequence is played in an update loop.")]
        [SerializeField, Min(0.001f)] private float minTime = 1;
        [SerializeField, Min(0.001f)] private float maxTime = 1;
        [SerializeField] private bool randomizeOnComplete = false;
        private float currentTime = 0;
        private float randomTime = -1;
        public override ActionEvent Invoke()
        {
            if(randomTime == -1)
            {
                randomTime = Random.Range(minTime, maxTime);
            }
            delay = randomTime;
            if (currentTime < randomTime)
            {
                currentTime += Time.deltaTime;
                return ActionEvent.Hold;
            }
            else
            {
                currentTime = 0;
                if(randomizeOnComplete == true)
                {
                    randomTime = -1;
                }
                return ActionEvent.Release;
            }
        }
    }

    [SRName("Time/Pause Sequence For Unscaled Time")]
    public class PauseSequenceUnscaledTimeAction : ActionModule
    {
        [Min(0.001f)]
        public float time = 1;
        private float currentTime = 0;
        public override ActionEvent Invoke()
        {
            if (currentTime < time)
            {
                currentTime += Time.unscaledDeltaTime;
                return ActionEvent.Hold;
            }
            else
            {
                currentTime = 0;
                return ActionEvent.Release;
            }
        }
    }

    [SRName("Time/Set Time Scale")]
    public class SetTimeScaleAction : ActionModule
    {
        public float timeScale = 1;

        public override ActionEvent Invoke()
        {
            Time.timeScale = timeScale;
            return ActionEvent.Continue;
        }
    }

    [SRName("Time/Conditions/Time Scale Comparison")]
    public class TimeScaleComparisonAction : ActionModule
    {
        public NumericalComparisons comparison = NumericalComparisons.EqualTo;
        public float value = 1;
        public ActionEvent ifTrue, ifFalse;
        public override ActionEvent Invoke() { return LogicOperations.NumericalComparison(Time.timeScale, comparison, value) ? ifTrue : ifFalse; }
    }
}