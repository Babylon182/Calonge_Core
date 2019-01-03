using System;
using System.Globalization;
using CalongeCore.Events;
using Events;
using UnityEngine;
using UnityEngine.UI;

namespace CalongeCore.ScoreManager
{
	public class ScoreManager : MonoBehaviour
	{
		[SerializeField]
		private FloatVariable totalScore;
		
		[SerializeField]
		private Text scoreText;
		
		[SerializeField]
		private GameObject combo;
		
		[SerializeField]
		private Text comboText;
		
		[SerializeField]
		private Image comboMeter;
		
		[SerializeField]
		private ComboValues comboValues;

		private float comboDuration;

		private const int COMBO_MIN_VALUE = 1;
		
		private void Awake()
		{
			EventsManager.SubscribeToEvent<ScoreEvent>(OnScoreEvent);
			EventsManager.SubscribeToEvent<HeroDamaged>(OnHeroDamaged);
			scoreText.text = totalScore.value.ToString(CultureInfo.CurrentCulture);
			combo.SetActive(false);
			comboDuration = comboValues.duration;
		}

		private void OnHeroDamaged()
		{
			ResetCombo();
		}

		private void ResetCombo()
		{
			comboValues.currentKillAmount = 0;
			comboValues.currentMultiplier = COMBO_MIN_VALUE;
			comboDuration = comboValues.duration;
			comboValues.isActive = false;
			combo.SetActive(false);
		}

		private void AddCComboMultiplier()
		{
			combo.SetActive(true);
			comboValues.isActive = true;
			comboValues.currentKillAmount = 0;
			comboValues.currentMultiplier = Mathf.Clamp(++comboValues.currentMultiplier, COMBO_MIN_VALUE , comboValues.maxMultiplier);
			comboText.text = comboValues.currentMultiplier.ToString(CultureInfo.CurrentCulture);
		}

		private void OnScoreEvent(ScoreEvent gameEvent)
		{
			AddPointsToScore(gameEvent.points);
			UpdateComboValues();
			scoreText.text = totalScore.value.ToString(CultureInfo.CurrentCulture);
		}

		private void AddPointsToScore(float points)
		{
			totalScore.value += points * comboValues.currentMultiplier;
		}

		private void UpdateComboValues()
		{
			comboValues.currentKillAmount++;
			comboDuration = comboValues.duration;

			if (comboValues.currentKillAmount >= comboValues.increaseMultiplierAt)
			{
				AddCComboMultiplier();
			}
		}

		private void CheckComboTimer()
		{
			if (comboValues.isActive)
			{
				comboMeter.fillAmount = comboDuration / comboValues.duration;
				
				comboDuration -= Time.deltaTime;

				if (comboDuration <= 0)
				{
					ResetCombo();
				}
			}
		}

		private void Update()
		{
			CheckComboTimer();
		}

		private void OnDestroy()
		{
			EventsManager.UnsubscribeToEvent<ScoreEvent>(OnScoreEvent);
			totalScore.value = 0f;
		}
	}

	internal class ScoreEvent : IGameEvent
	{
		public float points;

		public ScoreEvent(float points)
		{
			this.points = points;
		}
	}

	[Serializable]
	public class ComboValues
	{
		public float maxMultiplier;
		public float duration;
		public float increaseMultiplierAt;
		
		[HideInInspector] public float currentKillAmount;
		[HideInInspector] public float currentMultiplier = 1;
		[HideInInspector] public bool isActive;
	}
}