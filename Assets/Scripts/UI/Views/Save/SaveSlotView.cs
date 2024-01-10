using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI.Views.Save
{
	public class SaveSlotView : MonoBehaviour
	{
		[SerializeField]
		private RawImage _saveImage;
		
		[SerializeField]
		private Button _selectSaveButton;
		
		[SerializeField]
		private TextMeshProUGUI _saveName;
		
		public RawImage SaveImage => _saveImage;
		public Button SelectSaveButton => _selectSaveButton;
		public TextMeshProUGUI SaveName => _saveName;
	}
}