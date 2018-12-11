using UnityEngine;

namespace CalongeCore.PauseManager
{
	public class PauseManager : MonoBehaviour
	{
		private const int PAUSE_VALUE = 0;
		private const int NORMAL_VALUE = 1;

		private bool isPaused;
		
		public void Pause()
		{
			Time.timeScale = PAUSE_VALUE;
		}

		public void Unpause()
		{
			Time.timeScale = NORMAL_VALUE;
		}

		//Button Function
		public void TogglePause()
		{
			isPaused = !isPaused;
			Time.timeScale = isPaused ? NORMAL_VALUE : PAUSE_VALUE;
		}
	}
}
