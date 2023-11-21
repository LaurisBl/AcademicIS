using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManageGrades : MonoBehaviour {
	public TMP_Dropdown LectureDropdown;
	public TMP_Dropdown UserDropdown;
	public TMP_Dropdown GradeDropdown;
	public TMP_InputField GradeInput;

	public Transform ItemContainer;

	public Mode mode;

	public enum Mode { View, Add, Modify, Delete };

	public void OnEnable () {
		if(LectureDropdown != null) LectureDropdown.value = 0;
		if (UserDropdown != null) UserDropdown.value = 0;
		if (GradeDropdown != null) GradeDropdown.value = 0;
		if (GradeInput != null) GradeInput.text = "";

		UpdateValues();
	}

	public void UpdateValues () {
		switch (mode) {
			case Mode.View:
				LectureDropdown.options.Clear();

				LectureDropdown.options.Add(new TMP_Dropdown.OptionData("None"));

				foreach (var lecture in MainScript.instance.Database.lectures) {
					LectureDropdown.options.Add(new TMP_Dropdown.OptionData(lecture.LectureName));
				}

				LectureDropdown.RefreshShownValue();

				foreach(Transform child in ItemContainer) {
					Destroy(child.gameObject);
				}

				foreach(var grade in MainScript.instance.Database.grades) {
					if (LectureDropdown.value - 1 != -1 && LectureDropdown.value - 1 != grade.LectureID)
						continue;

					var lecturer = MainScript.instance.Database.lecturers[MainScript.instance.Database.lectures[grade.LectureID].LecturerID];

					var row = Instantiate(MainScript.instance.RowPrefab, ItemContainer);

					row.transform.GetChild(0).GetComponent<TMP_Text>().text = MainScript.instance.Database.students[grade.StudentID].StudentCode;
					row.transform.GetChild(1).GetComponent<TMP_Text>().text = MainScript.instance.Database.lectures[grade.LectureID].LectureName;
					row.transform.GetChild(2).GetComponent<TMP_Text>().text = lecturer.FirstName + " " + lecturer.LastName;
					row.transform.GetChild(3).GetComponent<TMP_Text>().text = "" + grade.Grade;
				}
				break;

			case Mode.Add:
				LectureDropdown.options.Clear();
				UserDropdown.options.Clear();

				LectureDropdown.options.Add(new TMP_Dropdown.OptionData("None"));
				UserDropdown.options.Add(new TMP_Dropdown.OptionData("None"));

				var lec = new LectureClass("", -1, -1);

				foreach (var lecture in MainScript.instance.Database.lectures) {
					LectureDropdown.options.Add(new TMP_Dropdown.OptionData(lecture.LectureName));

					if (LectureDropdown.options.Count > LectureDropdown.value && lecture.LectureName == LectureDropdown.options[LectureDropdown.value].text)
						lec = lecture;
				}

				foreach (var student in MainScript.instance.Database.students) {
					if (lec.GroupID != -1 && lec.GroupID != student.GroupID)
						continue;

					UserDropdown.options.Add(new TMP_Dropdown.OptionData(student.StudentCode + string.Format("({0} {1})", student.FirstName, student.LastName)));
				}

				LectureDropdown.RefreshShownValue();
				UserDropdown.RefreshShownValue();
				break;

			default:
				LectureDropdown.options.Clear();
				UserDropdown.options.Clear();
				GradeDropdown.options.Clear();

				LectureDropdown.options.Add(new TMP_Dropdown.OptionData("None"));
				UserDropdown.options.Add(new TMP_Dropdown.OptionData("None"));
				GradeDropdown.options.Add(new TMP_Dropdown.OptionData("None"));

				lec = new LectureClass("", -1, -1);

				foreach (var lecture in MainScript.instance.Database.lectures) {
					LectureDropdown.options.Add(new TMP_Dropdown.OptionData(lecture.LectureName));

					if (LectureDropdown.options.Count > LectureDropdown.value && lecture.LectureName == LectureDropdown.options[LectureDropdown.value].text)
						lec = lecture;
				}

				var stud = new StudentClass("", "", "");

				foreach (var student in MainScript.instance.Database.students) {
					UserDropdown.options.Add(new TMP_Dropdown.OptionData(student.StudentCode + string.Format("({0} {1})", student.FirstName, student.LastName)));

					if (UserDropdown.value - 1 != -1 && UserDropdown.options.Count > UserDropdown.value - 1 && student.StudentCode == UserDropdown.options[UserDropdown.value].text.Substring(0, 7))
						stud = student;

					if (lec.GroupID != -1 && lec.GroupID != student.GroupID)
						UserDropdown.options.RemoveAt(UserDropdown.options.Count - 1);
				}

				foreach(var grade in MainScript.instance.Database.grades) {
					if (UserDropdown.value - 1 != -1 && MainScript.instance.Database.FindStudentID(UserDropdown.options[UserDropdown.value].text.Substring(0, 7)) != grade.StudentID) 
						continue;

					GradeDropdown.options.Add(new TMP_Dropdown.OptionData("<size=0%>" + MainScript.instance.Database.grades.IndexOf(grade) + " </size>" + grade.Grade));
				}

				LectureDropdown.RefreshShownValue();
				UserDropdown.RefreshShownValue();
				GradeDropdown.RefreshShownValue();
				break;
		}
	}

	public void Execute () {
		switch (mode) {
			case Mode.Add:
				var studentID = MainScript.instance.Database.FindStudentID(UserDropdown.options[UserDropdown.value].text.Substring(0, 7));

				if(studentID != -1) MainScript.instance.AddGrade(LectureDropdown.value - 1, studentID, int.Parse(GradeInput.text));
				else MainScript.instance.CreatePopup("Error", "Your selection is invalid, please try again", PopupScript.PopupType.Error);
				break;

			case Mode.Delete:
				var gradeID = GradeDropdown.value - 1 != -1 ? int.Parse(GradeDropdown.options[GradeDropdown.value].text.Split(' ')[0].Replace("<size=0%>", "")) : -1;

				if (gradeID != -1) MainScript.instance.RemoveGrade(gradeID);
				else MainScript.instance.CreatePopup("Error", "Your selection is invalid, please try again", PopupScript.PopupType.Error);
				break;

			case Mode.Modify:
				gradeID = GradeDropdown.value - 1 != -1 ? int.Parse(GradeDropdown.options[GradeDropdown.value].text.Split(' ')[0].Replace("<size=0%>", "")) : -1;

				if (gradeID != -1) MainScript.instance.ModifyGrade(gradeID, int.Parse(GradeInput.text));
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
