using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ManageUsers : MonoBehaviour {
	public TMP_InputField FirstNameInput;
	public TMP_InputField LastNameInput;

	public TMP_Dropdown GroupDropdown;
	public TMP_Dropdown UserDropdown;

	public Mode mode;
	public User user;

	public enum Mode { Add, Delete };
	public enum User { Student, Lecturer };

	public void OnEnable () {
		if (GroupDropdown != null) GroupDropdown.value = 0;
		if (UserDropdown != null) UserDropdown.value = 0;
		if (FirstNameInput != null) FirstNameInput.text = "";
		if (LastNameInput != null) LastNameInput.text = "";

		UpdateValues();
	}

	public void UpdateValues () {
		switch (mode) {
			case Mode.Add:
				GroupDropdown.options.Clear();

				GroupDropdown.options.Add(new TMP_Dropdown.OptionData("None"));

				foreach (var group in MainScript.instance.Database.groups) {
					GroupDropdown.options.Add(new TMP_Dropdown.OptionData(group.GroupName));
				}

				GroupDropdown.RefreshShownValue();
				break;

			case Mode.Delete:
				switch (user) {
					case User.Student:
						GroupDropdown.options.Clear();
						UserDropdown.options.Clear();

						GroupDropdown.options.Add(new TMP_Dropdown.OptionData("None"));
						UserDropdown.options.Add(new TMP_Dropdown.OptionData("None"));

						foreach (var group in MainScript.instance.Database.groups) {
							GroupDropdown.options.Add(new TMP_Dropdown.OptionData(group.GroupName));
						}

						foreach (var student in MainScript.instance.Database.students) {
							if (GroupDropdown.value - 1 != -1 && GroupDropdown.value - 1 != student.GroupID)
								continue;

							UserDropdown.options.Add(new TMP_Dropdown.OptionData(student.StudentCode + string.Format("({0} {1})", student.FirstName, student.LastName)));
						}

						GroupDropdown.RefreshShownValue();
						UserDropdown.RefreshShownValue();
						break;

					case User.Lecturer:
						UserDropdown.options.Clear();

						foreach (var lecturer in MainScript.instance.Database.lecturers) {
							UserDropdown.options.Add(new TMP_Dropdown.OptionData(string.Format("{0} {1}", lecturer.FirstName, lecturer.LastName)));
						}

						UserDropdown.RefreshShownValue();
						break;
				}
				break;
		}
	}

	public void Execute () {
		switch (mode) {
			case Mode.Add:
				switch (user) {
					case User.Student:
						MainScript.instance.AddStudent(FirstNameInput.text, LastNameInput.text, GroupDropdown.value - 1);
						break;

					case User.Lecturer:
						MainScript.instance.AddLecturer(FirstNameInput.text, LastNameInput.text);
						break;
				}
				break;

			case Mode.Delete:
				switch (user) {
					case User.Student:
						var studentID = MainScript.instance.Database.FindStudentID(UserDropdown.options[UserDropdown.value].text.Substring(0, 7));
						if (studentID != -1) MainScript.instance.RemoveStudent(studentID);
						else MainScript.instance.CreatePopup("Error", "Your selection is invalid, please try again", PopupScript.PopupType.Error);
						break;

					case User.Lecturer:
						if (UserDropdown.value - 1 != -1) MainScript.instance.RemoveLecturer(UserDropdown.value - 1);
						else MainScript.instance.CreatePopup("Error", "Your selection is invalid, please try again", PopupScript.PopupType.Error);
						break;
				}
				break;
		}

		MainScript.instance.LoadMenu(1);
		MainScript.instance.Save();
	}

	public void Return () {
		MainScript.instance.LoadMenu(1);
	}
}
