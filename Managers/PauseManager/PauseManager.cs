using UnityEngine;
using UnityEngine.UI;

namespace CalongeCore.PauseManager
{
	public class PauseManager : Singleton<PauseManager>
	{
		private const int PAUSE_VALUE = 0;
		private const int NORMAL_VALUE = 1;

		private bool isPaused;
		
		public void Pause()
		{
            isPaused = true;
            Time.timeScale = PAUSE_VALUE;
		}

		public void Unpause()
		{
            isPaused = false;
            Time.timeScale = NORMAL_VALUE;
		}

		//Button Function
		public void TogglePause()
		{
			isPaused = !isPaused;
			Time.timeScale = isPaused ? PAUSE_VALUE : NORMAL_VALUE;
        }
	}
}
