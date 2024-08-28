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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OGK
{
    /// <summary>
    /// Consolidates <see cref="ActionSequencer"/> update loops and manages object pooling improving game performance.
    /// </summary>
    [HelpURL("https://kitbashery.com/docs/smart-gameobjects/smart-manager.html")]
    [DefaultExecutionOrder(-21)]
    [DisallowMultipleComponent]
    [AddComponentMenu("Open Game Kit/Gameplay/Application Manager")]
    public class AppManager : MonoBehaviour
    {
        #region Properties:

        /// <summary>
        /// An instance of <see cref="AppManager"/> in the current scene (there should only be one at a time).
        /// </summary>
        public static AppManager Singleton;

        [SerializeField, Min(1), Tooltip("The amount of framerate frames between updates. (Does not apply to Action Sequencers with rigidbodies, which update every fixed framerate frame).")]
        private int updateDelay = 1;
        [SerializeField, Tooltip("If enabled then a frame delay between Action Sequencer updates will be increased if the game's framerate is below the target framerate.")]
        private bool throttleUpdates = false;
        [SerializeField, Tooltip("The frames per second target the application will try to run at. (overrides Application.targetFrameRate)."), Range(-1, 300)]
        private int targetFrameRate = -1;
        /// <summary>
        /// The current frame count used when throttling <see cref="ActionSequencer"/> updates from the <see cref="AppManager"/> instance.
        /// </summary>
        private int frames = 0;
        [HideInInspector, Tooltip("The time between the manager's last frame update and the current one.")]
        public float currentFPS = 0;
        [SerializeField, Tooltip("If enabled pauses all Action Sequencer updates.")]
        private bool pauseUpdates = false;

        /// <summary>
        /// A non-reorderable dictionary of <see cref="Pool"/>s.
        /// </summary>
        [SerializeField, NonReorderable]
        private List<Pool> pools = new List<Pool>();
        /// <summary>
        /// A dictionary created from <see cref="pools"/> on Awake() so pools can be found via their prefab name as a key.
        /// </summary>
        private Dictionary<string, Pool> poolLookup = new Dictionary<string, Pool>();
        /// <summary>
        /// Temporary <see cref="GameObject"/> used when a <see cref="GameObject"/> is needed as a variable in an iteration loop.
        /// </summary>
        private GameObject tmp;
        /// <summary>
        /// The last GameObject activated from a pool;
        /// </summary>
        private GameObject lastSpawned;
        /// <summary>
        /// Temporary <see cref="Pool"/> used when a <see cref="Pool"/> struct is needed as a variable in an interation loop.
        /// </summary>
        private Pool tempPool;

        [SerializeField, Tooltip("Managed Sequencers (Updated once per frame")]
        private List<ActionSequencer> standardUpdateSequencers = new List<ActionSequencer>();
        [SerializeField, Tooltip("Managed Sequencers (Updated every fixed framerate frame)")]
        private List<ActionSequencer> fixedUpdateSequencers = new List<ActionSequencer>();
        [SerializeField, Tooltip("Managed Sequencers (Updated every frame after standard updates)")]
        private List<ActionSequencer> lateUpdateSequencers = new List<ActionSequencer>();

        [SerializeField] private ActionSequence onApplicationQuit;

        private const string space = " ";

        /// <summary>
        /// Time delay cache for coroutine yields.
        /// </summary>
        public Dictionary<float, WaitForSeconds> delays = new Dictionary<float, WaitForSeconds>();

        #endregion

        #region Initialization & Updates:

        private void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Destroy(gameObject);
            }

            Application.targetFrameRate = targetFrameRate;

            CreatePools();
        }

        // Update is called once per frame
        void Update()
        {
            if (pauseUpdates == false && standardUpdateSequencers.Count > 0)
            {
                if (throttleUpdates == true)
                {
                    currentFPS = 1f / Time.unscaledDeltaTime;

                    // TODO: Should there should be a target delta to compare the current fps to instead of directly to the target frame rate?
                    if (currentFPS < Application.targetFrameRate && updateDelay < Application.targetFrameRate)
                    {
                        updateDelay = Application.targetFrameRate - (int)currentFPS + 1;
                    }
                    else
                    {
                        updateDelay = 1;
                    }

                    frames++;
                    if (frames >= updateDelay)
                    {
                        for (int i = standardUpdateSequencers.Count - 1; i >= 0; i--)
                        {
                            standardUpdateSequencers[i].PlaySequence();
                        }
                        frames = 0;
                    }
                }
                else
                {
                    for (int i = standardUpdateSequencers.Count - 1; i >= 0; i--)
                    {
                        standardUpdateSequencers[i].PlaySequence();
                    }
                }
            }
        }

        // FixedUpdate is called every fixed framerate frame
        private void FixedUpdate()
        {
            if (pauseUpdates == false && fixedUpdateSequencers.Count > 0)
            {
                for (int i = fixedUpdateSequencers.Count - 1; i >= 0; i--)
                {
                    fixedUpdateSequencers[i].PlaySequence();
                }
            }
        }

        // LateUpdate is called every frame, if the monobehavior is enabled
        private void LateUpdate()
        {
            if (pauseUpdates == false && lateUpdateSequencers.Count > 0)
            {
                for (int i = lateUpdateSequencers.Count - 1; i >= 0; i--)
                {
                    lateUpdateSequencers[i].PlaySequence();
                }
            }
        }

        private void OnApplicationQuit()
        {
            if(onApplicationQuit != null)
            {
                onApplicationQuit.Play();
            }
        }

        #endregion

        #region Core Methods:

        /// <summary>
        /// Registers a <see cref="ActionSequencer"/> with this <see cref="AppManager"/> singleton instance.
        /// </summary>
        /// <param name="sequencer">The <see cref="ActionSequencer"/> to register.</param>
        public void Register(ActionSequencer sequencer)
        {
            switch(sequencer.updateMode)
            {
                case UpdateModes.Update:

                    if(standardUpdateSequencers.Contains(sequencer) == false)
                    {
                        standardUpdateSequencers.Add(sequencer);
                    }

                    break;

                case UpdateModes.FixedUpdate:

                    if (fixedUpdateSequencers.Contains(sequencer) == false)
                    {
                        fixedUpdateSequencers.Add(sequencer);
                    }

                    break;

                case UpdateModes.LateUpdate:

                    if (lateUpdateSequencers.Contains(sequencer) == false)
                    {
                        lateUpdateSequencers.Add(sequencer);
                    }

                    break;
            }
        }

        /// <summary>
        /// Unregisters a <see cref="ActionSequencer"/> with this <see cref="AppManager"/> singleton instance.
        /// </summary>
        /// <param name="sequencer">The <see cref="ActionSequencer"/> to unregister.</param>
        public void Unregister(ActionSequencer sequencer)
        {
            switch (sequencer.updateMode)
            {
                case UpdateModes.Update:

                    standardUpdateSequencers.Remove(sequencer);

                    break;

                case UpdateModes.FixedUpdate:

                    fixedUpdateSequencers.Remove(sequencer);

                    break;

                case UpdateModes.LateUpdate:

                    lateUpdateSequencers.Remove(sequencer);

                    break;
            }
        }

        public void RegisterAll()
        {
            standardUpdateSequencers.Clear();
            fixedUpdateSequencers.Clear();
            lateUpdateSequencers.Clear();
            if (FindObjectOfType<ActionSequencer>() != null)
            {
                foreach (ActionSequencer sequencer in FindObjectsOfType<ActionSequencer>())
                {
                    if (sequencer.gameObject.activeSelf == true)
                    {
                        Register(sequencer);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="pauseUpdates"/> boolean to the specified state.
        /// </summary>
        /// <param name="state">The state to se <see cref="pauseUpdates"/> to.</param>
        public void PauseActionSequencers(bool state)
        {
            pauseUpdates = state;
        }

        /// <summary>
        /// Sets the <see cref="pauseUpdates"/> boolean to the opposite of its current state.
        /// </summary>
        public void TogglePauseActionSequencers()
        {
            pauseUpdates = !pauseUpdates;
        }

        /// <summary>
        /// Sets the <see cref="throttleUpdates"/> boolean to the specified state.
        /// </summary>
        /// <param name="state">The state to se <see cref="throttleUpdates"/> to.</param>
        public void ThrottleActionSequencerUpdates(bool state)
        {
            throttleUpdates = state;
        }

        /// <summary>
        /// Sets the <see cref="throttleUpdates"/> boolean to the opposite of its current state.
        /// </summary>
        public void ToggleActionSequencerUpdateThrottle()
        {
            throttleUpdates = !throttleUpdates;
        }

        #endregion

        #region Pooling Methods:

        /// <summary>
        /// This should be called after the <see cref="pools"/> list is modified.
        /// </summary>
        public void CreatePools()
        {
            if (pools.Count > 0)
            {
                for (int i = pools.Count - 1; i >= 0; i--)
                {
                    if (pools[i].prefab != null && pools[i].amount > 0)
                    {
                        if (!poolLookup.ContainsKey(pools[i].prefab.name))
                        {
                            PopulatePool(pools[i]);
                            poolLookup.Add(pools[i].prefab.name, pools[i]);
                        }
                        else
                        {
                            Debug.LogWarningFormat("|Application Manager|: Duplicate pools for object named {0} detected, removing...", pools[i].prefab.name, gameObject);
                            pools.Remove(pools[i]);
                        }
                    }
                    else
                    {
                        pools.Remove(pools[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Instantiates GameObjects from the specified pool.
        /// </summary>
        /// <param name="pool">The pool to populate.</param>
        private void PopulatePool(Pool pool)
        {
            if (pool.pooledObjects.Count < pool.amount)
            {
                for (int i = 0; i < pool.amount; i++)
                {
                    tmp = Instantiate(pool.prefab);
                    tmp.SetActive(false);
                    tmp.gameObject.hideFlags = pool.hideFlags;
                    if (pool.sequencialNaming == true)
                    {
                        tmp.gameObject.name = pool.prefab.name + space + i;
                    }
                    pool.pooledObjects.Add(tmp);
                }
            }
        }

        /// <summary>
        /// Increases the size of a pool & instantiates the pool's prefab to meet the new amount.
        /// </summary>
        /// <param name="prefabName">The name of the pool to increase the amount of (same as the pool's prefab name).</param>
        /// <param name="amount">The amount to increase the pool's amount by (will only increase to the max amount set in the inspector).</param>
        public void ExpandPool(string prefabName, int amount)
        {
            if (poolLookup.ContainsKey(prefabName))
            {
                tempPool = poolLookup[prefabName];
                tempPool.amount += amount;
                if (tempPool.amount > tempPool.maxAmount)
                {
                    tempPool.amount = tempPool.maxAmount;
                }
                PopulatePool(tempPool);
                poolLookup[prefabName] = tempPool;
            }
        }

        /// <summary>
        /// Destroys all objects in all pools.
        /// </summary>
        /// <param name="omitActive">Preserve pooled GameObjects that are currently active in the scene.</param>
        public void DestroyPools(bool omitActive)
        {
            foreach (Pool pool in poolLookup.Values)
            {
                for (int i = pool.pooledObjects.Count; i > 0; i--)
                {
                    if (omitActive == false)
                    {
                        Destroy(pool.pooledObjects[i]);
                    }
                    else
                    {
                        if (pool.pooledObjects[i].activeSelf == false)
                        {
                            Destroy(pool.pooledObjects[i]);
                        }
                    }
                }

                pool.pooledObjects.Clear();
            }
        }

        /// <summary>
        /// Activates a pooled <see cref="GameObject"/> in a pool specified by the name of its prefab.
        /// </summary>
        /// <param name="prefabName">The name of the pool that contains the prefab to activate.</param>
        public void ActivatePooledObject(string prefabName)
        {
            if (!string.IsNullOrEmpty(prefabName) && poolLookup.ContainsKey(prefabName))
            {
                for (int i = 0; i < poolLookup[prefabName].amount; i++)
                {
                    if (!poolLookup[prefabName].pooledObjects[i].activeInHierarchy)
                    {
                        poolLookup[prefabName].pooledObjects[i].SetActive(true);
                    }
                }
            }
            else
            {
                Debug.LogWarningFormat("|Application Manager|: failed to activate GameObject from pool ({0}) GameObject will be null, make sure the name is correct.", prefabName, gameObject);
            }
        }

        /// <summary>
        /// Gets an inactive pooled <see cref="GameObject"/> by its prefab name.
        /// </summary>
        /// <param name="prefabName">The name of the prefab to get an instance of.</param>
        /// <returns>The first inactive prefab instance.</returns>
        public GameObject GetPooledObject(string prefabName)
        {
            if (!string.IsNullOrEmpty(prefabName) && poolLookup.ContainsKey(prefabName))
            {
                for (int i = 0; i < poolLookup[prefabName].amount; i++)
                {
                    if (!poolLookup[prefabName].pooledObjects[i].activeInHierarchy)
                    {
                        return poolLookup[prefabName].pooledObjects[i];
                    }
                }
            }
            else
            {
                Debug.LogWarningFormat("|Application Manager|: failed to get GameObject from pool ({0}) GameObject will be null, make sure the name is correct.", prefabName, gameObject);
            }

            return null;
        }

        public void SpawnAtRaycastHit(RaycastHit hit, string poolName)
        {
            if (hit.collider != null)
            {
                lastSpawned = GetPooledObject(poolName);
                if (lastSpawned != null)
                {
                    lastSpawned.transform.position = hit.point;
                    lastSpawned.transform.rotation = Quaternion.Euler(hit.normal);
                    lastSpawned.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("|Application Manager|: Tried to spawn a GameObject but failed to get one from the pool. Make sure the provided name is correct and there is an adequate amount available.", gameObject);
                }
            }
        }

        public void SpawnAtTransform(Transform target, string poolName, bool parent = false)
        {
            if (target != null)
            {
                lastSpawned = GetPooledObject(poolName);
                if (lastSpawned != null)
                {
                    lastSpawned.transform.position = target.transform.position;
                    lastSpawned.transform.rotation = target.transform.rotation;
                    if(parent == true)
                    {
                        lastSpawned.transform.SetParent(target);
                    }
                    lastSpawned.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("|Application Manager|: Tried to spawn a GameObject but failed to get one from the pool. Make sure the provided name is correct and there is an adequate amount available.", gameObject);
                }
            }
        }

        public void SpawnAtPosition(Vector3 position, Quaternion rotation, string poolName)
        {
            lastSpawned = GetPooledObject(poolName);
            if (lastSpawned != null)
            {
                lastSpawned.transform.position = position;
                lastSpawned.transform.rotation = rotation;
                lastSpawned.SetActive(true);
            }
            else
            {
                Debug.LogWarning("|Application Manager|: Tried to spawn a GameObject but failed to get one from the pool. Make sure the provided name is correct and there is an adequate amount available.", gameObject);
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a pool of <see cref="GameObject"/>s & their instantiation criteria.
    /// </summary>
    [Serializable]
    public struct Pool
    {
        [Tooltip("The object to pool. The name of this object is also the name of the pool.")]
        public GameObject prefab;
        [Range(1, 5000), Tooltip("The next amount of GameObjects to instantiate.")]
        public int amount;
        [Range(1, 5000), Tooltip("The maximum size of the pool, pools can not exceed this amount when expanding.")]
        public int maxAmount;
        [Tooltip("HideFlags for prefabs instantiated via the pooling system. Useful for hiding pooled objects in the heirarchy.")]
        public HideFlags hideFlags;
        [Tooltip("Use numbered names for GameObjects instead of name(clone).")]
        public bool sequencialNaming;
        [Tooltip("The GameObjects currently pooled.")]
        public List<GameObject> pooledObjects;
    }
}

