using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ManageLectures : MonoBehaviour {
	public TMP_InputField LectureInput;

	public TMP_Dropdown LectureDropdown;
	public TMP_Dropdown GroupDropdown;
	public TMP_Dropdown UserDropdown;

	public Mode mode;

	public enum Mode { Add, Delete, AssignLecturer, AssignGroup };

	public void OnEnable () {
		if (LectureDropdown != null) LectureDropdown.value = 0;
		if (GroupDropdown != null) GroupDropdown.value = 0;
		if (UserDropdown != null) UserDropdown.value = 0;
		if (LectureInput != null) LectureInput.text = "";

		UpdateValues();
	}

	public void UpdateValues () {
		switch (mode) {
			case Mode.Add:
				GroupDropdown.options.Clear();
				UserDropdown.options.Clear();

				GroupDropdown.options.Add(new TMP_Dropdown.OptionData("None"));
				UserDropdown.options.Add(new TMP_Dropdown.OptionData("None"));

				foreach (var group in MainScript.instance.Database.groups) {
					GroupDropdown.options.Add(new TMP_Dropdown.OptionData(group.GroupName));
				}

				foreach (var lecturer in MainScript.instance.Database.lecturers) {
					UserDropdown.options.Add(new TMP_Dropdown.OptionData(string.Format("{0} {1}", lecturer.FirstName, lecturer.LastName)));
				}

				GroupDropdown.RefreshShownValue();
				UserDropdown.RefreshShownValue();
				break;

			case Mode.Delete:
				LectureDropdown.options.Clear();

				LectureDropdown.options.Add(new TMP_Dropdown.OptionData("None"));

				foreach (var Lecture in MainScript.instance.Database.lectures) {
					LectureDropdown.options.Add(new TMP_Dropdown.OptionData(Lecture.LectureName));
				}

				LectureDropdown.RefreshShownValue();
				break;

			case Mode.AssignGroup:
				LectureDropdown.options.Clear();
				GroupDropdown.options.Clear();
				

				GroupDropdown.options.Add(new TMP_Dropdown.OptionData("None"));
				LectureDropdown.options.Add(new TMP_Dropdown.OptionData("None"));

				foreach (var lecture in MainScript.instance.Database.lectures) {
					LectureDropdown.options.Add(new TMP_Dropdown.OptionData(lecture.LectureName));
				}

				foreach (var group in MainScript.instance.Database.groups) {
					GroupDropdown.options.Add(new TMP_Dropdown.OptionData(group.GroupName));
				}

				LectureDropdown.RefreshShownValue();
				GroupDropdown.RefreshShownValue();
				break;

			case Mode.AssignLecturer:
				LectureDropdown.options.Clear();
				UserDropdown.options.Clear();


				LectureDropdown.options.Add(new TMP_Dropdown.OptionData("None"));
				UserDropdown.options.Add(new TMP_Dropdown.OptionData("None"));

				foreach (var lecture in MainScript.instance.Database.lectures) {
					LectureDropdown.options.Add(new TMP_Dropdown.OptionData(lecture.LectureName));
				}

				foreach (var lecturer in MainScript.instance.Database.lecturers) {
					UserDropdown.options.Add(new TMP_Dropdown.OptionData(string.Format("{0} {1}", lecturer.FirstName, lecturer.LastName)));
				}

				LectureDropdown.RefreshShownValue();
				UserDropdown.RefreshShownValue();
				break;
		}
	}

	public void Execute () {
		switch (mode) {
			case Mode.Add:
				MainScript.instance.AddLecture(LectureInput.text, GroupDropdown.value - 1, UserDropdown.value - 1);
				break;

			case Mode.Delete:
				if (LectureDropdown.value - 1 != -1) MainScript.instance.RemoveLecture(LectureDropdown.value - 1);
				else MainScript.instance.CreatePopup("Error", "Your selection is invalid, please try again", PopupScript.PopupType.Error);
				break;

			case Mode.AssignGroup:
				if (LectureDropdown.value - 1 != -1 && GroupDropdown.value != -1) MainScript.instance.AssignGroup(LectureDropdown.value - 1, GroupDropdown.value - 1);
				else MainScript.instance.CreatePopup("Error", "Your selection is invalid, please try again", PopupScript.PopupType.Error);
				break;

			case Mode.AssignLecturer:
				if (LectureDropdown.value - 1 != -1 && UserDropdown.value != -1) MainScript.instance.AssignLecturer(LectureDropdown.value - 1, UserDropdown.value - 1);
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
