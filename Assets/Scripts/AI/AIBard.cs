﻿using UnityEngine;
using System.Collections.Generic;

namespace Bardmages.AI {
    /// <summary>
    /// Hooks AI decisions into generating notes and attacks.
    /// </summary>
    class AIBard : BaseBard {

        /// <summary> The tune button currently being pressed. </summary>
        private ControllerInputWrapper.Buttons pressedButton;
        /// <summary> The rhythm that the current tune is being played at. </summary>
        private LevelManager.RhythmType rhythmType;

        /// <summary> The current tune that the bardmage is playing. </summary>
        internal Tune currentTune;
        /// <summary> The progress of the bardmage through its current tune. </summary>
        private int tuneProgress;
        /// <summary> Whether the bardmage is currently playing a tune. </summary>
        internal bool isPlayingTune {
            get { return currentTune != null; }
        }

        /// <summary> The minimum time delay between notes being played. </summary>
        private float noteDelay;

        /// <summary> The bardmage's variance for playing on the beat. </summary>
        [HideInInspector]
        internal float timingVariance;
        /// <summary> The percentage threshold of beat correctness for which the bardmage will make a note at. </summary>
        private float modifiedThreshold = Tune.PERFECT_THRESHOLD;

        /// <summary>
        /// Calculates the time delay between notes.
        /// </summary>
        protected override void Start() {
            base.Start();
            noteDelay = LevelManager.instance.Tempo / 3;
        }

        /// <summary>
        /// Checks if a certain button was pressed.
        /// </summary>
        /// <returns>Whether the button was pressed.</returns>
        /// <param name="button">The button to check for.</param>
        protected override bool GetButtonDown(ControllerInputWrapper.Buttons button) {
            return pressedButton == button;
        }

        /// <summary>
        /// Checks if a certain button is being pressed.
        /// </summary>
        /// <returns>Whether the button is being pressed.</returns>
        /// <param name="button">The button to check for.</param>
        protected override bool GetButton(ControllerInputWrapper.Buttons button) {
            return GetButtonDown(button);
        }

        /// <summary>
        /// Updates the bard's progress through the current tune.
        /// </summary>
        internal void UpdateTune() {
            // Reset the current button.
            pressedButton = ControllerInputWrapper.Buttons.Start;

            // Plays the tune on the beat.
            if (currentTune != null && buttonPressDelayTimer < 0f && LevelManager.instance.PerfectTiming(rhythmType) >= modifiedThreshold) {
                pressedButton = currentTune.tune[tuneProgress];
                if (++tuneProgress >= currentTune.tune.Length) {
                    tuneProgress = 0;
                    currentTune = null;
                }
                buttonPressDelay = Mathf.Max(buttonPressDelayTimer, noteDelay);

                modifiedThreshold = Tune.PERFECT_THRESHOLD + Random.Range(-timingVariance, timingVariance);
            }
        }

        /// <summary>
        /// Starts executing a tune attack.
        /// </summary>
        /// <param name="tuneIndex">The index of the tune attack.</param>
        /// <param name="overrideCurrent">Whether to override the current action if already executing one.</param>
        /// <param name="rhythmType">The rhythm to play the tune at.</param>
        internal void StartTune(int tuneIndex, bool overrideCurrent = false, LevelManager.RhythmType rhythmType = LevelManager.RhythmType.None) {
            if (overrideCurrent || !isPlayingTune) {
                currentTune = tunes[tuneIndex];
                this.rhythmType = rhythmType;
            }
        }
    }
}
