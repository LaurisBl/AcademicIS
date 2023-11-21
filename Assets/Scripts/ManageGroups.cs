using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ManageGroups : MonoBehaviour {
	public TMP_InputField GroupInput;

	public TMP_Dropdown GroupDropdown;
	public TMP_Dropdown UserDropdown;

	public Mode mode;

	public enum Mode { Add, Delete, Assign };

	public void OnEnable () {
		if (GroupDropdown != null) GroupDropdown.value = 0;
		if (UserDropdown != null) UserDropdown.value = 0;
		if (GroupInput != null) GroupInput.text = "";

		UpdateValues();
	}

	public void UpdateValues () {
		switch (mode) {
			case Mode.Delete:
				GroupDropdown.options.Clear();

				GroupDropdown.options.Add(new TMP_Dropdown.OptionData("None"));

				foreach (var group in MainScript.instance.Database.groups) {
					GroupDropdown.options.Add(new TMP_Dropdown.OptionData(group.GroupName));
				}

				GroupDropdown.RefreshShownValue();
				break;

			case Mode.Assign:
				GroupDropdown.options.Clear();
				UserDropdown.options.Clear();

				GroupDropdown.options.Add(new TMP_Dropdown.OptionData("None"));
				UserDropdown.options.Add(new TMP_Dropdown.OptionData("None"));

				foreach (var group in MainScript.instance.Database.groups) {
					GroupDropdown.options.Add(new TMP_Dropdown.OptionData(group.GroupName));
				}

				foreach (var student in MainScript.instance.Database.students) {
					if (GroupDropdown.value - 1 != -1 && GroupDropdown.value - 1 == student.GroupID)
						continue;

					UserDropdown.options.Add(new TMP_Dropdown.OptionData(student.StudentCode + string.Format("({0} {1})", student.FirstName, student.LastName)));
				}

				GroupDropdown.RefreshShownValue();
				UserDropdown.RefreshShownValue();
				break;
		}
	}

	public void Execute () {
		switch (mode) {
			case Mode.Add:
				MainScript.instance.AddGroup(GroupInput.text);
				break;

			case Mode.Delete:
				if (GroupDropdown.value - 1 != -1) MainScript.instance.RemoveGroup(GroupDropdown.value - 1);
				else MainScript.instance.CreatePopup("Error", "Your selection is invalid, please try again", PopupScript.PopupType.Error);
				break;

			case Mode.Assign:
				var studentID = MainScript.instance.Database.FindStudentID(UserDropdown.options[UserDropdown.value].text.Substring(0, 7));
				if (studentID != -1) MainScript.instance.AssignStudent(GroupDropdown.value - 1, studentID);
				else MainScript.instance.CreatePopup("Error", "Your selection is invalid, please try again", PopupScript.PopupType.Error);
				break;
		}

		MainScript.instance.LoadMenu(1);
		MainScript.instance.Save();
	}

	public void Return () {
		MainScript.instance.LoadMenu(1);
	}
}
