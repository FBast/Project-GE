using System;
using System.Collections.Generic;
using Production.Plugins.RyanScriptableObjects.SOEvents.IntEvents;
using Production.Plugins.RyanScriptableObjects.SOEvents.VoidEvents;
using Production.Plugins.RyanScriptableObjects.SOReferences.BoolReference;
using Production.Plugins.RyanScriptableObjects.SOReferences.FloatReference;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Production.Scripts.Events {
    public class EventEntity : MonoBehaviour {

        [Header("SO Events")]
        public List<IntEvent> RandomizedEvents = new List<IntEvent>();
        public IntEvent OnStartGame;
        public IntEvent OnEndGame;

        [Header("SO References")] 
        public BoolReference HasGameStarted;
        public FloatReference CurrentTime;
        
        [Header("Fixed Events Management")]
        public int CooldownBeforeStart;
        public int CooldownBeforeEnd;
        
        [Header("Random Events Management")]
        public AnimationCurve ProbabilityCurve;
        public float RandFrequency;
        public int MinDuration;
        public int MaxDuration;
        
        private float _probabilityIncrement;
        private float _eventTimer;
        private float _randTimer;
        private float _proba;
        private IntEvent _currentEvent;
        private bool _isGameEnding;
        
        private void Start() {
            _eventTimer = 0;
            _proba = 0;
        }

        private void Update() {
            if (CurrentTime.Value <= CooldownBeforeEnd && !_isGameEnding) {
                OnEndGame.Raise(CooldownBeforeEnd);
                _isGameEnding = true;
            }
            else {
                ComputeEventProbability();
            }
        }

        public void StartGame() {
            if (!HasGameStarted) {
                HasGameStarted.Value = true;
                OnStartGame.Raise(CooldownBeforeStart);
            }
        }

        private void ComputeEventProbability() {
            _eventTimer += Time.deltaTime;
            _randTimer += Time.deltaTime;
            if (_randTimer > RandFrequency) {
                _randTimer = 0;
                _proba = Random.Range(0f, 1f);
            }
            _probabilityIncrement = ProbabilityCurve.Evaluate(_eventTimer);
            if (_proba >= _probabilityIncrement) {
                TriggerEvent();
                _proba = 0;
                _eventTimer = 0;
            }
        }

        private void TriggerEvent() {
            _currentEvent = RandomizedEvents[Random.Range(0, RandomizedEvents.Count)];
            int eventDuration = Random.Range(MinDuration, MaxDuration);
            _currentEvent.Raise(eventDuration);
            Debug.Log("Event Triggered : " + _currentEvent.name + " for " + eventDuration + " seconds");
        }

    }

}