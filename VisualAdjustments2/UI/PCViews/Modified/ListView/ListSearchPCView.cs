using Kingmaker.Blueprints.Root.Strings;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.Controls.Other;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UniRx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UniRx;

namespace VisualAdjustments2.UI
{
    public class ListSearchPCView : ViewBase<ListSearchVM>
    {
		public void SetupFromChargenFeatureSearchPCView(CharGenFeatureSearchPCView old)
        {
			this.m_DropdownButton = old.m_DropdownButton;
			
			this.m_EnterPlaceholder = old.m_EnterPlaceholder;
			this.m_InputButton = old.m_InputButton;
			this.m_InputField = old.m_InputField;
			this.m_Placeholder = old.m_Placeholder;
			this.Dropdown = old.Dropdown;
			this.SearchRequest = old.SearchRequest;
			UnityEngine.Component.Destroy(old);
        }
		public void Initialize()
		{
			if (this.m_IsInit)
			{
				return;
			}
			this.m_IsInit = true;
		}
		public override void BindViewImplementation()
		{
			this.SearchRequest.Value = string.Empty;
			base.AddDisposable(this.SearchRequest.Subscribe(delegate (string val)
			{
				this.SearchRequest.Value = val;
				this.UpdatePlaceholder();
			}));
			base.AddDisposable(this.m_DropdownButton.OnLeftClickAsObservable().Subscribe(new Action(this.ShowDwopdown)));
			base.AddDisposable(this.m_InputField.OnEndEditAsObservable().Subscribe(new Action<string>(this.OnEndEdit)));
			base.AddDisposable(this.m_InputButton.OnLeftClickAsObservable().Subscribe(delegate (Unit _)
			{
				this.OnEdit();
			}));
			this.m_InputField.gameObject.SetActive(false);
			this.m_Placeholder.gameObject.SetActive(true);
			this.m_EnterPlaceholder.text = UIStrings.Instance.CharGen.EnterSearchTextHere;
			this.SetupDropdown();
			base.AddDisposable(this.Dropdown.onValueChanged.AsObservable<int>().Subscribe(new Action<int>(this.SetValueFromUI)));
		}
		public void ShowDwopdown()
		{
			if (this.Dropdown.IsExpanded)
			{
				this.Dropdown.Hide();
				return;
			}
			this.Dropdown.Show();
		}
		public void SetupDropdown()
		{
			this.Dropdown.ClearOptions();
			if (base.ViewModel.LocalizedValues != null && base.ViewModel.LocalizedValues.Count > 0)
			{
				this.Dropdown.gameObject.SetActive(true);
				this.Dropdown.AddOptions(base.ViewModel.LocalizedValues);
				return;
			}
			this.Dropdown.gameObject.SetActive(false);
			this.Dropdown.AddOptions((from i in Enumerable.Range(0, 10)
									  select string.Format("Generic option {0}", i)).ToList<string>());
		}
		public void SetValueFromUI(int value)
		{
			this.SearchRequest.Value = base.ViewModel.LocalizedValues[value];
		}
		public void UpdatePlaceholder()
		{
			if (string.IsNullOrEmpty(this.SearchRequest.Value))
			{
				this.m_Placeholder.text = UIStrings.Instance.CharGen.FeatureSearchText;
				return;
			}
			this.m_Placeholder.text = this.SearchRequest.Value;
		}
		public void OnEdit()
		{
			this.m_InputButton.gameObject.SetActive(false);
			this.m_InputField.gameObject.SetActive(true);
			this.m_InputField.Select();
			this.m_InputField.ActivateInputField();
		}
		public void OnEndEdit(string text)
		{
			this.m_InputField.gameObject.SetActive(false);
			this.m_InputButton.gameObject.SetActive(true);
			this.SearchRequest.Value = text;
			this.UpdatePlaceholder();
		}
		public override void DestroyViewImplementation()
		{
			this.SearchRequest.Value = string.Empty;
			this.m_Placeholder.text = UIStrings.Instance.CharGen.FeatureSearchText;
			this.m_InputField.text = string.Empty;
		}
		public TMP_InputField m_InputField;
		public TextMeshProUGUI m_EnterPlaceholder;
		public TextMeshProUGUI m_Placeholder;
		public OwlcatButton m_InputButton;
		public bool m_IsInit;
		public StringReactiveProperty SearchRequest = new StringReactiveProperty();
		public OwlcatButton m_DropdownButton;
		public TMP_Dropdown Dropdown;
	}
}
